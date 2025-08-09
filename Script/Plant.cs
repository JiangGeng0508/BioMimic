using Godot;
using System;

public partial class Plant : Area2D,IBio
{
	public AnimatedSprite2D anim;
	public Timer timer;
	[Signal]
	public delegate void BreedEventHandler(int count);
	
	[Export]
	public float MaxHealth { get; set; } = 100.0f;
	[Export]
	public float MaxHunger { get; set; } = 100.0f;
	private float _health;
	public float Health { get => _health; set => _health = value; }
	private float _hunger;
	public float Hunger { get => _hunger; set => _hunger = value; }
	[Export]
	public bool LabelVisible { get; set; } = false;
	public Label InfoLabel;
	public float EatEfficiency { get; set; } = 0.8f;
	public int MaxStage { get; set; } = 5;
	private int _stage = 1;
	public int Stage
	{
		get => _stage;
		set
		{
			_stage = value;
			anim.Frame = _stage;
			Health = MaxHealth * _stage;
			Hunger = MaxHunger * _stage;
		}
	}

	public override void _Ready()
	{
		AddToGroup("Plants");
		anim = GetNode<AnimatedSprite2D>("Sprite");
		timer = GetNode<Timer>("Timer");
		InfoLabel = GetNode<Label>("InfoLabel");
		InfoLabel.Visible = LabelVisible;
		MaxStage = (int)GD.Randi() % 5 + 5;
		timer.WaitTime = GD.RandRange(3, 20);
		timer.Start();
		Health = MaxHealth;
		Hunger = MaxHunger;
	}
	public void Grow()
	{
		Stage++;
		if (Stage >= MaxStage)
		{
			EmitSignal(nameof(Breed), 1);
			Stage = 1;
		}
	}
	public override void _PhysicsProcess(double delta)
	{
		InfoLabel.Text = $"Health: {Health:0.00}\nHunger: {Hunger:0.00}\nStage: {Stage}/{MaxStage}\nTimeLeft: {timer.TimeLeft:0.00}s";
	}
}
