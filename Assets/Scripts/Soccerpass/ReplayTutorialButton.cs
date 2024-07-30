using com.gamemaker.soccerskillz;
using UnityEngine;
using UnityEngine.UI;

namespace Soccerpass
{
	public class ReplayTutorialButton : MonoBehaviour
	{
		private void Start()
		{
			GetComponent<Button>().onClick.AddListener(OnClick);
		}

		private void OnClick()
		{
			SessionState sessionState = SessionState.Load();
			TutorialManager.IsTutorialDone = false;
			sessionState.isReload = true;
			sessionState.isRevive = false;
			Utils.LoadGameScene();
		}
	}
}
