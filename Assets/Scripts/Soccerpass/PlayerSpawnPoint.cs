using UnityEngine;

namespace Soccerpass
{
	public class PlayerSpawnPoint : MonoBehaviour
	{
		public PlayerRole role = PlayerRole.Default;

		public bool isHomeTeam;

		public int playerLevel = -1;

		public PlayerData data
		{
			get;
			set;
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = ((!isHomeTeam) ? Color.red : Color.blue);
			Gizmos.DrawSphere(base.transform.position, 2f);
		}
	}
}
