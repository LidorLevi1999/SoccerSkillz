using UnityEngine;

namespace Soccerpass
{
	public class FollowTarget : MonoBehaviour
	{
		public Transform target;

		private Vector3 _offset;

		public Vector3 followVector = Vector3.one;

		public bool lerpToTarget;

		public float lerpSpeed = 2f;

		public bool hasRangeX;

		public float maxX = 90f;

		public float minX = -90f;

		public void SetTarget(Transform target, bool updateOffset = false)
		{
			this.target = target;
			if (updateOffset)
			{
				SetOffset();
			}
		}

		private void OnEnable()
		{
			if (target != null)
			{
				SetOffset();
			}
		}

		private void SetOffset()
		{
			_offset = base.transform.position - target.position;
		}

		private void Update()
		{
			if (!(target == null) && lerpToTarget)
			{
				Vector3 targetPos = GetTargetPos();
				base.transform.position = Vector3.Lerp(base.transform.position, targetPos, lerpSpeed * Time.deltaTime);
			}
		}

		private void LateUpdate()
		{
			if (!(target == null) && !lerpToTarget)
			{
				base.transform.position = GetTargetPos();
			}
		}

		private Vector3 GetTargetPos()
		{
			Vector3 result = target.position + _offset;
			if (followVector.x == 0f)
			{
				Vector3 position = base.transform.position;
				result.x = position.x;
			}
			else if (hasRangeX)
			{
				result.x = Mathf.Clamp(result.x, minX, maxX);
			}
			if (followVector.y == 0f)
			{
				Vector3 position2 = base.transform.position;
				result.y = position2.y;
			}
			if (followVector.z == 0f)
			{
				Vector3 position3 = base.transform.position;
				result.z = position3.z;
			}
			return result;
		}
	}
}
