using UnityEngine;
using UnityEngine.UI;

public class UserSettingsGroup : MonoBehaviour
{
	public Button buttonOn;

	public Button buttonOff;

	public SettingType type;

	private void Start()
	{
		buttonOn.onClick.AddListener(OnClickOn);
		buttonOff.onClick.AddListener(OnClickOff);
	}

	private void OnEnable()
	{
		Toggle(UserSettings.GetSettingsValue(type));
	}

	private void OnClickOn()
	{
		UserSettings.SetSettingsValue(type, val: true);
		Toggle(toggle: true);
	}

	private void Toggle(bool toggle)
	{
		buttonOff.gameObject.SetActive(toggle);
		buttonOn.gameObject.SetActive(!toggle);
	}

	private void OnClickOff()
	{
		UserSettings.SetSettingsValue(type, val: false);
		Toggle(toggle: false);
	}
}
