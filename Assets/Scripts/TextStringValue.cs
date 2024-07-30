using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TextStringValue : MonoBehaviour
{
	public StringVar var;

	public string baseText = string.Empty;

	private Text uiText;

	private string value;

	private void Awake()
	{
		uiText = GetComponent<Text>();
	}

	private void Update()
	{
		if (value != var.value)
		{
			uiText.text = baseText + var.value;
			value = var.value;
		}
	}
}
