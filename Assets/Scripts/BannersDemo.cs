using com.F4A.MobileThird;
using UnityEngine;

public class BannersDemo : MonoBehaviour
{
	public void ShowBanners()
	{
		AdsManager.Instance.ShowBannerAds();
	}

	public void HideBanners()
	{
        AdsManager.Instance.HideBannerAds();
    }

    private void onPsdkReady()
	{
		UnityEngine.Debug.Log("Banners - onPsdkReady");
	}

	private void onBannerConfigurationUpdateEvent()
	{
		UnityEngine.Debug.Log("Banners - onBannerConfigurationUpdateEvent");
	}

	private void onBannerFailedEvent()
	{
		UnityEngine.Debug.Log("Banners - onBannerFailedEvent");
	}

	private void onBannerShownEvent()
	{
		UnityEngine.Debug.Log("Banners - onBannerShownEvent");
	}
}
