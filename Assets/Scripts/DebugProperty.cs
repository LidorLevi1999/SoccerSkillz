using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class DebugProperty : MonoBehaviour
{
	public ScriptableObject stateData;

	public string property;

	public string baseText;

	private Text txt;

	private FieldInfo _info;

	private void Start()
	{
		if (!string.IsNullOrEmpty(property))
		{
			txt = GetComponent<Text>();
			FieldInfo[] fields = stateData.GetType().GetFields();
			_info = fields.FirstOrDefault((FieldInfo x) => x.Name == property);
		}
	}

	private void Update()
	{
		if (_info != null)
		{
			txt.text = baseText + _info.GetValue(stateData);
		}
	}
}
