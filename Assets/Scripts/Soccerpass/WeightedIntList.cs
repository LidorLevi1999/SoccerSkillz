using System;

namespace Soccerpass
{
	[Serializable]
	public class WeightedIntList : WeightedElement<int>
	{
		public override bool IsValid()
		{
			return true;
		}
	}
}
