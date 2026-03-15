using Game.Upgrades;

namespace Game.Damage
{
    public struct Damage
    {
        public int baseDamage, magicDamage, fireDamage, iceDamage, poisonDamage, elecDamage, movePenalty, penetration, stun, explode;
        public float armorPiercing, crushingHit, criticalHit, damageTime, range, durability;
    }

    public enum DamageType
    {
        physical, magic, fire, ice, poison, electric
    }
}