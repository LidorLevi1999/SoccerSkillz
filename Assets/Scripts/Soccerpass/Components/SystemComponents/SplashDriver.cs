using UnityEngine;
using UnityEngine.UI;

namespace Soccerpass.Components.SystemComponents
{
	public class SplashDriver : MonoBehaviour
	{
		public Transform ResizeTarget;

		private void Awake()
		{
			AdjustSplash();
		}

		private void AdjustSplash()
		{
			Vector2 referenceResolution = GetComponent<CanvasScaler>().referenceResolution;
			float num = referenceResolution.x / referenceResolution.y;
			Vector2 vector = new Vector2(Screen.width, Screen.height);
			float num2 = vector.x / vector.y;
			float num3 = num2 / num;
			if (!(num3 < 1f))
			{
				ResizeTarget.localScale *= num3;
			}
		}
	}
}
