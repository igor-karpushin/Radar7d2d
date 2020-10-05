
using Radar7d2d;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Numerics;
using System.Timers;

namespace pcap_7day
{
    public class Network7d2d
    {

        internal ConcurrentDictionary<int, EntityInfo> Entities;
        internal BackgroundWorker Worker { get; private set; }
        private readonly Form1 BaseForm;
        //public int PlayerIndex { get { return BaseForm.Config.user; } }

        internal int Position { get; private set; }
        internal WorldInfo World { get { return BaseForm.World; } }

        private Dictionary<byte, INetPackage> _packages;
        private readonly Timer timer;

        public Dictionary<int, SpawnTypeIndex> SpawnTypes;

        public int Player;

        public Network7d2d(BackgroundWorker worker, Form1 form)
        {
            Worker = worker;
            BaseForm = form;

            Entities = new ConcurrentDictionary<int, EntityInfo>();

            // export to file ?
            SpawnTypes = new Dictionary<int, SpawnTypeIndex>
            {

                { -1767388301,  SpawnTypeIndex.Enemies },
                { -1142032673,  SpawnTypeIndex.Enemies },
                { 1598044477,   SpawnTypeIndex.Enemies },
                { 948863590,    SpawnTypeIndex.Enemies },
                { -272178566,   SpawnTypeIndex.Enemies },
                { -1286182028,  SpawnTypeIndex.Enemies },
                { -1767100012,  SpawnTypeIndex.Enemies },
                { -1825467729,  SpawnTypeIndex.Enemies },
                { 501574227,    SpawnTypeIndex.Enemies },
                { 920796401,    SpawnTypeIndex.Enemies },
                { 948863326,    SpawnTypeIndex.Enemies },
                { -272466062,   SpawnTypeIndex.Enemies },
                { -1599247748,  SpawnTypeIndex.Enemies },
                { 1314044779,   SpawnTypeIndex.Enemies },
                { -487604489,   SpawnTypeIndex.Enemies },
                { 1420033913,   SpawnTypeIndex.Enemies },
                { 948863421,    SpawnTypeIndex.Enemies },
                { -272358119,   SpawnTypeIndex.Enemies },
                { -1481984997,  SpawnTypeIndex.Enemies },
                { -1440238285,  SpawnTypeIndex.Enemies },
                { 633590055,    SpawnTypeIndex.Enemies },
                { -32115571,    SpawnTypeIndex.Enemies },
                { -570166383,   SpawnTypeIndex.Enemies },
                { 1347248853,   SpawnTypeIndex.Enemies },
                { -622664701,   SpawnTypeIndex.Enemies },
                { 1719843072,   SpawnTypeIndex.Enemies },
                { -1646138558,  SpawnTypeIndex.Enemies },
                { 1536062518,   SpawnTypeIndex.Enemies },
                { 2063434536,   SpawnTypeIndex.Enemies },
                { -1031427725,  SpawnTypeIndex.Enemies },
                { 226314231,    SpawnTypeIndex.Enemies },
                { 1626397409,   SpawnTypeIndex.Enemies },
                { -1031681462,  SpawnTypeIndex.Enemies },
                { -50001010,    SpawnTypeIndex.Enemies },
                { 1366828168,   SpawnTypeIndex.Enemies },
                { -1461252340,  SpawnTypeIndex.Enemies },
                { -544249000,   SpawnTypeIndex.Enemies },
                { -319198542,   SpawnTypeIndex.Enemies },
                { 1698308179,   SpawnTypeIndex.Enemies },
                { 1064404415,   SpawnTypeIndex.Enemies },
                { 2084698241,   SpawnTypeIndex.Enemies },
                { 813805287,    SpawnTypeIndex.Enemies },
                { 633811747,    SpawnTypeIndex.Enemies },
                { -1368567374,  SpawnTypeIndex.Enemies },
                { -374604218,   SpawnTypeIndex.Enemies },
                { -357242459,   SpawnTypeIndex.Enemies },
                { -1406822389,  SpawnTypeIndex.Enemies },
                { 494147535,    SpawnTypeIndex.Enemies },
                { 244206587,    SpawnTypeIndex.Enemies },
                { 2124421703,   SpawnTypeIndex.Enemies },
                { 1093758933,   SpawnTypeIndex.Enemies },
                { -1648911827,  SpawnTypeIndex.Enemies },
                { -1695970375,  SpawnTypeIndex.Enemies },
                { -399971291,   SpawnTypeIndex.Enemies },
                { 1516398389,   SpawnTypeIndex.Enemies },
                { 356599161,    SpawnTypeIndex.Enemies },
                { 714441665,    SpawnTypeIndex.Enemies },
                { -1097674075,  SpawnTypeIndex.Enemies },
                { -463141193,   SpawnTypeIndex.Enemies },
                { 180067742,    SpawnTypeIndex.Enemies },
                { 571916326,    SpawnTypeIndex.Enemies },
                { 464319586,    SpawnTypeIndex.Enemies },
                { -1844142961,  SpawnTypeIndex.Enemies },
                { 1922889038,   SpawnTypeIndex.Enemies },
                { -1854072766,  SpawnTypeIndex.Enemies },
                { -55595620,    SpawnTypeIndex.Enemies },
                { 2022221298,   SpawnTypeIndex.Enemies },
                { 2135613367,   SpawnTypeIndex.Enemies },
                { 1698531003,   SpawnTypeIndex.Enemies },
                { 794653185,    SpawnTypeIndex.Enemies },
                { 1349726372,   SpawnTypeIndex.Enemies },
                { -1091569624,  SpawnTypeIndex.Enemies },
                { 8628422,      SpawnTypeIndex.Enemies },
                { 1949862286,   SpawnTypeIndex.Enemies },
                { -20073622,    SpawnTypeIndex.Enemies },
                { 1191652936,   SpawnTypeIndex.Enemies },
                { -1743278235,  SpawnTypeIndex.Enemies },
                { -355316511,   SpawnTypeIndex.Enemies },
                { 89759735,     SpawnTypeIndex.Enemies },
                { -757275287,   SpawnTypeIndex.Enemies },
                { 2144621925,   SpawnTypeIndex.Enemies },
                { -509955191,   SpawnTypeIndex.Enemies },
                { 2112151141,   SpawnTypeIndex.Enemies },
                { -999264730,   SpawnTypeIndex.Enemies },

                { 1494923703,   SpawnTypeIndex.Animals },
                { 831878011,    SpawnTypeIndex.Animals },
                { -851512378,   SpawnTypeIndex.Animals },
                { 288356134,    SpawnTypeIndex.Animals },
                { 275096887,    SpawnTypeIndex.Animals },
                { -1647747018,  SpawnTypeIndex.Animals },
                { 1255106688,   SpawnTypeIndex.Animals },
                { 675036890,    SpawnTypeIndex.Animals },
                { 1960661222,   SpawnTypeIndex.Animals },
                { 1930842125,   SpawnTypeIndex.Animals },
                { 1055228428,   SpawnTypeIndex.Animals },
                { -456486343,   SpawnTypeIndex.Animals },
                { -1520029843,  SpawnTypeIndex.Animals },
                { -1109310417,  SpawnTypeIndex.Animals },
                { 1162937010,   SpawnTypeIndex.Animals },                

                { -1077398686,  SpawnTypeIndex.Animals },
                { -1574592863,  SpawnTypeIndex.Animals },

                { 475216491,    SpawnTypeIndex.Cars },
                { -859988484,   SpawnTypeIndex.Cars },
                { 1586892159,   SpawnTypeIndex.Cars },
                { -719262733,   SpawnTypeIndex.Cars },
                { 1003063392,   SpawnTypeIndex.Cars },

                { -66194395,    SpawnTypeIndex.SupplyCrates },
                { 669818205,    SpawnTypeIndex.SupplyCrates },

                { 84004336,     SpawnTypeIndex.Backpack },
                { -2021142581,  SpawnTypeIndex.Backpack },
                { 12718638,     SpawnTypeIndex.Backpack },
                { -699022105,   SpawnTypeIndex.Backpack },
                { 1737971533,   SpawnTypeIndex.Backpack },
                { -1001734244,  SpawnTypeIndex.Backpack },

                { 1883580775,   SpawnTypeIndex.Animals },
                { -1501719851,  SpawnTypeIndex.Animals },

                { -389248820,   SpawnTypeIndex.NPC },
                { -479122404,   SpawnTypeIndex.NPC },
                { -866611929,   SpawnTypeIndex.NPC },

                { -1324121743,  SpawnTypeIndex.Trader },
                { 843888929,    SpawnTypeIndex.Trader },
                { 454159241,    SpawnTypeIndex.Trader },
                { -1238134116,  SpawnTypeIndex.Trader },
                { 803483555,    SpawnTypeIndex.Trader },
                { -1721972210,  SpawnTypeIndex.Trader },
                { -245005049,   SpawnTypeIndex.Trader }
            };

            _packages = new Dictionary<byte, INetPackage>
            {
                { 1, default(NetPackageChunk) },
                { 2, default(NetPackageEntityPosAndRot) },
                { 3, default(NetPackagePlayerStats) },
                { 6, default(NetPackageChunkRemove) },
                { 7, default(NetPackageEntityRemove) },
                { 8, default(NetPackageEntitySpawn) },
                { 10, default(NetPackageAudio) },
                { 11, default(NetPackageDamageEntity) },
                { 13, default(NetPackageSetBlock) },
                { 14, default(NetPackageEntityRelPosAndRot) },
                { 16, default(NetPackageEntityRotation) },
                { 17, default(NetPackageEntityTeleport) },
                { 20, default(NetPackagePlayerData) },
                { 21, default(NetPackageEntityLookAt) },
                { 26, default(NetPackageClientInfo) },
                { 28, default(NetPackageTileEntity) },
                { 30, default(NetPackageTileEntity) },  // NetPackagePlayerInventoryForAI
                { 35, default(NetPackageAudio) },       // NetPackageEntityStatsBuff       
                { 36, default(NetPackageAudio) },       // NetPackageAddRemoveBuff
                { 38, default(NetPackageEntityStatChanged) },
                { 40, default(NetPackagePersistentPlayerState) },
                { 41, default(NetPackageTELock) },
                { 42, default(NetPackageItemActionEffects) },
                { 44, default(NetPackageEntitySpeeds) },
                { 45, default(NetPackageMapChunks) },   
                { 47, default(NetPackageAudio) },       // NetPackageParticleEffect
                { 50, default(NetPackageAudio) },       // NetPackageItemReload
                { 56, default(NetPackageAudio) },       // NetPackageWorldTime
                { 57, default(NetPackageGameMessage) },
                { 69, default(NetPackageAudio) },       // NetPackageWeather
                { 72, default(NetPackageAudio) },       // NetPackageVehicleDataSync
                { 73, default(NetPackageEntityAttach) },
                { 84, default(NetPackageAudio) },       // NetPackagePartyActions
                { 85, default(NetPackageAudio) },       // NetPackagePartyData
                { 90, default(NetPackageAudio) },       // NetPackageEntityMapMarkerRemove
                { 94, default(NetPackageAudio) },       // ??
                { 95, default(NetPackageAudio) },       // NetPackageSetBlockTexture
                { 96, default(NetPackageAudio) },       // NetPackageWireToolActions
                { 100, default(NetPackagePlayerSpawnedInWorld) },
            };
            
            timer = new Timer
            {
                Interval = 500
            };
            timer.Elapsed += OnTimerEvent;
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        internal void PlayerRotation(int user_id)
        {
            /*BaseForm.ChangeAll(Entities, user_id);

            var _player = Entities[user_id];
            var MvpMatrix = ProjectionMatrix * ExtensionsMatrix4x4.Init(_player.Position, _player.Rotation);

            foreach(var _user_id in Entities.Keys)
            {
                if(_user_id != user_id)
                {
                    var _user = Entities[_user_id];
                    //if(_user_id == 182051)
                    {
                        var MvpPosition = MvpMatrix.TransformVector(new Vector4(
                            _user.Position.X, 
                            _user.Position.Y, 
                            _user.Position.Z, 
                            1f
                        ));
                        var Postion = MvpPosition / -MvpPosition.W;

                        var _delta_x = ScreenSize.x * 0.5f + Postion.X * ScreenSize.x * 0.5f;
                        var _delta_y = ScreenSize.y * 0.5f - Postion.Y * ScreenSize.y * 0.5f;

                        Console.WriteLine($"pos >> {_delta_x} >> {_delta_y}");
                    }                   

                }
            }*/
        }

        internal bool IsMyPlayer(int user_id)
        {
            return Player == user_id;
        }

        private void OnTimerEvent(object sender, ElapsedEventArgs e)
        {

            var _bytes = new byte[9999];
            using (var binary = new BinaryWriter(new MemoryStream(_bytes)))
            {
                binary.Write(0);

                var _count = 0;
                foreach (int index in Entities.Keys)
                {
                    var _entity = Entities[index];
                    if(_entity.Position.Y > 0)
                    {
                        _count++;
                        binary.Write(index);
                        _entity.Write(binary);
                    }
                }

                var _length = (int)binary.BaseStream.Position;
                binary.BaseStream.Position = 0;
                binary.Write(_count);

                Worker.ReportProgress((int)ProgressIndex.EntityParse, new object[] { Convert.ToBase64String(_bytes, 0, _length) });
            }
        }

        public void ParsePackage(BinaryReader reader, int position, bool outcoming)
        {
            byte _type = reader.ReadByte();
            Position = position;
            if (_packages.ContainsKey(_type))
            {
                _packages[_type].ReadBinary(reader, this, outcoming);
            } else
            {
                //Console.WriteLine("");
            }
        }
    }
}
