using Soccerpass;
using System.Linq;
using GamemakerSuperCasual.Runtime.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace GamemakerSuperCasual
{
	public class RegisterButtonClicks : MonoBehaviour
	{
		private void Awake()
		{
			Button[] componentsInChildren = GetComponentsInChildren<Button>(includeInactive: true);
			componentsInChildren.ToList().ForEach(RegisterClick);
		}

		private void RegisterClick(Button b)
		{
			b.onClick.AddListener(OnClick);
			if (GetComponent<UIManager>() != null)
			{
				ButtonClickAnalyticsReport buttonClickAnalyticsReport = b.AddComponent<ButtonClickAnalyticsReport>();
				buttonClickAnalyticsReport.SetUIManagerRef(GetComponent<UIManager>());
			}
		}

		private void OnClick()
		{
			this.PlaySFX(SoundId.click);
		}
	}
}
