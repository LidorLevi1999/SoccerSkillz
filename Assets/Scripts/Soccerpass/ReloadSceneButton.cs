using com.gamemaker.soccerskillz;
using System;
using GamemakerSuperCasual;
using UnityEngine;
using UnityEngine.UI;

namespace Soccerpass
{
	[RequireComponent(typeof(Button))]
	public class ReloadSceneButton : MonoBehaviour
	{
		public static event Action SceneWillReloadEvent;

		private void Start()
		{
			GetComponent<Button>().onClick.AddListener(OnClick);
		}

		private void OnClick()
		{
			Singleton<PSDKWrapper>.Instance.ShowInterstitialRetry(delegate
            {
                SessionState sessionState = SessionState.Load();
                sessionState.isRevive = false;
                sessionState.isReload = false;
                ReloadSceneButton.SceneWillReloadEvent();
                Utils.LoadGameScene();
            });
		}

		static ReloadSceneButton()
		{
			ReloadSceneButton.SceneWillReloadEvent = delegate
			{
			};
		}
	}
}
