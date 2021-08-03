using Godot;
using System;


public class Player : KinematicBody2D
{

	public enum estados{
		Forte,
		Veloz,
		Inteligente
	}

	string estadoStr() {
		if(estado == estados.Forte) {
			return "Forte";

		} else if(estado == estados.Inteligente) {
			return "Intelig";
		}
		else if(estado == estados.Veloz) {
			return "Veloz";
		}
		else {
			return "InvalidState";
		}
	}

	public static Player player;

	public static estados estado;

	private float speed = 16000;
	private float spdVeloz = 30000; // speed
	private float spdComCaixa = 10000; // 
	private float jumpForce = 50000;
	private float downForce = 20000;
	private float gravity = 2600;
	private Vector2 velocity, spawnPoint;
	private RayCast2D ray;
	private AnimatedSprite animator = null;

	public bool movendoCaixa = false, walking = false;

	private float realSpeed;

	private KinematicBody2D caixa = null;
	private Sprite sprite;
	private float hspd = 0;

	private void ChangeType(estados type) {
		if(animator != null) {
			animator.Visible = false;
		}
		estado = type;
		animator = GetNode<AnimatedSprite>("Animated" + estadoStr());
		
		animator.Visible = true;
	}

	public void Die() {
		Position = spawnPoint;
		if(estado == estados.Forte) {
			ChangeType(estados.Inteligente);
		} else if(estado == estados.Inteligente) {
			ChangeType(estados.Veloz);
		}
		else if(estado == estados.Veloz) {
			ChangeType(estados.Forte);
		}
		GetNode<AudioStreamPlayer>("SoundDeath").Play();
	}

	public override void _Ready() {
		player = this;
		
		ray = GetNode<RayCast2D>("Point/Raio");
		//Position = GetTree().GetNode("Spawn").Position;
		ChangeType(estados.Forte);
		spawnPoint = Position;
 

		//GetTree().GetNode("Ghost").Connect("TouchedGhost", this, nameof(OnTouchedGhost));
		//GD.Print(Position.x);
		//sprite = GetNode<Sprite>("Sprite");
		//GD.Print(ray.Name);
	}

	void OnTouchedGhost() {

	}

	public override void _Process(float delta) {	
		if(!movendoCaixa){
			realSpeed = speed;
		}else{
			realSpeed = spdComCaixa;
		}

		if (estado == estados.Veloz) {
			realSpeed = spdVeloz;
		}


		GetInputs(delta);
		ControlAnimations();
		if(hspd != 0 && !movendoCaixa)
			ray.CastTo = new Vector2(hspd * 40f, 0);
		Move();
		PodeMoverCaixa(delta);
		Flip();
		
	}

	void Move() {
		velocity = MoveAndSlide(velocity, Vector2.Up);
	}

	void PodeMoverCaixa(float delta) {
		if(estado == estados.Forte) {
			if(Input.IsActionPressed("Pegar Caixa")){
				if(!ray.IsColliding()) {
					// GD.Print("Nao pegando caixa");
					return;
				}
				// GD.Print("pegando caixa");
				movendoCaixa = true;
				caixa = (KinematicBody2D)ray.GetCollider();
				if(caixa == null) {

				}
				if(Position.x < caixa.Position.x) {
					animator.FlipH = false;
				} else {
					animator.FlipH = true;
				}
				caixa.MoveAndSlide(new Vector2(hspd * spdComCaixa * delta, 0));
			}else{
				caixa = null;
				movendoCaixa = false;
			}
		}
	}

	bool isWalking() {
		return walking;
	}

	void ControlAnimations()
	{
		bool nowWalking = false;
		if(movendoCaixa) {
			animator.Play("push");
		}
		else if(hspd != 0){
			animator.Play("walk");
			nowWalking = true;
		}
		else if(!IsOnFloor()) {
			animator.Play("jump");
		}
		else {
			animator.Play("idle");
		}

		AudioStreamPlayer walkSound = GetNode<AudioStreamPlayer>("SoundSteps");

		if(!walking && nowWalking) {
			walkSound.Play();
		}
		if(walking && !nowWalking) {
			walkSound.Stop();
		}

		walking = nowWalking;
	}
	void Flip()
	{
		if(movendoCaixa) {
			return;
		}
		if(hspd < 0){
			animator.FlipH = true;
		}else if(hspd > 0){
			animator.FlipH = false;
		}
	}

	void GetInputs(float delta) {
		hspd = Convert.ToInt16(Input.IsActionPressed("ui_right")) - Convert.ToInt16(Input.IsActionPressed("ui_left"));
		velocity.x = hspd * realSpeed * delta;
		velocity.y += gravity * delta;
		
		if(Input.IsActionJustPressed("Jump") && IsOnFloor()) {
			velocity.y -= jumpForce * delta;
			GetNode<AudioStreamPlayer>("SoundJump" + estadoStr()).Play();
		}
		if(Input.IsActionJustPressed("ui_down") && !IsOnFloor()) {
			velocity.y += downForce * delta;
		}
	}

	void MoverCaixas() {
		caixa.Position += new Vector2(hspd, 0) * spdComCaixa;
	}

	
	
	// public void _on_fallzone_body_entered(object body)
	// {
	// 	GetTree().ChangeScene("res://Main.tscn");
	// }
	
}



