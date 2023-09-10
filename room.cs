using Godot;
using System;

public partial class room : Area2D {
	public int xPosition, yPosition;
	
	public room(int x, int y) {
		xPosition = x;
		yPosition = y;
	}
	
	public override void _Ready() {
		
	}
}
