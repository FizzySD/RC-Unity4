using System.Collections;
using ApplicationManagers;
using Constants;
using Settings;
using UI;
using UnityEngine;
using Weather;

internal class IN_GAME_MAIN_CAMERA : MonoBehaviour
{
	public enum RotationAxes
	{
		MouseXAndY = 0,
		MouseX = 1,
		MouseY = 2
	}

	public RotationAxes axes;

	public AudioSource bgmusic;

	public static float cameraDistance = 0.6f;

	public static CAMERA_TYPE cameraMode;

	public static int character = 1;

	private float closestDistance;

	private int currentPeekPlayerIndex;

	private float decay;

	public static int difficulty;

	private float distance = 10f;

	private float distanceMulti;

	private float distanceOffsetMulti;

	private float duration;

	private float flashDuration;

	private bool flip;

	public static GAMEMODE gamemode;

	public bool gameOver;

	public static GAMETYPE gametype = GAMETYPE.STOP;

	private bool hasSnapShot;

	private Transform head;

	private float height = 5f;

	private float heightDamping = 2f;

	private float heightMulti;

	public static bool isCheating;

	public static bool isTyping;

	public float justHit;

	public int lastScore;

	public static int level;

	private bool lockAngle;

	private Vector3 lockCameraPosition;

	private GameObject locker;

	private GameObject lockTarget;

	public GameObject main_object;

	public float maximumX = 360f;

	public float maximumY = 60f;

	public float minimumX = -360f;

	public float minimumY = -60f;

	public static bool needSetHUD;

	private float R;

	private float rotationY;

	public int score;

	public static string singleCharacter;

	public Material skyBoxDAWN;

	public Material skyBoxDAY;

	public Material skyBoxNIGHT;

	public GameObject snapShotCamera;

	public RenderTexture snapshotRT;

	public bool spectatorMode;

	public static STEREO_3D_TYPE stereoType;

	private Transform target;

	public Texture texture;

	public float timer;

	public static bool triggerAutoLock;

	public static bool usingTitan;

	private Vector3 verticalHeightOffset = Vector3.zero;

	private float verticalRotationOffset;

	private float xSpeed = -3f;

	private float ySpeed = -0.8f;

	public static IN_GAME_MAIN_CAMERA Instance;

	private Transform _transform;

	private UILabel _bottomRightText;

	private static float _lastRestartTime = 0f;

	private void Awake()
	{
		Cache();
		Instance = this;
		isTyping = false;
		GameMenu.TogglePause(false);
		base.name = "MainCamera";
		ApplyGraphicsSettings();
		CreateMinimap();
		WeatherManager.TakeFlashlight(base.transform);
	}

	public static void ApplyGraphicsSettings()
	{
		Camera main = Camera.main;
		GraphicsSettings graphicsSettings = SettingsManager.GraphicsSettings;
		if (graphicsSettings != null && main != null)
		{
			main.farClipPlane = graphicsSettings.RenderDistance.Value;
		}
	}

	private void Cache()
	{
		_transform = base.transform;
	}

	private void camareMovement()
	{
		Camera camera = base.camera;
		distanceOffsetMulti = cameraDistance * (200f - camera.fieldOfView) / 150f;
		_transform.position = ((head == null) ? main_object.transform.position : head.position);
		_transform.position += Vector3.up * heightMulti;
		_transform.position -= Vector3.up * (0.6f - cameraDistance) * 2f;
		float num = SettingsManager.GeneralSettings.MouseSpeed.Value;
		int num2 = ((!SettingsManager.GeneralSettings.InvertMouse.Value) ? 1 : (-1));
		if (GameMenu.InMenu())
		{
			num = 0f;
		}
		if (cameraMode == CAMERA_TYPE.WOW)
		{
			if (Input.GetKey(KeyCode.Mouse1))
			{
				float angle = Input.GetAxis("Mouse X") * 10f * num;
				float angle2 = (0f - Input.GetAxis("Mouse Y")) * 10f * num * (float)num2;
				_transform.RotateAround(_transform.position, Vector3.up, angle);
				_transform.RotateAround(_transform.position, _transform.right, angle2);
			}
			_transform.position -= _transform.transform.forward * distance * distanceMulti * distanceOffsetMulti;
		}
		else if (cameraMode == CAMERA_TYPE.ORIGINAL)
		{
			float num3 = 0f;
			if (Input.mousePosition.x < (float)Screen.width * 0.4f)
			{
				num3 = (0f - ((float)Screen.width * 0.4f - Input.mousePosition.x) / (float)Screen.width * 0.4f) * getSensitivityMultiWithDeltaTime(num) * 150f;
				_transform.RotateAround(_transform.position, Vector3.up, num3);
			}
			else if (Input.mousePosition.x > (float)Screen.width * 0.6f)
			{
				num3 = (Input.mousePosition.x - (float)Screen.width * 0.6f) / (float)Screen.width * 0.4f * getSensitivityMultiWithDeltaTime(num) * 150f;
				_transform.RotateAround(_transform.position, Vector3.up, num3);
			}
			float x = 140f * ((float)Screen.height * 0.6f - Input.mousePosition.y) / (float)Screen.height * 0.5f;
			_transform.rotation = Quaternion.Euler(x, _transform.rotation.eulerAngles.y, _transform.rotation.eulerAngles.z);
			_transform.position -= _transform.forward * distance * distanceMulti * distanceOffsetMulti;
		}
		else if (cameraMode == CAMERA_TYPE.TPS)
		{
			float angle3 = Input.GetAxis("Mouse X") * 10f * num;
			float num4 = (0f - Input.GetAxis("Mouse Y")) * 10f * num * (float)num2;
			_transform.RotateAround(_transform.position, Vector3.up, angle3);
			float num5 = _transform.rotation.eulerAngles.x % 360f;
			float num6 = num5 + num4;
			if ((num4 <= 0f || ((num5 >= 260f || num6 <= 260f) && (num5 >= 80f || num6 <= 80f))) && (num4 >= 0f || ((num5 <= 280f || num6 >= 280f) && (num5 <= 100f || num6 >= 100f))))
			{
				_transform.RotateAround(_transform.position, _transform.right, num4);
			}
			_transform.position -= _transform.forward * distance * distanceMulti * distanceOffsetMulti;
		}
		if (cameraDistance < 0.65f)
		{
			_transform.position += _transform.right * Mathf.Max((0.6f - cameraDistance) * 2f, 0.65f);
		}
	}

	public void CameraMovementLive(HERO hero)
	{
		float magnitude = hero.rigidbody.velocity.magnitude;
		if (magnitude > 10f)
		{
			Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, Mathf.Min(100f, magnitude + 40f), 0.1f);
		}
		else
		{
			Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 50f, 0.1f);
		}
		float num = hero.CameraMultiplier * (200f - Camera.main.fieldOfView) / 150f;
		base.transform.position = head.transform.position + Vector3.up * heightMulti - Vector3.up * (0.6f - cameraDistance) * 2f;
		base.transform.position -= base.transform.forward * distance * distanceMulti * num;
		if (hero.CameraMultiplier < 0.65f)
		{
			base.transform.position += base.transform.right * Mathf.Max((0.6f - hero.CameraMultiplier) * 2f, 0.65f);
		}
		base.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, hero.GetComponent<SmoothSyncMovement>().correctCameraRot, Time.deltaTime * 5f);
	}

	private void CreateMinimap()
	{
		LevelInfo info = LevelInfo.getInfo(FengGameManagerMKII.level);
		if (info != null)
		{
			Minimap minimap = base.gameObject.AddComponent<Minimap>();
			if (Minimap.instance.myCam == null)
			{
				Minimap.instance.myCam = new GameObject().AddComponent<Camera>();
				Minimap.instance.myCam.nearClipPlane = 0.3f;
				Minimap.instance.myCam.farClipPlane = 1000f;
				Minimap.instance.myCam.enabled = false;
			}
			if (!SettingsManager.GeneralSettings.MinimapEnabled.Value || SettingsManager.LegacyGameSettings.GlobalMinimapDisable.Value)
			{
				minimap.SetEnabled(false);
				Minimap.instance.myCam.gameObject.SetActive(false);
			}
			else
			{
				Minimap.instance.myCam.gameObject.SetActive(true);
				minimap.CreateMinimap(Minimap.instance.myCam, 512, 0.3f, info.minimapPreset);
			}
		}
	}

	public void createSnapShotRT2()
	{
		if (snapshotRT != null)
		{
			snapshotRT.Release();
		}
		if (SettingsManager.GeneralSettings.SnapshotsEnabled.Value)
		{
			snapShotCamera.SetActive(true);
			snapshotRT = new RenderTexture((int)((float)Screen.width * 0.4f), (int)((float)Screen.height * 0.4f), 24);
			snapShotCamera.GetComponent<Camera>().targetTexture = snapshotRT;
		}
		else
		{
			snapShotCamera.SetActive(false);
		}
	}

	private GameObject findNearestTitan()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("titan");
		GameObject result = null;
		float num = (closestDistance = float.PositiveInfinity);
		Vector3 position = main_object.transform.position;
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			float magnitude = (gameObject.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck").position - position).magnitude;
			if (magnitude < num && (gameObject.GetComponent<TITAN>() == null || !gameObject.GetComponent<TITAN>().hasDie))
			{
				result = gameObject;
				num = (closestDistance = magnitude);
			}
		}
		return result;
	}

	public void flashBlind()
	{
		GameObject.Find("flash").GetComponent<UISprite>().alpha = 1f;
		flashDuration = 2f;
	}

	private float getSensitivityMultiWithDeltaTime(float sensitivity)
	{
		return sensitivity * Time.deltaTime * 62f;
	}

	private void reset()
	{
		if (gametype == GAMETYPE.SINGLE)
		{
			GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().restartGameSingle2();
		}
	}

	private Texture2D RTImage2(Camera cam)
	{
		RenderTexture renderTexture = RenderTexture.active;
		RenderTexture.active = cam.targetTexture;
		cam.Render();
		Texture2D texture2D = new Texture2D(cam.targetTexture.width, cam.targetTexture.height, TextureFormat.RGB24, false);
		int num = (int)((float)cam.targetTexture.width * 0.04f);
		int num2 = (int)((float)cam.targetTexture.width * 0.02f);
		try
		{
			texture2D.SetPixel(0, 0, Color.white);
			texture2D.ReadPixels(new Rect(num, num, cam.targetTexture.width - num, cam.targetTexture.height - num), num2, num2);
			RenderTexture.active = renderTexture;
			return texture2D;
		}
		catch
		{
			texture2D = new Texture2D(1, 1);
			texture2D.SetPixel(0, 0, Color.white);
			return texture2D;
		}
	}

	public void UpdateSnapshotSkybox()
	{
		snapShotCamera.gameObject.GetComponent<Skybox>().material = base.gameObject.GetComponent<Skybox>().material;
	}

	private void UpdateBottomRightText()
	{
		if (_bottomRightText == null)
		{
			GameObject gameObject = GameObject.Find("LabelInfoBottomRight");
			if (gameObject != null)
			{
				_bottomRightText = gameObject.GetComponent<UILabel>();
			}
		}
		if (!(_bottomRightText != null))
		{
			return;
		}
		_bottomRightText.text = "Pause : " + SettingsManager.InputSettings.General.Pause.ToString() + " ";
		if (SettingsManager.UISettings.ShowInterpolation.Value && main_object != null)
		{
			HERO component = main_object.GetComponent<HERO>();
			if (component != null && component.baseRigidBody.interpolation == RigidbodyInterpolation.Interpolate)
			{
				_bottomRightText.text = "Interpolation : ON \n" + _bottomRightText.text;
			}
			else
			{
				_bottomRightText.text = "Interpolation: OFF \n" + _bottomRightText.text;
			}
		}
	}

	public void setHUDposition()
	{
		GameObject.Find("Flare").transform.localPosition = new Vector3((int)((float)(-Screen.width) * 0.5f) + 14, (int)((float)(-Screen.height) * 0.5f), 0f);
		GameObject gameObject = GameObject.Find("LabelInfoBottomRight");
		gameObject.transform.localPosition = new Vector3((int)((float)Screen.width * 0.5f), (int)((float)(-Screen.height) * 0.5f), 0f);
		GameObject.Find("LabelInfoTopCenter").transform.localPosition = new Vector3(0f, (int)((float)Screen.height * 0.5f), 0f);
		GameObject.Find("LabelInfoTopRight").transform.localPosition = new Vector3((int)((float)Screen.width * 0.5f), (int)((float)Screen.height * 0.5f), 0f);
		GameObject.Find("LabelNetworkStatus").transform.localPosition = new Vector3((int)((float)(-Screen.width) * 0.5f), (int)((float)Screen.height * 0.5f), 0f);
		GameObject.Find("LabelInfoTopLeft").transform.localPosition = new Vector3((int)((float)(-Screen.width) * 0.5f), (int)((float)Screen.height * 0.5f - 20f), 0f);
		GameObject.Find("Chatroom").transform.localPosition = new Vector3((int)((float)(-Screen.width) * 0.5f), (int)((float)(-Screen.height) * 0.5f), 0f);
		if (GameObject.Find("Chatroom") != null)
		{
			GameObject.Find("Chatroom").GetComponent<InRoomChat>().setPosition();
		}
		if (!usingTitan || gametype == GAMETYPE.SINGLE)
		{
			GameObject.Find("skill_cd_bottom").transform.localPosition = new Vector3(0f, (int)((float)(-Screen.height) * 0.5f + 5f), 0f);
			GameObject.Find("GasUI").transform.localPosition = GameObject.Find("skill_cd_bottom").transform.localPosition;
			GameObject.Find("stamina_titan").transform.localPosition = new Vector3(0f, 9999f, 0f);
			GameObject.Find("stamina_titan_bottom").transform.localPosition = new Vector3(0f, 9999f, 0f);
		}
		else
		{
			Vector3 localPosition = new Vector3(0f, 9999f, 0f);
			GameObject.Find("skill_cd_bottom").transform.localPosition = localPosition;
			GameObject.Find("skill_cd_armin").transform.localPosition = localPosition;
			GameObject.Find("skill_cd_eren").transform.localPosition = localPosition;
			GameObject.Find("skill_cd_jean").transform.localPosition = localPosition;
			GameObject.Find("skill_cd_levi").transform.localPosition = localPosition;
			GameObject.Find("skill_cd_marco").transform.localPosition = localPosition;
			GameObject.Find("skill_cd_mikasa").transform.localPosition = localPosition;
			GameObject.Find("skill_cd_petra").transform.localPosition = localPosition;
			GameObject.Find("skill_cd_sasha").transform.localPosition = localPosition;
			GameObject.Find("GasUI").transform.localPosition = localPosition;
			GameObject.Find("stamina_titan").transform.localPosition = new Vector3(-160f, (int)((float)(-Screen.height) * 0.5f + 15f), 0f);
			GameObject.Find("stamina_titan_bottom").transform.localPosition = new Vector3(-160f, (int)((float)(-Screen.height) * 0.5f + 15f), 0f);
		}
		if (main_object != null && main_object.GetComponent<HERO>() != null)
		{
			if (gametype == GAMETYPE.SINGLE)
			{
				main_object.GetComponent<HERO>().setSkillHUDPosition2();
			}
			else if (main_object.GetPhotonView() != null && main_object.GetPhotonView().isMine)
			{
				main_object.GetComponent<HERO>().setSkillHUDPosition2();
			}
		}
		if (stereoType == STEREO_3D_TYPE.SIDE_BY_SIDE)
		{
			base.gameObject.GetComponent<Camera>().aspect = Screen.width / Screen.height;
		}
		createSnapShotRT2();
	}

	public GameObject setMainObject(GameObject obj, bool resetRotation = true, bool lockAngle = false)
	{
		main_object = obj;
		if (obj == null)
		{
			head = null;
			distanceMulti = (heightMulti = 1f);
		}
		else if (main_object.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head") != null)
		{
			head = main_object.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head");
			distanceMulti = ((head != null) ? (Vector3.Distance(head.transform.position, main_object.transform.position) * 0.2f) : 1f);
			heightMulti = ((head != null) ? (Vector3.Distance(head.transform.position, main_object.transform.position) * 0.33f) : 1f);
			if (resetRotation)
			{
				base.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
			}
		}
		else if (main_object.transform.Find("Amarture/Controller_Body/hip/spine/chest/neck/head") != null)
		{
			head = main_object.transform.Find("Amarture/Controller_Body/hip/spine/chest/neck/head");
			distanceMulti = (heightMulti = 0.64f);
			if (resetRotation)
			{
				base.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
			}
		}
		else
		{
			head = null;
			distanceMulti = (heightMulti = 1f);
			if (resetRotation)
			{
				base.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
			}
		}
		this.lockAngle = lockAngle;
		return obj;
	}

	public GameObject setMainObjectASTITAN(GameObject obj)
	{
		main_object = obj;
		if (main_object.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head") != null)
		{
			head = main_object.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head");
			distanceMulti = ((head != null) ? (Vector3.Distance(head.transform.position, main_object.transform.position) * 0.4f) : 1f);
			heightMulti = ((head != null) ? (Vector3.Distance(head.transform.position, main_object.transform.position) * 0.45f) : 1f);
			base.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
		}
		return obj;
	}

	public void setSpectorMode(bool val)
	{
		spectatorMode = val;
		GameObject.Find("MainCamera").GetComponent<SpectatorMovement>().disable = !val;
		GameObject.Find("MainCamera").GetComponent<MouseLook>().disable = !val;
	}

	private void shakeUpdate()
	{
		if (duration > 0f)
		{
			duration -= Time.deltaTime;
			if (flip)
			{
				base.gameObject.transform.position += Vector3.up * R;
			}
			else
			{
				base.gameObject.transform.position -= Vector3.up * R;
			}
			flip = !flip;
			R *= decay;
		}
	}

	private void Start()
	{
		GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().addCamera(this);
		locker = GameObject.Find("locker");
		cameraDistance = SettingsManager.GeneralSettings.CameraDistance.Value + 0.3f;
		base.camera.farClipPlane = SettingsManager.GraphicsSettings.RenderDistance.Value;
		createSnapShotRT2();
	}

	public void startShake(float R, float duration, float decay = 0.95f)
	{
		if (this.duration < duration)
		{
			this.R = R;
			this.duration = duration;
			this.decay = decay;
		}
	}

	public void startSnapShot2(Vector3 p, int dmg, GameObject target, float startTime)
	{
		if (snapShotCamera.activeSelf && dmg >= SettingsManager.GeneralSettings.SnapshotsMinimumDamage.Value)
		{
			StartCoroutine(CreateSnapshot(p, dmg, target, startTime));
		}
	}

	private IEnumerator CreateSnapshot(Vector3 position, int damage, GameObject target, float startTime)
	{
		UITexture display = GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[0].transform.Find("snapshot1").GetComponent<UITexture>();
		yield return new WaitForSeconds(startTime);
		SetSnapshotPosition(target, position);
		Texture2D snapshot = RTImage2(snapShotCamera.GetComponent<Camera>());
		yield return new WaitForSeconds(0.2f);
		snapshot.Apply();
		display.mainTexture = snapshot;
		display.transform.localScale = new Vector3((float)Screen.width * 0.4f, (float)Screen.height * 0.4f, 1f);
		display.transform.localPosition = new Vector3((float)(-Screen.width) * 0.225f, (float)Screen.height * 0.225f, 0f);
		display.transform.rotation = Quaternion.Euler(0f, 0f, 10f);
		if (SettingsManager.GeneralSettings.SnapshotsShowInGame.Value)
		{
			display.enabled = true;
		}
		else
		{
			display.enabled = false;
		}
		yield return new WaitForSeconds(0.2f);
		SnapshotManager.AddSnapshot(snapshot, damage);
		yield return new WaitForSeconds(2f);
		display.enabled = false;
		Object.Destroy(snapshot);
	}

	private void SetSnapshotPosition(GameObject target, Vector3 snapshotPosition)
	{
		snapShotCamera.transform.position = ((head == null) ? main_object.transform.position : head.transform.position);
		snapShotCamera.transform.position += Vector3.up * heightMulti;
		snapShotCamera.transform.position -= Vector3.up * 1.1f;
		Vector3 position;
		Vector3 vector = (position = snapShotCamera.transform.position);
		Vector3 vector2 = (vector + snapshotPosition) * 0.5f;
		snapShotCamera.transform.position = vector2;
		vector = vector2;
		snapShotCamera.transform.LookAt(snapshotPosition);
		snapShotCamera.transform.RotateAround(base.transform.position, Vector3.up, Random.Range(-20f, 20f));
		snapShotCamera.transform.LookAt(vector);
		snapShotCamera.transform.RotateAround(vector, base.transform.right, Random.Range(-20f, 20f));
		float num = Vector3.Distance(snapshotPosition, position);
		if (target != null && target.GetComponent<TITAN>() != null)
		{
			num += target.transform.localScale.x * 15f;
		}
		snapShotCamera.transform.position -= snapShotCamera.transform.forward * Random.Range(num + 3f, num + 10f);
		snapShotCamera.transform.LookAt(vector);
		snapShotCamera.transform.RotateAround(vector, base.transform.forward, Random.Range(-30f, 30f));
		Vector3 end = ((head == null) ? main_object.transform.position : head.transform.position);
		Vector3 vector3 = ((head == null) ? main_object.transform.position : head.transform.position) - snapShotCamera.transform.position;
		end -= vector3;
		LayerMask layerMask = 1 << LayerMask.NameToLayer("Ground");
		LayerMask layerMask2 = 1 << LayerMask.NameToLayer("EnemyBox");
		LayerMask layerMask3 = (int)layerMask | (int)layerMask2;
		RaycastHit hitInfo;
		if (head != null)
		{
			if (Physics.Linecast(head.transform.position, end, out hitInfo, layerMask))
			{
				snapShotCamera.transform.position = hitInfo.point;
			}
			else if (Physics.Linecast(head.transform.position - vector3 * distanceMulti * 3f, end, out hitInfo, layerMask3))
			{
				snapShotCamera.transform.position = hitInfo.point;
			}
		}
		else if (Physics.Linecast(main_object.transform.position + Vector3.up, end, out hitInfo, layerMask3))
		{
			snapShotCamera.transform.position = hitInfo.point;
		}
	}

	public void update2()
	{
		UpdateBottomRightText();
		if (flashDuration > 0f)
		{
			flashDuration -= Time.deltaTime;
			if (flashDuration <= 0f)
			{
				flashDuration = 0f;
			}
			GameObject.Find("flash").GetComponent<UISprite>().alpha = flashDuration * 0.5f;
		}
		if (gametype == GAMETYPE.STOP)
		{
			return;
		}
		if (gametype != 0 && gameOver)
		{
			if (SettingsManager.InputSettings.Human.AttackSpecial.GetKeyDown())
			{
				if (spectatorMode)
				{
					setSpectorMode(false);
				}
				else
				{
					setSpectorMode(true);
				}
			}
			if (SettingsManager.InputSettings.General.SpectateNextPlayer.GetKeyDown())
			{
				currentPeekPlayerIndex++;
				int num = GameObject.FindGameObjectsWithTag("Player").Length;
				if (currentPeekPlayerIndex >= num)
				{
					currentPeekPlayerIndex = 0;
				}
				if (num > 0)
				{
					setMainObject(GameObject.FindGameObjectsWithTag("Player")[currentPeekPlayerIndex]);
					setSpectorMode(false);
					lockAngle = false;
				}
			}
			if (SettingsManager.InputSettings.General.SpectatePreviousPlayer.GetKeyDown())
			{
				currentPeekPlayerIndex--;
				int num2 = GameObject.FindGameObjectsWithTag("Player").Length;
				if (currentPeekPlayerIndex >= num2)
				{
					currentPeekPlayerIndex = 0;
				}
				if (currentPeekPlayerIndex < 0)
				{
					currentPeekPlayerIndex = num2 - 1;
				}
				if (num2 > 0)
				{
					setMainObject(GameObject.FindGameObjectsWithTag("Player")[currentPeekPlayerIndex]);
					setSpectorMode(false);
					lockAngle = false;
				}
			}
			if (spectatorMode)
			{
				return;
			}
		}
		if (GameMenu.Paused)
		{
			if (main_object != null)
			{
				Vector3 position = base.transform.position;
				position = ((head == null) ? main_object.transform.position : head.transform.position);
				position += Vector3.up * heightMulti;
				base.transform.position = Vector3.Lerp(base.transform.position, position - base.transform.forward * 5f, 0.2f);
			}
			return;
		}
		if (SettingsManager.InputSettings.General.Pause.GetKeyDown())
		{
			GameMenu.TogglePause(true);
		}
		if (needSetHUD)
		{
			needSetHUD = false;
			setHUDposition();
		}
		if (SettingsManager.InputSettings.General.ToggleFullscreen.GetKeyDown())
		{
			FullscreenHandler.ToggleFullscreen();
			needSetHUD = true;
		}
		if (SettingsManager.InputSettings.General.RestartGame.GetKeyDown())
		{
			float num3 = Time.realtimeSinceStartup - _lastRestartTime;
			if (gametype != 0 && PhotonNetwork.isMasterClient && num3 > 2f)
			{
				_lastRestartTime = Time.realtimeSinceStartup;
				object[] parameters = new object[2] { "<color=#FFCC00>MasterClient has restarted the game!</color>", "" };
				FengGameManagerMKII.instance.photonView.RPC("Chat", PhotonTargets.All, parameters);
				FengGameManagerMKII.instance.restartRC();
			}
		}
		if (SettingsManager.InputSettings.General.RestartGame.GetKeyDown() || SettingsManager.InputSettings.General.ChangeCharacter.GetKeyDown())
		{
			reset();
		}
		if (!(main_object != null))
		{
			return;
		}
		if (SettingsManager.InputSettings.General.ChangeCamera.GetKeyDown())
		{
			if (cameraMode == CAMERA_TYPE.ORIGINAL)
			{
				cameraMode = CAMERA_TYPE.WOW;
			}
			else if (cameraMode == CAMERA_TYPE.WOW)
			{
				cameraMode = CAMERA_TYPE.TPS;
			}
			else if (cameraMode == CAMERA_TYPE.TPS)
			{
				cameraMode = CAMERA_TYPE.ORIGINAL;
			}
			verticalRotationOffset = 0f;
		}
		if (SettingsManager.InputSettings.General.HideUI.GetKeyDown())
		{
			GameMenu.HideCrosshair = !GameMenu.HideCrosshair;
		}
		if (SettingsManager.InputSettings.Human.FocusTitan.GetKeyDown())
		{
			triggerAutoLock = !triggerAutoLock;
			if (triggerAutoLock)
			{
				lockTarget = findNearestTitan();
				if (closestDistance >= 150f)
				{
					lockTarget = null;
					triggerAutoLock = false;
				}
			}
		}
		if (gameOver && main_object != null)
		{
			if (SettingsManager.InputSettings.General.SpectateToggleLive.GetKeyDown())
			{
				SettingsManager.LegacyGeneralSettings.LiveSpectate.Value = !SettingsManager.LegacyGeneralSettings.LiveSpectate.Value;
			}
			HERO component = main_object.GetComponent<HERO>();
			if (component != null && SettingsManager.LegacyGeneralSettings.LiveSpectate.Value && component.GetComponent<SmoothSyncMovement>().enabled && component.isPhotonCamera)
			{
				CameraMovementLive(component);
			}
			else if (lockAngle)
			{
				base.transform.rotation = Quaternion.Lerp(base.transform.rotation, main_object.transform.rotation, 0.2f);
				base.transform.position = Vector3.Lerp(base.transform.position, main_object.transform.position - main_object.transform.forward * 5f, 0.2f);
			}
			else
			{
				camareMovement();
			}
		}
		else
		{
			camareMovement();
		}
		if (triggerAutoLock && lockTarget != null)
		{
			float z = base.transform.eulerAngles.z;
			Transform transform = lockTarget.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck");
			Vector3 vector = transform.position - ((head == null) ? main_object.transform.position : head.transform.position);
			vector.Normalize();
			lockCameraPosition = ((head == null) ? main_object.transform.position : head.transform.position);
			lockCameraPosition -= vector * distance * distanceMulti * distanceOffsetMulti;
			lockCameraPosition += Vector3.up * 3f * heightMulti * distanceOffsetMulti;
			base.transform.position = Vector3.Lerp(base.transform.position, lockCameraPosition, Time.deltaTime * 4f);
			if (head != null)
			{
				base.transform.LookAt(head.transform.position * 0.8f + transform.position * 0.2f);
			}
			else
			{
				base.transform.LookAt(main_object.transform.position * 0.8f + transform.position * 0.2f);
			}
			base.transform.localEulerAngles = new Vector3(base.transform.eulerAngles.x, base.transform.eulerAngles.y, z);
			Vector2 vector2 = base.camera.WorldToScreenPoint(transform.position - transform.forward * lockTarget.transform.localScale.x);
			locker.transform.localPosition = new Vector3(vector2.x - (float)Screen.width * 0.5f, vector2.y - (float)Screen.height * 0.5f, 0f);
			if (lockTarget.GetComponent<TITAN>() != null && lockTarget.GetComponent<TITAN>().hasDie)
			{
				lockTarget = null;
			}
		}
		else
		{
			locker.transform.localPosition = new Vector3(0f, (float)(-Screen.height) * 0.5f - 50f, 0f);
		}
		Vector3 end = ((head == null) ? main_object.transform.position : head.position);
		Vector3 normalized = (((head == null) ? main_object.transform.position : head.position) - _transform.position).normalized;
		end -= distance * normalized * distanceMulti;
		LayerMask layerMask = 1 << PhysicsLayer.Ground;
		LayerMask layerMask2 = 1 << PhysicsLayer.EnemyBox;
		LayerMask layerMask3 = (int)layerMask | (int)layerMask2;
		RaycastHit hitInfo;
		if (head != null)
		{
			if (Physics.Linecast(head.position, end, out hitInfo, layerMask))
			{
				_transform.position = hitInfo.point;
			}
			else if (Physics.Linecast(head.position - normalized * distanceMulti * 3f, end, out hitInfo, layerMask2))
			{
				_transform.position = hitInfo.point;
			}
		}
		else if (Physics.Linecast(main_object.transform.position + Vector3.up, end, out hitInfo, layerMask3))
		{
			_transform.position = hitInfo.point;
		}
		shakeUpdate();
	}
}
