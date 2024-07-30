using Soccerpass;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
	public float viewRadius;

	[Range(0f, 360f)]
	public float viewAngle;

	public LayerMask targetMask;

	public LayerMask obstacleMask;

	public List<Transform> visibleTargets = new List<Transform>();

	private readonly Collider[] targetsInViewRadius = new Collider[10];

	public bool fovActive;

	private void OnEnable()
	{
		StartCoroutine(FindTargetsWithDelay(0.02f));
	}

	private void OnDisable()
	{
		StopAllCoroutines();
	}

	private IEnumerator FindTargetsWithDelay(float delay)
	{
		WaitForSeconds wait = new WaitForSeconds(delay);
		while (true)
		{
			yield return wait;
			if (fovActive)
			{
				FindVisibleTargets();
			}
		}
	}

	private void OnDrawGizmos()
	{
		if (fovActive)
		{
			Gizmos.color = Color.white;
			Vector3 a = DirFromAngle((0f - viewAngle) / 2f, angleIsGlobal: false);
			Vector3 a2 = DirFromAngle(viewAngle / 2f, angleIsGlobal: false);
			Gizmos.DrawLine(base.transform.position + Vector3.up, base.transform.position + a * viewRadius);
			Gizmos.DrawLine(base.transform.position + Vector3.up, base.transform.position + a2 * viewRadius);
			foreach (Transform visibleTarget in visibleTargets)
			{
				SoccerPlayer component = visibleTarget.GetComponent<SoccerPlayer>();
				if (component == null)
				{
					Gizmos.color = Color.magenta;
				}
				else if (component.data.isHomeTeam)
				{
					Gizmos.color = Color.red;
				}
				else
				{
					Gizmos.color = Color.white;
				}
				Gizmos.DrawLine(base.transform.position, visibleTarget.position);
			}
		}
	}

	private void FindVisibleTargets()
	{
		visibleTargets.Clear();
		for (int i = 0; i < targetsInViewRadius.Length; i++)
		{
			targetsInViewRadius[i] = null;
		}
		int num = Physics.OverlapSphereNonAlloc(base.transform.position + Vector3.up, viewRadius, targetsInViewRadius, targetMask);
		for (int j = 0; j < num; j++)
		{
			Transform transform = targetsInViewRadius[j].transform;
			Vector3 normalized = (transform.position - base.transform.position).normalized;
			if (Vector3.Angle(base.transform.forward, normalized) < viewAngle / 2f)
			{
				visibleTargets.Add(transform);
			}
		}
	}

	public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
	{
		if (!angleIsGlobal)
		{
			float num = angleInDegrees;
			Vector3 eulerAngles = base.transform.eulerAngles;
			angleInDegrees = num + eulerAngles.y;
		}
		return new Vector3(Mathf.Sin(angleInDegrees * ((float)Math.PI / 180f)), 0f, Mathf.Cos(angleInDegrees * ((float)Math.PI / 180f)));
	}
}
