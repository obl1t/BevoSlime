using Godot;
using System;

public partial class MainCamera : Camera2D
{

	private Node2D player;
	private bool playerReady;
	private Node2D world;
	public override void _Ready()
	{
		// Change to get the player now that the player is instantiated by WorldController.
		playerReady = false;
		world = GetNode<Node2D>("/root/World");
		((WorldController) world).PlayerReady += OnPlayerReady;
	}


	public override void _Process(double delta)
	{
		// Move the camera to follow the player, if the player is in the scene.
		if(playerReady){
			if(player.GlobalPosition.X - GlobalPosition.X > 600){
				GlobalPosition = new Vector2(GlobalPosition.X + 1200, GlobalPosition.Y);
			}
			if(player.GlobalPosition.X - GlobalPosition.X < -600){
				GlobalPosition = new Vector2(GlobalPosition.X - 1200, GlobalPosition.Y);
			}
		}
	}

	// Called when the player is instantiated.
	private void OnPlayerReady(Node2D player){
		this.player = player;
		playerReady = true;
	}
}
