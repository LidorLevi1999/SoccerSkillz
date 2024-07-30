using Soccerpass;
using GamemakerSuperCasual;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class GlowComposite : MonoBehaviour
{
	[Range(0f, 10f)]
	public float Intensity = 2f;

	private Material _compositeMat;

	public PlayerControlParams controlParams;

	public QualityData qualityData;

	private float _scaledIntensity;

	public GameObject glowCamera;

	private void OnEnable()
	{
		_compositeMat = new Material(Shader.Find("Hidden/GlowComposite"));
	}

	private void Start()
	{
		_scaledIntensity = Intensity;
		glowCamera.SetActive(qualityData.glowEnabled);
	}

	private void Update()
	{
		if (qualityData.glowEnabled != glowCamera.activeInHierarchy)
		{
			glowCamera.SetActive(qualityData.glowEnabled);
		}
	}

    private void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        if (controlParams != null && InputManager.inst != null)
        {
            _scaledIntensity = controlParams.highlightAmount * InputManager.GetDPIFactor();
        }
        else
        {
            _scaledIntensity = Intensity;
        }
        if (_compositeMat)
        {
            _compositeMat.SetFloat("_Intensity", _scaledIntensity);
            Graphics.Blit(src, dst, _compositeMat, 0);
        }
    }
}
