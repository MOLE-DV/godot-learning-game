
using System;
using Godot;

public partial class Player : CharacterBody3D
{
	// How fast the player moves in meters per second.
	[Export]
	public int Speed { get; set; } = 10;
	[Export]
	public int SprintSpeed { get; set; } = 20;
	[Export]
	public float Damping { get; set; } = 10f;
	[Export]
	public int FallAcceleration { get; set; } = 75;
	[Export]
	public int JumpVelocity { get; set; } = 20;
	[Export]
	public float HeadBob_Frequency { get; set; } = 5;
	[Export]
	public float HeadBob_Sprint_Frequency { get; set; } = 10;
	[Export]
	public float HeadBob_Amplitude { get; set; } = .25f;
	[Export]
	public Camera3D Camera;


	private float headbob_time = 0f;
	private float _current_speed;
	private float _current_headbob_frequency;
	private bool sprinting = false;
	private Vector3 _targetVelocity = Vector3.Zero;
	private Node3D Head;
	public override void _Ready()
	{
		Head = GetNode<Node3D>("Head");
		_current_headbob_frequency = HeadBob_Frequency;
	}

	private void _OnBodyEntered(Node3D body)
	{
		GD.Print(body);
	}
	public override void _PhysicsProcess(double delta)
	{
		Vector3 direction;
		Vector2 inputDirection = Input.GetVector("move_left", "move_right", "move_forward", "move_back");
		sprinting = Input.IsActionPressed("sprint");
		_current_speed = sprinting ? SprintSpeed : Speed;


		direction = (Transform.Basis * new Vector3(inputDirection.X, 0, inputDirection.Y)).Normalized();

		if (!IsOnFloor())
		{
			_targetVelocity.Y -= FallAcceleration * (float)delta;
		}
		else
		{
			_targetVelocity.Y = 0;
		}

		if (Input.IsActionPressed("jump") && IsOnFloor())
		{
			_targetVelocity.Y = JumpVelocity;
		}

		if (Input.IsActionJustPressed("exit"))
		{
			GetTree().Quit();
		}


		if (direction != Vector3.Zero)
		{
			_targetVelocity.X = direction.X * _current_speed;
			_targetVelocity.Z = direction.Z * _current_speed;
			headbob_time += (float)delta;
			Camera.Position = headBob(headbob_time, (float)delta);
		}
		else
		{
			Vector3 horizontal = new Vector3(_targetVelocity.X, 0, _targetVelocity.Z);
			horizontal = horizontal.Lerp(Vector3.Zero, (float)delta * Damping);
			_targetVelocity = new Vector3(horizontal.X, _targetVelocity.Y, horizontal.Z);
		}
		Velocity = _targetVelocity;
		MoveAndSlide();
	}
	

	private Vector3 headBob(float headbob_time, float delta)
	{
		_current_headbob_frequency = sprinting ? HeadBob_Sprint_Frequency : HeadBob_Frequency;
		Vector3 head_position = Vector3.Zero;

		head_position.X = Mathf.Sin(headbob_time * _current_headbob_frequency) * HeadBob_Amplitude;
		head_position.Y = Mathf.Cos(headbob_time * _current_headbob_frequency * 2) * HeadBob_Amplitude / 5;
		return head_position;
	}

}
