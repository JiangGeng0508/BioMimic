using Godot;
using System;

public partial class Main : Node2D
{
	public PackedScene plantArea = GD.Load<PackedScene>("res://Scene/Plant.tscn");
	public PackedScene mouseArea = GD.Load<PackedScene>("res://Scene/Mouse.tscn");
	public PackedScene catArea = GD.Load<PackedScene>("res://Scene/Cat.tscn");
	[Export]
	public int MaxPlants = 100;
	public void SpawnPlant(int count = 1)
	{
		if (GetTree().GetNodeCountInGroup("Plants") >= MaxPlants)
			return;
		for (int i = 0; i < count; i++)
			{
				var plant = plantArea.Instantiate<Plant>();
				plant.Position = new Vector2(GD.Randf() * 1000, GD.Randf() * 500);
				plant.Spawn += SpawnPlant;
				AddChild(plant);
			}
		GD.Print("Spawned " + count + " plants.");
	}
	public void SpawnPlant(Vector2 position, int count = 1)
	{
		if (GetTree().GetNodeCountInGroup("Plants") >= MaxPlants)
			return;
		for (int i = 0; i < count; i++)
		{
			var plant = plantArea.Instantiate<Plant>();
			plant.Position = position;
			plant.Spawn += SpawnPlant;
			AddChild(plant);
		}
		GD.Print("Spawned " + count + " plants.");
	}
	public void OnSpawnPlantButtonPressed()
	{
		SpawnPlant(GetNode<LineEdit>("SpawnPlant/SpawnNum").Text.ToInt());
	}
}
