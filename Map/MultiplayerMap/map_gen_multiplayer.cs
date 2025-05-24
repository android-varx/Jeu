using Godot;
using System;

public partial class map_gen_multiplayer : Node2D
{
    [Export] private PackedScene leavesScene1; // On exporte la varibale pour la scene feuilles
    [Export] private PackedScene leavesScene2; // On exporte la varibale pour la scene feuilles
    [Export] private PackedScene rockScene1; // On exporte la varibale pour la scene rochers
    [Export] private PackedScene rockScene2; // On exporte la varibale pour la scene rochers
    [Export] private PackedScene rockScene3; // On exporte la varibale pour la scene rochers
    [Export] private PackedScene Ores1; // On exporte la varibale pour la scene rochers
    [Export] private PackedScene Ores2; // On exporte la varibale pour la scene rochers
    [Export] private PackedScene treeScene1; //On exporte la varibale pour la scene arbres
    [Export] private PackedScene treeScene2; //On exporte la varibale pour la scene arbres
    [Export] private PackedScene treeScene3; //On exporte la varibale pour la scene arbres
    
    private int mapWidth = 3328;  // On definit la largeur 
    private int mapHeight = 3328; // on definit la hauteur 
    
    private int leavesMin = 40, leavesMax = 60;   // Quantite min et max de feuilles
    private int rockMin = 15, rockMax = 40;   // Quantite min et max de cailloux
    private int treeMin = 15, treeMax = 40;   // Quantite min et max arbre
    private int oresMin= 10, oresMax = 20;   // Quantite min et max de fer

    private int seed = 1369181;

    private Random _globalRandom;
    
    public void GenerateNewMap(int x, int y)
    {
        GenerateMap(x, y);
    }
    
    private void GenerateMap(int x, int y)
    {
        _globalRandom = new Random(seed + x * 73856093 + y * 19349663); // mélange les coords pour varier
        
        GenerateObjects(leavesScene1, leavesMin, leavesMax, x, y);
        GenerateObjects(leavesScene2, leavesMin, leavesMax, x, y);
        
        GenerateObjects(rockScene1, rockMin, rockMax, x, y);
        GenerateObjects(Ores2, oresMin, oresMax, x, y);
        GenerateObjects(rockScene2, rockMin, rockMax, x, y);
        GenerateObjects(rockScene3, rockMin, rockMax, x, y);
        GenerateObjects(Ores1, oresMin, oresMax, x, y);
        GenerateObjects(rockScene3, rockMin, rockMax, x, y);
        
        
        GenerateObjects(treeScene1, treeMin, treeMax, x, y);
        GenerateObjects(treeScene2, treeMin, treeMax, x, y);
        GenerateObjects(treeScene3, treeMin, treeMax, x, y);
    }
    
    private void GenerateObjects(PackedScene scene, int minCount, int maxCount, int x, int y)
    {
        // determine le nombre d'objets a generer
        int objectCount = _globalRandom.Next(minCount, maxCount);

        for (int i = 0; i < objectCount; i++)
        {
            Node2D instance = (Node2D)scene.Instantiate();
            
            int a = _globalRandom.Next(x + 100, x + mapWidth - 100); // position x aleatoire
            
            int b = _globalRandom.Next(y + 100, y + mapHeight - 100); // position y aleatoire
            
            // placement de l'objet si ok
            instance.Position = new Vector2(a, b);
            AddChild(instance);
        }
    }
}
