using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "PsdkData", menuName = "Gamemaker Super Casual/Psdk Data", order = 1)]
public class ScriptablePsdkData : ScriptableObject
{
	[Header("URLs")]
	public string leaderboardIdIOS;

	public string leaderboardIdGP;

	[Header("Share")]
	public string shareHeader;

	public string shareSubject;

	public string shareBody;

	[Header("Achivements")]
	[SerializeField]
	public List<AchivementData> achievments;

	[Header("InApps")]
	[SerializeField]
	public List<InappData> inApps;

	private string leaderboardId;

	public static string PATH = "data/psdkData";

	public string LeaderboardId => leaderboardId;

	public static bool IS_GP => true;

	public static bool IS_IOS => false;

	public static ScriptablePsdkData Load()
	{
		return Resources.Load<ScriptablePsdkData>(PATH);
	}

	public void SetUrls()
	{
		leaderboardId = leaderboardIdGP;
	}

	public string GetAchivUrl(string achivId)
	{
		AchivementData achivementData = achievments.First((AchivementData ach) => ach.id == achivId);
		if (achivementData == null)
		{
			return string.Empty;
		}
		return (!IS_GP) ? achivementData.urlIOS : achivementData.urlGP;
	}

	public string GetInappStoreId(string id)
	{
		InappData inappData = inApps.First((InappData iap) => iap.id == id);
		if (inappData == null)
		{
			return string.Empty;
		}
		return (!IS_GP) ? inappData.iapIdIOS : inappData.iapIdGP;
	}

	public string GetItemIdOfIapId(string iapId)
	{
		InappData inappData = (!IS_GP) ? inApps.First((InappData iap) => iap.iapIdIOS == iapId) : inApps.First((InappData iap) => iap.iapIdGP == iapId);
		if (inappData == null)
		{
			return string.Empty;
		}
		return inappData.id;
	}
}
