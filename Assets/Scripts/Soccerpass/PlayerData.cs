using System;

namespace Soccerpass
{
	[Serializable]
	public class PlayerData
	{
		public bool isHomeTeam;

		public PlayerRole playerRole;

		public string playerName;

		public int level;

		public float forwardSpeed;

		public SpeedData speedData;

		public int fieldLevel;

		public int teamIndex;

		public int shirtNumber;

		public bool CanGlitch
		{
			get
			{
				switch (playerRole)
				{
				case PlayerRole.GlitchByDistance:
				case PlayerRole.RunningAndGlitch:
				case PlayerRole.GoalKeeper:
					return true;
				default:
					return false;
				}
			}
		}

		public bool CanJump
		{
			get
			{
				PlayerRole playerRole = this.playerRole;
				if (playerRole == PlayerRole.GoalKeeper)
				{
					return true;
				}
				return false;
			}
		}
	}
}
