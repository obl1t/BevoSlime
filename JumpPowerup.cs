using Godot;
using System;

public partial class JumpPowerup : Area2D
{
	private PackedScene particles = GD.Load<PackedScene>("res://JumpParticles.tscn");
	[Signal]
	public delegate void JumpPowerupCollectedEventHandler(); // Used to signal the player to update the number of jumps.

	private Node2D player;
	private Node2D world;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode<AnimatedSprite2D>("JumpSprite").Play("Default");
		world = GetNode<Node2D>("/root/World");
		
		// Connect the world's player ready signal to OnPlayerReady.
		((WorldController) world).PlayerReady += OnPlayerReady;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	// Called when a body enters the Area2D.
	private void OnBodyEntered(Node2D body){
		// If the body was a player body, destroy myself.
		if(body.HasMethod("IsPlayer")){
			// Create some particles.
			CpuParticles2D instance = (CpuParticles2D) particles.Instantiate();
			instance.Emitting = true;
			instance.GlobalPosition = GlobalPosition;
			GetTree().CurrentScene.AddChild(instance);
			((Player) body).EnterJump();
			QueueFree();
		}
	}

	// Used to connect to the player when the player is instantiated.
	private void OnPlayerReady(Node2D player){
		this.player = player;
	}
}
