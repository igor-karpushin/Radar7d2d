
using System;
using System.IO;

namespace pcap_7day
{
    public struct NetPackagePlayerStats : INetPackage
    {
        public void ReadBinary(BinaryReader reader, Network7d2d network, bool outcoming)
        {
            int _user_id = reader.ReadInt32();
            var _flags = reader.ReadUInt16();
            var _killed = reader.ReadInt32();

            lock (network.Entities)
            {
                if (network.Entities.ContainsKey(_user_id))
                {
                    var _user = network.Entities[_user_id];
                    if (_user.Name.Length == 0 && _user.type == SpawnTypeIndex.Players)
                    {
                        var index = 5;
                        while(index < 256)
                        {
                            reader.BaseStream.Position = network.Position - index;
                            var _length = Read7BitEncodedInt(reader);
                            if(_length < index)
                            {
                                if (_length > 2 && _length < 40)
                                {
                                    reader.BaseStream.Position = network.Position - index;
                                    var _read_name = reader.ReadString();
                                    if (IsString(_read_name))
                                    {
                                        _user.Name = _read_name;
                                        network.Entities[_user_id] = _user;

                                        Console.WriteLine($"NetPackagePlayerStats >> {_user_id} >> {_read_name}");

                                        network.Worker.ReportProgress((int)ProgressIndex.EntityName, new object[] { _user_id, _user.Name });
                                        break;
                                    }
                                }
                            }
                            index++;
                        }
                        
                    }
                }
            }
        }

        private int Read7BitEncodedInt(BinaryReader reader)
        {
            int count = 0;
            int shift = 0;
            byte b;
            do
            {
                // Check for a corrupted stream.  Read a max of 5 bytes.
                // In a future version, add a DataFormatException.
                if (shift == 5 * 7)  // 5 bytes max per Int32, shift += 7
                {
                    return -1;
                }

                // ReadByte handles end of stream cases for us.
                b = reader.ReadByte();
                count |= (b & 0x7F) << shift;
                shift += 7;
            } while ((b & 0x80) != 0);
            return count;
        }

        private bool IsString(string name)
        {
            //_min 1025  _max 1105

            for (int i = 0; i < name.Length; ++i)
            {
                var _char = name[i];
                if ((int)_char < 31) return false;
                if ((int)_char > 127 && (int)_char < 1025) return false;
                if ((int)_char > 1105) return false;
            }

            return true;
        }
    }
}
