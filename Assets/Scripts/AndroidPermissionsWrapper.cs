using System;
using UnityEngine;

public class AndroidPermissionsWrapper
{
	private static AndroidPermissionsWrapper _instace;

	private AndroidJavaObject _androidJavaObject;

	public static AndroidPermissionsWrapper Instance
	{
		get
		{
			if (_instace == null)
			{
				_instace = new AndroidPermissionsWrapper();
				if (!_instace.createJavaObject())
				{
					_instace = null;
					return null;
				}
			}
			return _instace;
		}
	}

	public event Action<string[], bool[]> OnRequestPermissionsResultEvent;

	private bool createJavaObject()
	{
		if (_androidJavaObject == null)
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			if (@static == null)
			{
				UnityEngine.Debug.LogError("failed to get activity java object.");
				return false;
			}
			AndroidJavaObject androidJavaObject = new AndroidJavaObject("");
			if (androidJavaObject == null)
			{
				UnityEngine.Debug.LogError("failed to create delegate java object.");
				return false;
			}
			_androidJavaObject = new AndroidJavaObject("", @static, androidJavaObject);
		}
		if (_androidJavaObject == null)
		{
			UnityEngine.Debug.LogError("was not able to initiate java object.");
			return false;
		}
		return true;
	}

	public void PassOnRequestPermissionsResult(string[] permissions, bool[] granted)
	{
		this.OnRequestPermissionsResultEvent(permissions, granted);
	}

	private void OnRequestPermissionsResult(string message)
	{
		string[] array = message.Split(';');
		string[] array2 = new string[array.Length / 2];
		bool[] array3 = new bool[array.Length / 2];
		int num = 0;
		int num2 = 0;
		string[] array4 = array;
		foreach (string text in array4)
		{
			if (num % 2 == 0)
			{
				array2[num2] = text;
			}
			else
			{
				bool result = false;
				bool.TryParse(array[num], out result);
				array3[num2] = result;
				num2++;
			}
			num++;
		}
		this.OnRequestPermissionsResultEvent(array2, array3);
	}

	public bool CheckSelfPermission(string permission)
	{
		if (_androidJavaObject != null)
		{
			return _androidJavaObject.Call<bool>("checkSelfPermission", new object[1]
			{
				permission
			});
		}
		UnityEngine.Debug.LogError("java object is not initiated.");
		return false;
	}

	public bool ShouldShowRequestPermissionRationale(string permission)
	{
		if (_androidJavaObject != null)
		{
			return _androidJavaObject.Call<bool>("shouldShowRequestPermissionRationale", new object[1]
			{
				permission
			});
		}
		UnityEngine.Debug.LogError("java object is not initiated.");
		return false;
	}

	public void RequestPermissions(string[] permissions)
	{
		if (_androidJavaObject != null && permissions != null)
		{
			string text = string.Empty;
			foreach (string str in permissions)
			{
				text = text + str + ";";
			}
			text.TrimEnd(';');
			_androidJavaObject.Call("requestPermissions", text);
		}
		else
		{
			UnityEngine.Debug.LogError("java object is not initiated.");
		}
	}
}
