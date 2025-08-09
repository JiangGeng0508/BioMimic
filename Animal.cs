using Godot;
using System;

public partial class Animal : CharacterBody2D, IBio
{
	[Export(PropertyHint.Enum, "Herbivore,Carnivore,Omnivore")]
	public AnimalTypeEnum Diet { get; set; } = AnimalTypeEnum.Omnivore;

	public AnimalStateEnum State { get; set; } = AnimalStateEnum.Fine;
	[Export]
	public float Health { get; set; } = 100.0f;
	[Export]
	public float Hunger { get; set; } = 100.0f;
	[Export]
	public float SightRange { get; set; } = 100.0f;

	public Node2D[] FoundedNodes { get; set; }

	public Label InfLabel { get; set; }

	public override void _Ready()
	{
		AddToGroup($"{Diet}Animals");
		GetNode<CollisionShape2D>("Sight/SightShape").Shape.Set("Radius", SightRange);

		InfLabel = new Label();
		AddChild(InfLabel);
	}
	public override void _PhysicsProcess(double delta)
	{
		Health -= (float)delta * 0.1f;
		Hunger -= (float)delta * 0.1f;
		InfLabel.Text = $"\nHealth: {Health:F1}\nHunger: {Hunger:F1}\nState: {State}";

		if (Hunger <= 50f && State!= AnimalStateEnum.Hunting)
		{
			State = AnimalStateEnum.Hunting;
		}
		else if (Health <= 50f && State!= AnimalStateEnum.Sleeping)
		{
			State = AnimalStateEnum.Sleeping;
		}
		else if ((Hunger >= 80f && State == AnimalStateEnum.Hunting) || (Health >= 80f && State == AnimalStateEnum.Sleeping))
		{
			State = AnimalStateEnum.Fine;
		}
	}
	public void OnBodyEntered(Node2D body)
	{
		FoundedNodes[FoundedNodes.Length] = body;
	}
}
public enum AnimalTypeEnum
{
	Herbivore,
	Carnivore,
	Omnivore
}
public enum AnimalStateEnum
{
	Fine,
	Hunting,
	Sleeping
}
