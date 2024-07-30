using System.Collections;
using UnityEngine;

#if DEFINE_UNITY_ADS
using UnityEngine.Advertisements;
#endif

namespace com.F4A.MobileThird
{
    /// <summary>
    /// 
    /// </summary>
	public partial class AdsManager
#if DEFINE_UNITY_ADS
        : IUnityAdsListener
#endif
    {
        private const string VideoUnityZone = "video";
	    private const string RewardedVideoUnityZone = "rewardedVideo";
	    private const string BannerUnityZone = "banner";

        private void StartUnityAds()
        {
#if DEFINE_UNITY_ADS
            Advertisement.AddListener(this);
#endif
        }

        private void InitializationUnityAds()
	    {
            RequestUnityAds();
        }

        private bool IsEnableUnityAd()
        {
            return adConfigData != null && adConfigData.unityAdConfig != null && adConfigData.unityAdConfig.enableAd;
        }

        private void RequestUnityAds()
        {
#if DEFINE_UNITY_ADS
            if (IsEnableUnityAd())
            {
                string id = adConfigData.unityAdConfig.GetIdAd();
                if (!string.IsNullOrEmpty(id))
                {
                    Debug.Log("RequestUnityAds id: " + id);
                    //------------------ initialize Unity Ads. ----------------------//
                    if (Advertisement.isSupported)
                    { // If the platform is supported,
                        Advertisement.Initialize(id, adConfigData.unityAdConfig.testMode);
                    }
                }
            }
#endif
        }

        protected bool IsRewardedUnityAdReady()
        {
#if DEFINE_UNITY_ADS
            if (IsEnableUnityAd() && Advertisement.IsReady(RewardedVideoUnityZone))
            {
                return true;
            }
#endif
            return false;
        }

        protected bool ShowRewardUnityAd()
        {
#if DEFINE_UNITY_ADS
            if (IsRewardedUnityAdReady())
            {
                _timeScalePrev = Time.timeScale;
                Time.timeScale = 0;
                Debug.Log("@LOG AdsManager.ShowRewardUnityAd");
                Advertisement.Show(RewardedVideoUnityZone);
                //Advertisement.Show(RewardedVideoUnityZone, new ShowOptions()
                //{
                //    resultCallback = result =>
                //    {
                //        Debug.Log("@LOG 1111");
                //        switch (result)
                //        {
                //            case ShowResult.Finished:
                //                OnRewardedAdCompleted?.Invoke(ERewardedAdNetwork.UnityAds, result.ToString(), DMCF4AConst.AmountAdsDefault);
                //                break;
                //            case ShowResult.Skipped:
                //                OnRewardedAdSkiped?.Invoke(ERewardedAdNetwork.UnityAds);
                //                break;
                //            case ShowResult.Failed:
                //                OnRewardedAdSkiped?.Invoke(ERewardedAdNetwork.UnityAds);
                //                break;
                //        }
                //    }
                //});
                return true;
            }
            return false;
#else
            return false;
#endif
        }

        protected bool IsVideoUnityAdReady()
        {
#if DEFINE_UNITY_ADS
            if (IsEnableUnityAd() && Advertisement.IsReady(VideoUnityZone.ToString()))
            {
                return true;
            }
#endif

            return false;
        }

        protected bool ShowVideoUnityAd()
        {
#if DEFINE_UNITY_ADS
            if (IsVideoUnityAdReady())
            {
                Advertisement.Show(VideoUnityZone.ToString(), new ShowOptions()
                {
                    resultCallback = result =>
                    {
                        switch (result)
                        {
                            case ShowResult.Finished:
                                if (OnVideodAdCompleted != null)
                                {
                                    OnVideodAdCompleted(EVideoAdNetwork.UnityAds);
                                }
                                break;
                            case ShowResult.Skipped:
                                break;
                            case ShowResult.Failed:
                                break;
                        }
                    }
                });
                return true;
            }
            return false;
#else
            return false;
#endif
        }

        protected bool IsInterstitialUnityAdReady()
        {
#if DEFINE_UNITY_ADS
            if (IsEnableUnityAd())
            {
                if (Advertisement.IsReady(VideoUnityZone.ToString()))
                {
                    return true;
                }
                else if (Advertisement.IsReady(RewardedVideoUnityZone.ToString()))
                {
                    return true;
                }
            }
#endif
            return false;
        }

        protected bool ShowInterstitialUnityAd()
        {
#if DEFINE_UNITY_ADS
            if (IsVideoUnityAdReady())
            {
                Advertisement.Show(VideoUnityZone.ToString(), new ShowOptions()
                {
                    resultCallback = result =>
                    {
                        switch (result)
                        {
                            case ShowResult.Finished:
                                OnInterstitialAdClosed?.Invoke(EInterstitialAdNetwork.UnityAds);
                                break;
                            case ShowResult.Skipped:
                                break;
                            case ShowResult.Failed:
                                break;
                        }
                    }
                });
                return true;
            }
            else if (IsRewardedUnityAdReady())
            {
                Advertisement.Show(RewardedVideoUnityZone, new ShowOptions()
                {
                    resultCallback = result =>
                    {
                        switch (result)
                        {
                            case ShowResult.Finished:
                                if (OnInterstitialAdClosed != null)
                                {
                                    OnInterstitialAdClosed(EInterstitialAdNetwork.UnityAds);
                                }
                                break;
                            case ShowResult.Skipped:
                                break;
                            case ShowResult.Failed:
                                break;
                        }
                    }
                });
                return true;
            }
#endif
            return false;
        }
        
	    protected bool IsBannerUnityAdReady()
	    {
#if DEFINE_UNITY_ADS
		    if (IsEnableUnityAd() && Advertisement.IsReady(BannerUnityZone.ToString()))
		    {
			    return true;
		    }
#endif
		    return false;
	    }
	    
	    protected bool ShowBannerUnityAd()
	    {
#if DEFINE_UNITY_ADS
		    if (IsBannerUnityAdReady())
		    {
			    Advertisement.Show(BannerUnityZone.ToString(), new ShowOptions()
			    {
				    resultCallback = result =>
				    {
					    switch (result)
					    {
						    case ShowResult.Finished:
							    break;
						    case ShowResult.Skipped:
							    break;
						    case ShowResult.Failed:
							    break;
					    }
				    }
                });
			    return true;
		    }
#endif
		    return false;
	    }

        public void OnUnityAdsReady(string placementId)
        {
            Debug.Log("@LOG OnUnityAdsReady");
        }

        public void OnUnityAdsDidError(string message)
        {
            Debug.Log("@LOG OnUnityAdsDidError");
        }

        public void OnUnityAdsDidStart(string placementId)
        {
            Debug.Log("@LOG OnUnityAdsDidStart");
        }

#if DEFINE_UNITY_ADS
        public void OnUnityAdsDidFinish(string placementId, ShowResult result)
        {
            //Debug.LogFormat("@LOG OnUnityAdsDidFinish {0},{1}", placementId, result.ToString());
            //if (RewardedVideoUnityZone.Equals(placementId))
            //{
            //    switch (result)
            //    {
            //        case ShowResult.Finished:
            //            OnRewardedAdCompleted?.Invoke(ERewardedAdNetwork.UnityAds, string.Empty, DMCF4AConst.AmountAdsDefault);
            //            break;
            //        case ShowResult.Skipped:
            //            OnRewardedAdSkiped?.Invoke(ERewardedAdNetwork.UnityAds);
            //            break;
            //        case ShowResult.Failed:
            //            OnRewardedAdFailed?.Invoke(ERewardedAdNetwork.UnityAds);
            //            break;
            //    }
            //}
            Time.timeScale = _timeScalePrev;
            StartCoroutine(IEUnityAdsDidFinish(placementId, result));
        }

        IEnumerator IEUnityAdsDidFinish(string placementId, ShowResult result)
        {
            yield return new WaitForEndOfFrame();
            Debug.LogFormat("@LOG OnUnityAdsDidFinish {0},{1}", placementId, result.ToString());
            if (RewardedVideoUnityZone.Equals(placementId))
            {
                switch (result)
                {
                    case ShowResult.Finished:
                        OnRewardedAdCompleted?.Invoke(ERewardedAdNetwork.UnityAds, string.Empty, DMCF4AConst.AmountAdsDefault);
                        break;
                    case ShowResult.Skipped:
                        OnRewardedAdSkiped?.Invoke(ERewardedAdNetwork.UnityAds);
                        break;
                    case ShowResult.Failed:
                        OnRewardedAdFailed?.Invoke(ERewardedAdNetwork.UnityAds);
                        break;
                }
            }
        }
#endif
    }
}