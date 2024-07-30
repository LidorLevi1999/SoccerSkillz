using System.Collections.Generic;
using UnityEngine;

public class DisalbleOnCollision : MonoBehaviour
{
	public List<string> tagsToDisable = new List<string>
	{
		"Player",
		"FieldElements"
	};

	private void OnTriggerEnter(Collider other)
	{
		if (tagsToDisable.Contains(other.tag))
		{
			if (other.transform.parent.CompareTag(other.tag))
			{
				SimplePool.Despawn(other.transform.parent.gameObject);
			}
			else
			{
				SimplePool.Despawn(other.gameObject);
			}
		}
	}
}
