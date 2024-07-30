using GamemakerSuperCasual;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ShareButton : MonoBehaviour
{
	private void Awake()
	{
		GetComponent<Button>().onClick.AddListener(OnClick);
	}

	private void OnClick()
	{
		Singleton<PSDKWrapper>.Instance.RequestShare();
	}
}
