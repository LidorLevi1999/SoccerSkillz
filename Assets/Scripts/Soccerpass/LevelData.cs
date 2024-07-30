using System;
using System.Collections.Generic;
using GamemakerSuperCasual;
using UnityEngine;

namespace Soccerpass
{
	[Serializable]
	public class LevelData
	{
		public SpeedData fieldPlayerSpeedData;

		public SpeedData goalkeeperSpeedData;

		public List<WeightedGroup> weightedGroupsList;

		public List<WeightedIntList> weightedAIList;

		public int levelSize = 1600;

		public float distanceBetweenFormations = 120f;

		public bool IsValid()
		{
			if (weightedGroupsList == null)
			{
				return false;
			}
			if (weightedAIList == null)
			{
				return false;
			}
			bool isValid = true;
			weightedAIList.ForEach(delegate(WeightedIntList x)
			{
				isValid &= x.IsValid();
			});
			weightedGroupsList.ForEach(delegate(WeightedGroup x)
			{
				isValid &= x.IsValid();
			});
			return isValid;
		}

		public GameObject GetRandomFormation()
		{
			return weightedGroupsList.SelectOneWeighted((WeightedGroup sp) => sp.weight).group.RandomElement;
		}

		public List<GameObject> GetFormations()
		{
			return weightedGroupsList.SelectOneWeighted((WeightedGroup sp) => sp.weight).group.list;
		}

		public int GetPlayerAILevel()
		{
			return weightedAIList.SelectOneWeighted((WeightedIntList sp) => sp.weight).value;
		}
	}
}
