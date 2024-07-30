using com.F4A.MobileThird;
using UnityEngine;
using UnityEngine.UI;

namespace GamemakerSuperCasual
{
	public class RateUsButton : MonoBehaviour
	{
		private void Start()
		{
			GetComponent<Button>().onClick.AddListener(OnClick);
		}

		private void OnClick()
		{
			//Singleton<PSDKWrapper>.Instance.RequestRateUs();
			SocialManager.Instance.OpenRateGame();
		}
	}
}
