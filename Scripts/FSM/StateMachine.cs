using Godot;
using System;
using System.Collections.Generic;

public abstract class State : Node
{

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

public class StateMachine : Node
{
	[Signal]
	public delegate void StateChange();
	
	public StateChange StateChanged;
	public Dictionary<string, State> StateMap;
	public Stack<State> States;
	
	public string StartState;

	public bool Active
	{
		get
		{
			return _active;
		}
		set
		{
			_active = value;
			SetPhysicsProcess(value);
			SetProcessInput(value);
		}
	}
	private bool _active;
	public float Timer;
	public State CurrentState { get
		{
			return _currentState;
		}
		set
		{
			EmitSignal(nameof(StateChange));
			Timer = 0f;
			_currentState = value;
		}
	}
		
	private State _currentState;

	public bool CheckStatesActive(params string[] states)
	{
		foreach (string state in states)
		{
			if (CurrentState == StateMap[state])
			{
				return true;
			}
		}
		return false;
	}
	public override void _Ready()
	{
		base._Ready();
		States = new Stack<State>();
		foreach (State child in GetChildren())
		{
			child.Connect(nameof(State.Finished), this, nameof(ChangeState));
		}
		Active = false;
		
	}

	public void ChangeState(string stateName)
	{
		GD.Print("Changing State");

		CurrentState.OnExit();
		if (stateName == "previous" && States.Count < 1)
		{
			States.Pop();
			CurrentState = States.Peek();
		}
		else
		{
			CurrentState = StateMap[stateName];
			States.Push(CurrentState);
		}
		
		CurrentState.OnEnter();
	}

	public override void _PhysicsProcess(float delta)
	{
		Timer += delta;
		CurrentState.Update(delta);
		
		
	}

	public override void _Input(InputEvent e)
	{
		base._Input(e);
		CurrentState.HandleInput();
	}
}

public delegate void EnterState();
public delegate void ExitState();
public delegate void UpdateState(float delta);
public delegate void HandleStateInput(InputEvent e);

