using Godot;

public partial class camera_follow : Node2D
{
	[Export] public Node2D _objectToFollow = null; // L'objet que la caméra doit suivre, assignable dans l'éditeur 

	public override void _Ready()
	{
		// Positionne la caméra sur le joueur dès le début
		if (_objectToFollow != null)
		{
			Position = _objectToFollow.Position;
		}
	}
	public override void _Process(double delta) // Méthode appelée à chaque frame
	{
		if (_objectToFollow != null)
		{
			Position = _objectToFollow.Position.Round(); // Met à jour la position de la caméra en fonction de l'objet choisit 
		}
	}
}


