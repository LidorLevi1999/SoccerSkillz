using UnityEngine;

public class PublishingSDKCoreDemo : MonoBehaviour
{
	//public void OnResume(AppLifeCycleResumeState resumeState)
	//{
	//	UnityEngine.Debug.Log("PublishingSDKCoreDemo - OnResume - resumeState = " + resumeState.ToString());
	//}

	public void OnConfigurationReady()
	{
		UnityEngine.Debug.Log("PublishingSDKCoreDemo - OnConfigurationReady");
	}

	public void OnPsdkReady()
	{
		UnityEngine.Debug.Log("PublishingSDKCoreDemo - OnPsdkReady");
	}
}
