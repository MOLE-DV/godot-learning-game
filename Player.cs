
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
	public float HeadBob_Amplitude { get; set; } = .25f;

	private float headbob_time = 0f;
	private bool sprinting = false;
	private Vector3 _targetVelocity = Vector3.Zero;
	private Camera3D Camera;

	public override void _Ready()
	{
		Camera = GetNode<Camera3D>("Camera3D");
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 direction = Vector3.Zero;
		Vector2 inputDirection = Input.GetVector("move_left", "move_right", "move_forward", "move_back");

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

		sprinting = Input.IsActionPressed("sprint");
		float currentSpeed = sprinting ? SprintSpeed : Speed;

		if (direction != Vector3.Zero)
		{
			_targetVelocity.X = direction.X * currentSpeed;
			_targetVelocity.Z = direction.Z * currentSpeed;
		}
		else
		{
			Vector3 horizontal = new Vector3(_targetVelocity.X, 0, _targetVelocity.Z);
			horizontal = horizontal.Lerp(Vector3.Zero, (float)delta * Damping);
			_targetVelocity = new Vector3(horizontal.X, _targetVelocity.Y, horizontal.Z);
		}
		headbob_time += (float)delta;
		Camera.Position = headBob(headbob_time);
		Velocity = _targetVelocity;
		MoveAndSlide();
	}

	private Vector3 headBob(float headbob_time)
	{
		Vector3 head_position = Vector3.Zero;
		head_position.X = Mathf.Sin(headbob_time * HeadBob_Frequency) * HeadBob_Amplitude ;
		head_position.Y = Camera.Position.Y + Mathf.Cos(headbob_time * HeadBob_Frequency / 2) * HeadBob_Amplitude / 20;
		return head_position;
	}
}