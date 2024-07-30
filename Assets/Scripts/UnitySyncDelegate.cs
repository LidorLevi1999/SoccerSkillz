using System;
using System.Reflection;
using UnityEngine;

public class UnitySyncDelegate : AndroidJavaProxy
{
	public UnitySyncDelegate()
		: base("")
	{
	}

	public void sendSyncMessage(string methodName, string message)
	{
		UnityEngine.Debug.Log("UnitySyncDelegate::sendSyncMessage - " + methodName + " - " + message);
		if (methodName != null)
		{
			//PsdkSingleton<PsdkEventSystem>.Instance.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic)?.Invoke(PsdkSingleton<PsdkEventSystem>.Instance, (message.Length <= 0) ? null : new object[1]
			//{
			//	message
			//});
		}
	}

	public string sendSyncMessageWithReturn(string methodName, string message)
	{
		UnityEngine.Debug.Log("UnitySyncDelegate::sendSyncMessageWithReturn - " + methodName + " - " + message);
		if (methodName != null)
		{
			//MethodInfo method = PsdkSingleton<PsdkEventSystem>.Instance.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
			//if (method != null)
			//{
			//	try
			//	{
			//		object obj = method.Invoke(PsdkSingleton<PsdkEventSystem>.Instance, (message.Length <= 0) ? null : new object[1]
			//		{
			//			message
			//		});
			//		if (obj != null && obj.GetType() == typeof(string))
			//		{
			//			return (string)obj;
			//		}
			//		UnityEngine.Debug.Log("UnitySyncDelegate::sendSyncMessageWithReturn - return object is not of type string or null.");
			//	}
			//	catch (Exception ex)
			//	{
			//		UnityEngine.Debug.Log("UnitySyncDelegate::sendSyncMessageWithReturn - invocation failed. Exception - " + ex.Message + ", " + ex.StackTrace);
			//	}
			//}
			//else
			//{
			//	UnityEngine.Debug.Log("UnitySyncDelegate::sendSyncMessageWithReturn - method info is null.");
			//}
		}
		else
		{
			UnityEngine.Debug.Log("UnitySyncDelegate::sendSyncMessageWithReturn - methodName is null.");
		}
		return null;
	}
}
