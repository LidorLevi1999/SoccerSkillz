using UnityEngine;
using UnityEngine.UI;

namespace GamemakerSuperCasual
{
	public class SocialLoginButtons : MonoBehaviour
	{
		public Button btnLogin;

		public Button btnLogout;

		private void Awake()
		{
		}

		private void OnEnable()
		{
			RefreshView();
			PSDKWrapper.onSocialAuthenticate += PSDKWrapper_onSocialAuthenticate;
			PSDKWrapper.onSocialSignOut += PSDKWrapper_onSocialSignOut;
		}

		private void RefreshView()
		{
			Toggle(Singleton<PSDKWrapper>.Instance.IsSocialConnected());
		}

		private void PSDKWrapper_onSocialSignOut()
		{
			Toggle(isLoggedIn: false);
		}

		private void PSDKWrapper_onSocialAuthenticate(bool success)
		{
			Toggle(success);
		}

		private void OnDisable()
		{
			PSDKWrapper.onSocialAuthenticate -= PSDKWrapper_onSocialAuthenticate;
			PSDKWrapper.onSocialSignOut -= PSDKWrapper_onSocialSignOut;
		}

		private void Toggle(bool isLoggedIn)
		{
			btnLogin.gameObject.SetActive(!isLoggedIn);
			btnLogout.gameObject.SetActive(isLoggedIn);
		}

		private void Start()
		{
			btnLogin.onClick.AddListener(Login);
			btnLogout.onClick.AddListener(Logout);
		}

		private void Login()
		{
			Singleton<PSDKWrapper>.Instance.RequestSocialConnect();
		}

		private void Logout()
		{
			Singleton<PSDKWrapper>.Instance.RequestSocialDisconnect();
			RefreshView();
		}
	}
}
