using Microsoft.Web.WebView2.Core;

using pcap_7day;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Media;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Radar7d2d
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [ClassInterface(ClassInterfaceType.AutoDual)]
        [ComVisible(true)]
        public class SoundWarning
        { 
            public Action ActionDanger;
            public Action ActionInit;

            public void Danger()
            {
                ActionDanger.Invoke();
            }

            public void Init()
            {
                ActionInit.Invoke();
            }
        }

        private struct MapRender
        {
            public Bitmap bitmap;
            
            public int minX;
            public int maxX;

            public int minZ;
            public int maxZ;

            public ConcurrentDictionary<int, byte[]> tiles;
        }

        private class UserItem
        {
            public string UserName { get; set; }
            public int UserId { get; set; }
        }

        public WorldInfo World { get; private set; }
        public int Device { get; private set; }

        private MapRender _map_render;
        private Size _last_size;
        private FormBorderStyle _last_border;
        private Point _last_location;

        private BackgroundWorker _map_runtime;
        private BackgroundWorker _map_worker;
        private BackgroundWorker _pcap_worker;

        private PcapProgram _pcap_program;
        private SoundWarning _callback_class;

        public Form1(int device, WorldInfo worldInfo)
        {
            World = worldInfo;
            Device = device;

            InitializeComponent();
            InitializeForm();
        }

        private void InitializeForm()
        {
            _map_render = new MapRender 
            { 
                bitmap  = new Bitmap(World.size_x, World.size_z, PixelFormat.Format16bppRgb555),
                minX    = -World.size_x / 32 + 1,
                maxX    = World.size_x / 32 - 2,
                minZ    = -World.size_z / 32 + 1,
                maxZ    = World.size_z / 32 - 2,
                tiles   = new ConcurrentDictionary<int, byte[]>()
            };
            Graphics.FromImage(_map_render.bitmap).Clear(Color.DarkGray);

            _last_size = new Size(640, 480);
            _last_border = FormBorderStyle;
            _last_location = new Point(
                Screen.PrimaryScreen.Bounds.Width / 2 - _last_size.Width / 2,
                Screen.PrimaryScreen.Bounds.Height / 2 - _last_size.Height / 2
            );

            var curDir = Directory.GetCurrentDirectory();
            var _player = new SoundPlayer($"file:///{curDir}/html/images/_tt_overlap.wav");

            playersList.DisplayMember = "UserName";
            playersList.ValueMember = "UserId";

            var _uri = new Uri($"file:///{curDir}/html/index.html");

            webView21.Source = _uri;
            webView21.ContentLoading += OnViewReady;

            // player
            _callback_class = new SoundWarning
            {
                ActionDanger = () =>
                {
                    _player.PlaySync();
                },

                ActionInit = () =>
                {
                    webView21.ExecuteScriptAsync("<style>#container{width:" + World.size_x + "px;height:"+ World.size_z + "px;}</style>");
                    webView21.ExecuteScriptAsync($"SetMapSize({World.size_x}, {World.size_z});");
                    if(World.traders != null)
                    {
                        webView21.ExecuteScriptAsync($"SetMapTraders('{World.traders}');");
                    }

                    this.Activated += OnFormActivated;
                    this.Deactivate += OnFormDeactivate;

                    _pcap_worker = new BackgroundWorker();
                    _pcap_worker.DoWork += new DoWorkEventHandler(ListenForPcap);
                    _pcap_worker.ProgressChanged += new ProgressChangedEventHandler(ProgressPcap);
                    _pcap_worker.WorkerReportsProgress = true;
                    _pcap_worker.RunWorkerAsync();

                    _map_worker = new BackgroundWorker();
                    _map_worker.DoWork += new DoWorkEventHandler(ListenForMap);
                    _map_worker.ProgressChanged += new ProgressChangedEventHandler(ProgressMap);
                    _map_worker.WorkerReportsProgress = true;
                    _map_worker.RunWorkerAsync();

                    _map_runtime = new BackgroundWorker();
                    _map_runtime.DoWork += new DoWorkEventHandler(ListenRuntimeMap);
                    _map_runtime.ProgressChanged += new ProgressChangedEventHandler(ProgressRuntimeMap);
                    _map_runtime.WorkerReportsProgress = true;
                    _map_runtime.RunWorkerAsync();
                }
            };
        }

        private void ProgressRuntimeMap(object sender, ProgressChangedEventArgs e)
        {
            var _objs = (object[])e.UserState;
            webView21.ExecuteScriptAsync($"SetMapRender('{(string)_objs[0]}');");
        }

        private void ListenRuntimeMap(object sender, DoWorkEventArgs e)
        {
            var _worker = (BackgroundWorker)sender;
            while (true)
            {
                Thread.Sleep(2000);
                lock (_map_render.tiles)
                {
                    if (_map_render.tiles.Count > 0)
                    {
                        var _timer = DateTime.Now;
                        var bmpData = _map_render.bitmap.LockBits(new Rectangle(0, 0, World.size_x, World.size_z), ImageLockMode.ReadWrite, _map_render.bitmap.PixelFormat);
                        IntPtr basePtr = bmpData.Scan0;
                        IntPtr ptr = basePtr;

                        foreach (int tile in _map_render.tiles.Keys)
                        {
                            int x = (Int16)(tile & 0xFFFF);
                            int z = (Int16)((tile >> 16));

                            int offset = ((x - _map_render.minX + 1) + (_map_render.maxZ - z + 1) * World.size_x) * 16;
                            ptr = basePtr + offset * 2;
                            for (int j = 0; j < 16; j++)
                            {
                                Marshal.Copy(_map_render.tiles[tile], 480 - j * 32, ptr, 32);
                                ptr += World.size_x * 2;
                            }
                        }

                        _map_render.bitmap.UnlockBits(bmpData);
                        _map_render.tiles.Clear();

                        using (var stream = new MemoryStream())
                        {
                            _map_render.bitmap.Save(stream, ImageFormat.Jpeg);
                            _worker.ReportProgress(1, new object[] { Convert.ToBase64String(stream.GetBuffer()) });
                            Console.WriteLine($"SetMapRender >> time:{(DateTime.Now - _timer).TotalMilliseconds}");
                        }
                    }
                }
            }
        }

        private void ListenForMap(object sender, DoWorkEventArgs e)
        {
            var _time = DateTime.Now;
            var _worker = (BackgroundWorker)sender;
            var rect = new Rectangle(0, 0, World.size_x, World.size_z);

            var _players_list = Directory.GetFiles(Path.Combine(World.path, "Player"));
            if (_players_list.Length > 0)
            {
                using (var bitmap_stream = new MemoryStream())
                {
                    var bmpData = _map_render.bitmap.LockBits(rect, ImageLockMode.ReadWrite, _map_render.bitmap.PixelFormat);
                    IntPtr basePtr = bmpData.Scan0;
                    IntPtr ptr = basePtr;
                    using (var stream = new FileStream(_players_list[0], FileMode.Open))
                    {
                        using (var binary = new BinaryReader(stream))
                        {
                            binary.ReadBytes(4);
                            var version = binary.ReadUInt32();
                            int maxCountOfTiles = binary.ReadInt32(); // 524288

                            var tiles_pos = maxCountOfTiles * 4 + 16;

                            var num = binary.ReadUInt32();
                            List<UInt32> tiles_index = new List<UInt32>((int)num);
                            for (int i = 0; i < num; i++)
                            {
                                tiles_index.Add(binary.ReadUInt32());
                            }
                            stream.Seek(tiles_pos, SeekOrigin.Begin);

                            for (int i = 0; i < num; i++)
                            {
                                var _tile = tiles_index[i];
                                var _tile_data = binary.ReadBytes(512);

                                int x = (Int16)(_tile & 0xFFFF);
                                int z = (Int16)((_tile >> 16));

                                if (x > _map_render.maxX + 1 || x < _map_render.minX - 1)
                                    continue;
                                if (z > _map_render.maxZ + 1 || z < _map_render.minZ - 1)
                                    continue;

                                int offset = ((x - _map_render.minX + 1) + (_map_render.maxZ - z + 1) * World.size_x) * 16;
                                ptr = basePtr + offset * 2;
                                for (int j = 0; j < 16; j++)
                                {
                                    Marshal.Copy(_tile_data, 480 - j * 32, ptr, 32);
                                    ptr += World.size_x * 2;
                                }
                            }
                        }
                    }

                    _map_render.bitmap.UnlockBits(bmpData);

                    _map_render.bitmap.Save(bitmap_stream, ImageFormat.Jpeg);

                    _worker.ReportProgress(1, new object[] { Convert.ToBase64String(bitmap_stream.GetBuffer()) });
                }   
            }

            Console.WriteLine("MapRender: " + (DateTime.Now - _time).TotalMilliseconds);
        }

        private void ProgressMap(object sender, ProgressChangedEventArgs e)
        {
            var _objs = (object[])e.UserState;
            switch (e.ProgressPercentage)
            {
                case 1:
                    webView21.ExecuteScriptAsync($"SetMapRender('{(string)_objs[0]}');");
                    break;
            }
        }

        private void OnFormDeactivate(object sender, EventArgs e)
        {
            right_panel.Visible = false;
            
            _last_size = Size;
            Size = new Size(250, 210);

            _last_border = FormBorderStyle;
            FormBorderStyle = FormBorderStyle.None;

            _last_location = Location;
            Location = new Point(0, 0);

            TopMost = true;

            try
            {
                Opacity = 0.9f;
            } catch(Exception err)
            {
                Console.WriteLine(err);
            }  

            webView21.ExecuteScriptAsync("SetWindowCompact(true);");            

            int initialStyle = GetWindowLong(this.Handle, -20);
            SetWindowLong(this.Handle, -20, initialStyle | 0x80000 | 0x20);            
        }

        private void OnFormActivated(object sender, EventArgs e)
        {
            right_panel.Visible = true;
            FormBorderStyle = _last_border;
            Size = _last_size;
            Location = _last_location;
            TopMost = false;
            webView21.ExecuteScriptAsync("SetWindowCompact(false);");
            Opacity = 1f;
        }

        private void OnViewReady(object sender, CoreWebView2ContentLoadingEventArgs e)
        {
            webView21.CoreWebView2.AddHostObjectToScript("_callback", _callback_class);
        }

        private void ListenForPcap(object sender, DoWorkEventArgs e)
        {
            try
            {
                _pcap_program = new PcapProgram(this, (BackgroundWorker)sender);
            }
            catch (Exception err)
            {
                Console.WriteLine("Error: " + err);
            }
        }

        private void ProgressPcap(object sender, ProgressChangedEventArgs e)
        {
            var _objs = (object[])e.UserState;
            switch((ProgressIndex)e.ProgressPercentage)
            {
                case ProgressIndex.EntityParse:
                    webView21.ExecuteScriptAsync($"ParseEntities('{(string)_objs[0]}');");
                    break;

                case ProgressIndex.EntityAdd:
                    {
                        var _user_id = (int)_objs[0];
                        for (int i = 0; i < playersList.Items.Count; ++i)
                        {
                            var _item = (UserItem)playersList.Items[i];
                            if (_item.UserId == _user_id)
                            {
                                break;
                            }
                        }
                        var _index = playersList.Items.Add(new UserItem { 
                            UserId = _user_id, 
                            UserName = _user_id.ToString() 
                        });
                    }
                    break;

                case ProgressIndex.EntityName:
                    {
                        var _user_id = (int)_objs[0];
                        for(int i = 0; i < playersList.Items.Count; ++i)
                        {
                            var _item = (UserItem)playersList.Items[i];
                            if(_item.UserId == _user_id)
                            {
                                _item.UserName = (string)_objs[1];
                                playersList.Items[i] = _item;
                                break;
                            }
                        }
                    }
                    break;


                case ProgressIndex.EntityRemove:
                    {
                        var _user_id = (int)_objs[0];
                        for (int i = 0; i < playersList.Items.Count; ++i)
                        {
                            var _item = (UserItem)playersList.Items[i];
                            if (_item.UserId == _user_id)
                            {
                                playersList.Items.RemoveAt(i);
                                break;
                            }
                        }
                    }
                    break;

                case ProgressIndex.TileMap:
                    lock (_map_render.tiles)
                    {
                        Console.WriteLine($"ProgressIndex.TileMap");
                        var _tiles = (List<int>)_objs[0];
                        var _tiles_data = (List<byte[]>)_objs[1];

                        for(int i = 0; i < _tiles.Count; ++i)
                        {
                            _map_render.tiles.TryAdd(_tiles[i], _tiles_data[i]);
                        }
                        //Console.WriteLine("ProgressIndex.TileMap");
                    }
                    break;

                case ProgressIndex.EntityPlayer:
                    webView21.ExecuteScriptAsync($"SetPlayer({(int)_objs[0]});");
                    break;

                case ProgressIndex.Error:
                    webView21.ExecuteScriptAsync($"alert('ERROR');");
                    break;

            }
        }

        private void playersList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if(playersList.SelectedIndex != -1)
            {
                var _item = (UserItem)playersList.Items[playersList.SelectedIndex];
                webView21.ExecuteScriptAsync($"SetEntityFocus('{_item.UserId}');");
                webView21.Focus();
            }
        }

    }
}
