using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GamemakerSuperCasual
{
	public static class EnumerableExtensions
	{
		public static int CalculateHash<TElement>(this IEnumerable<TElement> _this) where TElement : class
		{
			int num = 0;
			foreach (TElement _thi in _this)
			{
				TElement current = _thi;
				num = 31 * num + (current?.GetHashCode() ?? 0);
			}
			return num;
		}

		public static int FirstIndex<TElement>(this IEnumerable<TElement> _this, Predicate<TElement> predicate)
		{
			IEnumerator<TElement> enumerator = _this.GetEnumerator();
			int num = -1;
			while (enumerator.MoveNext())
			{
				num++;
				if (predicate(enumerator.Current))
				{
					return num;
				}
			}
			return -1;
		}

		public static IEnumerable<TElement> SelectDistinct<TElement, TKey>(this IEnumerable<TElement> _this, Func<TElement, TKey> groupSelector)
		{
			return _this.GroupBy(groupSelector).Select(Enumerable.First<TElement>);
		}

		public static int Max<TSource>(this IEnumerable<TSource> _this, Func<TSource, int> selector)
		{
			int num = int.MinValue;
			foreach (TSource _thi in _this)
			{
				int num2 = selector(_thi);
				if (num2 > num)
				{
					num = num2;
				}
			}
			return num;
		}

		public static int Sum<TSource>(this IEnumerable<TSource> _this, Func<TSource, int> selector)
		{
			int num = 0;
			foreach (TSource _thi in _this)
			{
				num += selector(_thi);
			}
			return num;
		}

		public static int Sum(this IEnumerable<int> _this)
		{
			int num = 0;
			foreach (int _thi in _this)
			{
				num += _thi;
			}
			return num;
		}

		public static int Max(this IEnumerable<int> _this)
		{
			int num = int.MinValue;
			foreach (int _thi in _this)
			{
				if (_thi > num)
				{
					num = _thi;
				}
			}
			return num;
		}

		public static TAccumulate Aggregate<TSource, TAccumulate>(this IEnumerable<TSource> _this, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
		{
			TAccumulate val = seed;
			foreach (TSource _thi in _this)
			{
				val = func(val, _thi);
			}
			return val;
		}

		public static IEnumerable<int> Range(int start, int count)
		{
			List<int> list = new List<int>();
			for (int i = start; i < start + count; i++)
			{
				list.Add(i);
			}
			return list;
		}

		public static IEnumerable<TElement> SelectRandom<TElement>(this IEnumerable<TElement> _this, int count = 1)
		{
			if (count >= _this.Count())
			{
				return _this;
			}
			List<int> list = Enumerable.Range(0, _this.Count()).ToList();
			List<int> list2 = new List<int>();
			for (int i = 0; i < count; i++)
			{
				int item = list.ElementAt(UnityEngine.Random.Range(0, list.Count()));
				list2.Add(item);
				list.Remove(item);
			}
			List<TElement> list3 = new List<TElement>();
			foreach (int item2 in list2)
			{
				list3.Add(_this.ElementAt(item2));
			}
			return list3;
		}

		public static IEnumerable<int> SelectRandomIndices<TElement>(this IEnumerable<TElement> _this, int count = 1)
		{
			List<int> list = Enumerable.Range(0, _this.Count()).ToList();
			List<int> list2 = new List<int>();
			int num = Mathf.Min(count, _this.Count());
			for (int i = 0; i < num; i++)
			{
				int item = list.ElementAt(UnityEngine.Random.Range(0, list.Count()));
				list2.Add(item);
				list.Remove(item);
			}
			return list2;
		}

		public static TElement SelectOneWeighted<TElement>(this IEnumerable<TElement> _this, Func<TElement, int> selector)
		{
			if (!_this.Any())
			{
				return default(TElement);
			}
			if (_this.Count() == 1)
			{
				return _this.First();
			}
			int num = _this.Sum(selector);
			int num2 = UnityEngine.Random.Range(1, num + 1);
			foreach (TElement _thi in _this)
			{
				if (num2 <= selector(_thi))
				{
					return _thi;
				}
				num2 -= selector(_thi);
			}
			return _this.First();
		}

		public static IEnumerable<TElement> SelectRandomWeighted<TElement>(this IEnumerable<TElement> _this, Func<TElement, int> selector, int count = 1)
		{
			if (count >= _this.Count())
			{
				return _this;
			}
			List<TElement> list = _this.ToList();
			List<TElement> list2 = new List<TElement>();
			for (int i = 0; i < count; i++)
			{
				TElement item = _this.SelectOneWeighted(selector);
				list2.Add(item);
				list.Remove(item);
			}
			return list2;
		}

		public static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
		{
			Dictionary<TKey, TElement> dictionary = new Dictionary<TKey, TElement>();
			foreach (TSource item in source)
			{
				dictionary.Add(keySelector(item), elementSelector(item));
			}
			return dictionary;
		}

		public static IEnumerable<T> Shuffle<T>(this IList<T> _this)
		{
			IList<T> list = _this.ToList();
			System.Random random = new System.Random();
			int num = list.Count;
			while (num > 1)
			{
				num--;
				int index = random.Next(num + 1);
				T value = list[index];
				list[index] = list[num];
				list[num] = value;
			}
			return list;
		}

		public static IEnumerable<int> Values(this System.Random random, int minValue, int maxValue)
		{
			while (true)
			{
				yield return random.Next(minValue, maxValue);
			}
		}

		public static IEnumerable<TResult> Pairwise<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TSource, TResult> resultSelector)
		{
			TSource previous = default(TSource);
			IEnumerator<TSource> it = source.GetEnumerator();
			try
			{
				if (it.MoveNext())
				{
					previous = it.Current;
				}
				while (it.MoveNext())
				{
					TSource arg = previous;
					TSource current;
					previous = (current = it.Current);
					yield return resultSelector(arg, current);
				}
			}
			finally
			{
                //@TODO _003C_003E__Finally0
                //base._003C_003E__Finally0();
            }
        }

		public static IEnumerable<T> Append<T>(this IEnumerable<T> source, params T[] args)
		{
			return source.Concat(args);
		}

		public static int Sum(this int[] arr)
		{
			int num = 0;
			for (int i = 0; i < arr.Length; i++)
			{
				num += arr[i];
			}
			return num;
		}

		public static Dictionary<TKey, TValue> WithoutNullKeys<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
		{
			return (from kv in dictionary
				where kv.Value != null
				select kv).ToDictionary((KeyValuePair<TKey, TValue> kv) => kv.Key, (KeyValuePair<TKey, TValue> kv) => kv.Value);
		}
	}
}
