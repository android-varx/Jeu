using Godot;
using EternalForest.Game;

public partial class HebergeMenu : Control
{
    
    public override void _Ready()
    {
        Button nouveauButton = GetNode<Button>("VBoxContainer/Nouveau");
        Button retourButton = GetNode<Button>("VBoxContainer/Retour");
        
        nouveauButton.Pressed += OnNouveauPressed;
        retourButton.Pressed += OnRetourPressed;
    }
    
    private async void OnNouveauPressed()
    {
        GetNode<GameManager>("/root/GameManager").IsNewGame = false;
        GameMultiplayer.IsServer = true;
        GetTree().ChangeSceneToFile("res://scenes/GameMultiplayer.tscn");
    }

    private void OnRetourPressed()
    {
        GetTree().ChangeSceneToFile("res://UI/MultiplayerMenu.tscn"); // retour au menu multi
    }
}