using Godot;
using System;

public partial class Tree : StaticBody2D
{
    [Export] private PackedScene treeTopScene, trunkScene;
    [Export] private int height, canopy;
    private Player player;
    private int canopyOffset;
    private AnimatedSprite2D treeTrunk;
    private AnimatedSprite2D[] trunkArray;
    private AnimatedSprite2D treeTop;
    private AnimatedSprite2D[] treeTopArray;
    private Vector2 baseTrunkPosition, baseCanopyPosition;

    private bool hasBeenShaken;


    public override void _Ready()
    {
        EventBus.Instance.ShakeTree += ShakeTree;
        GenerateTree();

        foreach (AnimatedSprite2D trunk in trunkArray)
        {
            trunk.Play("idle");
        }
        foreach (AnimatedSprite2D top in treeTopArray)
        {
            top.Play("idle");
        }
    }

    private void GenerateTree()
    {
        baseTrunkPosition = new Vector2(GlobalPosition.X, GlobalPosition.Y - 8);
        trunkArray = new AnimatedSprite2D[height];
        treeTopArray = new AnimatedSprite2D[canopy * 2 + 1];

        for (int i = 0; i < height; i++)
        {
            treeTrunk = trunkScene.Instantiate<AnimatedSprite2D>();
            AddChild(treeTrunk);
            trunkArray[i] = treeTrunk;
            treeTrunk.GlobalPosition = new Vector2(baseTrunkPosition.X, baseTrunkPosition.Y - (16 * i));
            GD.Print("Printed trunk at " + treeTrunk.GlobalPosition);
        }

        treeTop = treeTopScene.Instantiate<AnimatedSprite2D>();
        AddChild(treeTop);
        treeTopArray[0] = treeTop;
        treeTop.GlobalPosition = new Vector2(GlobalPosition.X, GlobalPosition.Y - (16 * height)-8);

        for (int i = 1; i <= canopy; i++)
        {
            treeTop = treeTopScene.Instantiate<AnimatedSprite2D>();
            AddChild(treeTop);
            treeTopArray[i * 2 - 1] = treeTop;
            treeTop.GlobalPosition = new Vector2(GlobalPosition.X + (16 * i), GlobalPosition.Y - (16 * height)-8);

            treeTop = treeTopScene.Instantiate<AnimatedSprite2D>();
            AddChild(treeTop);
            treeTopArray[i * 2] = treeTop;
            treeTop.GlobalPosition = new Vector2(GlobalPosition.X - (16 * i), GlobalPosition.Y - (16 * height)-8);
        }
    }

    private void ShakeTree(Tree tree)
    {
        if (tree == this && !hasBeenShaken)
        {
            foreach (AnimatedSprite2D trunk in trunkArray)
            {
                trunk.Play("shake");
            }
            foreach (AnimatedSprite2D top in treeTopArray)
            {
                top.Play("shake");
            }

        }
    }

    private void OnChargeDetectorEntered(Node2D body)
    {
        if (body.IsInGroup("Player"))
        {
            player = body.GetNode<Player>(".");
            player.SetTouchingTreeChargeDetector(true, this);

        }
    }

    private void OnChargeDetectorExited(Node2D body)
    {
        if (body.IsInGroup("Player"))
        {
            player = body.GetNode<Player>(".");
            player.SetTouchingTreeChargeDetector(false, this);
        }
    }

    private void OnStompDetectorEntered(Node2D body)
    {
        if (body.IsInGroup("Player"))
        {
            player = body.GetNode<Player>(".");
            player.SetTouchingTreeStompDetector(true, this);
        }
    }

    private void OnStompDetectorExited(Node2D body)
    {
        if (body.IsInGroup("Player"))
        {
            player = body.GetNode<Player>(".");
            player.SetTouchingTreeStompDetector(false, this);
        }
    }

}
