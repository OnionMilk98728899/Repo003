using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class HealthMeter : Node2D
{
	//[Signal] public delegate void GetHealthAndArmorEventHandler(int maxHealth, int);
	[Export] private TextureProgressBar _healthBar, _armorBar;
	private int _currentHealth, _maxHealth, _currentArmor, _maxArmor;

	public override void _Ready()
	{

	}

	private void OnDamageTaken(int damage, int armorDamage)
	{
		_currentHealth -= damage;
		OffsetHealthProgressBar(_healthBar, _currentHealth, _maxHealth);
		if (_currentArmor > 0)
		{
			_currentArmor -= armorDamage;
			OffsetHealthProgressBar(_armorBar, _currentArmor, _maxArmor);
		}
		if(_currentArmor <= 0)
		{
			_armorBar.Visible = false;
		}
	}

	private void OffsetHealthProgressBar(TextureProgressBar myBar, int currentHealth, int maxHealth)
	{
		float barValue = currentHealth / 3.3f + maxHealth * .35f;

		myBar.Value = barValue;
	}

	private void OnSetHealthAndArmor(int maxHealth, int maxArmor)
	{
		_maxHealth = maxHealth;
		_currentHealth = _maxHealth;
		_maxArmor = maxArmor;
		_currentArmor = maxArmor;

		OffsetHealthProgressBar(_healthBar, _currentHealth, _maxHealth);
		if (_currentArmor > 0)
		{
			OffsetHealthProgressBar(_armorBar, _currentArmor, _maxArmor);
		}
		else
		{
			_armorBar.Visible = false;
		}
	}
}
