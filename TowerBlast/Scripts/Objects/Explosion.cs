using Game.Damage;
using Godot;
using System;


public partial class Explosion : Node2D
{
	[Export] private Texture2D _basicExplosion, _bigExplosion;
	[Export] private AnimationPlayer _explodeAnim;
	private Damage _damages;
	private int _damage;
	//private MyGlobalResources._damageType _myDamageType;

	public void SetDamage(Damage damages)
	{
		_damages = damages;
	}
	public Damage GetDamage()
	{
		return _damages;
	}

	public void Explode()
	{
		_explodeAnim.Play("Explode");
	}

	public void SnapToNearestFloor()
	{
		int trueYVal =  (int)MathF.Round(GlobalPosition.Y/16);
		GlobalPosition = new Vector2(GlobalPosition.X, (float) trueYVal * 16);
	}
}
