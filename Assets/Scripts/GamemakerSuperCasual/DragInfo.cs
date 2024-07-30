using UnityEngine;

namespace GamemakerSuperCasual
{
	public class DragInfo
	{
		public Vector2 pos;

		public Vector2 delta;

		public int fingerCount = 1;

		public bool isFlick;

		public bool isMouse;

		public int index;

		public DragInfo(Vector2 p, Vector2 dir, int fCount)
			: this(p, dir, fCount, 0, iFlick: false, im: false)
		{
		}

		public DragInfo(Vector2 p, Vector2 dir, int fCount, bool iFlick)
			: this(p, dir, fCount, 0, iFlick, im: false)
		{
		}

		public DragInfo(Vector2 p, Vector2 dir, int fCount, int ind, bool im)
			: this(p, dir, fCount, ind, iFlick: false, im)
		{
		}

		public DragInfo(Vector2 p, Vector2 dir, int fCount, int ind, bool iFlick, bool im)
		{
			pos = p;
			delta = dir;
			fingerCount = fCount;
			index = ind;
			isFlick = iFlick;
			isMouse = im;
		}
	}
}
