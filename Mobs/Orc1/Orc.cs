using Godot;

public partial class Orc : CharacterBody2D
{
    [Export] private int _speed = 100; // Vitesse de déplacement de l'ennemi
    [Export] private AnimatedSprite2D _sprite; // Référence à l'AnimatedSprite2D
    [Export] private int _attackRange = 66; // Distance à laquelle l'ennemi attaque
    [Export] private int _minDistance = 65; // Distance minimale entre l'orc et le joueur
    [Export] private int _detectionRange = 650; // Distance de détection du joueur
    [Export] private int _patrolDistance = 100; // Distance à parcourir en patrouille

    private Vector2 _movementInput = Vector2.Zero; // Direction de mouvement
    private string _lastDirection = "right"; // Dernière direction pour l'animation
    private player _target; // Référence au joueur
    private bool _isAttacking = false; // Indique si l'orc est en train d'attaquer
    private Timer _attackCooldownTimer; // Timer pour gérer le délai entre les attaques
    private float _currentPatrolDistance = 0.0f; // Distance parcourue en patrouille
    private bool _isPatrolling = false; // Indique si l'ennemi est en train de patrouiller
    private Vector2 _patrolDirection = Vector2.Right; // Direction de patrouille actuelle

    public override void _Ready()
    {
        // Trouve le joueur dans la scène
        _target = GetNodeOrNull<player>("/root/GameSolo/Player"); // Remplace par le chemin correct

        // Initialise le Timer
        _attackCooldownTimer = new Timer();
        AddChild(_attackCooldownTimer);
        _attackCooldownTimer.WaitTime = 1.0; // Délai de 1 seconde entre les attaques
        _attackCooldownTimer.OneShot = true; // Le Timer ne se répète pas
        _attackCooldownTimer.Timeout += OnAttackCooldownTimeout;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_target == null || _isAttacking) return;

        // Récupère la position du joueur en temps réel
        Vector2 targetPosition = _target.BodyPosition;

        // Calcule la distance horizontale et verticale entre l'ennemi et le joueur
        float distanceX = Mathf.Abs(targetPosition.X - GlobalPosition.X);
        float distanceY = Mathf.Abs(targetPosition.Y - GlobalPosition.Y);

        // Si le joueur est à plus de 400 pixels dans une direction, patrouiller
        if (distanceX > _detectionRange || distanceY > _detectionRange)
        {
            Patrol(delta);
            return;
        }

        // Calcule la direction vers le joueur
        Vector2 direction = (targetPosition - GlobalPosition).Normalized();
        float distanceToTarget = GlobalPosition.DistanceTo(targetPosition);

        // Se déplacer vers le joueur, mais s'arrêter à une certaine distance
        if (distanceToTarget > _minDistance)
        {
            _movementInput = direction;
        }
        else
        {
            _movementInput = Vector2.Zero; // Arrêter de bouger si trop proche
        }

        // Appliquer le mouvement
        Velocity = _movementInput * _speed;
        MoveAndSlide();

        // Gérer les animations de déplacement
        if (direction.X > 0)
        {
            _lastDirection = "right";
            _sprite.Play("walk_right");
        }
        else if (direction.X < 0)
        {
            _lastDirection = "left";
            _sprite.Play("walk_left");
        }
        else
        {
            // Si l'ennemi ne bouge pas, utiliser la première image de l'animation
            _sprite.Stop();
            _sprite.Frame = 0; // Afficher la première image de l'animation
        }

        // Si le joueur est à portée d'attaque, attaquer
        if (distanceToTarget <= _attackRange && !_isAttacking)
        {
            Attack();
        }
    }

    private void Patrol(double delta)
    {
        // Appliquer le mouvement de patrouille
        Velocity = _patrolDirection * _speed;
        MoveAndSlide();

        // Mettre à jour la distance parcourue en patrouille
        _currentPatrolDistance += _speed * (float)delta;

        // Gérer les animations de patrouille
        if (_patrolDirection.X > 0)
        {
            _sprite.Play("walk_right");
        }
        else
        {
            _sprite.Play("walk_left");
        }

        // Si la distance de patrouille est atteinte, inverser la direction et réinitialiser
        if (_currentPatrolDistance >= _patrolDistance)
        {
            _currentPatrolDistance = 0; // Réinitialiser la distance parcourue
            _patrolDirection = -_patrolDirection; // Inverser la direction de patrouille
        }
    }

    private void Attack()
    {
        _isAttacking = true; // Définir l'état d'attaque

        // Jouer l'animation d'attaque en fonction de la dernière direction
        if (_lastDirection == "right")
        {
            _sprite.Play("attack_right");
        }
        else
        {
            _sprite.Play("attack_left");
        }

        // Démarrer le Timer pour le délai entre les attaques
        _attackCooldownTimer.Start();
    }

    private void OnAttackCooldownTimeout()
    {
        _isAttacking = false; // Réinitialiser l'état d'attaque
    }
}