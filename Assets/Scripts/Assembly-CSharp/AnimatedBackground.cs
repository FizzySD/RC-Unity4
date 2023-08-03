using System;
using UnityEngine;

public class AnimatedBackground : MonoBehaviour
{
	public int numHexagons = 10;

	public float speed = 1f;

	public float lineWidth = 0.1f;

	public float noiseScale = 1f;

	public float noiseSpeed = 1f;

	public Material material;

	public float scale = 1f;

	private LineRenderer[] lineRenderers;

	private float angleOffset;

	private void Start()
	{
		lineRenderers = new LineRenderer[numHexagons];
		angleOffset = 360f / (float)numHexagons;
		for (int i = 0; i < numHexagons; i++)
		{
			GameObject gameObject = new GameObject("Hexagon" + i);
			gameObject.transform.SetParent(base.transform);
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localScale = new Vector3(scale, scale, scale);
			lineRenderers[i] = gameObject.AddComponent<LineRenderer>();
			lineRenderers[i].material = material;
			lineRenderers[i].SetWidth(lineWidth, lineWidth);
			lineRenderers[i].SetVertexCount(7);
			lineRenderers[i].useWorldSpace = false;
		}
	}

	private void Update()
	{
		for (int i = 0; i < numHexagons; i++)
		{
			float angle = Time.time * speed + (float)i * angleOffset;
			Vector3[] array = CalculateHexagonPositions(angle);
			lineRenderers[i].SetVertexCount(array.Length);
			SetLineRendererPositions(lineRenderers[i], array);
			Vector3[] positions = ApplyMovementEffect(array);
			SetLineRendererPositions(lineRenderers[i], positions);
		}
	}

	private void SetLineRendererPositions(LineRenderer lineRenderer, Vector3[] positions)
	{
		for (int i = 0; i < positions.Length; i++)
		{
			lineRenderer.SetPosition(i, positions[i]);
		}
	}

	private Vector3[] CalculateHexagonPositions(float angle)
	{
		Vector3[] array = new Vector3[7];
		float num = 1f;
		float num2 = num * Mathf.Cos(angle * ((float)Math.PI / 180f));
		float num3 = num * Mathf.Sin(angle * ((float)Math.PI / 180f));
		for (int i = 0; i < 6; i++)
		{
			float num4 = (float)(60 * i) + angle;
			float x = num * Mathf.Cos(num4 * ((float)Math.PI / 180f));
			float y = num * Mathf.Sin(num4 * ((float)Math.PI / 180f));
			array[i] = new Vector3(x, y, 0f);
		}
		array[6] = array[0];
		return array;
	}

	private Vector3[] ApplyMovementEffect(Vector3[] positions)
	{
		Vector3[] array = new Vector3[positions.Length];
		float num = Time.time * noiseSpeed;
		for (int i = 0; i < positions.Length; i++)
		{
			float num2 = Mathf.PerlinNoise(positions[i].x * noiseScale + num, positions[i].y * noiseScale + num);
			array[i] = positions[i] + positions[i] * num2;
		}
		return array;
	}
}
