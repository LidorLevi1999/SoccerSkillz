using com.F4A.MobileThird;
using UnityEngine;
using UnityEngine.UI;

namespace GamemakerSuperCasual
{
	public class MoreAppsButton : MonoBehaviour
	{
		private void Start()
		{
			GetComponent<Button>().onClick.AddListener(OnClick);
		}

		private void OnClick()
		{
			SocialManager.Instance.OpenLinkDeveloper();
			//Singleton<PSDKWrapper>.Instance.RequestMoreApps().Done(delegate
			//{
			//});
		}
	}
}
