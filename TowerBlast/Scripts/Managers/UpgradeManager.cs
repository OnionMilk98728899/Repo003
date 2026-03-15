using Godot;
using System;
using Game.Upgrades;
using System.Collections.Generic;

public partial class UpgradeManager : Node2D
{
	public static UpgradeManager Instance { get; private set; }
	private List<Upgrade> _upgradePool;

	public override void _Ready()
	{
		Instance = this;
		_upgradePool = LoadUpgrades("res://Data/UpgradeData.json");
	}

	private List<Upgrade> LoadUpgrades(string path)
	{
		var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
		string jsonText = file.GetAsText();

		var parsed = Json.ParseString(jsonText);
		var root = parsed.AsGodotDictionary();
		var upgradesArray = root["upgrades"].AsGodotArray();

		var result = new List<Upgrade>();

		foreach (Godot.Collections.Dictionary entry in upgradesArray)
		{
			result.Add(Convert(entry));
		}

		return result;
	}

private Upgrade Convert(Godot.Collections.Dictionary json)
{

	var trapArray = json["trapTypes"].AsGodotArray();
	var trapTypes = new List<TrapType>();

	foreach (string t in trapArray)
		trapTypes.Add(Enum.Parse<TrapType>(t));

	var statArray = json["statTypes"].AsGodotArray();
	var statTypes = new List<StatType>();

	foreach (string s in statArray)
		statTypes.Add(Enum.Parse<StatType>(s));


	var weaponArray = json["weaponTypes"].AsGodotArray();
	var weaponTypes = new List<WeaponType>();

	foreach (string w in weaponArray)
		weaponTypes.Add(Enum.Parse<WeaponType>(w));

	return new Upgrade
	{
		id = (string)json["id"],
		description = (string)json["description"],
		category = Enum.Parse<Category>((string)json["category"]),

		statTypes = statTypes.ToArray(),
		weaponTypes = weaponTypes.ToArray(),
		trapTypes = trapTypes.ToArray(),

		//trapType = Enum.Parse<TrapType>((string)json["trapType"]),
		op = Enum.Parse<Operation>((string)json["op"]),
		value = (float)(double)json["value"]
	};
}

	public List<Upgrade> GetUpgrades()
	{
		return _upgradePool;
	}
}
