using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace Platformer.Scripts.States
{
	class Run : State
	{

		public override void OnEnter()
		{
			var player = (PlayerCharacter)GetOwner();
			//player.Velocity.x = 0f;
		}

		public override void Update(float delta)
		{


		}

		public override void HandleInput()
		{
			var isLeft = Input.IsActionPressed("game_left");
			var isRight = Input.IsActionPressed("game_right");
			GD.Print($"{isLeft}, {isRight}");
			var inputHDirection = (isLeft ? -1 : 0) + (isRight ? 1 : 0);
			if (inputHDirection != 0)
			{
				var player = (PlayerCharacter)GetOwner();
				var sprite = (AnimatedSprite)player.GetNode("./Sprite");
				sprite.FlipH = (inputHDirection < 0);
				player.Velocity.x += inputHDirection * player.GroundAcc;
				player.Velocity.x = Maths.Clamp(player.Velocity.x, player.MaxGroundSpeed);
			}
			else
			{
				EmitSignal(nameof(Finished), "idle");
			}
			
		}
	}
}
