using System;

namespace Soccerpass
{
	[Serializable]
	public class WeightedGroup
	{
		public ObjectsGroup group;

		public int weight;

		public bool IsValid()
		{
			return group != null && group.IsValid();
		}
	}
}
