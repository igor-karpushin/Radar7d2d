using pcap_7day;
using PcapDotNet.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace Radar7d2d
{
    public struct WorldInfo
    {
        public string path;
        public string server;
        public int size_x;
        public int size_z;
        public DateTime time;
        public string traders;
    }

    public struct PoiStruct
    {
        public byte type;
        public string name;
        public Vector2i position;

        public void Write(BinaryWriter binary)
        {
            binary.Write(type);
            binary.Write(position.x);
            binary.Write(position.y);
            binary.Write(name);
        }
    }

    static class Program
    {

        [STAThread]
        static void Main(string[] args)
        {
            // NETWORK ADAPTERS
            Console.WriteLine("Network Devices:");
            var allDevices = LivePacketDevice.AllLocalMachine;
            for (int i = 0; i < allDevices.Count; ++i)
            {
                Console.WriteLine($"{i}) {allDevices[i].Description}");
            }
            Console.WriteLine("");

            var _device_novalid = true;
            int _device_index = -1;
            if(args.Length > 0)
            {
                if (int.TryParse(args[0], out _device_index))
                {
                    if (_device_index < allDevices.Count)
                    {
                        _device_novalid = false;
                    }
                }
            }

            while(_device_novalid)
            {
                Console.Write("Select: ");

                if (int.TryParse(Console.ReadLine(), out _device_index))
                {
                    if (_device_index < allDevices.Count)
                    {
                        break;
                    }
                }
                Console.WriteLine("Error");
            }
            Console.WriteLine($"Select Device: {allDevices[_device_index].Description}\n");
            Console.WriteLine("");

            // WORLDS
            Console.WriteLine("Saved Worlds:");

            var _worlds = new List<WorldInfo>();

            // parse
            var _roaming = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var _directory = Path.Combine(_roaming, @"7DaysToDie\SavesLocal");
            string[] _entries = Directory.GetDirectories(_directory);

            for(int i = 0; i < _entries.Length; ++i)
            {
                var _world_entry = _entries[i];
                var _world_guid = Path.GetFileName(_world_entry);
                var _host_file = Path.Combine(_world_entry, "hosts.txt");

                var _world_struct = new WorldInfo 
                { 
                    path    = _world_entry,
                    time    = File.GetLastAccessTime(_host_file),
                    server  = File.ReadAllText(_host_file).Split(':')[0],
                    size_x  = 0,
                    size_z  = 0,
                    traders = null
                };

                //world size
                var _world_info_path = Path.Combine(_world_entry, @"World\map_info.xml");
                if(File.Exists(_world_info_path))
                {
                    var _world_info = new System.Xml.XmlDocument();
                    _world_info.Load(_world_info_path);

                    // worlds size
                    var heightmapNode = _world_info.SelectSingleNode("/MapInfo/property[@name='HeightMapSize']");
                    var valueAttribute = heightmapNode.Attributes["value"]?.Value;
                    if (valueAttribute != null)
                    {
                        string[] vs = valueAttribute.Split(',');
                        if (vs.Length >= 2)
                        {
                            int.TryParse(vs[0], out _world_struct.size_x);
                            int.TryParse(vs[1], out _world_struct.size_z);
                        }
                    }
                }

                // world poi
                var _poi_info_path = Path.Combine(_world_entry, @"World\prefabs.xml");
                if (File.Exists(_poi_info_path))
                {
                    var _poi_list = new List<PoiStruct>();
                    var _poi_info = new XmlDocument();
                    _poi_info.Load(_poi_info_path);
                    using (var binary = new BinaryWriter(new MemoryStream()))
                    {
                        foreach (XmlNode poi in _poi_info.SelectNodes("/prefabs/decoration"))
                        {
                            var _poi_name = poi.Attributes["name"]?.Value;
                            if (_poi_name != null)
                            {
                                // trader
                                if (_poi_name.Contains("trader_"))
                                {
                                    if(GetPoiStruct(poi, out PoiStruct element))
                                    {
                                        element.type = 1;
                                        _poi_list.Add(element);
                                    }
                                }
                                // books

                                if (_poi_name == "church_01" || _poi_name == "oldwest_business_12" || _poi_name == "skyscraper_02")
                                {
                                    if (GetPoiStruct(poi, out PoiStruct element))
                                    {
                                        element.type = 2;
                                        _poi_list.Add(element);
                                    }
                                }

                                if (_poi_name == "store_gun_01" || _poi_name == "store_gun_01")
                                {
                                    if (GetPoiStruct(poi, out PoiStruct element))
                                    {
                                        element.type = 3;
                                        _poi_list.Add(element);
                                    }
                                }

                            }
                        }

                        binary.Write((byte)_poi_list.Count);
                        for(int k = 0; k < _poi_list.Count; ++k)
                        {
                            _poi_list[k].Write(binary);
                        }

                        _world_struct.traders = Convert.ToBase64String(((MemoryStream)binary.BaseStream).GetBuffer());
                    }                        
                }
                _worlds.Add(_world_struct);
            }

            _worlds.Sort((WorldInfo a, WorldInfo b) => 
            {
                return b.time.CompareTo(a.time);
            });

            var _world_novalid = true;
            int _world_index = -1;

            int _min_count = Math.Min(10, _worlds.Count);
            for (int i = 0; i < _min_count; i++)
            {
                var _world = _worlds[i];
                Console.WriteLine($"{i}) {_worlds[i].server} {_world.size_x}x{_world.size_z}");
                if (args.Length > 1)
                {
                    if(args[1] == _worlds[i].server)
                    {
                        _world_novalid = false;
                        _world_index = i;

                    }
                }
            }
            Console.WriteLine("");

            while (_world_novalid)
            {
                Console.Write("Select: ");

                if (int.TryParse(Console.ReadLine(), out _world_index))
                {
                    if (_world_index < _min_count)
                    {
                        break;
                    }
                }
                Console.WriteLine("Error");
            }

            var _world_entity = _worlds[_world_index];

            Console.WriteLine($"Select World: {_world_entity.server}\n");

            
            if (_world_entity.size_x == 0)
            {
                while (true)
                {
                    Console.WriteLine("Write World Size (4096x4096): ");
                    Console.Write("Size: ");

                    var _split = Console.ReadLine().Split('x');
                    if(_split.Length == 2)
                    {
                        if (int.TryParse(_split[0], out _world_entity.size_x))
                        {
                            if(_world_entity.size_x > 0)
                            {
                                if (int.TryParse(_split[1], out _world_entity.size_z))
                                {
                                    if (_world_entity.size_z > 0)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    Console.WriteLine("Error");
                }
            }

            // form start
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1(_device_index, _world_entity));
        }

        private static bool GetPoiStruct(XmlNode poi, out PoiStruct element)
        {
            element = default;
            var _pos = poi.Attributes["position"]?.Value.Split(',');
            if (_pos.Length == 3)
            {
                int.TryParse(_pos[0], out int _map_x);
                int.TryParse(_pos[2], out int _map_z);
                //Console.WriteLine($"POI:TRADER {_map_x}/{_map_z}");
                element = new PoiStruct
                {
                    name = poi.Attributes["name"]?.Value,
                    position = new Vector2i(_map_x, _map_z)
                };
                return true;
            }

            return false;
        }
    }
}
