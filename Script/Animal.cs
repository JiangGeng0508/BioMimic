using Godot;
using System;
using System.Linq;

public partial class Animal : CharacterBody2D, IBio
{
	[Signal]
	public delegate void BreedEventHandler(int count);//繁殖信号
	[Export(PropertyHint.Enum, "Herbivore,Carnivore,Omnivore")]
	public AnimalTypeEnum Diet { get; set; } = AnimalTypeEnum.Omnivore;//食性
	private AnimalStateEnum _state = AnimalStateEnum.Fine;//状态
	public AnimalStateEnum State
	{
		get
		{
			return _state;
		}
		set
		{
			_state = value;
			switch (value)
			{
				case AnimalStateEnum.Fine:
					WanderTarget = GlobalPosition;
					break;
				case AnimalStateEnum.Hunting:
					break;
				case AnimalStateEnum.Sleeping:
					break;
				default:
					GD.Print($"State {value} not defined");
					break;
			}
		}
	}
	[Export]
	public float MaxHealth { get; set; } = 100.0f;
	[Export]
	public float MaxHunger { get; set; } = 100.0f;
	public float Health { get; set; } = 100.0f;
	public float Hunger { get; set; } = 100.0f;
	[Export]
	public float EatEfficiency { get; set; } = 0.8f;//食物链能量传递效率（本级传向下一级）
	[Export]
	public float EatRange { get; set; } = 30.0f;//检测食物区域的半径
	[Export]
	public bool BeingControlled { get; set; } = false;//是否被控制
	[Export]
	public bool LabelVisible { get; set; } = false;//是否显示信息标签
	public Node2D TargetNode { get; set; } = null;//当前捕猎目标节点
	public Vector2 WanderTarget { get; set; }//随机游走目标位置
	public Label InfoLabel { get; set; }//信息标签节点
	public AnimatedSprite2D anim;//动画节点

	public override void _Ready()
	{
		//添加分类
		AddToGroup($"{Diet}Animals");
		AddToGroup("Animals");
		GetNode<CollisionShape2D>("Mouth/ReachArea").Shape.Set("Radius", EatRange);//设置检测范围
																				   //初始化属性
		Hunger = MaxHunger;
		Health = MaxHealth;
		anim = GetNode<AnimatedSprite2D>("Anim");//初始化动画
		anim.Play("Idle");
		InfoLabel = GetNode<Label>("InfoLabel");//初始化信息标签
		InfoLabel.Visible = LabelVisible;
		WanderTarget = GlobalPosition;//初始化随机游走目标位置
	}
	public override void _PhysicsProcess(double delta)
	{
		Hunger -= (float)delta * 0.3f;//减少饥饿度
		InfoLabel.Visible = LabelVisible;//显示信息标签
		//更新信息标签内容
		InfoLabel.Text = $"\nHealth: {Health:F1}\nHunger: {Hunger:F1}\nState: {State}\nWanderTarget: {WanderTarget}\nVelocity: {Velocity}\n";
		if (Health <= 0)//死亡
		{
			QueueFree();
			return;
		}
		else if (Health >= MaxHealth * 2f)//繁殖
		{
			GD.Print("Health is too high");
			Health = MaxHealth;
			EmitSignal(nameof(Breed), 1);
		}
		if (Hunger <= 0)//饥饿
		{
			Health -= (float)delta * 0.2f;
			Hunger = 0;
		}
		else if (Hunger > MaxHunger * 0.5f && Hunger < MaxHunger)//饱腹
		{
			Health += (float)delta * 0.3f;
		}
		else if (Hunger >= MaxHunger)//过饱
		{
			Health += Hunger - MaxHunger;
			Hunger = MaxHunger;
		}

		if (Hunger <= MaxHunger * 0.5f)//饥饿状态
		{
			if (State != AnimalStateEnum.Hunting)
			{
				State = AnimalStateEnum.Hunting;

				switch (Diet)
				{
					case AnimalTypeEnum.Herbivore:
						var nodes1 = GetTree().GetNodesInGroup("Plants");
						nodes1 = (Godot.Collections.Array<Node>)nodes1.Except(GetTree().GetNodesInGroup("HuntingTargets"));
						if (nodes1.Count == 0)
						{
							State = AnimalStateEnum.Fine;
							return;
						}
						var plants = nodes1.Cast<Plant>();
						var nearestPlant = plants.OrderBy(p => p.GlobalPosition.DistanceTo(GlobalPosition)).First();
						SetTarget(nearestPlant);
						break;
					case AnimalTypeEnum.Carnivore:
						var nodes2 = GetTree().GetNodesInGroup("HerbivoreAnimals");
						nodes2 = (Godot.Collections.Array<Node>)nodes2.Except(GetTree().GetNodesInGroup("HuntingTargets"));
						if (nodes2.Count == 0)
						{
							State = AnimalStateEnum.Fine;
							return;
						}
						var animals = nodes2.Cast<Animal>();
						var nearestAnimal = animals.OrderBy(p => p.GlobalPosition.DistanceTo(GlobalPosition)).First();
						SetTarget(nearestAnimal);
						break;
					case AnimalTypeEnum.Omnivore:

						break;
					default:
						break;
				}
			}
		}
		else if (Health <= MaxHealth * 0.5f)//受伤状态
		{
			if (State != AnimalStateEnum.Sleeping)
			{
				State = AnimalStateEnum.Sleeping;
			}
		}
		//超过阈值恢复状态
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
				var pos1 = WanderTarget - GlobalPosition;
				if (pos1.Length() > 10f)
				{
					Velocity = pos1.Normalized() * 200f;
				}
				else
				{
					WanderTarget = GlobalPosition + new Vector2(GD.Randf() * 200f - 100f, GD.Randf() * 200f - 100f);
				}
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
		switch (Diet)
		{
			case AnimalTypeEnum.Herbivore:
				if (body is Plant plant)
				{
					Eat(plant);
				}
				break;
			case AnimalTypeEnum.Carnivore:
				if (body is Animal animal && animal != this)
				{
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
				GD.PrintErr("Diet not supported");
				break;
		}
	}
	public void Eat(IBio node)
	{
		Hunger += node.BeEaten();
		TargetNode = null;
		Velocity = Vector2.Zero;
	}
	public void SetTarget(Node2D node)
	{
		TargetNode = node;
		node.AddToGroup("HuntingTargets");//标记猎物，防止重复捕猎
		node.TreeExiting += () =>
		{
			TargetNode = null;
			State = AnimalStateEnum.Fine;
			Velocity = Vector2.Zero;
		};
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
