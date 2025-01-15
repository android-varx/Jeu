using Godot;
using System;

public partial class floor_gen : Node
{
	[Export] private PackedScene topLeftScene;
	[Export] private PackedScene topScene;
	[Export] private PackedScene topRightScene;
	[Export] private PackedScene leftScene;
	[Export] private PackedScene rightScene;
	[Export] private PackedScene downLeftScene;
	[Export] private PackedScene downScene;
	[Export] private PackedScene downRightScene;
	[Export] private PackedScene floorScene;
	
	private int mapWidth = 9984;  // On definit la largeur 
	private int mapHeight = 9984; // on definit la hauteur 
	private int tilesize = 32;
	
	public override void _Ready()
	{
		GenerateFloor();
	}
	private void GenerateFloor()
        {
            int horizontal = 312;
            int vertical = 312;
    
            for (int x = 0; x < horizontal; x++)
            {
                if (x == 0)
                {
                    PlaceTile(topLeftScene, x, 0);
                }
    
                if (x == horizontal - 1)
                {
                    PlaceTile(topRightScene, x, 0);
                }
                else
                {
                    PlaceTile(topScene, x, 0);
                }
            }
    
            for (int y = 1; y < vertical-1; y++)
            {
                for (int x = 0; x < horizontal; x++)
                {
                    if (x == 0)
                    {
                        PlaceTile(leftScene, 0, y);
                    }
    
                    if (x == horizontal - 1)
                    {
                        PlaceTile(rightScene, x, y);
                    }
                    else
                    {
                        PlaceTile(floorScene, x, y);
                    }
                }
            }
    
            for (int x = 0; x < horizontal; x++)
            {
                if (x == 0)
                {
                    PlaceTile(downLeftScene, x, vertical - 1);
                }
    
                if (x == horizontal - 1)
                {
                    PlaceTile(downRightScene, x, vertical - 1);
                }
                else
                {
                    PlaceTile(downScene, x, vertical - 1);
                }
            }
        }
        
        private void PlaceTile(PackedScene scene, int x, int y)
        {
            Node2D instance = (Node2D)scene.Instantiate();
            instance.Position = new Vector2(x * tilesize, y * tilesize);
            AddChild(instance);
        }
}
