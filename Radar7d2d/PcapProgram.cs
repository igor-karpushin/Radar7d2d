using Noemax.GZip;

using PcapDotNet.Core;
using PcapDotNet.Packets;
using PcapDotNet.Packets.IpV4;

using Radar7d2d;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Numerics;

namespace pcap_7day
{

    public class PcapProgram
    {
        private readonly NetPacketPool _net_pool;

        private class IncomingFragments
        {
            public NetPacket[] Fragments;
            public int ReceivedCount;
            public int TotalSize;
        }

        private readonly Form1 _base_form;

        private readonly Dictionary<ushort, IncomingFragments> _holdedFragments;
        private readonly DeflateInputStream _zip_stream;        
        private readonly MemoryStream _compressed_stream;
        private readonly MemoryStream _uncompressed_stream;

        private readonly Network7d2d _network;

        const uint _k_byte = 1024;
        const uint _m_byte = _k_byte * _k_byte;
        const uint _capacity = 8 * _m_byte;

        public PcapProgram(Form1 form, BackgroundWorker worker)
        {
            _base_form = form;

            // memory
            _compressed_stream = new MemoryStream(new byte[_capacity]);
            _uncompressed_stream = new MemoryStream(new byte[_capacity], true);
            _zip_stream = new DeflateInputStream(_compressed_stream);

            _network = new Network7d2d(worker, _base_form);
            PacketDevice selectedDevice = LivePacketDevice.AllLocalMachine[_base_form.Device];

            _net_pool = new NetPacketPool();
            _holdedFragments = new Dictionary<ushort, IncomingFragments>();

            using (PacketCommunicator communicator = selectedDevice.Open(65536, PacketDeviceOpenAttributes.DataTransferUdpRemote, 2000))
            {
                using (BerkeleyPacketFilter filter = communicator.CreateFilter("ip and udp"))
                {
                    communicator.SetFilter(filter);
                }
                communicator.ReceivePackets(0, PacketHandler);
            }
        }

        private void PacketHandler(Packet packet)
        {
            IpV4Datagram ip = packet.Ethernet.IpV4;

            bool _incoming = ip.Source.ToString() == _base_form.World.server;
            bool _outcoming = ip.Destination.ToString() == _base_form.World.server;
 
            if (_incoming || _outcoming)
            {
                var _buffer = new byte[packet.Buffer.Length - 42];
                Buffer.BlockCopy((Array)packet.Buffer, 42, (Array)_buffer, 0, _buffer.Length);
                var _packet = _net_pool.GetPacket(_buffer.Length, false);
                if (!_packet.FromBytes(_buffer, 0, _buffer.Length))
                {
                    _net_pool.Recycle(_packet);
                    //Console.WriteLine("Lost _packet");
                    return;
                }
                ProcessPacket(_packet, _outcoming);
            }
        }

        private void ProcessPacket(NetPacket packet, bool outcoming)
        {
            //Console.WriteLine($"packet: {packet.Property}");
            switch (packet.Property)
            {
                case PacketProperty.Unreliable:
                    break;

                case PacketProperty.ReliableSequenced:
                case PacketProperty.ReliableOrdered:
                    if (packet.IsFragmented)
                    {
                        if (!_holdedFragments.TryGetValue(packet.FragmentId, out IncomingFragments incomingFragments))
                        {
                            incomingFragments = new IncomingFragments()
                            {
                                Fragments = new NetPacket[(int)packet.FragmentsTotal]
                            };
                            _holdedFragments.Add(packet.FragmentId, incomingFragments);
                        }
                        NetPacket[] fragments = incomingFragments.Fragments;
                        if ((int)packet.FragmentPart >= fragments.Length || fragments[(int)packet.FragmentPart] != null)
                        {
                            _net_pool.Recycle(packet);
                            Console.WriteLine("Invalid fragment packet", (object[])Array.Empty<object>());
                            return;
                        }

                        fragments[(int)packet.FragmentPart] = packet;
                        ++incomingFragments.ReceivedCount;
                        int srcOffset = packet.GetHeaderSize() + 6;
                        incomingFragments.TotalSize += packet.Size - srcOffset;
                        if (incomingFragments.ReceivedCount != fragments.Length)
                            return;
                        NetPacket withProperty = _net_pool.GetWithProperty(packet.Property, incomingFragments.TotalSize);
                        int headerSize = withProperty.GetHeaderSize();
                        int num = fragments[0].Size - srcOffset;
                        for (int index = 0; index < incomingFragments.ReceivedCount; ++index)
                        {
                            int count = fragments[index].Size - srcOffset;
                            Buffer.BlockCopy((Array)fragments[index].RawData, srcOffset, (Array)withProperty.RawData, headerSize + num * index, count);
                            _net_pool.Recycle(fragments[index]);
                            fragments[index] = (NetPacket)null;
                        }
                        _holdedFragments.Remove(packet.FragmentId);
                        ReceiveFromPeer(withProperty, outcoming);
                    }
                    else
                    {
                        ReceiveFromPeer(packet, outcoming);
                    }
                    break;

                case PacketProperty.AckReliableOrdered:
                    _net_pool.Recycle(packet);
                    break;

                case PacketProperty.Merged:
                    int startIndex = 1;
                    while (startIndex < packet.Size)
                    {
                        ushort uint16 = BitConverter.ToUInt16(packet.RawData, startIndex);
                        int start = startIndex + 2;
                        NetPacket packet1 = _net_pool.GetPacket((int)uint16, false);
                        if (!packet1.FromBytes(packet.RawData, start, (int)uint16))
                        {
                            _net_pool.Recycle(packet);
                            break;
                        }
                        startIndex = start + (int)uint16;
                        ProcessPacket(packet1, outcoming);
                    }
                    break;

                case PacketProperty.Ping:
                case PacketProperty.Pong:
                    
                    break;

                default:
                    Console.WriteLine($"packet: {packet.Property}");
                    break;
            }
        }

        private void ReceiveFromPeer(NetPacket packet, bool outcoming)
        {
            var _reader = new NetDataReader(packet.RawData, packet.GetHeaderSize() + 1, packet.Size);
            if (_reader.AvailableBytes > 0)
            {
                var _size = _reader.GetInt();
                var _compressed = _reader.GetByte() == 1;
                var _packages = _reader.GetUShort();

                if (_packages > 0)
                {
                    var buffer = new byte[_size];
                    _reader.GetBytes(buffer, _size);

                    _compressed_stream.SetLength(0);
                    _compressed_stream.Write(buffer, 0, _size);
                    _compressed_stream.Position = 0;

                    if (_compressed)
                    {
                        _uncompressed_stream.SetLength(0);
                        _uncompressed_stream.Position = 0;
                        _zip_stream.Restart();
                        try
                        {
                            StreamCopy(_zip_stream, _uncompressed_stream, buffer, false);
                        }
                        catch (Exception error)
                        {
                            _compressed_stream.Position = 0;
                            _uncompressed_stream.Position = 0;
                            _zip_stream.Restart();

                            Console.WriteLine($"Error: {error.Message}");
                            _net_pool.Recycle(packet);
                            _network.Worker.ReportProgress((int)ProgressIndex.Error);

                            return;
                        }
                        ParseStream(_uncompressed_stream, _packages, outcoming);
                    }
                    else
                    {
                        ParseStream(_compressed_stream, _packages, outcoming);
                    }
                }
            }
            _net_pool.Recycle(packet);
        }

        private void ParseStream(MemoryStream stream, ushort packages, bool outcoming)
        {
            stream.Position = 0;
            var _reader = new BinaryReader(stream);
            {
                while (packages-- > 0)
                {
                    if (_reader.BaseStream.Length < 4)
                    {
                        Console.WriteLine("_reader.BaseStream.Length < 4");
                        return;
                    }
                    int num = _reader.ReadInt32();
                    if (num < 0)
                    {
                        Console.WriteLine("num < 0");
                        return;
                    }
                    long _position = stream.Position + num;

                    _network.ParsePackage(_reader, (int)_position, outcoming);

                    stream.Position = _position;
                }
            }
        }


        private void StreamCopy(Stream _source, Stream _destination, byte[] _tempBuf, bool _bFlush = true)
        {
            byte[] numArray = _tempBuf;
            bool flag = true;
            while (flag)
            {
                int count = _source.Read(numArray, 0, numArray.Length);
                if (count > 0)
                {
                    _destination.Write(numArray, 0, count);
                }
                else
                {
                    if (_bFlush)
                        _destination.Flush();
                    flag = false;
                }
            }
            if (_tempBuf != null)
                return;
        }
    }
}
