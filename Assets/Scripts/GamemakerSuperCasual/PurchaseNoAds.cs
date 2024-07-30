using com.F4A.MobileThird;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GamemakerSuperCasual
{
	public class PurchaseNoAds : MonoBehaviour
	{
		public Text price;

		private Button _btn;

		private CanvasGroup cg;

		private void Awake()
		{
			_btn = GetComponent<Button>();
			_btn.interactable = false;
			cg = GetComponent<CanvasGroup>();
			if (cg == null)
			{
				cg = base.gameObject.AddComponent<CanvasGroup>();
			}
			cg.alpha = 0f;

            IAPManager.OnBuyPurchaseSuccessed += IAPManager_OnBuyPurchaseSuccessed;
            IAPManager.OnBuyPurchaseFailed += IAPManager_OnBuyPurchaseFailed;
		}

        private void Start()
		{
			_btn.onClick.AddListener(OnClick);
		}

		private void OnDestroy()
		{
            IAPManager.OnBuyPurchaseSuccessed -= IAPManager_OnBuyPurchaseSuccessed;
            IAPManager.OnBuyPurchaseFailed -= IAPManager_OnBuyPurchaseFailed;
        }

        private void IAPManager_OnBuyPurchaseFailed(string id, string error)
        {
            RefreshView();
        }

        private void IAPManager_OnBuyPurchaseSuccessed(string id, bool modeTest)
        {
			var pr = IAPManager.Instance.GetProductInfoById(id);
			if(pr != null && pr.IsTypeRemoveAds())
            {
                Singleton<PSDKWrapper>.Instance.HideBanner();
                RefreshView();
            }
        }

		private void OnEnable()
		{
			if (!RefreshView() && base.gameObject.activeInHierarchy)
			{
				StartCoroutine(CheckAvailableCoro());
			}
		}

		private bool RefreshView()
		{
			if (!IAPManager.Instance.EnableUnityIAP())
			{
				cg.alpha = 0f;
				_btn.interactable = false;
				price.text = "---";
				return false;
			}
			if (AdsManager.Instance.IsRemoveAds())
			{
				base.gameObject.SetActive(value: false);
				return true;
			}
			var pr = IAPManager.Instance.GetProductRemoveAds();
			if(pr != null)
            {
                cg.alpha = 1f;
                _btn.interactable = true;
				price.text = Singleton<PSDKWrapper>.Instance.GetLocalizedPriceString(pr.Id);
            }
			return false;
		}

		private IEnumerator CheckAvailableCoro()
		{
			while (!_btn.interactable)
			{
				RefreshView();
				yield return new WaitForSeconds(0.5f);
			}
		}

		private void OnDisable()
		{
			StopAllCoroutines();
		}

		private void OnClick()
		{
			_btn.interactable = false;
            var pr = IAPManager.Instance.GetProductRemoveAds();
			if (pr != null) Singleton<PSDKWrapper>.Instance.PurchaseItem(pr.Id);
		}
	}
}
