using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameEvent : ScriptableObject
{
	private List<GameEventListener> listeners = new List<GameEventListener>();

	public void Raise()
	{
		for (int num = listeners.Count - 1; num >= 0; num--)
		{
			listeners[num].OnEventRaised();
		}
	}

	public void RegisterListener(GameEventListener listener)
	{
		if (listeners.Contains(listener))
		{
			throw new InvalidOperationException("Duplicate key");
		}
		listeners.Add(listener);
	}

	public void UnregisterListener(GameEventListener listener)
	{
		if (listeners.Contains(listener))
		{
			listeners.Remove(listener);
			return;
		}
		throw new InvalidOperationException("No listener to remove");
	}
}
