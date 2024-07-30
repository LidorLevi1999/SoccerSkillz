using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameEventArgs : ScriptableObject
{
	public List<GameEventArgsListener> listeners = new List<GameEventArgsListener>();

	public void Raise(params Object[] args)
	{
		for (int num = listeners.Count - 1; num >= 0; num--)
		{
			listeners[num].OnEventRaised(args);
		}
	}

	public void RegisterListener(GameEventArgsListener listener)
	{
		if (listeners.Contains(listener))
		{
			UnityEngine.Debug.LogError("listener is already in list");
		}
		else
		{
			listeners.Add(listener);
		}
	}

	public void UnregisterListener(GameEventArgsListener listener)
	{
		if (!listeners.Contains(listener))
		{
			UnityEngine.Debug.LogError("listener is not in list");
		}
		else
		{
			listeners.Remove(listener);
		}
	}
}
