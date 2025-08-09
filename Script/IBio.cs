using Godot;

public partial interface IBio
{
	public float Health { get; set; }
	public float Hunger { get; set; }
	public virtual void Eat(IBio node)
	{
		Hunger += node.Hunger;
		node.BeEaten();
	}
	public virtual void BeEaten() { GD.Print("I am being eaten");QueueFree(); }

	void QueueFree();
}