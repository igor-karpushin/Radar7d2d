
using System;
using System.Collections.Generic;
using System.IO;

namespace pcap_7day
{
    public struct NetPackageMapChunks : INetPackage
    {
        public void ReadBinary(BinaryReader reader, Network7d2d network, bool outcoming)
        {
            var _entityId = reader.ReadInt32();
            var _tiles = new List<int>();
            var _tiles_data = new List<byte[]>();

            int num = (int)reader.ReadUInt16();
            for (int index1 = 0; index1 < num; ++index1)
            {
                _tiles.Add(reader.ReadInt32());

                var bytes = new byte[512];
                for (int index2 = 0; index2 < bytes.Length; ++index2)
                    bytes[index2] = reader.ReadByte();

                _tiles_data.Add(bytes);

                //network.Worker.ReportProgress((int)ProgressIndex.TileMap, new object[] { _tile, bytes });
            }

            network.Worker.ReportProgress((int)ProgressIndex.TileMap, new object[] { _tiles, _tiles_data });
        }
    }
}
