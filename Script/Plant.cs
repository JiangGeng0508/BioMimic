using Godot;
using System;

public partial class Plant : Area2D,IBio
{
	public Sprite2D icon;
	[Export]
	public float MaxHealth { get; set; } = 100.0f;
	[Export]
	public float MaxHunger { get; set; } = 100.0f;
	public float Health { get; set; } = 100.0f;
	public float Hunger { get; set; } = 100.0f;
	public float EatEfficiency { get; set; } = 0.8f;

	public override void _Ready()
	{
		AddToGroup("Plants");
		icon = GetNode<Sprite2D>("Icon");
	}
	public override void _PhysicsProcess(double delta)
	{
		if (icon.Scale.Y < 2.0f)
		icon.Scale += Vector2.One * 0.1f* (float)delta;
	}
}
