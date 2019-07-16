using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platformer.Scripts
{
	public class CharacterStats
	{



		public float MaxGroundSpeed;
		public float GroundAcc;

		public float MaxAirSpeed;
		public float AirAcc;

		public float MaxFallSpeed;


		public float FALL_SPEED = 10f;
	

		public CharacterState State;

		public CharacterStats(float groundSpeed, float groundAcc, float airSpeed, float airAcc, float fallSpeed)
		{

			State = CharacterState.Air;
			MaxGroundSpeed = groundSpeed;
			GroundAcc = groundAcc;
			MaxAirSpeed = airSpeed;
			AirAcc = airAcc;
			MaxFallSpeed = fallSpeed;
		}





		public enum CharacterState
		{
			Ground,
			Air
		}
	}
}
