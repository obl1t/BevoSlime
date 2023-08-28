using Godot;
using System;

public partial class Spike : Area2D
{
	private Node2D world;
	private Node2D player;
	
	[Signal]
	public delegate void SpikeHitEventHandler(); // Used to signal the player to die. 

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		world = GetNode<Node2D>("/root/World");
		((WorldController) world).PlayerReady += OnPlayerReady;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnBodyEntered(Node2D body){
		// If the body was a player body, tell the player to die.
		if(body.HasMethod("IsPlayer")){
			((Player) body).Die();
		}
	}

	// Used to connect to the player when the player is instantiated.
	private void OnPlayerReady(Node2D player){
		this.player = player;
	}
	
}
