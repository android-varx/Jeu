using Godot;

public partial class player : Node
{
	[Export] private player_input _input = null; // Varibale pour stocker les entrées du joueur, exposées dans l'éditeur 
	[Export] private Character _character = null; // Variable pour le personnage joueur, exposée dans l'éditeur

	public override void _Process(double delta) //Méthode appelé à chaque frame 
	{
		_character.SetMovementInput(_input.MovementInput); // Appel la méthode 
	}
}
