using Godot;
using System;

public partial class HitEffect : Node2D
{
	[Export] private AnimationPlayer hitAnim;

	public void PlayHitEffect(Vector2 myPosition, float hitDegrees){


		GlobalPosition = myPosition;

		if(hitDegrees >=-22.5 && hitDegrees <22.5){     ///Right Facing Position

			hitAnim.Play("Hit_Right");

		}else if(hitDegrees >=-67.5 && hitDegrees <-22.5){   ///Right-up Facing Position

			hitAnim.Play("Hit_Up_Right_Down_Left");

		}else if(hitDegrees >=-112.5 && hitDegrees <-67.5){     ///Upward Facing Position

			hitAnim.Play("Hit_Up_Down");

		}else if(hitDegrees >= -157.5 && hitDegrees <-112.5){    ///Left-up Facing Position

			hitAnim.Play("Hit_Up_Left_Down_Right");

		}else if(hitDegrees >=157.5 || hitDegrees <-157.5){     ///Left Facing Position

			hitAnim.Play("Hit_Left");

		}else if(hitDegrees >=112.5 && hitDegrees <157.5){   ///Left-down Facing Position

			hitAnim.Play("Hit_Up_Right_Down_Left");

		}else if(hitDegrees >=67.5 && hitDegrees <112.5){     ///Downward Facing Position

			hitAnim.Play("Hit_Up_Down");

		}else if(hitDegrees >= 22.5 && hitDegrees <67.5){    ///Right-down Facing Position

			hitAnim.Play("Hit_Up_Left_Down_Right");

		}

		
	}
}
