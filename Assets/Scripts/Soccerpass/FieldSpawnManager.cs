using System.Collections;
using UnityEngine;

namespace Soccerpass
{
	public class FieldSpawnManager : MonoBehaviour
	{
		public LevelState levelState;

		public QualityData qualityData;

		[Header("Views")]
		public Transform parent;

		public Transform spawnPoint;

		public FieldSegment startSegment;

		[Header("prefabs")]
		public GameObject prefabCrowd;

		private float _nextSpawnDistance;

		private float _fieldSize;

		public void Init()
		{
			SimplePool.Preload(prefabCrowd, 3);
			_fieldSize = startSegment.size;
			float fieldSize = _fieldSize;
			Vector3 localPosition = startSegment.transform.localPosition;
			_nextSpawnDistance = fieldSize + localPosition.z;
			StartCoroutine(SpawnCoro());
		}

		private IEnumerator SpawnCoro()
		{
			while (true)
			{
				float nextSpawnDistance = _nextSpawnDistance;
				Vector3 position = spawnPoint.position;
				if (nextSpawnDistance < position.z)
				{
					Spawn();
				}
				yield return new WaitForSeconds(0.5f);
			}
		}

		private void Spawn()
		{
			GameObject gameObject = SimplePool.Spawn(prefabCrowd, Vector3.zero, Quaternion.identity);
			gameObject.transform.SetParent(parent);
			Vector3 localPosition = gameObject.transform.localPosition;
			localPosition.z = _nextSpawnDistance;
			gameObject.transform.localPosition = localPosition;
			_nextSpawnDistance += _fieldSize;
		}
	}
}
