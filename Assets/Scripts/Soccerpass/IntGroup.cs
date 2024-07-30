using System.Collections.Generic;
using UnityEngine;

namespace Soccerpass
{
	[CreateAssetMenu(fileName = "IntGroup", menuName = "Data/IntGroup")]
	public class IntGroup : ScriptableObject
	{
		public List<int> list;

		public int RandomElement => list[Random.Range(0, list.Count)];
	}
}
