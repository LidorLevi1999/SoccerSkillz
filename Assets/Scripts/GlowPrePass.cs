using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class GlowPrePass : MonoBehaviour
{
	private static RenderTexture PrePass;

	private static RenderTexture Blurred;

	private Material _blurMat;

	private void OnEnable()
	{
		PrePass = new RenderTexture(Screen.width, Screen.height, 24);
		PrePass.antiAliasing = 1;
		Blurred = new RenderTexture(Screen.width >> 2, Screen.height >> 2, 0);
		Camera component = GetComponent<Camera>();
		Shader shader = Shader.Find("Hidden/GlowReplace");
		component.targetTexture = PrePass;
		component.SetReplacementShader(shader, "Glowable");
		Shader.SetGlobalTexture("_GlowPrePassTex", PrePass);
		Shader.SetGlobalTexture("_GlowBlurredTex", Blurred);
		_blurMat = new Material(Shader.Find("Hidden/Blur"));
		Material blurMat = _blurMat;
		Vector2 texelSize = Blurred.texelSize;
		float x = texelSize.x * 1.5f;
		Vector2 texelSize2 = Blurred.texelSize;
		blurMat.SetVector("_BlurSize", new Vector2(x, texelSize2.y * 1.5f));
	}

	private void OnRenderImage(RenderTexture src, RenderTexture dst)
	{
		Graphics.Blit(src, dst);
		Graphics.SetRenderTarget(Blurred);
		GL.Clear(clearDepth: false, clearColor: true, Color.clear);
		Graphics.Blit(src, Blurred);
		for (int i = 0; i < 4; i++)
		{
			RenderTexture temporary = RenderTexture.GetTemporary(Blurred.width, Blurred.height);
			Graphics.Blit(Blurred, temporary, _blurMat, 0);
			Graphics.Blit(temporary, Blurred, _blurMat, 1);
			RenderTexture.ReleaseTemporary(temporary);
		}
	}
}
