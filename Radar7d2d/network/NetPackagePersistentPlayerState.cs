
using System;
using System.IO;

namespace pcap_7day
{
    public struct NetPackagePersistentPlayerState : INetPackage
    {
        public void ReadBinary(BinaryReader reader, Network7d2d network, bool outcoming)
        {
            var m_reason = reader.ReadByte();
            //Console.WriteLine($"EnumPersistentPlayerDataReason:{m_reason}");
        }
    }
}
