using System;
using System.Text;

namespace pcap_7day
{
    public class NetDataReader
    {
        protected byte[] _data;
        protected int _position;
        protected int _dataSize;
        private int _offset;

        public byte[] RawData
        {
            get
            {
                return this._data;
            }
        }

        public int RawDataSize
        {
            get
            {
                return this._dataSize;
            }
        }

        public int UserDataOffset
        {
            get
            {
                return this._offset;
            }
        }

        public int UserDataSize
        {
            get
            {
                return this._dataSize - this._offset;
            }
        }

        public bool IsNull
        {
            get
            {
                return this._data == null;
            }
        }

        public int Position
        {
            get
            {
                return this._position;
            }
        }

        public bool EndOfData
        {
            get
            {
                return this._position == this._dataSize;
            }
        }

        public int AvailableBytes
        {
            get
            {
                return this._dataSize - this._position;
            }
        }

        public void SetSource(byte[] source)
        {
            this._data = source;
            this._position = 0;
            this._offset = 0;
            this._dataSize = source.Length;
        }

        public void SetSource(byte[] source, int offset)
        {
            this._data = source;
            this._position = offset;
            this._offset = offset;
            this._dataSize = source.Length;
        }

        public void SetSource(byte[] source, int offset, int maxSize)
        {
            this._data = source;
            this._position = offset;
            this._offset = offset;
            this._dataSize = maxSize;
        }

        public NetDataReader()
        {
        }

        public NetDataReader(byte[] source)
        {
            this.SetSource(source);
        }

        public NetDataReader(byte[] source, int offset)
        {
            this.SetSource(source, offset);
        }

        public NetDataReader(byte[] source, int offset, int maxSize)
        {
            this.SetSource(source, offset, maxSize);
        }

        public byte GetByte()
        {
            int num = (int)this._data[this._position];
            ++this._position;
            return (byte)num;
        }

        public sbyte GetSByte()
        {
            int num = (int)(sbyte)this._data[this._position];
            ++this._position;
            return (sbyte)num;
        }

        public bool[] GetBoolArray()
        {
            ushort uint16 = BitConverter.ToUInt16(this._data, this._position);
            this._position += 2;
            bool[] flagArray = new bool[(int)uint16];
            for (int index = 0; index < (int)uint16; ++index)
                flagArray[index] = this.GetBool();
            return flagArray;
        }

        public ushort[] GetUShortArray()
        {
            ushort uint16 = BitConverter.ToUInt16(this._data, this._position);
            this._position += 2;
            ushort[] numArray = new ushort[(int)uint16];
            for (int index = 0; index < (int)uint16; ++index)
                numArray[index] = this.GetUShort();
            return numArray;
        }

        public short[] GetShortArray()
        {
            ushort uint16 = BitConverter.ToUInt16(this._data, this._position);
            this._position += 2;
            short[] numArray = new short[(int)uint16];
            for (int index = 0; index < (int)uint16; ++index)
                numArray[index] = this.GetShort();
            return numArray;
        }

        public long[] GetLongArray()
        {
            ushort uint16 = BitConverter.ToUInt16(this._data, this._position);
            this._position += 2;
            long[] numArray = new long[(int)uint16];
            for (int index = 0; index < (int)uint16; ++index)
                numArray[index] = this.GetLong();
            return numArray;
        }

        public ulong[] GetULongArray()
        {
            ushort uint16 = BitConverter.ToUInt16(this._data, this._position);
            this._position += 2;
            ulong[] numArray = new ulong[(int)uint16];
            for (int index = 0; index < (int)uint16; ++index)
                numArray[index] = this.GetULong();
            return numArray;
        }

        public int[] GetIntArray()
        {
            ushort uint16 = BitConverter.ToUInt16(this._data, this._position);
            this._position += 2;
            int[] numArray = new int[(int)uint16];
            for (int index = 0; index < (int)uint16; ++index)
                numArray[index] = this.GetInt();
            return numArray;
        }

        public uint[] GetUIntArray()
        {
            ushort uint16 = BitConverter.ToUInt16(this._data, this._position);
            this._position += 2;
            uint[] numArray = new uint[(int)uint16];
            for (int index = 0; index < (int)uint16; ++index)
                numArray[index] = this.GetUInt();
            return numArray;
        }

        public float[] GetFloatArray()
        {
            ushort uint16 = BitConverter.ToUInt16(this._data, this._position);
            this._position += 2;
            float[] numArray = new float[(int)uint16];
            for (int index = 0; index < (int)uint16; ++index)
                numArray[index] = this.GetFloat();
            return numArray;
        }

        public double[] GetDoubleArray()
        {
            ushort uint16 = BitConverter.ToUInt16(this._data, this._position);
            this._position += 2;
            double[] numArray = new double[(int)uint16];
            for (int index = 0; index < (int)uint16; ++index)
                numArray[index] = this.GetDouble();
            return numArray;
        }

        public string[] GetStringArray()
        {
            ushort uint16 = BitConverter.ToUInt16(this._data, this._position);
            this._position += 2;
            string[] strArray = new string[(int)uint16];
            for (int index = 0; index < (int)uint16; ++index)
                strArray[index] = this.GetString();
            return strArray;
        }

        public string[] GetStringArray(int maxStringLength)
        {
            ushort uint16 = BitConverter.ToUInt16(this._data, this._position);
            this._position += 2;
            string[] strArray = new string[(int)uint16];
            for (int index = 0; index < (int)uint16; ++index)
                strArray[index] = this.GetString(maxStringLength);
            return strArray;
        }

        public bool GetBool()
        {
            int num = this._data[this._position] > (byte)0 ? 1 : 0;
            ++this._position;
            return num != 0;
        }

        public char GetChar()
        {
            int num = (int)BitConverter.ToChar(this._data, this._position);
            this._position += 2;
            return (char)num;
        }

        public ushort GetUShort()
        {
            int uint16 = (int)BitConverter.ToUInt16(this._data, this._position);
            this._position += 2;
            return (ushort)uint16;
        }

        public short GetShort()
        {
            int int16 = (int)BitConverter.ToInt16(this._data, this._position);
            this._position += 2;
            return (short)int16;
        }

        public long GetLong()
        {
            long int64 = BitConverter.ToInt64(this._data, this._position);
            this._position += 8;
            return int64;
        }

        public ulong GetULong()
        {
            long uint64 = (long)BitConverter.ToUInt64(this._data, this._position);
            this._position += 8;
            return (ulong)uint64;
        }

        public int GetInt()
        {
            int int32 = BitConverter.ToInt32(this._data, this._position);
            this._position += 4;
            return int32;
        }

        public uint GetUInt()
        {
            int uint32 = (int)BitConverter.ToUInt32(this._data, this._position);
            this._position += 4;
            return (uint)uint32;
        }

        public float GetFloat()
        {
            double single = (double)BitConverter.ToSingle(this._data, this._position);
            this._position += 4;
            return (float)single;
        }

        public double GetDouble()
        {
            double num = BitConverter.ToDouble(this._data, this._position);
            this._position += 8;
            return num;
        }

        public string GetString(int maxLength)
        {
            int count = this.GetInt();
            if (count <= 0 || count > maxLength * 2 || Encoding.UTF8.GetCharCount(this._data, this._position, count) > maxLength)
                return string.Empty;
            string str = Encoding.UTF8.GetString(this._data, this._position, count);
            this._position += count;
            return str;
        }

        public string GetString()
        {
            int count = this.GetInt();
            if (count <= 0)
                return string.Empty;
            string str = Encoding.UTF8.GetString(this._data, this._position, count);
            this._position += count;
            return str;
        }

        public byte[] GetRemainingBytes()
        {
            byte[] numArray = new byte[this.AvailableBytes];
            Buffer.BlockCopy((Array)this._data, this._position, (Array)numArray, 0, this.AvailableBytes);
            this._position = this._data.Length;
            return numArray;
        }

        public void GetBytes(byte[] destination, int start, int count)
        {
            Buffer.BlockCopy((Array)this._data, this._position, (Array)destination, start, count);
            this._position += count;
        }

        public void GetBytes(byte[] destination, int count)
        {
            Buffer.BlockCopy((Array)this._data, this._position, (Array)destination, 0, count);
            this._position += count;
        }

        public byte[] GetBytesWithLength()
        {
            int count = this.GetInt();
            byte[] numArray = new byte[count];
            Buffer.BlockCopy((Array)this._data, this._position, (Array)numArray, 0, count);
            this._position += count;
            return numArray;
        }

        public byte PeekByte()
        {
            return this._data[this._position];
        }

        public sbyte PeekSByte()
        {
            return (sbyte)this._data[this._position];
        }

        public bool PeekBool()
        {
            return this._data[this._position] > (byte)0;
        }

        public char PeekChar()
        {
            return BitConverter.ToChar(this._data, this._position);
        }

        public ushort PeekUShort()
        {
            return BitConverter.ToUInt16(this._data, this._position);
        }

        public short PeekShort()
        {
            return BitConverter.ToInt16(this._data, this._position);
        }

        public long PeekLong()
        {
            return BitConverter.ToInt64(this._data, this._position);
        }

        public ulong PeekULong()
        {
            return BitConverter.ToUInt64(this._data, this._position);
        }

        public int PeekInt()
        {
            return BitConverter.ToInt32(this._data, this._position);
        }

        public uint PeekUInt()
        {
            return BitConverter.ToUInt32(this._data, this._position);
        }

        public float PeekFloat()
        {
            return BitConverter.ToSingle(this._data, this._position);
        }

        public double PeekDouble()
        {
            return BitConverter.ToDouble(this._data, this._position);
        }

        public string PeekString(int maxLength)
        {
            int int32 = BitConverter.ToInt32(this._data, this._position);
            return int32 <= 0 || int32 > maxLength * 2 || Encoding.UTF8.GetCharCount(this._data, this._position + 4, int32) > maxLength ? string.Empty : Encoding.UTF8.GetString(this._data, this._position + 4, int32);
        }

        public string PeekString()
        {
            int int32 = BitConverter.ToInt32(this._data, this._position);
            return int32 <= 0 ? string.Empty : Encoding.UTF8.GetString(this._data, this._position + 4, int32);
        }

        public bool TryGetByte(out byte result)
        {
            if (this.AvailableBytes >= 1)
            {
                result = this.GetByte();
                return true;
            }
            result = (byte)0;
            return false;
        }

        public bool TryGetSByte(out sbyte result)
        {
            if (this.AvailableBytes >= 1)
            {
                result = this.GetSByte();
                return true;
            }
            result = (sbyte)0;
            return false;
        }

        public bool TryGetBool(out bool result)
        {
            if (this.AvailableBytes >= 1)
            {
                result = this.GetBool();
                return true;
            }
            result = false;
            return false;
        }

        public bool TryGetChar(out char result)
        {
            if (this.AvailableBytes >= 2)
            {
                result = this.GetChar();
                return true;
            }
            result = char.MinValue;
            return false;
        }

        public bool TryGetShort(out short result)
        {
            if (this.AvailableBytes >= 2)
            {
                result = this.GetShort();
                return true;
            }
            result = (short)0;
            return false;
        }

        public bool TryGetUShort(out ushort result)
        {
            if (this.AvailableBytes >= 2)
            {
                result = this.GetUShort();
                return true;
            }
            result = (ushort)0;
            return false;
        }

        public bool TryGetInt(out int result)
        {
            if (this.AvailableBytes >= 4)
            {
                result = this.GetInt();
                return true;
            }
            result = 0;
            return false;
        }

        public bool TryGetUInt(out uint result)
        {
            if (this.AvailableBytes >= 4)
            {
                result = this.GetUInt();
                return true;
            }
            result = 0U;
            return false;
        }

        public bool TryGetLong(out long result)
        {
            if (this.AvailableBytes >= 8)
            {
                result = this.GetLong();
                return true;
            }
            result = 0L;
            return false;
        }

        public bool TryGetULong(out ulong result)
        {
            if (this.AvailableBytes >= 8)
            {
                result = this.GetULong();
                return true;
            }
            result = 0UL;
            return false;
        }

        public bool TryGetFloat(out float result)
        {
            if (this.AvailableBytes >= 4)
            {
                result = this.GetFloat();
                return true;
            }
            result = 0.0f;
            return false;
        }

        public bool TryGetDouble(out double result)
        {
            if (this.AvailableBytes >= 8)
            {
                result = this.GetDouble();
                return true;
            }
            result = 0.0;
            return false;
        }

        public bool TryGetString(out string result)
        {
            if (this.AvailableBytes >= 4 && this.AvailableBytes >= this.PeekInt() + 4)
            {
                result = this.GetString();
                return true;
            }
            result = (string)null;
            return false;
        }

        public bool TryGetStringArray(out string[] result)
        {
            ushort result1;
            if (!this.TryGetUShort(out result1))
            {
                result = (string[])null;
                return false;
            }
            result = new string[(int)result1];
            for (int index = 0; index < (int)result1; ++index)
            {
                if (!this.TryGetString(out result[index]))
                {
                    result = (string[])null;
                    return false;
                }
            }
            return true;
        }

        public bool TryGetBytesWithLength(out byte[] result)
        {
            if (this.AvailableBytes >= 4 && this.AvailableBytes >= this.PeekInt() + 4)
            {
                result = this.GetBytesWithLength();
                return true;
            }
            result = (byte[])null;
            return false;
        }

        public void Clear()
        {
            this._position = 0;
            this._dataSize = 0;
            this._data = (byte[])null;
        }
    }
}
