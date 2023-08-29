using Godot;
using System;

public partial class WinScreen : CanvasLayer
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	// Called when the exit button is pressed.
	private void OnExitPressed(){
		GetTree().Quit();
	}

	// Called when the start over button is pressed.
	private void OnStartoverPressed(){
		GetTree().ReloadCurrentScene();
	}
}
