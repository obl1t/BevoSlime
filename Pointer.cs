using Godot;
using System;

public partial class Pointer : Area2D
{
	public enum PointerState { Idle, Aggroing, Deaggroing, Dashing, Frozen};
	public PointerState myState;
	private AnimatedSprite2D mySprite;
	private Vector2 velocity;
	private float timeElapsed;
	private float shakeRadius; // The intensity of shaking.

	private Vector2 startPosition;
	private float shakeStartTime;
	private Node2D player;
	private Node2D world;

	private const float AGGRO_TIME = 0.8f; // How long after the player enters the pointer's eyesight before the pointer starts to dash.

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		myState = PointerState.Idle;
		timeElapsed = 0;
		shakeRadius = 0;
		mySprite = GetNode<AnimatedSprite2D>("PointerSprite");
		world = GetNode<Node2D>("/root/World");
		// Connect the world's player ready signal to OnPlayerReady.
		((WorldController) world).PlayerReady += OnPlayerReady;
	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		timeElapsed += (float) delta;
		// Float up and down.
		if(myState == PointerState.Idle){
			velocity.Y = MathF.Sin(timeElapsed) * 10f;
			Position += velocity * (float)delta;
		}
		// Start to shake.
		if(myState == PointerState.Aggroing){
			Shake();
			shakeRadius += (float) delta * 3f;
			if(timeElapsed - shakeStartTime > AGGRO_TIME){
				myState = PointerState.Dashing;
				mySprite.Play("Dashing");
			}
		}
		if(myState == PointerState.Dashing){
			velocity.X -= 3000 * (float) delta;
			Position += velocity * (float)delta;
			if(GlobalPosition.X < -1000){
				QueueFree();
			}
		}
		if(myState == PointerState.Deaggroing){
			Shake();
			shakeRadius -= (float) delta * 2;
			if(shakeRadius < 0.1f){
				shakeRadius = 0;
				myState = PointerState.Idle;
				mySprite.Play("Idle");
			}
		}
		
	}

	// Called when a body enters the eyesight of the pointer.
	private void OnEyesightEntered(Node2D body){
		if(body.HasMethod("IsPlayer")){
			if(myState == PointerState.Idle){
				startPosition = Position;
				myState = PointerState.Aggroing;
				mySprite.Play("Aggroed");
				shakeStartTime = timeElapsed;
			}
		}
	}

	// Called when a body exits the eyesight of the pointer.
	private void OnEyesightExited(Node2D body){
		if(body.HasMethod("IsPlayer")){
			if(myState == PointerState.Aggroing){
				myState = PointerState.Deaggroing;
			}
		}
	}

	// Moves the pointer's location to a random point on a circle around its starting position.
	private void Shake(){
		float angle = GD.Randf() * 2f * MathF.PI;
		Position = startPosition + new Vector2(MathF.Sin(angle) * shakeRadius, MathF.Cos(angle) * shakeRadius);
	}

	// Called when the player is instantiated.
	private void OnPlayerReady(Node2D player){
		this.player = player;
	}

	// Called when a body enters the Area2D.
	private void OnBodyEntered(Node2D body){
		if(body.HasMethod("IsPlayer")){
			((Player) body).Die();
			myState = PointerState.Frozen;
			velocity.X = 0;
			velocity.Y = 0;
		}
	}
}
