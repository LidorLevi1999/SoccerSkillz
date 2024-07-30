using System;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	private static T _instance;

	private static object _lock = new object();

	protected static string pathToResource;

	private static bool applicationIsQuitting = false;

	public static bool IsLoaded => (UnityEngine.Object)_instance != (UnityEngine.Object)null;

	public static T Instance
	{
		get
		{
			if (applicationIsQuitting)
			{
				return (T)null;
			}
			lock (_lock)
			{
				if ((UnityEngine.Object)_instance == (UnityEngine.Object)null)
				{
					_instance = (T)UnityEngine.Object.FindObjectOfType(typeof(T));
					if (UnityEngine.Object.FindObjectsOfType(typeof(T)).Length > 1)
					{
						UnityEngine.Debug.LogError("[PsdkSingleton] Something went really wrong  - there should never be more than 1 singleton! Reopenning the scene might fix it.");
						return _instance;
					}
					if ((UnityEngine.Object)_instance == (UnityEngine.Object)null)
					{
						if (pathToResource != null)
						{
							return CreateFromResources();
						}
						GameObject gameObject = new GameObject();
						Type typeFromHandle = typeof(T);
						Component component = gameObject.AddComponent(typeFromHandle);
						_instance = (T)component;
						string[] array = typeof(T).ToString().Split('.');
						gameObject.name = array[array.Length - 1];
						UnityEngine.Object.DontDestroyOnLoad(gameObject);
					}
				}
				return _instance;
			}
		}
	}

	private static T CreateFromResources()
	{
		GameObject original = Resources.Load(pathToResource) as GameObject;
		GameObject gameObject = UnityEngine.Object.Instantiate(original);
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
		_instance = gameObject.GetComponent<T>();
		return _instance;
	}

	protected virtual void OnDestroy()
	{
		applicationIsQuitting = true;
	}

	protected virtual void Awake()
	{
		if ((bool)(UnityEngine.Object)_instance)
		{
			UnityEngine.Object.DestroyImmediate(base.gameObject);
			applicationIsQuitting = false;
		}
		else
		{
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			_instance = base.gameObject.GetComponent<T>();
		}
	}
}
