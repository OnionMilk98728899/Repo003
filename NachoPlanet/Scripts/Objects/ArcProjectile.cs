using Godot;
using System;

public partial class ArcProjectile : Node2D
{
	
	 [Export] public float Speed           = 350f;   // horizontal travel speed (px/s)
	[Export] public float InitialHeight   = 80f;    // peak "height" of 1st bounce (px)
	[Export] public float BounceDuration  = 0.45f;  // seconds per full arc
	[Export] public float HeightDecay     = 0.50f;  // each bounce reaches this fraction of previous
	[Export] public float DurationDecay   = 0.75f;  // each bounce is shorter in time
	[Export] public float ScalePerHeight  = 0.012f; // scale multiplier per pixel of height
	[Export] public float YOffsetPerHeight= 0.6f;   // vertical screen offset per pixel of height
	[Export] public int   MaxBounces      = 3;

	// ─── Node References ──────────────────────────────────────────────
	private Sprite2D _body;
	private Sprite2D _shadow;

	// ─── Runtime State ────────────────────────────────────────────────
	private Vector2 _direction;
	private float   _currentMaxHeight;
	private float   _currentBounceDuration;
	private float   _bounceTimer;
	private int     _bounceCount;
	private bool    _dead;

	// ─── Initialization (call after spawning) ─────────────────────────
	public void Launch(Vector2 direction)
	{
		_direction             = direction.Normalized();
		_currentMaxHeight      = InitialHeight;
		_currentBounceDuration = BounceDuration;
		_bounceTimer           = 0f;
		_bounceCount           = 0;
		_dead                  = false;
	}

	public override void _Ready()
	{
		_body   = GetNode<Sprite2D>("Body");
		_shadow = GetNode<Sprite2D>("Shadow");

		// Default fallback if Launch() wasn't called (editor preview, etc.)
		if (_direction == Vector2.Zero)
			_direction = Vector2.Right;

		_currentMaxHeight      = InitialHeight;
		_currentBounceDuration = BounceDuration;
	}

	public override void _Process(double delta)
	{
		if (_dead) return;

		float dt = (float)delta;

		// ── Horizontal movement (ground plane) ────────────────────────
		Position += _direction * Speed * dt;

		// ── Advance arc timer ─────────────────────────────────────────
		_bounceTimer += dt;

		if (_bounceTimer >= _currentBounceDuration)
		{
			_bounceTimer -= _currentBounceDuration;
			_bounceCount++;

			if (_bounceCount >= MaxBounces)
			{
				Die();
				return;
			}

			// Shrink the next bounce
			_currentMaxHeight      *= HeightDecay;
			_currentBounceDuration *= DurationDecay;

			OnBounce(); // hook for particles, sound, etc.
		}

		// ── Compute current height (parabolic arc) ────────────────────
		//   t ∈ [0, 1];  h = 4·H·t·(1−t)  →  peak H at t = 0.5
		float t      = _bounceTimer / _currentBounceDuration;
		float height = _currentMaxHeight * 4f * t * (1f - t);

		// ── Apply visuals ─────────────────────────────────────────────
		float scaleFactor = 1f + height * ScalePerHeight;

		// Body rises on-screen & scales up
		_body.Position = new Vector2(0f, -height * YOffsetPerHeight);
		_body.Scale    = new Vector2(scaleFactor, scaleFactor);

		// Shadow stays on the ground and shrinks as projectile rises
		float shadowScale = Mathf.Lerp(1f, 0.4f, height / Mathf.Max(_currentMaxHeight, 1f));
		_shadow.Position = Vector2.Zero;
		_shadow.Scale    = new Vector2(shadowScale, shadowScale);
		_shadow.Modulate = new Color(0f, 0f, 0f, Mathf.Lerp(0.45f, 0.15f, height / Mathf.Max(_currentMaxHeight, 1f)));
	}

	// ─── Hooks ────────────────────────────────────────────────────────
	private void OnBounce()
	{
		// Spawn dust particles, play a "boing" sound, etc.
		// Example:
		// GetTree().CreateTimer(0).Timeout += () => { /* particles */ };
	}

	private void Die()
	{
		_dead = true;
		// Optional: play a small impact effect at final position
		QueueFree();
	}

	// ─── Hit Detection (connect Area2D signals in editor or code) ─────
	public void OnHitboxBodyEntered(Node2D body)
	{
		// Filter out other projectiles, walls, etc.
		if (body is ArcProjectile) return;

		// Deal damage, apply status, etc.
		Die();
	}
}
