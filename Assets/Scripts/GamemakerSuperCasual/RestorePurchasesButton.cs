using UnityEngine;
using UnityEngine.UI;

namespace GamemakerSuperCasual
{
	public class RestorePurchasesButton : MonoBehaviour
	{
		private Button _btn;

		private void Awake()
		{
			_btn = GetComponent<Button>();
			base.gameObject.SetActive(value: false);
		}

		private void Start()
		{
			GetComponent<Button>().onClick.AddListener(OnClick);
		}

		private void OnEnable()
		{
			PSDKWrapper.NotifyOnBillingPurchaseRestored += PSDKWrapper_NotifyOnBillingPurchaseRestored;
		}

		private void PSDKWrapper_NotifyOnBillingPurchaseRestored(bool result)
		{
			if (result)
			{
				_btn.interactable = false;
			}
		}

		private void OnDisable()
		{
			PSDKWrapper.NotifyOnBillingPurchaseRestored -= PSDKWrapper_NotifyOnBillingPurchaseRestored;
		}

		private void OnClick()
		{
			Singleton<PSDKWrapper>.Instance.RequestRestorePurchases();
		}
	}
}
