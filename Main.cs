using Godot;
using System;

public partial class Main : Node2D
{
	public PackedScene plantArea = GD.Load<PackedScene>("res://Plant.tscn");
	public PackedScene mouseArea = GD.Load<PackedScene>("res://Mouse.tscn");
	public PackedScene catArea = GD.Load<PackedScene>("res://Cat.tscn");

	public void SpawnPlant(int count = 1)
	{
		for (int i = 0; i < count; i++)
		{
			var plant = plantArea.Instantiate<Area2D>();
			plant.Position = new Vector2(GD.Randf() * 1000, GD.Randf() * 500);
			AddChild(plant);
		}
		GD.Print("Spawned " + count + " plants.");
	}
	public void OnSpawnPlantButtonPressed()
	{
		SpawnPlant(GetNode<LineEdit>("SpawnPlant/SpawnNum").Text.ToInt());
	}
}
