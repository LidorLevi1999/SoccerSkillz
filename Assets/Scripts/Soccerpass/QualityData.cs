using UnityEngine;

namespace Soccerpass
{
	[CreateAssetMenu(fileName = "QualityData", menuName = "Data/QualityData")]
	public class QualityData : ScriptableObject
	{
		public bool crowdEnabled = true;

		public bool shadowsEnabled = true;

		public bool glowEnabled;

		public static int qualityLevel;

		private void OnEnable()
		{
			qualityLevel = QualitySettings.GetQualityLevel();
		}
	}
}
