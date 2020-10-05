
using System;
using System.IO;
using System.Numerics;

namespace pcap_7day
{
    public struct NetPackageEntityTeleport : INetPackage
    {
        public void ReadBinary(BinaryReader reader, Network7d2d network, bool outcoming)
        {
            int _user_id = reader.ReadInt32();
            var _position = new Vector3(
                (float)(reader.ReadSingle() * 32.0 + 0.5),
                (float)(reader.ReadSingle() * 32.0 + 0.5),
                (float)(reader.ReadSingle() * 32.0 + 0.5)
            );

            var _bUseQRotation = reader.ReadBoolean();

            reader.ReadSingle();
            reader.ReadSingle();
            reader.ReadSingle();

            if (_bUseQRotation)
            {
                reader.ReadSingle();
            }

            if (network.Entities.ContainsKey(_user_id))
            {

                lock (network.Entities)
                {
                    var _info = network.Entities[_user_id];
                    _info.Position = _position;
                    _info.init = true;
                    network.Entities[_user_id] = _info;

                    Console.WriteLine($"NetPackageEntityTeleport user:{_info.Name} >> death ? >> teleport >> x:{_position.X / 32} z:{_position.Z / 32}");
                }
            }
        }
    }
}
