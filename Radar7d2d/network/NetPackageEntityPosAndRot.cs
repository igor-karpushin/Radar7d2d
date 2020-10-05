
using System;
using System.IO;
using System.Numerics;

namespace pcap_7day
{
    public struct NetPackageEntityPosAndRot : INetPackage
    {
        public void ReadBinary(BinaryReader reader, Network7d2d network, bool outcoming)
        {
            int _entity_id = reader.ReadInt32();
            var _user_position = new Vector3(
                (float)(reader.ReadSingle() * 32 + 0.5f),
                (float)(reader.ReadSingle() * 32 + 0.5f),
                (float)(reader.ReadSingle() * 32 + 0.5f)
            );

            var _bUseQRotation = reader.ReadBoolean();

            lock (network.Entities)
            {
                if (!network.Entities.ContainsKey(_entity_id))
                {
                    network.Entities.TryAdd(_entity_id, new EntityInfo 
                    { 
                        Name = "",
                        Position = _user_position
                    });
                }

                var _info = network.Entities[_entity_id];
                _info.Position = _user_position;
                _info.init = true;
                network.Entities[_entity_id] = _info;

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
