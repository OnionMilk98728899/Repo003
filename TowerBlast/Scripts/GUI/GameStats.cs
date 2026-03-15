using Godot;
using System;

public partial class GameStats : Control
{
	[Export] private RichTextLabel _goldCount;
	[Export] private TextureProgressBar _weapon1CooldownBar, _weapon2CooldownBar;
	[Export] private Sprite2D _weapon1IconSprite, _weapon2IconSprite;
	[Export] private Texture2D[] _weaponIcons;
	[Export] private Timer _weapon1CooldownTimer, _weapon2CooldownTimer;
 	private float _weapon1CoolDownValue, _weapon2CooldownValue;

	public override void _Ready()
	{
		GlobalSignals.Instance.PlayerAttack += OnAttackTriggered;
	}


	public override void _PhysicsProcess(double delta)
	{
		_goldCount.Text = MyGlobalResources._playerGoldQuantity.ToString();

		_weapon1CooldownBar.Value = _weapon1CooldownTimer.TimeLeft/_weapon1CooldownTimer.WaitTime*100;
		_weapon2CooldownBar.Value = _weapon2CooldownTimer.TimeLeft/_weapon2CooldownTimer.WaitTime*100;
	}

	// private void OffsetHealthProgressBar(TextureProgressBar myBar, int currentValue, int maxValue)
	// {
	// 	float barValue = currentValue / 3.3f + maxValue * .35f;

	// 	myBar.Value = barValue;
	// }

	public void SetWeaponIcon(int weaponIndex, int iconIndex)
	{
		if(weaponIndex == 1)
		{
			_weapon1IconSprite.Texture = _weaponIcons[iconIndex];
			GD.Print("Icon Texture Set!");
		}
		if(weaponIndex == 2)
		{
			_weapon2IconSprite.Texture = _weaponIcons[iconIndex];
			GD.Print("Icon Texture Set!");
		}
	}

	private void OnAttackTriggered(bool isWeapon1)
	{
		if (isWeapon1)
		{
			_weapon1CooldownTimer.Start();
			GD.Print("Attak!");
		}
		else
		{
			_weapon2CooldownTimer.Start();
		}
	}

}
