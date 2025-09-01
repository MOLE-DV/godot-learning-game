using Godot;
using System;
using System.Threading.Tasks;

public partial class Camera : Camera3D
{
	[Export]
	public CharacterBody3D Player;
	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	private float pitch = 0f;

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion mm)
		{
			Player.RotateY(-mm.Relative.X * 0.002f);

			pitch -= mm.Relative.Y * 0.002f;
			pitch = Mathf.Clamp(pitch, -Mathf.Pi / 2f, Mathf.Pi / 2f);

			Rotation = new Vector3(pitch, Rotation.Y, Rotation.Z);
		}
	}
}
