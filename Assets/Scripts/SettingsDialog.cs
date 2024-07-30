using UnityEngine;

public class SettingsDialog : MonoBehaviour
{
	//@TODO: SettingsDialog
	[SerializeField]
	private GameObject mTermsOfUse;

	[SerializeField]
	private GameObject mPrivacyPolicy;

	[SerializeField]
	private GameObject mPrivacySettings;

	public void OnPrivacySettingsButtonClicked()
	{
		//PSDKMgr.Instance.GetConsentService().ShowPrivacySettings();
	}

	private void Awake()
	{
		UpdateView();
		//PsdkSingleton<PsdkEventSystem>.Instance.onPsdkReady += OnPsdkReady;
	}

	private void OnPsdkReady()
	{
		//PsdkSingleton<PsdkEventSystem>.Instance.onPsdkReady -= OnPsdkReady;
		UpdateView();
	}

	private void UpdateView()
	{
		try
		{
			//bool flag = PSDKMgr.Instance.GetConsentService().ShouldShowPrivacyPolicyOption();
			bool flag = true;
			mTermsOfUse.SetActive(!flag);
			mPrivacyPolicy.SetActive(!flag);
			mPrivacySettings.SetActive(flag);
		}
		catch
		{
		}
	}
}
