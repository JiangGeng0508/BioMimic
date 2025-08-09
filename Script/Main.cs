using Godot;
using System;

public partial class Main : Node2D
{
	public PackedScene plantArea = GD.Load<PackedScene>("res://Scene/Plant.tscn");
	public PackedScene mouseArea = GD.Load<PackedScene>("res://Scene/Mouse.tscn");
	public PackedScene catArea = GD.Load<PackedScene>("res://Scene/Cat.tscn");
	[Export]
	public int MaxPlants = 100;
	[Export]
	public int MaxAnimals = 100;
	public void Spawn(string type, Vector2 position, int count = 1)
	{
		switch (type)
		{
			case "Plant":
				if (GetTree().GetNodeCountInGroup("Plants") >= MaxPlants)
					return;
				for (int i = 0; i < count; i++)
				{
					var plant = plantArea.Instantiate<Plant>();
					plant.Position = position;
					plant.Breed += (count) => { Spawn("Plant", count); };
					AddChild(plant);
				}
				break;
			case "Mouse":
				if (GetTree().GetNodeCountInGroup("Animals") >= MaxAnimals)
					return;
				for (int i = 0; i < count; i++)
				{
					var mouse = mouseArea.Instantiate<Animal>();
					mouse.Position = position;
					mouse.Breed += (count) => { Spawn("Mouse", count); };
					AddChild(mouse);
				}
				break;
			case "Cat":
				if (GetTree().GetNodeCountInGroup("Animals") >= MaxAnimals)
					return;
				for (int i = 0; i < count; i++)
				{
					var cat = catArea.Instantiate<Animal>();
					cat.Position = position;
					cat.Breed += (count) => { Spawn("Cat", count); };
					AddChild(cat);
				}
				break;
			default:
				GD.PrintErr("Invalid type: " + type);
				break;
		}
	}
	public void Spawn(string type, int count = 1)
	{
		GD.Print("Spawn " + type + " count: " + count);
		switch (type)
		{
			case "Plant":
				if (GetTree().GetNodeCountInGroup("Plants") >= MaxPlants)
					return;
				for (int i = 0; i < count; i++)
				{
					var plant = plantArea.Instantiate<Plant>();
					plant.Position = new Vector2(GD.Randf() * 1000, GD.Randf() * 500);
					plant.Breed += (count) => { Spawn("Plant", count); };
					AddChild(plant);
				}
				break;
			case "Mouse":
				for (int i = 0; i < count; i++)
				{
					var mouse = mouseArea.Instantiate<Animal>();
					mouse.Position = new Vector2(GD.Randf() * 1000, GD.Randf() * 500);
					mouse.Breed += (count) => { Spawn("Mouse", count); };
					AddChild(mouse);
				}
				break;
			case "Cat":
				for (int i = 0; i < count; i++)
				{
					var cat = catArea.Instantiate<Animal>();
					cat.Position = new Vector2(GD.Randf() * 1000, GD.Randf() * 500);
					cat.Breed += (count) => { Spawn("Cat", count); };
					AddChild(cat);
				}
				break;
			default:
				GD.PrintErr("Invalid type: " + type);
				break;
		}
	}
	public void OnSpawnPlantButtonPressed()
	{
		Spawn("Plant", GetNode<LineEdit>("SpawnPlant/SpawnNum").Text.ToInt());
	}
	public void OnSpawnMouseButtonPressed()
	{
		Spawn("Mouse", GetNode<LineEdit>("SpawnMouse/SpawnNum").Text.ToInt());
	}
	public void OnSpawnCatButtonPressed()
	{
		Spawn("Cat", GetNode<LineEdit>("SpawnCat/SpawnNum").Text.ToInt());
	}
}
