
using System;
using System.IO;
using System.Numerics;

namespace pcap_7day
{
    public struct NetPackageClientInfo : INetPackage
    {
        public void ReadBinary(BinaryReader reader, Network7d2d network, bool outcoming)
        {
            //NetPackageClientInfo
            int _number = reader.ReadUInt16();

            lock(network.Entities)
            {
                for (int k = 0; k < _number; ++k)
                {
                    var _entity_id = reader.ReadInt32();

                    var _ping_time = (int)reader.ReadInt16();
                    var _is_admin = reader.ReadBoolean();

                    if(network.Entities.ContainsKey(_entity_id))
                    {
                        var _entity = network.Entities[_entity_id];
                        if (_entity.type != SpawnTypeIndex.Players)
                        {
                            if (network.Entities.TryRemove(_entity_id, out EntityInfo remove))
                            {
                                network.Worker.ReportProgress((int)ProgressIndex.EntityRemove, new object[] { _entity_id });
                            }
                        }                        
                    }

                    if (network.Entities.TryAdd(_entity_id, new EntityInfo
                    {
                        Position = new Vector3(),
                        Name = "",
                        type = SpawnTypeIndex.Players
                    }))
                    {
                        network.Worker.ReportProgress((int)ProgressIndex.EntityAdd, new object[] { _entity_id });
                    }
                }
            }            
        }
    }
}
