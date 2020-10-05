using System;
using System.IO;

namespace pcap_7day
{
    public struct NetPackageEntityRemove : INetPackage
    {
        public void ReadBinary(BinaryReader reader, Network7d2d network, bool outcoming)
        {
            var _entity_id = reader.ReadInt32();
            var _reason = reader.ReadByte();
            Console.WriteLine($"NetPackageEntityRemove >> {_entity_id}");
            lock(network.Entities)
            {
                if (network.Entities.TryRemove(_entity_id, out EntityInfo _))
                {
                    network.Worker.ReportProgress((int)ProgressIndex.EntityRemove, new object[] { _entity_id });
                }
            }                
        }
    }
}
