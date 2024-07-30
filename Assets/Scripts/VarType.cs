using UnityEngine;

public class VarType<T> : ScriptableObject
{
	public T value;

	public VarType()
	{
	}

	public VarType(T val)
	{
		value = val;
	}

	public static implicit operator T(VarType<T> s)
	{
		return s.value;
	}

	public void SetValue(T val)
	{
		value = val;
	}

	public override string ToString()
	{
		return value.ToString();
	}
}
