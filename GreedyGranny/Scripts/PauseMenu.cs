using Godot;

public partial class PauseMenu : Control
{
	// Assign these in the editor or via code
	[Export] public TextureButton ResumeButton { get; set; }
	[Export] public TextureButton QuitButton  { get; set; }

	public override void _Ready()
	{
		// Ensure buttons can receive keyboard focus
		if (ResumeButton != null) ResumeButton.FocusMode = FocusModeEnum.All;
		ResumeButton.MouseFilter = MouseFilterEnum.Ignore;
		if (QuitButton != null)  QuitButton.FocusMode  = FocusModeEnum.All;

		// Optional: Load textures via code if not assigned in the editor
		// ResumeButton.TextureNormal  = GD.Load<Texture2D>("res://assets/btn_normal.png");
		// ResumeButton.TexturePressed = GD.Load<Texture2D>("res://assets/btn_pressed.png");
		// ResumeButton.TextureFocused = GD.Load<Texture2D>("res://assets/btn_focused.png");
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Input.IsActionJustPressed("Action4"))
		{
			OpenMenu();
		}
	}


	public void OpenMenu()
	{
		Visible = true;
		GetTree().Paused = true;

		// Grab focus on the default button. CallDeferred ensures it runs 
		// after the node is fully visible and processed in the scene tree.
		ResumeButton?.CallDeferred("grab_focus");
	
	}

	public void CloseMenu()
	{
		Visible = false;
		GetTree().Paused = false;
		// Release focus to prevent stuck UI state
		GetTree().Root.GuiGetFocusOwner()?.ReleaseFocus();
	}

	public override void _Input(InputEvent @event)
	{
		if (!Visible) return;

		// Close menu on Escape (ui_cancel)
		if (@event.IsActionPressed("ui_cancel"))
		{
			CloseMenu();
			GetViewport().SetInputAsHandled();
		}
	}
}
