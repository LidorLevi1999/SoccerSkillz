using System;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GamemakerSuperCasual
{
	public static class TTPlayerPrefs
	{
		[CompilerGenerated]
		private static Func<string, int> _003C_003Ef__mg_0024cache0;

		public static void SetVector3(string name, Vector3 value)
		{
			SetString(name, value.ToString());
		}

		public static void SetInt(string name, int value)
		{
			PlayerPrefs.SetInt(name, value);
		}

		public static void SetFloat(string name, float value)
		{
			PlayerPrefs.SetFloat(name, value);
		}

		public static void SetBool(string name, bool value)
		{
			PlayerPrefs.SetInt(name, value ? 1 : 0);
		}

		public static void SetString(string name, string value)
		{
			PlayerPrefs.SetString(name, value);
		}

		public static void SetDateTime(string name, DateTime value)
		{
			string value2 = string.Empty + value.Year + "_" + value.Month + "_" + value.Day + "_" + value.Hour + "_" + value.Minute + "_" + value.Second;
			PlayerPrefs.SetString(name, value2);
		}

		public static int GetInt(string name, int defaultValue = 0)
		{
			return PlayerPrefs.GetInt(name, defaultValue);
		}

		public static float GetFloat(string name, float defaultValue = 0f)
		{
			return PlayerPrefs.GetFloat(name, defaultValue);
		}

		public static bool GetBool(string name, bool defaultValue = false)
		{
			return PlayerPrefs.GetInt(name, defaultValue ? 1 : 0) != 0;
		}

		public static string GetString(string name, string defaultValue = "")
		{
			return PlayerPrefs.GetString(name, defaultValue);
		}

		public static DateTime GetDateTime(string name, DateTime value)
		{
			string @string = PlayerPrefs.GetString(name, string.Empty);
			string[] array = @string.Split('_');
			if (array.Length != 6)
			{
				return value;
			}
			try
			{
				int[] array2 = array.Select(int.Parse).ToArray();
				return new DateTime(array2[0], array2[1], array2[2], array2[3], array2[4], array2[5], 0);
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.Log("TTPlayerPrefs GetDateTime Found illegalString: [" + @string + "] ex=" + ex.Message);
				return value;
			}
		}

		public static void DeleteAll()
		{
			PlayerPrefs.DeleteAll();
			Save();
		}

		public static void Save()
		{
			PlayerPrefs.Save();
		}
	}
}
