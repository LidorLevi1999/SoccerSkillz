using UnityEngine;
using UnityEngine.UI;

public class TimeScaleText : MonoBehaviour
{
	private float lastScale;

	private Text uiText;

	private void Start()
	{
		uiText = GetComponent<Text>();
	}

	private void Update()
	{
		if (lastScale != Time.timeScale)
		{
			lastScale = Time.timeScale;
			uiText.text = Time.timeScale.ToString();
		}
	}
}
