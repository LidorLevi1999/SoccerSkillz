using System;
using UnityEngine;

public class AnimationStateMachine : StateMachineBehaviour
{
	public string id;

	private bool _didSendCompleteEvent;

	public static event Action<string> StateCompleteEvent;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		_didSendCompleteEvent = false;
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (stateInfo.normalizedTime > 0.999f)
		{
			AnimationStateMachine.StateCompleteEvent(id);
			_didSendCompleteEvent = true;
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}

	static AnimationStateMachine()
	{
		AnimationStateMachine.StateCompleteEvent = delegate
		{
		};
	}
}
