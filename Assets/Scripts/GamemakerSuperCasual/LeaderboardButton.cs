using com.F4A.MobileThird;
using UnityEngine;
using UnityEngine.UI;

namespace GamemakerSuperCasual
{
	public class LeaderboardButton : MonoBehaviour
	{
		private void Start()
		{
			GetComponent<Button>().onClick.AddListener(OnClick);
		}

		private void OnClick()
		{
			SocialManager.Instance.ShowLeaderBoard();
			//Singleton<PSDKWrapper>.Instance.RequestOpenLeaderboard();
		}
	}
}
