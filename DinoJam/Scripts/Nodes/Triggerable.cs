using Godot;
using System;

public partial class Triggerable : Node2D
{
    private enum state { on, off }
    [Export] private state currentState;
    [Export] private Sprite2D switchSprite;
    [Export] private AnimationPlayer switchAnim;
    [Export] private Timer activateTimer, doorTimer;
    [Export] private int switchId;
    private Player myPlayer;
    private Edible myEdible;
    [Export] private bool isOn;

    public override void _Ready()
    {
        if (isOn)
        {
            switchSprite.Frame = 5;
        }
        else
        {
            switchSprite.Frame = 0;
        }
    }

    private void OnDetectorBodyEntered(Node2D body)
    {
        if (body is Player player)
        {
            myPlayer = player;
            if(myPlayer.GetSpecialState() == Player.specialState.charge || myPlayer.GetSpecialState() == Player.specialState.stomp ||
            myPlayer.GetSpecialState() == Player.specialState.stompland || myPlayer.GetSpecialState() == Player.specialState.chargeland)
            {
                ActivateSwitch();
            }
            

        }else if(body is Edible edible)
        {
            myEdible = edible;
            ActivateSwitch();
        }

    }

    private void ActivateSwitch()
    {
        if (isOn)
        {
            isOn = false;
            switchAnim.Play("switchoff");
        }
        else
        {
            isOn = true;
            switchAnim.Play("switchon");
        }
        EventBus.Instance.EmitSignal(EventBus.SignalName.ActivateSwitchDoor, switchId, isOn);
        AudioManager.Instance.PlaySFX(AudioManager.Instance.sfx2Player, AudioManager.Instance.audioLibrary.switchFlip);
    }
}
