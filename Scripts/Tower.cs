using Godot;
using System;

public class Tower : TileMap
{

	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";
	[Export]
	private int width = 5;
	[Export]
	private int height = 5;
	[Export]
	private int roomWidth = 8;
	[Export]
	private int roomHeight = 8;

	/// <summary>
	/// The raw cells for the entire tower that make up the rooms
	/// </summary>
	private int[,] TowerCells;
	/// <summary>
	/// The rooms for the entire tower
	/// </summary>
	private int[,] TowerRooms;

	private Button genTower;
    private Position2D PlayerStart;
    private int startingRoomX, startingRoomY;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
		GenTower();
		CreateTileMap();
        PrintTower();
		PlayerStart = (Position2D)GetNode("./PlayerStart");
        PlayerStart.SetGlobalPosition(new Vector2(roomWidth * startingRoomX * 16, roomHeight * startingRoomY * 16)
        + new Vector2(roomWidth * 16f * 0.5f, roomHeight * 16f * 0.5f));
    }


	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		if (Input.IsActionJustPressed("ui_accept"))
		{
			GD.Print("New tower");
			TowerRooms = GenTowerRoomPath();
			//TowerRooms = ConnectTowerPath(TowerRooms);

			
		}
	}

	public void CreateTileMap() 
	{
		SetCellSize(new Vector2(16,16));
		for (int x = 0; x < width * roomWidth; x++) 
		{
			for (int y = 0; y < height* roomHeight; y++) 
			{
				SetCell(x, y, TowerCells[x,y] == 0 ? -1 : 1);
			}
		}
		UpdateDirtyQuadrants();

	}

	public void GenTower()
	{
		TowerRooms = GenTowerRoomPath();
        PrintTowerRooms();
        TowerCells = new int[width * roomWidth, height * roomHeight];
		//GD.Print($"{width * roomWidth}, {height * roomHeight}");
		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
                var room = TowerRooms[x, y];
                var type = (RoomType)room;
                GenRoomInTower(type, x*roomWidth, y*roomHeight);
			}
		}
	}

	public void GenRoomInTower(RoomType type, int startX, int startY)
	{
		if (type == RoomType.Null)
		{
			for (int y = 0; y < roomHeight; y++)
			{
				for (int x = 0; x < roomWidth; x++)
				{
					TowerCells[startX + x, startY + y] = 1;
				}
			}
			return;
		}
		var file = new File();
		file.Open($"res://Templates/{type.ToString()}.txt", (int)File.ModeFlags.Read);
		GD.Print(type.ToString());
		string[] room = file.GetAsText().Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
		//GD.Print(room);
		for (int y = 0; y < roomHeight; y++)
		{ 
			for (int x = 0; x < roomWidth; x++)
			{
				if (y == room.Length || x == room[y].Length) 
				{
					GD.Print("heck");

				}
				//GD.Print($"{x}, {y}");
				var cell = room[y][x];
				
				TowerCells[startX + x, startY + y] = int.Parse(cell.ToString());

				
			}
		}
	}

	public int[,] GenTowerRoomPath()
	{
		var TowerPath = new int[width, height];
		var rnd = new Random();
		int x = rnd.Next(0, width);
		int y = height - 1;
        startingRoomX = x;
        startingRoomY = y;
        bool finishedPath = false;
		Exit direction = PickDirection(rnd);
		while (!finishedPath)
		{
			bool validDir = true;
			do
			{
				validDir = true;
				direction = PickDirection(rnd);

				TowerPath[x, y] = (int)RoomType.LeftRight;
				if (y != height - 1 
					&& (TowerPath[x,y+1] == (int)RoomType.LeftRightTop || TowerPath[x, y + 1] == (int)RoomType.LeftRightBottomTop))
				{
					TowerPath[x, y] = (int)RoomType.LeftRightBottom;
				}
				switch (direction)
				{
					case Exit.Up:
						if (y == 0)
						{
							finishedPath = true;
							break;
						}
						if (TowerPath[x,y] == (int)RoomType.LeftRightBottom)
						{
							TowerPath[x, y] = (int)RoomType.LeftRightBottomTop;
						}
						else
						{
							TowerPath[x, y] = (int)RoomType.LeftRightTop;
						}
						

						y--; //Go up
						break;
					case Exit.Right:
						if (x == width - 1)
						{
							validDir = false;
							break;
						}
						x++; //Go right
						break;
					case Exit.Down:
						if (y == height - 1)
						{
							validDir = false;
							break;
						}
						y++; //Go down
						break;
					case Exit.Left:
						if (x == 0)
						{
							validDir = false;
							break;
						}
						x--; //Go left
						break;
				}
			} while (!validDir);

		}
		return TowerPath;
	}



	public Exit PickDirection(Random rnd)
	{
		var num = rnd.Next(1, 11);
		Exit direction;
		if (num <= 6)
		{
			direction = rnd.Next(0, 2) == 0 ? Exit.Left : Exit.Right;
		}
		else
		{
			direction = Exit.Up;
		}
		return direction;
	}

	public void PrintTowerRooms()
	{
		for (int y = 0; y < height; y++)
		{
			string line = "";
			for (int x = 0; x < width; x++)
			{
				Char cell = '█';
				switch (TowerRooms[x, y])
				{
					case (int)RoomType.LeftRight:
						cell = '─';
						break;
					case (int)RoomType.LeftRightBottom:
						cell = '┬';
						break;
					case (int)RoomType.LeftRightBottomTop:
						cell = '┼';
						break;
					case (int)RoomType.LeftRightTop:
						cell = '┴';
						break;
				}
				line += cell;
			}
			GD.Print(line);
		}
	}
	public void PrintTower()
    {
        for (int y = 0; y < height*roomHeight; y++)
        {
            string line = "";
            for (int x = 0; x < width*roomWidth; x++)
            {
                String cell = TowerCells[x, y].ToString();

                line += cell;
            }
            GD.Print(line);
        }
    }
	//  // Called every frame. 'delta' is the elapsed time since the previous frame.
	//  public override void _Process(float delta)
	//  {
	//      
	//  }
}

public class Room
{
	public bool[] Exits;
	const int UP = 0;
	const int RIGHT = 1;
	const int DOWN = 2;
	const int LEFT = 3;
	public RoomType Type;
	public Room(RoomType type)
	{
		switch (type)
		{
			case RoomType.Side:
				break;
			case RoomType.LeftRight:
				Exits[LEFT] = true;
				Exits[RIGHT] = true;
				break;
			case RoomType.LeftRightBottom:
				Exits[LEFT] = true;
				Exits[RIGHT] = true;
				Exits[DOWN] = true;
				break;
			case RoomType.LeftRightTop:
				Exits[LEFT] = true;
				Exits[RIGHT] = true;
				Exits[UP] = true;
				break;
		}
	}
}

public enum RoomType
{
	Null,
	Side,
	LeftRight,
	LeftRightBottom,
	LeftRightTop,
	LeftRightBottomTop
}

public enum Exit
{
	Up,
	Right,
	Down,
	Left

}
