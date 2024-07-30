using System;
using System.Linq;
using System.Reflection;
using GamemakerSuperCasual;
using UnityEngine;
using UnityEngine.UI;

public class DebugInputField : MonoBehaviour
{
	public ScriptableObject data;

	public string property;

	public Text txt;

	public InputField input;

	private FieldInfo _info;

	private void Start()
	{
		if (!string.IsNullOrEmpty(property))
		{
			FieldInfo[] fields = data.GetType().GetFields();
			_info = fields.FirstOrDefault((FieldInfo x) => x.Name == property);
			input.onEndEdit.AddListener(OnEndEditInput);
			Refresh();
		}
	}

	private void OnEndEditInput(string s)
	{
		if (string.IsNullOrEmpty(s))
		{
			return;
		}
		Type fieldType = _info.FieldType;
		if (fieldType == typeof(int))
		{
			_info.SetValue(data, int.Parse(s));
		}
		else if (fieldType == typeof(float))
		{
			_info.SetValue(data, float.Parse(s));
		}
		else if (fieldType == typeof(bool))
		{
			if (s == "1")
			{
				_info.SetValue(data, true);
			}
			else if (s == "0")
			{
				_info.SetValue(data, false);
			}
			else
			{
				_info.SetValue(data, bool.Parse(s));
			}
		}
		else if (fieldType == typeof(string))
		{
			_info.SetValue(data, s);
		}
	}

	private void OnEnable()
	{
		Refresh();
	}

	private void Refresh()
	{
		if (_info != null)
		{
			object value = _info.GetValue(data);
			Type fieldType = _info.FieldType;
			if (fieldType == typeof(bool))
			{
				bool flag = (bool)value;
				input.text = ((!flag) ? "0" : "1");
			}
			else
			{
				input.text = value.ToString();
			}
			txt.text = _info.Name.SplitCamelCase();
		}
	}
}
