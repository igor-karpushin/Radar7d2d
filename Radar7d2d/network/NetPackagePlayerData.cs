
using System.IO;
using System.Numerics;

namespace pcap_7day
{
    public struct NetPackagePlayerData : INetPackage
    {
        public void ReadBinary(BinaryReader reader, Network7d2d network, bool outcoming)
        {
            var _readFileVersion = reader.ReadByte();
            var _entityClass = reader.ReadInt32();
            var _user_index = reader.ReadInt32();
            var _lifetime = reader.ReadSingle();

            lock(network.Entities)
            {
                if(network.Player != _user_index)
                {
                    network.Player = _user_index;
                    network.Worker.ReportProgress((int)ProgressIndex.EntityPlayer, new object[] { _user_index }); 
                }

                if (network.Entities.ContainsKey(_user_index))
                {
                    var _info = network.Entities[_user_index];
                    _info.init = true;
                    _info.Position = new Vector3(
                        (float)(reader.ReadSingle() * 32 + 0.5),
                        (float)(reader.ReadSingle() * 32 + 0.5),
                        (float)(reader.ReadSingle() * 32 + 0.5)
                    );
                    network.Entities[_user_index] = _info;
                }
            }            
        }
    }
}
