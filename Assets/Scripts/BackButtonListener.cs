using System;
using UnityEngine;

public class BackButtonListener : MonoBehaviour
{
	public static event Action BackButtonEvent;

	private void Update()
	{
		if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
		{
			BackButtonListener.BackButtonEvent();
		}
	}

	static BackButtonListener()
	{
		BackButtonListener.BackButtonEvent = delegate
		{
		};
	}
}
