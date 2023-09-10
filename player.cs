using Godot;
using System;

public partial class player : Area2D {
	[Export]
	public int speed; 
	[Export]
	public int dash_mult;
	[Export]
	public double dash_length;
	public bool canDash = true;
	
	public Vector2 direction = new Vector2(1, 0); // direction being faced, in radians
	// one direction must be 0 at all times
	
	public int dash_buffer = 130; // buffer to prevent dash spam
	
	public Vector2 screenSize;
	
	public override void _Ready() {
		//ViewportRect().Size = new Vector2 (1280, 960);
		screenSize = new Vector2 (1280, 720);
	}
	
	public override void _Process(double delta) {
		var animatedSprite2D = GetNode<AnimatedSprite2D>("PlayerAnimation");
		Vector2 velocity = Vector2.Zero; // velocity vector
		Timer dash_timer = GetNode<Timer>("DashTimer");
		Timer dash_ani_timer = GetNode<Timer>("DashAnimationTimer");
		dash_timer.WaitTime = dash_length; // allows for variable dash lengths
		dash_ani_timer.WaitTime = dash_length + .15; // delays end of animation
		
		if (Input.IsActionPressed("move_left")) { // move left
			velocity.X -= 1;
			direction.X = -1;
			direction.Y = 0;
		}
		if (Input.IsActionPressed("move_right")) { // move right
			velocity.X += 1;
			direction.X = 1;
			direction.Y = 0;
		}
		if (Input.IsActionPressed("move_up")) { // move up
			velocity.Y -= 1;
			direction.X = 0;
			direction.Y = 1;
		}
		if (Input.IsActionPressed("move_down")) { // move down
			velocity.Y += 1;
			direction.X = 0;
			direction.Y = -1;
		}
		
		if (Input.IsActionPressed("move_dash") && canDash) { // dash
			dash_timer.Start(); // starts dash timer
			dash_ani_timer.Start();
			dash_buffer -= 70;
			canDash = false; // blocks player from holding dash
		}
		
		if (dash_timer.TimeLeft > 0) { // player speeds up while dash timer is active
			velocity = velocity.Normalized() * (speed * dash_mult);
			
		} else if (velocity.Length() > 0) { // normal movement
			velocity = velocity.Normalized() * speed;
		}
		
		if (dash_buffer < 130) {
			dash_buffer++;
		} else {
			canDash = true;
		}
		
		// ANIMATION IMPLEMENTATION GOES HERE
		
		if (dash_ani_timer.TimeLeft > 0) { // dash animation
			animatedSprite2D.Animation = "dash";
		} else {
			animatedSprite2D.Animation = "default";
		}
		
		if (direction.X != 0) { // flips sprite if you turn
			animatedSprite2D.FlipH = direction.X < 0;
		}
		
		Position += velocity * (float) delta;
		//Position = new Vector2 (
			//x: Mathf.Clamp(Position.X, 0, screenSize.X),
			//y: Mathf.Clamp(Position.Y, 0, screenSize.Y)
		//);
	}
	
}
