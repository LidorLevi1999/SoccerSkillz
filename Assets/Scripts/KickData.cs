using UnityEngine;

namespace Soccerpass
{
	public struct KickData
	{
		public SoccerPlayer kicker;

		public Vector3 origin;

		public Vector3 destination;

		public bool isGoalKick;

		public bool isOpeningKick;

		public Vector3 goalPos;

		public bool active;

		public bool didScore;

		public float duration;

		public void Reset()
		{
			kicker = null;
			origin = Vector3.zero;
			destination = Vector3.zero;
			isGoalKick = false;
			isOpeningKick = false;
			goalPos = Vector3.zero;
			active = false;
			didScore = false;
			duration = -1f;
		}
	}
}
