using com.gamemaker.soccerskillz;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GamemakerSuperCasual;
using UnityEngine;

namespace Soccerpass
{
//	[RequireComponent(typeof(Rigidbody))]
	public class Ball : MonoBehaviour, IEventListener
	{
		public enum State
		{
			Init,
			Held,
			KickStart,
			InAir,
			Connecting,
			Ground,
			Out
		}

		[Header("Data")]
		public GameParams gameParams;

		public KickData kickData;

		[Header("Views")]
		public BallRadar airRadar;

		public TrailRenderer trail;

		public LayerMask goalMask;

		public bool invincible = true;

		public State state;

		private TutorialManager _tutorialManager;

		private Color _trailDefaultColor;

		private readonly List<SoccerPlayer> candidates = new List<SoccerPlayer>();

		private SoccerPlayer _currentPlayer;

		private PlayersSpawnManager _spawnManager;

		private FieldElementsSpawnManager _fieldElementsManager;

		private Rigidbody _rb;

		private Transform _originalParent;

		private Collider _collider;

		private float _lastTimeScale = 1f;

		private int _slowDownTimeID;

		private float _touchGroundTime = -1f;

		[CompilerGenerated]
		private static Action<float> _003C_003Ef__mg_0024cache0;

		public bool IsOpeningKickInAir => kickData.isOpeningKick && state == State.InAir;

		public Vector3 GroundPosition
		{
			get
			{
				Vector3 position = base.transform.position;
				position.y = 0f;
				return position;
			}
		}

		public bool CanConnectToPlayer => (state == State.InAir || state == State.Ground) && NextPlayer != null;

		public SoccerPlayer NextPlayer
		{
			get
			{
				if (candidates.IsNullOrEmpty())
				{
					return null;
				}
				return candidates[0];
			}
		}

		private float TimeToGround
		{
			get
			{
				float num = 2f * DistanceToGround;
				Vector3 gravity = Physics.gravity;
				float f = num / Mathf.Abs(gravity.y) / Time.timeScale;
				return Mathf.Sqrt(f);
			}
		}

		private float DistanceToGround
		{
			get
			{
				Vector3 position = base.transform.position;
				float y = position.y;
				Vector3 groundPosition = GroundPosition;
				float num = y - groundPosition.y;
				float num2 = num;
				Vector3 size = _collider.bounds.size;
				return num2 - size.y / 2f;
			}
		}

		private float KickTime
		{
			get
			{
				float num = 2f * DistanceToGround;
				Vector3 gravity = Physics.gravity;
				float f = num / Mathf.Abs(gravity.y) / Time.timeScale;
				return Mathf.Sqrt(f);
			}
		}

		public Vector3 Velocity
		{
			get
			{
				if (state == State.InAir || _currentPlayer == null)
				{
					return _rb.velocity;
				}
				return _currentPlayer.Velocity;
			}
		}

		public bool IsNearGoal
		{
			get
			{
				Vector3 position = _fieldElementsManager.nextGoal.transform.position;
				float z = position.z;
				Vector3 groundPosition = GroundPosition;
				if (z < groundPosition.z)
				{
					return false;
				}
				return DistanceFromGoal <= gameParams.goalKickDistance;
			}
		}

		public float DistanceFromGoal => Vector3.Distance(_fieldElementsManager.nextGoal.position, GroundPosition);

		public static event Action<KickData> KickEvent;

		public static event Action<SoccerPlayer> CatchBallEvent;

		public static event Action HitFieldEvent;

		public static event Action<SoccerPlayer, FailureType> TackleEvent;

		public static event Action FallEvent;

		public static event Action GoalEvent;

		public static event Action HitKickLineEvent;

		public static event Action CrossCenterLineEvent;

		public static event Action ConnectEvent;

		public bool IsCandidate(SoccerPlayer p)
		{
			return candidates != null && candidates.Contains(p);
		}

		private void Awake()
		{
			_rb = GetComponent<Rigidbody>();
			_collider = GetComponent<Collider>();
			_spawnManager = UnityEngine.Object.FindObjectOfType<PlayersSpawnManager>();
			_fieldElementsManager = UnityEngine.Object.FindObjectOfType<FieldElementsSpawnManager>();
			_tutorialManager = UnityEngine.Object.FindObjectOfType<TutorialManager>();
			_trailDefaultColor = trail.startColor;
		}

		public void Init()
		{
			_currentPlayer = null;
			candidates.Clear();
			airRadar.Toggle(active: false);
			trail.enabled = false;
			state = State.Init;
			_originalParent = base.transform.parent;
		}

		private void Start()
		{
			AddListeners();
		}

		private void OnDestroy()
		{
			RemoveListeners();
		}

		public void Pause()
		{
			_lastTimeScale = Time.timeScale;
			Utils.UpdateTimeScale(1f);
			_rb.Pause();
		}

		public void Resume()
		{
			_rb.UnPause();
			Utils.UpdateTimeScale(_lastTimeScale);
		}

		public void OnUpdate()
		{
			if (state == State.InAir)
			{
				Vector3 position = base.transform.position;
				if (position.y < -10f)
				{
					Out();
					return;
				}
				Vector3 velocity = _rb.velocity;
				if (velocity.y < 0f && NextPlayer != null)
				{
					float timeToGround = TimeToGround;
					if (Time.timeScale == gameParams.fastTimeScale)
					{
						if (timeToGround <= gameParams.timeToTriggerSlowScale)
						{
							if (TutorialManager.IsTutorialDone)
							{
								Utils.UpdateTimeScale(gameParams.slowTimeScale);
							}
						}
						else if (timeToGround < gameParams.timeToTriggerNormalScale)
						{
							Utils.UpdateTimeScale(gameParams.normalTimeScale);
						}
					}
					else if (Time.timeScale == gameParams.normalTimeScale && timeToGround <= gameParams.timeToTriggerSlowScale && TutorialManager.IsTutorialDone)
					{
						if (kickData.isOpeningKick)
						{
							Utils.UpdateTimeScale(gameParams.slowTimeScale * 0.5f);
						}
						else
						{
							Utils.UpdateTimeScale(gameParams.slowTimeScale);
						}
					}
				}
				if (InputManager.isMouseDown)
				{
					TryToConnect();
				}
			}
			else if (state == State.Ground && Velocity.magnitude < 10f && NextPlayer == null && Time.time - _touchGroundTime > gameParams.MaxTimeOnGround)
			{
				Out();
			}
		}

		public void AddListeners()
		{
			airRadar.TriggerEnterE += OnEnterAirRadar;
			airRadar.TriggerExitE += OnExitAirRadar;
		}

		public void TryToConnect()
		{
			if (CanConnectToPlayer)
			{
				ConnectToPlayer(NextPlayer);
			}
		}

		public void RemoveListeners()
		{
			airRadar.TriggerEnterE -= OnEnterAirRadar;
			airRadar.TriggerExitE -= OnExitAirRadar;
		}

		public void Kick(SoccerPlayer player)
		{
			bool isNearGoal = IsNearGoal;
			bool flag = player.HasHomePlayersInView();
			if ((IsNearGoal && !flag) || DistanceFromGoal < gameParams.goalKickDistance / 5f)
			{
				KickToGoal(player);
				return;
			}
			Quaternion rotation = player.playerBody.rotation;
			float forwardSpeed = player.controlParams.forwardSpeed;
			float num = gameParams.kickForce;
			if (isNearGoal && flag && DistanceFromGoal < gameParams.goalKickDistance / 2f)
			{
				num /= 2f;
			}
			Vector3 kickForce = GetKickForce(rotation, forwardSpeed, num);
			DoKick(kickForce);
			trail.startColor = _trailDefaultColor;
		}

		public void OpeningKick()
		{
			Vector3 position = _spawnManager.closestPlayerToBall.position;
			float z = position.z;
			Quaternion identity = Quaternion.identity;
			if (_currentPlayer != null)
			{
			}
			float num = gameParams.kickForce + 20f;
			float num2 = CalculateForwardForce(num, z);
			num2 += 10f;
			Vector3 kickForce = GetKickForce(identity, num2, num);
			kickData.isOpeningKick = true;
			DoKick(kickForce);
			trail.startColor = Color.yellow;
		}

		private void KickToGoal(SoccerPlayer activePlayer)
		{
			Quaternion rotation = Quaternion.identity;
			if (activePlayer != null)
			{
				rotation = activePlayer.playerBody.localRotation;
			}
			float num = gameParams.kickForce * 2f / 5f;
			Vector3 position = _fieldElementsManager.nextGoal.transform.position;
			float distance = position.z + gameParams.firstFormationAfterGoal;
			float speed = CalculateForwardForce(num, distance);
			Vector3 kickForce = GetKickForce(rotation, speed, num);
			kickData.isGoalKick = true;
			kickData.goalPos = _fieldElementsManager.nextGoal.transform.position;
			DoKick(kickForce, scaleTime: false);
			trail.startColor = Color.yellow;
		}

		private void DoKick(Vector3 kickForce, bool scaleTime = true)
		{
			LeanTween.cancel(base.gameObject);
			_rb.isKinematic = false;
			_rb.AddForce(kickForce, ForceMode.Impulse);
			if (scaleTime)
			{
				ScaleTime(gameParams.fastTimeScale);
			}
			kickData.origin = base.transform.position;
			kickData.destination = CalculateLandingPosition(kickForce);
			kickData.kicker = _currentPlayer;
			kickData.active = true;
			SetKickState();
			Ball.KickEvent(kickData);
		}

		private Vector3 GetKickForce(Quaternion rotation, float speed, float yForce)
		{
			Vector3 result = rotation * Vector3.forward * speed;
			result.y = yForce;
			return result;
		}

		private float CalculateForwardForce(float kickYForce, float distance)
		{
			Vector3 gravity = Physics.gravity;
			float num = 0f - gravity.y;
			float num2 = 2f * kickYForce / num / _rb.mass;
			Vector3 position = base.transform.position;
			return (distance - position.z) / num2;
		}

		private Vector3 CalculateLandingPosition(Vector3 kickForce)
		{
			Vector3 gravity = Physics.gravity;
			float num = 0f - gravity.y;
			float num2 = 2f * kickForce.y / num / _rb.mass;
			kickData.duration = num2;
			Vector3 position = base.transform.position;
			float num3 = position.x + kickForce.x * num2;
			Vector3 position2 = base.transform.position;
			float z = position2.z + kickForce.z * num2;
			float x = num3;
			Vector3 position3 = base.transform.position;
			return new Vector3(x, position3.y, z);
		}

		public void ReleaseBallUnintended(SoccerPlayer player)
		{
			Quaternion rotation = player.playerBody.rotation;
			float speed = player.controlParams.forwardSpeed / 2f;
			float yForce = gameParams.kickForce / 5f;
			rotation.SetLookRotation(base.transform.position + Vector3.forward);
			Vector3 kickForce = GetKickForce(rotation, speed, yForce);
			DoKick(kickForce);
		}

		private void SetKickState()
		{
			candidates.Clear();
			base.transform.SetParent(_originalParent);
			_rb.useGravity = true;
			trail.enabled = true;
			state = State.KickStart;
			LeanTween.delayedCall(base.gameObject, 0.5f, (Action)delegate
			{
				state = State.InAir;
			});
			float delayTime = 0.5f;
			if (kickData.isOpeningKick)
			{
				delayTime = 0.9f;
			}
			else if (kickData.isGoalKick)
			{
				delayTime = 1.2f;
			}
			LeanTween.delayedCall(base.gameObject, delayTime, (Action)delegate
			{
				airRadar.Toggle(active: true);
			});
			if (kickData.isOpeningKick)
			{
				float num = kickData.duration * 6f / 7f;
				float delayTime2 = num * 3f / 4f;
				if (!TutorialManager.IsTutorialDone)
				{
					LeanTween.delayedCall(base.gameObject, num, (Action)delegate
					{
						if (!InputManager.isMouseDown)
						{
							Utils.UpdateTimeScale(gameParams.slowTimeScale / 7f);
						}
					});
				}
				LeanTween.delayedCall(base.gameObject, delayTime2, (Action)delegate
				{
					if (!InputManager.isMouseDown)
					{
						_tutorialManager.ShowMessage("Tap & Hold");
					}
				});
			}
		}

		private void ScaleTime(float target, float animationDuration = 0.1f)
		{
			LeanTween.cancel(_slowDownTimeID);
			float timeScale = Time.timeScale;
			LTDescr lTDescr = LeanTween.value(base.gameObject, Utils.UpdateTimeScale, timeScale, target, animationDuration);
			_slowDownTimeID = lTDescr.uniqueId;
		}

		private void PlayersCollisionHeld(SoccerPlayer otherPlayer)
		{
			if (!otherPlayer.data.isHomeTeam && !invincible)
			{
				_currentPlayer.SetFall();
				Vector3 force = otherPlayer.playerBody.rotation * Vector3.forward * otherPlayer.Speed;
				force.y = 0f;
				_rb.isKinematic = false;
				_rb.AddForce(force, ForceMode.Impulse);
				OnTackle(otherPlayer, FailureType.Tackle);
			}
		}

		private void PlayerCollisionOnGround(SoccerPlayer otherPlayer)
		{
			if (!CanCatchBall(otherPlayer) && !otherPlayer.data.isHomeTeam)
			{
				_rb.velocity = Vector3.zero;
				OnTackle(otherPlayer, FailureType.BadPass);
			}
		}

		private void PlayerCollisionInAir(SoccerPlayer otherPlayer)
		{
			if (!otherPlayer.data.isHomeTeam)
			{
				_rb.velocity = Vector3.zero;
				OnTackle(otherPlayer, FailureType.BadPass);
				if (otherPlayer.IsGoalkeeper && otherPlayer.state != SoccerPlayer.State.GoalJump)
				{
					otherPlayer.SetGoalCatch();
				}
			}
		}

		private void OnTackle(SoccerPlayer player, FailureType type)
		{
			airRadar.Toggle(active: false);
			LeanTween.cancel(base.gameObject);
			Ball.TackleEvent(player, type);
		}

		private void CollideWithPlayer(SoccerPlayer otherPlayer)
		{
			switch (state)
			{
			case State.Held:
				PlayersCollisionHeld(otherPlayer);
				break;
			case State.Ground:
				PlayerCollisionOnGround(otherPlayer);
				break;
			case State.InAir:
				PlayerCollisionInAir(otherPlayer);
				break;
			}
		}

		private void SetHeld()
		{
			candidates.Clear();
			kickData.Reset();
			Utils.UpdateTimeScale(1f);
			base.transform.SetParent(_currentPlayer.ballHolder);
			base.transform.localPosition = Vector3.zero;
			_rb.isKinematic = false;
			airRadar.Toggle(active: false, animated: false);
			_rb.useGravity = false;
			state = State.Held;
			_currentPlayer.state = SoccerPlayer.State.WithBall;
			trail.enabled = false;
		}

		private bool CanCatchBall(SoccerPlayer otherPlayer)
		{
			if (_currentPlayer == null)
			{
				return otherPlayer.data.isHomeTeam && otherPlayer.state != SoccerPlayer.State.WithBall && otherPlayer != kickData.kicker;
			}
			return otherPlayer.data.isHomeTeam && otherPlayer != _currentPlayer && otherPlayer.state != SoccerPlayer.State.WithBall && otherPlayer != kickData.kicker;
		}

		private int CompareDistanceToBall(SoccerPlayer a, SoccerPlayer b)
		{
			float value = Vector3.Distance(GroundPosition, a.transform.position);
			return Vector3.Distance(GroundPosition, b.transform.position).CompareTo(value);
		}

		private void OnEnterAirRadar(GameObject other)
		{
			if (!other.CompareTag("Player"))
			{
				return;
			}
			SoccerPlayer component = other.GetComponent<SoccerPlayer>();
			if (component == _currentPlayer || component == kickData.kicker || candidates.Contains(component))
			{
				return;
			}
			State state = this.state;
			if ((state == State.InAir || state == State.Init || state == State.Ground) && CanCatchBall(component))
			{
				if (NextPlayer != null)
				{
					NextPlayer.ToggleCatching(toggle: false);
				}
				candidates.Add(component);
				candidates.Sort(CompareDistanceToBall);
				NextPlayer.ToggleCatching(toggle: true);
				Ball.ConnectEvent();
			}
		}

		private void OnExitAirRadar(GameObject other)
		{
			if (!other.CompareTag("Player"))
			{
				return;
			}
			SoccerPlayer component = other.GetComponent<SoccerPlayer>();
			if (component == null)
			{
				return;
			}
			component.ToggleCatching(toggle: false);
			candidates.Remove(component);
			candidates.Sort(CompareDistanceToBall);
			if (NextPlayer == null)
			{
				switch (state)
				{
				case State.Ground:
					Utils.UpdateTimeScale(1f);
					break;
				case State.InAir:
					Utils.UpdateTimeScale(gameParams.fastTimeScale);
					break;
				}
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.gameObject.CompareTag("Goal"))
			{
				switch (state)
				{
				}
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.gameObject.CompareTag("Player"))
			{
				SoccerPlayer component = other.gameObject.GetComponent<SoccerPlayer>();
				if (component != _currentPlayer)
				{
					State state = this.state;
					if (state == State.Held || state == State.Ground || state == State.InAir)
					{
						CollideWithPlayer(component);
					}
				}
			}
			else if (other.gameObject.CompareTag("Goal"))
			{
				switch (this.state)
				{
				case State.Held:
				case State.KickStart:
				case State.InAir:
				case State.Ground:
					OnGoal();
					break;
				default:
					Out();
					break;
				}
			}
			else if (other.CompareTag("GoalKickLine"))
			{
				OnHitKickLine();
			}
			else if (other.CompareTag("CenterLine"))
			{
				Ball.CrossCenterLineEvent();
			}
		}

		private void OnCollisionEnter(Collision collision)
		{
			bool flag = collision.gameObject.CompareTag("Fence");
			bool flag2 = collision.gameObject.CompareTag("Player");
			bool flag3 = collision.gameObject.CompareTag("Crowd");
			bool flag4 = collision.gameObject.CompareTag("Fail");
			bool flag5 = collision.gameObject.CompareTag("Field");
			bool flag6 = collision.gameObject.CompareTag("GoalPost");
			switch (state)
			{
			case State.Out:
				return;
			case State.Held:
				if (flag)
				{
					Out();
				}
				else if (flag2)
				{
					SoccerPlayer component = collision.gameObject.GetComponent<SoccerPlayer>();
					if (component != _currentPlayer)
					{
						Ball.TackleEvent(component, FailureType.Tackle);
					}
				}
				break;
			case State.Ground:
				if (flag2)
				{
					SoccerPlayer component2 = collision.gameObject.GetComponent<SoccerPlayer>();
					if (component2 != _currentPlayer)
					{
						Ball.TackleEvent(component2, FailureType.Tackle);
					}
				}
				else if (flag && kickData.isGoalKick && !kickData.didScore)
				{
					Out();
				}
				break;
			case State.InAir:
				if (flag5)
				{
					SetGround();
				}
				else if (flag && kickData.isGoalKick && !kickData.didScore)
				{
					Out();
				}
				else if (flag6)
				{
					ContactPoint contactPoint = collision.contacts[0];
					Vector3 normal = contactPoint.normal;
					if (normal.z <= -0.05f)
					{
						Out();
					}
				}
				break;
			}
			if (flag4 || flag3)
			{
				Out();
			}
		}

		private void OnGoal()
		{
			if (_rb.velocity.magnitude > 150f)
			{
				_rb.velocity *= 0.3f;
			}
			else
			{
				_rb.velocity *= 0.7f;
			}
			kickData.didScore = true;
			ScaleTime(gameParams.goalTimeScale);
			Ball.GoalEvent();
			LeanTween.delayedCall(base.gameObject, 0.5f, ConnectToClosestPlayer);
		}

		private void ConnectToClosestPlayer()
		{
			Utils.UpdateTimeScale(1f);
		}

		private void OnHitKickLine()
		{
			if (_currentPlayer != null && _currentPlayer.state == SoccerPlayer.State.WithBall)
			{
				_currentPlayer.ToggleSpecialGlow();
			}
			Ball.HitKickLineEvent();
		}

		private void Out()
		{
			Utils.UpdateTimeScale(1f);
			airRadar.Toggle(active: false, animated: false);
			state = State.Out;
			if (_currentPlayer != null)
			{
				_currentPlayer.SetIdle(rotateToBall: true);
			}
			Ball.FallEvent();
		}

		private void SetGround()
		{
			Utils.UpdateTimeScale(1f);
			state = State.Ground;
			Ball.HitFieldEvent();
			_touchGroundTime = Time.time;
		}

		private void ConnectToPlayer(SoccerPlayer player)
		{
			if (state != State.Connecting)
			{
				airRadar.Toggle(active: false);
				_currentPlayer = player;
				candidates.Remove(_currentPlayer);
				candidates.ForEach(delegate(SoccerPlayer c)
				{
					c.MarkAsAvailable(highlight: false);
				});
				candidates.Clear();
				LeanTween.cancel(base.gameObject);
				Utils.UpdateTimeScale(1f);
				_currentPlayer.state = SoccerPlayer.State.Connecting;
				Ball.CatchBallEvent(_currentPlayer);
				_rb.isKinematic = true;
				LeanTween.move(base.gameObject, _currentPlayer.ballHolder, 0.2f).setEaseInOutSine().setOnComplete(SetHeld);
				state = State.Connecting;
			}
		}

		public void OnGameOver()
		{
			GetComponentInChildren<Rotate>().speed /= 10f;
		}

		static Ball()
		{
			Ball.KickEvent = delegate
			{
			};
			Ball.CatchBallEvent = delegate
			{
			};
			Ball.HitFieldEvent = delegate
			{
			};
			Ball.TackleEvent = delegate
			{
			};
			Ball.FallEvent = delegate
			{
			};
			Ball.GoalEvent = delegate
			{
			};
			Ball.HitKickLineEvent = delegate
			{
			};
			Ball.CrossCenterLineEvent = delegate
			{
			};
			Ball.ConnectEvent = delegate
			{
			};
		}
	}
}
