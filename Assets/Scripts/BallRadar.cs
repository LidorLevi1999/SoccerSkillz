using System;
using UnityEngine;

public class BallRadar : MonoBehaviour
{
	public bool isActive;

	public GameObject visual;

	private Collider _collider;

	public AnimationCurve scaleCurve;

	public AnimationCurve fadeCurve;

	public float scaleTime = 0.5f;

	public float fadeTime = 0.5f;

	private Vector3 _originalScale;

	public event Action<GameObject> TriggerEnterE = delegate
	{
	};

	public event Action<GameObject> TriggerExitE = delegate
	{
	};

	private void Awake()
	{
		_collider = GetComponent<Collider>();
		_originalScale = base.transform.localScale;
	}

	private void SetAlpha(float val)
	{
		SpriteRenderer component = visual.GetComponent<SpriteRenderer>();
		Color color = component.color;
		color.a = val;
		component.color = color;
	}

	public void Toggle(bool active, bool animated = true)
	{
		if (_collider != null)
		{
			_collider.enabled = active;
		}
		isActive = active;
		if (!animated)
		{
			visual.SetActive(isActive);
		}
		else if (isActive)
		{
			visual.SetActive(value: true);
			Vector3 originalScale = _originalScale;
			Vector3 localScale = new Vector3(0f, _originalScale.y, 0f);
			base.transform.localScale = localScale;
			LeanTween.scale(base.gameObject, originalScale, scaleTime).setEase(scaleCurve);
			SetAlpha(0f);
			LeanTween.alpha(visual, 1f, fadeTime).setEase(fadeCurve);
		}
		else
		{
			LeanTween.scale(to: new Vector3(0f, _originalScale.y, 0f), gameObject: base.gameObject, time: scaleTime).setEase(scaleCurve);
			SetAlpha(1f);
			LeanTween.alpha(visual, 0f, 0.15f).setEase(fadeCurve).setOnComplete((Action)delegate
			{
				visual.SetActive(value: false);
			});
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (isActive)
		{
			this.TriggerEnterE(other.gameObject);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (isActive)
		{
			this.TriggerExitE(other.gameObject);
		}
	}
}
