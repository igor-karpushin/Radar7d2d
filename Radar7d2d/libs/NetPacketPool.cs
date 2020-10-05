using System;
using System.Threading;

namespace pcap_7day
{
    internal sealed class NetPacketPool
    {
        private readonly NetPacket[] _pool = new NetPacket[1000];
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        private int _count;

        public NetPacket GetWithData(
          PacketProperty property,
          byte[] data,
          int start,
          int length)
        {
            NetPacket withProperty = this.GetWithProperty(property, length);
            Buffer.BlockCopy((Array)data, start, (Array)withProperty.RawData, NetPacket.GetHeaderSize(property), length);
            return withProperty;
        }

        public NetPacket GetPacket(int size, bool clear)
        {
            NetPacket netPacket = (NetPacket)null;
            if (size <= NetConstants.MaxPacketSize)
            {
                this._lock.EnterUpgradeableReadLock();
                if (this._count > 0)
                {
                    this._lock.EnterWriteLock();
                    --this._count;
                    netPacket = this._pool[this._count];
                    this._pool[this._count] = (NetPacket)null;
                    this._lock.ExitWriteLock();
                }
                this._lock.ExitUpgradeableReadLock();
            }
            if (netPacket == null)
                netPacket = new NetPacket(size);
            else
                netPacket.Realloc(size, clear);
            return netPacket;
        }

        public NetPacket GetWithProperty(PacketProperty property, int size)
        {
            size += NetPacket.GetHeaderSize(property);
            NetPacket packet = this.GetPacket(size, true);
            packet.Property = property;
            return packet;
        }

        public void Recycle(NetPacket packet)
        {
            if (packet.RawData.Length > NetConstants.MaxPacketSize)
                return;
            packet.RawData[0] = (byte)0;
            this._lock.EnterUpgradeableReadLock();
            if (this._count == 1000)
            {
                this._lock.ExitUpgradeableReadLock();
            }
            else
            {
                this._lock.EnterWriteLock();
                this._pool[this._count] = packet;
                ++this._count;
                this._lock.ExitWriteLock();
                this._lock.ExitUpgradeableReadLock();
            }
        }
    }
}
