using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LinqExtensions
{
	public static bool IsNullOrEmpty(this ICollection _this)
	{
		if (_this == null)
		{
			return true;
		}
		return _this.Count == 0;
	}

	public static TElement RandomElement<TElement>(this IList<TElement> _this)
	{
		if (_this == null)
		{
			return default(TElement);
		}
		int index = UnityEngine.Random.Range(0, _this.Count);
		return _this[index];
	}

	public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
	{
		return source.MinBy(selector, null);
	}

	public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, IComparer<TKey> comparer)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (selector == null)
		{
			throw new ArgumentNullException("selector");
		}
		comparer = (comparer ?? Comparer<TKey>.Default);
		using (IEnumerator<TSource> enumerator = source.GetEnumerator())
		{
			if (!enumerator.MoveNext())
			{
				throw new InvalidOperationException("Sequence contains no elements");
			}
			TSource val = enumerator.Current;
			TKey y = selector(val);
			while (enumerator.MoveNext())
			{
				TSource current = enumerator.Current;
				TKey val2 = selector(current);
				if (comparer.Compare(val2, y) < 0)
				{
					val = current;
					y = val2;
				}
			}
			return val;
		}
	}
}
