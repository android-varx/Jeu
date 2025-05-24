using EternalForest.Game;
using Godot;

public partial class Orc : CharacterBody2D
{
    [Export] private int _speed = 100; // Vitesse de l'ennemi
    [Export] private AnimatedSprite2D _sprite; // recuperation de la sprite annimée
    [Export] private int _attackRange = 41; // distance d'attaque
    [Export] private int _minDistance = 40; // distance de rapprochement de l'orc 
    [Export] private int _detectionRange = 600; // distance de détection
    [Export] private int _patrolDistance = 100; // distance de patrouille
    [Export] private int _damage = 1; // degat de l'orc
    [Export] private int _life = 4; // vie de l'orc

    private Vector2 _movementInput = Vector2.Zero;
    private Player principal_target; // le joueur que l'orc va attaquer
    private string _lastDirection = "right"; // varibale pour stocker la dérniere direction ou l'ennemi regardait 
    private Player _target; // le joueur que l'orc va attaquer
    private Player _target2; // le joueur 2 que l'orc va attaquer
    private bool _isAttacking = false; // varible pour indiquer si l'orc attaque ou pas 
    private Timer _attackCooldownTimer; // temps pour gerer le temps d'attaque
    private Timer _deathCooldownTimer; // temps pour gerer le temps d'attaque
    private float _currentPatrolDistance = 0.0f; // distance de patrouille
    private bool _isPatrolling = false; // variable pour indiquer si l'ennemi est entrain de patrouiller
    private Vector2 _patrolDirection = Vector2.Right;
    private bool _isDead = false; // variable pour indiquer si l'ennemi est mort
    private bool _waitingToHit = false;


    private int notvisible = 1500;

    public override void _Ready()
    {
        AddToGroup("Orcs");
        
        _target = GetNodeOrNull<Player>("/root/GameSolo/Node2D/Player");// permet de trouver le Player dans la scène
        if (!GetNode<GameManager>("/root/GameManager").IsNewGame)
        {
            _target2 = GetNodeOrNull<Player>("/root/GameSolo/Node2D/Player2");// permet de trouver le Player2 dans la scène
        }
        
        //Timer
        _attackCooldownTimer = new Timer();
        AddChild(_attackCooldownTimer);
        _attackCooldownTimer.WaitTime = 0.6; // Délai de 1 seconde entre les attaques
        _attackCooldownTimer.OneShot = true; // Le Timer ne se répète pas
        _attackCooldownTimer.Timeout += OnAttackCooldownTimeout;
        _sprite.AnimationFinished += OnAnimationFinished;
        _deathCooldownTimer = new Timer();
        AddChild(_deathCooldownTimer);
        _deathCooldownTimer.WaitTime = 1; // Délai de 1 seconde entre les attaques
        _deathCooldownTimer.OneShot = true; // Le Timer ne se répète pas
        _deathCooldownTimer.Timeout += OnAttackCooldownTimeout;


    }

    public override void _PhysicsProcess(double delta)
    {
        if (_target == null || _isAttacking)
        {
            return;
        }
        
        principal_target = _target;
        
        Vector2 targetPosition = _target.GlobalPosition;// recuperation de la position du joueur a chque frame
        // calcul pour la distance verticale et horizontale entre le joueur et l'ennemi
        float distanceX = Mathf.Abs(targetPosition.X - GlobalPosition.X);
        float distanceY = Mathf.Abs(targetPosition.Y - GlobalPosition.Y);
        
        if (!GetNode<GameManager>("/root/GameManager").IsNewGame)
        {
            Vector2 targetPosition2 = _target2.GlobalPosition;// recuperation de la position du joueur a chque frame
            float distanceX2 = Mathf.Abs(targetPosition2.X - GlobalPosition.X);
            float distanceY2 = Mathf.Abs(targetPosition2.Y - GlobalPosition.Y);
            
            // si le joueur 2 est plus proche que le joueur 1 alors on attaque le joueur 2
            if (distanceX2 < distanceX && distanceY2 < distanceY)
            {
                targetPosition = targetPosition2;
                principal_target = _target2;
                distanceX = distanceX2;
                distanceY = distanceY2;
            }
            else
            {
                principal_target = _target;
            }
        }
        
        // si le joueur n'est pas dans le zone alors on lance la fonction pour patrouiller
        if (distanceX > notvisible || distanceY > notvisible)
        {
            return;
        }
        if (distanceX > _detectionRange || distanceY > _detectionRange)
        {
            Patrol(delta);
            return;
        }

        // calcul de la direction vers le joueur
        Vector2 direction = (targetPosition - GlobalPosition).Normalized();
        float distanceToTarget = GlobalPosition.DistanceTo(targetPosition);

        // se deplacer vers le joueur mais si trop proche alors on s'arrete
        if (distanceToTarget > _minDistance)
        {
            _movementInput = direction;
        }
        else
        {
            _movementInput = Vector2.Zero; // on s'arrete si trop proche
        }

        // mouvement
        Velocity = _movementInput * _speed;
        MoveAndSlide();

        //cette partie gére les animations de mouvement
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
            // si il ne bouge pas il reste sur la premiere frame 
            _sprite.Stop();
            _sprite.Frame = 0; // premiere frame
        }

        // si le joueur entre dans la zone d'attaque alors l'orc l'attaque
        if (distanceToTarget <= _attackRange && !_isAttacking)
        {
            Attack(principal_target);
        }
    }
    
    public void TakeDamage(int damage, string fromDirection)
    {
        if (_isDead) return;

        _life -= damage;

        string anim = fromDirection == "right" ? "damaged_left" : "damaged_right";
        GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play(anim);
        _deathCooldownTimer.Start();

        if (_life <= 0)
        {
            Death();
        }
    }


    private void Death()
    {
        _isDead = true;
    
        if (_lastDirection == "right")
            _sprite.Play("death_right");
        else
            _sprite.Play("death_left");
        
        _attackCooldownTimer.Start();

        // L'Orc sera supprimé quand l'animation de mort est terminée
    }
    
    private void OnAnimationFinished()
    {
        if (_waitingToHit)
        {
            _waitingToHit = false;

            if (principal_target != null &&
                principal_target.GlobalPosition.DistanceTo(GlobalPosition) <= _attackRange)
            {
                principal_target.TakeDamage(_damage);
            }

            _isAttacking = false;
        }
    }

    

    private void Patrol(double delta) // cette fonction est la fonction de patrouille de l'orc
    {
        Velocity = _patrolDirection * _speed;
        MoveAndSlide();

        // met a jour la distance parcourue en patrouille
        _currentPatrolDistance += _speed * (float)delta;

        // cette partie permet de gerer les annimations lors de la patrouille de l'orc
        if (_patrolDirection.X > 0)
        {
            _sprite.Play("walk_right");
        }
        else
        {
            _sprite.Play("walk_left");
        }

        // si on atteint la distance de patrouille maximale alors on inverse le sens de la patrouille
        if (_currentPatrolDistance >= _patrolDistance)
        {
            _currentPatrolDistance = 0; // on reinitialise la varibale
            _patrolDirection = -_patrolDirection; // on inverse la direction
        }
    }

    private void Attack(Player player)
    {
        _isAttacking = true; // varibale a true pour dire que il attaque 
        _waitingToHit = true;

        // animations d'attaque en fonction du dernier endroit ou il regardait 
        if (_lastDirection == "right")
        {
            _sprite.Play("attack_right");
        }
        else
        {
            _sprite.Play("attack_left");
        }
        
        // lance le timer pour attaquer
        _attackCooldownTimer.Start();
        
        // inflige des degats au joueur si il est dans la range d'attaque
        if (principal_target.GlobalPosition.DistanceTo(GlobalPosition) <= _attackRange)
        {
            principal_target.TakeDamage(_damage); // inflige des degats au joueur
        }
    }

    private void OnAttackCooldownTimeout()
    {
        if (_isDead)
        {
            QueueFree();
        }
        _isAttacking = false; // on remet l'état d'attauqe a false pour dire que on attaque plus 
    }
}