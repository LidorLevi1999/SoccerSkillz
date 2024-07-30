using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GamemakerSuperCasual
{
	public static class EnumUtils
	{
		private static Random rng = new Random();

		public static T RandomEnumValue<T>(bool excludeFirstValue = false)
		{
			Array values = Enum.GetValues(typeof(T));
			int minValue = excludeFirstValue ? 1 : 0;
			return (T)values.GetValue(new Random().Next(minValue, values.Length));
		}

		public static T RandomEnumValue<T>(T excludeType)
		{
			Array values = Enum.GetValues(typeof(T));
			List<T> list = new List<T>((T[])values);
			list.Remove(excludeType);
			return list.ElementAt(new Random().Next(0, list.Count));
		}

		public static Array EnumValues<T>()
		{
			return Enum.GetValues(typeof(T));
		}

		public static T Next<T>(this T src, bool skipFirstIndex = false) where T : struct
		{
			if (!typeof(T).IsEnum)
			{
				throw new ArgumentException($"Argumnent {typeof(T).FullName} is not an Enum");
			}
			T[] array = (T[])Enum.GetValues(src.GetType());
			int num = Array.IndexOf(array, src) + 1;
			return (array.Length != num) ? array[num] : array[skipFirstIndex ? 1 : 0];
		}

		public static void Shuffle2<T>(this IList<T> list)
		{
			int num = list.Count;
			while (num > 1)
			{
				num--;
				int index = rng.Next(num + 1);
				T value = list[index];
				list[index] = list[num];
				list[num] = value;
			}
		}

		public static IEnumerable<TValue> RandomValues<TKey, TValue>(this IDictionary<TKey, TValue> dict)
		{
			Random rand = new Random();
			List<TValue> values = dict.Values.ToList();
			int size = dict.Count;
			while (true)
			{
				yield return values[rand.Next(size)];
			}
		}

		public static IEnumerable<TKey> RandomKeys<TKey, TValue>(this IDictionary<TKey, TValue> dict)
		{
			Random rand = new Random();
			List<TKey> values = dict.Keys.ToList();
			int size = dict.Count;
			while (true)
			{
				yield return values[rand.Next(size)];
			}
		}

		public static List<TKey> RandomUniqueKeys<TKey, TValue>(this IDictionary<TKey, TValue> dict, int count)
		{
			List<TKey> list = dict.Keys.ToList();
			int count2 = dict.Count;
			list.Shuffle2();
			if (count >= list.Count)
			{
				return list;
			}
			return list.GetRange(0, count);
		}

		public static string ToCommaSeparatedString<T>(this List<T> list)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < list.Count; i++)
			{
				T val = list[i];
				if (i > 0)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append(val.ToString());
			}
			return stringBuilder.ToString();
		}
	}
}
