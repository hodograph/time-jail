using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class MazeGenerator : MonoBehaviour {
	public int width, height;
	public Material brick;
	private int[,] Maze;
	private List<Vector3> pathMazes = new List<Vector3>();
	private Stack<Vector2> _tiletoTry = new Stack<Vector2>();
	private List<Vector2> offsets = new List<Vector2> { new Vector2(0, 1), new Vector2(0, -1), new Vector2(1, 0), new Vector2(-1, 0) };
	private System.Random rnd = new System.Random();
	private int _width, _height;
	private Vector2 _currentTile;
	public Vector2 CurrentTile
	{
		get { return _currentTile; }
		private set
		{
			if (value.x < 1 || value.x >= this.width - 1 || value.y < 1 || value.y >= this.height - 1)
			{
				throw new ArgumentException("CurrentTile must be within the one tile border all around the maze");
			}
			if (value.x % 2 == 1 || value.y % 2 == 1)
			{ _currentTile = value; }
			else
			{
				throw new ArgumentException("The current square must not be both on an even X-axis and an even Y-axis, to ensure we can get walls around all tunnels");
			}
		}
	}

	private static MazeGenerator instance;
	public static MazeGenerator Instance
	{
		get
		{
			return instance;
		}
	}

	void Awake()
	{
		instance = this;
	}

	void Start()
	{
		//Camera.main.orthographic = true;
		//Camera.main.orthographicSize = 30;
		
		GameObject floor = GameObject.CreatePrimitive (PrimitiveType.Plane);
		floor.transform.parent = transform;
		MeshCollider floorCollider = (MeshCollider)floor.gameObject.AddComponent(typeof(MeshCollider));
		floor.transform.localScale = new Vector3 (width/10, 1, height/10);
		floor.transform.position = new Vector3 (width / 2, -0.5f, height / 2);
		floor.GetComponent<Renderer>().material.color = Color.gray;
		GenerateMaze();
		//GenerateGuards();

		
		//GameObject player = GameObject.CreatePrimitive (PrimitiveType.Cube);
		//player.transform.position = new Vector3 (1, -.37f, 1);
		//player.transform.localScale = new Vector3 (0.25f, 0.25f, 0.25f);
		//player.gameObject.AddComponent(typeof(BoxCollider));
		//player.gameObject.AddComponent<Rigidbody>();
		//player.gameObject.AddComponent<PlayerController>();
		//player.GetComponent<Renderer>().material.color = Color.red;
		//Camera.main.transform.parent = player.transform;
		
		
	}

	void GenerateMaze()
	{
		width = width + 1;
		height = height + 1;
		Maze = new int[width, height];
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				Maze[x, y] = 1;
			}
		}
		CurrentTile = Vector2.one;
		_tiletoTry.Push(CurrentTile);
		Maze = CreateMaze();
		GameObject ptype = null;
		for(int i = 0; i<UnityEngine.Random.Range (2, 7); i++)
		Maze = setRoom ();
		Maze = setWalls ();
		Maze [0, 1] = 3;
		Maze [1, 0] = 2;
		Maze [1, height - 1] = 2;
		Maze [width - 1, 1] = 3;
		for (int i = 0; i <= Maze.GetUpperBound(0); i++)
		{
			for (int j = 0; j <= Maze.GetUpperBound(1); j++)
			{
				if (Maze[i, j] >= 2)
				{
					ptype = GameObject.CreatePrimitive(PrimitiveType.Cube);
					
					ptype.GetComponent<Renderer>().material.color = Color.blue;
					
					BoxCollider box = (BoxCollider)ptype.gameObject.AddComponent(typeof(BoxCollider));

					ptype.transform.position = new Vector3(i , 0, j * ptype.transform.localScale.y);
					//ptype.transform.localScale = new Vector3 (0.25f, 1, 0.25f);
					if(Maze[i,j] == 2)
						ptype.transform.localScale = new Vector3 (2.25f, 1, 0.25f);
					if(Maze[i,j] == 3)
						ptype.transform.localScale = new Vector3 (0.25f, 1, 2.25f);
					if (brick != null)
					{
						ptype.GetComponent<Renderer>().material = brick;
					}
					ptype.transform.parent = transform;
				}
				else if (Maze[i, j] == 0)
				{
					pathMazes.Add(new Vector3(i, 0, j));
				}

			}
		}
	}

	void GenerateGuards () {
		for (int i = 1; i <= 20; i++) {
			int ex = -1;
			int wy = -1;
			while (Maze[ex+1,wy+1] > 0) {
				ex = UnityEngine.Random.Range (0, 49);
				wy = UnityEngine.Random.Range (0, 49);
			}

			GameObject guard = GameObject.CreatePrimitive(PrimitiveType.Cube);

			guard.GetComponent<Renderer>().material.color = Color.white;

			BoxCollider box = (BoxCollider)guard.gameObject.AddComponent(typeof(BoxCollider));

			guard.transform.position = new Vector3(ex+1, 0, wy+1);

			guard.transform.localScale = new Vector3 (0.5f, 1, 0.5f);
			{
				guard.GetComponent<Renderer>().material = brick;
			}
			guard.transform.parent = transform;


			GameObject flashlight = new GameObject ("flashlight");
			flashlight.AddComponent (typeof(MeshRenderer));
			flashlight.AddComponent (typeof(MeshFilter));
			Material mymat = Resources.Load("FlashLight", typeof(Material)) as Material;
			flashlight.GetComponent<Renderer> ().material = mymat;
			flashlight.transform.parent = guard.transform;
			flashlight.transform.position = guard.transform.position;


			guard.AddComponent (typeof(FieldOfView));
			FieldOfView fov = guard.GetComponent(typeof(FieldOfView)) as FieldOfView;
			fov.viewRadius = 10;
			fov.viewAngle = 62;
			fov.startAngle = UnityEngine.Random.Range (0, 360);
			fov.endAngle = (fov.startAngle + 180) % 360;
			fov.speed = 10;
			fov.targetMask.value = 0;
			fov.obstacleMask.value = ~0;
			fov.meshResolution = 10;
			fov.viewMeshFilter = flashlight.GetComponent<MeshFilter>();






		}



	}

	public int[,] setRoom()
	{
		int roomwidth, roomheight;
		roomwidth = UnityEngine.Random.Range (7, 12);
		roomheight = UnityEngine.Random.Range (7, 12);
		int startx = UnityEngine.Random.Range (5, width - roomwidth - 5);
		int starty = UnityEngine.Random.Range (5, height - roomheight - 5);
		for (int i = startx; i < startx+roomwidth; i++) {
			for (int j = starty; j < starty+roomheight; j++) {
				Maze [i, j] = 0;
				if (i == startx || j == starty)
					Maze [i, j] = 1;
				if (i == startx+roomwidth - 1)
					Maze [i, j] = 1;
				if (j == starty+roomheight - 1)
					Maze [i, j] = 1;
			}
		}
		return Maze;
	}

	public int[,] setWalls()
	{
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				if(Maze[i,j] != 0){
					if (Horizontal (i, j))
						Maze [i, j] = 2;
					else if (Vertical (i, j))
						Maze [i, j] = 3;
					else if (oneNeighbor (i, j))
						Maze [i, j] = 0;
				}
			}
		}
		return Maze;

	}

	public Boolean Horizontal(int i, int j){
		if(i-1>0 && i+1<width)
			if (Maze [i - 1, j] >= 1 && Maze [i + 1, j] >= 1)
				return true;
		return false;
	}

	public Boolean Vertical(int i, int j){
		if(j-1>0 && j+1<height)
			if (Maze [i, j-1] >= 1 && Maze [i, j+1] >= 1)
				return true;
		return false;
	}

	public Boolean oneNeighbor(int i, int j)
	{
		if(j-1>0 && j+1<height && i-1>0 && i+1<width)
		if (Maze [i, j-1] >= 1 && Maze [i, j+1] == 0 && Maze [i - 1, j] == 0 && Maze [i + 1, j] == 0)
			return true;
		else if (Maze [i, j-1] == 0 && Maze [i, j+1] >= 1 && Maze [i - 1, j] == 0 && Maze [i + 1, j] == 0)
			return true;
		else if (Maze [i, j-1] == 0 && Maze [i, j+1] == 0 && Maze [i - 1, j] >= 1 && Maze [i + 1, j] == 0)
			return true;
		else if (Maze [i, j-1] == 0 && Maze [i, j+1] == 0 && Maze [i - 1, j] == 0 && Maze [i + 1, j] >= 1)
			return true;
		return false;
	}

	public int[,] CreateMaze()
	{
		//local variable to store neighbors to the current square
		//as we work our way through the maze
		List<Vector2> neighbors;
		//as long as there are still tiles to try
		while (_tiletoTry.Count > 0)
		{
			//excavate the square we are on
			Maze[(int)CurrentTile.x, (int)CurrentTile.y] = 0;

			//get all valid neighbors for the new tile
			neighbors = GetValidNeighbors(CurrentTile);

			//if there are any interesting looking neighbors
			if (neighbors.Count > 0)
			{
				//remember this tile, by putting it on the stack
				_tiletoTry.Push(CurrentTile);
				//move on to a random of the neighboring tiles
				CurrentTile = neighbors[rnd.Next(neighbors.Count)];
			}
			else
			{
				//if there were no neighbors to try, we are at a dead-end
				//toss this tile out
				//(thereby returning to a previous tile in the list to check).
				CurrentTile = _tiletoTry.Pop();
			}
		}

		return Maze;
	}
	/// <summary>
	/// Get all the prospective neighboring tiles
	/// </summary>
	/// <param name="centerTile">The tile to test</param>
	/// <returns>All and any valid neighbors</returns>
	private List<Vector2> GetValidNeighbors(Vector2 centerTile)
	{

		List<Vector2> validNeighbors = new List<Vector2>();

		//Check all four directions around the tile
		foreach (var offset in offsets)
		{
			//find the neighbor's position
			Vector2 toCheck = new Vector2(centerTile.x + offset.x, centerTile.y + offset.y);

			//make sure the tile is not on both an even X-axis and an even Y-axis
			//to ensure we can get walls around all tunnels
			if (toCheck.x % 2 == 1 || toCheck.y % 2 == 1)
			{
				//if the potential neighbor is unexcavated (==1)
				//and still has three walls intact (new territory)
				if (Maze[(int)toCheck.x, (int)toCheck.y] == 1 && HasThreeWallsIntact(toCheck))
				{
					//add the neighbor
					validNeighbors.Add(toCheck);
				}
			}
		}

		return validNeighbors;
	}


	/// <summary>
	/// Counts the number of intact walls around a tile
	/// </summary>
	/// <param name="Vector2ToCheck">The coordinates of the tile to check</param>
	/// <returns>Whether there are three intact walls (the tile has not been dug into earlier.</returns>
	private bool HasThreeWallsIntact(Vector2 Vector2ToCheck)
	{
		int intactWallCounter = 0;

		//Check all four directions around the tile
		foreach (var offset in offsets)
		{
			//find the neighbor's position
			Vector2 neighborToCheck = new Vector2(Vector2ToCheck.x + offset.x, Vector2ToCheck.y + offset.y);

			//make sure it is inside the maze, and it hasn't been dug out yet
			if (IsInside(neighborToCheck) && Maze[(int)neighborToCheck.x, (int)neighborToCheck.y] == 1)
			{
				intactWallCounter++;
			}
		}

		//tell whether three walls are intact
		return intactWallCounter == 3;

	}

	private bool IsInside(Vector2 p)
	{
		return p.x >= 0 && p.y >= 0 && p.x < width && p.y < height;
	}
}

