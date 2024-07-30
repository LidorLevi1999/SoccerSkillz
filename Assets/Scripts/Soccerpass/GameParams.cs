using UnityEngine;

namespace Soccerpass
{
	[CreateAssetMenu(fileName = "GameParams", menuName = "Data/GameParams")]
	public class GameParams : ScriptableObject
	{
		public float MaxTimeOnGround = 3f;

		public float openingKickDelay = 1f;

		[Header("Physics")]
		public float gravity = -55f;

		public float kickForce = 65f;

		[Header("player formations")]
		public float firstFormationAfterGoal = 200f;

		[Header("time scale effects")]
		public float fastTimeScale = 1.5f;

		public float normalTimeScale = 1f;

		public float slowTimeScale = 0.6f;

		public float goalTimeScale = 0.7f;

		public float timeToTriggerNormalScale = 1f;

		public float timeToTriggerSlowScale = 0.7f;

		[Header("scoring")]
		public float scoreDistanceUnit = 100f;

		public int goalBonus = 100;

		public float goalKickDistance = 150f;

		public int reviveThreshold = 15;

		public int reviveOfferTime = 10;

		public int reviveRestartButtonDelay = 5;
	}
}
