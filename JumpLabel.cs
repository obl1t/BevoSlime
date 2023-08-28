using Godot;
using System;
using System.Drawing;

public partial class JumpLabel : Label
{
	// Called when the node enters the scene tree for the first time.
	private Node2D player;
	private int jumps;
	private Node2D world;
	public override void _Ready()
	{
		Text = $": 1";
		world = GetNode<Node2D>("/root/World");
		((WorldController) world).PlayerReady += OnPlayerReady;

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	
	}

	private void OnJumpUpdate(int numJumps){
		jumps = numJumps;
		Text = $": {jumps}";
		if(numJumps == 0){
			AddThemeColorOverride("font_color", Colors.Red);
		}
		else{
			AddThemeColorOverride("font_color", Colors.White);
		}
	}

	// Used to connect to the player when the player is instantiated.
	private void OnPlayerReady(Node2D player){
		this.player = (CharacterBody2D) player;
		((Player) player).UpdateJumps += OnJumpUpdate;
		jumps = ((Player) player).numJumps;
		Text = $": {jumps}";
		AddThemeColorOverride("font_color", Colors.White);
	}
}
