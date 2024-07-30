using com.gamemaker.soccerskillz;
using System;
using GamemakerSuperCasual;
using UnityEngine;

namespace Soccerpass
{
	public class RevivePanel : MonoBehaviour
	{
		public HUDState hudState;

		public GameParams gameParams;

		public LevelState levelState;

		public CountDownView countdownView;

		public GameObject restartButton;

		private void OnEnable()
		{
			PSDKWrapper.LogRVImpression();
			countdownView.countDownTime = gameParams.reviveOfferTime;
			restartButton.SetActive(value: false);
			countdownView.Toggle(toggle: true);
			LeanTween.delayedCall(base.gameObject, (float)gameParams.reviveRestartButtonDelay, (Action)delegate
			{
				restartButton.SetActive(value: true);
			});
		}

		public void OnReplayButtonClicked()
		{
			PSDKWrapper.LogRVClicked();
			countdownView.Toggle(toggle: false);
			hudState.value = HUDType.None;
			Singleton<PSDKWrapper>.Instance.ShowRV(delegate
			{
				SessionState sessionState = SessionState.Load();
				sessionState.isReload = true;
				sessionState.isRevive = true;
				Utils.LoadGameScene();
			}, delegate { OnTimeout(); }, delegate { OnTimeout(); });
		}

		public void OnTimeout()
		{
			Singleton<PSDKWrapper>.Instance.RequestUpdateLeaderboard(UnityEngine.Object.FindObjectOfType<StatsManager>().highscore);
			hudState.value = HUDType.GameOver;
		}

		private void OnDisable()
		{
			countdownView.Toggle(toggle: false);
			LeanTween.cancel(base.gameObject);
		}
	}
}
