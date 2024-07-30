using UnityEngine;

public class ScaleUpDown : MonoBehaviour
{
	public float scaleAmount = 1.3f;

	public float animTime = 0.7f;

	private void Start()
	{
		ToggleAnimation(toggle: true);
	}

	public void ToggleAnimation(bool toggle)
	{
		if (!toggle)
		{
			LeanTween.cancel(base.gameObject);
		}
		else
		{
			LeanTween.scale(base.gameObject, Vector3.one * scaleAmount, animTime).setEaseInOutSine().setLoopPingPong(-1);
		}
	}
}
