using Godot;
using System;

public partial class GUI : Control
{
	[Export] private Sprite2D[] healthBarSprites;
	[Export] private Label healthLabel;


	private int currentHealth, maxHealth;

	private void SubtractHealthFromHealthBar(int incomingDmg){

		currentHealth -= incomingDmg;
		healthLabel.Text = currentHealth.ToString() + "/" +maxHealth.ToString();
		foreach(Sprite2D healthBar in healthBarSprites){
		   // healthBar.Scale = ;

		   float xBarLength = (float)currentHealth/100;

			healthBar.Scale = new Vector2(xBarLength, 1);
		}
	}

	private void OnPlayerHurt(int damagePts){
		SubtractHealthFromHealthBar(damagePts);
	}
}
