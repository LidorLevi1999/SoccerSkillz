using System;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace GamemakerSuperCasual
{
	public static class StringExtensions
	{
		public static string ReplaceAt(this string input, int index, char newChar)
		{
			if (input == null || index >= input.Length)
			{
				return input;
			}
			char[] array = input.ToCharArray();
			array[index] = newChar;
			return new string(array);
		}

		public static int CountChar(this string input, char countChar)
		{
			int num = 0;
			for (int i = 0; i < input.Length; i++)
			{
				if (input[i] == countChar)
				{
					num++;
				}
			}
			return num;
		}

		public static int CountSpaces(this string input)
		{
			return input.CountChar(' ');
		}

		public static string RemoveChar(this string input, char removeChar)
		{
			string text = string.Empty;
			for (int i = 0; i < input.Length; i++)
			{
				if (input[i] != removeChar)
				{
					text += input[i];
				}
			}
			return text;
		}

		public static string RemoveSpaces(this string input)
		{
			return input.RemoveChar(' ');
		}

		public static Vector2 PlayerPrefToVector2(this string input)
		{
			Vector2 zero = Vector2.zero;
			try
			{
				int num = input.IndexOf('_');
				if (num > 0)
				{
					string s = input.Substring(0, num);
					string s2 = input.Substring(num + 1);
					return new Vector2(float.Parse(s), float.Parse(s2));
				}
				return zero;
			}
			catch (Exception)
			{
				return Vector2.zero;
			}
		}

		public static Vector3 PlayerPrefToVector3(this string input)
		{
			Vector3 zero = Vector3.zero;
			try
			{
				int num = input.IndexOf('_');
				int num2 = (num != -1) ? input.IndexOf('_', num + 1) : (-1);
				if (num2 > 0)
				{
					string s = input.Substring(0, num);
					string s2 = input.Substring(num + 1, num2 - num - 1);
					string s3 = input.Substring(num2 + 1);
					return new Vector3(float.Parse(s), float.Parse(s2), float.Parse(s3));
				}
				return zero;
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.Log("PlayerPrefToVector3 exception=" + ex.Message);
				return Vector3.zero;
			}
		}

		public static string SplitCamelCase(this string input)
		{
			string text = Regex.Replace(input, "([A-Z])", " $1", RegexOptions.Singleline).Trim();
			return text.First().ToString().ToUpper() + text.Substring(1);
		}
	}
}
