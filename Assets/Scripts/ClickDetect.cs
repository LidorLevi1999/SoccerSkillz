using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ClickDetect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler
{
	public UnityEvent PointerDown;

	public UnityEvent PointerUp;

	public void OnPointerDown(PointerEventData eventData)
	{
		if (PointerDown != null)
		{
			PointerDown.Invoke();
		}
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (PointerUp != null)
		{
			PointerUp.Invoke();
		}
	}
}
