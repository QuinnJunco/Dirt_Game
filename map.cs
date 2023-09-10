using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public struct RoomData {
	public bool active; // true if this is a room and not an empty slot
	public int doors; // number of doors, should be the same as number of neibors
	public int neighbors; // number of neibors, should be the same as number of doors
	public int xPos; // x position on the map
	public int yPos; // y position on the map
	public bool start; // starting room
	public bool boss; // boss room
	public bool clear; // if the room is completed
}

public class map {
	
	[Export]
	public TileMap dungeon, backMap;
	[Export]
	public PackedScene roomScene {get; set;}
	public RoomData[,] grid;
	public int numRows, numCols;
	
	
	public void Start(int numRows, int numCols) {
		map newMap = new map();
		grid = newMap.MapGenerate(grid, numRows, numCols);
		newMap.MapBuild(grid);
	}
	
	public map() {
		
	}
	// ACTIVE TASK IS PREVENTING BOSS ROOM FROM HAVING NEIGHBORS
	public RoomData[,] MapGenerate(RoomData[,] mapGrid, int rows, int cols) {
		mapGrid = new RoomData[rows, cols]; // new 2d array of struct room
		Vector2 firstRoom = new Vector2 (
			x: (int)GD.RandRange(1, rows - 2),
			y: (int)GD.RandRange(1, cols - 2)
		); // vector2 containing the first generated room
		for (int i = 0; i < rows; i++) {
			for (int j = 0; j < cols; j++) {
				mapGrid[i,j].active = false;
			}
		} // used to ensure every slot exists
		
		int roomCount = 0;
		int r = (int)firstRoom.X;
		int c = (int)firstRoom.Y;
		mapGrid[(int)firstRoom.X, (int)firstRoom.Y].active = true;
		mapGrid[(int)firstRoom.X, (int)firstRoom.Y].boss = true;
		GD.Print(); // DEBUG TOOL
		
		
		while (roomCount < (rows + cols)) {
			List<Vector2> neighbors = PotentialNeighbors(mapGrid, r, c);
			// DEBUG TOOL
			GD.Print("c " + r + " " + c);
			for (int i = 0; i < neighbors.Count; i++) {
				GD.Print("n " + neighbors[i].X + " " + neighbors[i].Y);
			}
			
			int dice = -1;
			if (neighbors.Count > 0) { // prevents rolling -1
				dice = GD.RandRange(0, neighbors.Count - 1);
				GD.Print("d " + dice + "\nBREAK"); // DEBUG TOOL
			} else {
				if (roomCount > (rows + cols) / 2) {
					break;
				} else {
					GD.Print("RESTART"); // DEBUG TOOL
					MapGenerate(mapGrid, rows, cols);
				}
			}
			if (dice >= 0) { // if the dice properly rolls, it will roll for next gen
				mapGrid[(int)neighbors[dice].X, (int)neighbors[dice].Y].active = true;
				roomCount++;
				if (r == firstRoom.X && c == firstRoom.Y) {
					r = (int) neighbors[dice].X;
					c = (int) neighbors[dice].Y;
				} else if (PotentialNeighbors(mapGrid, (int) neighbors[dice].X, (int) neighbors[dice].Y).Count > 1) {
					if ((int)GD.RandRange(0,2) > 1 && neighbors.Count > 1) {
						continue;
					} else {
						r = (int) neighbors[dice].X;
						c = (int) neighbors[dice].Y;
					}
				} else if (neighbors.Count > 1) {
					continue;
				} else {
					break;
				}
			}
		}
		
		if (roomCount < (rows + cols)/2) {
			GD.Print("RESTART");
			MapGenerate(mapGrid, rows, cols);
		}
		// counts all neighbors for each room
		for (int i = 0; i < rows; i++) {
			for (int j = 0; j < cols; j++) {
				if  (mapGrid[i,j].active) {
					getNeighbors(mapGrid, i, j);
				}
			}
		}
	
		RoomAssignment(mapGrid); // completes assignment of boss and start;
	
	// DEBUG TOOL
	StringBuilder grid = new StringBuilder();
	for (int i = 0; i < rows; i++) {
		for (int j = 0; j < cols; j++) {
			if (mapGrid[i,j].boss) {
				grid.Append("B ");
			} else if (mapGrid[i,j].start) {
				grid.Append("S ");
			} else if (mapGrid[i,j].active) {
				grid.Append(mapGrid[i,j].neighbors + " ");
			} else {
				grid.Append("0 ");
			}
		}
		grid.Append("\n");
	}
	String gridString = grid.ToString();
	GD.Print(roomCount);
	GD.Print(gridString);
	
		return mapGrid;
	}

// builds an arraylist of all potential neighbors
	public List<Vector2> PotentialNeighbors(RoomData[,] mapGrid, int currR, int currC) {
		List<Vector2> neighbors = new List<Vector2>(); // stores potential neighbors
		for (int r = 0; r < mapGrid.GetLength(0); r++) {
			for (int c = 0; c < mapGrid.GetLength(0); c++) {
				if (mapGrid[r,c].active) { // full room case
					continue;
				} else if (r == currR && c != currC) { // same row case
					if (c == currC - 1) {
						neighbors.Add(new Vector2 (r, c));
					} else if (c == currC + 1) {
						neighbors.Add(new Vector2 (r, c));
					}
				} else if (c == currC && r != currR) { // same col case
					if (r == currR - 1) {
						neighbors.Add(new Vector2 (r, c));
					} else if (r == currR + 1) {
						neighbors.Add(new Vector2 (r, c));
					}
				} 
			}
		}
		return neighbors;
	}
	
	public void getNeighbors(RoomData[,] mapGrid, int currR, int currC) {
		mapGrid[currR,currC].neighbors = 0; // initializes neighbors to 0
		for (int r = 0; r < mapGrid.GetLength(0); r++) {
			for (int c = 0; c < mapGrid.GetLength(1); c++) {
				if (r == currR && c != currC) { // same row case
					if (mapGrid[r,c].active) {
						if (c == currC - 1) {
							mapGrid[currR,currC].neighbors++;
						} else if (c == currC + 1) {
							mapGrid[currR,currC].neighbors++;
						}
					}
				} else if (c == currC && r != currR) { // same col case
					if (mapGrid[r,c].active) {
						if (r == currR - 1) {
							mapGrid[currR,currC].neighbors++;
						} else if (r == currR + 1) {
							mapGrid[currR,currC].neighbors++;
						}
					}
				} 
			}
		}
	}
	
	// algorithm to place boss and start rooms
	public void RoomAssignment(RoomData[,] mapGrid) {
		RoomData[,] emptyGrid = new RoomData[mapGrid.GetLength(0),mapGrid.GetLength(1)]; // used to ensure boss and start are not next to eachother
		List<Vector2> high = new List<Vector2>(); // stores rooms with high usage priority
		List<Vector2> medium = new List<Vector2>(); // stores rooms with medium usage priority
		List<Vector2> low = new List<Vector2>(); // stores rooms with low usage priority
		Vector2 boss;
		int dice;
		
		for (int r = 0; r < mapGrid.GetLength(0); r++) {
			for (int c = 0; c < mapGrid.GetLength(1); c++) {
				switch (mapGrid[r,c].neighbors) { // assigns priority weightings to rooms depending on number of neighbors
					case 1:
						high.Add(new Vector2 (r, c));
						continue;
					case 2:
						medium.Add(new Vector2 (r, c));
						continue;
					case 3:
						medium.Add(new Vector2 (r, c));
						continue;
					case 4:
						low.Add(new Vector2 (r, c));
						continue;
				}
			}
		}
		if (high.Count >= 2) {
			dice = GD.RandRange(0, high.Count - 1); // rolls for a boss room
			mapGrid[(int)high[dice].X, (int)high[dice].Y].boss = true; 
			boss = high[dice];
			high.Remove(high[dice]); // removes the used room from high list
			dice = GD.RandRange(0, high.Count - 1); // rolls for a starting room
			
			// this condition prevents boss and start from being neighbors
			if (PotentialNeighbors(emptyGrid, (int)high[dice].X, (int)high[dice].Y).Contains(boss)) {
				high.Remove(high[dice]); // removes the used room from list
				dice = GD.RandRange(0, high.Count - 1); // rolls for a starting room
			}
			mapGrid[(int)high[dice].X, (int)high[dice].Y].start = true;
		} else if (medium.Count + high.Count >= 2) {
			if (high.Count == 1) { // ensures that the room with least neighbors is boss
				mapGrid[(int)high[0].X, (int)high[0].Y].boss = true;
				boss = high[0];
				high.Remove(high[0]);
			} else { // as long as boss hasnt yet been assigned
				dice = GD.RandRange(0, medium.Count - 1); // rolls for a boss room
				mapGrid[(int)medium[dice].X, (int)medium[dice].Y].boss = true; 
				boss = medium[dice];
				medium.Remove(medium[dice]); // removes the used room from medium list
			}
			dice = GD.RandRange(0, medium.Count - 1); // rolls for a starting room
			// this condition prevents boss and start from being neighbors
			if (PotentialNeighbors(emptyGrid, (int)medium[dice].X, (int)medium[dice].Y).Contains(boss)) {
				medium.Remove(medium[dice]); // removes the used room from list
				dice = GD.RandRange(0, medium.Count - 1); // rolls for a starting room
			}
			mapGrid[(int)medium[dice].X, (int)medium[dice].Y].start = true;
		} else { // if somehow there's not enough medium + high rooms
			/*
				CHANGE THIS MAKE THIS REROLL THE ENTIRE MAP, START FRESH
			*/ 
			grid = MapGenerate(mapGrid, this.numRows, this.numCols);
			
			for (int i = 0; i < high.Count; i++) {
				medium.Add(high[i]);
			}
			for (int i = 0; i < low.Count; i++) {
				medium.Add(low[i]);
			}
			dice = GD.RandRange(0, medium.Count - 1); // rolls for a boss room
			mapGrid[(int)medium[dice].X, (int)medium[dice].Y].boss = true; 
			boss = medium[dice];
			medium.Remove(medium[dice]); // removes the used room from medium list
			dice = GD.RandRange(0, medium.Count - 1); // rolls for a starting room
			// this condition prevents boss and start from being neighbors
			if (PotentialNeighbors(emptyGrid, (int)medium[dice].X, (int)medium[dice].Y).Contains(boss)) {
				medium.Remove(medium[dice]); // removes the used room from list
				dice = GD.RandRange(0, medium.Count - 1); // rolls for a starting room
			}
			mapGrid[(int)medium[dice].X, (int)medium[dice].Y].start = true;
		}
	}
	
	public void MapBuild(RoomData[,] gridMap) {
		// TODO: FIGURE OUT HOW TO DRAW TILEMAP INTO GAME WORLD
	}
}
