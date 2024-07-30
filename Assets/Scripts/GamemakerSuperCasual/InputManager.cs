using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GamemakerSuperCasual
{
	public class InputManager : MonoBehaviour
	{
		public static InputManager inst;

		public bool useDPIScaling = true;

		public float defaultDPI = 441f;

		public StringVar dpi;

		public float minDragDistance = 15f;

		private Vector2 startPos;

		private Vector2 lastPos;

		public static bool isMouseDown
		{
			get;
			private set;
		}

		public static bool dragStarted
		{
			get;
			private set;
		}

		public static event Action<DragInfo> DragStart;

		public static event Action<DragInfo> Dragging;

		public static event Action<DragInfo> DragEnd;

		public static event Action<Vector2> Tap;

		public static event Action<Vector2> MouseDown;

		public static event Action<Vector2> MouseUp;

		public float GetDefaultDPI()
		{
			return defaultDPI;
		}

		public float GetCurrentDPI()
		{
			return (Screen.dpi == 0f) ? GetDefaultDPI() : Screen.dpi;
		}

		public float DPIFactor()
		{
			return useDPIScaling ? (GetCurrentDPI() / GetDefaultDPI()) : 1f;
		}

		public static float GetDPIFactor()
		{
			if (inst == null)
			{
				return 2f;
			}
			return inst.DPIFactor();
		}

		private void Awake()
		{
			inst = this;
			if (dpi != null)
			{
				dpi.value = GetDPIFactor().ToString();
			}
		}

		private void Start()
		{
			isMouseDown = false;
		}

		public void Reset()
		{
			isMouseDown = false;
		}

		private void Update()
		{
			if (EventSystem.current.IsPointerOverGameObject())
			{
				return;
			}
			Vector2 vector = UnityEngine.Input.mousePosition;
			if (isMouseDown)
			{
				if (Input.GetMouseButtonUp(0))
				{
					isMouseDown = false;
					if (dragStarted)
					{
						OnDragEnd();
					}
					else
					{
						InputManager.Tap(vector);
					}
					InputManager.MouseUp(vector);
					return;
				}
				if (!dragStarted)
				{
					if (Vector3.Distance(vector, startPos) > minDragDistance * GetDPIFactor())
					{
						OnDragStart(vector);
					}
				}
				else if (vector != lastPos)
				{
					OnDrag(vector);
				}
				lastPos = vector;
			}
			else if (Input.GetMouseButtonDown(0))
			{
				isMouseDown = true;
				InputManager.MouseDown(vector);
				startPos = vector;
				lastPos = startPos;
			}
		}

		private void OnDrag(Vector2 curPos)
		{
			Vector2 a = curPos - lastPos;
			float dPIFactor = GetDPIFactor();
			a *= dPIFactor;
			DragInfo obj = new DragInfo(curPos, a, 1, 0, im: false);
			InputManager.Dragging(obj);
		}

		private void OnDragStart(Vector2 curPos)
		{
			dragStarted = true;
			Vector2 a = curPos - startPos;
			float dPIFactor = GetDPIFactor();
			a *= dPIFactor;
			DragInfo obj = new DragInfo(curPos, a, 1, 0, im: false);
			InputManager.DragStart(obj);
		}

		private void OnDragEnd()
		{
			isMouseDown = false;
			dragStarted = false;
			Vector2 a = lastPos - startPos;
			float dPIFactor = GetDPIFactor();
			a *= dPIFactor;
			DragInfo obj = new DragInfo(lastPos, a, 1, 0, iFlick: false, im: false);
			InputManager.DragEnd(obj);
		}

		static InputManager()
		{
			InputManager.DragStart = delegate
			{
			};
			InputManager.Dragging = delegate
			{
			};
			InputManager.DragEnd = delegate
			{
			};
			InputManager.Tap = delegate
			{
			};
			InputManager.MouseDown = delegate
			{
			};
			InputManager.MouseUp = delegate
			{
			};
		}
	}
}
