using UnityEngine;

namespace Soccerpass
{
	[CreateAssetMenu(fileName = "LevelState", menuName = "Data/LevelState")]
	public class LevelState : ScriptableObject
	{
		public int levelIndex;

		public LevelsConfig config;

		public LevelData CurrentLevel => config[levelIndex];
	}
}
