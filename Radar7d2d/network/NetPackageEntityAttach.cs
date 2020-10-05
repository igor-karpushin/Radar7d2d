
using System;
using System.IO;

namespace pcap_7day
{
    public struct NetPackageEntityAttach : INetPackage
    {
        public void ReadBinary(BinaryReader reader, Network7d2d network, bool outcoming)
        {
            //NetPackageEntityAttach
            if(outcoming)
            {
                var _attachType = reader.ReadByte();
                var _riderId = reader.ReadInt32();
                var _vehicleId = reader.ReadInt32();

                switch (_attachType)
                {

                    case 0:
                    case 1:
                        lock (network.Entities)
                        {
                            if (network.Entities.ContainsKey(_riderId))
                            {
                                if (network.Entities.ContainsKey(_vehicleId))
                                {
                                    var _vehicle = network.Entities[_vehicleId];
                                    _vehicle.attach = _riderId;
                                    network.Entities[_vehicleId] = _vehicle;

                                    var _rider = network.Entities[_riderId];
                                    _rider.attach = _vehicleId;
                                    network.Entities[_riderId] = _rider;
                                }
                            }
                        }
                        break;

                    case 2:
                        lock (network.Entities)
                        {
                            if (network.Entities.ContainsKey(_riderId))
                            {
                                var _rider = network.Entities[_riderId];

                                if (network.Entities.ContainsKey(_rider.attach))
                                {
                                    var _vehicle = network.Entities[_rider.attach];
                                    _vehicle.attach = 0;
                                    network.Entities[_rider.attach] = _vehicle;
                                }
                                _rider.attach = 0;
                                network.Entities[_riderId] = _rider;
                            }
                        }
                        break;
                }
            }
        }
    }
}
