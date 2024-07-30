using UnityEngine;
using UnityEngine.UI;

public class BuyForCurrency : MonoBehaviour
{
	public GameEvent NotEnoughCurrencyEvent;

	public IntVar currency;

	public IntVar product;

	public int price;

	public int reward;

	public float priceIncrease = 1f;

	public Text uiTextPrice;

	private void Start()
	{
		UpdatePriceText();
	}

	public void OnClick()
	{
		if ((int)currency - price > 0)
		{
			currency.value -= price;
			product.value += reward;
			price = (int)((float)price * priceIncrease);
			UpdatePriceText();
		}
		else
		{
			NotEnoughCurrencyEvent.Raise();
		}
	}

	private void UpdatePriceText()
	{
		uiTextPrice.text = "x" + price.ToString();
	}
}
