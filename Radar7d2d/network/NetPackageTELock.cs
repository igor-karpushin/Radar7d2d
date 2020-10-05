
using System;
using System.IO;

namespace pcap_7day
{
    public struct NetPackageTELock : INetPackage
    {
        public void ReadBinary(BinaryReader reader, Network7d2d network, bool outcoming)
        {
            var _ttype = reader.ReadByte();
            var _clrIdx = (int)reader.ReadUInt16();
            var _entity_position = new Vector3i(
                reader.ReadInt32() * 32,
                reader.ReadInt32() * 32,
                reader.ReadInt32() * 32
            );
            var _entity_index = reader.ReadInt32();
            var _user_index = reader.ReadInt32();

            if (_user_index > 0)
            {
                if (network.Entities.ContainsKey(_user_index))
                {
                    var _info = network.Entities[_user_index];
                    //_info.Position = _entity_position;
                    //Console.WriteLine($"NetPackageTELock users: {_user_index} x:{_entity_position.x} z:{_entity_position.z}");
                    //_users[_user_index] = _info;
                }
            }
        }
    }
}
