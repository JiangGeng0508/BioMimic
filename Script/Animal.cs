using Godot;
using System;
using System.Linq;

public partial class Animal : CharacterBody2D, IBio
{
	[Export(PropertyHint.Enum, "Herbivore,Carnivore,Omnivore")]
	public AnimalTypeEnum Diet { get; set; } = AnimalTypeEnum.Omnivore;

	public AnimalStateEnum State { get; set; } = AnimalStateEnum.Fine;
	[Export]
	public float MaxHealth { get; set; } = 100.0f;
	[Export]
	public float MaxHunger { get; set; } = 100.0f;
	public float Health { get; set; } = 100.0f;
	public float Hunger { get; set; } = 100.0f;
	[Export]
	public float EatEfficiency { get; set; } = 0.8f;

	[Export]
	public float EatRange { get; set; } = 30.0f;
	public Node2D TargetNode { get; set; } = null;
	public Label InfLabel { get; set; }

	public override void _Ready()
	{
		AddToGroup($"{Diet}Animals");
		GetNode<CollisionShape2D>("Mouth/ReachArea").Shape.Set("Radius", EatRange);
		Hunger = MaxHunger;
		Health = MaxHealth;


		InfLabel = new Label();
		AddChild(InfLabel);
	}
	public override void _PhysicsProcess(double delta)
	{
		Health -= (float)delta * 0.1f;
		Hunger -= (float)delta * 0.1f;
		InfLabel.Text = $"\nHealth: {Health:F1}\nHunger: {Hunger:F1}\nState: {State}";

		if (Hunger <= 50f)
		{
			if (State != AnimalStateEnum.Hunting)
			{
				State = AnimalStateEnum.Hunting;

				switch (Diet)
				{
					case AnimalTypeEnum.Herbivore:
						var nodes = GetTree().GetNodesInGroup("Plants");
						if (nodes.Count == 0)
						{
							State = AnimalStateEnum.Fine;
							return;
						}
						var plants = nodes.Cast<Plant>();
						var nearest = plants.OrderBy(p => p.GlobalPosition.DistanceTo(GlobalPosition)).First();
						TargetNode = nearest;
						GD.Print($"Hunting {nearest.Name} at {nearest.GlobalPosition}");
						break;
					default:
						break;
				}
			}
		}
		else if (Health <= 50f)
		{
			if (State != AnimalStateEnum.Sleeping)
			{
				State = AnimalStateEnum.Sleeping;
			}
		}
		else if ((Hunger >= 50f && State == AnimalStateEnum.Hunting) || (Health >= 50f && State == AnimalStateEnum.Sleeping))
		{
			State = AnimalStateEnum.Fine;
		}
		else
		{
			if (Hunger >= MaxHunger)
			{
				Hunger = MaxHunger;
			}
			if (Health >= MaxHealth)
			{
				Health = MaxHealth;
			}
		}

		Velocity = Input.GetVector("Left", "Right", "Up", "Down") * 300f;
		if (TargetNode != null)
		{
			var pos = TargetNode.GlobalPosition - GlobalPosition;
			if (pos.Length() > 10f)
			{
				Velocity = pos.Normalized() * 200f;
			}
		}
		MoveAndSlide();
	}
	public void OnBodyEntered(Node2D body)
	{
		switch (Diet)
		{
			case AnimalTypeEnum.Herbivore:
				if (body is Plant plant)
				{
					Eat(plant);
				}
				break;
			default:
				break;
		}
	}
	public void Eat(IBio node)
	{
		Hunger += node.BeEaten();
		TargetNode = null;
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
