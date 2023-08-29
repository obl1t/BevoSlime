using Godot;
using System;

public partial class Trophy : Area2D
{
	private Node2D world;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		world = GetNode<Node2D>("/root/World");
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	// Called when a body enters the Area2D. 
	private void OnBodyEntered(Node body){
		if(body.HasMethod("Player")){
			((WorldController) world).CreateVictory();
			QueueFree();
		}
	}
}
