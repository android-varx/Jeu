using Godot;

public partial class player_input : Node
{
	[Export] private string _leftAxis = "left";
	[Export] private string _rightAxis = "right";
	[Export] private string _upAxis = "up";
	[Export] private string _downAxis = "down";
	private Vector2 _movementInput = Vector2.Zero; //Variable pour stocker les input de mouvement , à zér
	public Vector2 MovementInput => _movementInput; //propriété pour acceder aux entrées de mouvement
	
	public override void _Process(double delta) // Méthode appelée à chaque frame 
	{
		//Mise à jour des entrées de mouvement selon les touches préssées
		_movementInput = new Vector2(
			Input.GetAxis(_leftAxis, _rightAxis), //Mouvement Horizontal gauche droite
			Input.GetAxis(_upAxis, _downAxis)); //Mouvement vertical Haut bas 
	}
}
