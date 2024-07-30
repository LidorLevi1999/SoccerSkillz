using UnityEngine;
using UnityEngine.UI;

namespace GamemakerSuperCasual
{
	public class PrivacyPolicyButton : MonoBehaviour
	{
		private void Start()
		{
			GetComponent<Button>().onClick.AddListener(OnClick);
		}

		private void OnClick()
		{
			Application.OpenURL("");
		}
	}
}
