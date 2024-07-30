using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GamemakerSuperCasual
{
	public class CountDownView : MonoBehaviour
	{
		public UnityEvent onTimeout;

		public Image fill;

		public Text txt;

		public int countDownTime = 10;

		private int _currentCount;

		public void Toggle(bool toggle)
		{
			if (toggle)
			{
				StartCoroutine(CountdownCoro());
			}
			else
			{
				base.gameObject.SetActive(value: false);
			}
		}

		public void Stop()
		{
			LeanTween.cancel(base.gameObject);
			StopAllCoroutines();
		}

		private IEnumerator CountdownCoro()
		{
			_currentCount = countDownTime;
			LeanTween.value(base.gameObject, UpdateFill, 1f, 0f, countDownTime);
			WaitForSeconds wait = new WaitForSeconds(1f);
			while (_currentCount >= 0)
			{
				txt.text = _currentCount.ToString();
				yield return wait;
				_currentCount--;
			}
			onTimeout.Invoke();
		}

		private void UpdateFill(float value)
		{
			fill.fillAmount = value;
		}

		private void OnDisable()
		{
			Stop();
		}
	}
}
