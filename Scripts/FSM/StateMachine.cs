using Godot;
using System;
using System.Collections.Generic;

public class StateMachine : Node
{

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
            child.Connect(State.FINISHED, this, nameof(ChangeState));
		}
		Active = false;
		
	}

	public void ChangeState(string newState)
	{
		GD.Print($"Changing State ->{newState}");

		CurrentState.OnExit();
		if (newState == "previous" && States.Count < 1)
		{
			States.Pop();
			CurrentState = States.Peek();
		}
		else if (newState == "previous")
		{
            CurrentState = StateMap[StartState];
            States.Push(CurrentState);

		}
		else
		{
			CurrentState = StateMap[newState];
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

