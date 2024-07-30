namespace Gamemaker
{
	public class PurchaseIAPResult
	{
		public InAppPurchasableItem purchasedItem;

		public PurchaseIAPResultCode result;

		public BillerErrors error;

		public PurchaseIAPResult(InAppPurchasableItem purchasedItem, PurchaseIAPResultCode result = PurchaseIAPResultCode.Success, BillerErrors error = BillerErrors.NO_ERROR)
		{
			this.purchasedItem = purchasedItem;
			this.result = result;
			this.error = error;
		}
	}
}
