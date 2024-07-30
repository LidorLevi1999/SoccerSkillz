using com.F4A.MobileThird;
using System.Collections;
using UnityEngine;

namespace GamemakerSuperCasual
{
	public class NetworkCheck : MonoBehaviour
	{
		public float RefreshTime = 0.5f;

		public static NetworkCheck Instance
		{
			get;
			private set;
		}

		public NetworkStatus Status
		{
			get;
			private set;
		}

		public static bool HasInternetConnection
		{
			get
			{
				return DMCMobileUtils.IsInternetAvailable();
			}
		}

		public void Awake()
		{
			Status = NetworkStatus.Connected;
			Instance = this;
		}

		private string GenerateAntiCachingValue()
		{
			return "?p=" + UnityEngine.Random.Range(1, 100000000).ToString();
		}
	}
}
