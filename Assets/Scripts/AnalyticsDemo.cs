using com.F4A.MobileThird;
using System.Collections.Generic;
using UnityEngine;

public class AnalyticsDemo : MonoBehaviour
{
	public void LogEvent()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("paramKey", "paramVal");
		EventsManager.Instance.LogEvent("testEventName", dictionary);
	}

	public void LogTimedEvent()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("paramKey", "paramVal");
		EventsManager.Instance.LogEvent("testTimedEventName", dictionary);
	}

	public void EndTimedEvent()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("paramKey2", "paramVal2");
		EventsManager.Instance.LogEvent("testTimedEventName", dictionary);
	}
}
