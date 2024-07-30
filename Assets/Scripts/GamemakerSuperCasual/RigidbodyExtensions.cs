using System.Collections.Generic;
using UnityEngine;

namespace GamemakerSuperCasual
{
	public static class RigidbodyExtensions
	{
		private static Dictionary<Rigidbody, Vector3[]> velocities = new Dictionary<Rigidbody, Vector3[]>();

		private const int VELOCITY_INDEX = 0;

		private const int ANGULAR_VELOCITY_INDEX = 1;

		public static void Pause(this Rigidbody rb)
		{
			RemoveNullVelocities();
			Vector3[] value = new Vector3[2]
			{
				rb.velocity,
				rb.angularVelocity
			};
			if (velocities.ContainsKey(rb))
			{
				velocities[rb] = value;
			}
			else
			{
				velocities.Add(rb, value);
			}
			rb.isKinematic = true;
		}

		public static void UnPause(this Rigidbody rb)
		{
			RemoveNullVelocities();
			rb.isKinematic = false;
			if (velocities.TryGetValue(rb, out Vector3[] value))
			{
				rb.velocity = value[0];
				rb.angularVelocity = value[1];
				velocities.Remove(rb);
			}
		}

		public static void ClearVelocity(this Rigidbody rb)
		{
			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
		}

		private static void RemoveNullVelocities()
		{
			velocities = velocities.WithoutNullKeys();
		}
	}
}
