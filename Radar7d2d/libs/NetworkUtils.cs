using System;
using System.IO;
using System.Numerics;
using System.Threading;

namespace pcap_7day
{

    public class Vector2
    {
        public float x;
        public float y;

        public Vector2(float v1, float v2)
        {
            this.x = v1;
            this.y = v2;
        }
    }

    public class Vector2i
    {
        public int x;
        public int y;

        public Vector2i(int v1, int v2)
        {
            this.x = v1;
            this.y = v2;
        }
    }

    /*public class Vector3
    {
        public float x;
        public float y;
        public float z;

        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }*/

    public class Quaternion
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public Quaternion(float v1, float v2, float v3, float v4)
        {
            this.x = v1;
            this.y = v2;
            this.z = v3;
            this.w = v4;
        }
    }

    public class Vector3i
    {
        public int x;
        public int y;
        public int z;

        public Vector3i(int v1, int v2, int v3)
        {
            this.x = v1;
            this.y = v2;
            this.z = v3;
        }
    }

    public enum ProgressType : int
    {
        UserAdd,
        UserRemove,
        UserUpdate,
        UserName,
        BotAdd,
        BotRemove,
        BotUpdate
    }

    public class NetworkUtils
    {
        public static long ReadInt64(Stream clientStream)
        {
            return (long)(clientStream.ReadByte() & (int)byte.MaxValue) | (long)(clientStream.ReadByte() & (int)byte.MaxValue) << 8 | (long)(clientStream.ReadByte() & (int)byte.MaxValue) << 16 | (long)(clientStream.ReadByte() & (int)byte.MaxValue) << 24 | (long)(clientStream.ReadByte() & (int)byte.MaxValue) << 32 | (long)(clientStream.ReadByte() & (int)byte.MaxValue) << 40 | (long)(clientStream.ReadByte() & (int)byte.MaxValue) << 48 | (long)(clientStream.ReadByte() & (int)byte.MaxValue) << 56;
        }

        public static void Write(Stream clientStream, long v)
        {
            clientStream.WriteByte((byte)((ulong)v & (ulong)byte.MaxValue));
            clientStream.WriteByte((byte)((ulong)(v >> 8) & (ulong)byte.MaxValue));
            clientStream.WriteByte((byte)((ulong)(v >> 16) & (ulong)byte.MaxValue));
            clientStream.WriteByte((byte)((ulong)(v >> 24) & (ulong)byte.MaxValue));
            clientStream.WriteByte((byte)((ulong)(v >> 32) & (ulong)byte.MaxValue));
            clientStream.WriteByte((byte)((ulong)(v >> 40) & (ulong)byte.MaxValue));
            clientStream.WriteByte((byte)((ulong)(v >> 48) & (ulong)byte.MaxValue));
            clientStream.WriteByte((byte)((ulong)(v >> 56) & (ulong)byte.MaxValue));
        }

        public static int ReadInt32(Stream clientStream)
        {
            return 0 | clientStream.ReadByte() & (int)byte.MaxValue | (clientStream.ReadByte() & (int)byte.MaxValue) << 8 | (clientStream.ReadByte() & (int)byte.MaxValue) << 16 | (clientStream.ReadByte() & (int)byte.MaxValue) << 24;
        }

        public static int ReadInt32(byte[] buffer, int offset)
        {
            return 0 | (int)buffer[offset] | (int)buffer[offset + 1] << 8 | (int)buffer[offset + 2] << 16 | (int)buffer[offset + 3] << 24;
        }

        public static void Write(Stream clientStream, int v)
        {
            clientStream.WriteByte((byte)(v & (int)byte.MaxValue));
            clientStream.WriteByte((byte)(v >> 8 & (int)byte.MaxValue));
            clientStream.WriteByte((byte)(v >> 16 & (int)byte.MaxValue));
            clientStream.WriteByte((byte)(v >> 24 & (int)byte.MaxValue));
        }

        public static ushort ReadUInt16(Stream clientStream)
        {
            return (ushort)((uint)(ushort)(0U | (uint)(ushort)(clientStream.ReadByte() & (int)byte.MaxValue)) | (uint)(ushort)((clientStream.ReadByte() & (int)byte.MaxValue) << 8));
        }

        public static ushort ReadUInt16(byte[] buffer, int offset)
        {
            return (ushort)((uint)buffer[offset] | (uint)buffer[offset + 1] << 8);
        }

        public static void Write(Stream clientStream, ushort v)
        {
            clientStream.WriteByte((byte)((uint)v & (uint)byte.MaxValue));
            clientStream.WriteByte((byte)((int)v >> 8 & (int)byte.MaxValue));
        }

        public static short ReadInt16(Stream clientStream)
        {
            return (short)((int)(short)(0 | (int)(short)(clientStream.ReadByte() & (int)byte.MaxValue)) | (int)(short)((clientStream.ReadByte() & (int)byte.MaxValue) << 8));
        }

        public static short ReadInt16(byte[] buffer, int offset)
        {
            return (short)((int)buffer[offset] | (int)buffer[offset + 1] << 8);
        }

        public static void Write(Stream clientStream, short v)
        {
            clientStream.WriteByte((byte)((uint)v & (uint)byte.MaxValue));
            clientStream.WriteByte((byte)((int)v >> 8 & (int)byte.MaxValue));
        }

        public static byte ReadByte(Stream clientStream)
        {
            return (byte)clientStream.ReadByte();
        }

        public static byte ReadByte(byte[] buffer, int offset)
        {
            return buffer[offset];
        }

        public static void Write(Stream clientStream, byte _b)
        {
            clientStream.WriteByte(_b);
        }

        public static void Write(BinaryWriter _bw, Vector3 _v)
        {
            _bw.Write(_v.X);
            _bw.Write(_v.Y);
            _bw.Write(_v.Z);
        }

        public static Vector3 ReadVector3(BinaryReader _br)
        {
            return new Vector3(_br.ReadSingle(), _br.ReadSingle(), _br.ReadSingle());
        }

        public static void Write(BinaryWriter _bw, Vector3i _v)
        {
            _bw.Write(_v.x);
            _bw.Write(_v.y);
            _bw.Write(_v.z);
        }

        public static Vector3i ReadVector3i(BinaryReader _br)
        {
            return new Vector3i(_br.ReadInt32(), _br.ReadInt32(), _br.ReadInt32());
        }

        public static void Write(BinaryWriter _bw, Vector2 _v)
        {
            _bw.Write(_v.x);
            _bw.Write(_v.y);
        }

        public static Vector2 ReadVector2(BinaryReader _br)
        {
            return new Vector2(_br.ReadSingle(), _br.ReadSingle());
        }

        public static void Write(BinaryWriter _bw, Vector2i _v)
        {
            _bw.Write(_v.x);
            _bw.Write(_v.y);
        }

        public static Vector2i ReadVector2i(BinaryReader _br)
        {
            return new Vector2i(_br.ReadInt32(), _br.ReadInt32());
        }

        public static void Write(BinaryWriter _bw, Quaternion _q)
        {
            _bw.Write(_q.x);
            _bw.Write(_q.y);
            _bw.Write(_q.z);
            _bw.Write(_q.w);
        }

        public static Quaternion ReadQuaterion(BinaryReader _br)
        {
            return new Quaternion(_br.ReadSingle(), _br.ReadSingle(), _br.ReadSingle(), _br.ReadSingle());
        }

    }
}
