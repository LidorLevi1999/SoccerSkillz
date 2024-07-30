using UnityEngine;

[CreateAssetMenu(fileName = "IntRangeVar", menuName = "Data/IntRangeVar")]
public class IntRangeVar : IntVar
{
	public int min;

	public int max;

	public float Normalized => (float)value * 1f / (float)max;

	public void Add(int amount)
	{
		value += amount;
		if (value > max)
		{
			value = max;
		}
		if (value < min)
		{
			value = min;
		}
	}
}
