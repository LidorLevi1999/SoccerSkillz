using UnityEngine;

public class TweenScalePingPong : MonoBehaviour
{
	public void Scale(float amount)
	{
		if (!LeanTween.isTweening(base.gameObject))
		{
			LeanTween.scale(base.gameObject, Vector3.one * amount, 0.2f).setEaseInOutSine().setLoopPingPong(1);
		}
	}
}
