using UnityEngine;

namespace Soccerpass
{
	[CreateAssetMenu(fileName = "PlayerConfig", menuName = "Data/PlayerConfig")]
	public class PlayerConfig : ScriptableObject
	{
		public SpeedData fieldPlayerSpeedData;

		public SpeedData goalkeeperSpeedData;
	}
}
