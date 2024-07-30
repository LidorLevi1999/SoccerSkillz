using com.gamemaker.soccerskillz;
using System;
using System.Collections.Generic;
using System.Linq;
using GamemakerSuperCasual;
using GamemakerSuperCasual.Runtime.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Soccerpass
{
	public class SoccerPlayer : MonoBehaviour
	{
		private struct DistanceData
		{
			public bool isInActivationDistance;

			public bool isInFollowPlayerDistance;

			public bool isInFollowBallDistance;

			public bool isInFollowGroundBallDistance;

			public bool isInGlitchDistance;

			public bool isInJumpDistance;
		}

		private static class AnimParams
		{
			public const string SaveLeft = "SaveLeft";

			public const string SaveRight = "SaveRight";

			public const string Idle = "Idle";

			public const string SideWalk = "SideWalk";

			public const string RunSpeed = "RunSpeed";

			public const string hOffset = "hOffset";

			public const string Tackle = "Tackle";

			public const string Kick = "Kick";

			public const string Fall = "Fall";

			public const string Catch = "Catch";
		}

		public enum State
		{
			Init,
			Idle,
			WithBall,
			Active,
			FollowingBall,
			Glitch,
			GoalJump,
			Recovering,
			Kick,
			Connecting,
			Fall
		}

		[Header("Data")]
		public PlayerControlParams controlParams;

		public PlayerData data;

		public State state;

		public QualityData qualityData;

		[Header("Views")]
		public GameObject UI;

		public Text playerName;

		public Text playerLevel;

		public Transform playerBody;

		public Image energyImage;

		public Renderer modelRenderer;

		public Color highlightTextColor = Color.cyan;

		public SpriteRenderer highlight;

		public TrailRenderer trail;

		private FieldOfView _fov;

		private Animator _animator;

		private float _speedMultiplier;

		private Ball _ball;

		private float _energyAmount;

		private Vector3 _orgPosition;

		private float _rotateAngle;

		private DistanceData _distanceData;

		private Rect _levelBounds;

		private bool _highlighted;

		private float _lastAnimSpeed;

		public Transform ballHolder
		{
			get;
			private set;
		}

		public bool IsGoalkeeper => data.playerRole == PlayerRole.GoalKeeper;

		public Vector3 Velocity
		{
			get
			{
				State state = this.state;
				if (state == State.WithBall)
				{
					Quaternion localRotation = playerBody.localRotation;
					float x = Speed * Mathf.Sin(localRotation.y);
					float z = Speed * Mathf.Cos(localRotation.y);
					return new Vector3(x, 0f, z);
				}
				return Vector3.zero;
			}
		}

		public float Speed
		{
			get;
			private set;
		}

		public float GlitchSpeed
		{
			get;
			private set;
		}

		private float DistanceFromBall
		{
			get
			{
				Vector3 position = _ball.transform.position;
				position.y = 0f;
				return Vector3.Distance(base.transform.position, position);
			}
		}

		private bool CanFollowBall
		{
			get
			{
				switch (_ball.state)
				{
				case Ball.State.Ground:
					if (!data.isHomeTeam)
					{
						return _distanceData.isInFollowGroundBallDistance;
					}
					if (_ball.NextPlayer == null)
					{
						return _distanceData.isInFollowGroundBallDistance;
					}
					if (_ball.NextPlayer != this)
					{
						return false;
					}
					return _distanceData.isInFollowGroundBallDistance;
				case Ball.State.Held:
					if (!data.isHomeTeam)
					{
						return _distanceData.isInFollowPlayerDistance;
					}
					return false;
				case Ball.State.InAir:
					if (_distanceData.isInFollowBallDistance)
					{
						if (data.isHomeTeam)
						{
							return _ball.NextPlayer == null;
						}
						if (data.playerRole == PlayerRole.GoalKeeper)
						{
							return true;
						}
						break;
					}
					return false;
				}
				return false;
			}
		}

		private bool CanGlitch
		{
			get
			{
				if (!_distanceData.isInGlitchDistance)
				{
					return false;
				}
				if (data.isHomeTeam)
				{
					return false;
				}
				return data.CanGlitch && _ball.state == Ball.State.Held;
			}
		}

		private bool CanJump => data.CanJump && _distanceData.isInJumpDistance && (_ball.state == Ball.State.InAir || _ball.state == Ball.State.Ground) && _ball.kickData.isGoalKick;

		private Vector3 FollowTarget
		{
			get
			{
				Vector3 result = (_ball.state != Ball.State.InAir) ? _ball.GroundPosition : _ball.kickData.destination;
				Vector3 position = base.transform.position;
				result.y = position.y;
				return result;
			}
		}

		public static event Action<SoccerPlayer> DisableEvent;

		public bool IsActiveInLevel(int level)
		{
			return state == State.WithBall || data.fieldLevel == level;
		}

		public bool HasHomePlayersInView()
		{
			List<Transform> visibleTargets = _fov.visibleTargets;
			IEnumerable<SoccerPlayer> source = from t in visibleTargets
				select t.GetComponent<SoccerPlayer>();
			IEnumerable<SoccerPlayer> source2 = from p in source
				where p != null && p.data != null && p.data.isHomeTeam
				select p;
			return source2.Any();
		}

		private void Awake()
		{
			if (ballHolder == null)
			{
				ballHolder = playerBody.Find("BallTarget");
			}
			trail.enabled = false;
			state = State.Init;
			_animator = GetComponentInChildren<Animator>();
			_fov = GetComponentInChildren<FieldOfView>();
		}

		public void OnGameOver()
		{
			LeanTween.cancel(base.gameObject);
			State state = this.state;
			if (state == State.FollowingBall || state == State.Active)
			{
				SetRecovering();
			}
		}

		private void OnEnable()
		{
			if (IsGoalkeeper)
			{
				AnimationStateMachine.StateCompleteEvent += AnimationStateMachineOnStateCompleteEvent;
			}
		}

		private void AnimationStateMachineOnStateCompleteEvent(string state_id)
		{
			if (state_id == "goalkeeper_save" && state == State.GoalJump)
			{
				SetIdle(rotateToBall: true);
			}
		}

		private void OnDisable()
		{
			LeanTween.cancel(base.gameObject);
			SoccerPlayer.DisableEvent(this);
			if (IsGoalkeeper)
			{
				AnimationStateMachine.StateCompleteEvent -= AnimationStateMachineOnStateCompleteEvent;
			}
		}

		public void OnUpdate()
		{
			UpdateStateByDistance();
			if (state != State.WithBall && IsNearFence() && IsNearFence())
			{
				FixAngleToForward();
			}
			Move();
			if (_fov != null)
			{
				_fov.fovActive = (state == State.WithBall);
			}
		}

		private bool IsNearFence()
		{
			Bounds currentLevelBounds = FieldElementsSpawnManager.inst.currentLevelBounds;
			Vector3 position = base.transform.position;
			float num = position.x + 50f;
			Vector3 max = currentLevelBounds.max;
			int result;
			if (!(num > max.x))
			{
				Vector3 position2 = base.transform.position;
				float num2 = position2.x - 50f;
				Vector3 min = currentLevelBounds.min;
				result = ((num2 < min.x) ? 1 : 0);
			}
			else
			{
				result = 1;
			}
			return (byte)result != 0;
		}

		private bool IsNearFrontFence()
		{
			Bounds currentLevelBounds = FieldElementsSpawnManager.inst.currentLevelBounds;
			Vector3 position = base.transform.position;
			float num = position.z + 20f;
			Vector3 max = currentLevelBounds.max;
			return num > max.z;
		}

		public void SetBall(Ball ball)
		{
			_ball = ball;
		}

		public void SetData(PlayerData data)
		{
			this.data = data;
			UpdateViewsByData();
			_orgPosition = base.transform.position;
			ToggleHighlightCircle(toggle: false);
		}

		public void SetMaterials(Material outfitMat, Color hairColor, Color skinTone)
		{
			modelRenderer.material = new Material(outfitMat);
			if (modelRenderer.materials.Length > 2)
			{
				modelRenderer.materials[1].color = hairColor;
				modelRenderer.materials[2].color = skinTone;
			}
			else if (modelRenderer.materials.Length > 1)
			{
				modelRenderer.materials[1].color = hairColor;
			}
			GlowObject componentInChildren = GetComponentInChildren<GlowObject>();
			if (componentInChildren != null)
			{
				componentInChildren.RefreshMaterials();
			}
		}

		private void UpdateViewsByData()
		{
			SetPlayerName(data.playerName);
			Vector3 localPosition = playerLevel.transform.localPosition;
			localPosition.y = 0f;
			playerLevel.transform.localPosition = localPosition;
			localPosition = playerLevel.transform.parent.localPosition;
			localPosition.y = 0f;
			playerLevel.transform.parent.localPosition = localPosition;
			playerLevel.text = data.shirtNumber.ToString();
			playerLevel.color = Color.white;
		}

		public void SetPlayerName(string name)
		{
			playerName.text = name;
			playerName.fontSize = 45;
			playerName.color = Color.white;
		}

		public void SetOutOfEnergy()
		{
			playerName.color = Color.red;
			playerName.text = "Out of energy";
		}

		public void CatchBall(bool hasEnergyLimit)
		{
			state = State.WithBall;
			if (_ball.IsNearGoal)
			{
				playerLevel.color = Color.yellow;
			}
			ToggleHighlightCircle(toggle: true);
			playerLevel.color = Color.white;
			playerName.color = highlightTextColor;
			LeanTween.value(base.gameObject, UpdateFontSize, playerName.fontSize, 50f, 0.2f).setEaseInOutSine();
		}

		private void UpdateFontSize(float val)
		{
			playerName.fontSize = (int)val;
		}

		private void SetDistanceData()
		{
			DetectionData detectionData = (!IsGoalkeeper) ? controlParams.fieldPlayerDetection : controlParams.goalkeeperDetection;
			float num = DistanceFromBall;
			if (_ball.state == Ball.State.InAir && _ball.kickData.isGoalKick && data.isHomeTeam)
			{
				num /= 2f;
			}
			_distanceData.isInActivationDistance = (num <= detectionData.distanceToActivate);
			_distanceData.isInFollowPlayerDistance = (num <= detectionData.distanceToFollow);
			_distanceData.isInFollowBallDistance = (num <= detectionData.distanceToFollowBallInAir);
			_distanceData.isInGlitchDistance = (num <= detectionData.distanceToGlitch);
			_distanceData.isInJumpDistance = (num <= detectionData.distanceToJump);
			_distanceData.isInFollowGroundBallDistance = (num <= detectionData.distanceToFollowGroundBall);
		}

		private void UpdateStateByDistance()
		{
			if (state == State.Connecting || _highlighted)
			{
				return;
			}
			SetDistanceData();
			switch (state)
			{
			case State.WithBall:
				break;
			case State.Idle:
				if (IsGoalkeeper && CanJump)
				{
					SetGoalkeeperJump();
				}
				else if (_distanceData.isInActivationDistance && (!IsNearFrontFence() || !data.isHomeTeam))
				{
					SetActiveState();
				}
				break;
			case State.Active:
				if (IsGoalkeeper && CanJump)
				{
					SetGoalkeeperJump();
				}
				else if (!IsGoalkeeper && data.isHomeTeam && IsNearFrontFence())
				{
					SetIdle(rotateToBall: false);
				}
				else if (!_distanceData.isInActivationDistance)
				{
					SetIdle(rotateToBall: false);
				}
				else if (CanFollowBall)
				{
					SetFollowBall();
				}
				break;
			case State.FollowingBall:
				if (!IsGoalkeeper)
				{
					if (data.isHomeTeam && IsNearFrontFence())
					{
						SetIdle(rotateToBall: false);
					}
					if (!CanFollowBall)
					{
						SetActiveState();
					}
					else if (CanGlitch)
					{
						SetGlitch();
					}
				}
				else if (CanJump)
				{
					SetGoalkeeperJump();
				}
				else if (CanGlitch)
				{
					SetGlitch();
				}
				break;
			case State.Glitch:
				if (!CanGlitch)
				{
					GlitchSpeed *= 0.3f;
				}
				break;
			}
		}

		public void SetKick()
		{
			ToggleHighlightCircle(toggle: false);
			playerLevel.color = Color.white;
			state = State.Kick;
			playerName.color = Color.white;
			ToggleCatching(toggle: false);
			LeanTween.cancel(base.gameObject);
			_animator.SetTrigger("Kick");
			LeanTween.delayedCall(base.gameObject, 1f, (Action)delegate
			{
				SetIdle(rotateToBall: false);
			});
		}

		public void ToggleEnergyBar(bool toggle)
		{
			energyImage.transform.parent.SetActive(toggle);
			SetPlayerName(data.playerName);
		}

		public void SetIdle(bool rotateToBall)
		{
			_highlighted = false;
			Speed = 0f;
			if (IsGoalkeeper)
			{
				_animator.SetTrigger("Idle");
			}
			_animator.SetFloat("IdleDelay", UnityEngine.Random.Range(0.01f, 1f));
			UpdateAnimSpeed(0f);
			UpdateViewsByData();
			ToggleCatching(toggle: false);
			LeanTween.cancel(base.gameObject);
			state = State.Idle;
			ToggleEnergyBar(toggle: false);
			trail.enabled = false;
			if (rotateToBall)
			{
				RotateTowards(_ball.GroundPosition);
			}
		}

		public void SetRecovering()
		{
			if (highlight != null)
			{
				highlight.enabled = false;
			}
			ToggleCatching(toggle: false);
			ToggleEnergyBar(toggle: false);
			Speed = 0f;
			UpdateAnimSpeed(0f);
			LeanTween.cancel(base.gameObject);
			state = State.Recovering;
		}

		public void SetFall()
		{
			if (highlight != null)
			{
				highlight.enabled = false;
			}
			state = State.Fall;
			ToggleCatching(toggle: false);
			ToggleEnergyBar(toggle: false);
			if (!IsGoalkeeper)
			{
				_animator.SetTrigger("Fall");
			}
		}

		public void SetActiveState()
		{
			trail.enabled = false;
			state = State.Active;
			UI.SetActive(value: true);
			ToggleEnergyBar(toggle: false);
			if (data.isHomeTeam)
			{
				LeanTween.rotateLocal(playerBody.gameObject, Quaternion.identity.eulerAngles, 0.3f);
			}
			switch (data.playerRole)
			{
			case PlayerRole.Static:
			case PlayerRole.GlitchByDistance:
				_speedMultiplier = 0f;
				break;
			case PlayerRole.Running:
			case PlayerRole.RunningAndGlitch:
				_speedMultiplier = UnityEngine.Random.Range(1f, 2f);
				break;
			case PlayerRole.GoalKeeper:
				_animator.SetTrigger("Idle");
				_speedMultiplier = 0f;
				break;
			}
		}

		private void SetFollowBall()
		{
			state = State.FollowingBall;
			ToggleHighlightCircle(toggle: false);
		}

		private void SetGlitch()
		{
			if (data.level == 5 || IsGoalkeeper)
			{
				bool aSuccess = false;
				Vector3 forward = Utils.CalculateInterceptCourse(_ball.GroundPosition, _ball.Velocity, base.transform.position, data.speedData.glitchSpeed, out aSuccess);
				if (aSuccess)
				{
					playerBody.localRotation = Quaternion.LookRotation(forward, Vector3.up);
					trail.enabled = true;
					state = State.Glitch;
					GlitchSpeed = data.speedData.glitchSpeed;
					return;
				}
				Vector3 groundPosition = _ball.GroundPosition;
				forward = groundPosition - base.transform.position;
				playerBody.localRotation = Quaternion.LookRotation(forward, Vector3.up);
				trail.enabled = true;
				state = State.Glitch;
				GlitchSpeed = data.speedData.glitchSpeed;
			}
			else
			{
				Vector3 zero = Vector3.zero;
				float num = DistanceFromBall / data.speedData.glitchSpeed;
				Vector3 velocity = _ball.Velocity;
				float x = velocity.x * num;
				Vector3 velocity2 = _ball.Velocity;
				float z = velocity2.z * num;
				Vector3 a = new Vector3(x, 0f, z);
				AnimationCurve animationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
				float time = 0.333333343f * (float)(data.level - 1);
				float d = animationCurve.Evaluate(time);
				a *= d;
				Vector3 a2 = _ball.GroundPosition + a;
				zero = a2 - base.transform.position;
				playerBody.localRotation = Quaternion.LookRotation(zero, Vector3.up);
				trail.enabled = true;
				state = State.Glitch;
				GlitchSpeed = data.speedData.glitchSpeed;
			}
			if (!IsGoalkeeper)
			{
				_animator.SetTrigger("Tackle");
			}
			this.PlaySFX(SoundId.player_slide);
		}

		private void LogError()
		{
			UnityEngine.Debug.LogError("t");
		}

		private void SetGoalkeeperJump()
		{
			state = State.GoalJump;
			this.PlaySFX(SoundId.goal_keeper_jump);
			float z = _orgPosition.z;
			Vector3 position = _ball.transform.position;
			float num = z - position.z;
			Vector3 velocity = _ball.Velocity;
			if (velocity.z <= 0.02f)
			{
				LeanTween.move(base.gameObject, _ball.GroundPosition, 0.25f);
				return;
			}
			float num2 = num;
			Vector3 velocity2 = _ball.Velocity;
			float num3 = num2 / velocity2.z;
			Vector3 velocity3 = _ball.Velocity;
			float x = velocity3.x * num3;
			Vector3 velocity4 = _ball.Velocity;
			float z2 = velocity4.z * num3;
			Vector3 b = new Vector3(x, 0f, z2);
			Vector3 to = _ball.GroundPosition + b;
			if (to.z > _orgPosition.z)
			{
				to.z = _orgPosition.z;
			}
			float x2 = to.x;
			Vector3 position2 = base.transform.position;
			float num4 = Mathf.Abs(x2 - position2.x);
			if (num4 > 10f)
			{
				float x3 = to.x;
				Vector3 position3 = base.transform.position;
				if (x3 > position3.x)
				{
					_animator.SetTrigger("SaveLeft");
				}
				else
				{
					_animator.SetTrigger("SaveRight");
				}
			}
			LeanTween.move(base.gameObject, to, 0.25f);
		}

		public void UpdateEnergyAmount(float val)
		{
			_energyAmount = val;
			energyImage.fillAmount = val;
		}

		private void Move()
		{
			switch (state)
			{
			case State.Init:
				UpdateRotation();
				break;
			case State.Idle:
				MoveForward();
				break;
			case State.Active:
				MoveForward();
				break;
			case State.FollowingBall:
				if (data.playerRole == PlayerRole.GoalKeeper)
				{
					GoalKeeperFollow(DistanceFromBall);
				}
				else
				{
					Follow();
				}
				break;
			case State.WithBall:
				MoveForward();
				FixAngleToForward();
				UpdateRotation();
				break;
			case State.Glitch:
				Glitch();
				break;
			}
		}

		private void UpdateRotation()
		{
			Quaternion localRotation = playerBody.localRotation;
			Vector3 eulerAngles = localRotation.eulerAngles;
			eulerAngles.y = _rotateAngle;
			localRotation.eulerAngles = eulerAngles;
			playerBody.localRotation = localRotation;
		}

		private void MoveForward()
		{
			switch (state)
			{
			case State.Active:
			{
				float num = controlParams.forwardSpeed * controlParams.freeBallSpeedModifier;
				Speed = num * _speedMultiplier;
				break;
			}
			case State.WithBall:
				Speed = controlParams.forwardSpeedWithBall;
				if (_energyAmount <= 0f)
				{
					Speed = controlParams.forwardSpeedWithBall * controlParams.noEnergySpeedModifier;
				}
				else if (_energyAmount <= 0.25f)
				{
					Speed = controlParams.forwardSpeedWithBall * controlParams.lowEnergySpeedModifier;
				}
				break;
			case State.Idle:
				Speed *= 0.95f;
				break;
			case State.Glitch:
				Speed = 0f;
				break;
			}
			UpdateAnimSpeed(Speed);
			Vector3 translation = Speed * playerBody.forward * Time.deltaTime;
			base.transform.Translate(translation);
		}

		private void RotateTowards(Vector3 targetPos)
		{
			Vector3 forward = targetPos - base.transform.position;
			if (forward.magnitude > Mathf.Epsilon)
			{
				playerBody.localRotation = Quaternion.LookRotation(forward, Vector3.up);
			}
		}

		private void Follow()
		{
			RotateTowards(FollowTarget);
			float num = GetFollowSpeed(_ball.state);
			if (_highlighted)
			{
				num *= 1.1f;
			}
			UpdateAnimSpeed(num);
			Vector3 position = base.transform.position;
			base.transform.position = Vector3.MoveTowards(base.transform.position, FollowTarget, num * Time.deltaTime);
		}

		private float GetFollowSpeed(Ball.State state)
		{
			switch (state)
			{
			case Ball.State.InAir:
				return data.speedData.followBallSpeed;
			case Ball.State.Ground:
				return data.speedData.followGroundBallSpeed;
			case Ball.State.Held:
				return data.speedData.followPlayerSpeed;
			default:
				return 0f;
			}
		}

		private void UpdateAnimSpeed(float s)
		{
			if (_animator != null)
			{
				_animator.SetFloat("RunSpeed", s);
			}
		}

		private void GoalKeeperFollow(float distance)
		{
			if (distance > 50f)
			{
				_animator.SetBool("SideWalk", value: true);
				Animator animator = _animator;
				Vector3 position = base.transform.position;
				float x = position.x;
				Vector3 position2 = _ball.transform.position;
				animator.SetFloat("hOffset", x - position2.x);
			}
			else
			{
				_animator.SetBool("SideWalk", value: false);
			}
			float followPlayerSpeed = data.speedData.followPlayerSpeed;
			float maxDistanceDelta = followPlayerSpeed * Time.deltaTime;
			Vector3 followTarget = FollowTarget;
			Vector3 followTarget2 = FollowTarget;
			followTarget.x = followTarget2.x;
			Vector3 position3 = base.transform.position;
			followTarget.y = position3.y;
			Ball.State state = _ball.state;
			if (state != Ball.State.Ground)
			{
				followTarget.z = Mathf.Clamp(followTarget.z, _orgPosition.z - 15f, _orgPosition.z);
			}
			base.transform.position = Vector3.MoveTowards(base.transform.position, followTarget, maxDistanceDelta);
			UpdateAnimSpeed(followPlayerSpeed);
		}

		private void Glitch()
		{
			Vector3 translation = GlitchSpeed * playerBody.forward * Time.deltaTime;
			base.transform.Translate(translation);
			GlitchSpeed *= 0.999f;
		}

		public void RotateBy(float value)
		{
			_rotateAngle += value * controlParams.rotationSpeed;
			_rotateAngle = Mathf.Clamp(_rotateAngle, 0f - controlParams.maxAngle, controlParams.maxAngle);
		}

		private void FixAngleToForward()
		{
			_rotateAngle = Mathf.Lerp(_rotateAngle, 0f, controlParams.lerpFixTime * Time.deltaTime);
		}

		public void ToggleCatching(bool toggle)
		{
			MarkAsAvailable(toggle);
			playerLevel.color = ((!toggle) ? Color.white : Color.yellow);
			if (state != 0 && toggle)
			{
				SetFollowBall();
			}
		}

		private void ToggleHighlightCircle(bool toggle)
		{
			if (highlight != null)
			{
				highlight.enabled = toggle;
			}
		}

		public void MarkAsAvailable(bool highlight, bool specialGlow = false)
		{
			if (_highlighted == highlight)
			{
				return;
			}
			_highlighted = highlight;
			if (specialGlow && highlight)
			{
				playerLevel.color = Color.yellow;
			}
			if (qualityData.glowEnabled)
			{
				GlowObject componentInChildren = GetComponentInChildren<GlowObject>();
				if (componentInChildren != null)
				{
					componentInChildren.ToggleGlow(highlight);
				}
			}
		}

		public void ToggleSpecialGlow()
		{
			playerLevel.color = Color.yellow;
		}

		private void SetDebugText(string text, FontStyle fontStyle)
		{
			playerName.fontStyle = fontStyle;
			playerName.text = text;
		}

		private void DebugFov()
		{
			if (_fov != null && _fov.fovActive)
			{
				IEnumerable<SoccerPlayer> source = from t in _fov.visibleTargets
					select t.GetComponent<SoccerPlayer>();
				List<SoccerPlayer> list = (from p in source
					where p != null && p.data != null && p.data.isHomeTeam
					select p).ToList();
				playerLevel.text = list.Count.ToString();
				playerLevel.color = ((list.Count <= 0) ? Color.white : Color.magenta);
			}
			else
			{
				playerLevel.text = data.level.ToString();
			}
		}

		public void SetGoalCatch()
		{
			_animator.SetTrigger("Catch");
		}

		public void TogglePause(bool paused)
		{
			if (paused)
			{
				_lastAnimSpeed = _animator.speed;
				_animator.speed = 0f;
			}
			else
			{
				_animator.speed = _lastAnimSpeed;
			}
		}

		static SoccerPlayer()
		{
			SoccerPlayer.DisableEvent = delegate
			{
			};
		}
	}
}
