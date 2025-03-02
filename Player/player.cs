using Godot;

public partial class player : CharacterBody2D
{
	[Export] public string PlayerName { get; set; }
	[Export] private player_input _input = null; // varibale pour stocker les entrees du joueur 
	[Export] private Character _character = null; // variable pour le personnage joueur
	public Vector2 BodyPosition => _character.GlobalPosition;

	public override void _Process(double delta)
	{
		_character.SetMovementInput(_input.MovementInput);
	}
	
}	
