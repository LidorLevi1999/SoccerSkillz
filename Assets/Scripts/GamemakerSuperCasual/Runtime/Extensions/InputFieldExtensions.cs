using UnityEngine.UI;

namespace GamemakerSuperCasual.Runtime.Extensions
{
	public static class InputFieldExtensions
	{
		public static float GetFloat(this InputField field)
		{
			float result = 0f;
			return (!float.TryParse(field.text, out result)) ? 0f : result;
		}
	}
}
