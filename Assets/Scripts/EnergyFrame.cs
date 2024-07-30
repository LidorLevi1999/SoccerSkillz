using System;
using UnityEngine;
using UnityEngine.UI;

public class EnergyFrame : MonoBehaviour
{
	public enum Mode
	{
		Low,
		No
	}

	public Image frameImage;

	public Color lowEnergy;

	public Color noEnergy;

	public void Show(Mode mode)
	{
		bool flag = false;
		if (!base.gameObject.activeInHierarchy)
		{
			flag = true;
		}
		base.gameObject.SetActive(value: true);
		Color color = (mode != 0) ? noEnergy : lowEnergy;
		if (flag)
		{
			Color color2 = color;
			color2.a = 0f;
			frameImage.color = color2;
			LeanTween.alpha(frameImage.rectTransform, 1f, 0.15f).setEaseInOutSine();
		}
		else
		{
			LeanTween.color(frameImage.rectTransform, color, 0.3f).setEaseInOutSine();
		}
	}

	public void Hide()
	{
		if (base.gameObject.activeInHierarchy)
		{
			LeanTween.alpha(frameImage.rectTransform, 0f, 0.3f).setEaseInOutSine().setOnComplete((Action)delegate
			{
				base.gameObject.SetActive(value: false);
			});
			return;
		}
		frameImage.color = Color.white;
		base.gameObject.SetActive(value: false);
	}
}
