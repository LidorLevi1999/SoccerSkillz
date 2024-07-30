namespace GamemakerSuperCasual.Runtime.Extensions
{
	public static class FloatExtensions
	{
		public static float GetNormalized(this float value)
		{
			return (value > 1f) ? 1f : ((!(value < 0f)) ? value : 0f);
		}
	}
}
