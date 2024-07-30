using UnityEngine;

public class SplashDemo : MonoBehaviour
{
    public void Awake()
    {
        //PsdkSingleton<PsdkEventSystem>.Instance.onConfigurationLoadedEvent += OnSplashConfigurationLoaded;
        //PsdkSingleton<PsdkEventSystem>.Instance.onPsdkReady += OnPsdkReady;
        //if (PSDKMgr.Instance.Setup())
        //{
        //    PSDKMgr.Instance.AppIsReady();
        //}
    }

    private void OnSplashConfigurationLoaded()
	{
		UnityEngine.Debug.Log("DemoAll::OnSplashConfigurationLoaded !");
	}

	private void OnPsdkReady()
	{
		UnityEngine.Debug.Log("OnPsdkReady !");
		UnityEngine.SceneManagement.SceneManager.LoadScene(1);
	}
}
