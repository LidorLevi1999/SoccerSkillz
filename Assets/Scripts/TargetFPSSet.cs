using UnityEngine;

public class TargetFPSSet : MonoBehaviour
{
	public int value = 60;

	private void Start()
	{
		Application.targetFrameRate = value;
	}
}
