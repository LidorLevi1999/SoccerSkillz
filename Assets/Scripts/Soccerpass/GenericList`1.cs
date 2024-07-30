using System.Collections.Generic;
using UnityEngine;

namespace Soccerpass
{
	[CreateAssetMenu(fileName = "ColorList", menuName = "Data/ColorList")]
	public abstract class GenericList<T> : ScriptableObject
	{
		public List<T> list;

		public T this[int i] => list[i];

		public int Count => (!list.IsNullOrEmpty()) ? list.Count : 0;

		public T RandomElement => list.RandomElement();
	}
}
