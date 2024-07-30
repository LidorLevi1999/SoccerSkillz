using System.Collections.Generic;
using UnityEngine;

namespace Soccerpass
{
	[CreateAssetMenu(fileName = "LevelsConfig", menuName = "Data/LevelsConfig")]
	public class LevelsConfig : ScriptableObject
	{
		public List<LevelData> levels;

		public PlayerConfig defaultPlayerConfig;

		private int numLevels = 6;

		public bool createLevelsOnValidateIfEmpty;

		public bool checkValid;

		public LevelData this[int i] => levels[i];

		[ContextMenu("Validate")]
		public void CheckValidity()
		{
			for (int i = 0; i < levels.Count; i++)
			{
				if (levels[i] == null)
				{
					UnityEngine.Debug.LogError("Level: " + i + " is empty!");
				}
				else if (!levels[i].IsValid())
				{
					UnityEngine.Debug.LogError("Level: " + i + " is not valid!");
				}
			}
		}

		private void OnValidate()
		{
			if (createLevelsOnValidateIfEmpty)
			{
				if ((levels == null || levels.Count == 0) && !(defaultPlayerConfig == null))
				{
					levels = new List<LevelData>();
					for (int i = 0; i < numLevels; i++)
					{
						LevelData levelData = new LevelData();
						levelData.fieldPlayerSpeedData = defaultPlayerConfig.fieldPlayerSpeedData;
						levelData.goalkeeperSpeedData = defaultPlayerConfig.goalkeeperSpeedData;
						levels.Add(levelData);
					}
				}
			}
			else if (checkValid)
			{
				checkValid = false;
				CheckValidity();
			}
		}
	}
}
