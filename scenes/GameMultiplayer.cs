using Godot;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public partial class GameMultiplayer : Node2D
{
    [Export] private floor_gen floor_spring;
    [Export] private map_gen_multiplayer map_spring;
    public int Width { get; private set; } = 3328;
    public int Height { get; private set; } = 3328;

    public int count = 1;

    public static string ip;
    
    [Export] private PackedScene _playerScene; // Scène du joueur (préfabriqué)
    private const int Port = 138; // Port d'écoute du serveur
    ENetMultiplayerPeer peer = new ENetMultiplayerPeer();

    public static bool IsServer {get; set;}

    public override void _Ready()
    {
        map_spring.GenerateNewMap(0, 0);
        floor_spring.GenerateNewFloor(0, 0);
        Generate8();
        
        if (IsServer)
        {
            var error = peer.CreateServer(Port);
            if (error != Error.Ok)
            {
                return;
            }

            Multiplayer.MultiplayerPeer = peer;

            Multiplayer.PeerConnected += id =>
            {
                SpawnPlayer((int)id);
            };
            SpawnHost();

        }
        else if (!IsServer)
        {
	        var error = peer.CreateClient(ip, Port);
	        if (error != Error.Ok)
	        {
		        return;
	        }

	        Multiplayer.MultiplayerPeer = peer;
        }

    }

    public void Generate8()
    {
		map_spring.GenerateNewMap(-Width, -Width);
		floor_spring.GenerateNewFloor(-Width, -Width);
		
		map_spring.GenerateNewMap(0, -Width);
		floor_spring.GenerateNewFloor(0, -Width);
		
		map_spring.GenerateNewMap(Width, -Width);
		floor_spring.GenerateNewFloor(Width, -Width);
		
		map_spring.GenerateNewMap(-Width, 0);
		floor_spring.GenerateNewFloor(-Width, 0);
		
		map_spring.GenerateNewMap(Width, 0);
		floor_spring.GenerateNewFloor(Width, 0);
		
		map_spring.GenerateNewMap(-Width, Width);
		floor_spring.GenerateNewFloor(-Width, Width);
		
		map_spring.GenerateNewMap(0, Width);
		floor_spring.GenerateNewFloor(0, Width);
		
		map_spring.GenerateNewMap(Width, Width);
		floor_spring.GenerateNewFloor(Width, Width);
	}

    public void SpawnPlayer(int id)
    {
        Player player = (Player) _playerScene.Instantiate();
        GD.Print("Spawning player: " + id);
        player.Name = id.ToString(); 
        player.SetMultiplayerAuthority(id);
        AddChild(player, true); // Ajoute le nœud avec un nom unique
        if (count == 1)
        {
	        player.GlobalPosition = new Vector2(1000, 1500); // Position initiale du joueur
        }
        else
        {
	        player.GlobalPosition = new Vector2(1500, 1500);
        }
        count += 1;
        if (player.IsMultiplayerAuthority())
        {
            player.GetNode<Camera2D>("Camera2D").Enabled = true;
            player.GetNode<Camera2D>("Camera2D").MakeCurrent();
        }
        else
        {
            player.GetNode<Camera2D>("Camera2D").Enabled = false;
        }
    }

    public void SpawnHost(int id = 1)
    {
        GD.Print("Spawning host: " + id);
        SpawnPlayer(id);
    }
}