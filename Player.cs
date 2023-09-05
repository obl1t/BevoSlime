using Godot;
using System;
using System.Data;

public partial class Player : CharacterBody2D
{
	private const float GRAVITY = 1200f;
	private const float JUMP_STRENGTH = 700f;
	private Vector2 velocity;

	private const float ACCELERATION = 1000f; // How quickly the player speeds up after pressing a horizontal input key. 
	private const float DECELERATION = 5f; // How quickly the player slows down after releasing horizontal input keys. 
	// Not on the same scale as acceleration, as deceleration is multiplicative for a smoother slow. 
	private const float MAX_SPEED = 300f;
	private const float AIR_CONTROL = 0.5f; // The proportion of full control the player has when airborne.
	private const float SIZE = 0.7f; // The size of the player sprite.
	private float control; // Additional multiplier on how quickly the player can change directions/accelerate.
	public enum PlayerState {Moving, Idle, Jumping, Dying, Winning};
	public PlayerState myState;
	private AnimatedSprite2D mySprite;
	private bool beenOffFloor; // Used to determine if the player has left the ground when jumping.
	public int numJumps; // The number of jumps the player has left. Once this hits zero, the player is unable to jump. 

	[Signal]
	public delegate void UpdateJumpsEventHandler(int jumps); // Used to update a label with the number of jumps. 

	[Signal]
	public delegate void PlayerDeathEventHandler(); // Used to signal the world to display a death screen.

	[Signal]
	public delegate void ResetEventHandler(); // Used to signal the world to reset the level. 

	[Signal]
	public delegate void NextLevelEventHandler(); // Used to signal the world to load the next level.

	private bool canMoveToNextLevel; // Determines whether the player can advance levels with A.

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		myState = PlayerState.Jumping;
		mySprite = GetNode<AnimatedSprite2D>("PlayerSprite");
		control = 1f;
		beenOffFloor = false;
		canMoveToNextLevel = false;
		numJumps = 1;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
        if(myState == PlayerState.Jumping){
			
			control = AIR_CONTROL;
		}
		else{
			control = 1f;
		}
		if(myState == PlayerState.Moving){
			mySprite.Play("Move");
		}
	}

	public override void _PhysicsProcess(double delta){
		DoMovement(delta);
		// If we're not on the ground yet, keep accelerating downwards.
		if(!IsOnFloor()){
			velocity.Y += (float) delta * GRAVITY;
		}
		if(myState == PlayerState.Jumping && IsOnCeilingOnly()){
			velocity.Y = 0;
		}

		if(myState == PlayerState.Jumping && !IsOnFloor() && !beenOffFloor){
			beenOffFloor = true;
		}
		// If we've left the ground in this jump, and have just reached the ground again, end the jump.
		if(myState == PlayerState.Jumping && IsOnFloor() && beenOffFloor){
			myState = PlayerState.Idle;
			mySprite.Play("Squish");
			beenOffFloor = false;
			GetNode<CpuParticles2D>("Particles").Emitting = true;
		}
		Velocity = velocity; 
		MoveAndSlide();
	}

	// Changes velocity based on input keys.
	private void DoMovement(double delta){
		// Don't allow any input when in the victory screen.
		if(myState == PlayerState.Winning){
			return;
		}
		if (Input.IsActionJustPressed("input_r")){
			EmitSignal(SignalName.Reset);
		}
		if (Input.IsActionJustPressed("input_a")){
			if(canMoveToNextLevel){
				EmitSignal(SignalName.NextLevel);
			}
		}
		// Used for debugging. Allows the player to skip levels without needing to be at the exit.
		if(Input.IsActionJustPressed("input_k")){
			EmitSignal(SignalName.NextLevel);
		}
		// Don't allow control when dead.
		if(myState == PlayerState.Dying){
			return;
		}
		if (Input.IsActionPressed("input_right"))
		{
			if(myState == PlayerState.Idle){
				myState = PlayerState.Moving;
			}
			mySprite.Scale = new Vector2(SIZE, SIZE);
			// Acceleration boost if currently moving other direction.
			if(velocity.X < 0){
				velocity.X += 2 * ACCELERATION * (float) delta * control;
			}
			if(velocity.X < MAX_SPEED){
				velocity.X += ACCELERATION * (float) delta * control;
			}
			}
		else if (Input.IsActionPressed("input_left"))
		{
			
			if(myState == PlayerState.Idle){
				myState = PlayerState.Moving;
			}
			// Flip sprite horizontally. 
			mySprite.Scale = new Vector2(-SIZE, SIZE);
			if(velocity.X > 0){
				velocity.X -= 2 * ACCELERATION * (float) delta * control;
			}
			if(velocity.X > -MAX_SPEED){
				velocity.X -= ACCELERATION * (float) delta * control;
			}
		}
		else{
			// Gradually slow down the player.
			velocity.X *= (1-(DECELERATION * (float) delta));
			if(Mathf.Abs(velocity.X) < 1){
				velocity.X = 0;
			}
			if(myState == PlayerState.Moving){
					mySprite.Play("Idle");
					mySprite.Stop();
					myState = PlayerState.Idle;
			}
			
		}
		// If we can jump, jump and send a signal for the jump label to use.
		if(Input.IsActionPressed("input_z")){
			if(myState != PlayerState.Jumping && numJumps > 0){
				GetNode<AudioStreamPlayer>("Jump").Play();
				velocity.Y = -JUMP_STRENGTH;
				mySprite.Play("Jump");
				numJumps--;
				EmitSignal(SignalName.UpdateJumps, numJumps);
				myState = PlayerState.Jumping;
			}
		}
	}

	// Called when grabbing a jump powerup. 
	public void EnterJump(){
		numJumps++;
		EmitSignal(SignalName.UpdateJumps, numJumps);
	}

	


	// Used to identify the player.
	public bool IsPlayer(){
		return true;
	}


	
	// Kill the player and send a signal to the world to display a death screen.
	public void Die(){
		myState = PlayerState.Dying;
		mySprite.Play("Death");
		velocity.X = 0;
		velocity.Y = 0;
		GetNode<AudioStreamPlayer>("Lose").Play();
		EmitSignal(SignalName.PlayerDeath);
	}

	// Change the state to winning.
	public void Win(){
		myState = PlayerState.Winning;
		mySprite.Play("Win");
		velocity.X = 0;
		velocity.Y = 0;
	}

	// Allows the player to move to the next level.
	public void AllowAscend(){
		canMoveToNextLevel = true;
	}

	// Disallows the player to move to the next level.
	public void DisallowAscend(){
		canMoveToNextLevel = false;
	}
}
