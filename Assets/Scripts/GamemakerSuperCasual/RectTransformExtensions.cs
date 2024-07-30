using UnityEngine;

namespace GamemakerSuperCasual
{
	public static class RectTransformExtensions
	{
		public static Rect RectTransformToScreenSpaceRect(this RectTransform transform)
		{
			Vector2 vector = Vector2.Scale(transform.rect.size, transform.lossyScale);
			Vector3 position = transform.position;
			float x = position.x;
			float num = Screen.height;
			Vector3 position2 = transform.position;
			Rect result = new Rect(x, num - position2.y, vector.x, vector.y);
			float x2 = result.x;
			Vector2 pivot = transform.pivot;
			result.x = x2 - pivot.x * vector.x;
			float y = result.y;
			Vector2 pivot2 = transform.pivot;
			result.y = y - (1f - pivot2.y) * vector.y;
			return result;
		}
	}
}
