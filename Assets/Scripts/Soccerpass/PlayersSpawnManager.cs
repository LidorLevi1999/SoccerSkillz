using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GamemakerSuperCasual;
using UnityEngine;

namespace Soccerpass
{
	public class PlayersSpawnManager : MonoBehaviour, IEventListener
	{
		[Header("Data")]
		public PlayersMetaData playersMetaData;

		public GameParams gameParams;

		public PlayerConfig defaultPlayerConfig;

		public ColorList hairColor;

		public ColorList skinTones;

		[Header("scene views")]
		public Transform parent;

		public Transform spawnPoint;

		[Header("prefabs")]
		public GameObject friendlyFormation;

		public GameObject playerPrefab;

		public GameObject goalkeeperPrefab;

		[Header("Materials")]
		public Material homeTeamMaterial;

		public Material goalkeeperMaterial;

		private Ball _ball;

		private List<SoccerPlayer> _activePlayers = new List<SoccerPlayer>();

		public LevelState levelState;

		public List<SoccerPlayer> _activePlayerForCurrentLevel;

		private PlayerFormation _lastFormation;

		public SoccerPlayer _goalKeeper;

		private bool _firstLevel = true;

		private LevelData _currentLevel;

		private int _currentLevelIndex;

		private int _teamIndex;

		public Transform followTransform;

		private List<string> _levelHomeTeamNames;

		private List<string> _levelOpponentTeamNames;

		private List<PlayerSpawnPoint> waitingSpawnPoints = new List<PlayerSpawnPoint>();

		private int _currentShirtNumber = 1;

		public Transform closestPlayerToBall
		{
			get;
			private set;
		}

		public void UpdateActivePlayerForLevel(int level)
		{
			_activePlayerForCurrentLevel = (from p in _activePlayers
				where p.IsActiveInLevel(level)
				select p).ToList();
		}

		public void Init(int startingLevel)
		{
			_ball = UnityEngine.Object.FindObjectOfType<Ball>();
			AddListeners();
			SimplePool.Preload(playerPrefab, 30);
			SimplePool.Preload(goalkeeperPrefab, 2);
			PlayerFormation[] array = Resources.LoadAll<PlayerFormation>("Data/Config/_Formations");
			PlayerFormation[] array2 = array;
			foreach (PlayerFormation playerFormation in array2)
			{
				SimplePool.Preload(playerFormation.gameObject, 3);
			}
			SimplePool.Preload(friendlyFormation, 2);
			StartCoroutine(SpawnPlayersCoro());
		}

		private IEnumerator SpawnPlayersCoro()
		{
			while (true)
			{
				//waitingSpawnPoints.ForEach(delegate(PlayerSpawnPoint sp)
				//{
				//	Vector3 position = followTransform.position;
				//	float z = position.z;
				//	Vector3 position2 = sp.transform.position;
				//	if (z > position2.z && sp.data.playerRole != PlayerRole.GoalKeeper)
				//	{
				//		SoccerPlayer soccerPlayer = AddPlayer(sp, playerPrefab);
				//		_activePlayers.Add(soccerPlayer);
				//		if (levelState.levelIndex == soccerPlayer.data.fieldLevel)
				//		{
				//			_activePlayerForCurrentLevel.Add(soccerPlayer);
				//		}
				//		SimplePool.Despawn(sp.transform.parent.gameObject);
				//	}
				//});

				foreach (var sp in waitingSpawnPoints.ToList())
				{
					Vector3 position = followTransform.position;
					float z = position.z;
					Vector3 position2 = sp.transform.position;
					if (z > position2.z && sp.data.playerRole != PlayerRole.GoalKeeper)
					{
						SoccerPlayer soccerPlayer = AddPlayer(sp, playerPrefab);
						_activePlayers.Add(soccerPlayer);
						if (levelState.levelIndex == soccerPlayer.data.fieldLevel)
						{
							_activePlayerForCurrentLevel.Add(soccerPlayer);
						}
						SimplePool.Despawn(sp.transform.parent.gameObject);
					}
				}

                yield return new WaitForSeconds(0.5f);
			}
		}

		public void SpawnFormationsForLevel(LevelData levelData, float startZ, int levelIndex)
		{
			_currentLevel = levelData;
			_currentLevelIndex = levelIndex;
			_teamIndex = _currentLevelIndex - 1;
			if (_teamIndex < 0)
			{
				_teamIndex = 0;
			}
			if (_teamIndex >= playersMetaData.outfits.Count)
			{
				_teamIndex = UnityEngine.Random.Range(0, playersMetaData.outfits.Count);
			}
			float num = startZ + (float)levelData.levelSize;
			float num2 = (!_firstLevel) ? gameParams.firstFormationAfterGoal : 350f;
			float num3 = startZ + num2;
			bool flag = true;
			playersMetaData.names.Shuffle2();
			int count = playersMetaData.names.Count;
			_levelHomeTeamNames = playersMetaData.names.GetRange(0, count / 2);
			_levelOpponentTeamNames = playersMetaData.names.GetRange(count / 2, count / 2 - 1);
			while (num3 + _currentLevel.distanceBetweenFormations < num)
			{
				GameObject formationPrefab = (!flag) ? GetNextFormation() : friendlyFormation;
				Vector3 position = spawnPoint.transform.position;
				position.z = num3;
				SpawnPlayersBatch(formationPrefab, position, flag);
				num3 += _currentLevel.distanceBetweenFormations;
				flag = false;
			}
			_firstLevel = false;
		}

		public void OnGameOver()
		{
			_activePlayers.ForEach(delegate(SoccerPlayer p)
			{
				p.OnGameOver();
			});
		}

		public void AddListeners()
		{
			SoccerPlayer.DisableEvent += SoccerPlayer_DisableEvent;
		}

		public void RemoveListeners()
		{
			SoccerPlayer.DisableEvent -= SoccerPlayer_DisableEvent;
		}

		private void OnDestroy()
		{
			RemoveListeners();
		}

		public void AddGoalKeeper(PlayerSpawnPoint sp)
		{
			sp.data = CreatePlayerData(sp);
			_goalKeeper = AddPlayer(sp, goalkeeperPrefab);
			_goalKeeper.transform.localScale = Vector3.one;
			_activePlayers.Add(_goalKeeper);
		}

		private GameObject GetNextFormation()
		{
			List<GameObject> formations = _currentLevel.GetFormations();
			try
			{
				IEnumerable<PlayerFormation> @this = from o in formations
					select o.GetComponent<PlayerFormation>();
				if (_lastFormation != null && _currentLevelIndex > 0)
				{
					PlayerFormation playerFormation = _lastFormation;
					for (int i = 0; i < 4; i++)
					{
						playerFormation = @this.SelectRandom().FirstOrDefault();
						if (playerFormation != _lastFormation)
						{
							return playerFormation.gameObject;
						}
					}
					return playerFormation.gameObject;
				}
				return @this.SelectRandom().FirstOrDefault().gameObject;
			}
			catch (Exception exception)
			{
				UnityEngine.Debug.LogException(exception);
				return friendlyFormation;
			}
		}

		private SoccerPlayer AddPlayer(PlayerSpawnPoint sp, GameObject prefab)
		{
			GameObject gameObject = SpawnPlayerPrefab(prefab, sp.transform);
			SoccerPlayer component = gameObject.GetComponent<SoccerPlayer>();
			SetSoccerPlayer(sp.data, component);
			waitingSpawnPoints.Remove(sp);
			return component;
		}

		private void SpawnPlayersBatch(GameObject formationPrefab, Vector3 pos, bool firstFormation)
		{
			GameObject gameObject = SimplePool.Spawn(formationPrefab, pos, Quaternion.identity);
			gameObject.transform.SetParent(parent);
			_lastFormation = gameObject.GetComponent<PlayerFormation>();
			gameObject.transform.position = pos;
			int num = (UnityEngine.Random.Range(0, 2) == 0) ? 1 : (-1);
			int num2 = (UnityEngine.Random.Range(0, 2) == 0) ? 1 : (-1);
			if (_currentLevelIndex == 0)
			{
				num = 1;
				num2 = 1;
			}
			gameObject.transform.localScale = new Vector3(num, 1f, num2);
			IEnumerator enumerator = gameObject.transform.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform transform = (Transform)enumerator.Current;
					PlayerSpawnPoint component = transform.GetComponent<PlayerSpawnPoint>();
					component.data = CreatePlayerData(component);
					waitingSpawnPoints.Add(component);
					if (firstFormation)
					{
						SoccerPlayer soccerPlayer = AddPlayer(component, playerPrefab);
						_activePlayers.Add(soccerPlayer);
						_activePlayerForCurrentLevel.Add(soccerPlayer);
						if (firstFormation)
						{
							UpdateClosestPlayer(soccerPlayer.gameObject);
						}
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
		}

		private GameObject SpawnPlayerPrefab(GameObject prefab, Transform holder)
		{
			GameObject gameObject = SimplePool.Spawn(prefab, Vector3.zero, Quaternion.identity);
			gameObject.transform.SetParent(holder);
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.SetParent(parent);
			gameObject.transform.localScale = Vector3.one;
			return gameObject;
		}

		private void SetSoccerPlayer(PlayerData data, SoccerPlayer soccerPlayer)
		{
			soccerPlayer.SetBall(_ball);
			soccerPlayer.playerBody.localRotation = Quaternion.identity;
			soccerPlayer.SetData(data);
			if (!soccerPlayer.IsGoalkeeper)
			{
				soccerPlayer.SetMaterials(GetMaterial(data), hairColor.RandomElement, skinTones.RandomElement);
			}
			soccerPlayer.SetIdle(rotateToBall: true);
			if (soccerPlayer.IsGoalkeeper)
			{
				soccerPlayer.gameObject.name = "Goalkeeper";
			}
			else
			{
				soccerPlayer.gameObject.name = data.playerName;
			}
		}

		private Material GetMaterial(PlayerData data)
		{
			if (data.playerRole == PlayerRole.GoalKeeper)
			{
				return goalkeeperMaterial;
			}
			if (data.isHomeTeam)
			{
				return homeTeamMaterial;
			}
			return playersMetaData.outfits[data.teamIndex];
		}

		private void SetPlayerRole(PlayerData data, PlayerSpawnPoint sp)
		{
			PlayerRole role = sp.role;
			if (role == PlayerRole.Default)
			{
				data.playerRole = ((!data.isHomeTeam) ? PlayerRole.GlitchByDistance : PlayerRole.Running);
			}
			else
			{
				data.playerRole = sp.role;
			}
		}

		private void SetPlayerLevel(PlayerData data, PlayerSpawnPoint sp)
		{
			if (data.isHomeTeam)
			{
				data.level = 1;
			}
			else if (sp.playerLevel == -1)
			{
				data.level = _currentLevel.GetPlayerAILevel();
			}
			else
			{
				data.level = sp.playerLevel;
			}
		}

		private PlayerData CreatePlayerData(PlayerSpawnPoint sp)
		{
			PlayerData playerData = new PlayerData();
			playerData.fieldLevel = _currentLevelIndex;
			playerData.teamIndex = _teamIndex;
			playerData.isHomeTeam = sp.isHomeTeam;
			playerData.shirtNumber = _currentShirtNumber;
			_currentShirtNumber = (_currentShirtNumber + 1) % 10;
			if (_currentShirtNumber == 0)
			{
				_currentShirtNumber = 1;
			}
			if (_levelHomeTeamNames.IsNullOrEmpty())
			{
				playerData.playerName = playersMetaData.RandomName;
			}
			else if (playerData.isHomeTeam)
			{
				playerData.playerName = _levelHomeTeamNames.RandomElement();
				_levelHomeTeamNames.Remove(playerData.playerName);
			}
			else
			{
				playerData.playerName = _levelOpponentTeamNames.RandomElement();
				_levelOpponentTeamNames.Remove(playerData.playerName);
			}
			SetPlayerRole(playerData, sp);
			SetPlayerLevel(playerData, sp);
			if (playerData.isHomeTeam)
			{
				PlayerRole playerRole = playerData.playerRole;
				if (playerRole == PlayerRole.GoalKeeper)
				{
					playerData.speedData = defaultPlayerConfig.goalkeeperSpeedData;
				}
				else
				{
					playerData.speedData = defaultPlayerConfig.fieldPlayerSpeedData;
				}
			}
			else
			{
				PlayerRole playerRole2 = playerData.playerRole;
				if (playerRole2 == PlayerRole.GoalKeeper)
				{
					playerData.speedData = _currentLevel.goalkeeperSpeedData;
				}
				else
				{
					playerData.speedData = _currentLevel.fieldPlayerSpeedData;
				}
			}
			return playerData;
		}

		private void UpdateClosestPlayer(GameObject player)
		{
			if (closestPlayerToBall == null)
			{
				closestPlayerToBall = player.transform;
				return;
			}
			float num = player.transform.Distance(_ball.transform);
			float num2 = closestPlayerToBall.Distance(_ball.transform);
			if (num < num2)
			{
				closestPlayerToBall = player.transform;
			}
		}

		public void OnUpdate()
		{
			_activePlayerForCurrentLevel.ForEach(delegate(SoccerPlayer p)
			{
				p.OnUpdate();
			});
		}

		public void TogglePause(bool paused)
		{
			_activePlayerForCurrentLevel.ForEach(delegate(SoccerPlayer p)
			{
				p.TogglePause(paused);
			});
		}

		private void SoccerPlayer_DisableEvent(SoccerPlayer obj)
		{
			_activePlayers.Remove(obj);
			_activePlayerForCurrentLevel.Remove(obj);
		}
	}
}
