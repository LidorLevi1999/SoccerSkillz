using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Soccerpass
{
	public class FieldElementsSpawnManager : MonoBehaviour
	{
		public static FieldElementsSpawnManager inst;

		[Header("data")]
		public LevelState levelState;

		public MaterialList materialList;

		[Header("views")]
		public Transform parent;

		public Transform followTarget;

		public GameObject prefab;

		public PlayersSpawnManager playersSpawner;

		private float _nextSpawnDistance;

		private Dictionary<int, FieldElements> _dict = new Dictionary<int, FieldElements>();

		private FieldElements _latest;

		private FieldElements _current;

		private int _currentLevelIndex;

		public Bounds currentLevelBounds
		{
			get
			{
				FieldElements fieldElements = _dict[levelState.levelIndex];
				return fieldElements.bounds;
			}
		}

		public Transform nextGoal
		{
			get
			{
				FieldElements fieldElements = _dict[levelState.levelIndex];
				return fieldElements.GetComponentInChildren<Goal>().transform;
			}
		}

		public float DistanceFromGoal
		{
			get
			{
				Vector3 position = _current.goalSpawnPoint.transform.position;
				float z = position.z;
				Vector3 position2 = followTarget.position;
				return z - position2.z;
			}
		}

		private void Awake()
		{
			inst = this;
		}

		public void Init()
		{
			_dict = new Dictionary<int, FieldElements>();
			_currentLevelIndex = levelState.levelIndex;
			SimplePool.Preload(prefab, 3);
			Spawn();
			_current = _latest;
			StartCoroutine(SpawnCoro());
		}

		private void Spawn()
		{
			GameObject gameObject = SimplePool.Spawn(prefab, Vector3.zero, Quaternion.identity);
			gameObject.transform.SetParent(parent);
			Vector3 localPosition = gameObject.transform.localPosition;
			localPosition.y = 0f;
			localPosition.z = _nextSpawnDistance;
			gameObject.transform.localPosition = localPosition;
			_latest = gameObject.GetComponent<FieldElements>();
			Material fieldMaterial = materialList[_currentLevelIndex % materialList.Count];
			_latest.SetFieldMaterial(fieldMaterial);
			LevelData levelData = levelState.config[_currentLevelIndex];
			_latest.size = levelData.levelSize;
			_latest.SetViewsBySize();
			_nextSpawnDistance += _latest.size;
			playersSpawner.SpawnFormationsForLevel(levelData, localPosition.z, _currentLevelIndex);
			playersSpawner.AddGoalKeeper(_latest.goalSpawnPoint);
			if (!_dict.ContainsKey(_currentLevelIndex))
			{
				_dict.Add(_currentLevelIndex, _latest);
			}
			if (_currentLevelIndex + 1 < levelState.config.levels.Count)
			{
				_currentLevelIndex++;
			}
		}

		private IEnumerator SpawnCoro()
		{
			WaitForSeconds waitForSeconds = new WaitForSeconds(2f);
			while (true)
			{
				Vector3 position = followTarget.position;
				if (position.z > _nextSpawnDistance - _latest.size / 2f)
				{
					Spawn();
				}
				Vector3 position2 = _current.goalSpawnPoint.transform.position;
				float z = position2.z;
				Vector3 position3 = followTarget.position;
				if (z < position3.z)
				{
					_current = _latest;
				}
				yield return waitForSeconds;
			}
		}
	}
}
