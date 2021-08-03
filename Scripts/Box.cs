using Godot;
using System;

public class Box : KinematicBody2D
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	// Called when the node enters the scene tree for the first time.
	private float downSpeed = 60;
	private float gravity = 2600;
	private Vector2 velocity;

	public override void _Ready()
	{
		
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		// velocity.y += gravity * delta;
		// Move();
	}

	void Move() {
		velocity = MoveAndSlide(velocity);
	}
}
