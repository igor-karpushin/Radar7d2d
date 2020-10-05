
using System;
using System.IO;

namespace pcap_7day
{
    public struct NetPackageEntitySpeeds : INetPackage
    {
        public void ReadBinary(BinaryReader reader, Network7d2d network, bool outcoming)
        {
            int _user_id = reader.ReadInt32();
            
        }
    }
}
