namespace Game.Upgrades
{
    public struct Upgrade
    {
        public string id;
        public string description;
        public Operation op;
        public Category category;
        //public StatType statType;
        public StatType[] statTypes;
        //public TrapType trapType;
        public TrapType[] trapTypes;
       // public WeaponType weaponType;
        public WeaponType[] weaponTypes;
        public float value;
        public int cost;
    }

    public enum Operation
    {
        buy, add, multiply
    }

    public enum Category
    {
        trap, weapon, trapAndWeapon, player
    }

    public enum StatType
    {
        baseDamage, magicDamage, fireDamage, iceDamage, poisonDamage, elecDamage, armorPiercing, crushingHit, criticalHit, 
        penetration, stun, range, durability, explode, treasure, moveSpeed, recoveryTime, coolDownTime, maxHealth, buyWeapon
    }

    public enum TrapType
    {
        ballista, spear, shuriken, cannon, bigcannon, tricannon, magic, fireorb,
	 	iceorb, lightningorb, poisonorb, spikes, firetile, icetile, poisontile,
	 	lightningtile, swarm, bugswarm, ratswarm, treasure, warpdoor, none
    }

    public enum WeaponType
    {
        sword, hammer, axe, dagger, spear, club, bomb, scepter, bow, shield, potion, shuriken, none
    }

}