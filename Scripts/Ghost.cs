using Godot;
using System;

public class Ghost : KinematicBody2D
{
	public enum States {
		Wander,
		Seek
	}
	
	public States state = States.Wander;
	public bool pertoDoChao = false;
	
	private Vector2 velocity;
	private Vector2 direction;
	private RayCast2D ray;
	private RayCast2D olho;
	private float downSpeed = 30;
	
	Vector2 pointA = new Vector2(250, 0);
	Vector2 pointB = new Vector2(1300, 0);
	Vector2 targetPoint, originPoint;

	private float speed = 40;
	private float seekSpeed = 35;
	private float wanderSpeed = 70;
	KinematicBody2D player;
	SeekInfo infoSeekPlayer;
	Random random;
	static bool playerWon = false;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		random = new Random();
		olho = GetNode<RayCast2D>("Olho");
		pointA.y = Position.y;
		pointB.y = Position.y;
		originPoint = pointA;
		targetPoint = pointB;
		//player = GetTree().GetNode<KinematicBody2D>("../Player");
	}
	
	void ChangeState(States newState) {
		bool newStateIsDifferent = state != newState;
		if(!newStateIsDifferent) {
			return;
		}
		if(newState == States.Wander) {
			speed = wanderSpeed;
		}
		else if(newState == States.Seek) {
			speed = seekSpeed;
			GetNode<AudioStreamPlayer>("Roar" + random.Next(1, 4)).Play();
		}
		state = newState;
	}

	public override void _Process(float delta) {

		
		infoSeekPlayer = calcSeek(Player.player.Position);
		
		if(infoSeekPlayer.distance > 500 || PlayerIsSafe()) {
			ChangeState(States.Wander);
			
		}
		else {
			ChangeState(States.Seek);
		}


		if(state == States.Seek) {
			SeekPlayer();
		}
		else if(state == States.Wander) {
			SeekPoint();
		}

		//if(olho.IsColliding()){
		//}

		Move();
	}
	
	void Move() {
		velocity = MoveAndSlide(velocity);
	}

	private void _on_KillerZone_body_entered(object body) {
		Player.player.Die();
	}
	
	void SeekPoint() {
		
		SeekInfo infoSeek = calcSeek(targetPoint);
		if(infoSeek.distance < 10) {
			// swap origin and target
			Vector2 aux = targetPoint;
			targetPoint = originPoint;
			originPoint = aux;
		}
		velocity = infoSeek.velocity;
	}

	bool PlayerIsSafe() {
		if(playerWon) return true;
		if(Player.player.Position.y < -1130 && Player.player.Position.x < 1180) {
			playerWon = true;
			return true;
		}
		return false;
	}

	void SeekPlayer() {
		velocity += infoSeekPlayer.velocity;
	}


	// useful methods

	Vector2 diffVector(Vector2 a, Vector2 b) {
		return new Vector2(a.x - b.x, a.y - b.y);
	}
	
	float calcDist(Vector2 a, Vector2 b) {
		float x = b.x-a.x;
		float y = b.y-a.y;
		float distance = (float) Math.Sqrt(Math.Pow(x, 2f) + Math.Pow(y, 2f));
		return distance;
	}
	
	SeekInfo calcSeek(Vector2 target) {
		Vector2 diff = diffVector(target, Position);
		float distance = calcDist(target, Position);
		Vector2 direction = new Vector2(diff.x / distance, diff.y / distance);
		// velocity = direction * speed;
		return new SeekInfo(direction * speed, distance);
	}

	class SeekInfo {
		public Vector2 velocity;
		public float distance;

		public SeekInfo(Vector2 velocity, float distance) {
			this.velocity = velocity;
			this.distance = distance;
		}
	}

}


