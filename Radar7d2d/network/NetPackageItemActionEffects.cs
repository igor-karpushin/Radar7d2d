
using System;
using System.IO;

namespace pcap_7day
{
    public struct NetPackageItemActionEffects : INetPackage
    {
        public void ReadBinary(BinaryReader reader, Network7d2d network, bool outcoming)
        {
            // item action effect
            var _entityId = reader.ReadInt32();
            var _slotIdx = reader.ReadByte();
            var _actionIdx = reader.ReadByte();
            var _firingState = reader.ReadByte();
            if (reader.ReadBoolean())
            {
                var _user_position = new Vector3i(
                    (int)(reader.ReadSingle() * 32.0 + 0.5),
                    (int)(reader.ReadSingle() * 32.0 + 0.5),
                    (int)(reader.ReadSingle() * 32.0 + 0.5)
                );
                var _direction = NetworkUtils.ReadVector3(reader);
                var _user_id = reader.ReadInt32();
                if (network.Entities.ContainsKey(_user_id))
                {
                    Console.WriteLine($"ItemActionEffect user: {_user_id} x:{_user_position.x} z:{_user_position.z}");
                    /*var _info = _users[_user_id];
                    _info.Position = _user_position;
                    _users[_user_id] = _info;*/
                }
            }
        }
    }
}
