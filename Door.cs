using Godot;
using System;

public partial class Door : Area2D
{

	[Signal]
	public delegate void DoorEnteredEventHandler(); // Used to signal the world to switch levels.
	private Node2D world;
	private AnimatedSprite2D mySprite;
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

	private void OnBodyEntered(Node2D body){
		if(body.HasMethod("IsPlayer")){
			mySprite.Play("Open");
		}
	}

	private void OnBodyExited(Node2D body){
		if(body.HasMethod("IsPlayer")){
			mySprite.Play("Close");
		}
	}

	private void OnPlayerReady(Node2D player){
		
	}


}
