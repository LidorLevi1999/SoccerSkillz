using com.F4A.MobileThird;
using Gamemaker;
using UnityEngine;
using UnityEngine.UI;

public class BillingDemo : MonoBehaviour
{
	public void onClick(int mNum)
	{
		string text = GetComponentInChildren<InputField>().text;
		switch (mNum)
		{
			case 0:
				break;
			case 1:
				break;
			case 2:
				break;
			case 3:
				break;
			case 4:
				IAPManager.Instance.RestorePurchases();
				break;
			case 5:
				IAPManager.Instance.BuyProductByID(text);
				break;
			case 6:
				break;
			case 7:
				break;
			case 8:
				break;
			case 9:
				break;
			case 10:
				break;
			case 11:
				//PSDKMgr.Instance.GetBilling().ClearTransactions();
				break;
			case 12:
				break;
		}
	}

	private void onBillingPurchased(PurchaseIAPResult result)
	{
		UnityEngine.Debug.Log("PurchaseResult - " + result.purchasedItem.id + " - " + result.result.ToString());
	}
}
