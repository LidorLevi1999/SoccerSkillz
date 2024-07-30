using UnityEngine;

public class Spinner : MonoBehaviour
{
	public Vector3 EulersPerSecond;

	private void Update()
	{
		base.transform.Rotate(EulersPerSecond * Time.deltaTime);
	}
}
