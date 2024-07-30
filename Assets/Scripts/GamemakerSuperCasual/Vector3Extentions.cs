using UnityEngine;

namespace GamemakerSuperCasual
{
	public static class Vector3Extentions
	{
		public static Vector3 MultiplyByScalars(this Vector3 input, Vector3 other)
		{
			Vector3 result = input;
			result.x *= other.x;
			result.y *= other.y;
			result.z *= other.z;
			return result;
		}

		public static string ToString(this Vector3 input)
		{
			return "(" + input.x + "," + input.y + "," + input.z + ")";
		}

		public static Vector3 Add(this Vector3 v, Vector2 v2)
		{
			v.x += v2.x;
			v.y += v2.y;
			return v;
		}
	}
}
