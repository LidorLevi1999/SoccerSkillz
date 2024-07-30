using com.gamemaker.soccerskillz;
using GamemakerSuperCasual;
using UnityEngine;
using UnityEngine.UI;

namespace Soccerpass
{
	[RequireComponent(typeof(Button))]
	public class ReplayButton : MonoBehaviour
	{
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
                sessionState.isReload = true;
                Utils.LoadGameScene();
            });
		}
	}
}
