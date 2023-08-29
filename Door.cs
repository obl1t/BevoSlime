using Godot;
using System;

public partial class Door : Area2D
{

	[Signal]
	public delegate void DoorEnteredEventHandler(); // Used to signal the world to switch levels.
	private Node2D world;
	private AnimatedSprite2D mySprite;

	private Node2D player;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		world = GetNode<Node2D>("/root/World");
		mySprite = GetNode<AnimatedSprite2D>("DoorSprite");
		((WorldController) world).PlayerReady += OnPlayerReady;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	// Called when a body enters the Area2D.
	private void OnBodyEntered(Node2D body){
		if(body.HasMethod("IsPlayer")){
			((Player) player).AllowAscend();
			mySprite.Play("Open");
		}
	}

	// Called when a body exits the Area2D.
	private void OnBodyExited(Node2D body){
		if(body.HasMethod("IsPlayer")){
			((Player) player).DisallowAscend();
			mySprite.Play("Close");
		}
	}

	// Used to connect to the player when the player is instantiated.
	private void OnPlayerReady(Node2D player){
		this.player = player;
	}


}
