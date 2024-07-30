using UnityEngine;

namespace Soccerpass
{
	public class Rotate : MonoBehaviour
	{
		public Vector3 rotationAmount;

		public float speed = 10f;

		private void Update()
		{
			base.transform.Rotate(rotationAmount * speed * Time.deltaTime);
		}
	}
}
