using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;


public partial class main : Node {
	[Export]
	public PackedScene EnemyScene {get; set;}
	
	
	
	public override void _Ready() {
		map tMap = new map();
		tMap.Start(5,5);
		//enemy nikolai = EnemyScene.Instantiate<enemy>();
	}
	
	public override void _Process(double delta) {
		//var enemy = GetNode<enemy>("Enemy");
		//var player = GetNode<player>("Main/Player");
		//if (enemy._player_enter_view) {
			//enemy.follow(player);
		//}
	}
	
}
