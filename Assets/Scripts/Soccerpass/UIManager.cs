using System;
using System.Collections.Generic;
using System.Linq;
using GamemakerSuperCasual;
using UnityEngine;

namespace Soccerpass
{
	public class UIManager : MonoBehaviour
	{
		[Serializable]
		public class UIState
		{
			public HUDType type;

			public List<Transform> activeTransforms;

			public UIState()
			{
			}

			public UIState(HUDType type)
			{
				this.type = type;
			}

			public void Toggle(bool toggle)
			{
				foreach (Transform activeTransform in activeTransforms)
				{
					ITogglable togglable = (ITogglable)activeTransform.GetComponent(typeof(ITogglable));
					if (togglable != null)
					{
						if (toggle)
						{
							togglable.Show();
						}
						else
						{
							togglable.Hide();
						}
					}
					else
					{
						activeTransform.gameObject.SetActive(toggle);
					}
				}
			}
		}

		public HUDState hudState;

		private HUDType _lastState;

		public List<UIState> list;

		private Dictionary<HUDType, UIState> dict;

		public bool createListOnValidate;

		public GameObject touchBlockerCanvas;

		public GameObject noInternetCanvas;

		private void OnValidate()
		{
			if (!createListOnValidate)
			{
				return;
			}
			createListOnValidate = false;
			list.Clear();
			HUDType[] array = (HUDType[])Enum.GetValues(typeof(HUDType));
			HUDType[] array2 = array;
			foreach (HUDType hUDType in array2)
			{
				if (hUDType != 0)
				{
					list.Add(new UIState(hUDType));
				}
			}
		}

		private void Awake()
		{
			dict = list.ToDictionary((UIState x) => x.type);
			PSDKWrapper.ToggleBlockTouchesEvent += PSDKWrapper_ToggleBlockTouchesEvent;
			PSDKWrapper.NoInternetEvent += PSDKWrapper_NoInternetEvent;
			touchBlockerCanvas.SetActive(value: false);
		}

		private void PSDKWrapper_NoInternetEvent()
		{
			noInternetCanvas.SetActive(value: true);
		}

		private void PSDKWrapper_ToggleBlockTouchesEvent(bool toggle)
		{
			touchBlockerCanvas.SetActive(toggle);
		}

		private void OnDestroy()
		{
			PSDKWrapper.ToggleBlockTouchesEvent -= PSDKWrapper_ToggleBlockTouchesEvent;
			PSDKWrapper.NoInternetEvent -= PSDKWrapper_NoInternetEvent;
		}

		private void Update()
		{
			if (_lastState != hudState.value)
			{
				SetHUD();
			}
		}

		private void SetHUD()
		{
			if (_lastState != 0 || (HUDType)hudState == HUDType.None)
			{
				HideCurrentHUD();
			}
			if ((HUDType)hudState != 0)
			{
				UIState uIState = dict[hudState.value];
				uIState.Toggle(toggle: true);
			}
			_lastState = hudState.value;
		}

		public void HideCurrentHUD()
		{
			if (_lastState != 0)
			{
				UIState uIState = dict[_lastState];
				uIState.Toggle(toggle: false);
			}
		}
	}
}
