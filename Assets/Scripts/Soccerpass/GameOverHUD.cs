using System.Collections.Generic;
using UnityEngine;

namespace Soccerpass
{
	public class GameOverHUD : MonoBehaviour, ITogglable
	{
		public List<Transform> highscoreElements;

		public List<Transform> normalScoreElements;

		public void Show()
		{
			bool isNewHighscore = StatsManager.IsNewHighscore;
			highscoreElements.ForEach(delegate(Transform e)
			{
				e.gameObject.SetActive(isNewHighscore);
			});
			normalScoreElements.ForEach(delegate(Transform e)
			{
				e.gameObject.SetActive(!isNewHighscore);
			});
			base.gameObject.SetActive(value: true);
		}

		public void Hide()
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
