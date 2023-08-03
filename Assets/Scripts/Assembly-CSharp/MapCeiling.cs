using System;
using UnityEngine;

public class MapCeiling : MonoBehaviour
{
	private GameObject _barrierRef;

	private Color _color;

	private float _minAlpha;

	private float _maxAlpha = 0.6f;

	private float _minimumHeight = 3f;

	private static float _forestHeight = 280f;

	private static float _cityHeight = 210f;

	private static float _forestWidth = 1320f;

	private static float _cityWidth = 1400f;

	private static float _depth = 20f;

	public static void CreateMapCeiling()
	{
		if (FengGameManagerMKII.level.StartsWith("The Forest"))
		{
			CreateMapCeilingWithDimensions(_forestHeight, _forestWidth, _depth);
		}
		else if (FengGameManagerMKII.level.StartsWith("The City"))
		{
			CreateMapCeilingWithDimensions(_cityHeight, _cityWidth, _depth);
		}
	}

	private static void CreateMapCeilingWithDimensions(float height, float width, float depth)
	{
		GameObject gameObject = new GameObject();
		gameObject.AddComponent<MapCeiling>();
		gameObject.transform.position = new Vector3(0f, height, 0f);
		gameObject.transform.rotation = Quaternion.identity;
		gameObject.transform.localScale = new Vector3(width, depth, width);
	}

	private void Start()
	{
		CreateCeilingPart("barrier");
		_barrierRef = CreateCeilingPart("killcuboid");
		_color = new Color(1f, 0f, 0f, _maxAlpha);
		UpdateTransparency();
	}

	private GameObject CreateCeilingPart(string asset)
	{
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(FengGameManagerMKII.RCassets.Load(asset), Vector3.zero, Quaternion.identity);
		gameObject.transform.position = base.transform.position;
		gameObject.transform.rotation = base.transform.rotation;
		gameObject.transform.localScale = base.transform.localScale;
		return gameObject;
	}

	private void Update()
	{
		UpdateTransparency();
	}

	private float getMinAlpha()
	{
		return _minAlpha;
	}

	private void setMinAlpha(float newMinAlpha)
	{
		if (newMinAlpha > 1f || newMinAlpha < 0f)
		{
			throw new Exception("Error: _minAlpha must in range (0 <= _minAlpha <= 1)");
		}
		_minAlpha = newMinAlpha;
	}

	public float getMaxAlpha()
	{
		return _maxAlpha;
	}

	public void setMaxAlpha(float newMaxAlpha)
	{
		if (newMaxAlpha > 1f || newMaxAlpha < 0f)
		{
			throw new Exception("Error: _minAlpha must in range (0 <= _minAlpha <= 1)");
		}
		_maxAlpha = newMaxAlpha;
	}

	public void UpdateTransparency()
	{
		if (Camera.main != null && _barrierRef != null && _barrierRef.renderer != null)
		{
			float num = _maxAlpha;
			try
			{
				float num2 = _barrierRef.transform.position.y / _minimumHeight;
				num = ((!(Camera.main.transform.position.y < num2)) ? Map(Camera.main.transform.position.y, num2, _barrierRef.transform.position.y, _minAlpha, _maxAlpha) : _minAlpha);
				num = fadeByGradient(num);
			}
			catch
			{
			}
			_color.a = num;
			_barrierRef.renderer.material.color = _color;
		}
	}

	public float fadeByGradient(float x)
	{
		float num = 10f;
		float value = num * x * x;
		return Mathf.Clamp(value, _minAlpha, _maxAlpha);
	}

	public float Map(float x, float inMin, float inMax, float outMin, float outMax)
	{
		if (x > inMax || x < inMin)
		{
			throw new Exception("Error,\npublic float map(float x, float inMin, float inMax, float outMin, float outMax)\nis not defined for values (x > inMax || x < inMin)");
		}
		return (x - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
	}
}
