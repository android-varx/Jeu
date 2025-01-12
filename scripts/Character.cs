using Godot;

public partial class Character : CharacterBody2D
{
	[Export] private float _speed = 600f; //Varibale qui stock la vitesse du mouvement du personnage 
	private Vector2 _movementInput = Vector2.Zero; //Stoke la direction de mouvement, initialisée à zéro 
	public void SetMovementInput(Vector2 input) //Méthode pour gérer l'entrée de mouvement 
	{
		_movementInput = input.Normalized(); //Normalise l'entrée pour un vitesse constante dans toutes les directions
	}
	public override void _PhysicsProcess(double delta) // méthode apelée à chaque frame physique(pour un mouvement précis)
	{
		Velocity = _movementInput * _speed; // calcule la vélocité en fonction de la direction et de la vitesse
		MoveAndSlide(); // Applique le mouvement tout en gérant les collisions
	}
}
