using com.gamemaker.soccerskillz;
using System;
using System.Collections.Generic;
using System.Linq;
using GamemakerSuperCasual;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Soccerpass
{
	public class GameManager : MonoBehaviour, IEventListener
	{
		public enum State
		{
			Menu,
			Playing,
			GameOver,
			Paused,
			Countdown
		}

		[Header("data")]
		public GameParams gameParams;

		public StringVar failReason;

		public Messages messages;

		public LevelState levelState;

		public PlayerControlParams controlParams;

		public HUDState hudState;

		public SessionState sessionState;

		[Header("scene")]
		public StatsManager statsManager;

		[Header("UI")]
		public EnergyFrame energyFrame;

		public GameObject countdownView;

		public List<RectTransform> iphoneXfixItems;

		public bool debugXfix;

		private Ball _ball;

		private TutorialManager _tutorialManager;

		private PlayersSpawnManager _playersSpawnManager;

		private FieldSpawnManager _fieldSpawnManager;

		private FieldElementsSpawnManager _fieldElementsSpawnManager;

		private PlayerController _playerController;

		private List<FollowTarget> _followTargetList;

		private State _state;

		private State _lastState;

		private bool ready;

		private void Start()
		{
			Advertisements.Instance.ShowBanner(BannerPosition.BOTTOM);
            sessionState.runCount++;
            hudState.value = HUDType.None;
            if (TTAudioManager.Instance == null)
            {
                SceneManager.LoadScene(0);
                return;
            }
            _tutorialManager = UnityEngine.Object.FindObjectOfType<TutorialManager>();
            if (!sessionState.isRevive)
            {
                levelState.levelIndex = controlParams.startLevel;
            }
            if (levelState.levelIndex == 0)
            {
                if (TutorialManager.IsTutorialDone)
                {
                    levelState.levelIndex = 1;
                }
                else
                {
                    _tutorialManager.Init();
                }
            }
            if (!sessionState.isRevive)
            {
                statsManager.ResetStats();
            }
            else
            {
                statsManager.OnRevive();
            }
            Physics.gravity = new Vector3(0f, gameParams.gravity, 0f);
            AddListeners();
            _ball = UnityEngine.Object.FindObjectOfType<Ball>();
            _state = State.Menu;
            _ball.Init();
            _playersSpawnManager = GetComponentInChildren<PlayersSpawnManager>();
            _fieldSpawnManager = GetComponentInChildren<FieldSpawnManager>();
            _fieldElementsSpawnManager = GetComponentInChildren<FieldElementsSpawnManager>();
            _playerController = GetComponentInChildren<PlayerController>();
            _followTargetList = UnityEngine.Object.FindObjectsOfType<FollowTarget>().ToList();
            _playersSpawnManager.Init(levelState.levelIndex);
            _fieldElementsSpawnManager.Init();
            _fieldSpawnManager.Init();
            _playerController.Init();
            Utils.ApplySafeArea(iphoneXfixItems, debugXfix);
            ready = true;
            GameReadyAfterSessionStartInterstital();
        }

		private void OnGameReady()
		{
			if (sessionState.runCount == 1)
			{
				PlayStartAudio();
			}
			if (!sessionState.isReload)
			{
				hudState.value = HUDType.TapToPlay;
			}
			else
			{
				hudState.value = HUDType.Replay;
			}
			PSDKWrapper.LogMainScreen();
			ready = true;
		}

		private void GameReadyAfterSessionStartInterstital()
		{
			if (Singleton<PSDKWrapper>.Instance.SessionStartPending && Singleton<PSDKWrapper>.Instance.SessionStartReady)
			{
				Singleton<PSDKWrapper>.Instance.ShowSessionStart().Done(delegate
				{
					OnGameReady();
				});
				return;
			}
			Singleton<PSDKWrapper>.Instance.ShowBanner();
			OnGameReady();
		}

		public void StartGame()
		{
			_state = State.Playing;
			if (!sessionState.isRevive)
			{
				statsManager.ResetStats();
			}
			if (StatsManager.runCount == 0)
			{
				PSDKWrapper.LogLevelUp(0);
			}
			statsManager.StartRun();
			PSDKWrapper.LogMissionStarted(sessionState.startRunType);
			_playersSpawnManager.UpdateActivePlayerForLevel(levelState.levelIndex);
			hudState.value = HUDType.Playing;
			this.PlayMusic(SoundId.crowd_ambiance_loop);
			this.PlaySFX(SoundId.start_game_kick);
			InputManager.inst.Reset();
			float delayTime = gameParams.openingKickDelay;
			if (sessionState.isReload || sessionState.isRevive)
			{
				delayTime = 0.1f;
			}
			LeanTween.delayedCall(base.gameObject, delayTime, (Action)delegate
			{
				_ball.OpeningKick();
			});
		}

		private void PlayStartAudio()
		{
			this.PlayMusic(SoundId.start_music);
			float soundLength = this.GetSoundLength(SoundId.start_music);
			this.PlayMusic(SoundId.bg_loop_music, PlayFlags.Loop, soundLength);
		}

		private void OnDestroy()
		{
			LeanTween.cancelAll();
			RemoveListeners();
		}

		private void Update()
		{
			if (!ready)
			{
				return;
			}
			if (_state != State.Paused)
			{
				_ball.OnUpdate();
			}
			State state = _state;
			if (state == State.Playing)
			{
				if (!_ball.IsOpeningKickInAir)
				{
					_playersSpawnManager.OnUpdate();
				}
				_playerController.OnUpdate();
				StatsManager obj = statsManager;
				Vector3 position = _ball.transform.position;
				obj.UpdateScoreByDistance(position.z);
			}
		}

		public void AddListeners()
		{
			Ball.HitFieldEvent += Ball_HitFieldEvent;
			Ball.TackleEvent += Ball_PlayerCollideEvent;
			Ball.FallEvent += Ball_FallEvent;
			PlayerController.OutOfEnergyEvent += PlayerController_OutOfEnergy;
			PlayerController.LowEnergyEvent += PlayerController_LowEnergyEvent;
			Ball.KickEvent += Ball_KickEvent;
			Ball.GoalEvent += Ball_GoalEvent;
			Ball.CatchBallEvent += Ball_CatchBallEvent;
			Ball.ConnectEvent += Ball_ConnectEvent;
			StatsManager.NewHighScoreEvent += StatsManager_NewHighScoreEvent;
			UserSettings.SettingsChangedEvent += UserSettings_SettingsChangedEvent;
			Ball.CrossCenterLineEvent += Ball_CrossCenterLineEvent;
			PSDKWrapper.PSDKReadyEvent += PSDKWrapper_PSDKReadyEvent;
			BackButtonListener.BackButtonEvent += HandleBackButton;
			AnimationStateMachine.StateCompleteEvent += AnimationStateMachine_StateComplete;
			PSDKWrapper.ReturnFromBackgroundNewSession += PSDKWrapper_ReturnFromBackgroundNewSession;
			PSDKWrapper.ReturnFromBackgroundRestartApp += PSDKWrapper_ReturnFromBackgroundRestartApp;
			PSDKWrapper.OnLocationLoaded += PSDKWrapper_OnLocationLoaded;
			StatsManager.NewMaxGoalsEvent += StatsManagerOnNewMaxGoalsEvent;
			ReloadSceneButton.SceneWillReloadEvent += ReloadSceneButtonOnSceneWillReloadEvent;
		}

		private void ReloadSceneButtonOnSceneWillReloadEvent()
		{
			PSDKWrapper.LogMissionAbandoned(sessionState.startRunType);
		}

		private void StatsManagerOnNewMaxGoalsEvent(int maxGoal)
		{
			PSDKWrapper.LogLevelUp(maxGoal);
		}

		private void PSDKWrapper_OnLocationLoaded(string locationId)
		{
			if (locationId == "sessionStart" && Singleton<PSDKWrapper>.Instance.SessionStartPending && (HUDType)hudState == HUDType.TapToPlay)
			{
				Singleton<PSDKWrapper>.Instance.ShowSessionStart();
			}
		}

		private void PSDKWrapper_ReturnFromBackgroundRestartApp()
		{
			Utils.LoadGameScene();
		}

		private void PSDKWrapper_ReturnFromBackgroundNewSession()
		{
			if (Singleton<PSDKWrapper>.Instance.SessionStartPending)
			{
				Singleton<PSDKWrapper>.Instance.ShowSessionStart();
			}
		}

		private void AnimationStateMachine_StateComplete(string id)
		{
			if (id == "Countdown")
			{
				LeanTween.resumeAll();
				_state = State.Playing;
				_ball.Resume();
				_playersSpawnManager.TogglePause(paused: false);
				countdownView.SetActive(value: false);
			}
		}

		private void HandleBackButton()
		{
			switch (_state)
			{
			case State.Playing:
			case State.Countdown:
				PauseGame();
				break;
			case State.Paused:
				switch (hudState.value)
				{
				case HUDType.Settings:
					hudState.value = HUDType.Paused;
					break;
				case HUDType.Paused:
					ResumeGame();
					break;
				}
				break;
			case State.Menu:
			{
				HUDType value = hudState.value;
				if (value == HUDType.Settings)
				{
					hudState.value = HUDType.TapToPlay;
				}
				else
				{
					Utils.Quit();
				}
				break;
			}
			case State.GameOver:
				_state = State.Menu;
				hudState.value = HUDType.TapToPlay;
				break;
			}
		}

		private void OnApplicationPause(bool paused)
		{
			State state = _state;
			if ((state == State.Playing || state == State.Countdown) && paused)
			{
				PauseGame();
			}
		}

		private void PauseGame()
		{
			statsManager.inRun = false;
			LeanTween.pauseAll();
			countdownView.SetActive(value: false);
			_lastState = _state;
			_state = State.Paused;
			if (_lastState == State.Playing)
			{
				_ball.Pause();
				_playersSpawnManager.TogglePause(paused: true);
			}
			hudState.value = HUDType.Paused;
			Advertisements.Instance.ShowInterstitial(()=> Debug.Log("Interstitial Closed"));
		}

		private void ResumeGame()
		{
			statsManager.inRun = true;
			hudState.value = HUDType.Playing;
			SetCountdown();
		}

		private void SetCountdown()
		{
			_state = State.Countdown;
			countdownView.SetActive(value: true);
		}

		public void ToggleSettings(bool toggle)
		{
			if (toggle)
			{
				hudState.value = HUDType.Settings;
			}
			else
			{
				HandleBackButton();
			}
		}

		private void PSDKWrapper_PSDKReadyEvent()
		{
			PSDKWrapper.PSDKReadyEvent -= PSDKWrapper_PSDKReadyEvent;
			GameReadyAfterSessionStartInterstital();
		}

		private void Ball_CrossCenterLineEvent()
		{
			this.PlayMusic(SoundId.crowd_excited);
		}

		private void PlayerController_LowEnergyEvent()
		{
			energyFrame.Show(EnergyFrame.Mode.Low);
		}

		private void UserSettings_SettingsChangedEvent(SettingType type, bool value)
		{
			if (value && (type == SettingType.Sound || type == SettingType.Music) && !TTAudioManager.Instance.IsPlayingMusic(SoundId.bg_loop_music))
			{
				this.PlayMusic(SoundId.bg_loop_music, PlayFlags.Loop);
			}
		}

		private void StatsManager_NewHighScoreEvent(int obj)
		{
			this.PlaySFX(SoundId.new_highscore);
		}

		private void Ball_ConnectEvent()
		{
		}

		private void Ball_CatchBallEvent(SoccerPlayer obj)
		{
			this.PlaySFX(SoundId.catch_ball);
			if (TutorialManager.IsTutorialDone)
			{
				_tutorialManager.HideMessage();
			}
			statsManager.CountPass();
		}

		private void Ball_GoalEvent()
		{
			if (_state == State.Playing)
			{
				this.PlaySFX(SoundId.goal);
				hudState.value = HUDType.Goal;
				LeanTween.delayedCall(base.gameObject, 1.5f, (Action)delegate
				{
					hudState.value = HUDType.Playing;
				});
				statsManager.CountGoal();
				PSDKWrapper.LogMissionLevelUp(sessionState.startRunType);
				Utils.Vibrate();
				if (levelState.config.levels.Count > levelState.levelIndex + 1)
				{
					levelState.levelIndex++;
				}
				_playersSpawnManager.UpdateActivePlayerForLevel(levelState.levelIndex);
				LeanTween.delayedCall(base.gameObject, 0.5f, (Action)delegate
				{
					this.StopMusic(SoundId.crowd_excited);
					_state = State.Playing;
				});
			}
		}

		private void Ball_KickEvent(KickData kickData)
		{
			this.PlaySFX(SoundId.kick);
			if (TutorialManager.IsTutorialDone)
			{
				_tutorialManager.HideMessage();
			}
			energyFrame.Hide();
		}

		private void Ball_PlayerCollideEvent(SoccerPlayer player, FailureType failType)
		{
			if (player.IsGoalkeeper)
			{
				switch (failType)
				{
				case FailureType.Tackle:
					failType = FailureType.Block;
					break;
				case FailureType.BadPass:
					failType = FailureType.Save;
					break;
				}
			}
			GameOver(failType, player.data.playerName);
		}

		private void PlayerController_OutOfEnergy()
		{
			energyFrame.Show(EnergyFrame.Mode.No);
		}

		private void Ball_FallEvent()
		{
			GameOver(FailureType.Out, string.Empty);
		}

		private void Ball_HitFieldEvent()
		{
		}

		private void GameOver(FailureType failureType, string message = "")
		{
			if (_state == State.Playing)
			{
				_tutorialManager.tutorialHUD.SetActive(value: false);
				statsManager.inRun = false;
				this.StopMusic(SoundId.crowd_excited);
				TTAudioManager.Instance.StopMusic(SoundId.crowd_ambiance_loop);
				_followTargetList.ForEach(delegate(FollowTarget s)
				{
					s.enabled = false;
				});
				Utils.Vibrate();
				string messageByType = messages.GetMessageByType(failureType);
				failReason.value = string.Format(messageByType, message);
				_playersSpawnManager.OnGameOver();
				_ball.OnGameOver();
				this.PlaySFX(SoundId.dead);
				_state = State.GameOver;
				hudState.value = HUDType.Fail;
				LeanTween.delayedCall(base.gameObject, 2f, (Action)delegate
				{
					ShowGameOverScreen(failureType);
				});
			}
		}

		private void ShowGameOverScreen(FailureType failureType)
		{
			_tutorialManager.tutorialHUD.SetActive(value: false);
			PSDKWrapper.LogMissionFailed(failureType.ToString(), sessionState.startRunType);
			Singleton<PSDKWrapper>.Instance.ShowInterstitialFail(delegate
            {
                energyFrame.Hide();
                Singleton<PSDKWrapper>.Instance.RequestUpdateLeaderboard(statsManager.highscore);
                if (Singleton<PSDKWrapper>.Instance.IsRVReady() && statsManager.CanRevive)
                {
                    hudState.value = HUDType.ReviveOffer;
                }
                else
                {
                    hudState.value = HUDType.GameOver;
                    Advertisements.Instance.ShowInterstitial(()=> Debug.Log("Interstitial Closed"));
                }
            });
		}

		public void RemoveListeners()
		{
			Ball.HitFieldEvent -= Ball_HitFieldEvent;
			Ball.FallEvent -= Ball_FallEvent;
			PlayerController.OutOfEnergyEvent -= PlayerController_OutOfEnergy;
			Ball.TackleEvent -= Ball_PlayerCollideEvent;
			Ball.KickEvent -= Ball_KickEvent;
			Ball.GoalEvent -= Ball_GoalEvent;
			Ball.CatchBallEvent -= Ball_CatchBallEvent;
			Ball.ConnectEvent -= Ball_ConnectEvent;
			StatsManager.NewHighScoreEvent -= StatsManager_NewHighScoreEvent;
			UserSettings.SettingsChangedEvent -= UserSettings_SettingsChangedEvent;
			PlayerController.LowEnergyEvent -= PlayerController_LowEnergyEvent;
			Ball.CrossCenterLineEvent -= Ball_CrossCenterLineEvent;
			PSDKWrapper.PSDKReadyEvent -= PSDKWrapper_PSDKReadyEvent;
			BackButtonListener.BackButtonEvent -= HandleBackButton;
			AnimationStateMachine.StateCompleteEvent -= AnimationStateMachine_StateComplete;
			PSDKWrapper.ReturnFromBackgroundNewSession -= PSDKWrapper_ReturnFromBackgroundNewSession;
			PSDKWrapper.ReturnFromBackgroundRestartApp -= PSDKWrapper_ReturnFromBackgroundRestartApp;
			PSDKWrapper.OnLocationLoaded -= PSDKWrapper_OnLocationLoaded;
			StatsManager.NewMaxGoalsEvent -= StatsManagerOnNewMaxGoalsEvent;
			ReloadSceneButton.SceneWillReloadEvent -= ReloadSceneButtonOnSceneWillReloadEvent;
		}
	}
}
