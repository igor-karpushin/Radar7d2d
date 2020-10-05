
using System;
using System.IO;

namespace pcap_7day
{
    public struct NetPackageGameMessage : INetPackage
    {
        public void ReadBinary(BinaryReader reader, Network7d2d network, bool outcoming)
        {
            var _msgType = reader.ReadByte();
            var _msg = reader.ReadString();
            var _mainName = reader.ReadString();
            var _localizeMain = reader.ReadBoolean();
            var _secondaryName = reader.ReadString();
            var _localizeSecondary = reader.ReadBoolean();
            //Console.WriteLine($"type:{_msgType} msg:{_msg} n1:{_mainName} l1:{_localizeMain} n2:{_secondaryName} l2:{_localizeSecondary}");
        }
    }
}
