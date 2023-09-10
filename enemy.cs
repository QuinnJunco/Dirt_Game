using Godot;
using System;

public partial class enemy : RigidBody2D {
	
	[Export]
	public int speed;
	
	[Signal]
	public delegate void ViewEventHandler();
	
	public bool view = false;
	
	public enemy(int xPos, int yPos) {
		Position = new Vector2(
			x: xPos,
			y: yPos
		);
	}
	
	public override void _Ready() {
		
	}
	
	public override void _Process(double delta) {
		if (view) {
			follow();
		}
	}
	
	private void _on_view() { // if the signal for view is received then enemy will follow
		//view = true;
	}
	
	public void follow() {
		player playerChar = GetNode<player>("player");
		var path = GetNode<Path2D>("PlayerFollow");
		var follow = GetNode<PathFollow2D>("PlayerFollow/PathFollow2D");
		Curve2D pathCurve = new Curve2D();
		
		// pathCurve.RemovePoint(pathCurve.PointCount - 1); // removes end point on every call
		pathCurve.ClearPoints();
		pathCurve.AddPoint(Position);
		pathCurve.AddPoint(playerChar.Position);
		path.Curve = pathCurve;
		// TODO: implement movement for following
		
		follow.HOffset += 1;
	}
}





