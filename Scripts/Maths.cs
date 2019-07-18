using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platformer.Scripts
{
	public static class Maths
	{
		public static float RoundToZero(float value, int places = 2)
		{

			value = Math.Round(value, places) == 0 ? (float)Math.Round(value, 2) : value;
			return value;
		}

		public static float Clamp(float value, float max)
		{
			if (Math.Abs(value) > max)
			{
				value = max * Math.Sign(value);
			}
			return value;
		}
	}
}
