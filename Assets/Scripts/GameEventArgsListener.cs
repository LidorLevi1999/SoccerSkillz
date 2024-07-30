using UnityEngine;
using UnityEngine.Events;

public class GameEventArgsListener : MonoBehaviour
{
	public GameEventArgs Event;

	public UnityEngine.Object Sender;

	public UnityEvent Response;

	private void OnEnable()
	{
		Event.RegisterListener(this);
	}

	private void OnDisable()
	{
		Event.UnregisterListener(this);
	}

	public void OnEventRaised(Object[] args)
	{
		if (Sender == null || Sender == args[0])
		{
			Response.Invoke();
		}
	}
}
