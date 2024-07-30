using GamemakerSuperCasual;
using UnityEngine;

namespace Soccerpass
{
	[CreateAssetMenu(fileName = "PlayerControlParams", menuName = "Data/PlayerControlParams")]
	public class PlayerControlParams : ScriptableObject
	{
		public int startLevel;

		[Header("ui intensity")]
		public float highlightAmount = 2f;

		[Header("energy")]
		public float holdBallTime = 5f;

		public float lowEnergyTime = 2f;

		[Header("movement")]
		[DebugHide]
		public float freeBallSpeedModifier = 0.2f;

		public float forwardSpeed = 50f;

		public float forwardSpeedWithBall = 60f;

		public float lowEnergySpeedModifier = 0.75f;

		public float noEnergySpeedModifier = 0.5f;

		public float dragToSteerTutorialThreshold = 15f;

		public SpeedData homePlayerSpeedData;

		public SpeedData homeGoalkeeperSpeedData;

		[Header("rotation")]
		public float rotationSpeed = 0.3f;

		[DebugHide]
		public float lerpFixTime = 3f;

		public float maxAngle = 60f;

		[Header("detection")]
		public DetectionData fieldPlayerDetection;

		public DetectionData goalkeeperDetection;
	}
}
