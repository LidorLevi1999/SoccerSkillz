using UnityEngine;

namespace Soccerpass
{
	[CreateAssetMenu(fileName = "SessionState", menuName = "Data/SessionState")]
	public class SessionState : ScriptableObject
	{
		public int runCount;

		public bool isRevive;

		public bool isReload;

		public static string PATH = "data/state/SessionState";

		public string startRunType
		{
			get
			{
				if (isRevive)
				{
					return "revive";
				}
				if (runCount > 1)
				{
					return "restart";
				}
				return "new";
			}
		}

		public static SessionState Load()
		{
			return Resources.Load<SessionState>(PATH);
		}
	}
}
