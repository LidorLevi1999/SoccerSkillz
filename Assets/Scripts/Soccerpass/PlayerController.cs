using System;
using GamemakerSuperCasual;
using UnityEngine;

namespace Soccerpass
{
	public class PlayerController : MonoBehaviour, IEventListener
	{
		public SoccerPlayer activePlayer;

		public PlayerControlParams controlParams;

		public GameParams gameParams;

		public FollowTarget playerHL;

		public PlayersSpawnManager playersManager;

		private Ball _ball;

		public static event Action OutOfEnergyEvent;

		public static event Action LowEnergyEvent;

		public static event Action<float> DragEvent;

		public void Init()
		{
			_ball = UnityEngine.Object.FindObjectOfType<Ball>();
			playersManager = UnityEngine.Object.FindObjectOfType<PlayersSpawnManager>();
		}

		private void OnEnable()
		{
			AddListeners();
		}

		private void OnDisable()
		{
			RemoveListeners();
		}

		public void OnUpdate()
		{
			if (!InputManager.isMouseDown)
			{
				if (activePlayer != null && activePlayer.state == SoccerPlayer.State.WithBall)
				{
					Kick();
				}
			}
			else
			{
				_ball.TryToConnect();
			}
		}

		public void AddListeners()
		{
			InputManager.Dragging += OnDrag;
			Ball.CatchBallEvent += Ball_CatchBallEvent;
			Ball.GoalEvent += Ball_GoalEvent;
			Ball.TackleEvent += Ball_TackleEvent;
		}

		private void Ball_TackleEvent(SoccerPlayer arg1, FailureType arg2)
		{
			ResetEnergy();
		}

		private void Ball_GoalEvent()
		{
			ResetEnergy();
			if (activePlayer != null && activePlayer.state == SoccerPlayer.State.WithBall)
			{
				ActivateEnergy();
			}
		}

		private void ResetEnergy()
		{
			LeanTween.cancel(base.gameObject);
			if (activePlayer != null)
			{
				activePlayer.ToggleEnergyBar(toggle: false);
			}
		}

		private void Ball_CatchBallEvent(SoccerPlayer player)
		{
			activePlayer = player;
			activePlayer.CatchBall(!_ball.IsNearGoal);
			if (!_ball.invincible)
			{
				ActivateEnergy();
			}
			else
			{
				activePlayer.UpdateEnergyAmount(1f);
			}
		}

		private void OnDrag(DragInfo dragInfo)
		{
			if (activePlayer != null && (activePlayer.state == SoccerPlayer.State.WithBall || activePlayer.state == SoccerPlayer.State.Connecting))
			{
				activePlayer.RotateBy(dragInfo.delta.x);
				if (!TutorialManager.IsTutorialDone && Mathf.Abs(dragInfo.delta.x) >= controlParams.dragToSteerTutorialThreshold)
				{
					PlayerController.DragEvent(dragInfo.delta.x);
				}
			}
		}

		public void RemoveListeners()
		{
			Ball.GoalEvent -= Ball_GoalEvent;
			Ball.TackleEvent -= Ball_TackleEvent;
			InputManager.Dragging -= OnDrag;
			Ball.CatchBallEvent -= Ball_CatchBallEvent;
		}

		private void ActivateEnergy()
		{
			activePlayer.ToggleEnergyBar(toggle: true);
			LeanTween.value(base.gameObject, activePlayer.UpdateEnergyAmount, 1f, 0f, controlParams.holdBallTime).setOnComplete(OutOfEnergy);
			float delayTime = controlParams.holdBallTime - controlParams.lowEnergyTime;
			LeanTween.delayedCall(base.gameObject, delayTime, LowEnergy);
		}

		private void OutOfEnergy()
		{
			activePlayer.SetOutOfEnergy();
			PlayerController.OutOfEnergyEvent();
		}

		private void LowEnergy()
		{
			PlayerController.LowEnergyEvent();
		}

		private void Kick()
		{
			if (_ball.state == Ball.State.Held && !(activePlayer == null))
			{
				LeanTween.cancel(base.gameObject);
				_ball.Kick(activePlayer);
				activePlayer.SetKick();
				activePlayer.ToggleEnergyBar(toggle: false);
				activePlayer = null;
			}
		}

		static PlayerController()
		{
			PlayerController.OutOfEnergyEvent = delegate
			{
			};
			PlayerController.LowEnergyEvent = delegate
			{
			};
			PlayerController.DragEvent = delegate
			{
			};
		}
	}
}
