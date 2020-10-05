
using System;
using System.IO;

namespace pcap_7day
{
    public struct NetPackageDamageEntity : INetPackage
    {
        public void ReadBinary(BinaryReader reader, Network7d2d network, bool outcoming)
        {
            //NetPackageDamageEntity
            var _entityId = reader.ReadInt32();
            var _damageSrc = reader.ReadByte();
            var _damageType = (EnumDamageTypes)reader.ReadByte();

            var _strength = reader.ReadUInt16();
            var _hitDirection = (int)reader.ReadByte();
            var _hitBodyPart = (int)reader.ReadByte();
            var _movementState = (int)reader.ReadByte();
            var _bPainHit = reader.ReadBoolean();
            var _bFatal = reader.ReadBoolean();
            var _bCritical = reader.ReadBoolean();
            var _attackerEntityId = reader.ReadInt32();

            var _dirX = reader.ReadSingle();
            var _dirY = reader.ReadSingle();
            var _dirZ = reader.ReadSingle();
            var _hitTransformName = reader.ReadString();

            var hitTransformPosition_x = reader.ReadSingle();
            var hitTransformPosition_y = reader.ReadSingle();
            var hitTransformPosition_z = reader.ReadSingle();

            var uvHit_x = reader.ReadSingle();
            var uvHit_y = reader.ReadSingle();
            var damageMultiplier = reader.ReadSingle();
            var random = reader.ReadSingle();
            var bIgnoreConsecutiveDamages = reader.ReadBoolean();
            var bIsDamageTransfer = reader.ReadBoolean();
            var bDismember = reader.ReadBoolean();
            var bCrippleLegs = reader.ReadBoolean();
            var bTurnIntoCrawler = reader.ReadBoolean();
            var bonusDamageType = reader.ReadByte();
            var StunType = reader.ReadByte();
            var StunDuration = reader.ReadSingle();
            var ArmorSlot = (EnumEquipmentSlot)reader.ReadByte();
            var ArmorSlotGroup = (EnumEquipmentSlotGroup)reader.ReadByte();
            var ArmorDamage = (int)reader.ReadInt16();

            if(network.Entities.ContainsKey(_entityId))
            {
                var _user = network.Entities[_entityId];
                var _health = _user.Health - ArmorDamage;

                if (network.Entities.ContainsKey(_attackerEntityId))
                {
                    var _attacker = network.Entities[_attackerEntityId];
                    //Console.WriteLine($"NetPackageDamageEntity >> {_attacker.Name} >> {_user.Name} >> {ArmorDamage}");
                }
            }
            
        }
    }
}
