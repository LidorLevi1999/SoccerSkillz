using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GamemakerSuperCasual;
using UnityEngine;

public class DebugValuesPanel : MonoBehaviour
{
	public ScriptableObject data;

	public GameObject valueInputPrefab;

	public GameObject valueBoolPrefab;

	public Transform valuesParent;

	private void Awake()
	{
		List<FieldInfo> list = data.GetType().GetFields().ToList();
		list.ForEach(delegate(FieldInfo x)
		{
			DebugHideAttribute debugHideAttribute = Attribute.GetCustomAttribute(x, typeof(DebugHideAttribute)) as DebugHideAttribute;
			if (debugHideAttribute == null)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(valueInputPrefab);
				gameObject.transform.SetParent(valuesParent);
				gameObject.transform.localScale = Vector3.one;
				DebugInputField component = gameObject.GetComponent<DebugInputField>();
				if (component != null)
				{
					component.data = data;
					component.property = x.Name;
				}
			}
		});
	}

	private void Start()
	{
		base.gameObject.SetActive(value: false);
	}
}
