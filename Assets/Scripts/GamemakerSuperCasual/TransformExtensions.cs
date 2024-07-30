using UnityEngine;

namespace GamemakerSuperCasual
{
	public static class TransformExtensions
	{
		public static void RemoveAllChildren(this Transform transform)
		{
			Transform[] array = new Transform[transform.childCount];
			for (int i = 0; i < transform.childCount; i++)
			{
				array[i] = transform.GetChild(i);
			}
			transform.DetachChildren();
			foreach (Transform transform2 in array)
			{
				UnityEngine.Object.DestroyImmediate(transform2.gameObject, allowDestroyingAssets: false);
			}
		}

		public static void DestroyAllChildren(this Transform transform, bool allowDestroyingAssets)
		{
			while (transform.childCount > 0)
			{
				Transform child = transform.GetChild(0);
				UnityEngine.Object.DestroyImmediate(child.gameObject, allowDestroyingAssets);
			}
		}

		public static Vector3 left(this Transform transform)
		{
			return transform.right * -1f;
		}

		public static float Distance(this Transform transform, Transform other)
		{
			return Vector3.Distance(transform.position, other.position);
		}
	}
}
