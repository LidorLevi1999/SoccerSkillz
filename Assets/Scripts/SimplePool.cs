using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class SimplePool
{
	private class Pool
	{
		private int nextId = 1;

		private List<GameObject> poolList;

		private GameObject prefab;

		public Pool(GameObject prefab, int initialQty)
		{
			this.prefab = prefab;
			poolList = new List<GameObject>();
		}

		private GameObject AddToPool(Vector3 pos, Quaternion rot)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(prefab, pos, rot);
			gameObject.name = prefab.name + " (" + nextId++ + ")";
			gameObject.AddComponent<PoolMember>().myPool = this;
			poolList.Add(gameObject);
			totalObjects++;
			return gameObject;
		}

		public GameObject Spawn(Vector3 pos, Quaternion rot)
		{
			GameObject gameObject = poolList.FirstOrDefault((GameObject x) => x != null && !x.activeInHierarchy);
			if (gameObject == null)
			{
				gameObject = AddToPool(pos, rot);
			}
			gameObject.transform.position = pos;
			gameObject.transform.rotation = rot;
			gameObject.SetActive(value: true);
			return gameObject;
		}

		public void Despawn(GameObject obj)
		{
			obj.SetActive(value: false);
		}
	}

	private class PoolMember : MonoBehaviour
	{
		public Pool myPool;
	}

	public static int totalObjects;

	private const int DEFAULT_POOL_SIZE = 3;

	private static Dictionary<GameObject, Pool> pools;

	private static GameObject inactiveItemsHolder;

	private static void Init(GameObject prefab = null, int qty = 3)
	{
		if (pools == null)
		{
			pools = new Dictionary<GameObject, Pool>();
		}
		if (prefab != null && !pools.ContainsKey(prefab))
		{
			pools[prefab] = new Pool(prefab, qty);
		}
	}

	public static void Preload(GameObject prefab, int qty = 1)
	{
		Init(prefab, qty);
		GameObject[] array = new GameObject[qty];
		for (int i = 0; i < qty; i++)
		{
			array[i] = Spawn(prefab, Vector3.zero, Quaternion.identity);
		}
		for (int j = 0; j < qty; j++)
		{
			Despawn(array[j]);
		}
	}

	public static GameObject Spawn(GameObject prefab, Vector3 pos, Quaternion rot)
	{
		if (prefab == null)
		{
			UnityEngine.Debug.Log("SimplePool got null prefab!!");
		}
		Init(prefab);
		return pools[prefab].Spawn(pos, rot);
	}

	public static void Despawn(GameObject obj)
	{
		if (inactiveItemsHolder == null)
		{
			inactiveItemsHolder = new GameObject();
			inactiveItemsHolder.transform.SetParent(null);
			inactiveItemsHolder.transform.position = Vector3.zero;
			inactiveItemsHolder.name = "InactiveItemsHolder";
		}
		PoolMember component = obj.GetComponent<PoolMember>();
		if (component == null)
		{
			UnityEngine.Object.Destroy(obj);
			return;
		}
		obj.transform.SetParent(inactiveItemsHolder.transform, worldPositionStays: false);
		component.myPool.Despawn(obj);
	}
}
