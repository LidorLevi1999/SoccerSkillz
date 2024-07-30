using UnityEngine;

namespace Soccerpass
{
	public class FieldElements : MonoBehaviour
	{
		[HideInInspector]
		public float size;

		public float minSize = 500f;

		public float maxSize = 3000f;

		public PlayerSpawnPoint goalSpawnPoint;

		public Transform endPoint;

		public Transform centerPoint;

		public Transform goalKickLine;

		public Transform soccerField;

		public GameParams gameParams;

		public BoxCollider col;

		public Bounds bounds => soccerField.GetComponent<MeshRenderer>().bounds;

		public void SetViewsBySize()
		{
			SetLocalZ(endPoint, size);
			SetLocalZ(centerPoint, size / 2f);
			SetLocalZ(goalKickLine, size - gameParams.goalKickDistance);
			SetLocalZ(soccerField, size / 2f);
			Vector3 localScale = soccerField.localScale;
			localScale.z = size / 10f;
			soccerField.localScale = localScale;
			Vector3 center = col.center;
			center.z = size;
			col.center = center;
		}

		private void SetLocalZ(Transform t, float z)
		{
			Vector3 localPosition = t.localPosition;
			localPosition.z = z;
			t.localPosition = localPosition;
		}

		public void SetFieldMaterial(Material mat)
		{
			soccerField.GetComponent<Renderer>().material = new Material(mat);
		}
	}
}
