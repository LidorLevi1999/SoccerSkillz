using com.F4A.MobileThird;
using com.gamemaker.soccerskillz;
using UnityEngine;
using UnityEngine.UI;

namespace GamemakerSuperCasual
{
	public class TermsOfServiceButton : MonoBehaviour
	{
		private void Start()
		{
			GetComponent<Button>().onClick.AddListener(OnClick);
		}

		private void OnClick()
		{
			SocialManager.Instance.OpenLinkTerms();
		}
	}
}
