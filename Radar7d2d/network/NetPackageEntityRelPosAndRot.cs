
using System;
using System.IO;
using System.Numerics;

namespace pcap_7day
{

    public struct NetPackageEntityRelPosAndRot : INetPackage
    {
        public void ReadBinary(BinaryReader reader, Network7d2d network, bool outcoming)
        {
            int _user_id = reader.ReadInt32();
            if (network.Entities.ContainsKey(_user_id))
            {
                lock (network.Entities)
                {
                    var _info = network.Entities[_user_id];
                    if (_info.init)
                    {
                        var bUseQRotation = reader.ReadBoolean();
                        if (!bUseQRotation)
                        {
                            _info.Rotation = new Vector3(
                                (float)(reader.ReadInt16() * 360) / 256f,
                                (float)(reader.ReadInt16() * 360) / 256f,
                                (float)(reader.ReadInt16() * 360) / 256f
                            );
                        }
                        else
                        {
                            reader.ReadSingle();
                            reader.ReadSingle();
                            reader.ReadSingle();
                            reader.ReadSingle();
                        }

                        var _delta_position = new Vector3i(
                            reader.ReadInt16(),
                            reader.ReadInt16(),
                            reader.ReadInt16()
                        );

                        _info.Position.X += _delta_position.x;
                        _info.Position.Y += _delta_position.y;
                        _info.Position.Z += _delta_position.z;

                        network.Entities[_user_id] = _info;

                        if (_info.attach > 0)
                        {
                            if (network.Entities.ContainsKey(_info.attach))
                            {
                                var _entity = network.Entities[_info.attach];
                                _entity.Position = _info.Position;
                                network.Entities[_info.attach] = _entity;
                            }
                        }
                    }
                }
            }
            
        }
    }
}
