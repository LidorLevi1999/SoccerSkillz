using UnityEngine;

namespace GamemakerSuperCasual
{
	public static class RectExtensions
	{
		public static Rect Inflate(this Rect rect, float amount)
		{
			float width = rect.width;
			float height = rect.height;
			float x = rect.x;
			float y = rect.y;
			float num = width * amount;
			float num2 = height * amount;
			Vector2 center = rect.center;
			float x2 = center.x - num / 2f;
			Vector2 center2 = rect.center;
			float y2 = center2.y - num2 / 2f;
			rect.Set(x2, y2, num, num2);
			return rect;
		}
	}
}
