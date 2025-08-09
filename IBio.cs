using System.Collections;
using Godot;

public partial interface IBio
{
	public float Health { get; set; }
	public float Hunger { get; set; }
	public virtual void Eat(IBio node) { node.OnBeEaten(); }
	public virtual void OnBeEaten() { GD.Print("I am being eaten");QueueFree(); }

	void QueueFree();
}