using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;



public class MazeGenerator1 : MonoBehaviour {
	public int width, height, guards,roomcount;
	public Transform Key;
	public Transform Guard;
	public Material brick;
	static int[,] SpaceMaze;
	public int[,] Maze;
	static int[,] tMaze;
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

	private static MazeGenerator1 instance;
	public static MazeGenerator1 Instance
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
		floor.transform.localScale = new Vector3 (width/10f, 1, height/10f);
		floor.transform.position = new Vector3 (width / 2, -0.5f, height / 2);
		floor.GetComponent<Renderer>().material.color = Color.gray;
		GenerateMaze();
		GenerateGuards();

		
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
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				Maze [x, y] = 1;
			}
		}

		int rowLength = Maze.GetLength (0);
		int colLength = Maze.GetLength (1);


		CurrentTile = Vector2.one;
		_tiletoTry.Push (CurrentTile);
		Maze = CreateMaze ();

		tMaze = new int[width, height];
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				if(Maze[i,j] != 0){
					if (Horizontal (i, j))
						tMaze [i, j] = 2;
					else if (Vertical (i, j))
						tMaze [i, j] = 3;
					else if (oneNeighbor (i, j))
						tMaze [i, j] = 0;
				}
			}
		}
		tMaze [0, 1] = 3;
		tMaze [1, 0] = 2;
		tMaze [1, height - 1] = 2;
		tMaze [width - 1, 1] = 3;

		GameObject ptype = null;
		for (int q = 0; q < roomcount; q++) {
			Maze = setRoom ();
		}

//			s = "";
//			for (int i = 0; i < rowLength; i++) {
//				for (int j = 0; j < colLength; j++) {
//					s = String.Concat (s, String.Format ("{0}", Maze [i, j]));
//				}
//				s = String.Concat (s, String.Format ("{0}", Environment.NewLine));
	//		}
		//}
	
		
		Maze = setWalls ();

		Maze [0, 1] = 3;
		Maze [1, 0] = 2;
		Maze [1, height - 1] = 2;
		Maze [width - 1, 1] = 3;


	//	for (int x = 1; x < width - 1; x++) {
	//		for (int y = 1; y < height - 1; y++) {
	//			if (Maze [x, y] == 3 && (Maze [x + 1, y] == 3 || Maze [x - 1, y] == 3)) {
	//				Maze [x, y] = 0;
	//			}
	//			if (Maze [x, y] == 2 && (Maze [x, y + 1] == 2 || Maze [x, y - 1] == 2)) {
	//				Maze [x, y] = 0;
	//			}
	//		}
	//	}



		SpaceMaze = new int[width, height];
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				if (Maze [x, y] > 1) {
					SpaceMaze [x, y] = -1;
				}
				if (Maze[x, y] == 0) {
					SpaceMaze [x, y] = 0;
				}
				if (Maze[x, y] == 1) {
						if (x == 0 || x == width-1 || y == 0 || y == height-1) {SpaceMaze [x, y] = -1;}
						else if ((Maze[x,y-1] == 2 || Maze[x,y+1] == 2) && (Maze[x+1,y] == 3 || Maze[x-1,y] == 3)) {SpaceMaze [x, y] = -1;}
						else {SpaceMaze[x,y] = 0;}
				}
			}
		}

		List<Vector4> Spaces = new List<Vector4>();
		//contains each ones area and index, length and height



		int spacecounter = 1;
		Stack<Vector2> _spacetiles = new Stack<Vector2> ();
		_spacetiles.Push(Vector2.one);


	
		while (true) {
			bool breaker = true;
			int areacounter = 0;
			float xmin = 100f;
			float xmax = 0f;
			float ymin = 100f;
			float ymax = 0f;
			while (_spacetiles.Count > 0) {
				Vector2 ThisTile = _spacetiles.Pop ();
				if(SpaceMaze [(int)ThisTile.x, (int)ThisTile.y] == 0) {
					SpaceMaze [(int)ThisTile.x, (int)ThisTile.y] = spacecounter;
					areacounter++;
				}
				if (ThisTile.x < xmin) {xmin = ThisTile.x;}
				if (ThisTile.x > xmax) {xmax = ThisTile.x;}
				if (ThisTile.y < ymin) {ymin = ThisTile.y;}
				if (ThisTile.y > ymax) {ymax = ThisTile.y;}
				foreach (var offset in offsets) {
					Vector2 toCheck = new Vector2 (ThisTile.x + offset.x, ThisTile.y + offset.y);	
					if (toCheck.x >= 0 && toCheck.y >= 0 && toCheck.x < width && toCheck.y < height) {
						if (SpaceMaze [(int)toCheck.x, (int)toCheck.y] == 0) {
							_spacetiles.Push (toCheck);
						}
					}
				}
			}
			Spaces.Add(new Vector4(spacecounter,areacounter,(xmax+1)-xmin,(ymax+1)-ymin));
			spacecounter++;
			for (int x = 0; x < width; x++) {
				for (int y = 0; y < height; y++) {
					if (SpaceMaze [x, y] == 0) {
						breaker = false;
						_spacetiles.Push(new Vector2(x,y));
						break;
					}
					if (breaker == false) {break;}
				}}
			if (breaker == true) {break;}
		}


		string ss = "";
		for (int i = 0; i < rowLength; i++) {
			for (int j = 0; j < colLength; j++) {
				Char c = (Char)((false ? 65 : 97) + (SpaceMaze[i, j] - 1));
				if (SpaceMaze[i,j] == -1) {c = '0';}
				ss = String.Concat (ss, String.Format ("{0}", c));
			}
			ss = String.Concat (ss, String.Format ("{0}", Environment.NewLine));
		}

		List<int> smallrooms = new List<int>();
		List<int> largerooms = new List<int>();
		List<int> smallpaths = new List<int>();
		List<int> largepaths = new List<int>();






		int roomdim = Mathf.Min (width, height);
		foreach (var ThisSpace in Spaces) {
			if (ThisSpace.y < 9) {
				smallrooms.Add ((int)ThisSpace.x);
			}
			else if((ThisSpace.z >= Mathf.Floor(0.15f*roomdim)+1f) || (ThisSpace.w >= Mathf.Floor(0.15f*roomdim)+1f)) {
				if (ThisSpace.y >= 0.5f* ThisSpace.z * ThisSpace.w && ThisSpace.y < 0.3*width*height) {
					largerooms.Add ((int)ThisSpace.x);
				} else {
					largepaths.Add ((int)ThisSpace.x);
				}
			}
			else {
				smallpaths.Add ((int)ThisSpace.x);
			}
		}

		List<Vector2>[,] neighborhell = new List<Vector2>[Spaces.Count,Spaces.Count];
		int[,] hasdoor = new int[Spaces.Count,Spaces.Count];

		foreach (var space in Spaces) {
			foreach (var space2 in Spaces) {
				neighborhell [(int)space.x-1, (int)space2.x-1] = new List<Vector2> ();
			}
			for (int x = 2; x < width - 2; x++) {
				for (int y = 2; y < height - 2; y++) {
					if (SpaceMaze[x, y] == space.x) {
						foreach (var offset in offsets) {
							Vector2 toCheck1 = new Vector2 (x + offset.x, y + offset.y);	
							if (SpaceMaze [(int)toCheck1.x, (int)toCheck1.y] == -1 && toCheck1.x > 0 && toCheck1.x < width-1 && toCheck1.y > 0 && toCheck1.x < height-1) {
								Vector2 toCheck2 = new Vector2 (toCheck1.x + offset.x, toCheck1.y + offset.y);
								if (SpaceMaze [(int)toCheck2.x, (int)toCheck2.y] > 0 && SpaceMaze [(int)toCheck2.x, (int)toCheck2.y] != space.x) {
									neighborhell[(int)space.x-1, SpaceMaze[(int)toCheck2.x,(int)toCheck2.y]-1].Add(new Vector2(toCheck1.x, toCheck1.y));
									break;
								}
							}
						}
					}
				}
			}
		}

		List<Vector2>[] doorhell = new List<Vector2>[Spaces.Count];
		for (int i = 0; i < Spaces.Count; i++) {
			doorhell[i] = new List<Vector2>(); }

		for (int i = 0; i < largerooms.Count; i++) {
			int temp = largerooms [i];
			int randomIndex = UnityEngine.Random.Range (i, largerooms.Count);
			largerooms [i] = largerooms [randomIndex];
			largerooms [randomIndex] = temp;
		}
		//doors for small rooms
		for (int n = 0; n < smallrooms.Count; n++) {
			int smallroom = smallrooms [n];
			bool haswife = false;
			for (int o = 0; o < largerooms.Count; o++) {
				int largeroom = largerooms [o];
				if (neighborhell [smallroom - 1, largeroom - 1].Count > 0) {
					List<Vector2> temp = neighborhell [smallroom - 1, largeroom - 1];
					Vector2 doorplacement = temp [UnityEngine.Random.Range (0, neighborhell [smallroom - 1, largeroom - 1].Count)];
					Maze [(int)doorplacement.x, (int)doorplacement.y] += 5;
					hasdoor [smallroom - 1, largeroom - 1] = 1;
					hasdoor [largeroom - 1, smallroom - 1] = 1;
					doorhell [smallroom-1].Add (doorplacement);
					doorhell [largeroom-1].Add (doorplacement);
					haswife = true;
					break;
				}
			}
			if (!haswife) { 
				for (int o = 0; o < largepaths.Count; o++) {
					int largepath = largepaths [0];
					if (neighborhell [smallroom - 1, largepath - 1].Count > 0) {
						List<Vector2> temp = neighborhell [smallroom - 1, largepath - 1];
						Vector2 doorplacement = temp [UnityEngine.Random.Range (0, neighborhell [smallroom - 1, largepath - 1].Count)];
						Maze [(int)doorplacement.x, (int)doorplacement.y] += 5;
						hasdoor [smallroom - 1, largepath - 1] = 1;
						hasdoor [largepath - 1, smallroom - 1] = 1;
						doorhell [smallroom-1].Add (doorplacement);
						doorhell [largepath-1].Add (doorplacement);
						haswife = true;
						break;
					}
				}
			}
			if (!haswife) { 
				for (int o = 0; o < smallrooms.Count; o++) {
					int smallroom2 = smallrooms [o];
					if (neighborhell [smallroom - 1, smallroom2 - 1].Count > 0) {
						List<Vector2> temp = neighborhell [smallroom - 1, smallroom2 - 1];
						Vector2 doorplacement = temp [UnityEngine.Random.Range (0, neighborhell [smallroom - 1, smallroom2 - 1].Count)];
						Maze [(int)doorplacement.x, (int)doorplacement.y] += 5;
						doorhell [smallroom-1].Add (doorplacement);
						doorhell [smallroom2-1].Add (doorplacement);
						hasdoor [smallroom - 1, smallroom2 - 1] = 1;
						hasdoor [smallroom2 - 1, smallroom - 1] = 1;
						break;
					}
				}
			}
		}

		//doors for large rooms
		for (int n = 0; n < largerooms.Count; n++) {
			int largeroom = largerooms[n];
			int doorcount = 0;

			for (int i = 0; i < Spaces.Count; i++) {
				if (doorcount == 2) {
					break;
				}
				if (neighborhell [largeroom-1, i].Count > 0) {
					if (largerooms.Contains (i + 1) && hasdoor [largeroom - 1, i] == 0) {
						bool check = false;
						for (int j = 0; j < Spaces.Count; j++) {
							if (hasdoor [i, j] == 1 && (largerooms.Contains (j + 1) || largepaths.Contains (j + 1) || smallpaths.Contains (j + 1))) {
								check = true;
							}
							if (check) {
								break;
							}
						}
						if (!check) {
							List<Vector2> temp = neighborhell [largeroom - 1, i];
							Vector2 doorplacement = temp [UnityEngine.Random.Range (0, neighborhell [largeroom - 1, i].Count)];
							Maze [(int)doorplacement.x, (int)doorplacement.y] += 5;
							doorhell [largeroom-1].Add (doorplacement);
							doorhell [i].Add (doorplacement);
							hasdoor [largeroom - 1, i] = 1;
							hasdoor [i, largeroom - 1] = 1;
							doorcount++;
						}
					}
				}
			}

			for (int i = 0; i < Spaces.Count; i++) {
				if (doorcount == 2) {
					break;
				}
				if (neighborhell [largeroom - 1, i].Count > 0) {
					if (smallpaths.Contains (i + 1) && hasdoor [largeroom - 1, i] == 0) {
						List<Vector2> temp = neighborhell [largeroom - 1, i];
						Vector2 doorplacement = temp [UnityEngine.Random.Range (0, temp.Count)];
						Maze [(int)doorplacement.x, (int)doorplacement.y] += 5;
						doorhell [largeroom-1].Add (doorplacement);
						doorhell [i].Add (doorplacement);
						hasdoor [largeroom - 1, i] = 1;
						hasdoor [i, largeroom - 1] = 1; 
						doorcount++;
					}
				}
			}
			for (int i = 0; i < Spaces.Count; i++) {
				if (doorcount == 2) {
					break;
				}
				if (neighborhell [largeroom - 1, i].Count > 0) {
					 if (largepaths.Contains (i + 1) && hasdoor[largeroom - 1, i] == 0) {
						List<Vector2> temp = neighborhell [largeroom - 1, i];
						Vector2 doorplacement = temp [UnityEngine.Random.Range (0, neighborhell [largeroom - 1, i].Count)];
						Maze [(int)doorplacement.x, (int)doorplacement.y] += 5;
						doorhell [largeroom-1].Add (doorplacement);
						doorhell [i].Add (doorplacement);
						hasdoor [largeroom - 1, i] = 1;
						hasdoor [i, largeroom - 1] = 1;
						doorcount++;}
				}
			}
		}


		//doors for small paths
		for (int n = 0; n < smallpaths.Count; n++) {
			int smallpath = smallpaths[n];
			for (int i = 0; i < Spaces.Count; i++) {
				if (neighborhell [smallpath - 1, i].Count > 0) {
					if (largepaths.Contains (i + 1) && hasdoor [smallpath - 1, i] == 0) {
						List<Vector2> temp = neighborhell [smallpath - 1, i];
						Vector2 doorplacement = temp [UnityEngine.Random.Range (0, neighborhell [smallpath - 1, i].Count)];
						Maze [(int)doorplacement.x, (int)doorplacement.y] += 5;
						doorhell [smallpath-1].Add (doorplacement);
						doorhell [i].Add (doorplacement);
						hasdoor [smallpath - 1, i] = 1;
						hasdoor [i, smallpath - 1] = 1;
					} else if (largerooms.Contains (i + 1) && hasdoor [smallpath - 1, i] == 0) {
						List<Vector2> temp = neighborhell [smallpath - 1, i];
						Vector2 doorplacement = temp [UnityEngine.Random.Range (0, neighborhell [smallpath - 1, i].Count)];
						Maze [(int)doorplacement.x, (int)doorplacement.y] += 5;
						doorhell [smallpath-1].Add (doorplacement);
						doorhell [i].Add (doorplacement);
						hasdoor [smallpath - 1, i] = 1;
						hasdoor [i, smallpath - 1] = 1;
					}

				}
			}
		}

			//doors for large paths
			for (int n = 0; n < largepaths.Count; n++) {
				int largepath = largepaths[n];
				for (int i = 0; i < Spaces.Count; i++) {
					if (neighborhell [largepath - 1, i].Count > 0) {
						if (largepaths.Contains (i + 1) && hasdoor [largepath - 1, i] == 0) {
							List<Vector2> temp = neighborhell [largepath - 1, i];
							Vector2 doorplacement = temp [UnityEngine.Random.Range (0, neighborhell [largepath - 1, i].Count)];
							Maze [(int)doorplacement.x, (int)doorplacement.y] += 5;
							doorhell [largepath-1].Add (doorplacement);
							doorhell [i].Add (doorplacement);
							hasdoor [largepath - 1, i] = 1;
							hasdoor [i, largepath - 1] = 1;
						}

					}
				}
			}


					
		string s = "";
		for (int i = 0; i < rowLength; i++) {
			for (int j = 0; j < colLength; j++) {
				s = String.Concat (s, String.Format ("{0}", Maze [i, j]));
			}
			s = String.Concat (s, String.Format ("{0}", Environment.NewLine));
		}




		List<List<int>> SectorLevels = new List<List<int>> ();
		List<int> possibleExitSectors = new List<int> ();
		List<int> newpes = new List<int> ();
		List<int> usedpes = new List<int> ();
		possibleExitSectors.Add (0);

		while (true) {
			for (int i = 0; i < possibleExitSectors.Count; i++) {
				for (int j = 0; j < Spaces.Count; j++) {
					if ((hasdoor [possibleExitSectors [i], j] == 1) && (usedpes.Contains (j) == false) && (usedpes.Contains (i) == false) && possibleExitSectors.Contains(j) == false && newpes.Contains(j) == false) {
						newpes.Add (j);
					}
				}
			}

			if (newpes.Count == 0) {
				break;
			} else if (newpes.Count == 1) {
				List<int> thislv = new List<int> ();
				foreach (var q in possibleExitSectors) {
					thislv.Add (q);
				}
				SectorLevels.Add (thislv);

				possibleExitSectors.Clear ();
				possibleExitSectors = newpes;
				break;
			} else {
				List<int> thislv = new List<int> ();
				foreach (int q in possibleExitSectors) {
					usedpes.Add (q);
					thislv.Add (q);

				}
				SectorLevels.Add (thislv);
				possibleExitSectors.Clear ();
				foreach (var q in newpes) {
					possibleExitSectors.Add (q);
				}
				newpes.Clear ();
			}
		}


	int exitSector = possibleExitSectors [UnityEngine.Random.Range (0, possibleExitSectors.Count - 1)] + 1;
	if (possibleExitSectors.Count > 1) {
	 	possibleExitSectors.Remove (exitSector-1);
			SectorLevels.Add (possibleExitSectors);
			List<int> tteeppo = new List<int> ();
			tteeppo.Add (exitSector-1);
			SectorLevels.Add (tteeppo);
		} else {
			SectorLevels.Add (possibleExitSectors);
		}


		Spaces.Sort((a, b) => a.x.CompareTo(b.x));
		int keycount = 0;

		for (int i = 0; i < SectorLevels.Count; i++) {
			if (SectorLevels [i].Contains (exitSector-1)) {
				foreach (var door in doorhell[exitSector-1]) {
					SpaceMaze [(int)door.x, (int)door.y] = (i * -1) -1;
				}
			}
			else {
			
			for (int j = 0; j < SectorLevels [i].Count; j++) {
				int currentsector = SectorLevels [i] [j];
				if (i == 0) {
						int keysector = SectorLevels [0] [0];
						int tilecount = 0;
						int keytile = UnityEngine.Random.Range (0, (int)(Spaces [keysector].y / 2f)) + (int)(Spaces [keysector].y / 4f);
						bool keyhome = false;
						for (int x = 0; x < width; x++) {
							for (int y = 0; y < height; y++) {
								if (SpaceMaze [x, y] == keysector + 1) {
									if (tilecount < keytile) {
										tilecount++;
									} else {
										SpaceMaze [x, y] = (i * -1) - 2;
										keycount++;
										keyhome = true;
										break;

									}
								}
								if (keyhome) {
									break;
								}
							}
						}

				} else {
					foreach (var door in doorhell[currentsector]) {
						SpaceMaze [(int)door.x, (int)door.y] = (i * -1) -1;
					}
				}
			}
				if ((i > 0) && (SectorLevels [i].Count > 0)) {
					int keysector = SectorLevels [i] [UnityEngine.Random.Range (0, SectorLevels [i].Count - 1)];

					int keytile = UnityEngine.Random.Range (0, (int)(Spaces [keysector].y / 2f)) + (int)(Spaces [keysector].y / 4f);
					int tilecount = 0;
					bool keyhome = false;
					for (int x = 0; x < width; x++) {
						for (int y = 0; y < height; y++) {
							if (SpaceMaze [x, y] == keysector + 1) {
								if (tilecount < keytile) {
									tilecount++;
								} else {
									SpaceMaze [x, y] = (i * -1) - 2;
									keycount++;
									keyhome = true;
									break;

								}
							}
							if (keyhome) {
								break;
							}
						}
					}
				}
			}
		}

		while (keycount < SectorLevels.Count - 1) {
			int keysector = SectorLevels [0] [0];
			int tilecount = 0;
			int keytile = UnityEngine.Random.Range (0, (int)(Spaces [keysector].y) - (int)(Spaces [keysector].y / 4f));
			bool keyhome = false;
			for (int x = 0; x < width; x++) {
				for (int y = 0; y < height; y++) {
					if (SpaceMaze [x, y] == keysector + 1) {
						if (tilecount < keytile) {
							tilecount++;
						} else {
							SpaceMaze [x, y] = (keycount * -1) - 1;
							keycount++;
							keyhome = true;
							break;

						}
					}
					if (keyhome) {
						break;
					}
				}
			}
		}
				



		bool setset = false;
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				if (SpaceMaze [x, y] == exitSector) {
					Maze [x, y] = 9;
					setset = true;
					break;
				}
			}
			if (setset) {break;}
		}

		for (int q = 0; q < guards; q++) {
			int tiltiltil = (int) (UnityEngine.Random.Range(0, width * height) *0.75 +0.25*width * height);
			int countcount = 0;
			bool setsetset = false;
			for (int x = 0; x < width; x++) {
				for (int y = 0; y < height; y++) {
					countcount++;
					if (countcount >= tiltiltil) {
							if (Maze [x, y] == 0 && SpaceMaze [x, y] > 0) {
							SpaceMaze [x, y] = 100;
							setsetset = true;
							break;}
					}
				}
				if (setsetset) {
					break;
				}
						
			}






		}





















































		ss = "";
		for (int i = 0; i < rowLength; i++) {
			for (int j = 0; j < colLength; j++) {
				//Char c = (Char)((false ? 65 : 97) + (SpaceMaze[i, j] - 1));
				//if (SpaceMaze[i,j] == -1) {c = '0';}
				ss = String.Concat (ss, String.Format ("{0}", SpaceMaze[i, j]));
			}
			ss = String.Concat (ss, String.Format ("{0}", Environment.NewLine));
		}












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
					if (Maze [i, j] == 2) {
						ptype.transform.localScale = new Vector3 (2.25f, 1, 0.25f);
						if (i > 0) {
							if (Maze [i - 1, j] == 7) {
								ptype.transform.localScale = new Vector3 (1.5f, 1, 0.25f);
								ptype.transform.position = ptype.transform.position + new Vector3 (0.375f, 0, 0);
							}
						}
						if (i < Maze.GetUpperBound (0) - 1) {
							if (Maze [i + 1, j] == 7) {
								ptype.transform.localScale = new Vector3 (1.5f, 1, 0.25f);
								ptype.transform.position = ptype.transform.position + new Vector3 (-0.375f, 0, 0);
							}
						}
					}
		
					if (Maze [i, j] == 3) {
						ptype.transform.localScale = new Vector3 (0.25f, 1, 2.25f);
						if (j > 0) {
							if (Maze [i, j-1] == 8) {
								ptype.transform.localScale = new Vector3 (0.25f, 1, 1.5f);
								ptype.transform.position = ptype.transform.position + new Vector3 (0, 0, 0.375f);
							}
						}
						if (j < Maze.GetUpperBound (1) - 1) {
							if (Maze [i , j+1] == 8) {
								ptype.transform.localScale = new Vector3 (0.25f, 1, 1.5f);
								ptype.transform.position = ptype.transform.position + new Vector3 (0, 0, -0.375f);
							}
						}
					}
					if (Maze [i, j] == 7) {
						ptype.transform.localScale = new Vector3 (0.75f, 1, 0.25f);
						ptype.transform.position = ptype.transform.position + new Vector3 (-0.75f, 0f, 0f);

						GameObject ptype2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
						ptype2.GetComponent<Renderer>().material.color = Color.blue;
						ptype2.transform.localScale = new Vector3 (0.75f, 1, 0.25f);
						ptype2.transform.position = ptype.transform.position + new Vector3 (1.5f, 0f, 0f);

						GameObject ptype1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
						GameObject ptype4 = new GameObject ();
						ptype4.AddComponent (typeof(OpenDoorWithKey));
						ptype4.AddComponent (typeof(BoxCollider));
						BoxCollider bc = ptype4.GetComponent (typeof(BoxCollider)) as BoxCollider;
						bc.isTrigger = true;
						bc.size = new Vector3 (1, 2, 0.25f);
						bc.center = new Vector3 (0.35f, 0, 0);
						OpenDoorWithKey odwk = ptype4.GetComponent(typeof(OpenDoorWithKey)) as OpenDoorWithKey;
						odwk.Door = ptype1;
						int lvvvv = SpaceMaze [i, j];
						odwk.level = (lvvvv+1) * -1;


						ptype1.transform.parent = ptype4.transform;
						ptype1.GetComponent<Renderer>().material.color = Color.white;
						ptype1.transform.localScale = new Vector3 (0.75f, 1, 0.075f);
						ptype4.transform.position = ptype.transform.position + new Vector3 (0.75f, 0f, 0f);
						ptype1.transform.position = ptype1.transform.position + new Vector3 (0.375f, 0f, 0f);
						ptype4.transform.position = ptype4.transform.position + new Vector3 (-0.375f, 0f, 0f);


					}
					if (Maze [i, j] == 8) {
						ptype.transform.localScale = new Vector3 (0.25f, 1, 0.75f);
						ptype.transform.position = ptype.transform.position + new Vector3 (0, 0f, -0.75f);

						GameObject ptype2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
						ptype2.GetComponent<Renderer>().material.color = Color.blue;
						ptype2.transform.localScale = new Vector3 (0.25f, 1, 0.75f);
						ptype2.transform.position = ptype.transform.position + new Vector3 (0, 0f, 1.5f);

						GameObject ptype1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
						GameObject ptype4 = new GameObject ();
						ptype4.AddComponent (typeof(OpenDoorWithKey));
						ptype4.AddComponent (typeof(BoxCollider));
						BoxCollider bc = ptype4.GetComponent (typeof(BoxCollider)) as BoxCollider;
						bc.isTrigger = true;
						bc.size = new Vector3 (0.25f, 2, 1);
						bc.center = new Vector3 (0, 0, 0.35f);
						OpenDoorWithKey odwk = ptype4.GetComponent(typeof(OpenDoorWithKey)) as OpenDoorWithKey;
						odwk.Door = ptype1;
						int lvvvv = SpaceMaze [i, j];
						odwk.level = (lvvvv+1) * -1;

						ptype1.transform.parent = ptype4.transform;
						ptype1.GetComponent<Renderer>().material.color = Color.white;
						ptype1.transform.localScale = new Vector3 (0.075f, 1, 0.75f);
						ptype4.transform.position = ptype.transform.position + new Vector3 (0, 0f, 0.75f);
						ptype1.transform.position = ptype1.transform.position + new Vector3 (0f, 0f, 0.375f);
						ptype4.transform.position = ptype4.transform.position + new Vector3 (0f, 0f, -0.375f);


					}
					if (Maze [i, j] == 9) {
						Destroy (ptype);
						ptype = GameObject.CreatePrimitive(PrimitiveType.Plane);
						ptype.GetComponent<Renderer>().material.color = Color.black;
						ptype.transform.localScale = new Vector3 (0.1f, 1, 0.1f);
						ptype.AddComponent (typeof(BoxCollider));
						BoxCollider bc = ptype.GetComponent (typeof(BoxCollider)) as BoxCollider;
						bc.isTrigger = true;
						bc.size = new Vector3 (2, 2, 2);
						ptype.AddComponent (typeof(FinishLoadNextLevel));
						ptype.transform.position = new Vector3(i , 0, j * ptype.transform.localScale.y) + new Vector3(0,-0.499f,0);


					}


					if (brick != null)
					{
						ptype.GetComponent<Renderer>().material = brick;
					}
					ptype.transform.parent = transform;
					ptype.layer = 9;
				}

				else if (Maze[i, j] == 0)
				{
					if (SpaceMaze [i, j] < -1) {
						var qqq = Instantiate (Key, new Vector3 (i, 0, j), Quaternion.identity);
						KeyKey qq = qqq.GetComponent (typeof(KeyKey)) as KeyKey;
						qq.Key = (SpaceMaze [i, j] + 1) * -1;

					}
					if (SpaceMaze [i, j] == 100) {
						var qqq = Instantiate (Guard, new Vector3 (i, 0, j), Quaternion.identity);
						GuardTestMovement1 qq = qqq.GetComponent (typeof(GuardTestMovement1)) as GuardTestMovement1;
						qq.MazeGen = this;
						//qq.Key = (SpaceMaze [i, j] + 1) * -1;
					}

					pathMazes.Add(new Vector3(i, 0, j));
				}

			}
		}






						








	}


	void GenerateGuards () {




		/*
		for (int i = 1; i <= guards; i++) {
			int ex = -1;
			int wy = -1;
			while (Maze[ex+1,wy+1] > 0) {
				ex = UnityEngine.Random.Range (0, width);
				wy = UnityEngine.Random.Range (0, height);
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


*/
	}




	public int[,] setRoom()
	{
		int roomdim = Mathf.Min (width, height);
		int roomwidth, roomheight;
		roomwidth = UnityEngine.Random.Range ((int)Mathf.Floor(0.15f*(float)roomdim)+2,(int)Mathf.Floor(0.25f*(float)roomdim)+2);
		roomheight = UnityEngine.Random.Range ((int)Mathf.Floor(0.15f*(float)roomdim)+2,(int)Mathf.Floor(0.25f*(float)roomdim)+2);
		int startx = UnityEngine.Random.Range ((int)Mathf.Floor(0.15f*(float)roomdim)+1, width - (int)Mathf.Floor(0.25f*(float)roomdim)-1);
		int starty = UnityEngine.Random.Range ((int)Mathf.Floor(0.15f*(float)roomdim)+1, height - (int)Mathf.Floor(0.25f*(float)roomdim)-1);



		for (int i = startx; i < startx+roomwidth; i++) {
			for (int j = starty; j < starty+roomheight; j++) {
				Maze [i, j] = 0;
				if (i == startx || j == starty)
					Maze [i, j] = 1;
					tMaze[i, j] = -1;
				if (i == startx+roomwidth - 1)
					Maze [i, j] = 1;
					tMaze[i, j] = -1;
				if (j == starty+roomheight - 1)
					Maze [i, j] = 1;
					tMaze[i, j] = -1;
			}
		}

		for (int i = startx; i < startx+roomwidth; i++) {
			if (tMaze [i, starty+roomheight] != 3 && tMaze[i, starty+roomheight] != -1) {
				Maze [i, starty+roomheight] = 0;
			}
			if (tMaze [i, starty - 1] != 3 && tMaze[i, starty - 1] != -1) {
				Maze [i, starty - 1] = 0;
			}
		}

		for (int i = starty; i < starty+roomheight; i++) {
			if (tMaze [startx-1, i] != 2 && tMaze[startx-1, i] != -1) {
				Maze [startx-1, i] = 0;
			}
			if (tMaze [startx+roomwidth, i] != 2 && tMaze[startx+roomwidth, i] != -1) {
				Maze [startx+roomwidth, i] = 0;
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
