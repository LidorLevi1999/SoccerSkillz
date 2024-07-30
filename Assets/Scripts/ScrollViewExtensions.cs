using UnityEngine;
using UnityEngine.UI;

public static class ScrollViewExtensions
{
	public static void ScrollToTop(this ScrollRect scrollRect)
	{
		scrollRect.normalizedPosition = new Vector2(0f, 1f);
	}

	public static void ScrollToBottom(this ScrollRect scrollRect)
	{
		scrollRect.normalizedPosition = new Vector2(0f, 0f);
	}

	public static void ScrollByAmount(this ScrollRect scrollRect, float amount)
	{
		Vector2 sizeDelta = scrollRect.content.sizeDelta;
		float y = sizeDelta.y;
		float num = amount / y;
		Vector2 normalizedPosition = scrollRect.normalizedPosition;
		normalizedPosition.y += num;
		scrollRect.normalizedPosition = normalizedPosition;
	}

	public static void ScrollTo(this ScrollRect scrollRect, RectTransform rt)
	{
		Vector3 localPosition = scrollRect.content.localPosition;
		Vector2 offsetMin = rt.offsetMin;
		localPosition.y = 0f - offsetMin.y;
		scrollRect.content.localPosition = localPosition;
	}
}
