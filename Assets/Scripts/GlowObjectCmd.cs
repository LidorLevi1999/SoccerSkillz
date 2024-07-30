using UnityEngine;

public class GlowObjectCmd : MonoBehaviour
{
	public Color GlowColor;

	public float LerpFactor = 10f;

	private Color _currentColor;

	private Color _targetColor;

	public Renderer[] Renderers
	{
		get;
		private set;
	}

	public Color CurrentColor => _currentColor;

	private void Start()
	{
		Renderers = GetComponentsInChildren<Renderer>();
		GlowController.RegisterObject(this);
	}

	private void OnMouseEnter()
	{
		_targetColor = GlowColor;
		base.enabled = true;
	}

	private void OnMouseExit()
	{
		_targetColor = Color.black;
		base.enabled = true;
	}

	private void Update()
	{
		_currentColor = Color.Lerp(_currentColor, _targetColor, Time.deltaTime * LerpFactor);
		if (_currentColor.Equals(_targetColor))
		{
			base.enabled = false;
		}
	}
}
