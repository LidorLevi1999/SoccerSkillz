using System.Collections.Generic;
using UnityEngine;

public class GlowObject : MonoBehaviour
{
	public Color GlowColor;

	public float LerpFactor = 10f;

	private List<Material> _materials = new List<Material>();

	private Color _currentColor;

	private Color _targetColor;

	private Color _orgColor;

	private bool updateColors;

	public bool debugGlow;

	public Renderer[] Renderers
	{
		get;
		private set;
	}

	public Color CurrentColor => _currentColor;

	private void Start()
	{
		RefreshMaterials();
		_orgColor = GlowColor;
	}

	public void RefreshMaterials()
	{
		_materials.Clear();
		Renderers = GetComponentsInChildren<Renderer>();
		Renderer[] renderers = Renderers;
		foreach (Renderer renderer in renderers)
		{
			_materials.AddRange(renderer.materials);
		}
	}

	public void ToggleGlow(bool toggle)
	{
		ToggleGlow(toggle, Color.black);
	}

	public void ToggleGlow(bool toggle, Color color)
	{
		if (color == Color.black)
		{
			color = GlowColor;
		}
		_targetColor = ((!toggle) ? Color.black : color);
		updateColors = true;
	}

	private void Update()
	{
		if (debugGlow)
		{
			if (Input.GetMouseButtonDown(0))
			{
				ToggleGlow(toggle: true);
			}
			else if (Input.GetMouseButtonUp(0))
			{
				ToggleGlow(toggle: false);
			}
		}
		if (updateColors)
		{
			_currentColor = Color.Lerp(_currentColor, _targetColor, Time.deltaTime * LerpFactor);
			for (int i = 0; i < _materials.Count; i++)
			{
				_materials[i].SetColor("_GlowColor", _currentColor);
			}
			if (_currentColor.Equals(_targetColor))
			{
				updateColors = false;
			}
		}
	}
}
