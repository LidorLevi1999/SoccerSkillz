using UnityEngine;
using UnityEngine.UI;

namespace GamemakerSuperCasual
{
	[RequireComponent(typeof(Text))]
	public class VersionText : MonoBehaviour
	{
		public StringVar buildData;

		private Text uiText;

		private string value;

		private void Awake()
		{
			uiText = GetComponent<Text>();
			SetText();
		}

		private void SetText()
		{
			uiText.text = "v." + Application.version + "." + buildData.value;
		}
	}
}
