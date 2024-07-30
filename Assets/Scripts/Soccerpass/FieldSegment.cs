using UnityEngine;

namespace Soccerpass
{
	public class FieldSegment : MonoBehaviour
	{
		public QualityData data;

		public Transform soccerField;

		public Transform crowdLeft;

		public Transform crowdRight;

		public float minWidth = 18f;

		public float maxWidth = 25f;

		public GameObject crowdRootLeft;

		public GameObject crowdRootRight;

		[HideInInspector]
		public float width = 18f;

		public Collider col;

		public float size
		{
			get;
			private set;
		}

		private void OnEnable()
		{
			Vector3 size = col.bounds.size;
			this.size = size.z;
		}

		private void Update()
		{
			if (!(data == null) && data.crowdEnabled != crowdRootLeft.gameObject.activeInHierarchy)
			{
				ToggleCrowd(data.crowdEnabled);
			}
		}

		private void ToggleCrowd(bool toggle)
		{
			crowdRootLeft.gameObject.SetActive(toggle);
			crowdRootRight.gameObject.SetActive(toggle);
		}

		public void SetViewsByWidth()
		{
			Vector3 localScale = soccerField.localScale;
			localScale.x = width + 5f;
			soccerField.localScale = localScale;
			Vector3 localPosition = crowdLeft.transform.localPosition;
			localPosition.x = (0f - width) * 5f;
			crowdLeft.transform.localPosition = localPosition;
			localPosition = crowdRight.transform.localPosition;
			localPosition.x = width * 5f;
			crowdRight.transform.localPosition = localPosition;
		}

		public void SetFieldMaterial(Material mat)
		{
			soccerField.GetComponent<Renderer>().material = new Material(mat);
		}
	}
}
