using Soccerpass;
using GamemakerSuperCasual;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonClickAnalyticsReport : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
{
	private HUDType _hudType;

	private UIManager _uiManager;

	private void Start()
	{
		GetComponent<Button>().onClick.AddListener(ReportClick);
	}

	private void ReportClick()
	{
	}

	private void OnEnable()
	{
		//_hudType = _uiManager.hudState.value;
	}

	public void SetUIManagerRef(UIManager uiManager)
	{
		_uiManager = uiManager;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		PSDKWrapper.LogUIInteraction(base.name, _hudType.ToString());
	}
}
