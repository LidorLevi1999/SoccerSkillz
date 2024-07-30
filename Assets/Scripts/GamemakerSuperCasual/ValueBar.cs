using System;
using UnityEngine;
using UnityEngine.UI;

namespace GamemakerSuperCasual
{
	public class ValueBar : MonoBehaviour
	{
		public IntRangeVar rangedVar;

		public Image fillImage;

		public Image bgImage;

		public Color blinkColor = Color.yellow;

		private Color originalBGColor;

		public float animationTime = 0.1f;

		private void Start()
		{
			originalBGColor = bgImage.color;
		}

		private void Update()
		{
			fillImage.fillAmount = Mathf.Lerp(fillImage.fillAmount, rangedVar.Normalized, animationTime * Time.deltaTime);
		}

		internal void Blink()
		{
			LeanTween.cancel(base.gameObject);
			bgImage.color = blinkColor;
			LeanTween.delayedCall(base.gameObject, 0.1f, (Action)delegate
			{
				bgImage.color = originalBGColor;
			});
		}
	}
}
