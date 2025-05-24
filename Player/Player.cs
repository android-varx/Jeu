using Godot;
using System;
using EternalForest.Game;

public partial class Player : CharacterBody2D
{
	[Export] private int _speed = 300;
	[Export] private AnimatedSprite2D _sprite;
	[Export] private Camera2D _camera; // Référence à la caméra
	
	
	[Export] private string _leftAxis = "left";
	[Export] private string _rightAxis = "right";
	[Export] private string _upAxis = "up";
	[Export] private string _downAxis = "down";

	private Vector2 _movementInput = Vector2.Zero;
	private string _lastDirection = "R";
	
	private int _damage = 20; // degat du joueur
	public int _life = 20; // vie du joueur
	
	private bool _isDead = false; // état du joueur
	
	private Timer _timer;
	
	private bool _isAttacking = false; // état d'attaque du joueur

	public override void _EnterTree()
	{
		if (GetNode<GameManager>("/root/GameManager").IsNewGame)
		{
			GlobalPosition = new Vector2(1500, 1500); // Position initiale du joueur
		}
		else
		{
			int id = int.Parse(GetName());
			SetMultiplayerAuthority(id);
			GlobalPosition = new Vector2(1500, 1500); // Position initiale du joueur
		}
		
	}
	
	public override void _Ready()
	{
		// Si ce n'est pas le joueur local, on désactive le traitement des entrées
		if (!IsMultiplayerAuthority())
		{
			SetProcess(false);
			SetPhysicsProcess(false);
			if (_camera != null)
			{
				_camera.Enabled = false; // Désactive la caméra pour les autres joueurs
			}
			return;
		}
		if (IsMultiplayerAuthority())
		{
			_camera.Enabled = true;
			_camera.MakeCurrent(); // Fait de cette caméra la caméra active
		}
		
		// Ajout du joueur au groupe "Players"
		AddToGroup("Players");
		
	}

	public override void _Process(double delta)
	{
		if (!IsMultiplayerAuthority() || _isDead) return;
		
		_movementInput = new Vector2(
			Input.GetAxis(_leftAxis, _rightAxis),
			Input.GetAxis(_upAxis, _downAxis)
		).Normalized();

		Velocity = _movementInput * _speed;
		MoveAndSlide();

		if (Input.IsActionPressed("attack"))
		{
			PerformAttack();
		}
		else
		{
			UpdateAnimation();
		}
		
		CheckLife();
	}

	private void PerformAttack()
	{
		
		bool isSolo = GetNode<GameManager>("/root/GameManager").IsNewGame;

		string animation = _lastDirection == "R" ? 
			(isSolo ? "attack_right" : "attack2_right") : 
			(isSolo ? "attack_left" : "attack2_left");

		PlayAnimationSync(animation);

		if (isSolo)
		{
			AttackNearbyOrcs();
		}
		else
		{
			AttackNearbyPlayers();
		}
	}
	
	private void AttackNearbyPlayers()
	{

		int damage = _damage * 2;
		
		float attackRange = 45f;
		Vector2 attackDirection = _lastDirection == "R" ? Vector2.Right : Vector2.Left;
		Vector2 attackOrigin = GlobalPosition + attackDirection * 20f;

		foreach (Node node in GetTree().GetNodesInGroup("Players"))
		{
			if (node is Player otherPlayer && otherPlayer != this)
			{
				float distance = otherPlayer.GlobalPosition.DistanceTo(attackOrigin);
				if (distance <= attackRange)
				{
					// Au lieu d'appeler directement TakeDamage, on utilise un RPC ciblé
					int targetPeerId = otherPlayer.GetMultiplayerAuthority();
					if (targetPeerId != 0)
					{
						if (!_isAttacking)
						{
							RpcId(targetPeerId, nameof(TakeDamage), damage);
						}
					}
					if (!_isAttacking)
					{
						_timer = new Timer();
						AddChild(_timer);
						_timer.WaitTime = 0.8f;
						_timer.OneShot = true;
						_isAttacking = true;
						_timer.Timeout += OnAttackCooldownTimeout;
						_timer.Start();
					}
					
				}
			}
		}
		

	}

	[Rpc(MultiplayerApi.RpcMode.Authority)]
	public void TakeDamage(int damage)
	{
		_life -= damage;
		CheckLife();
	}
	
	private void AttackNearbyOrcs()
	{
		
		float attackRange = 45f; // Distance maximale pour toucher un orc
		Vector2 attackDirection = _lastDirection == "R" ? Vector2.Right : Vector2.Left;
		Vector2 attackOrigin = GlobalPosition + attackDirection * 20f;

		foreach (Node node in GetTree().GetNodesInGroup("Orcs"))
		{
			if (node is Orc orc)
			{
				float distance = orc.GlobalPosition.DistanceTo(attackOrigin);
				if (distance <= attackRange)
				{
					string dir = _lastDirection == "R" ? "right" : "left";
					if (!_isAttacking)
					{
						orc.TakeDamage(_damage, dir);
					}

					if (!_isAttacking)
					{
						// timer pour eviter de spam l'attaqua avant l'annimation
						_timer = new Timer();
						AddChild(_timer);
						_timer.WaitTime = 0.8; // Délai de 1 seconde entre les attaques
						_timer.OneShot = true;
						_isAttacking = true;
						_timer.Timeout += OnAttackCooldownTimeout;
						_timer.Start();
						_isAttacking = true;
					}
					
				}
			}
			else if (node is God god)
			{
				float distance = god.GlobalPosition.DistanceTo(attackOrigin);
				if (distance <= attackRange)
				{
					string dir = _lastDirection == "R" ? "right" : "left";
					if (!_isAttacking)
					{
						god.TakeDamage(_damage, dir);
					}

					if (!_isAttacking)
					{
						// timer pour eviter de spam l'attaqua avant l'annimation
						_timer = new Timer();
						AddChild(_timer);
						_timer.WaitTime = 0.8; // Délai de 1 seconde entre les attaques
						_timer.OneShot = true;
						_isAttacking = true;
						_timer.Timeout += OnAttackCooldownTimeout;
						_timer.Start();
						_isAttacking = true;
					}
					
				}
				
			}
		}
	}

	private void UpdateAnimation()
	{
		if (_movementInput.X > 0)
		{
			_lastDirection = "R";
			PlayAnimationSync("walk_right");
		}
		else if (_movementInput.X < 0)
		{
			_lastDirection = "L";
			PlayAnimationSync("walk_left");
		}
		else if (_movementInput.Y != 0)
		{
			PlayAnimationSync(_lastDirection == "R" ? "walk_right" : "walk_left");
		}
		else
		{
			PlayAnimationSync("");
		}
	}
	
	private void CheckLife()
	{
		if (_life <= 0 && !_isDead)
		{
			_isDead = true;
			Die();
		}
	}

	private void Die()
	{
		SetProcess(false);
		SetPhysicsProcess(false);
		
		string animation = _lastDirection == "R" ? "death_right" : "death_left";
		
		PlayAnimationSync(animation);
		
		Timer deathtimer = new Timer();
		deathtimer.WaitTime = 1f;
		deathtimer.OneShot = true;
		deathtimer.Connect("timeout", Callable.From(() => FinishDeath()));
		AddChild(deathtimer);
		deathtimer.Start();
	}
	
	private void OnAttackCooldownTimeout()
	{
		// Réinitialiser l'état d'attaque
		_isAttacking = false;
	}
	
	private void FinishDeath()
	{
		QueueFree();
		GetTree().ChangeSceneToFile("res://UI/GameOver.tscn");
	}
	
	public void PlayAnimationSync(string animation)
	{
		if (animation == "")
		{
			_sprite.Stop();
		}
		else
		{
			_sprite.Play(animation);
		}
		
		if (IsMultiplayerAuthority())
		{
			Rpc("RemotePlayAnimation", animation);
		}
	}


	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	public void RemotePlayAnimation(string animation)
	{
		if (IsMultiplayerAuthority()) return;
		if (animation == "")
		{
			_sprite.Stop();
		}
		else
		{
			_sprite.Play(animation);
		}
	}
}
