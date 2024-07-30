using System;

namespace Soccerpass
{
	[Serializable]
	public class DetectionData
	{
		public float distanceToActivate = 20f;

		public float distanceToFollow = 15f;

		public float distanceToFollowBallInAir = 55f;

		public float distanceToGlitch = 10f;

		public float distanceToJump = 20f;

		public float distanceToFollowGroundBall = 80f;
	}
}
