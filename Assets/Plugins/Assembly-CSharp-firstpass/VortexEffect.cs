using UnityEngine;

[AddComponentMenu("Image Effects/Vortex")]
[ExecuteInEditMode]
public class VortexEffect : ImageEffectBase
{
	public Vector2 radius = new Vector2(0.4f, 0.4f);

	public float angle = 50f;

	public Vector2 center = new Vector2(0.5f, 0.5f);

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		ImageEffects.RenderDistortion(base.material, source, destination, angle, center, radius);
	}
}
