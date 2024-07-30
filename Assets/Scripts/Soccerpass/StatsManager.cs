using System;
using GamemakerSuperCasual;
using UnityEngine;

namespace Soccerpass
{
	public class StatsManager : MonoBehaviour
	{
		public IntVar score;

		public IntVar highscore;

		public IntVar passCount;

		public IntVar goalCount;

		public int maxGoals;

		public FloatVar distance;

		public GameParams gameParams;

		public int lastDistanceScore;

		public int reviveCount;

		public bool inRun;

		private bool highscoreNotified;

		public bool CanRevive => (int)goalCount > 0 && reviveCount == 0;

		public static bool IsNewHighscore
		{
			get
			{
				return TTPlayerPrefs.GetBool("new_highscore");
			}
			private set
			{
				TTPlayerPrefs.SetBool("new_highscore", value);
			}
		}

		public static int runCount
		{
			get
			{
				return TTPlayerPrefs.GetInt("runCount");
			}
			private set
			{
				TTPlayerPrefs.SetInt("runCount", value);
				TTPlayerPrefs.Save();
			}
		}

		public static event Action<int> NewHighScoreEvent;

		public static event Action<int> NewMaxGoalsEvent;

		public void ResetStats()
		{
			inRun = false;
			score.value = 0;
			highscore.value = TTPlayerPrefs.GetInt("highscore");
			passCount.value = 0;
			goalCount.value = 0;
			distance.value = 0f;
			highscoreNotified = false;
			IsNewHighscore = false;
			lastDistanceScore = 0;
			reviveCount = 0;
			maxGoals = TTPlayerPrefs.GetInt("maxgoals");
		}

		public void StartRun()
		{
			runCount++;
			inRun = true;
		}

		public void OnRevive()
		{
			reviveCount++;
			lastDistanceScore = (int)((float)distance / gameParams.scoreDistanceUnit);
		}

		public void UpdateScoreByDistance(float ballDistance)
		{
			if ((float)distance != ballDistance)
			{
				distance.value = ballDistance;
				UpdateScore();
			}
		}

		private void UpdateScore()
		{
			int num = (int)((float)distance / gameParams.scoreDistanceUnit);
			int num2 = (int)goalCount * gameParams.goalBonus;
			score.value = lastDistanceScore + num2 + num;
			UpdateHighScore();
		}

		private void UpdateHighScore()
		{
			if (score.value > (int)highscore)
			{
				if ((int)highscore > 15)
				{
					IsNewHighscore = true;
				}
				highscore.value = score.value;
				TTPlayerPrefs.SetInt("highscore", highscore.value);
				TTPlayerPrefs.Save();
			}
			if (IsNewHighscore && !highscoreNotified)
			{
				highscoreNotified = true;
				StatsManager.NewHighScoreEvent(highscore);
			}
		}

		public void CountPass()
		{
			passCount.value++;
			UpdateScore();
		}

		public void CountGoal()
		{
			goalCount.value++;
			if (goalCount.value >= maxGoals)
			{
				TTPlayerPrefs.SetInt("maxgoals", maxGoals);
				TTPlayerPrefs.Save();
			}
			UpdateScore();
		}

		static StatsManager()
		{
			StatsManager.NewHighScoreEvent = delegate
			{
			};
			StatsManager.NewMaxGoalsEvent = delegate
			{
			};
		}
	}
}
