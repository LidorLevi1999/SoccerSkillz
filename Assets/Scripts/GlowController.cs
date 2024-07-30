using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GlowController : MonoBehaviour
{
	private static GlowController _instance;

	private CommandBuffer _commandBuffer;

	private List<GlowObjectCmd> _glowableObjects = new List<GlowObjectCmd>();

	private Material _glowMat;

	private Material _blurMaterial;

	private Vector2 _blurTexelSize;

	private int _prePassRenderTexID;

	private int _blurPassRenderTexID;

	private int _tempRenderTexID;

	private int _blurSizeID;

	private int _glowColorID;

	private void Awake()
	{
		_instance = this;
		_glowMat = new Material(Shader.Find("Hidden/GlowCmdShader"));
		_blurMaterial = new Material(Shader.Find("Hidden/Blur"));
		_prePassRenderTexID = Shader.PropertyToID("_GlowPrePassTex");
		_blurPassRenderTexID = Shader.PropertyToID("_GlowBlurredTex");
		_tempRenderTexID = Shader.PropertyToID("_TempTex0");
		_blurSizeID = Shader.PropertyToID("_BlurSize");
		_glowColorID = Shader.PropertyToID("_GlowColor");
		_commandBuffer = new CommandBuffer();
		_commandBuffer.name = "Glowing Objects Buffer";
		GetComponent<Camera>().AddCommandBuffer(CameraEvent.BeforeImageEffects, _commandBuffer);
	}

	public static void RegisterObject(GlowObjectCmd glowObj)
	{
		if (_instance != null)
		{
			_instance._glowableObjects.Add(glowObj);
		}
	}

	private void RebuildCommandBuffer()
	{
		_commandBuffer.Clear();
		_commandBuffer.GetTemporaryRT(_prePassRenderTexID, Screen.width, Screen.height, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, QualitySettings.antiAliasing);
		_commandBuffer.SetRenderTarget(_prePassRenderTexID);
		_commandBuffer.ClearRenderTarget(clearDepth: true, clearColor: true, Color.clear);
		MonoBehaviour.print($"glowable obj count: {_glowableObjects.Count}");
		for (int i = 0; i < _glowableObjects.Count; i++)
		{
			_commandBuffer.SetGlobalColor(_glowColorID, _glowableObjects[i].CurrentColor);
			for (int j = 0; j < _glowableObjects[i].Renderers.Length; j++)
			{
				MonoBehaviour.print($"{_glowableObjects[i].name} length: {_glowableObjects[i].Renderers.Length}");
				_commandBuffer.DrawRenderer(_glowableObjects[i].Renderers[j], _glowMat);
			}
		}
		_commandBuffer.GetTemporaryRT(_blurPassRenderTexID, Screen.width >> 1, Screen.height >> 1, 0, FilterMode.Bilinear);
		_commandBuffer.GetTemporaryRT(_tempRenderTexID, Screen.width >> 1, Screen.height >> 1, 0, FilterMode.Bilinear);
		_commandBuffer.Blit(_prePassRenderTexID, _blurPassRenderTexID);
		_blurTexelSize = new Vector2(1.5f / (float)(Screen.width >> 1), 1.5f / (float)(Screen.height >> 1));
		_commandBuffer.SetGlobalVector(_blurSizeID, _blurTexelSize);
		for (int k = 0; k < 4; k++)
		{
			_commandBuffer.Blit(_blurPassRenderTexID, _tempRenderTexID, _blurMaterial, 0);
			_commandBuffer.Blit(_tempRenderTexID, _blurPassRenderTexID, _blurMaterial, 1);
		}
	}

	private void Update()
	{
		RebuildCommandBuffer();
	}
}
