using UnityEngine;

public class ToggleDebugValues : MonoBehaviour
{
	public GameObject debugValues;

	public void OnClick()
	{
		debugValues.SetActive(!debugValues.activeInHierarchy);
		Time.timeScale = ((!debugValues.activeInHierarchy) ? 1 : 0);
	}
}
