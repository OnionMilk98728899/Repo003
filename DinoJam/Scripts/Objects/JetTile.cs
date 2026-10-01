using Godot;
using System;
using System.Collections.Generic;

public enum jetType { lava, water }
public partial class JetTile : StaticBody2D
{
    [Export] private int height;
    [Export] private float onTime, offTime, platformSpeed;
    [Export] private Timer onTimer, offTimer;
    [Export] private PackedScene jetSegmentScene, jetTopScene;
    [Export] public jetType myJetType;
    [Export] private PackedScene myPlatformScene;
    private WaterPlatform myPlatform;
    private JetSegment myJetSegment;
    private StaticBody2D myJetTop;
    //private Vector2 GlobalPosition;
    private List<JetSegment> jetSegments = new List<JetSegment>();
    private bool hasBeenGenerated, isOn, isCollisionEnabled, isSecondCollisionEnabled;

    public override void _Ready()
    {
        offTimer.Start();
        GenerateJets();
        AnimateJet();
        onTimer.WaitTime = onTime;
        offTimer.WaitTime = offTime;
        if (myJetType == jetType.water)
        {
            InitializeWaterPlatform();
        }
        //Engine.TimeScale = .4;
    }

    public override void _PhysicsProcess(double delta)
    {
        ShowHideJet();

        if (myJetTop.Visible && !isCollisionEnabled)
        {
            DisableEnableColliders();
            isCollisionEnabled = true;
        }
        if (!myJetTop.Visible && isCollisionEnabled)
        {
            DisableEnableColliders();
            isCollisionEnabled = false;
        }

    }
    private void GenerateJets()
    {
        if (!hasBeenGenerated)
        {
            for (int i = 0; i < height; i++)
            {
                myJetSegment = jetSegmentScene.Instantiate<JetSegment>();
                AddChild(myJetSegment);
                Vector2 position = new Vector2(GlobalPosition.X, GlobalPosition.Y - (i * 16) - 16);
                myJetSegment.GlobalPosition = position;
                myJetSegment.myJetType = myJetType;
                jetSegments.Add(myJetSegment);
            }

            myJetTop = jetTopScene.Instantiate<StaticBody2D>();
            AddChild(myJetTop);
            Vector2 pos = new Vector2(GlobalPosition.X, GlobalPosition.Y - height * 16);
            myJetTop.GlobalPosition = pos;
            hasBeenGenerated = true;
        }
    }

    private void ShowHideJet()
    {
        if (!onTimer.IsStopped())
        {

            if (onTimer.WaitTime - onTimer.TimeLeft <= .1)
            {
                jetSegments[0].Visible = true;
                Vector2 pos = new Vector2(GlobalPosition.X, GlobalPosition.Y - 16);
                myJetTop.GlobalPosition = pos;
                myJetTop.Visible = true;

            }
            else
            {
                foreach (JetSegment segment in jetSegments)
                {
                    segment.Visible = true;
                }
                if (!isSecondCollisionEnabled)
                {
                    DisableEnableColliders();
                    isSecondCollisionEnabled = true;
                }
                Vector2 pos = new Vector2(GlobalPosition.X, GlobalPosition.Y - height * 16);
                myJetTop.GlobalPosition = pos;
            }
        }
        if (!offTimer.IsStopped())
        {
            if (offTimer.WaitTime - offTimer.TimeLeft <= .1)
            {
                int index = 0;
                foreach (JetSegment segment in jetSegments)
                {
                    if (index > 0)
                    {

                        segment.Visible = false;

                    }
                    index++;
                }
                if (isSecondCollisionEnabled)
                {
                    DisableEnableColliders();
                    isSecondCollisionEnabled = false;
                }

                Vector2 pos = new Vector2(GlobalPosition.X, GlobalPosition.Y - 16);
                myJetTop.GlobalPosition = pos;
                DisableEnableColliders();
            }
            else
            {
                foreach (JetSegment segment in jetSegments)
                {
                    segment.Visible = false;
                }

                myJetTop.Visible = false;
            }
        }
    }

    private void DisableEnableColliders()
    {
        foreach (JetSegment segment in jetSegments)
        {
            if (segment.Visible)
            {
                segment.DisableEnableCollider(false);
            }
            else
            {
                segment.DisableEnableCollider(true);
            }

        }

        foreach (Node node in myJetTop.GetChildren())
        {
            if (node is CollisionShape2D collider)
            {
                if (myJetTop.Visible)
                {
                    collider.Disabled = false;
                }
                else
                {
                    collider.Disabled = true;
                }

            }
        }
    }

    private void AnimateJet()
    {
        foreach (JetSegment segment in jetSegments)
        {
            if (myJetType == jetType.lava)
            {
                segment.AnimateJet("move");
            }
            else
            {
                segment.AnimateJet("watermove");
            }
        }

        foreach (Node node in myJetTop.GetChildren())
        {
            if (node is AnimatedSprite2D sprite)
            {
                if (myJetType == jetType.lava)
                {
                    sprite.Play("move");
                }
                else
                {
                    sprite.Play("watermove");
                }
            }
        }
    }

    private void InitializeWaterPlatform()
    {
        myPlatform = myPlatformScene.Instantiate<WaterPlatform>();
        AddChild(myPlatform);
        myPlatform.GlobalPosition = GlobalPosition;
        Vector2 peakPosition = new Vector2(GlobalPosition.X, GlobalPosition.Y - 16 * height - 8);
        myPlatform.InitializeValues(platformSpeed, peakPosition);

    }
    private void OnTimerTimeout()
    {
        offTimer.Start();
        isOn = false;
        if (myJetType == jetType.water)
        {
            myPlatform.OnJetOff();
        }
    }

    private void OffTimerTimeout()
    {
        onTimer.Start();
        isOn = true;
        if (myJetType == jetType.water)
        {
            myPlatform.OnJetOn();
        }
    }

}
