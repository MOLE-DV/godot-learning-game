using Godot;
using System;

public partial class ShowGrab : Node3D
{
	[Export]
	public Sprite3D Sprite;
	public void _on_area_3d_body_entered(Node3D body)
	{
		Sprite.Visible = true;
	}
	public void _on_area_3d_body_exited(Node3D body)
	{
		Sprite.Visible = false;
	}
}
