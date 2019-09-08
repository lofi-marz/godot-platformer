using Godot;
using Platformer.Scripts.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platformer.Scripts
{
	public class PlayerStateMachine : StateMachine
	{
		public override void _Ready()
		{
			base._Ready();
			StateMap = new Dictionary<string, State>
			{
				{ "idle", (State)GetNode("Idle")},
				{"run", (State)GetNode("Run") },
				{"jump", (State)GetNode("Jump") }
			};
			CurrentState = StateMap["idle"];
			Active = true;
			CurrentState.OnEnter();

		}

		public override void _PhysicsProcess(float delta)
		{
			base._PhysicsProcess(delta);
			
		}

		public override void _Input(InputEvent e)
		{
			base._Input(e);
			if (e.IsActionPressed("game_up"))
			{
				if (CheckStatesActive("run", "idle"))
				{
					ChangeState("jump");
				}
			}
		}
	}
}
