using com.F4A.MobileThird;
using UnityEngine;
using UnityEngine.UI;

public class ShareDemo : MonoBehaviour
{
	public Transform subjectInputField;

	public Transform bodyInputField;

	public Transform appStoreUrlInputField;

	public Transform headLineInputField;

	public void onClick()
	{
		string title = subjectInputField.GetComponent<InputField>().text;
		string msg = bodyInputField.GetComponent<InputField>().text;
		SocialManager.Instance.ShareNative("", msg, title, "", false);
	}
}
