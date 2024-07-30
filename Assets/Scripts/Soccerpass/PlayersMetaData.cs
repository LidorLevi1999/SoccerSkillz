using System.Collections.Generic;
using UnityEngine;

namespace Soccerpass
{
	[CreateAssetMenu(fileName = "PlayersMetaData", menuName = "Data/PlayersMetaData")]
	public class PlayersMetaData : ScriptableObject
	{
		public List<string> names = new List<string>
		{
			"Messi",
			"Ronaldo",
			"Zehavi",
			"Pique",
			"Atar",
			"Dasa",
			"Benayun",
			"Berko",
			"Ofira"
		};

		public List<Material> outfits;

		public string RandomName => names.RandomElement();
	}
}
