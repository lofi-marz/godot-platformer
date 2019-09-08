using Godot;

public abstract class State : Node
{
	public const string FINISHED = "Finished";
    [Signal]
    public delegate void Finished();
	public abstract void OnEnter();
	public virtual void OnExit()
	{

	}
	public abstract void Update(float delta);
	public virtual void HandleInput()
	{

	}
	
}

