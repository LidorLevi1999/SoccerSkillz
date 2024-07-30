namespace com.gamemaker.soccerskillz
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using GamemakerSuperCasual;
	using UnityEngine;
	using UnityEngine.SceneManagement;

	public static class Utils
	{
		public static void ApplySafeArea(List<RectTransform> iphoneXfixItems, bool debugXfix = false)
		{
			Rect safeArea = Screen.safeArea;
			Vector2 position = safeArea.position;
			Vector2 anchorMax = safeArea.position + safeArea.size;
			position.x /= Screen.width;
			position.y /= Screen.height;
			anchorMax.x /= Screen.width;
			anchorMax.y /= Screen.height;
			if (position.x <= 0f)
			{
				position.x = 0f;
			}
			if (position.y <= 0f)
			{
				position.y = 0f;
			}
			if (anchorMax.x >= 1f)
			{
				anchorMax.x = 1f;
			}
			if (anchorMax.y >= 1f)
			{
				anchorMax.y = 1f;
			}
			if (debugXfix)
			{
				anchorMax.y = 0.9f;
				position.y = 0.1f;
			}
			foreach (RectTransform iphoneXfixItem in iphoneXfixItems)
			{
				iphoneXfixItem.anchorMin = position;
				iphoneXfixItem.anchorMax = anchorMax;
			}
		}

		public static float CalculateAngle(Vector3 from, Vector3 to)
		{
			Vector3 eulerAngles = Quaternion.FromToRotation(Vector3.up, to - from).eulerAngles;
			return eulerAngles.z;
		}

		public static float CalculateAngle(Vector2 from, Vector2 to)
		{
			Vector3 eulerAngles = Quaternion.FromToRotation(Vector3.up, to - from).eulerAngles;
			return eulerAngles.z;
		}

		public static IEnumerable<int> SplitToParts(this int value, int parts, int minValue)
		{
			if (parts <= 0)
			{
				throw new ArgumentException("count must be greater than zero.", "count");
			}
			int[] array = new int[parts];
			int num = 0;
			for (int i = 0; i < parts; i++)
			{
				int num2 = value - num;
				int num3 = (num2 > 0) ? (num2 / (parts - i)) : 0;
				int num4 = parts - i;
				num += (array[i] = UnityEngine.Random.Range(minValue, num2 - num4 * minValue));
			}
			if (num < value)
			{
				array[array.Length - 1] += value - num;
			}
			return array;
		}

		public static IEnumerable<int> SplitToMinMax(this int value, int minValue, int maxValue)
		{
			List<int> list = new List<int>();
			int num;
			for (int i = 0; i < value; i += num)
			{
				num = UnityEngine.Random.Range(minValue, maxValue + 1);
				if (i + num > value)
				{
					num = value - i;
				}
				list.Add(num);
			}
			return list;
		}

		public static List<int> SplitAndShuffle(this int value, int parts, int minValue)
		{
			return value.SplitToParts(parts, minValue).ToList().Shuffle().ToList();
		}

		public static List<int> RandomSplit(this int value, int minParts, int maxParts)
		{
			int num = UnityEngine.Random.Range(minParts, maxParts + 1);
			if (num > value)
			{
				num = value;
			}
			return value.SplitToParts(num, 2).ToList().Shuffle().ToList();
		}

		public static void UpdateTimeScale(float value)
		{
			Time.timeScale = value;
			Time.fixedDeltaTime = 0.02f * Time.timeScale;
		}

		public static void Quit()
		{
			Application.Quit();
		}

		public static void Vibrate()
		{
			if (UserSettings.Vibrates)
			{
				Handheld.Vibrate();
			}
		}

		public static void LoadGameScene()
		{
			TTAudioManager.Instance.StopMusic(SoundId.crowd_ambiance_loop);
			TTAudioManager.Instance.StopMusic(SoundId.crowd_excited);
			SceneManager.LoadScene(1);
		}

		public static Vector3 CalculateInterceptCourse(Vector3 aTargetPos, Vector3 aTargetSpeed, Vector3 aInterceptorPos, float aInterceptorSpeed, out bool aSuccess)
		{
			aSuccess = true;
			Vector3 vector = aTargetPos - aInterceptorPos;
			float num = aInterceptorSpeed * aInterceptorSpeed;
			float sqrMagnitude = aTargetSpeed.sqrMagnitude;
			float num2 = Vector3.Dot(vector, aTargetSpeed);
			float sqrMagnitude2 = vector.sqrMagnitude;
			float num3 = num2 * num2 - sqrMagnitude2 * (sqrMagnitude - num);
			if (num3 < 0.1f)
			{
				aSuccess = false;
			}
			float num4 = Mathf.Sqrt(num3);
			float num5 = (0f - num2 - num4) / sqrMagnitude2;
			float num6 = (0f - num2 + num4) / sqrMagnitude2;
			if (num5 < 0.0001f)
			{
				if (num6 < 0.0001f)
				{
					return Vector3.zero;
				}
				return num6 * vector + aTargetSpeed;
			}
			if (num6 < 0.0001f)
			{
				return num5 * vector + aTargetSpeed;
			}
			if (num5 < num6)
			{
				return num6 * vector + aTargetSpeed;
			}
			return num5 * vector + aTargetSpeed;
		}

		public static Vector3 AcquireTargetLock(Vector3 targetPos, Vector3 targetVelocity, Vector3 sourcePos, float sourceSpeed, out bool success)
		{
			float num = targetPos.x - sourcePos.x;
			float num2 = targetPos.z - sourcePos.z;
			float num3 = targetVelocity.z * targetVelocity.z;
			float num4 = targetVelocity.x * targetVelocity.x;
			float num5 = sourceSpeed * sourceSpeed;
			float num6 = num4 + num3 - num5;
			float num7 = 2f * (targetVelocity.x * num + targetVelocity.z * num2);
			float num8 = num * num + num2 * num2;
			float num9 = num7 * num7 - 4f * num6 * num8;
			if (num9 < 0f)
			{
				success = false;
				return Vector3.zero;
			}
			float a = (-1f * num7 + Mathf.Sqrt(num9)) / (2f * num6);
			float b = (-1f * num7 - Mathf.Sqrt(num9)) / (2f * num6);
			float num10 = Mathf.Max(a, b);
			float x = targetVelocity.x * num10 + targetPos.x;
			float z = sourcePos.z + targetVelocity.z * num10;
			float y = targetPos.y;
			success = true;
			return new Vector3(x, y, z);
		}
	}
}