using Godot;
using System;

public partial class Plant : Area2D,IBio
{
	public AnimatedSprite2D anim;
	public Timer timer;
	[Signal]
	

	public delegate void SpawnEventHandler(int count);
	[Export]
	public float MaxHealth { get; set; } = 100.0f;
	[Export]
	public float MaxHunger { get; set; } = 100.0f;
	public float Health { get; set; } = 100.0f;
	public float Hunger { get; set; } = 100.0f;
	public float EatEfficiency { get; set; } = 0.8f;
	// [Export]
	public int MaxStage { get; set; } = 5;
	private int _stage = 1;
	public int Stage
	{
		get => _stage;
		set
		{
			_stage = value;
			anim.Frame = _stage;
			Health = MaxHealth * (_stage / MaxStage);
			Hunger = MaxHunger * (_stage / MaxStage);
		}
	}

	public override void _Ready()
	{
		AddToGroup("Plants");
		anim = GetNode<AnimatedSprite2D>("Sprite");
		timer = GetNode<Timer>("Timer");
		MaxStage = (int)GD.Randi() % 5 + 5;
		timer.WaitTime = (float)GD.RandRange(3, 20);
		timer.Start();
	}
	public void Grow()
	{
		Stage++;
		if (Stage >= MaxStage)
		{
			EmitSignal(nameof(Spawn), 1);
			Stage = 1;
		}
	}
	public override void _PhysicsProcess(double delta)
	{
	}
}
