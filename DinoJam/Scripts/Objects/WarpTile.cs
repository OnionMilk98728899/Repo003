using Godot;
using System;

public partial class WarpTile : StaticBody2D
{
    [Export] private Texture2D leftDoorTexture, rightDoorTexture, tubeTexture;
    [Export] private Sprite2D doorSprite;
    [Export] private AnimationPlayer doorAnim;
    [Export] private CollisionShape2D floorCollider;
    public enum doorType{left, right, tube}
    [Export] public doorType myDoorType;
    [Export] public WarpTile partnerTile;
    private Player myPlayer;
    private Vector2 inputKey;

    public override void _Ready()
    {
        switch (myDoorType)
        {
            case doorType.left:
            doorSprite.Texture = leftDoorTexture;
            inputKey = new Vector2(0,-1);
            break;
            case doorType.right:
            doorSprite.Texture = rightDoorTexture;
            inputKey = new Vector2(0,-1);
            break;
            case doorType.tube:
             doorSprite.Texture = tubeTexture;
             doorSprite.Hframes = 8;
            inputKey = new Vector2(0,1);
            floorCollider.Disabled = false;
            break;
        }
    }


    private void OnDoorAreaEntered(Node2D body)
    {
        if (body.IsInGroup("Player"))
        {
            myPlayer = body.GetNode<Player>(".");
            myPlayer.isTouchingDoor = true;
            myPlayer.doorEntryInput = inputKey;
            myPlayer.SetCurrentWarpTiles(this, partnerTile);
        }
    }

    private void OnDoorAreaExited(Node2D body)
    {
        if (body.IsInGroup("Player"))
        {
            myPlayer.isTouchingDoor = false;
        }
    }

    public void WarpPlayer()
    {
        myPlayer.GlobalPosition = partnerTile.GlobalPosition;
        if(myDoorType == doorType.tube)
        {
            doorAnim.Play("enter");
        }
    }
}
