using GamemakerSuperCasual;
using UnityEngine;

namespace Soccerpass
{
	public class TutorialManager : MonoBehaviour
	{
		public enum TutorialState
		{
			WaitForOpeningKick,
			OpeningKick,
			PressAndHold,
			DragToSteer,
			ReleaseToKick,
			WaitForGoal,
			Finished
		}

		public TutorialState state;

		public GameObject tutorialHUD;

		public StringVar tutorialMessage;

		private Ball _ball;

		private bool didSteer;

		private int numReleaseToKick;

		private bool IsStepMandatory
		{
			get
			{
				switch (state)
				{
				case TutorialState.DragToSteer:
					return false;
				case TutorialState.Finished:
					return true;
				case TutorialState.OpeningKick:
					return true;
				case TutorialState.PressAndHold:
					return true;
				case TutorialState.ReleaseToKick:
					return true;
				case TutorialState.WaitForGoal:
					return false;
				case TutorialState.WaitForOpeningKick:
					return true;
				default:
					return false;
				}
			}
		}

		public static bool IsTutorialDone
		{
			get
			{
				return TTPlayerPrefs.GetBool("tutorial_done");
			}
			set
			{
				TTPlayerPrefs.SetBool("tutorial_done", value);
				TTPlayerPrefs.Save();
			}
		}

		public void Init()
		{
			state = TutorialState.WaitForOpeningKick;
			_ball = UnityEngine.Object.FindObjectOfType<Ball>();
			AddListeners();
		}

		private void OnDestroy()
		{
			RemoveListeners();
		}

		private void AddListeners()
		{
			Ball.ConnectEvent += Ball_ConnectEvent;
			Ball.CatchBallEvent += Ball_CatchBallEvent;
			Ball.GoalEvent += Ball_GoalEvent;
			Ball.KickEvent += Ball_KickEvent;
			PlayerController.DragEvent += PlayerController_DragEvent;
			Ball.HitKickLineEvent += Ball_HitKickLineEvent;
		}

		private void Ball_HitKickLineEvent()
		{
			PSDKWrapper.LogTutorialStep((int)state, state.ToString(), TutorialStepStage.Complete, IsStepMandatory);
			state = TutorialState.WaitForGoal;
			ShowMessage("Release to Score!");
			PSDKWrapper.LogTutorialStep((int)state, state.ToString(), TutorialStepStage.Start, IsStepMandatory);
		}

		public void ShowMessage(string message)
		{
			tutorialMessage.value = message;
			tutorialHUD.SetActive(value: true);
		}

		public void HideMessage()
		{
			tutorialHUD.SetActive(value: false);
		}

		private void PlayerController_DragEvent(float amount)
		{
			if (!didSteer)
			{
				didSteer = true;
				if (state == TutorialState.DragToSteer)
				{
					PSDKWrapper.LogTutorialStep((int)state, state.ToString(), TutorialStepStage.Complete, IsStepMandatory);
				}
			}
			TutorialState tutorialState = state;
			if (tutorialState == TutorialState.DragToSteer)
			{
				tutorialHUD.SetActive(value: false);
			}
		}

		private void Ball_KickEvent(KickData obj)
		{
			switch (state)
			{
			case TutorialState.WaitForOpeningKick:
				state = TutorialState.OpeningKick;
				PSDKWrapper.LogTutorialStep((int)state, state.ToString(), TutorialStepStage.Start, IsStepMandatory);
				break;
			case TutorialState.ReleaseToKick:
				numReleaseToKick++;
				tutorialHUD.SetActive(value: false);
				PSDKWrapper.LogTutorialStep((int)state, state.ToString(), TutorialStepStage.Complete, IsStepMandatory);
				break;
			}
		}

		private void Ball_GoalEvent()
		{
			PSDKWrapper.LogTutorialStep((int)state, state.ToString(), TutorialStepStage.Complete, IsStepMandatory);
			state = TutorialState.Finished;
			tutorialHUD.SetActive(value: false);
			IsTutorialDone = true;
			RemoveListeners();
			PSDKWrapper.LogTutorialStep((int)state, state.ToString(), TutorialStepStage.Start, IsStepMandatory);
			PSDKWrapper.LogTutorialStep((int)state, state.ToString(), TutorialStepStage.Complete, IsStepMandatory);
		}

		private void Ball_CatchBallEvent(SoccerPlayer obj)
		{
			if (state != TutorialState.WaitForGoal)
			{
				tutorialHUD.SetActive(value: false);
				if (numReleaseToKick < 2)
				{
					PSDKWrapper.LogTutorialStep((int)state, state.ToString(), TutorialStepStage.Complete, IsStepMandatory);
					ShowMessage("Release to kick");
					state = TutorialState.ReleaseToKick;
					PSDKWrapper.LogTutorialStep((int)state, state.ToString(), TutorialStepStage.Start, IsStepMandatory);
				}
				else if (!didSteer)
				{
					PSDKWrapper.LogTutorialStep((int)state, state.ToString(), TutorialStepStage.Complete, IsStepMandatory);
					ShowMessage("Drag to steer");
					state = TutorialState.DragToSteer;
					PSDKWrapper.LogTutorialStep((int)state, state.ToString(), TutorialStepStage.Start, IsStepMandatory);
				}
			}
		}

		private void Ball_ConnectEvent()
		{
			if (state != TutorialState.WaitForGoal)
			{
				PSDKWrapper.LogTutorialStep((int)state, state.ToString(), TutorialStepStage.Complete, IsStepMandatory);
				state = TutorialState.PressAndHold;
				PSDKWrapper.LogTutorialStep((int)state, state.ToString(), TutorialStepStage.Start, IsStepMandatory);
				if (!_ball.kickData.isOpeningKick)
				{
					ShowMessage("Press & Hold");
				}
			}
		}

		private void RemoveListeners()
		{
			Ball.ConnectEvent -= Ball_ConnectEvent;
			Ball.CatchBallEvent -= Ball_CatchBallEvent;
			Ball.GoalEvent -= Ball_GoalEvent;
			Ball.KickEvent -= Ball_KickEvent;
			PlayerController.DragEvent -= PlayerController_DragEvent;
			Ball.HitKickLineEvent -= Ball_HitKickLineEvent;
		}
	}
}
