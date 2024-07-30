using com.F4A.MobileThird;
using RSG;
using Soccerpass;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Gamemaker;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace GamemakerSuperCasual
{
	public class PSDKWrapper : Singleton<PSDKWrapper>
	{
		private bool countGameTime = true;

		private int gameTimeSeconds;

		private int gameTimeMaxMinutes = 300;

		private int gameTimeMaxSeconds;

		private Promise<bool> _showLocationPromise;

		private int lastReportedLeaderboardScore;

		private bool appIsReadyFlag_psdkResumed;

		private static bool isPsdkReady;

		private ScriptablePsdkData psdkData;


		private Dictionary<string, object> eventParams = new Dictionary<string, object>();

		private StatsManager _statsManager;

		public bool SessionStartReady => IsLocationReady("sessionStart");

		public bool SessionStartPending
		{
			get;
			private set;
		}

		private StatsManager statsManager
		{
			get
			{
				if (_statsManager == null)
				{
					_statsManager = UnityEngine.Object.FindObjectOfType<StatsManager>();
				}
				return _statsManager;
			}
		}

		public bool IsBillingInitialized
		{
			get
			{
				if (IAPManager.Instance == null)
				{
					return false;
				}
				return IAPManager.Instance.IsInitialized();
			}
		}

		public static event Action PSDKReadyEvent;

		public static event Action<bool> PauseGameMusicEvent;

		public static event Action<bool> RVClosedEvent;

		public static event Action ReturnFromBackgroundNewSession;

		public static event Action ReturnFromBackgroundRestartApp;

		public static event Action<bool> NotifyOnBillingPurchaseRestored;

		public static event Action<bool> onSocialAuthenticate;

		public static event Action onSocialSignOut;

		public static event Action<bool> ToggleBlockTouchesEvent;

		public static event Action NoInternetEvent;

		public static event Action<string> OnLocationLoaded;

		public Action onInterstitialDone = null;
		public Action onRewardCompleted, onRewardSkiped, onRewardFailed;

        private void OnEnable()
        {
            AdsManager.OnInterstitialAdClosed += AdsManager_OnInterstitialAdClosed;
            AdsManager.OnInterstitialAdFailed += AdsManager_OnInterstitialAdFailed;

            AdsManager.OnRewardedAdCompleted += AdsManager_OnRewardedAdCompleted;
            AdsManager.OnRewardedAdFailed += AdsManager_OnRewardedAdFailed;
            AdsManager.OnRewardedAdSkiped += AdsManager_OnRewardedAdSkiped;
        }

        private void OnDisable()
        {
            AdsManager.OnInterstitialAdClosed -= AdsManager_OnInterstitialAdClosed;
            AdsManager.OnInterstitialAdFailed -= AdsManager_OnInterstitialAdFailed;

            AdsManager.OnRewardedAdCompleted -= AdsManager_OnRewardedAdCompleted;
            AdsManager.OnRewardedAdFailed -= AdsManager_OnRewardedAdFailed;
            AdsManager.OnRewardedAdSkiped -= AdsManager_OnRewardedAdSkiped;
        }

        private void AdsManager_OnRewardedAdSkiped(ERewardedAdNetwork adNetwork)
        {
			onRewardSkiped?.Invoke();
        }

        private void AdsManager_OnRewardedAdFailed(ERewardedAdNetwork adNetwork)
        {
			onRewardFailed?.Invoke();
        }

        private void AdsManager_OnRewardedAdCompleted(ERewardedAdNetwork adNetwork, string adName, double value)
        {
			onRewardCompleted?.Invoke();
        }
        private void AdsManager_OnInterstitialAdFailed(EInterstitialAdNetwork adNetwork)
        {
        }

        private void AdsManager_OnInterstitialAdClosed(EInterstitialAdNetwork adNetwork)
        {
			onInterstitialDone?.Invoke();

		}

        public IPromise ShowSessionStart()
		{
			Promise p = new Promise();
			ShowLocation("sessionStart").Done(delegate
			{
				ShowBanner();
				SessionStartPending = false;
				p.Resolve();
			});
			return p;
		}

		protected override void Awake()
		{
			base.Awake();
			psdkData = ScriptablePsdkData.Load();
			psdkData.SetUrls();
			lastReportedLeaderboardScore = TTPlayerPrefs.GetInt("LastReportedLeaderboardScore");
			if (countGameTime)
			{
				gameTimeMaxSeconds = 60 * gameTimeMaxMinutes;
				gameTimeSeconds = PlayerPrefs.GetInt("CountGameTimeSeconds", 0);
				if (gameTimeSeconds < gameTimeMaxSeconds)
				{
					StartCoroutine(CountGameTimeCoro());
				}
				else
				{
					countGameTime = false;
				}
			}
		}

		private IEnumerator CountGameTimeCoro()
		{
			while (countGameTime)
			{
				yield return new WaitForSeconds(1f);
				gameTimeSeconds++;
				PlayerPrefs.SetInt("CountGameTimeSeconds", gameTimeSeconds);
				if (gameTimeSeconds >= gameTimeMaxSeconds)
				{
					countGameTime = false;
				}
			}
		}

		private void PsdkEvents_onBlockTouches(bool obj)
		{
			PSDKWrapper.ToggleBlockTouchesEvent(obj);
		}

		private void PsdkEvents_onRewardedAdDidClosedWithResultEvent(bool rvResult)
		{
			PSDKWrapper.ToggleBlockTouchesEvent(obj: false);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
		}

		private void OnPsdkReady()
		{
			if (isPsdkReady)
			{
				return;
			}
			isPsdkReady = true;
			SessionStartPending = true;
			PSDKWrapper.PSDKReadyEvent();
		}

		private void PsdkEvents_onLocationFailedEvent(string location, long attr)
		{
			PSDKWrapper.ToggleBlockTouchesEvent(obj: false);
			ShowBanner();
			if (_showLocationPromise == null)
			{
				UnityEngine.Debug.Log("@LOG PsdkEvents_onLocationFailedEvent _showLocationPromise=null");
			}
			else
			{
				UnityEngine.Debug.Log("@LOG PsdkEvents_onLocationFailedEvent _showLocationPromise.CurState=" + _showLocationPromise.CurState);
			}
		}

		private void PsdkEvents_onLocationLoadedEvent(string location, long attributes)
		{
			PSDKWrapper.OnLocationLoaded(location);
		}

		private void PsdkEvents_onClosedEvent(string location, long attributes)
		{
			if (_showLocationPromise == null)
			{
				UnityEngine.Debug.Log("PsdkEvents_onClosedEvent _showLocationPromise=null");
			}
			else
			{
				UnityEngine.Debug.Log("PsdkEvents_onClosedEvent _showLocationPromise.CurState=" + _showLocationPromise.CurState);
			}
			if (_showLocationPromise != null && _showLocationPromise.CurState == PromiseState.Pending)
			{
				_showLocationPromise.Resolve(value: true);
			}
			LeanTween.delayedCall(0.01f, (Action)delegate
			{
				PSDKWrapper.ToggleBlockTouchesEvent(obj: false);
			});
		}

		private void PsdkEvents_pauseGameMusicEvent(bool pause)
		{
			PSDKWrapper.PauseGameMusicEvent(pause);
		}

		private bool IsLocationReady(string locationId)
		{
			return true;
		}

		private IPromise<bool> ShowLocation(string locationId, bool showLoader = false)
		{
			UnityEngine.Debug.Log("@LOG PSDKWrapper ShowLocation " + locationId);
			_showLocationPromise = new Promise<bool>();
			if (!isPsdkReady)
			{
				UnityEngine.Debug.Log("@LOG PSDKWrapper ShowLocation !IsPSDKReady");
				PSDKWrapper.ToggleBlockTouchesEvent(obj: false);
				_showLocationPromise.Resolve(value: false);
				return _showLocationPromise;
			}
			//if (locationService == null)
			//{
			//	UnityEngine.Debug.Log("@LOG PSDKWrapper ShowLocation locationService is null");
			//	PSDKWrapper.ToggleBlockTouchesEvent(obj: false);
			//	_showLocationPromise.Resolve(value: false);
			//	return _showLocationPromise;
			//}
			//UnityEngine.Debug.Log("@LOG PsdkLocationManagerProvider.Show for location " + locationId);
			//if (!IsLocationReady(locationId))
			//{
			//	UnityEngine.Debug.Log("@LOG PSDKWrapper ShowLocation !IsLocationReady");
			//	PSDKWrapper.ToggleBlockTouchesEvent(obj: false);
			//	_showLocationPromise.Resolve(value: false);
			//}
			//else
			//{
			//	UnityEngine.Debug.Log("@LOG PsdkLocationManagerProvider : attempting to show " + locationId);
			//	locationService.ReportLocation(locationId);
			//	AddBreadCrumb("PSDK.LocationManager.ShowLocation: " + locationId);
			//	long num = locationService.Show(locationId);
			//	if (num == 0)
			//	{
			//		UnityEngine.Debug.Log("@LOG PsdkLocationManagerProvider - no source for location:" + locationId);
			//		PSDKWrapper.ToggleBlockTouchesEvent(obj: false);
			//		_showLocationPromise.Resolve(value: false);
			//	}
			//}
			return _showLocationPromise;
		}

		public void ShowInterstitialMoreApps()
		{
			ShowInterstitial("moreApps");
		}

		public void ShowInterstitialMoreAppsInThisSeries()
		{
			ShowInterstitial("moreAppsInThisSeries");
		}

		public void ShowInterstitialBackToMenu()
		{
			ShowInterstitial("backToMainMenu");
		}

		public void ShowInterstitialSceneTransition()
		{
			ShowInterstitial("sceneTransitions");
		}

		public void ShowInterstitialReplayLevel()
		{
			ShowInterstitial("replayLevel");
		}

		public void ShowInterstitialPreLevel()
		{
			ShowInterstitial("preLevel");
		}

		public void ShowInterstitialPostLevel()
		{
			ShowInterstitial("postLevel");
		}

		public void ShowInterstitialFail(Action onDone)
		{
			ShowInterstitial("fail", onDone);
		}

		public void ShowInterstitialRetry(Action onDone)
		{
			ShowInterstitial("retry", onDone);
		}

		private static void LogEvent(string eventName, IDictionary<string, object> eventParams, bool isPSDKEvent = false)
		{
			EventsManager.Instance.LogEvent(eventName, (Dictionary<string, object>)eventParams);
		}

		private Dictionary<string, object> AddGenericParams(IDictionary<string, object> eParams)
		{
			eParams.Add("userXP", StatsManager.runCount);
			eParams.Add("userScore", statsManager.score.value);
			eParams.Add("missionLevel", statsManager.goalCount.value);
			return eventParams;
		}

		public static void LogMissionStarted(string startType)
		{
			Dictionary<string, object> dictionary = Singleton<PSDKWrapper>.Instance.eventParams;
			dictionary.Clear();
			dictionary.Add("isTutorial", !TutorialManager.IsTutorialDone);
			dictionary.Add("missionName", "run");
			dictionary.Add("startType", startType);
			LogEvent("missionStarted", dictionary);
		}

		public static void LogMissionLevelUp(string startType)
		{
			Dictionary<string, object> dictionary = Singleton<PSDKWrapper>.Instance.eventParams;
			dictionary.Clear();
			dictionary.Add("startType", startType);
			LogEvent("missionLevelUp", dictionary);
		}

		public static void LogMissionFailed(string terminationReason, string startType)
		{
			Dictionary<string, object> dictionary = Singleton<PSDKWrapper>.Instance.eventParams;
			StatsManager statsManager = Singleton<PSDKWrapper>.Instance.statsManager;
			dictionary.Clear();
			string value = "offered";
			if (statsManager.reviveCount > 0)
			{
				value = "alreadyUsed";
			}
			else if ((int)statsManager.goalCount == 0)
			{
				value = "notOffered";
			}
			else if (!Singleton<PSDKWrapper>.Instance.IsRVReady())
			{
				value = "rvNotAvailable";
			}
			dictionary.Add("terminationReason", terminationReason);
			dictionary.Add("reviveOffered", value);
			dictionary.Add("missionName", "run");
			dictionary.Add("isTutorial", !TutorialManager.IsTutorialDone);
			dictionary.Add("startType", startType);
			LogEvent("missionFailed", dictionary);
		}

		public static void LogMissionAbandoned(string startType)
		{
			Dictionary<string, object> dictionary = Singleton<PSDKWrapper>.Instance.eventParams;
			dictionary.Clear();
			dictionary.Add("missionName", "run");
			dictionary.Add("isTutorial", !TutorialManager.IsTutorialDone);
			dictionary.Add("startType", startType);
			LogEvent("missionAbandoned", dictionary);
		}

		public static void LogUIInteraction(string uiName, string uiLocation)
		{
			Dictionary<string, object> dictionary = Singleton<PSDKWrapper>.Instance.eventParams;
			dictionary.Clear();
			dictionary.Add("UIName", uiName);
			dictionary.Add("UILocation", uiLocation);
			dictionary.Add("UIType", "Button");
			dictionary.Add("UIAction", "Click");
			LogEvent("uiInteraction", dictionary);
		}

		public static void LogRVImpression(string rvType = "revive", string rvOrigin = "run")
		{
			Dictionary<string, object> dictionary = Singleton<PSDKWrapper>.Instance.eventParams;
			dictionary.Clear();
			dictionary.Add("rvType", rvType);
			dictionary.Add("rvOrigin", rvOrigin);
			LogEvent("rvImpression", dictionary);
		}

		public static void LogRVClicked(string rvType = "revive", string rvOrigin = "run")
		{
			Dictionary<string, object> dictionary = Singleton<PSDKWrapper>.Instance.eventParams;
			dictionary.Clear();
			dictionary.Add("rvType", rvType);
			dictionary.Add("rvOrigin", rvOrigin);
			LogEvent("rvClicked", dictionary);
		}

		public static void LogTutorialStep(int stepId, string stepName, TutorialStepStage stepStage, bool isMandatory)
		{
			string tutorialStepName = string.Format("{0}_{1}", stepName, (stepStage != 0) ? "Complete" : "Start");
			EventsManager.Instance.LogEvent(tutorialStepName, new Dictionary<string, string> { { "stepId", stepId.ToString() } });
		}

		public static void LogLevelUp(int maxGoalCount)
		{
            EventsManager.Instance.LogEvent("bestLevel", new Dictionary<string, string> { { "maxGoalCount", maxGoalCount.ToString() } });
		}

		public static void LogMainScreen()
		{
			EventsManager.Instance.LogEvent("ReachedMainScreen");
		}

		public void ShowBanner()
		{
			bool flag = AdsManager.Instance.ShowBannerAds();
			UnityEngine.Debug.Log("@LOG PsdkWrapper ShowBanner result=" + flag);
		}

		public void HideBanner()
		{
			AdsManager.Instance.HideBannerAds();
		}

		public bool IsInterstitialReady()
        {
			return AdsManager.Instance.IsInterstitialAdsReady();
        }

		public bool ShowInterstitial(string locationId, Action onDone = null)
		{
			if (IsInterstitialReady())
			{
                onInterstitialDone = onDone;
                return AdsManager.Instance.ShowInterstitialAds(locationId: locationId);
			}
            else
            {
				onDone?.Invoke();
				return false;
            }
		}

		public bool IsRVReady()
		{
			return AdsManager.Instance.IsRewardAdsReady();
		}

		public bool ShowRV(Action onCompleted, Action onSkiped, Action onFailed)
		{
			if (!IsRVReady())
			{
				onFailed?.Invoke();
				return false;
			}
			else
			{
				onRewardCompleted = onCompleted;
				onRewardSkiped = onSkiped;
				onRewardFailed = onFailed;
				return AdsManager.Instance.ShowRewardAds();
			}
		}

		public bool IsSocialServiceAvailable()
		{
			return true;
		}

		public bool IsSocialConnected()
		{
			return SocialManager.Instance.IsLoginGameServiceSuccess;
		}

		public bool ShouldShowSocialConnectButton()
		{
			return !SocialManager.Instance.IsLoginGameServiceSuccess;
		}

		public bool ShouldShowSocialDisconnectButton()
		{
			return SocialManager.Instance.IsLoginGameServiceSuccess;
		}

		public void RequestSocialConnect()
		{
			if (CheckNetwork())
			{
				try
				{
					SocialManager.Instance.SigninSocialServices();
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}
		}

		public void RequestSocialDisconnect()
		{
			if (CheckNetwork())
			{
				try
				{
					SocialManager.Instance.LogOutServices();
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}
		}

		public void PsdkEvents_OnSocialAuthenticate(bool result)
		{
			UnityEngine.Debug.Log("PsdkWrapper PsdkEvents_OnSocialAuthenticate result=" + result);
			PSDKWrapper.onSocialAuthenticate(result);
		}

		public void PsdkEvents_OnSocialSignOut()
		{
			UnityEngine.Debug.Log("PsdkWrapper PsdkEvents_OnSocialSignOut");
			PSDKWrapper.onSocialSignOut();
		}

		public void RequestUpdateLeaderboard(int score)
		{
			if (NetworkCheck.HasInternetConnection)
			{
				UnityEngine.Debug.Log("@LOG PsdkWrapper RequestUpdateLeaderboard score=" + score);
				if (score > lastReportedLeaderboardScore && psdkData.LeaderboardId.Length > 0)
				{
					SocialManager.Instance.SetLeaderBoard(GPGSIds.leaderboard_high_score, score);
					TTPlayerPrefs.SetInt("LastReportedLeaderboardScore", lastReportedLeaderboardScore);
				}
			}
		}

		public void RequestOpenLeaderboard()
		{
			if (CheckNetwork())
			{
				UnityEngine.Debug.Log("@LOG PsdkWrapper RequestOpenLeaderboard");
				SocialManager.Instance.ShowLeaderBoard();
			}
		}

		public bool RequestReportAchivement(string achivementId)
		{
			SocialManager.Instance.UnlockAchievement(achivementId);
			return true;
		}

		public void RequestOpenAchivements()
		{
			UnityEngine.Debug.Log("@LOG PsdkWrapper RequestOpenLeaderboard");
			SocialManager.Instance.ShowAchievementsUI();
		}

		public void RequestShare()
		{
			SocialManager.Instance.ShareNative();
		}

		private bool CheckNetwork()
		{
			if (!NetworkCheck.HasInternetConnection)
			{
				PSDKWrapper.NoInternetEvent();
				return false;
			}
			return true;
		}

		public IPromise<bool> RequestMoreApps()
		{
			if (CheckNetwork())
			{
				return ShowLocation("moreApps");
			}
			Promise<bool> promise = new Promise<bool>();
			promise.Resolve(value: false);
			return promise;
		}

		public bool IsMoreAppsReady()
		{
			return true;
		}

		public void RequestRateUs()
		{
			SocialManager.Instance.OpenRateGame();
		}

		public void RequestRestorePurchases()
		{
			if (CheckNetwork())
			{
				IAPManager.Instance.RestorePurchases();
			}
		}

		[Conditional("UNITY_EDITOR")]
		private void ForceRestore()
		{
			PsdkEvents_BillingPurchaseRestored(restoreResult: true);
		}

		public bool IsRetoreInProgress()
		{
			//return billingService != null && billingService.IsRestoreInProgress();
			return true;
		}

		public void PsdkEvents_BillingPurchaseRestored(bool restoreResult)
		{
			PSDKWrapper.NotifyOnBillingPurchaseRestored(restoreResult);
		}

		public bool IsPurchased(string itemId)
		{
			//return billingService != null && billingService.IsPurchased(itemId);
			return false;
		}

		public bool PurchaseItem(string itemId)
		{
			if (!CheckNetwork())
			{
				return false;
			}
            var status = IAPManager.Instance.BuyProductByID(itemId);
            if (status == EStatusBuyIAP.InProcess || status == EStatusBuyIAP.Success)
			{
				return true;
			}
			return false;
		}

		public bool IsPurchaseInProgress()
		{
			return true;
		}

		public void PsdkEvents_NotifyOnBillingPurchaseRestored(bool success)
		{
		}

		public void ClearTransactions()
		{

		}

		public bool IsConsumable(string itemId)
		{
			return IAPManager.Instance.IsConsumableById(itemId);
		}

		public string GetLocalizedPriceString(string itemId)
		{
			return IAPManager.Instance.GetProductPriceStringById(itemId);
		}

		public string GetISOCurrencySymbol(string itemId)
		{
			return IAPManager.Instance.GetIsoCurrencyCodeById(itemId);
		}

		public decimal GetPriceInLocalCurrency(string itemId)
		{
			return (decimal)IAPManager.Instance.GetProductPriceById(itemId);
		}

		public bool IsNoAdsItem(string itemId)
		{
			return IAPManager.Instance.IsRemoveAdsById(itemId);
		}

		static PSDKWrapper()
		{
			PSDKWrapper.PSDKReadyEvent = delegate
			{
			};
			PSDKWrapper.PauseGameMusicEvent = delegate
			{
			};
			PSDKWrapper.RVClosedEvent = delegate
			{
			};
			PSDKWrapper.ReturnFromBackgroundNewSession = delegate
			{
			};
			PSDKWrapper.ReturnFromBackgroundRestartApp = delegate
			{
			};
			PSDKWrapper.NotifyOnBillingPurchaseRestored = delegate
			{
			};
			PSDKWrapper.onSocialAuthenticate = delegate
			{
			};
			PSDKWrapper.onSocialSignOut = delegate
			{
			};
			PSDKWrapper.ToggleBlockTouchesEvent = delegate
			{
			};
			PSDKWrapper.NoInternetEvent = delegate
			{
			};
			PSDKWrapper.OnLocationLoaded = delegate
			{
			};
			isPsdkReady = false;
		}
	}
}
