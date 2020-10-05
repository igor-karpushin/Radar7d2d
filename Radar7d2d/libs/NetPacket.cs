using LiteNetLib.Utils;
using System;

namespace pcap_7day
{
    internal enum PacketProperty : byte
    {
        Unreliable,
        ReliableUnordered,
        Sequenced,
        ReliableOrdered,
        AckReliable,
        AckReliableOrdered,
        Ping,
        Pong,
        ConnectRequest,
        ConnectAccept,
        Disconnect,
        UnconnectedMessage,
        NatIntroductionRequest,
        NatIntroduction,
        NatPunchMessage,
        MtuCheck,
        MtuOk,
        DiscoveryRequest,
        DiscoveryResponse,
        Merged,
        ShutdownOk,
        ReliableSequenced,
        AckReliableSequenced,
        PeerNotFound,
        InvalidProtocol,
    }

    internal sealed class NetPacket
    {
        private const int LastProperty = 24;
        public byte[] RawData;
        public int Size;

        public PacketProperty Property
        {
            get
            {
                return (PacketProperty)((uint)this.RawData[0] & 31U);
            }
            set
            {
                this.RawData[0] = (byte)((PacketProperty)((int)this.RawData[0] & 224) | value);
            }
        }

        public byte ConnectionNumber
        {
            get
            {
                return (byte)(((int)this.RawData[0] & 96) >> 5);
            }
            set
            {
                this.RawData[0] = (byte)((int)this.RawData[0] & 159 | (int)value << 5);
            }
        }

        public ushort Sequence
        {
            get
            {
                return BitConverter.ToUInt16(this.RawData, 1);
            }
            set
            {
                FastBitConverter.GetBytes(this.RawData, 1, value);
            }
        }

        public bool IsFragmented
        {
            get
            {
                return ((uint)this.RawData[0] & 128U) > 0U;
            }
        }

        public void MarkFragmented()
        {
            this.RawData[0] |= (byte)128;
        }

        public ushort FragmentId
        {
            get
            {
                return BitConverter.ToUInt16(this.RawData, 3);
            }
            set
            {
                FastBitConverter.GetBytes(this.RawData, 3, value);
            }
        }

        public ushort FragmentPart
        {
            get
            {
                return BitConverter.ToUInt16(this.RawData, 5);
            }
            set
            {
                FastBitConverter.GetBytes(this.RawData, 5, value);
            }
        }

        public ushort FragmentsTotal
        {
            get
            {
                return BitConverter.ToUInt16(this.RawData, 7);
            }
            set
            {
                FastBitConverter.GetBytes(this.RawData, 7, value);
            }
        }

        public NetPacket(int size)
        {
            this.RawData = new byte[size];
            this.Size = size;
        }

        public NetPacket(PacketProperty property, int size)
        {
            size += NetPacket.GetHeaderSize(property);
            this.RawData = new byte[size];
            this.Property = property;
            this.Size = size;
        }

        public void Realloc(int toSize, bool clear)
        {
            this.Size = toSize;
            if (this.RawData.Length < toSize)
            {
                this.RawData = new byte[toSize];
            }
            else
            {
                if (!clear)
                    return;
                Array.Clear((Array)this.RawData, 0, toSize);
            }
        }

        public static int GetHeaderSize(PacketProperty property)
        {
            switch (property)
            {
                case PacketProperty.ReliableUnordered:
                case PacketProperty.Sequenced:
                case PacketProperty.ReliableOrdered:
                case PacketProperty.AckReliable:
                case PacketProperty.AckReliableOrdered:
                case PacketProperty.Ping:
                case PacketProperty.ReliableSequenced:
                case PacketProperty.AckReliableSequenced:
                    return 3;
                case PacketProperty.Pong:
                    return 11;
                case PacketProperty.ConnectRequest:
                    return 13;
                case PacketProperty.ConnectAccept:
                    return 11;
                case PacketProperty.Disconnect:
                    return 9;
                default:
                    return 1;
            }
        }

        public int GetHeaderSize()
        {
            return NetPacket.GetHeaderSize(this.Property);
        }

        public bool FromBytes(byte[] data, int start, int packetSize)
        {
            int num = (int)(byte)((uint)data[start] & 31U);
            bool flag = ((uint)data[start] & 128U) > 0U;
            int headerSize = NetPacket.GetHeaderSize((PacketProperty)num);
            if (num > 24 || packetSize < headerSize || flag && packetSize < headerSize + 6)
                return false;
            Buffer.BlockCopy((Array)data, start, (Array)this.RawData, 0, packetSize);
            this.Size = packetSize;
            return true;
        }
    }
}
