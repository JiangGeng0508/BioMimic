using Godot;

public interface IBio
{
	public float Health { get; set; }
	public float Hunger { get; set; }
	public float EatEfficiency { get; set; }
	public virtual void Eat(IBio node)
	{ 
		Hunger += node.BeEaten();
	}
	public virtual float BeEaten()
	{
		GD.Print("I am being eaten");
		QueueFree();
		return Hunger * EatEfficiency;
	}

	void QueueFree();
}