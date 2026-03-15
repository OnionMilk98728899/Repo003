using System.Collections.Generic;

public class UpgradeDatabase
{
    public List<UpgradeJson> upgrades;
}

public class UpgradeJson
{
    public string id;
    public string description;
    public string category;
    //public string statType;
    public string[] statTypes;
    //public string trapType;
    public string[] trapTypes;
    //public string weaponType;
    public string[] weaponTypes;
    public string op;
    public float value;
    public int cost;
}