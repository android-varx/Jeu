using Godot;
using System.Net;
using EternalForest.Game;


public partial class RejoindreMenu : Control
{
    
    private LineEdit _ipInput; // Champ de saisie pour l'IP
    public override void _Ready()
    {
        Button rejoindreButton = GetNode<Button>("Se Connecter");
        Button retourButton = GetNode<Button>("Retour");

        // Connexion des signaux avec les boutons
        rejoindreButton.Pressed += OnRejoindrePressed;
        retourButton.Pressed += OnRetourPressed;
        
        _ipInput = GetNode<LineEdit>("IPInput");

    }
    
    private void OnRejoindrePressed()
    {
        GetNode<GameManager>("/root/GameManager").IsNewGame = false;
        GameMultiplayer.IsServer = false;
        string ip = _ipInput.Text.Trim();
        
        GameMultiplayer.ip = ip;
        GetTree().ChangeSceneToFile("res://scenes/GameMultiplayer.tscn"); // start la scene multiplayer
    }
    
    private void OnRetourPressed()
    {
        GetTree().ChangeSceneToFile("res://UI/MultiplayerMenu.tscn"); //pour retourner au menu principal 
    }
}