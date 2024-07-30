using System.Collections.Generic;
using UnityEngine;

namespace Soccerpass
{
	[CreateAssetMenu(fileName = "ObjectsGroup", menuName = "Data/ObjectsGroup")]
	public class ObjectsGroup : ScriptableObject
	{
		public List<GameObject> list;

		public GameObject RandomElement => list[Random.Range(0, list.Count)];

		public bool IsValid()
		{
			if (list == null)
			{
				return false;
			}
			bool isValid = true;
			list.ForEach(delegate(GameObject x)
			{
				isValid &= (x != null);
			});
			return isValid;
		}
	}
}
