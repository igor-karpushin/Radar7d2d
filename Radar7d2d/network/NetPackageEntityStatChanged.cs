
using System;
using System.IO;

namespace pcap_7day
{
    public struct NetPackageEntityStatChanged : INetPackage
    {
        public void ReadBinary(BinaryReader reader, Network7d2d network, bool outcoming)
        {
            int _user_id = reader.ReadInt32();
            int _instigator = reader.ReadInt32();
            byte _stat = reader.ReadByte();
            if (_stat == 0)
            {
                var _health = reader.ReadSingle();
                var _max_health = reader.ReadSingle();
                lock(network.Entities)
                {
                    if (network.Entities.ContainsKey(_user_id))
                    {
                        var _info = network.Entities[_user_id];
                        if (network.Entities.ContainsKey(_instigator) && _instigator != _user_id)
                        {
                            var _i_user = network.Entities[_instigator];
                            Console.WriteLine($"shooter:{_instigator}<{_i_user.Name}> target:{_user_id}<{_info.Name}> damage:{_info.Health - _health}");
                        }
                        _info.MaxHealth = _max_health;
                        _info.Health = _health;
                        network.Entities[_user_id] = _info;
                    }
                }
                
            }
        }
    }
}
