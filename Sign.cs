using Godot;
using System;

public partial class Sign : Area2D
{
	private Label label;
	private string textToShow; // Stores the text to show when the player is in the sign's area.

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		label = GetNode<Label>("Label");
		textToShow = label.Text;
		label.Text = "";
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	// Called when a body enters the Area2D.
	private void OnBodyEntered(Node2D body){
		// Show the sign's text.
		if(body.HasMethod("IsPlayer")){
			label.Text = textToShow;
		}
	}

	// Called when a body exits the Area2D.
	private void OnBodyExited(Node2D body){
		// Hide the sign's text.
		if(body.HasMethod("IsPlayer")){
			label.Text = "";
		}
	}
}
