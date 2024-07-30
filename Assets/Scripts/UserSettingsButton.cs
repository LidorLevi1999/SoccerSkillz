using UnityEngine;

public class UserSettingsButton : MonoBehaviour
{
	public GameObject on;

	public GameObject off;

	public SettingType type;

	private void OnEnable()
	{
		Toggle(UserSettings.GetSettingsValue(type));
	}

	public void OnClick()
	{
		bool settingsValue = UserSettings.GetSettingsValue(type);
		UserSettings.SetSettingsValue(type, !settingsValue);
		Toggle(!settingsValue);
	}

	private void Toggle(bool toggle)
	{
		on.gameObject.SetActive(toggle);
		off.gameObject.SetActive(!toggle);
	}
}
