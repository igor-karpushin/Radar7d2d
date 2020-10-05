using System;
using System.IO;
using System.Numerics;

namespace pcap_7day
{
    public struct NetPackageEntitySpawn : INetPackage
    {
        public void ReadBinary(BinaryReader reader, Network7d2d network, bool outcoming)
        {
            var _readFileVersion = reader.ReadByte();
            var _entityClass = reader.ReadInt32();
            var _user_id = reader.ReadInt32();
            var _lifetime = reader.ReadSingle();

            var _position = new Vector3(
                (float)(reader.ReadSingle() * 32.0 + 0.5),
                (float)(reader.ReadSingle() * 32.0 + 0.5),
                (float)(reader.ReadSingle() * 32.0 + 0.5)
            );

            lock (network.Entities)
            {
                if (!network.Entities.ContainsKey(_user_id))
                {
                    network.Entities.TryAdd(_user_id, new EntityInfo
                    {
                        Position = new Vector3(),
                        Name = ""
                    });
                }

                var _entity = network.Entities[_user_id];
                _entity.Position = _position;
                if (network.SpawnTypes.TryGetValue(_entityClass, out SpawnTypeIndex type))
                {
                    _entity.type = type;
                }
                network.Entities[_user_id] = _entity;
                //Console.WriteLine($"NetPackageEntitySpawn >> id:{_user_id} >> type:{_entity.type} >> class: {_entityClass}");
            }
        }
    }
}
