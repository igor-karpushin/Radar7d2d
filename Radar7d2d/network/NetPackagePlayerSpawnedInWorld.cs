using System.IO;
using System.Numerics;

namespace pcap_7day
{
    public struct NetPackagePlayerSpawnedInWorld : INetPackage
    {
        public void ReadBinary(BinaryReader reader, Network7d2d network, bool outcoming)
        {
            //NetPackagePlayerSpawnedInWorld
            var _respawnReason = reader.ReadInt32();
            var _entity_position = new Vector3(
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32()
            );
            var _entityId = reader.ReadInt32();

            lock (network.Entities)
            {
                if (network.Entities.ContainsKey(_entityId))
                {
                    /*var _info = network.Entities[_entityId];
                    _info.Position = _entity_position;
                    network.Entities[_entityId] = _info;*/
                }
            }
        }
    }
}
