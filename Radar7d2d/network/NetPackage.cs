
using System.IO;

namespace pcap_7day
{

    public interface INetPackage
    {
        void ReadBinary(BinaryReader reader, Network7d2d network, bool outcoming);
    }
}
