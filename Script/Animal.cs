using Godot;
using System;
using System.Linq;

public partial class Animal : CharacterBody2D, IBio
{
	[Signal]
	public delegate void BreedEventHandler(int count);
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
	[Export]
	public bool BeingControlled { get; set; } = false;
	[Export]
	public bool LabelVisible { get; set; } = false;
	public Node2D TargetNode { get; set; } = null;
	public Label InfoLabel { get; set; }
	public AnimatedSprite2D anim;

	public override void _Ready()
	{
		AddToGroup($"{Diet}Animals");
		AddToGroup("Animals");
		GetNode<CollisionShape2D>("Mouth/ReachArea").Shape.Set("Radius", EatRange);
		Hunger = MaxHunger;
		Health = MaxHealth;
		anim = GetNode<AnimatedSprite2D>("Anim");
		anim.Play("Idle");
		InfoLabel = GetNode<Label>("InfoLabel");
		InfoLabel.Visible = LabelVisible;
	}
	public override void _PhysicsProcess(double delta)
	{
		Hunger -= (float)delta * 0.3f;
		InfoLabel.Text = $"\nHealth: {Health:F1}\nHunger: {Hunger:F1}\nState: {State}";

		if (Health <= 0)
		{
			QueueFree();
			return;
		}
		else if (Health >= MaxHealth * 2f)
		{
			GD.Print("Health is too high");
			Health = MaxHealth;
			EmitSignal(nameof(Breed), 1);
		}

		if (Hunger <= 0)
		{
			Health -= (float)delta * 0.2f;
			Hunger = 0;
		}
		else if (Hunger > MaxHunger * 0.5f && Hunger < MaxHunger)
		{
			Health += (float)delta * 0.3f;
		}
		else if (Hunger >= MaxHunger)
		{
			Health += Hunger - MaxHunger;
			Hunger = MaxHunger;
		}


		if (Hunger <= MaxHunger * 0.5f)
		{
			if (State != AnimalStateEnum.Hunting)
			{
				State = AnimalStateEnum.Hunting;

				switch (Diet)
				{
					case AnimalTypeEnum.Herbivore:
						var nodes1 = GetTree().GetNodesInGroup("Plants");
						if (nodes1.Count == 0)
						{
							State = AnimalStateEnum.Fine;
							return;
						}
						var plants = nodes1.Cast<Plant>();
						var nearestPlant = plants.OrderBy(p => p.GlobalPosition.DistanceTo(GlobalPosition)).First();
						TargetNode = nearestPlant;
						TargetNode.TreeExiting += () =>
						{
							TargetNode = null;
							State = AnimalStateEnum.Fine;
							Velocity = Vector2.Zero;
						};
						GD.Print($"Hunting {nearestPlant.Name} at {nearestPlant.GlobalPosition}");
						break;
					case AnimalTypeEnum.Carnivore:
						var nodes2 = GetTree().GetNodesInGroup("HerbivoreAnimals");
						if (nodes2.Count == 0)
						{
							State = AnimalStateEnum.Fine;
							return;
						}
						var animals = nodes2.Cast<Animal>();
						var nearestAnimal = animals.OrderBy(p => p.GlobalPosition.DistanceTo(GlobalPosition)).First();
						TargetNode = nearestAnimal;
						break;
					case AnimalTypeEnum.Omnivore:

						break;
					default:
						break;
				}
			}
		}
		else if (Health <= MaxHealth * 0.5f)
		{
			if (State != AnimalStateEnum.Sleeping)
			{
				State = AnimalStateEnum.Sleeping;
			}
		}
		else if ((Hunger >= MaxHunger * 0.7f && State == AnimalStateEnum.Hunting) || (Health >= MaxHealth * 0.7f && State == AnimalStateEnum.Sleeping))
		{
			State = AnimalStateEnum.Fine;
		}
		switch (State)
		{
			case AnimalStateEnum.Hunting:
				if (TargetNode != null)
				{
					var pos = TargetNode.GlobalPosition - GlobalPosition;
					if (pos.Length() > 10f)
					{
						Velocity = pos.Normalized() * 200f;
					}
				}
				break;
			case AnimalStateEnum.Sleeping:
				Velocity = Vector2.Zero;
				Health += (float)delta * 0.1f;
				break;
			case AnimalStateEnum.Fine:
				break;
			default:
				break;
		}

		if (BeingControlled)
		{
			Velocity = Input.GetVector("Left", "Right", "Up", "Down") * 300f;
		}
		MoveAndSlide();

		if (Velocity.Length() > 0)
		{
			anim.Play("Walk");
			if (Velocity.X > 0)
				anim.FlipH = false;
			else if (Velocity.X < 0)
				anim.FlipH = true;
		}
		else
		{
			anim.Play("Idle");
		}
	}
	public void OnBodyEntered(Node2D body)
	{
		GD.Print($"Entered {body.Name} {body.GetType()}");
		switch (Diet)
		{
			case AnimalTypeEnum.Herbivore:
				if (body is Plant plant)
				{
					GD.Print($"{Name} ate {plant.Name} plant");
					Eat(plant);
				}
				break;
			case AnimalTypeEnum.Carnivore:
				if (body is Animal animal && animal != this)
				{
					GD.Print($"{Name} was eaten by {animal.Name} animal");
					Eat(animal);
				}
				break;
			case AnimalTypeEnum.Omnivore:

				if (body is IBio bio && bio != this)
				{
					Eat(bio);
				}
				break;
			default:
				GD.Print("Diet not supported");
				break;
		}
	}
	public void Eat(IBio node)
	{
		Hunger += node.BeEaten();
		TargetNode = null;
		Velocity = Vector2.Zero;
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
