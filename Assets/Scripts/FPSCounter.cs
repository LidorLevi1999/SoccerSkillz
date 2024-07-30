using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
	public Text Display;

	public float DeltaTime
	{
		get;
		private set;
	}

	private IEnumerator DisplayFpsCoro()
	{
		while (true)
		{
			int fps = (int)(1f / DeltaTime);
			Display.text = $"{fps} fps";
			yield return new WaitForSeconds(0.5f);
		}
	}

	private void Start()
	{
		StartCoroutine(DisplayFpsCoro());
	}

	public void Update()
	{
		DeltaTime += (Time.unscaledDeltaTime - DeltaTime) * 0.1f;
	}
}
