using System.Collections;
using Settings;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class Minimap : MonoBehaviour
{
	public enum IconStyle
	{
		CIRCLE = 0,
		SUPPLY = 1
	}

	public class MinimapIcon
	{
		private Transform obj;

		private RectTransform pointerRect;

		public readonly bool rotation;

		public readonly IconStyle style;

		private RectTransform uiRect;

		public MinimapIcon(GameObject trackedObject, GameObject uiElement, IconStyle style)
		{
			rotation = false;
			this.style = style;
			obj = trackedObject.transform;
			uiRect = uiElement.GetComponent<RectTransform>();
			CatchDestroy component = obj.GetComponent<CatchDestroy>();
			if (component == null)
			{
				obj.gameObject.AddComponent<CatchDestroy>().target = uiElement;
			}
			else if (component.target != null && component.target != uiElement)
			{
				Object.Destroy(component.target);
			}
			else
			{
				component.target = uiElement;
			}
		}

		public MinimapIcon(GameObject trackedObject, GameObject uiElement, GameObject uiPointer, IconStyle style)
		{
			rotation = true;
			this.style = style;
			obj = trackedObject.transform;
			uiRect = uiElement.GetComponent<RectTransform>();
			pointerRect = uiPointer.GetComponent<RectTransform>();
			CatchDestroy component = obj.GetComponent<CatchDestroy>();
			if (component == null)
			{
				obj.gameObject.AddComponent<CatchDestroy>().target = uiElement;
			}
			else if (component.target != null && component.target != uiElement)
			{
				Object.Destroy(component.target);
			}
			else
			{
				component.target = uiElement;
			}
		}

		public static MinimapIcon Create(RectTransform parent, GameObject trackedObject, IconStyle style)
		{
			UnityEngine.Sprite spriteForStyle = GetSpriteForStyle(style);
			GameObject gameObject = new GameObject("MinimapIcon");
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			Vector2 anchorMin = (rectTransform.anchorMax = new Vector3(0.5f, 0.5f));
			rectTransform.anchorMin = anchorMin;
			rectTransform.sizeDelta = new Vector2(spriteForStyle.texture.width, spriteForStyle.texture.height);
			Image image = gameObject.AddComponent<Image>();
			image.sprite = spriteForStyle;
			image.type = Image.Type.Simple;
			gameObject.transform.SetParent(parent, false);
			return new MinimapIcon(trackedObject, gameObject, style);
		}

		public static MinimapIcon CreateWithRotation(RectTransform parent, GameObject trackedObject, IconStyle style, float pointerDist)
		{
			UnityEngine.Sprite spriteForStyle = GetSpriteForStyle(style);
			GameObject gameObject = new GameObject("MinimapIcon");
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			Vector2 anchorMin = (rectTransform.anchorMax = new Vector3(0.5f, 0.5f));
			rectTransform.anchorMin = anchorMin;
			rectTransform.sizeDelta = new Vector2(spriteForStyle.texture.width, spriteForStyle.texture.height);
			Image image = gameObject.AddComponent<Image>();
			image.sprite = spriteForStyle;
			image.type = Image.Type.Simple;
			gameObject.transform.SetParent(parent, false);
			GameObject gameObject2 = new GameObject("IconPointer");
			RectTransform rectTransform2 = gameObject2.AddComponent<RectTransform>();
			anchorMin = (rectTransform2.anchorMax = rectTransform.anchorMin);
			rectTransform2.anchorMin = anchorMin;
			rectTransform2.sizeDelta = new Vector2(pointerSprite.texture.width, pointerSprite.texture.height);
			Image image2 = gameObject2.AddComponent<Image>();
			image2.sprite = pointerSprite;
			image2.type = Image.Type.Simple;
			gameObject2.transform.SetParent(rectTransform, false);
			rectTransform2.anchoredPosition = new Vector2(0f, pointerDist);
			return new MinimapIcon(trackedObject, gameObject, gameObject2, style);
		}

		public void Destroy()
		{
			if (uiRect != null)
			{
				Object.Destroy(uiRect.gameObject);
			}
		}

		public void SetColor(Color color)
		{
			if (uiRect != null)
			{
				uiRect.GetComponent<Image>().color = color;
			}
		}

		public void SetDepth(bool aboveAll)
		{
			if (uiRect != null)
			{
				if (aboveAll)
				{
					uiRect.SetAsLastSibling();
				}
				else
				{
					uiRect.SetAsFirstSibling();
				}
			}
		}

		public void SetPointerSize(float size, float originDistance)
		{
			if (pointerRect != null)
			{
				pointerRect.sizeDelta = new Vector2(size, size);
				pointerRect.anchoredPosition = new Vector2(0f, originDistance);
			}
		}

		public void SetSize(Vector2 size)
		{
			if (uiRect != null)
			{
				uiRect.sizeDelta = size;
			}
		}

		public bool UpdateUI(Bounds worldBounds, float minimapSize)
		{
			if (obj == null)
			{
				return false;
			}
			float x = worldBounds.size.x;
			Vector3 vector = obj.position - worldBounds.center;
			vector.y = vector.z;
			vector.z = 0f;
			float num = Mathf.Abs(vector.x) / x;
			vector.x = ((vector.x < 0f) ? (0f - num) : num);
			float num2 = Mathf.Abs(vector.y) / x;
			vector.y = ((vector.y < 0f) ? (0f - num2) : num2);
			Vector2 anchoredPosition = vector * minimapSize;
			uiRect.anchoredPosition = anchoredPosition;
			if (rotation)
			{
				float z = Mathf.Atan2(obj.forward.z, obj.forward.x) * 57.29578f - 90f;
				uiRect.eulerAngles = new Vector3(0f, 0f, z);
			}
			return true;
		}
	}

	public class Preset
	{
		public readonly Vector3 center;

		public readonly float orthographicSize;

		public Preset(Vector3 center, float orthographicSize)
		{
			this.center = center;
			this.orthographicSize = orthographicSize;
		}
	}

	private bool assetsInitialized;

	private static UnityEngine.Sprite borderSprite;

	private RectTransform borderT;

	private Canvas canvas;

	private Vector2 cornerPosition;

	private float cornerSizeRatio;

	private Preset initialPreset;

	public static Minimap instance;

	private bool isEnabled;

	private bool isEnabledTemp;

	private Vector3 lastMinimapCenter;

	private float lastMinimapOrthoSize;

	private Camera lastUsedCamera;

	private bool maximized;

	private RectTransform minimap;

	private float MINIMAP_CORNER_SIZE;

	private float MINIMAP_CORNER_SIZE_SCALED;

	private Vector2 MINIMAP_ICON_SIZE;

	private float MINIMAP_POINTER_DIST;

	private float MINIMAP_POINTER_SIZE;

	private int MINIMAP_SIZE;

	private Vector2 MINIMAP_SUPPLY_SIZE;

	private MinimapIcon[] minimapIcons;

	private bool minimapIsCreated;

	private RectTransform minimapMaskT;

	private Bounds minimapOrthographicBounds;

	public RenderTexture minimapRT;

	public Camera myCam;

	private static UnityEngine.Sprite pointerSprite;

	private CanvasScaler scaler;

	private static UnityEngine.Sprite supplySprite;

	private static UnityEngine.Sprite whiteIconSprite;

	private void AddBorderToTexture(ref Texture2D texture, Color borderColor, int borderPixelSize)
	{
		int num = texture.width * borderPixelSize;
		Color[] array = new Color[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = borderColor;
		}
		texture.SetPixels(0, texture.height - borderPixelSize, texture.width - 1, borderPixelSize, array);
		texture.SetPixels(0, 0, texture.width, borderPixelSize, array);
		texture.SetPixels(0, 0, borderPixelSize, texture.height, array);
		texture.SetPixels(texture.width - borderPixelSize, 0, borderPixelSize, texture.height, array);
		texture.Apply();
	}

	private void AutomaticSetCameraProperties(Camera cam)
	{
		Renderer[] array = Object.FindObjectsOfType<Renderer>();
		if (array.Length != 0)
		{
			minimapOrthographicBounds = new Bounds(array[0].transform.position, Vector3.zero);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].gameObject.layer == 9)
				{
					minimapOrthographicBounds.Encapsulate(array[i].bounds);
				}
			}
		}
		Vector3 size = minimapOrthographicBounds.size;
		float num = ((size.x > size.z) ? size.x : size.z);
		size.z = (size.x = num);
		minimapOrthographicBounds.size = size;
		cam.orthographic = true;
		cam.orthographicSize = num * 0.5f;
		Vector3 center = minimapOrthographicBounds.center;
		center.y = cam.farClipPlane * 0.5f;
		Transform transform = cam.transform;
		transform.position = center;
		transform.eulerAngles = new Vector3(90f, 0f, 0f);
		cam.aspect = 1f;
		lastMinimapCenter = center;
		lastMinimapOrthoSize = cam.orthographicSize;
	}

	private void AutomaticSetOrthoBounds()
	{
		Renderer[] array = Object.FindObjectsOfType<Renderer>();
		if (array.Length != 0)
		{
			minimapOrthographicBounds = new Bounds(array[0].transform.position, Vector3.zero);
			for (int i = 0; i < array.Length; i++)
			{
				minimapOrthographicBounds.Encapsulate(array[i].bounds);
			}
		}
		Vector3 size = minimapOrthographicBounds.size;
		float num = ((size.x > size.z) ? size.x : size.z);
		size.z = (size.x = num);
		minimapOrthographicBounds.size = size;
		lastMinimapCenter = minimapOrthographicBounds.center;
		lastMinimapOrthoSize = num * 0.5f;
	}

	private void Awake()
	{
		instance = this;
	}

	private Texture2D CaptureMinimap(Camera cam)
	{
		RenderTexture renderTexture = RenderTexture.active;
		RenderTexture.active = cam.targetTexture;
		cam.Render();
		Texture2D texture2D = new Texture2D(cam.targetTexture.width, cam.targetTexture.height, TextureFormat.RGB24, false)
		{
			filterMode = FilterMode.Bilinear
		};
		texture2D.ReadPixels(new Rect(0f, 0f, cam.targetTexture.width, cam.targetTexture.height), 0, 0);
		texture2D.Apply();
		RenderTexture.active = renderTexture;
		return texture2D;
	}

	private void CaptureMinimapRT(Camera cam)
	{
		RenderTexture renderTexture = RenderTexture.active;
		RenderTexture.active = minimapRT;
		cam.targetTexture = minimapRT;
		cam.Render();
		RenderTexture.active = renderTexture;
	}

	private void CheckUserInput()
	{
		if (SettingsManager.GeneralSettings.MinimapEnabled.Value && !SettingsManager.LegacyGameSettings.GlobalMinimapDisable.Value)
		{
			if (!minimapIsCreated)
			{
				return;
			}
			if (SettingsManager.InputSettings.General.MinimapMaximize.GetKey())
			{
				if (!maximized)
				{
					Maximize();
				}
			}
			else if (maximized)
			{
				Minimize();
			}
			if (SettingsManager.InputSettings.General.MinimapToggle.GetKeyDown())
			{
				SetEnabled(!isEnabled);
			}
			if (!maximized)
			{
				return;
			}
			bool flag = false;
			if (SettingsManager.InputSettings.General.MinimapReset.GetKey())
			{
				if (initialPreset != null)
				{
					ManualSetCameraProperties(lastUsedCamera, initialPreset.center, initialPreset.orthographicSize);
				}
				else
				{
					AutomaticSetCameraProperties(lastUsedCamera);
				}
				flag = true;
			}
			else
			{
				float num = Input.GetAxis("Mouse ScrollWheel");
				if (num != 0f)
				{
					if (Input.GetKey(KeyCode.LeftShift))
					{
						num *= 3f;
					}
					lastMinimapOrthoSize = Mathf.Max(lastMinimapOrthoSize + num, 1f);
					flag = true;
				}
				if (Input.GetKey(KeyCode.UpArrow))
				{
					float num2 = Time.deltaTime * ((Input.GetKey(KeyCode.LeftShift) ? 2f : 0.75f) * lastMinimapOrthoSize);
					lastMinimapCenter.z += num2;
					flag = true;
				}
				else if (Input.GetKey(KeyCode.DownArrow))
				{
					float num2 = Time.deltaTime * ((Input.GetKey(KeyCode.LeftShift) ? 2f : 0.75f) * lastMinimapOrthoSize);
					lastMinimapCenter.z -= num2;
					flag = true;
				}
				if (Input.GetKey(KeyCode.RightArrow))
				{
					float num2 = Time.deltaTime * ((Input.GetKey(KeyCode.LeftShift) ? 2f : 0.75f) * lastMinimapOrthoSize);
					lastMinimapCenter.x += num2;
					flag = true;
				}
				else if (Input.GetKey(KeyCode.LeftArrow))
				{
					float num2 = Time.deltaTime * ((Input.GetKey(KeyCode.LeftShift) ? 2f : 0.75f) * lastMinimapOrthoSize);
					lastMinimapCenter.x -= num2;
					flag = true;
				}
			}
			if (flag)
			{
				RecaptureMinimap(lastUsedCamera, lastMinimapCenter, lastMinimapOrthoSize);
			}
		}
		else if (isEnabled)
		{
			SetEnabled(!isEnabled);
		}
	}

	public void CreateMinimap(Camera cam, int minimapResolution = 512, float cornerSize = 0.3f, Preset mapPreset = null)
	{
		if (Supported())
		{
			isEnabled = true;
			lastUsedCamera = cam;
			if (!assetsInitialized)
			{
				Initialize();
			}
			GameObject gameObject = GameObject.Find("mainLight");
			Light light = null;
			Quaternion rotation = Quaternion.identity;
			LightShadows shadows = LightShadows.None;
			Color color = Color.clear;
			float intensity = 0f;
			float nearClipPlane = cam.nearClipPlane;
			float farClipPlane = cam.farClipPlane;
			int cullingMask = cam.cullingMask;
			if (gameObject != null)
			{
				light = gameObject.GetComponent<Light>();
				rotation = light.transform.rotation;
				shadows = light.shadows;
				intensity = light.intensity;
				color = light.color;
				light.shadows = LightShadows.None;
				light.color = Color.white;
				light.intensity = 0.5f;
				light.transform.eulerAngles = new Vector3(90f, 0f, 0f);
			}
			cam.nearClipPlane = 0.3f;
			cam.farClipPlane = 1000f;
			cam.cullingMask = 512;
			cam.clearFlags = CameraClearFlags.Color;
			MINIMAP_SIZE = minimapResolution;
			MINIMAP_CORNER_SIZE = (float)MINIMAP_SIZE * cornerSize;
			cornerSizeRatio = cornerSize;
			CreateMinimapRT(cam, minimapResolution);
			if (mapPreset != null)
			{
				initialPreset = mapPreset;
				ManualSetCameraProperties(cam, mapPreset.center, mapPreset.orthographicSize);
			}
			else
			{
				AutomaticSetCameraProperties(cam);
			}
			CaptureMinimapRT(cam);
			if (gameObject != null)
			{
				light.shadows = shadows;
				light.transform.rotation = rotation;
				light.color = color;
				light.intensity = intensity;
			}
			cam.nearClipPlane = nearClipPlane;
			cam.farClipPlane = farClipPlane;
			cam.cullingMask = cullingMask;
			cam.orthographic = false;
			cam.clearFlags = CameraClearFlags.Skybox;
			CreateUnityUIRT(minimapResolution);
			minimapIsCreated = true;
			StartCoroutine(HackRoutine());
		}
	}

	private void CreateMinimapRT(Camera cam, int pixelSize)
	{
		if (minimapRT == null)
		{
			bool flag = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.RGB565);
			RenderTextureFormat renderTextureFormat = (flag ? RenderTextureFormat.RGB565 : RenderTextureFormat.Default);
			minimapRT = new RenderTexture(pixelSize, pixelSize, 16, RenderTextureFormat.RGB565);
			if (!flag)
			{
				Debug.Log(SystemInfo.graphicsDeviceName + " (" + SystemInfo.graphicsDeviceVendor + ") does not support RGB565 format, the minimap will have transparency issues on certain maps");
			}
		}
		cam.targetTexture = minimapRT;
	}

	private void CreateUnityUI(Texture2D map, int minimapResolution)
	{
		GameObject gameObject = new GameObject("Canvas");
		gameObject.AddComponent<RectTransform>();
		canvas = gameObject.AddComponent<Canvas>();
		canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		scaler = gameObject.AddComponent<CanvasScaler>();
		scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
		scaler.referenceResolution = new Vector2(900f, 600f);
		scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
		GameObject gameObject2 = new GameObject("CircleMask");
		gameObject2.transform.SetParent(gameObject.transform, false);
		minimapMaskT = gameObject2.AddComponent<RectTransform>();
		gameObject2.AddComponent<CanvasRenderer>();
		RectTransform rectTransform = minimapMaskT;
		Vector2 anchorMin = (minimapMaskT.anchorMax = Vector2.one);
		rectTransform.anchorMin = anchorMin;
		float num = MINIMAP_CORNER_SIZE * 0.5f;
		cornerPosition = new Vector2(0f - (num + 5f), 0f - (num + 70f));
		minimapMaskT.anchoredPosition = cornerPosition;
		minimapMaskT.sizeDelta = new Vector2(MINIMAP_CORNER_SIZE, MINIMAP_CORNER_SIZE);
		GameObject gameObject3 = new GameObject("Minimap");
		gameObject3.transform.SetParent(minimapMaskT, false);
		minimap = gameObject3.AddComponent<RectTransform>();
		gameObject3.AddComponent<CanvasRenderer>();
		RectTransform rectTransform2 = minimap;
		anchorMin = (minimap.anchorMax = new Vector2(0.5f, 0.5f));
		rectTransform2.anchorMin = anchorMin;
		minimap.anchoredPosition = Vector2.zero;
		minimap.sizeDelta = minimapMaskT.sizeDelta;
		Image image = gameObject3.AddComponent<Image>();
		Rect rect = new Rect(0f, 0f, map.width, map.height);
		image.sprite = UnityEngine.Sprite.Create(map, rect, new Vector3(0.5f, 0.5f));
		image.type = Image.Type.Simple;
	}

	private void CreateUnityUIRT(int minimapResolution)
	{
		GameObject gameObject = new GameObject("Canvas");
		gameObject.AddComponent<RectTransform>();
		canvas = gameObject.AddComponent<Canvas>();
		canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		scaler = gameObject.AddComponent<CanvasScaler>();
		scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
		scaler.referenceResolution = new Vector2(800f, 600f);
		scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
		scaler.matchWidthOrHeight = 1f;
		GameObject gameObject2 = new GameObject("Mask");
		gameObject2.transform.SetParent(gameObject.transform, false);
		minimapMaskT = gameObject2.AddComponent<RectTransform>();
		gameObject2.AddComponent<CanvasRenderer>();
		RectTransform rectTransform = minimapMaskT;
		Vector2 anchorMin = (minimapMaskT.anchorMax = Vector2.one);
		rectTransform.anchorMin = anchorMin;
		float num = MINIMAP_CORNER_SIZE * 0.5f;
		cornerPosition = new Vector2(0f - (num + 5f), 0f - (num + 70f));
		minimapMaskT.anchoredPosition = cornerPosition;
		minimapMaskT.sizeDelta = new Vector2(MINIMAP_CORNER_SIZE, MINIMAP_CORNER_SIZE);
		GameObject gameObject3 = new GameObject("MapBorder");
		gameObject3.transform.SetParent(minimapMaskT, false);
		borderT = gameObject3.AddComponent<RectTransform>();
		RectTransform rectTransform2 = borderT;
		anchorMin = (borderT.anchorMax = new Vector2(0.5f, 0.5f));
		rectTransform2.anchorMin = anchorMin;
		borderT.sizeDelta = minimapMaskT.sizeDelta;
		gameObject3.AddComponent<CanvasRenderer>();
		Image image = gameObject3.AddComponent<Image>();
		image.sprite = borderSprite;
		image.type = Image.Type.Sliced;
		GameObject gameObject4 = new GameObject("Minimap");
		gameObject4.transform.SetParent(minimapMaskT, false);
		minimap = gameObject4.AddComponent<RectTransform>();
		minimap.SetAsFirstSibling();
		gameObject4.AddComponent<CanvasRenderer>();
		RectTransform rectTransform3 = minimap;
		anchorMin = (minimap.anchorMax = new Vector2(0.5f, 0.5f));
		rectTransform3.anchorMin = anchorMin;
		minimap.anchoredPosition = Vector2.zero;
		minimap.sizeDelta = minimapMaskT.sizeDelta;
		RawImage rawImage = gameObject4.AddComponent<RawImage>();
		rawImage.texture = minimapRT;
		rawImage.maskable = true;
		gameObject4.AddComponent<Mask>().showMaskGraphic = true;
	}

	private Vector2 GetSizeForStyle(IconStyle style)
	{
		switch (style)
		{
		case IconStyle.CIRCLE:
			return MINIMAP_ICON_SIZE;
		case IconStyle.SUPPLY:
			return MINIMAP_SUPPLY_SIZE;
		default:
			return Vector2.zero;
		}
	}

	private static UnityEngine.Sprite GetSpriteForStyle(IconStyle style)
	{
		switch (style)
		{
		case IconStyle.CIRCLE:
			return whiteIconSprite;
		case IconStyle.SUPPLY:
			return supplySprite;
		default:
			return null;
		}
	}

	private IEnumerator HackRoutine()
	{
		yield return new WaitForEndOfFrame();
		RecaptureMinimap(lastUsedCamera, lastMinimapCenter, lastMinimapOrthoSize);
	}

	private void Initialize()
	{
		Vector3 vector = new Vector3(0.5f, 0.5f);
		Texture2D texture2D = (Texture2D)FengGameManagerMKII.RCassets.Load("icon");
		Rect rect = new Rect(0f, 0f, texture2D.width, texture2D.height);
		whiteIconSprite = UnityEngine.Sprite.Create(texture2D, rect, vector);
		texture2D = (Texture2D)FengGameManagerMKII.RCassets.Load("iconpointer");
		rect = new Rect(0f, 0f, texture2D.width, texture2D.height);
		pointerSprite = UnityEngine.Sprite.Create(texture2D, rect, vector);
		texture2D = (Texture2D)FengGameManagerMKII.RCassets.Load("supplyicon");
		rect = new Rect(0f, 0f, texture2D.width, texture2D.height);
		supplySprite = UnityEngine.Sprite.Create(texture2D, rect, vector);
		texture2D = (Texture2D)FengGameManagerMKII.RCassets.Load("mapborder");
		borderSprite = UnityEngine.Sprite.Create(rect: new Rect(0f, 0f, texture2D.width, texture2D.height), border: new Vector4(5f, 5f, 5f, 5f), texture: texture2D, pivot: vector, pixelsPerUnit: 100f, extrude: 1u, meshType: SpriteMeshType.FullRect);
		MINIMAP_ICON_SIZE = new Vector2(whiteIconSprite.texture.width, whiteIconSprite.texture.height);
		MINIMAP_POINTER_SIZE = (float)(pointerSprite.texture.width + pointerSprite.texture.height) / 2f;
		MINIMAP_POINTER_DIST = (MINIMAP_ICON_SIZE.x + MINIMAP_ICON_SIZE.y) * 0.25f;
		MINIMAP_SUPPLY_SIZE = new Vector2(supplySprite.texture.width, supplySprite.texture.height);
		assetsInitialized = true;
	}

	private void ManualSetCameraProperties(Camera cam, Vector3 centerPoint, float orthoSize)
	{
		Transform transform = cam.transform;
		centerPoint.y = cam.farClipPlane * 0.5f;
		transform.position = centerPoint;
		transform.eulerAngles = new Vector3(90f, 0f, 0f);
		cam.orthographic = true;
		cam.orthographicSize = orthoSize;
		float num = orthoSize * 2f;
		minimapOrthographicBounds = new Bounds(centerPoint, new Vector3(num, 0f, num));
		lastMinimapCenter = centerPoint;
		lastMinimapOrthoSize = orthoSize;
	}

	private void ManualSetOrthoBounds(Vector3 centerPoint, float orthoSize)
	{
		float num = orthoSize * 2f;
		minimapOrthographicBounds = new Bounds(centerPoint, new Vector3(num, 0f, num));
		lastMinimapCenter = centerPoint;
		lastMinimapOrthoSize = orthoSize;
	}

	public void Maximize()
	{
		isEnabledTemp = true;
		if (!isEnabled)
		{
			SetEnabledTemp(true);
		}
		RectTransform rectTransform = minimapMaskT;
		Vector2 anchorMin = (minimapMaskT.anchorMax = new Vector2(0.5f, 0.5f));
		rectTransform.anchorMin = anchorMin;
		minimapMaskT.anchoredPosition = Vector2.zero;
		minimapMaskT.sizeDelta = new Vector2(MINIMAP_SIZE, MINIMAP_SIZE);
		minimap.sizeDelta = minimapMaskT.sizeDelta;
		borderT.sizeDelta = minimapMaskT.sizeDelta;
		if (minimapIcons != null)
		{
			for (int i = 0; i < minimapIcons.Length; i++)
			{
				MinimapIcon minimapIcon = minimapIcons[i];
				if (minimapIcon != null)
				{
					minimapIcon.SetSize(GetSizeForStyle(minimapIcon.style));
					if (minimapIcon.rotation)
					{
						minimapIcon.SetPointerSize(MINIMAP_POINTER_SIZE, MINIMAP_POINTER_DIST);
					}
				}
			}
		}
		maximized = true;
	}

	public void Minimize()
	{
		isEnabledTemp = false;
		if (!isEnabled)
		{
			SetEnabledTemp(false);
		}
		RectTransform rectTransform = minimapMaskT;
		Vector2 anchorMin = (minimapMaskT.anchorMax = Vector2.one);
		rectTransform.anchorMin = anchorMin;
		minimapMaskT.anchoredPosition = cornerPosition;
		minimapMaskT.sizeDelta = new Vector2(MINIMAP_CORNER_SIZE, MINIMAP_CORNER_SIZE);
		minimap.sizeDelta = minimapMaskT.sizeDelta;
		borderT.sizeDelta = minimapMaskT.sizeDelta;
		if (minimapIcons != null)
		{
			float num = 1f - ((float)MINIMAP_SIZE - MINIMAP_CORNER_SIZE) / (float)MINIMAP_SIZE;
			float a = MINIMAP_POINTER_SIZE * num;
			a = Mathf.Max(a, MINIMAP_POINTER_SIZE * 0.5f);
			float num2 = (MINIMAP_POINTER_SIZE - a) / MINIMAP_POINTER_SIZE;
			num2 = MINIMAP_POINTER_DIST * num2;
			for (int i = 0; i < minimapIcons.Length; i++)
			{
				MinimapIcon minimapIcon = minimapIcons[i];
				if (minimapIcon != null)
				{
					Vector2 sizeForStyle = GetSizeForStyle(minimapIcon.style);
					sizeForStyle.x = Mathf.Max(sizeForStyle.x * num, sizeForStyle.x * 0.5f);
					sizeForStyle.y = Mathf.Max(sizeForStyle.y * num, sizeForStyle.y * 0.5f);
					minimapIcon.SetSize(sizeForStyle);
					if (minimapIcon.rotation)
					{
						minimapIcon.SetPointerSize(a, num2);
					}
				}
			}
		}
		maximized = false;
	}

	public static void OnScreenResolutionChanged()
	{
		if (instance != null && Supported())
		{
			Minimap minimap = instance;
			minimap.StartCoroutine(minimap.ScreenResolutionChangedRoutine());
		}
	}

	private void RecaptureMinimap()
	{
		if (lastUsedCamera != null)
		{
			RecaptureMinimap(lastUsedCamera, lastMinimapCenter, lastMinimapOrthoSize);
		}
	}

	private void RecaptureMinimap(Camera cam, Vector3 centerPosition, float orthoSize)
	{
		if (minimap != null)
		{
			GameObject gameObject = GameObject.Find("mainLight");
			Light light = null;
			Quaternion rotation = Quaternion.identity;
			LightShadows shadows = LightShadows.None;
			Color color = Color.clear;
			float intensity = 0f;
			float nearClipPlane = cam.nearClipPlane;
			float farClipPlane = cam.farClipPlane;
			int cullingMask = cam.cullingMask;
			if (gameObject != null)
			{
				light = gameObject.GetComponent<Light>();
				rotation = light.transform.rotation;
				shadows = light.shadows;
				color = light.color;
				intensity = light.intensity;
				light.shadows = LightShadows.None;
				light.color = Color.white;
				light.intensity = 0.5f;
				light.transform.eulerAngles = new Vector3(90f, 0f, 0f);
			}
			cam.nearClipPlane = 0.3f;
			cam.farClipPlane = 1000f;
			cam.clearFlags = CameraClearFlags.Color;
			cam.cullingMask = 512;
			CreateMinimapRT(cam, MINIMAP_SIZE);
			ManualSetCameraProperties(cam, centerPosition, orthoSize);
			CaptureMinimapRT(cam);
			if (gameObject != null)
			{
				light.shadows = shadows;
				light.transform.rotation = rotation;
				light.color = color;
				light.intensity = intensity;
			}
			cam.nearClipPlane = nearClipPlane;
			cam.farClipPlane = farClipPlane;
			cam.cullingMask = cullingMask;
			cam.orthographic = false;
			cam.clearFlags = CameraClearFlags.Skybox;
		}
	}

	private IEnumerator ScreenResolutionChangedRoutine()
	{
		yield return 0;
		RecaptureMinimap();
	}

	public void SetEnabled(bool enabled)
	{
		isEnabled = enabled;
		if (canvas != null)
		{
			canvas.gameObject.SetActive(enabled);
		}
	}

	public void SetEnabledTemp(bool enabled)
	{
		if (canvas != null)
		{
			canvas.gameObject.SetActive(enabled);
		}
	}

	public void TrackGameObjectOnMinimap(GameObject objToTrack, Color iconColor, bool trackOrientation, bool depthAboveAll = false, IconStyle iconStyle = IconStyle.CIRCLE)
	{
		if (!(minimap != null))
		{
			return;
		}
		MinimapIcon minimapIcon = ((!trackOrientation) ? MinimapIcon.Create(minimap, objToTrack, iconStyle) : MinimapIcon.CreateWithRotation(minimap, objToTrack, iconStyle, MINIMAP_POINTER_DIST));
		minimapIcon.SetColor(iconColor);
		minimapIcon.SetDepth(depthAboveAll);
		Vector2 sizeForStyle = GetSizeForStyle(iconStyle);
		if (maximized)
		{
			minimapIcon.SetSize(sizeForStyle);
			if (minimapIcon.rotation)
			{
				minimapIcon.SetPointerSize(MINIMAP_POINTER_SIZE, MINIMAP_POINTER_DIST);
			}
		}
		else
		{
			float num = 1f - ((float)MINIMAP_SIZE - MINIMAP_CORNER_SIZE) / (float)MINIMAP_SIZE;
			sizeForStyle.x = Mathf.Max(sizeForStyle.x * num, sizeForStyle.x * 0.5f);
			sizeForStyle.y = Mathf.Max(sizeForStyle.y * num, sizeForStyle.y * 0.5f);
			minimapIcon.SetSize(sizeForStyle);
			if (minimapIcon.rotation)
			{
				float a = MINIMAP_POINTER_SIZE * num;
				a = Mathf.Max(a, MINIMAP_POINTER_SIZE * 0.5f);
				float num2 = (MINIMAP_POINTER_SIZE - a) / MINIMAP_POINTER_SIZE;
				num2 = MINIMAP_POINTER_DIST * num2;
				minimapIcon.SetPointerSize(a, num2);
			}
		}
		if (minimapIcons == null)
		{
			minimapIcons = new MinimapIcon[1] { minimapIcon };
			return;
		}
		MinimapIcon[] array = new MinimapIcon[minimapIcons.Length + 1];
		for (int i = 0; i < minimapIcons.Length; i++)
		{
			array[i] = minimapIcons[i];
		}
		array[array.Length - 1] = minimapIcon;
		minimapIcons = array;
	}

	public static void TryRecaptureInstance()
	{
		if (instance != null)
		{
			instance.RecaptureMinimap();
		}
	}

	public IEnumerator TryRecaptureInstanceE(float time)
	{
		yield return new WaitForSeconds(time);
		TryRecaptureInstance();
	}

	private void Update()
	{
		CheckUserInput();
		if ((!isEnabled && !isEnabledTemp) || !minimapIsCreated || minimapIcons == null)
		{
			return;
		}
		for (int i = 0; i < minimapIcons.Length; i++)
		{
			MinimapIcon minimapIcon = minimapIcons[i];
			if (minimapIcon == null)
			{
				RCextensions.RemoveAt(ref minimapIcons, i);
			}
			else if (!minimapIcon.UpdateUI(minimapOrthographicBounds, maximized ? ((float)MINIMAP_SIZE) : MINIMAP_CORNER_SIZE))
			{
				minimapIcon.Destroy();
				RCextensions.RemoveAt(ref minimapIcons, i);
			}
		}
	}

	public static void WaitAndTryRecaptureInstance(float time)
	{
		instance.StartCoroutine(instance.TryRecaptureInstanceE(time));
	}

	private static bool Supported()
	{
		return Application.platform == RuntimePlatform.WindowsPlayer;
	}
}
