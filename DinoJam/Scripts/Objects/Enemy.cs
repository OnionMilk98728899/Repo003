using Godot;
using System;
using System.Collections.Generic;

public enum enemyMoveState { move, emerge, emergeprep, prepare, stun, attack, jump, hurt, dying, flee }
public enum enemyDeathType { splat, spin }
public partial class Enemy : CharacterBody2D
{
    [Export] public EnemyAttacker attacker;
    [Export] public EnemyDeath death;
    [Export] public float attackDelay, gravity, maxGravity;
    [Export] private float bounceBack, aggression;
    [Export] private HealthComponent healthComp;
    [Export] public AnimationPlayer enemyAnim;
    [Export] public Sprite2D enemySprite;
    [Export] private Timer hurtTimer, attackDelayTimer, deathTimer, prepareTimer;
    public ulong enemyId { get; private set; }
    public enemyMoveState currentMoveState;
    public enemyDeathType currentDeathType;
    public Player myPlayer;
    public Vector2 enemyVelocity, deathVelocity;
    public bool isBounced;
    //private List<HurtBox> hurtBoxes = new();
    [Export] private HurtBox[] hurtBoxes;
    public override void _Ready()
    {
        EventBus.Instance.HurtEnemy += OnEnemyHurt;
        EventBus.Instance.ChargeEnemy += OnEnemyCharged;
        EventBus.Instance.KillEnemy += OnEnemyKilled;
        EventBus.Instance.HurtEnemyTimeout += OnHurtEnemyTimeout;

        enemyId = GetInstanceId();
        death.SetGravity(gravity, maxGravity);
        GetHurtboxes();
        GetHealthComponent();
        attackDelayTimer.WaitTime = attackDelay;
    }

    private void GetHurtboxes()
    {
        foreach (HurtBox box in hurtBoxes)
        {
            box.Initialize(this);
        }
    }

    private void GetHealthComponent()
    {
        healthComp.Initialize(this);
    }

    private void OnPlayerDetectorEntered(Node2D body)
    {
        if (body.IsInGroup("Player") && currentMoveState != enemyMoveState.hurt && currentMoveState != enemyMoveState.dying
        && attackDelayTimer.IsStopped())
        {
            myPlayer = body.GetNode<Player>(".");
            PlayerDetected();
        }
    }

    public virtual void PlayerDetected()
    {
        float r = GD.Randf();

        if (r < aggression / 100)
        {
            currentMoveState = enemyMoveState.prepare;
            attackDelayTimer.Start();
            prepareTimer.Start();
        }

    }
    public void StunEnemy()
    {
        if (currentMoveState != enemyMoveState.hurt && currentMoveState != enemyMoveState.dying)
        {
            currentMoveState = enemyMoveState.stun;
        }
    }


    private void OnEnemyHurt(ulong ID, Vector2 position)
    {
        if (enemyId == ID)
        {
            currentMoveState = enemyMoveState.hurt;
            GD.Print("Calling Hurt");
            AudioManager.Instance.PlaySFX(AudioManager.Instance.enemySFX1, AudioManager.Instance.audioLibrary.enemyHurt);
            Vector2 smackPosition = GlobalPosition + ((position - GlobalPosition) / 2);
            EffectsManager.Instance.PlaySmackEffect(smackPosition, 1);
        }
    }

    private void OnHurtEnemyTimeout()
    {
        currentMoveState = enemyMoveState.move;
    }

    private void OnEnemyKilled(ulong ID, Vector2 strikeVelocity, Vector2 position)
    {
        if (enemyId == ID)
        {
            Vector2 newVel = Vector2.Zero;
            if (strikeVelocity.Y == 0)
            {

                newVel = new Vector2(strikeVelocity.X * 3, -300);
            }
            else
            {
                SetCollisionMaskValue(1, false);
                newVel = new Vector2(0, strikeVelocity.Y * 2);
            }
            AudioManager.Instance.PlaySFX(AudioManager.Instance.enemySFX1, AudioManager.Instance.audioLibrary.enemyDie);
            Vector2 smackPosition = GlobalPosition + ((position - GlobalPosition) / 2);
            EffectsManager.Instance.PlaySmackEffect(smackPosition, 2);
            deathVelocity = newVel;
            currentMoveState = enemyMoveState.dying;
            isBounced = true;
            enemyVelocity = deathVelocity;
            deathTimer.Start();
        }
    }

    private void OnEnemyCharged(ulong ID, Vector2 strikeVelocity, Vector2 position)
    {

        if (enemyId == ID)
        {
            currentMoveState = enemyMoveState.hurt;
            if (IsOnFloor())
            {
                if (strikeVelocity.X < 0)
                {
                    enemyVelocity.X = -bounceBack;
                }
                else
                {
                    enemyVelocity.X = bounceBack;
                }
                //enemyVelocity.X
            }
            AudioManager.Instance.PlaySFX(AudioManager.Instance.enemySFX1, AudioManager.Instance.audioLibrary.enemyHurt);
            Vector2 smackPosition = GlobalPosition + ((position - GlobalPosition) / 2);
            EffectsManager.Instance.PlaySmackEffect(smackPosition, 1);
        }
    }

    public virtual void ApplyGravity()
    {
        if (!IsOnFloor())
        {
            if (enemyVelocity.Y < maxGravity)
            {
                enemyVelocity.Y += gravity;
            }
        }
        else
        {
            if (!isBounced && enemyVelocity.Y != 0)
            {
                enemyVelocity.Y = 0;
            }
        }
    }
    private void OnPrepareTimerTimeout()
    {
        if (currentMoveState != enemyMoveState.hurt && currentMoveState != enemyMoveState.dying)
        {
            InitiateAttack();
        }
    }

    public virtual void AnimateEnemy()
    {
        enemyAnim.Play(currentMoveState.ToString());
        if (currentMoveState != enemyMoveState.hurt && currentMoveState != enemyMoveState.dying)
        {
            if (enemyVelocity.X < 0)
            {
                enemySprite.FlipH = false;
            }
            else if (enemyVelocity.X > 0)
            {
                enemySprite.FlipH = true;
            }
        }
    }

    public virtual void InitiateAttack()
    {
        currentMoveState = enemyMoveState.attack;
    }
    private void OnDeathTimerTimeout()
    {
        QueueFree();
    }

}
