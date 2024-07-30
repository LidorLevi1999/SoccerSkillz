using System;
using UnityEngine;
using UnityEngine.UI;

public class ScoreText : MonoBehaviour
{
	private Text uiText;

	private CanvasGroup cg;

	public Score score;

	private void Awake()
	{
		uiText = GetComponent<Text>();
		cg = GetComponent<CanvasGroup>();
	}

	private void Start()
	{
		RefreshScoreText();
	}

	private void RefreshScoreText()
	{
		uiText.text = score.ToString();
	}

	public void Toggle(bool toggle, bool animated = true)
	{
		if (toggle)
		{
			base.gameObject.SetActive(value: true);
			RefreshScoreText();
			if (animated)
			{
				LeanTween.cancel(base.gameObject);
				cg.alpha = 0f;
				LeanTween.alphaCanvas(cg, 1f, 0.2f).setEaseInOutSine();
			}
		}
		else if (animated)
		{
			LeanTween.cancel(base.gameObject);
			cg.alpha = 1f;
			LeanTween.alphaCanvas(cg, 0f, 1.2f).setEaseInOutSine().setOnComplete((Action)delegate
			{
				base.gameObject.SetActive(value: false);
			});
		}
	}
}
