using Soccerpass;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ToggleGameElementButton : MonoBehaviour
{
	public enum Type
	{
		Crowd,
		Glow,
		Shadows
	}

	public QualityData data;

	public bool toggled = true;

	public Type qualityType;

	private Button _btn;

	private int _baseQualityLevel;

	private void Awake()
	{
		_btn = GetComponent<Button>();
		_btn.onClick.AddListener(Toggle);
		switch (qualityType)
		{
		case Type.Crowd:
			toggled = data.crowdEnabled;
			break;
		case Type.Glow:
			toggled = data.glowEnabled;
			break;
		case Type.Shadows:
			toggled = data.shadowsEnabled;
			break;
		}
		UpdateColor();
		_baseQualityLevel = QualityData.qualityLevel;
	}

	private void UpdateColor()
	{
		ColorBlock colors = _btn.colors;
		colors.normalColor = ((!toggled) ? Color.white : Color.yellow);
		_btn.colors = colors;
	}

	private void Toggle()
	{
		toggled = !toggled;
		switch (qualityType)
		{
		case Type.Crowd:
			data.crowdEnabled = toggled;
			break;
		case Type.Glow:
			data.glowEnabled = toggled;
			break;
		case Type.Shadows:
		{
			data.shadowsEnabled = toggled;
			int qualityLevel = QualitySettings.GetQualityLevel();
			if (toggled)
			{
				if (_baseQualityLevel > qualityLevel)
				{
					QualitySettings.SetQualityLevel(_baseQualityLevel, applyExpensiveChanges: false);
				}
			}
			else if (_baseQualityLevel == qualityLevel)
			{
				QualitySettings.SetQualityLevel(_baseQualityLevel - 1, applyExpensiveChanges: false);
			}
			break;
		}
		}
		UpdateColor();
	}
}
