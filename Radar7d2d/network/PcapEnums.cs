
using System;
using System.IO;
using System.Numerics;

namespace pcap_7day
{

    public enum SpawnTypeIndex : int
    {
        Undefined,
        Cars,
        NPC,
        SupplyCrates,
        Backpack,        
        Trader,
        Animals,
        Enemies,
        Players
    }

    public enum ProgressIndex : int
    {
        EntityParse,
        EntityAdd,
        EntityName,
        EntityRemove,
        EntityPlayer,
        TileMap,
        Error
    }
    public enum EnumDamageTypes
    {
        None,
        Piercing,
        Slashing,
        Bashing,
        Crushing,
        Corrosive,
        Heat,
        Cold,
        Radiation,
        Toxic,
        Electrical,
        Disease,
        Infection,
        Starvation,
        Dehydration,
        Falling,
        Suffocation,
        BloodLoss,
        Sprain,
        Break,
        Stun,
        Concuss,
        KnockOut,
        BlackOut,
        KnockDown,
        BarbedWire,
        Suicide,
        VehicleInside,
        COUNT,
    }

    public enum EnumEquipmentSlotGroup
    {
        Head,
        UpperBody,
        LowerBody,
    }

    public enum EnumEquipmentSlot
    {
        Head = 0,
        Eyes = 1,
        Face = 2,
        Chest = 3,
        Hands = 4,
        Legs = 5,
        Feet = 6,
        Back = 7,
        Count = 8,
        None = 8,
    }

    public struct EntityInfo
    {
        public bool init;
        public Vector3 Position;
        public Vector3 Rotation;

        public int attach;
        public SpawnTypeIndex type;

        // player
        public bool Listed;
        public float MaxHealth;
        public float Health;
        public string Name;   

        public void Write(BinaryWriter binary)
        {
            binary.Write((byte)type);

            binary.Write((int)Position.X);
            binary.Write((int)Position.Y);
            binary.Write((int)Position.Z);

            switch (type)
            {
                case SpawnTypeIndex.Players:
                    binary.Write((int)Health);
                    binary.Write((int)MaxHealth);
                    binary.Write(Name);
                    break;
            }            
        }

    }

    public struct BotInfo
    {
        public Vector3i Position;
        public DateTime time;
        public int attach;

        public void Write(BinaryWriter binary)
        {
            binary.Write(Position.x);
            binary.Write(Position.y);
            binary.Write(Position.z);
        }

    }
}
