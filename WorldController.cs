using Godot;
using System;

public partial class WorldController : Node2D
{
	// Called when the node enters the scene tree for the first time.
	private CharacterBody2D player;
	private PackedScene deathScreen = GD.Load<PackedScene>("res://DeathScreen.tscn");
	private PackedScene level1 = GD.Load<PackedScene>("res://Level1.tscn");
	private PackedScene level2 = GD.Load<PackedScene>("res://Level2.tscn");
	private PackedScene level3 = GD.Load<PackedScene>("res://Level3.tscn");
	private Node2D currentLevel;
	private int currentLevelIndex; // Used to determine logic related to the current game level.
	private PackedScene playerScene = GD.Load<PackedScene>("res://Player.tscn");

	[Signal]
	public delegate void PlayerReadyEventHandler(Node2D player); // Used to signal the camera that the player is ready to use. 

	private bool isInDeath; // Used to determine if the DeathScreen is in the scene tree.
	public override void _Ready()
	{
		currentLevelIndex = 1;
		currentLevel = (Node2D) level1.Instantiate();
		isInDeath = false;
		AddChild(currentLevel);
		CreatePlayer();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	// Called when the the player dies. 
	private void OnPlayerDeath(){
		isInDeath = true;
		CanvasLayer instance = (CanvasLayer) deathScreen.Instantiate();
		AddChild(instance);
	}

	// Called when the player resets the level.
	private void OnReset(){
		// Destroy the current level.
		currentLevel.QueueFree();
		// Create a new level.
		if(currentLevelIndex == 1){
			currentLevel = (Node2D) level1.Instantiate();
		}
		else if(currentLevelIndex == 2){
			currentLevel = (Node2D) level2.Instantiate();
		}
		else if(currentLevelIndex == 3){
			currentLevel = (Node2D) level3.Instantiate();
		}
		if(isInDeath){
			// Destroy the death screen.
			GetNode<CanvasLayer>("DeathScreen").QueueFree();
			isInDeath = false;
		}
		AddChild(currentLevel);
		player.QueueFree();
		CreatePlayer();
		
	}

	// Used to create a new player, when resetting or loading a new level.
	private void CreatePlayer(){
		player = (CharacterBody2D) playerScene.Instantiate();
		AddChild(player);
		((Player) player).PlayerDeath += OnPlayerDeath;
		((Player) player).Reset += OnReset;
		((Player) player).NextLevel += OnNextLevel;
		EmitSignal(SignalName.PlayerReady, player); 
	}

	private void OnNextLevel(){
		// Reset the level, with current level index incremented so that a new level is loaded.
		currentLevelIndex++;
		OnReset();
	}
}
