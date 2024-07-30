using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TextValue : MonoBehaviour
{
	private Text txt;

	public string baseText = string.Empty;

	public IntVar var;

	private int _lastValue = int.MaxValue;

	private void Awake()
	{
		txt = GetComponent<Text>();
		_lastValue = int.MaxValue;
	}

	private void Update()
	{
		if (_lastValue != var.value)
		{
			_lastValue = var.value;
			if (var is IntRangeVar)
			{
				txt.text = $"{baseText}{var.value}/{((IntRangeVar)var).max}";
			}
			else
			{
				txt.text = $"{baseText}{var.value}";
			}
		}
	}
}
