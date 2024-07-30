using System;
using System.Collections;
using GamemakerSuperCasual;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Soccerpass
{
	public class Loader : MonoBehaviour
	{
		public Image splashImage;

		public float timeForSplash = 2f;

		private IEnumerator Start()
		{
			SessionState s = SessionState.Load();
			s.isReload = false;
			s.runCount = 0;
			s.isRevive = false;
			yield return new WaitForSeconds(0.1f);
			SceneManager.LoadScene(1);
			yield return null;
		}

		private void OnGameSceneLoaded()
		{
			LeanTween.alpha(splashImage.rectTransform, 0f, 0.6f).setEaseInOutSine().setOnComplete((Action)delegate
			{
				SceneManager.UnloadSceneAsync(0);
			});
		}
	}
}
