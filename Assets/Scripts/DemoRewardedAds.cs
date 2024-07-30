using com.F4A.MobileThird;
using UnityEngine;

public class DemoRewardedAds : MonoBehaviour
{
	public void Show()
	{
		AdsManager.Instance.ShowRewardAds();
	}

	public bool IsAdReady()
	{
		return AdsManager.Instance.IsRewardAdsReady();
	}

	private void adIsReady()
	{
		UnityEngine.Debug.Log("DemoRewardedAds::adIsReady !");
	}

	private void adIsNotReady()
	{
		UnityEngine.Debug.Log("DemoRewardedAds::adIsNotReady !");
	}

	private void adWillShow()
	{
		UnityEngine.Debug.Log("DemoRewardedAds::adWillShow !");
	}

	private void adDidClose(bool rewarded)
	{
		if (rewarded)
		{
			adShouldReward();
		}
		else
		{
			adShouldNotReward();
		}
		UnityEngine.Debug.Log("DemoRewardedAds::adDidClose !");
	}

	private void adShouldReward()
	{
		UnityEngine.Debug.Log("DemoRewardedAds::adShouldReward !");
	}

	private void adShouldNotReward()
	{
		UnityEngine.Debug.Log("DemoRewardedAds::adShouldNotReward !");
	}
}
