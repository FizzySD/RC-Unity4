using System;
using System.Collections;
using System.Collections.Generic;
using Constants;
using CustomSkins;
using ExitGames.Client.Photon;
using GameProgress;
using Photon;
using Settings;
using UI;
using UnityEngine;
using Weather;
using Xft;

internal class HERO : Photon.MonoBehaviour
{
	public bool hasAlreadyBursted;

	public bool isGasBursting;

	public Vector3 ReplayLeftHookPos;

	public Vector3 ReplayRightHookPos;

	private HERO_STATE _state;

	private bool almostSingleHook;

	private string attackAnimation;

	private int attackLoop;

	private bool attackMove;

	private bool attackReleased;

	private GameObject badGuy;

	public Animation baseAnimation;

	public Rigidbody baseRigidBody;

	public Transform baseTransform;

	private bool bigLean;

	public float bombCD;

	public bool bombImmune;

	public float bombRadius;

	public float bombSpeed;

	public float bombTime;

	public float bombTimeMax;

	private float buffTime;

	public GameObject bulletLeft;

	private int bulletMAX = 7;

	public GameObject bulletRight;

	private bool buttonAttackRelease;

	public Dictionary<string, UISprite> cachedSprites;

	public float CameraMultiplier;

	public bool canJump = true;

	public GameObject checkBoxLeft;

	public GameObject checkBoxRight;

	public GameObject cross1;

	public GameObject cross2;

	public GameObject crossL1;

	public GameObject crossL2;

	public GameObject crossR1;

	public GameObject crossR2;

	public string currentAnimation;

	private int currentBladeNum = 5;

	private float currentBladeSta = 100f;

	private BUFF currentBuff;

	public Camera currentCamera;

	private float currentGas = 100f;

	public float currentSpeed;

	private bool dashD;

	private Vector3 dashDirection;

	public bool dashL;

	public bool dashR;

	private float dashTime;

	public bool dashU;

	public Vector3 dashV;

	public bool detonate;

	private float dTapTime = -1f;

	private bool EHold;

	private GameObject eren_titan;

	private int escapeTimes = 1;

	private float facingDirection;

	private float flare1CD;

	private float flare2CD;

	private float flare3CD;

	private float flareTotalCD = 30f;

	private Transform forearmL;

	private Transform forearmR;

	private float gravity = 20f;

	private bool grounded;

	private GameObject gunDummy;

	private Vector3 gunTarget;

	private Transform handL;

	private Transform handR;

	public bool hasDied;

	public bool hasspawn;

	private bool hookBySomeOne = true;

	public GameObject hookRefL1;

	public GameObject hookRefL2;

	public GameObject hookRefR1;

	public GameObject hookRefR2;

	private bool hookSomeOne;

	private GameObject hookTarget;

	private float invincible = 3f;

	public bool isCannon;

	public bool isLaunchLeft;

	public bool isLaunchRight;

	private bool isLeftHandHooked;

	private bool isMounted;

	public bool isPhotonCamera;

	private bool isRightHandHooked;

	public float jumpHeight = 2f;

	private bool justGrounded;

	public GameObject LabelDistance;

	public Transform lastHook;

	private float launchElapsedTimeL;

	private float launchElapsedTimeR;

	private Vector3 launchForce;

	private Vector3 launchPointLeft;

	private Vector3 launchPointRight;

	private bool leanLeft;

	private bool leftArmAim;

	public XWeaponTrail leftbladetrail;

	public XWeaponTrail leftbladetrail2;

	private int leftBulletLeft = 7;

	private bool leftGunHasBullet = true;

	private float lTapTime = -1f;

	public GameObject maincamera;

	public float maxVelocityChange = 10f;

	public AudioSource meatDie;

	public Bomb myBomb;

	public GameObject myCannon;

	public Transform myCannonBase;

	public Transform myCannonPlayer;

	public CannonPropRegion myCannonRegion;

	public GROUP myGroup;

	private GameObject myHorse;

	public GameObject myNetWorkName;

	public float myScale = 1f;

	public int myTeam = 1;

	public List<TITAN> myTitans;

	private bool needLean;

	private Quaternion oldHeadRotation;

	private float originVM;

	private bool QHold;

	private string reloadAnimation = string.Empty;

	private bool rightArmAim;

	public XWeaponTrail rightbladetrail;

	public XWeaponTrail rightbladetrail2;

	private int rightBulletLeft = 7;

	private bool rightGunHasBullet = true;

	public AudioSource rope;

	private float rTapTime = -1f;

	public HERO_SETUP setup;

	private GameObject skillCD;

	public float skillCDDuration;

	public float skillCDLast;

	public float skillCDLastCannon;

	private string skillId;

	public string skillIDHUD;

	public AudioSource slash;

	public AudioSource slashHit;

	public ParticleSystem smoke_3dmg;

	private ParticleSystem sparks;

	public float speed = 10f;

	public GameObject speedFX;

	public GameObject speedFX1;

	private ParticleSystem speedFXPS;

	private bool spinning;

	private string standAnimation = "stand";

	private Quaternion targetHeadRotation;

	private Quaternion targetRotation;

	private bool throwedBlades;

	public bool titanForm;

	private GameObject titanWhoGrabMe;

	private int titanWhoGrabMeID;

	private int totalBladeNum = 5;

	public float totalBladeSta = 100f;

	public float totalGas = 100f;

	private Transform upperarmL;

	private Transform upperarmR;

	private float useGasSpeed = 0.2f;

	public bool useGun;

	private float uTapTime = -1f;

	private bool wallJump;

	private float wallRunTime;

	private float _reelInAxis;

	private float _reelOutAxis;

	private float _reelOutScrollTimeLeft;

	private bool _animationStopped;

	private GameObject ThunderSpearL;

	private GameObject ThunderSpearR;

	public GameObject ThunderSpearLModel;

	public GameObject ThunderSpearRModel;

	private bool _hasRunStart;

	private bool _needSetupThunderspears;

	public HumanCustomSkinLoader _customSkinLoader;

	private bool _cancelGasDisable;

	private float _currentEmoteActionTime;

	public float _flareDelayAfterEmote;

	private float _dashCooldownLeft;

	public bool isGrabbed
	{
		get
		{
			return state == HERO_STATE.Grab;
		}
	}

	private HERO_STATE state
	{
		get
		{
			return _state;
		}
		set
		{
			if (_state == HERO_STATE.AirDodge || _state == HERO_STATE.GroundDodge)
			{
				dashTime = 0f;
			}
			_state = value;
		}
	}

	public bool IsMine()
	{
		if (IN_GAME_MAIN_CAMERA.gametype != 0)
		{
			return base.photonView.isMine;
		}
		return true;
	}

	public void EmoteAction(string animation)
	{
		if (state != HERO_STATE.Grab && state != HERO_STATE.AirDodge)
		{
			state = HERO_STATE.Salute;
			crossFade(animation, 0.1f);
			_currentEmoteActionTime = baseAnimation[animation].length;
		}
	}

	private void UpdateInput()
	{
		if (!GameMenu.Paused)
		{
			if (SettingsManager.InputSettings.Interaction.EmoteMenu.GetKeyDown())
			{
				GameMenu.ToggleEmoteWheel(!GameMenu.WheelMenu);
			}
			if (SettingsManager.InputSettings.Interaction.MenuNext.GetKeyDown())
			{
				GameMenu.NextEmoteWheel();
			}
		}
		UpdateReelInput();
	}

	private void UpdateReelInput()
	{
		_reelOutScrollTimeLeft -= Time.deltaTime;
		if (_reelOutScrollTimeLeft <= 0f)
		{
			_reelOutAxis = 0f;
		}
		if (SettingsManager.InputSettings.Human.ReelIn.GetKey())
		{
			_reelInAxis = -1f;
		}
		foreach (InputKey inputKey in SettingsManager.InputSettings.Human.ReelOut.InputKeys)
		{
			if (inputKey.GetKey())
			{
				_reelOutAxis = 1f;
				if (inputKey.IsWheel())
				{
					_reelOutScrollTimeLeft = SettingsManager.InputSettings.Human.ReelOutScrollSmoothing.Value;
				}
			}
		}
	}

	private float GetReelAxis()
	{
		if (_reelInAxis != 0f)
		{
			return _reelInAxis;
		}
		return _reelOutAxis;
	}

	private void SetupThunderSpears()
	{
		if (base.photonView.isMine)
		{
			base.photonView.RPC("SetupThunderSpearsRPC", PhotonTargets.AllBuffered);
		}
	}

	[RPC]
	private void SetupThunderSpearsRPC(PhotonMessageInfo info)
	{
		if (info.sender == base.photonView.owner)
		{
			if (!_hasRunStart)
			{
				_needSetupThunderspears = true;
			}
			else
			{
				CreateAndAttachThunderSpears();
			}
		}
	}

	private void CreateAndAttachThunderSpears()
	{
		ThunderSpearL = (GameObject)UnityEngine.Object.Instantiate(FengGameManagerMKII.RCassets.Load("ThunderSpearProp"));
		ThunderSpearR = (GameObject)UnityEngine.Object.Instantiate(FengGameManagerMKII.RCassets.Load("ThunderSpearProp"));
		ThunderSpearLModel = ThunderSpearL.transform.Find("ThunderSpearModel").gameObject;
		ThunderSpearRModel = ThunderSpearR.transform.Find("ThunderSpearModel").gameObject;
		AttachThunderSpear(ThunderSpearL, handL.transform, true);
		AttachThunderSpear(ThunderSpearR, handR.transform, false);
		currentBladeNum = (totalBladeNum = 0);
		totalBladeSta = (currentBladeSta = 0f);
		setup.part_blade_l.SetActive(false);
		setup.part_blade_r.SetActive(false);
	}

	private void AttachThunderSpear(GameObject thunderSpear, Transform mount, bool left)
	{
		thunderSpear.transform.parent = mount.parent;
		Vector3 localPosition = (left ? new Vector3(-0.001649f, 0.000775f, -0.000227f) : new Vector3(-0.001649f, -0.000775f, -0.000227f));
		Quaternion localRotation = (left ? Quaternion.Euler(5f, -85f, 10f) : Quaternion.Euler(-5f, -85f, -10f));
		thunderSpear.transform.localPosition = localPosition;
		thunderSpear.transform.localRotation = localRotation;
	}

	private void SetThunderSpears(bool hasLeft, bool hasRight)
	{
		base.photonView.RPC("SetThunderSpearsRPC", PhotonTargets.All, hasLeft, hasRight);
	}

	[RPC]
	private void SetThunderSpearsRPC(bool hasLeft, bool hasRight, PhotonMessageInfo info)
	{
		if (info.sender == base.photonView.owner)
		{
			if (ThunderSpearLModel != null)
			{
				ThunderSpearLModel.SetActive(hasLeft);
			}
			if (ThunderSpearRModel != null)
			{
				ThunderSpearRModel.SetActive(hasRight);
			}
		}
	}

	private void applyForceToBody(GameObject GO, Vector3 v)
	{
		GO.rigidbody.AddForce(v);
		GO.rigidbody.AddTorque(UnityEngine.Random.Range(-10f, 10f), UnityEngine.Random.Range(-10f, 10f), UnityEngine.Random.Range(-10f, 10f));
	}

	public void attackAccordingToMouse()
	{
		if ((double)Input.mousePosition.x < (double)Screen.width * 0.5)
		{
			attackAnimation = "attack2";
		}
		else
		{
			attackAnimation = "attack1";
		}
	}

	public void attackAccordingToTarget(Transform a)
	{
		Vector3 vector = a.position - base.transform.position;
		float current = (0f - Mathf.Atan2(vector.z, vector.x)) * 57.29578f;
		float num = 0f - Mathf.DeltaAngle(current, base.transform.rotation.eulerAngles.y - 90f);
		if (Mathf.Abs(num) < 90f && vector.magnitude < 6f && a.position.y <= base.transform.position.y + 2f && a.position.y >= base.transform.position.y - 5f)
		{
			attackAnimation = "attack4";
		}
		else if (num > 0f)
		{
			attackAnimation = "attack1";
		}
		else
		{
			attackAnimation = "attack2";
		}
	}

	private void Awake()
	{
		cache();
		setup = base.gameObject.GetComponent<HERO_SETUP>();
		baseRigidBody.freezeRotation = true;
		baseRigidBody.useGravity = false;
		handL = baseTransform.Find("Amarture/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L/forearm_L/hand_L");
		handR = baseTransform.Find("Amarture/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R");
		forearmL = baseTransform.Find("Amarture/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L/forearm_L");
		forearmR = baseTransform.Find("Amarture/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R");
		upperarmL = baseTransform.Find("Amarture/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L");
		upperarmR = baseTransform.Find("Amarture/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R");
		_customSkinLoader = base.gameObject.AddComponent<HumanCustomSkinLoader>();
	}

	public void backToHuman()
	{
		base.gameObject.GetComponent<SmoothSyncMovement>().disabled = false;
		base.rigidbody.velocity = Vector3.zero;
		titanForm = false;
		ungrabbed();
		falseAttack();
		skillCDDuration = skillCDLast;
		GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(base.gameObject);
		if (IN_GAME_MAIN_CAMERA.gametype != 0)
		{
			base.photonView.RPC("backToHumanRPC", PhotonTargets.Others);
		}
	}

	[RPC]
	private void backToHumanRPC()
	{
		titanForm = false;
		eren_titan = null;
		base.gameObject.GetComponent<SmoothSyncMovement>().disabled = false;
	}

	[RPC]
	public void badGuyReleaseMe()
	{
		hookBySomeOne = false;
		badGuy = null;
	}

	[RPC]
	public void blowAway(Vector3 force, PhotonMessageInfo info)
	{
		if (info != null)
		{
			if (Math.Abs(force.x) > 500f || Math.Abs(force.y) > 500f || Math.Abs(force.z) > 500f)
			{
				FengGameManagerMKII.instance.kickPlayerRCIfMC(info.sender, true, "hero blowaway exploit");
				return;
			}
			if (!info.sender.isMasterClient && (Convert.ToInt32(info.sender.customProperties[PhotonPlayerProperty.isTitan]) == 1 || Convert.ToBoolean(info.sender.customProperties[PhotonPlayerProperty.dead])))
			{
				return;
			}
		}
		if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE || base.photonView.isMine)
		{
			base.rigidbody.AddForce(force, ForceMode.Impulse);
			base.transform.LookAt(base.transform.position);
		}
	}

	private void bodyLean()
	{
		if (IN_GAME_MAIN_CAMERA.gametype != 0 && !base.photonView.isMine)
		{
			return;
		}
		float z = 0f;
		needLean = false;
		if (!useGun && state == HERO_STATE.Attack && attackAnimation != "attack3_1" && attackAnimation != "attack3_2" && !IsFiringThunderSpear())
		{
			float y = base.rigidbody.velocity.y;
			float x = base.rigidbody.velocity.x;
			float z2 = base.rigidbody.velocity.z;
			float x2 = Mathf.Sqrt(x * x + z2 * z2);
			float num = Mathf.Atan2(y, x2) * 57.29578f;
			targetRotation = Quaternion.Euler((0f - num) * (1f - Vector3.Angle(base.rigidbody.velocity, base.transform.forward) / 90f), facingDirection, 0f);
			if ((isLeftHandHooked && bulletLeft != null) || (isRightHandHooked && bulletRight != null))
			{
				base.transform.rotation = targetRotation;
			}
			return;
		}
		if (isLeftHandHooked && bulletLeft != null && isRightHandHooked && bulletRight != null)
		{
			if (almostSingleHook)
			{
				needLean = true;
				z = getLeanAngle(bulletRight.transform.position, true);
			}
		}
		else if (isLeftHandHooked && bulletLeft != null)
		{
			needLean = true;
			z = getLeanAngle(bulletLeft.transform.position, true);
		}
		else if (isRightHandHooked && bulletRight != null)
		{
			needLean = true;
			z = getLeanAngle(bulletRight.transform.position, false);
		}
		if (needLean)
		{
			float num2 = 0f;
			if (!useGun && state != HERO_STATE.Attack)
			{
				num2 = currentSpeed * 0.1f;
				num2 = Mathf.Min(num2, 20f);
			}
			targetRotation = Quaternion.Euler(0f - num2, facingDirection, z);
		}
		else if (state != HERO_STATE.Attack)
		{
			targetRotation = Quaternion.Euler(0f, facingDirection, 0f);
		}
	}

	public void bombInit()
	{
		skillIDHUD = skillId;
		skillCDDuration = skillCDLast;
		if (SettingsManager.LegacyGameSettings.BombModeEnabled.Value)
		{
			int num = SettingsManager.AbilitySettings.BombRadius.Value;
			int num2 = SettingsManager.AbilitySettings.BombCooldown.Value;
			int num3 = SettingsManager.AbilitySettings.BombSpeed.Value;
			int num4 = SettingsManager.AbilitySettings.BombRange.Value;
			if (num + num2 + num3 + num4 > 16)
			{
				num = (num3 = 6);
				num4 = 3;
				num2 = 1;
			}
			bombTimeMax = ((float)num4 * 60f + 200f) / ((float)num3 * 60f + 200f);
			bombRadius = (float)num * 4f + 20f;
			bombCD = (float)(num2 + 4) * -0.4f + 5f;
			bombSpeed = (float)num3 * 60f + 200f;
			ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
			hashtable.Add(PhotonPlayerProperty.RCBombR, SettingsManager.AbilitySettings.BombColor.Value.r);
			hashtable.Add(PhotonPlayerProperty.RCBombG, SettingsManager.AbilitySettings.BombColor.Value.g);
			hashtable.Add(PhotonPlayerProperty.RCBombB, SettingsManager.AbilitySettings.BombColor.Value.b);
			hashtable.Add(PhotonPlayerProperty.RCBombA, SettingsManager.AbilitySettings.BombColor.Value.a);
			hashtable.Add(PhotonPlayerProperty.RCBombRadius, bombRadius);
			PhotonNetwork.player.SetCustomProperties(hashtable);
			skillId = "bomb";
			skillIDHUD = "armin";
			skillCDLast = bombCD;
			skillCDDuration = 10f;
			if (FengGameManagerMKII.instance.roundTime > 10f)
			{
				skillCDDuration = 5f;
			}
		}
	}

	public void teleport(Vector3 position)
	{
		base.transform.position = position;
	}

	private void breakApart2(Vector3 v, bool isBite)
	{
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Character_parts/AOTTG_HERO_body"), base.transform.position, base.transform.rotation);
		gameObject.gameObject.GetComponent<HERO_SETUP>().myCostume = setup.myCostume;
		gameObject.GetComponent<HERO_SETUP>().isDeadBody = true;
		gameObject.GetComponent<HERO_DEAD_BODY_SETUP>().init(currentAnimation, base.animation[currentAnimation].normalizedTime, BODY_PARTS.ARM_R);
		if (!isBite)
		{
			GameObject gameObject2 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Character_parts/AOTTG_HERO_body"), base.transform.position, base.transform.rotation);
			GameObject gameObject3 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Character_parts/AOTTG_HERO_body"), base.transform.position, base.transform.rotation);
			GameObject gameObject4 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Character_parts/AOTTG_HERO_body"), base.transform.position, base.transform.rotation);
			gameObject2.gameObject.GetComponent<HERO_SETUP>().myCostume = setup.myCostume;
			gameObject3.gameObject.GetComponent<HERO_SETUP>().myCostume = setup.myCostume;
			gameObject4.gameObject.GetComponent<HERO_SETUP>().myCostume = setup.myCostume;
			gameObject2.GetComponent<HERO_SETUP>().isDeadBody = true;
			gameObject3.GetComponent<HERO_SETUP>().isDeadBody = true;
			gameObject4.GetComponent<HERO_SETUP>().isDeadBody = true;
			gameObject2.GetComponent<HERO_DEAD_BODY_SETUP>().init(currentAnimation, base.animation[currentAnimation].normalizedTime, BODY_PARTS.UPPER);
			gameObject3.GetComponent<HERO_DEAD_BODY_SETUP>().init(currentAnimation, base.animation[currentAnimation].normalizedTime, BODY_PARTS.LOWER);
			gameObject4.GetComponent<HERO_DEAD_BODY_SETUP>().init(currentAnimation, base.animation[currentAnimation].normalizedTime, BODY_PARTS.ARM_L);
			applyForceToBody(gameObject2, v);
			applyForceToBody(gameObject3, v);
			applyForceToBody(gameObject4, v);
			if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE || base.photonView.isMine)
			{
				currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(gameObject2, false);
			}
		}
		else if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE || base.photonView.isMine)
		{
			currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(gameObject, false);
		}
		applyForceToBody(gameObject, v);
		Transform transform = base.transform.Find("Amarture/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L/forearm_L/hand_L").transform;
		Transform transform2 = base.transform.Find("Amarture/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R").transform;
		GameObject gameObject5;
		GameObject gameObject6;
		GameObject gameObject7;
		GameObject gameObject8;
		GameObject gameObject9;
		if (useGun)
		{
			gameObject5 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Character_parts/character_gun_l"), transform.position, transform.rotation);
			gameObject6 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Character_parts/character_gun_r"), transform2.position, transform2.rotation);
			gameObject7 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Character_parts/character_3dmg_2"), base.transform.position, base.transform.rotation);
			gameObject8 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Character_parts/character_gun_mag_l"), base.transform.position, base.transform.rotation);
			gameObject9 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Character_parts/character_gun_mag_r"), base.transform.position, base.transform.rotation);
		}
		else
		{
			gameObject5 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Character_parts/character_blade_l"), transform.position, transform.rotation);
			gameObject6 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Character_parts/character_blade_r"), transform2.position, transform2.rotation);
			gameObject7 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Character_parts/character_3dmg"), base.transform.position, base.transform.rotation);
			gameObject8 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Character_parts/character_3dmg_gas_l"), base.transform.position, base.transform.rotation);
			gameObject9 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Character_parts/character_3dmg_gas_r"), base.transform.position, base.transform.rotation);
		}
		gameObject5.renderer.material = CharacterMaterials.materials[setup.myCostume._3dmg_texture];
		gameObject6.renderer.material = CharacterMaterials.materials[setup.myCostume._3dmg_texture];
		gameObject7.renderer.material = CharacterMaterials.materials[setup.myCostume._3dmg_texture];
		gameObject8.renderer.material = CharacterMaterials.materials[setup.myCostume._3dmg_texture];
		gameObject9.renderer.material = CharacterMaterials.materials[setup.myCostume._3dmg_texture];
		applyForceToBody(gameObject5, v);
		applyForceToBody(gameObject6, v);
		applyForceToBody(gameObject7, v);
		applyForceToBody(gameObject8, v);
		applyForceToBody(gameObject9, v);
	}

	private void bufferUpdate()
	{
		if (!(buffTime > 0f))
		{
			return;
		}
		buffTime -= Time.deltaTime;
		if (buffTime <= 0f)
		{
			buffTime = 0f;
			if (currentBuff == BUFF.SpeedUp && base.animation.IsPlaying("run_sasha"))
			{
				crossFade("run", 0.1f);
			}
			currentBuff = BUFF.NoBuff;
		}
	}

	public void cache()
	{
		baseTransform = base.transform;
		baseRigidBody = base.rigidbody;
		maincamera = GameObject.Find("MainCamera");
		if (IN_GAME_MAIN_CAMERA.gametype != 0 && !base.photonView.isMine)
		{
			return;
		}
		baseAnimation = base.animation;
		cross1 = GameObject.Find("cross1");
		cross2 = GameObject.Find("cross2");
		crossL1 = GameObject.Find("crossL1");
		crossL2 = GameObject.Find("crossL2");
		crossR1 = GameObject.Find("crossR1");
		crossR2 = GameObject.Find("crossR2");
		LabelDistance = GameObject.Find("LabelDistance");
		cachedSprites = new Dictionary<string, UISprite>();
		UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(GameObject));
		for (int i = 0; i < array.Length; i++)
		{
			GameObject gameObject = (GameObject)array[i];
			if (gameObject.GetComponent<UISprite>() != null && gameObject.activeInHierarchy)
			{
				string text = gameObject.name;
				if ((text.Contains("blade") || text.Contains("bullet") || text.Contains("gas") || text.Contains("flare") || text.Contains("skill_cd")) && !cachedSprites.ContainsKey(text))
				{
					cachedSprites.Add(text, gameObject.GetComponent<UISprite>());
				}
			}
		}
		SetupCrosshairs();
	}

	private void SetupCrosshairs()
	{
		cross1.transform.localPosition = Vector3.up * 10000f;
		cross2.transform.localPosition = Vector3.up * 10000f;
		LabelDistance.transform.localPosition = Vector3.up * 10000f;
	}

	private void calcFlareCD()
	{
		if (flare1CD > 0f)
		{
			flare1CD -= Time.deltaTime;
			if (flare1CD < 0f)
			{
				flare1CD = 0f;
			}
		}
		if (flare2CD > 0f)
		{
			flare2CD -= Time.deltaTime;
			if (flare2CD < 0f)
			{
				flare2CD = 0f;
			}
		}
		if (flare3CD > 0f)
		{
			flare3CD -= Time.deltaTime;
			if (flare3CD < 0f)
			{
				flare3CD = 0f;
			}
		}
	}

	private void calcSkillCD()
	{
		if (skillCDDuration > 0f)
		{
			skillCDDuration -= Time.deltaTime;
			if (skillCDDuration < 0f)
			{
				skillCDDuration = 0f;
			}
		}
	}

	private float CalculateJumpVerticalSpeed()
	{
		return Mathf.Sqrt(2f * jumpHeight * gravity);
	}

	private void changeBlade()
	{
		if (useGun && !grounded && LevelInfo.getInfo(FengGameManagerMKII.level).type == GAMEMODE.PVP_AHSS)
		{
			return;
		}
		state = HERO_STATE.ChangeBlade;
		throwedBlades = false;
		if (useGun)
		{
			if (!leftGunHasBullet && !rightGunHasBullet)
			{
				if (grounded)
				{
					reloadAnimation = "AHSS_gun_reload_both";
				}
				else
				{
					reloadAnimation = "AHSS_gun_reload_both_air";
				}
			}
			else if (!leftGunHasBullet)
			{
				if (grounded)
				{
					reloadAnimation = "AHSS_gun_reload_l";
				}
				else
				{
					reloadAnimation = "AHSS_gun_reload_l_air";
				}
			}
			else if (!rightGunHasBullet)
			{
				if (grounded)
				{
					reloadAnimation = "AHSS_gun_reload_r";
				}
				else
				{
					reloadAnimation = "AHSS_gun_reload_r_air";
				}
			}
			else
			{
				if (grounded)
				{
					reloadAnimation = "AHSS_gun_reload_both";
				}
				else
				{
					reloadAnimation = "AHSS_gun_reload_both_air";
				}
				leftGunHasBullet = (rightGunHasBullet = false);
			}
			crossFade(reloadAnimation, 0.05f);
		}
		else
		{
			if (!grounded)
			{
				reloadAnimation = "changeBlade_air";
			}
			else
			{
				reloadAnimation = "changeBlade";
			}
			crossFade(reloadAnimation, 0.1f);
		}
	}

	private void checkDashDoubleTap()
	{
		if (uTapTime >= 0f)
		{
			uTapTime += Time.deltaTime;
			if (uTapTime > 0.2f)
			{
				uTapTime = -1f;
			}
		}
		if (dTapTime >= 0f)
		{
			dTapTime += Time.deltaTime;
			if (dTapTime > 0.2f)
			{
				dTapTime = -1f;
			}
		}
		if (lTapTime >= 0f)
		{
			lTapTime += Time.deltaTime;
			if (lTapTime > 0.2f)
			{
				lTapTime = -1f;
			}
		}
		if (rTapTime >= 0f)
		{
			rTapTime += Time.deltaTime;
			if (rTapTime > 0.2f)
			{
				rTapTime = -1f;
			}
		}
		if (SettingsManager.InputSettings.General.Forward.GetKeyDown())
		{
			if (uTapTime == -1f)
			{
				uTapTime = 0f;
			}
			if (uTapTime != 0f)
			{
				dashU = true;
			}
		}
		if (SettingsManager.InputSettings.General.Back.GetKeyDown())
		{
			if (dTapTime == -1f)
			{
				dTapTime = 0f;
			}
			if (dTapTime != 0f)
			{
				dashD = true;
			}
		}
		if (SettingsManager.InputSettings.General.Left.GetKeyDown())
		{
			if (lTapTime == -1f)
			{
				lTapTime = 0f;
			}
			if (lTapTime != 0f)
			{
				dashL = true;
			}
		}
		if (SettingsManager.InputSettings.General.Right.GetKeyDown())
		{
			if (rTapTime == -1f)
			{
				rTapTime = 0f;
			}
			if (rTapTime != 0f)
			{
				dashR = true;
			}
		}
	}

	private void checkDashRebind()
	{
		if (SettingsManager.InputSettings.Human.Dash.GetKeyDown())
		{
			if (SettingsManager.InputSettings.General.Forward.GetKey())
			{
				dashU = true;
			}
			else if (SettingsManager.InputSettings.General.Back.GetKey())
			{
				dashD = true;
			}
			else if (SettingsManager.InputSettings.General.Left.GetKey())
			{
				dashL = true;
			}
			else if (SettingsManager.InputSettings.General.Right.GetKey())
			{
				dashR = true;
			}
		}
	}

	public void checkTitan()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		LayerMask layerMask = 1 << PhysicsLayer.PlayerAttackBox;
		LayerMask layerMask2 = 1 << PhysicsLayer.Ground;
		LayerMask layerMask3 = 1 << PhysicsLayer.EnemyBox;
		RaycastHit[] array = Physics.RaycastAll(ray, 180f, ((LayerMask)((int)layerMask | (int)layerMask2 | (int)layerMask3)).value);
		List<RaycastHit> list = new List<RaycastHit>();
		List<TITAN> list2 = new List<TITAN>();
		foreach (RaycastHit item in array)
		{
			list.Add(item);
		}
		list.Sort((RaycastHit x, RaycastHit y) => x.distance.CompareTo(y.distance));
		float num = 180f;
		for (int i = 0; i < list.Count; i++)
		{
			GameObject gameObject = list[i].collider.gameObject;
			if (gameObject.layer == 16)
			{
				if (!gameObject.name.Contains("PlayerDetectorRC"))
				{
					continue;
				}
				RaycastHit raycastHit;
				RaycastHit raycastHit2 = (raycastHit = list[i]);
				if (raycastHit2.distance < num)
				{
					num -= 60f;
					if (num <= 60f)
					{
						i = list.Count;
					}
					TITAN component = gameObject.transform.root.gameObject.GetComponent<TITAN>();
					if (component != null)
					{
						list2.Add(component);
					}
				}
			}
			else
			{
				i = list.Count;
			}
		}
		for (int i = 0; i < myTitans.Count; i++)
		{
			TITAN tITAN = myTitans[i];
			if (!list2.Contains(tITAN))
			{
				tITAN.isLook = false;
			}
		}
		for (int i = 0; i < list2.Count; i++)
		{
			TITAN tITAN2 = list2[i];
			tITAN2.isLook = true;
		}
		myTitans = list2;
	}

	public void ClearPopup()
	{
		FengGameManagerMKII.instance.ShowHUDInfoCenter(string.Empty);
	}

	public void continueAnimation()
	{
		if (!_animationStopped)
		{
			return;
		}
		_animationStopped = false;
		foreach (AnimationState item in base.animation)
		{
			if (item.speed == 1f)
			{
				return;
			}
			item.speed = 1f;
		}
		customAnimationSpeed();
		playAnimation(currentPlayingClipName());
		if (IN_GAME_MAIN_CAMERA.gametype != 0 && base.photonView.isMine)
		{
			base.photonView.RPC("netContinueAnimation", PhotonTargets.Others);
		}
	}

	public void crossFade(string aniName, float time)
	{
		currentAnimation = aniName;
		base.animation.CrossFade(aniName, time);
		if (PhotonNetwork.connected && base.photonView.isMine)
		{
			object[] parameters = new object[2] { aniName, time };
			base.photonView.RPC("netCrossFade", PhotonTargets.Others, parameters);
		}
	}

	public string currentPlayingClipName()
	{
		foreach (AnimationState item in base.animation)
		{
			if (base.animation.IsPlaying(item.name))
			{
				return item.name;
			}
		}
		return string.Empty;
	}

	private void customAnimationSpeed()
	{
		base.animation["attack5"].speed = 1.85f;
		base.animation["changeBlade"].speed = 1.2f;
		base.animation["air_release"].speed = 0.6f;
		base.animation["changeBlade_air"].speed = 0.8f;
		base.animation["AHSS_gun_reload_both"].speed = 0.38f;
		base.animation["AHSS_gun_reload_both_air"].speed = 0.5f;
		base.animation["AHSS_gun_reload_l"].speed = 0.4f;
		base.animation["AHSS_gun_reload_l_air"].speed = 0.5f;
		base.animation["AHSS_gun_reload_r"].speed = 0.4f;
		base.animation["AHSS_gun_reload_r_air"].speed = 0.5f;
	}

	private void dash(float horizontal, float vertical)
	{
		if (dashTime <= 0f && currentGas > 0f && !isMounted && _dashCooldownLeft <= 0f)
		{
			isGasBursting = true;
			useGas(totalGas * 0.04f);
			facingDirection = getGlobalFacingDirection(horizontal, vertical);
			dashV = getGlobaleFacingVector3(facingDirection);
			originVM = currentSpeed;
			Quaternion rotation = Quaternion.Euler(0f, facingDirection, 0f);
			base.rigidbody.rotation = rotation;
			targetRotation = rotation;
			if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
			{
				UnityEngine.Object.Instantiate(Resources.Load("FX/boost_smoke"), base.transform.position, base.transform.rotation);
			}
			else
			{
				PhotonNetwork.Instantiate("FX/boost_smoke", base.transform.position, base.transform.rotation, 0);
			}
			dashTime = 0.5f;
			crossFade("dash", 0.1f);
			base.animation["dash"].time = 0.1f;
			state = HERO_STATE.AirDodge;
			falseAttack();
			base.rigidbody.AddForce(dashV * 40f, ForceMode.VelocityChange);
			_dashCooldownLeft = 0.2f;
		}
	}

	public void die(Vector3 v, bool isBite)
	{
		if (invincible <= 0f)
		{
			if (titanForm && eren_titan != null)
			{
				eren_titan.GetComponent<TITAN_EREN>().lifeTime = 0.1f;
			}
			if (bulletLeft != null)
			{
				bulletLeft.GetComponent<Bullet>().removeMe();
			}
			if (bulletRight != null)
			{
				bulletRight.GetComponent<Bullet>().removeMe();
			}
			meatDie.Play();
			if ((IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE || base.photonView.isMine) && !useGun)
			{
				leftbladetrail.Deactivate();
				rightbladetrail.Deactivate();
				leftbladetrail2.Deactivate();
				rightbladetrail2.Deactivate();
			}
			if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
			{
				GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().ReportKillToChatFeed("Titan", "You", 0);
			}
			breakApart2(v, isBite);
			currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
			GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().gameLose2();
			falseAttack();
			hasDied = true;
			Transform transform = base.transform.Find("audio_die");
			transform.parent = null;
			transform.GetComponent<AudioSource>().Play();
			if (SettingsManager.GeneralSettings.SnapshotsEnabled.Value)
			{
				GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().startSnapShot2(base.transform.position, 0, null, 0.02f);
			}
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	public void die2(Transform tf)
	{
		if (invincible <= 0f)
		{
			if (titanForm && eren_titan != null)
			{
				eren_titan.GetComponent<TITAN_EREN>().lifeTime = 0.1f;
			}
			if (bulletLeft != null)
			{
				bulletLeft.GetComponent<Bullet>().removeMe();
			}
			if (bulletRight != null)
			{
				bulletRight.GetComponent<Bullet>().removeMe();
			}
			Transform transform = base.transform.Find("audio_die");
			transform.parent = null;
			transform.GetComponent<AudioSource>().Play();
			meatDie.Play();
			currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(null);
			currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
			if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
			{
				GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().ReportKillToChatFeed("Titan", "You", 0);
			}
			GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().gameLose2();
			falseAttack();
			hasDied = true;
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("hitMeat2"));
			gameObject.transform.position = base.transform.position;
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void dodge2(bool offTheWall = false)
	{
		if (SettingsManager.InputSettings.Human.HorseMount.GetKey() && !(myHorse == null) && !isMounted && !(Vector3.Distance(myHorse.transform.position, base.transform.position) >= 15f))
		{
			return;
		}
		state = HERO_STATE.GroundDodge;
		if (!offTheWall)
		{
			float num = (SettingsManager.InputSettings.General.Forward.GetKey() ? 1f : ((!SettingsManager.InputSettings.General.Back.GetKey()) ? 0f : (-1f)));
			float num2 = (SettingsManager.InputSettings.General.Left.GetKey() ? (-1f) : ((!SettingsManager.InputSettings.General.Right.GetKey()) ? 0f : 1f));
			float globalFacingDirection = getGlobalFacingDirection(num2, num);
			if (num2 != 0f || num != 0f)
			{
				facingDirection = globalFacingDirection + 180f;
				targetRotation = Quaternion.Euler(0f, facingDirection, 0f);
			}
			crossFade("dodge", 0.1f);
		}
		else
		{
			playAnimation("dodge");
			playAnimationAt("dodge", 0.2f);
		}
		sparks.enableEmission = false;
	}

	private void erenTransform()
	{
		skillCDDuration = skillCDLast;
		if (bulletLeft != null)
		{
			bulletLeft.GetComponent<Bullet>().removeMe();
		}
		if (bulletRight != null)
		{
			bulletRight.GetComponent<Bullet>().removeMe();
		}
		if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
		{
			eren_titan = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("TITAN_EREN"), base.transform.position, base.transform.rotation);
		}
		else
		{
			eren_titan = PhotonNetwork.Instantiate("TITAN_EREN", base.transform.position, base.transform.rotation, 0);
		}
		eren_titan.GetComponent<TITAN_EREN>().realBody = base.gameObject;
		GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().flashBlind();
		GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(eren_titan);
		eren_titan.GetComponent<TITAN_EREN>().born();
		eren_titan.rigidbody.velocity = base.rigidbody.velocity;
		base.rigidbody.velocity = Vector3.zero;
		base.transform.position = eren_titan.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck").position;
		titanForm = true;
		if (IN_GAME_MAIN_CAMERA.gametype != 0)
		{
			object[] parameters = new object[1] { eren_titan.GetPhotonView().viewID };
			base.photonView.RPC("whoIsMyErenTitan", PhotonTargets.Others, parameters);
		}
		if (smoke_3dmg.enableEmission && IN_GAME_MAIN_CAMERA.gametype != 0 && base.photonView.isMine)
		{
			object[] parameters2 = new object[1] { false };
			base.photonView.RPC("net3DMGSMOKE", PhotonTargets.Others, parameters2);
		}
		smoke_3dmg.enableEmission = false;
	}

	private void escapeFromGrab()
	{
	}

	public void falseAttack()
	{
		attackMove = false;
		if (useGun)
		{
			if (!attackReleased)
			{
				continueAnimation();
				attackReleased = true;
			}
			return;
		}
		if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE || base.photonView.isMine)
		{
			checkBoxLeft.GetComponent<TriggerColliderWeapon>().active_me = false;
			checkBoxRight.GetComponent<TriggerColliderWeapon>().active_me = false;
			checkBoxLeft.GetComponent<TriggerColliderWeapon>().clearHits();
			checkBoxRight.GetComponent<TriggerColliderWeapon>().clearHits();
			leftbladetrail.StopSmoothly(0.2f);
			rightbladetrail.StopSmoothly(0.2f);
			leftbladetrail2.StopSmoothly(0.2f);
			rightbladetrail2.StopSmoothly(0.2f);
		}
		attackLoop = 0;
		if (!attackReleased)
		{
			continueAnimation();
			attackReleased = true;
		}
	}

	public void fillGas()
	{
		currentGas = totalGas;
	}

	private GameObject findNearestTitan()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("titan");
		GameObject result = null;
		float num = float.PositiveInfinity;
		Vector3 position = base.transform.position;
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			float sqrMagnitude = (gameObject.transform.position - position).sqrMagnitude;
			if (sqrMagnitude < num)
			{
				result = gameObject;
				num = sqrMagnitude;
			}
		}
		return result;
	}

	public void Update()
	{
		if (dashTime <= 0.49f)
		{
			isGasBursting = false;
		}
	}

	private void FixedUpdate()
	{
		if (!titanForm && !isCannon && (!GameMenu.Paused || IN_GAME_MAIN_CAMERA.gametype != 0))
		{
			currentSpeed = baseRigidBody.velocity.magnitude;
			if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE || base.photonView.isMine)
			{
				GameProgressManager.RegisterSpeed(base.gameObject, baseRigidBody.velocity.magnitude);
				if (!baseAnimation.IsPlaying("attack3_2") && !baseAnimation.IsPlaying("attack5") && !baseAnimation.IsPlaying("special_petra"))
				{
					baseRigidBody.rotation = Quaternion.Lerp(base.gameObject.transform.rotation, targetRotation, Time.deltaTime * 6f);
				}
				if (state == HERO_STATE.Grab)
				{
					baseRigidBody.AddForce(-baseRigidBody.velocity, ForceMode.VelocityChange);
				}
				else
				{
					if (IsGrounded())
					{
						if (!grounded)
						{
							justGrounded = true;
						}
						grounded = true;
					}
					else
					{
						grounded = false;
					}
					if (hookSomeOne)
					{
						if (hookTarget != null)
						{
							Vector3 vector = hookTarget.transform.position - baseTransform.position;
							float magnitude = vector.magnitude;
							if (magnitude > 2f)
							{
								baseRigidBody.AddForce(vector.normalized * Mathf.Pow(magnitude, 0.15f) * 30f - baseRigidBody.velocity * 0.95f, ForceMode.VelocityChange);
							}
						}
						else
						{
							hookSomeOne = false;
						}
					}
					else if (hookBySomeOne && badGuy != null)
					{
						if (badGuy != null)
						{
							Vector3 vector2 = badGuy.transform.position - baseTransform.position;
							float magnitude2 = vector2.magnitude;
							if (magnitude2 > 5f)
							{
								baseRigidBody.AddForce(vector2.normalized * Mathf.Pow(magnitude2, 0.15f) * 0.2f, ForceMode.Impulse);
							}
						}
						else
						{
							hookBySomeOne = false;
						}
					}
					float num = 0f;
					float num2 = 0f;
					if (!IN_GAME_MAIN_CAMERA.isTyping && !GameMenu.InMenu())
					{
						num2 = (SettingsManager.InputSettings.General.Forward.GetKey() ? 1f : ((!SettingsManager.InputSettings.General.Back.GetKey()) ? 0f : (-1f)));
						num = (SettingsManager.InputSettings.General.Left.GetKey() ? (-1f) : ((!SettingsManager.InputSettings.General.Right.GetKey()) ? 0f : 1f));
					}
					bool flag = false;
					bool flag2 = false;
					bool flag3 = false;
					isLeftHandHooked = false;
					isRightHandHooked = false;
					if (isLaunchLeft)
					{
						if (bulletLeft != null && bulletLeft.GetComponent<Bullet>().isHooked())
						{
							isLeftHandHooked = true;
							Vector3 vector3 = bulletLeft.transform.position - baseTransform.position;
							vector3.Normalize();
							vector3 *= 10f;
							if (!isLaunchRight)
							{
								vector3 *= 2f;
							}
							if (Vector3.Angle(baseRigidBody.velocity, vector3) > 90f && SettingsManager.InputSettings.Human.Jump.GetKey())
							{
								flag2 = true;
								flag = true;
							}
							if (!flag2)
							{
								baseRigidBody.AddForce(vector3);
								if (Vector3.Angle(baseRigidBody.velocity, vector3) > 90f)
								{
									baseRigidBody.AddForce(-baseRigidBody.velocity * 2f, ForceMode.Acceleration);
								}
							}
						}
						launchElapsedTimeL += Time.deltaTime;
						if (QHold && currentGas > 0f)
						{
							useGas(useGasSpeed * Time.deltaTime);
						}
						else if (launchElapsedTimeL > 0.3f)
						{
							isLaunchLeft = false;
							if (bulletLeft != null)
							{
								bulletLeft.GetComponent<Bullet>().disable();
								releaseIfIHookSb();
								bulletLeft = null;
								flag2 = false;
							}
						}
					}
					if (isLaunchRight)
					{
						if (bulletRight != null && bulletRight.GetComponent<Bullet>().isHooked())
						{
							isRightHandHooked = true;
							Vector3 vector4 = bulletRight.transform.position - baseTransform.position;
							vector4.Normalize();
							vector4 *= 10f;
							if (!isLaunchLeft)
							{
								vector4 *= 2f;
							}
							if (Vector3.Angle(baseRigidBody.velocity, vector4) > 90f && SettingsManager.InputSettings.Human.Jump.GetKey())
							{
								flag3 = true;
								flag = true;
							}
							if (!flag3)
							{
								baseRigidBody.AddForce(vector4);
								if (Vector3.Angle(baseRigidBody.velocity, vector4) > 90f)
								{
									baseRigidBody.AddForce(-baseRigidBody.velocity * 2f, ForceMode.Acceleration);
								}
							}
						}
						launchElapsedTimeR += Time.deltaTime;
						if (EHold && currentGas > 0f)
						{
							useGas(useGasSpeed * Time.deltaTime);
						}
						else if (launchElapsedTimeR > 0.3f)
						{
							isLaunchRight = false;
							if (bulletRight != null)
							{
								bulletRight.GetComponent<Bullet>().disable();
								releaseIfIHookSb();
								bulletRight = null;
								flag3 = false;
							}
						}
					}
					if (grounded)
					{
						Vector3 vector5 = Vector3.zero;
						if (state == HERO_STATE.Attack)
						{
							if (attackAnimation == "attack5")
							{
								if (baseAnimation[attackAnimation].normalizedTime > 0.4f && baseAnimation[attackAnimation].normalizedTime < 0.61f)
								{
									baseRigidBody.AddForce(base.gameObject.transform.forward * 200f);
								}
							}
							else if (attackAnimation == "special_petra")
							{
								if (baseAnimation[attackAnimation].normalizedTime > 0.35f && baseAnimation[attackAnimation].normalizedTime < 0.48f)
								{
									baseRigidBody.AddForce(base.gameObject.transform.forward * 200f);
								}
							}
							else if (baseAnimation.IsPlaying("attack3_2"))
							{
								vector5 = Vector3.zero;
							}
							else if (baseAnimation.IsPlaying("attack1") || baseAnimation.IsPlaying("attack2"))
							{
								baseRigidBody.AddForce(base.gameObject.transform.forward * 200f);
							}
							if (baseAnimation.IsPlaying("attack3_2"))
							{
								vector5 = Vector3.zero;
							}
						}
						if (justGrounded)
						{
							if (state != HERO_STATE.Attack || (attackAnimation != "attack3_1" && attackAnimation != "attack5" && attackAnimation != "special_petra"))
							{
								if (state != HERO_STATE.Attack && num == 0f && num2 == 0f && bulletLeft == null && bulletRight == null && state != HERO_STATE.FillGas)
								{
									state = HERO_STATE.Land;
									crossFade("dash_land", 0.01f);
								}
								else
								{
									buttonAttackRelease = true;
									if (state != HERO_STATE.Attack && baseRigidBody.velocity.x * baseRigidBody.velocity.x + baseRigidBody.velocity.z * baseRigidBody.velocity.z > speed * speed * 1.5f && state != HERO_STATE.FillGas)
									{
										state = HERO_STATE.Slide;
										crossFade("slide", 0.05f);
										facingDirection = Mathf.Atan2(baseRigidBody.velocity.x, baseRigidBody.velocity.z) * 57.29578f;
										targetRotation = Quaternion.Euler(0f, facingDirection, 0f);
										sparks.enableEmission = true;
									}
								}
							}
							justGrounded = false;
							vector5 = baseRigidBody.velocity;
						}
						if (state == HERO_STATE.Attack && attackAnimation == "attack3_1" && baseAnimation[attackAnimation].normalizedTime >= 1f)
						{
							playAnimation("attack3_2");
							resetAnimationSpeed();
							Vector3 zero = Vector3.zero;
							baseRigidBody.velocity = zero;
							vector5 = zero;
							currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().startShake(0.2f, 0.3f);
						}
						if (state == HERO_STATE.GroundDodge)
						{
							if (baseAnimation["dodge"].normalizedTime >= 0.2f && baseAnimation["dodge"].normalizedTime < 0.8f)
							{
								vector5 = -baseTransform.forward * 2.4f * speed;
							}
							if (baseAnimation["dodge"].normalizedTime > 0.8f)
							{
								vector5 = baseRigidBody.velocity * 0.9f;
							}
						}
						else if (state == HERO_STATE.Idle)
						{
							Vector3 vector6 = new Vector3(num, 0f, num2);
							float num3 = getGlobalFacingDirection(num, num2);
							vector5 = getGlobaleFacingVector3(num3);
							float num4 = ((!(vector6.magnitude <= 0.95f)) ? 1f : ((vector6.magnitude >= 0.25f) ? vector6.magnitude : 0f));
							vector5 *= num4;
							vector5 *= speed;
							if (buffTime > 0f && currentBuff == BUFF.SpeedUp)
							{
								vector5 *= 4f;
							}
							if (num != 0f || num2 != 0f)
							{
								if (!baseAnimation.IsPlaying("run") && !baseAnimation.IsPlaying("jump") && !baseAnimation.IsPlaying("run_sasha") && (!baseAnimation.IsPlaying("horse_geton") || baseAnimation["horse_geton"].normalizedTime >= 0.5f))
								{
									if (buffTime > 0f && currentBuff == BUFF.SpeedUp)
									{
										crossFade("run_sasha", 0.1f);
									}
									else
									{
										crossFade("run", 0.1f);
									}
								}
							}
							else
							{
								if (!baseAnimation.IsPlaying(standAnimation) && state != HERO_STATE.Land && !baseAnimation.IsPlaying("jump") && !baseAnimation.IsPlaying("horse_geton") && !baseAnimation.IsPlaying("grabbed"))
								{
									crossFade(standAnimation, 0.1f);
									vector5 *= 0f;
								}
								num3 = -874f;
							}
							if (num3 != -874f)
							{
								facingDirection = num3;
								targetRotation = Quaternion.Euler(0f, facingDirection, 0f);
							}
						}
						else if (state == HERO_STATE.Land)
						{
							vector5 = baseRigidBody.velocity * 0.96f;
						}
						else if (state == HERO_STATE.Slide)
						{
							vector5 = baseRigidBody.velocity * 0.99f;
							if (currentSpeed < speed * 1.2f)
							{
								idle();
								sparks.enableEmission = false;
							}
						}
						Vector3 velocity = baseRigidBody.velocity;
						Vector3 force = vector5 - velocity;
						force.x = Mathf.Clamp(force.x, 0f - maxVelocityChange, maxVelocityChange);
						force.z = Mathf.Clamp(force.z, 0f - maxVelocityChange, maxVelocityChange);
						force.y = 0f;
						if (baseAnimation.IsPlaying("jump") && baseAnimation["jump"].normalizedTime > 0.18f)
						{
							force.y += 8f;
						}
						if (baseAnimation.IsPlaying("horse_geton") && baseAnimation["horse_geton"].normalizedTime > 0.18f && baseAnimation["horse_geton"].normalizedTime < 1f)
						{
							float num5 = 6f;
							force = -baseRigidBody.velocity;
							force.y = num5;
							float num6 = Vector3.Distance(myHorse.transform.position, baseTransform.position);
							float num7 = 0.6f * gravity * num6 / (2f * num5);
							force += num7 * (myHorse.transform.position - baseTransform.position).normalized;
						}
						if (state != HERO_STATE.Attack || !useGun)
						{
							baseRigidBody.AddForce(force, ForceMode.VelocityChange);
							baseRigidBody.rotation = Quaternion.Lerp(base.gameObject.transform.rotation, Quaternion.Euler(0f, facingDirection, 0f), Time.deltaTime * 10f);
						}
					}
					else
					{
						if (sparks.enableEmission)
						{
							sparks.enableEmission = false;
						}
						if (myHorse != null && (baseAnimation.IsPlaying("horse_geton") || baseAnimation.IsPlaying("air_fall")) && baseRigidBody.velocity.y < 0f && Vector3.Distance(myHorse.transform.position + Vector3.up * 1.65f, baseTransform.position) < 0.5f)
						{
							baseTransform.position = myHorse.transform.position + Vector3.up * 1.65f;
							baseTransform.rotation = myHorse.transform.rotation;
							isMounted = true;
							if (!base.animation.IsPlaying("horse_idle"))
							{
								crossFade("horse_idle", 0.1f);
							}
							myHorse.GetComponent<Horse>().mounted();
						}
						if ((state == HERO_STATE.Idle && !baseAnimation.IsPlaying("dash") && !baseAnimation.IsPlaying("wallrun") && !baseAnimation.IsPlaying("toRoof") && !baseAnimation.IsPlaying("horse_geton") && !baseAnimation.IsPlaying("horse_getoff") && !baseAnimation.IsPlaying("air_release") && !isMounted && (!baseAnimation.IsPlaying("air_hook_l_just") || baseAnimation["air_hook_l_just"].normalizedTime >= 1f) && (!baseAnimation.IsPlaying("air_hook_r_just") || baseAnimation["air_hook_r_just"].normalizedTime >= 1f)) || baseAnimation["dash"].normalizedTime >= 0.99f)
						{
							if (!isLeftHandHooked && !isRightHandHooked && (baseAnimation.IsPlaying("air_hook_l") || baseAnimation.IsPlaying("air_hook_r") || baseAnimation.IsPlaying("air_hook")) && baseRigidBody.velocity.y > 20f)
							{
								baseAnimation.CrossFade("air_release");
							}
							else
							{
								bool flag4 = Mathf.Abs(baseRigidBody.velocity.x) + Mathf.Abs(baseRigidBody.velocity.z) > 25f;
								bool flag5 = baseRigidBody.velocity.y < 0f;
								if (!flag4)
								{
									if (flag5)
									{
										if (!baseAnimation.IsPlaying("air_fall"))
										{
											crossFade("air_fall", 0.2f);
										}
									}
									else if (!baseAnimation.IsPlaying("air_rise"))
									{
										crossFade("air_rise", 0.2f);
									}
								}
								else if (!isLeftHandHooked && !isRightHandHooked)
								{
									float current = (0f - Mathf.Atan2(baseRigidBody.velocity.z, baseRigidBody.velocity.x)) * 57.29578f;
									float num8 = 0f - Mathf.DeltaAngle(current, baseTransform.rotation.eulerAngles.y - 90f);
									if (Mathf.Abs(num8) < 45f)
									{
										if (!baseAnimation.IsPlaying("air2"))
										{
											crossFade("air2", 0.2f);
										}
									}
									else if (num8 < 135f && num8 > 0f)
									{
										if (!baseAnimation.IsPlaying("air2_right"))
										{
											crossFade("air2_right", 0.2f);
										}
									}
									else if (num8 > -135f && num8 < 0f)
									{
										if (!baseAnimation.IsPlaying("air2_left"))
										{
											crossFade("air2_left", 0.2f);
										}
									}
									else if (!baseAnimation.IsPlaying("air2_backward"))
									{
										crossFade("air2_backward", 0.2f);
									}
								}
								else if (useGun)
								{
									if (!isRightHandHooked)
									{
										if (!baseAnimation.IsPlaying("AHSS_hook_forward_l"))
										{
											crossFade("AHSS_hook_forward_l", 0.1f);
										}
									}
									else if (!isLeftHandHooked)
									{
										if (!baseAnimation.IsPlaying("AHSS_hook_forward_r"))
										{
											crossFade("AHSS_hook_forward_r", 0.1f);
										}
									}
									else if (!baseAnimation.IsPlaying("AHSS_hook_forward_both"))
									{
										crossFade("AHSS_hook_forward_both", 0.1f);
									}
								}
								else if (!isRightHandHooked)
								{
									if (!baseAnimation.IsPlaying("air_hook_l"))
									{
										crossFade("air_hook_l", 0.1f);
									}
								}
								else if (!isLeftHandHooked)
								{
									if (!baseAnimation.IsPlaying("air_hook_r"))
									{
										crossFade("air_hook_r", 0.1f);
									}
								}
								else if (!baseAnimation.IsPlaying("air_hook"))
								{
									crossFade("air_hook", 0.1f);
								}
							}
						}
						if (!baseAnimation.IsPlaying("air_rise"))
						{
							if (state == HERO_STATE.Idle && baseAnimation.IsPlaying("air_release") && baseAnimation["air_release"].normalizedTime >= 1f)
							{
								crossFade("air_rise", 0.2f);
							}
							if (baseAnimation.IsPlaying("horse_getoff") && baseAnimation["horse_getoff"].normalizedTime >= 1f)
							{
								crossFade("air_rise", 0.2f);
							}
						}
						if (baseAnimation.IsPlaying("toRoof"))
						{
							if (baseAnimation["toRoof"].normalizedTime < 0.22f)
							{
								baseRigidBody.velocity = Vector3.zero;
								baseRigidBody.AddForce(new Vector3(0f, gravity * baseRigidBody.mass, 0f));
							}
							else
							{
								if (!wallJump)
								{
									wallJump = true;
									baseRigidBody.AddForce(Vector3.up * 8f, ForceMode.Impulse);
								}
								baseRigidBody.AddForce(baseTransform.forward * 0.05f, ForceMode.Impulse);
							}
							if (baseAnimation["toRoof"].normalizedTime >= 1f)
							{
								playAnimation("air_rise");
							}
						}
						else if (state == HERO_STATE.Idle && isPressDirectionTowardsHero(num, num2) && !SettingsManager.InputSettings.Human.Jump.GetKey() && !SettingsManager.InputSettings.Human.HookLeft.GetKey() && !SettingsManager.InputSettings.Human.HookRight.GetKey() && !SettingsManager.InputSettings.Human.HookBoth.GetKey() && IsFrontGrounded() && !baseAnimation.IsPlaying("wallrun") && !baseAnimation.IsPlaying("dodge"))
						{
							crossFade("wallrun", 0.1f);
							wallRunTime = 0f;
						}
						else if (baseAnimation.IsPlaying("wallrun"))
						{
							baseRigidBody.AddForce(Vector3.up * speed - baseRigidBody.velocity, ForceMode.VelocityChange);
							wallRunTime += Time.deltaTime;
							if (wallRunTime > 1f || (num2 == 0f && num == 0f))
							{
								baseRigidBody.AddForce(-baseTransform.forward * speed * 0.75f, ForceMode.Impulse);
								dodge2(true);
							}
							else if (!IsUpFrontGrounded())
							{
								wallJump = false;
								crossFade("toRoof", 0.1f);
							}
							else if (!IsFrontGrounded())
							{
								crossFade("air_fall", 0.1f);
							}
						}
						else if (!baseAnimation.IsPlaying("attack5") && !baseAnimation.IsPlaying("special_petra") && !baseAnimation.IsPlaying("dash") && !baseAnimation.IsPlaying("jump") && !IsFiringThunderSpear())
						{
							Vector3 vector7 = new Vector3(num, 0f, num2);
							float num9 = getGlobalFacingDirection(num, num2);
							Vector3 globaleFacingVector = getGlobaleFacingVector3(num9);
							float num10 = ((!(vector7.magnitude <= 0.95f)) ? 1f : ((vector7.magnitude >= 0.25f) ? vector7.magnitude : 0f));
							globaleFacingVector *= num10;
							globaleFacingVector *= (float)setup.myCostume.stat.ACL / 10f * 2f;
							if (num == 0f && num2 == 0f)
							{
								if (state == HERO_STATE.Attack)
								{
									globaleFacingVector *= 0f;
								}
								num9 = -874f;
							}
							if (num9 != -874f)
							{
								facingDirection = num9;
								targetRotation = Quaternion.Euler(0f, facingDirection, 0f);
							}
							if (!flag2 && !flag3 && !isMounted && SettingsManager.InputSettings.Human.Jump.GetKey() && currentGas > 0f)
							{
								if (num != 0f || num2 != 0f)
								{
									baseRigidBody.AddForce(globaleFacingVector, ForceMode.Acceleration);
								}
								else
								{
									baseRigidBody.AddForce(baseTransform.forward * globaleFacingVector.magnitude, ForceMode.Acceleration);
								}
								flag = true;
							}
						}
						if (baseAnimation.IsPlaying("air_fall") && currentSpeed < 0.2f && IsFrontGrounded())
						{
							crossFade("onWall", 0.3f);
						}
					}
					spinning = false;
					if (flag2 && flag3)
					{
						float num11 = currentSpeed + 0.1f;
						baseRigidBody.AddForce(-baseRigidBody.velocity, ForceMode.VelocityChange);
						Vector3 current2 = (bulletRight.transform.position + bulletLeft.transform.position) * 0.5f - baseTransform.position;
						float reelAxis = GetReelAxis();
						reelAxis = Mathf.Clamp(reelAxis, -0.8f, 0.8f);
						float num12 = 1f + reelAxis;
						Vector3 vector8 = Vector3.RotateTowards(current2, baseRigidBody.velocity, 1.53938f * num12, 1.53938f * num12);
						vector8.Normalize();
						spinning = true;
						baseRigidBody.velocity = vector8 * num11;
					}
					else if (flag2)
					{
						float num13 = currentSpeed + 0.1f;
						baseRigidBody.AddForce(-baseRigidBody.velocity, ForceMode.VelocityChange);
						Vector3 current3 = bulletLeft.transform.position - baseTransform.position;
						float reelAxis2 = GetReelAxis();
						reelAxis2 = Mathf.Clamp(reelAxis2, -0.8f, 0.8f);
						float num14 = 1f + reelAxis2;
						Vector3 vector9 = Vector3.RotateTowards(current3, baseRigidBody.velocity, 1.53938f * num14, 1.53938f * num14);
						vector9.Normalize();
						spinning = true;
						baseRigidBody.velocity = vector9 * num13;
					}
					else if (flag3)
					{
						float num15 = currentSpeed + 0.1f;
						baseRigidBody.AddForce(-baseRigidBody.velocity, ForceMode.VelocityChange);
						Vector3 current4 = bulletRight.transform.position - baseTransform.position;
						float reelAxis3 = GetReelAxis();
						reelAxis3 = Mathf.Clamp(reelAxis3, -0.8f, 0.8f);
						float num16 = 1f + reelAxis3;
						Vector3 vector10 = Vector3.RotateTowards(current4, baseRigidBody.velocity, 1.53938f * num16, 1.53938f * num16);
						vector10.Normalize();
						spinning = true;
						baseRigidBody.velocity = vector10 * num15;
					}
					if (state == HERO_STATE.Attack && (attackAnimation == "attack5" || attackAnimation == "special_petra") && baseAnimation[attackAnimation].normalizedTime > 0.4f && !attackMove)
					{
						attackMove = true;
						if (launchPointRight.magnitude > 0f)
						{
							Vector3 force2 = launchPointRight - baseTransform.position;
							force2.Normalize();
							force2 *= 13f;
							baseRigidBody.AddForce(force2, ForceMode.Impulse);
						}
						if (attackAnimation == "special_petra" && launchPointLeft.magnitude > 0f)
						{
							Vector3 force3 = launchPointLeft - baseTransform.position;
							force3.Normalize();
							force3 *= 13f;
							baseRigidBody.AddForce(force3, ForceMode.Impulse);
							if (bulletRight != null)
							{
								bulletRight.GetComponent<Bullet>().disable();
								releaseIfIHookSb();
							}
							if (bulletLeft != null)
							{
								bulletLeft.GetComponent<Bullet>().disable();
								releaseIfIHookSb();
							}
						}
						baseRigidBody.AddForce(Vector3.up * 2f, ForceMode.Impulse);
					}
					bool flag6 = false;
					if (bulletLeft != null || bulletRight != null)
					{
						if (bulletLeft != null && bulletLeft.transform.position.y > base.gameObject.transform.position.y && isLaunchLeft && bulletLeft.GetComponent<Bullet>().isHooked())
						{
							flag6 = true;
						}
						if (bulletRight != null && bulletRight.transform.position.y > base.gameObject.transform.position.y && isLaunchRight && bulletRight.GetComponent<Bullet>().isHooked())
						{
							flag6 = true;
						}
					}
					if (flag6)
					{
						baseRigidBody.AddForce(new Vector3(0f, -10f * baseRigidBody.mass, 0f));
					}
					else
					{
						baseRigidBody.AddForce(new Vector3(0f, (0f - gravity) * baseRigidBody.mass, 0f));
					}
					if (currentSpeed > 10f)
					{
						currentCamera.GetComponent<Camera>().fieldOfView = Mathf.Lerp(currentCamera.GetComponent<Camera>().fieldOfView, Mathf.Min(100f, currentSpeed + 40f), 0.1f);
					}
					else
					{
						currentCamera.GetComponent<Camera>().fieldOfView = Mathf.Lerp(currentCamera.GetComponent<Camera>().fieldOfView, 50f, 0.1f);
					}
					if (!_cancelGasDisable)
					{
						if (flag)
						{
							useGas(useGasSpeed * Time.deltaTime);
							if (!smoke_3dmg.enableEmission && IN_GAME_MAIN_CAMERA.gametype != 0 && base.photonView.isMine)
							{
								object[] parameters = new object[1] { true };
								base.photonView.RPC("net3DMGSMOKE", PhotonTargets.Others, parameters);
							}
							smoke_3dmg.enableEmission = true;
						}
						else
						{
							if (smoke_3dmg.enableEmission && IN_GAME_MAIN_CAMERA.gametype != 0 && base.photonView.isMine)
							{
								object[] parameters2 = new object[1] { false };
								base.photonView.RPC("net3DMGSMOKE", PhotonTargets.Others, parameters2);
							}
							smoke_3dmg.enableEmission = false;
						}
					}
					else
					{
						_cancelGasDisable = false;
					}
					if (WindWeatherEffect.WindEnabled)
					{
						if (!speedFXPS.enableEmission)
						{
							speedFXPS.enableEmission = true;
						}
						speedFXPS.startSpeed = 100f;
						speedFX.transform.LookAt(baseTransform.position + WindWeatherEffect.WindDirection);
					}
					else if (currentSpeed > 80f && SettingsManager.GraphicsSettings.WindEffectEnabled.Value)
					{
						if (!speedFXPS.enableEmission)
						{
							speedFXPS.enableEmission = true;
						}
						speedFXPS.startSpeed = currentSpeed;
						speedFX.transform.LookAt(baseTransform.position + baseRigidBody.velocity);
					}
					else if (speedFXPS.enableEmission)
					{
						speedFXPS.enableEmission = false;
					}
				}
			}
			setHookedPplDirection();
			bodyLean();
		}
		_reelInAxis = 0f;
	}

	public string getDebugInfo()
	{
		string text = "\n";
		text = "Left:" + isLeftHandHooked + " ";
		if (isLeftHandHooked && bulletLeft != null)
		{
			Vector3 vector = bulletLeft.transform.position - base.transform.position;
			text += (int)(Mathf.Atan2(vector.x, vector.z) * 57.29578f);
		}
		string text2 = text;
		object[] array = new object[4] { text2, "\nRight:", isRightHandHooked, " " };
		text = string.Concat(array);
		if (isRightHandHooked && bulletRight != null)
		{
			Vector3 vector2 = bulletRight.transform.position - base.transform.position;
			text += (int)(Mathf.Atan2(vector2.x, vector2.z) * 57.29578f);
		}
		text = text + "\nfacingDirection:" + (int)facingDirection + "\nActual facingDirection:" + (int)base.transform.rotation.eulerAngles.y + "\nState:" + state.ToString() + "\n\n\n\n\n";
		if (state == HERO_STATE.Attack)
		{
			targetRotation = Quaternion.Euler(0f, facingDirection, 0f);
		}
		return text;
	}

	private Vector3 getGlobaleFacingVector3(float resultAngle)
	{
		float num = 0f - resultAngle + 90f;
		float x = Mathf.Cos(num * 0.01745329f);
		return new Vector3(x, 0f, Mathf.Sin(num * 0.01745329f));
	}

	private Vector3 getGlobaleFacingVector3(float horizontal, float vertical)
	{
		float num = 0f - getGlobalFacingDirection(horizontal, vertical) + 90f;
		float x = Mathf.Cos(num * 0.01745329f);
		return new Vector3(x, 0f, Mathf.Sin(num * 0.01745329f));
	}

	private float getGlobalFacingDirection(float horizontal, float vertical)
	{
		if (vertical == 0f && horizontal == 0f)
		{
			return base.transform.rotation.eulerAngles.y;
		}
		float y = currentCamera.transform.rotation.eulerAngles.y;
		float num = Mathf.Atan2(vertical, horizontal) * 57.29578f;
		num = 0f - num + 90f;
		return y + num;
	}

	private float getLeanAngle(Vector3 p, bool left)
	{
		if (!useGun && state == HERO_STATE.Attack)
		{
			return 0f;
		}
		float num = p.y - base.transform.position.y;
		float num2 = Vector3.Distance(p, base.transform.position);
		float num3 = Mathf.Acos(num / num2) * 57.29578f;
		num3 *= 0.1f;
		num3 *= 1f + Mathf.Pow(base.rigidbody.velocity.magnitude, 0.2f);
		Vector3 vector = p - base.transform.position;
		float current = Mathf.Atan2(vector.x, vector.z) * 57.29578f;
		float target = Mathf.Atan2(base.rigidbody.velocity.x, base.rigidbody.velocity.z) * 57.29578f;
		float num4 = Mathf.DeltaAngle(current, target);
		num3 += Mathf.Abs(num4 * 0.5f);
		if (state != HERO_STATE.Attack)
		{
			num3 = Mathf.Min(num3, 80f);
		}
		if (num4 > 0f)
		{
			leanLeft = true;
		}
		else
		{
			leanLeft = false;
		}
		if (useGun)
		{
			return num3 * ((num4 >= 0f) ? 1f : (-1f));
		}
		float num5 = 0f;
		num5 = (((!left || !(num4 < 0f)) && (left || !(num4 > 0f))) ? 0.5f : 0.1f);
		return num3 * ((num4 >= 0f) ? num5 : (0f - num5));
	}

	private void getOffHorse()
	{
		playAnimation("horse_getoff");
		base.rigidbody.AddForce(Vector3.up * 10f - base.transform.forward * 2f - base.transform.right * 1f, ForceMode.VelocityChange);
		unmounted();
	}

	private void getOnHorse()
	{
		playAnimation("horse_geton");
		facingDirection = myHorse.transform.rotation.eulerAngles.y;
		targetRotation = Quaternion.Euler(0f, facingDirection, 0f);
	}

	public void getSupply()
	{
		if ((base.animation.IsPlaying(standAnimation) || base.animation.IsPlaying("run") || base.animation.IsPlaying("run_sasha")) && (currentBladeSta != totalBladeSta || currentBladeNum != totalBladeNum || currentGas != totalGas || leftBulletLeft != bulletMAX || rightBulletLeft != bulletMAX))
		{
			state = HERO_STATE.FillGas;
			crossFade("supply", 0.1f);
		}
	}

	public void grabbed(GameObject titan, bool leftHand)
	{
		if (isMounted)
		{
			unmounted();
		}
		state = HERO_STATE.Grab;
		GetComponent<CapsuleCollider>().isTrigger = true;
		falseAttack();
		titanWhoGrabMe = titan;
		if (titanForm && eren_titan != null)
		{
			eren_titan.GetComponent<TITAN_EREN>().lifeTime = 0.1f;
		}
		if (!useGun && (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE || base.photonView.isMine))
		{
			leftbladetrail.Deactivate();
			rightbladetrail.Deactivate();
			leftbladetrail2.Deactivate();
			rightbladetrail2.Deactivate();
		}
		smoke_3dmg.enableEmission = false;
		sparks.enableEmission = false;
	}

	public bool HasDied()
	{
		if (!hasDied)
		{
			return isInvincible();
		}
		return true;
	}

	private void headMovement()
	{
		Transform transform = base.transform.Find("Amarture/Controller_Body/hip/spine/chest/neck/head");
		Transform transform2 = base.transform.Find("Amarture/Controller_Body/hip/spine/chest/neck");
		float x = Mathf.Sqrt((gunTarget.x - base.transform.position.x) * (gunTarget.x - base.transform.position.x) + (gunTarget.z - base.transform.position.z) * (gunTarget.z - base.transform.position.z));
		targetHeadRotation = transform.rotation;
		Vector3 vector = gunTarget - base.transform.position;
		float current = (0f - Mathf.Atan2(vector.z, vector.x)) * 57.29578f;
		float value = 0f - Mathf.DeltaAngle(current, base.transform.rotation.eulerAngles.y - 90f);
		value = Mathf.Clamp(value, -40f, 40f);
		float y = transform2.position.y - gunTarget.y;
		float value2 = Mathf.Atan2(y, x) * 57.29578f;
		value2 = Mathf.Clamp(value2, -40f, 30f);
		targetHeadRotation = Quaternion.Euler(transform.rotation.eulerAngles.x + value2, transform.rotation.eulerAngles.y + value, transform.rotation.eulerAngles.z);
		oldHeadRotation = Quaternion.Lerp(oldHeadRotation, targetHeadRotation, Time.deltaTime * 60f);
		transform.rotation = oldHeadRotation;
	}

	public void hookedByHuman(int hooker, Vector3 hookPosition)
	{
		object[] parameters = new object[2] { hooker, hookPosition };
		base.photonView.RPC("RPCHookedByHuman", base.photonView.owner, parameters);
	}

	[RPC]
	public void hookFail()
	{
		hookTarget = null;
		hookSomeOne = false;
	}

	public void hookToHuman(GameObject target, Vector3 hookPosition)
	{
		releaseIfIHookSb();
		hookTarget = target;
		hookSomeOne = true;
		if (target.GetComponent<HERO>() != null)
		{
			target.GetComponent<HERO>().hookedByHuman(base.photonView.viewID, hookPosition);
		}
		launchForce = hookPosition - base.transform.position;
		float num = Mathf.Pow(launchForce.magnitude, 0.1f);
		if (grounded)
		{
			base.rigidbody.AddForce(Vector3.up * Mathf.Min(launchForce.magnitude * 0.2f, 10f), ForceMode.Impulse);
		}
		base.rigidbody.AddForce(launchForce * num * 0.1f, ForceMode.Impulse);
	}

	private void idle()
	{
		if (state == HERO_STATE.Attack)
		{
			falseAttack();
		}
		state = HERO_STATE.Idle;
		crossFade(standAnimation, 0.1f);
	}

	private bool IsFrontGrounded()
	{
		LayerMask layerMask = 1 << LayerMask.NameToLayer("Ground");
		LayerMask layerMask2 = 1 << LayerMask.NameToLayer("EnemyBox");
		LayerMask layerMask3 = (int)layerMask2 | (int)layerMask;
		return Physics.Raycast(base.gameObject.transform.position + base.gameObject.transform.up * 1f, base.gameObject.transform.forward, 1f, layerMask3.value);
	}

	public bool IsGrounded()
	{
		LayerMask layerMask = 1 << LayerMask.NameToLayer("Ground");
		LayerMask layerMask2 = 1 << LayerMask.NameToLayer("EnemyBox");
		LayerMask layerMask3 = (int)layerMask2 | (int)layerMask;
		return Physics.Raycast(base.gameObject.transform.position + Vector3.up * 0.1f, -Vector3.up, 0.3f, layerMask3.value);
	}

	public bool isInvincible()
	{
		return invincible > 0f;
	}

	private bool isPressDirectionTowardsHero(float h, float v)
	{
		if (h == 0f && v == 0f)
		{
			return false;
		}
		return Mathf.Abs(Mathf.DeltaAngle(getGlobalFacingDirection(h, v), base.transform.rotation.eulerAngles.y)) < 45f;
	}

	private bool IsUpFrontGrounded()
	{
		LayerMask layerMask = 1 << LayerMask.NameToLayer("Ground");
		LayerMask layerMask2 = 1 << LayerMask.NameToLayer("EnemyBox");
		LayerMask layerMask3 = (int)layerMask2 | (int)layerMask;
		return Physics.Raycast(base.gameObject.transform.position + base.gameObject.transform.up * 3f, base.gameObject.transform.forward, 1.2f, layerMask3.value);
	}

	[RPC]
	private void killObject(PhotonMessageInfo info)
	{
		if (info != null)
		{
			FengGameManagerMKII.instance.kickPlayerRCIfMC(info.sender, true, "hero killObject exploit");
		}
	}

	private void LateUpdate()
	{
	}

	public void lateUpdate2()
	{
		if (IN_GAME_MAIN_CAMERA.gametype != 0 && myNetWorkName != null)
		{
			if (titanForm && eren_titan != null)
			{
				myNetWorkName.transform.localPosition = Vector3.up * Screen.height * 2f;
			}
			Vector3 vector = new Vector3(baseTransform.position.x, baseTransform.position.y + 2f, baseTransform.position.z);
			GameObject gameObject = maincamera;
			LayerMask layerMask = 1 << LayerMask.NameToLayer("Ground");
			LayerMask layerMask2 = 1 << LayerMask.NameToLayer("EnemyBox");
			LayerMask layerMask3 = (int)layerMask2 | (int)layerMask;
			if (Vector3.Angle(gameObject.transform.forward, vector - gameObject.transform.position) > 90f || Physics.Linecast(vector, gameObject.transform.position, layerMask3))
			{
				myNetWorkName.transform.localPosition = Vector3.up * Screen.height * 2f;
			}
			else
			{
				Vector2 vector2 = maincamera.GetComponent<Camera>().WorldToScreenPoint(vector);
				myNetWorkName.transform.localPosition = new Vector3((int)(vector2.x - (float)Screen.width * 0.5f), (int)(vector2.y - (float)Screen.height * 0.5f), 0f);
			}
		}
		if (titanForm || isCannon)
		{
			return;
		}
		if (SettingsManager.GeneralSettings.CameraTilt.Value && (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE || base.photonView.isMine))
		{
			Vector3 vector3 = Vector3.zero;
			Vector3 vector4 = Vector3.zero;
			if (isLaunchLeft && bulletLeft != null && bulletLeft.GetComponent<Bullet>().isHooked())
			{
				vector3 = bulletLeft.transform.position;
			}
			if (isLaunchRight && bulletRight != null && bulletRight.GetComponent<Bullet>().isHooked())
			{
				vector4 = bulletRight.transform.position;
			}
			Vector3 vector5 = Vector3.zero;
			if (vector3.magnitude != 0f && vector4.magnitude == 0f)
			{
				vector5 = vector3;
			}
			else if (vector3.magnitude == 0f && vector4.magnitude != 0f)
			{
				vector5 = vector4;
			}
			else if (vector3.magnitude != 0f && vector4.magnitude != 0f)
			{
				vector5 = (vector3 + vector4) * 0.5f;
			}
			Vector3 vector6 = Vector3.Project(vector5 - baseTransform.position, maincamera.transform.up);
			Vector3 vector7 = Vector3.Project(vector5 - baseTransform.position, maincamera.transform.right);
			Quaternion to2;
			if (vector5.magnitude > 0f)
			{
				Vector3 to = vector6 + vector7;
				float num = Vector3.Angle(vector5 - baseTransform.position, baseRigidBody.velocity) * 0.005f;
				Vector3 vector8 = maincamera.transform.right + vector7.normalized;
				to2 = Quaternion.Euler(maincamera.transform.rotation.eulerAngles.x, maincamera.transform.rotation.eulerAngles.y, (vector8.magnitude >= 1f) ? ((0f - Vector3.Angle(vector6, to)) * num) : (Vector3.Angle(vector6, to) * num));
			}
			else
			{
				to2 = Quaternion.Euler(maincamera.transform.rotation.eulerAngles.x, maincamera.transform.rotation.eulerAngles.y, 0f);
			}
			maincamera.transform.rotation = Quaternion.Lerp(maincamera.transform.rotation, to2, Time.deltaTime * 2f);
		}
		if (state == HERO_STATE.Grab && titanWhoGrabMe != null)
		{
			if (titanWhoGrabMe.GetComponent<TITAN>() != null)
			{
				baseTransform.position = titanWhoGrabMe.GetComponent<TITAN>().grabTF.transform.position;
				baseTransform.rotation = titanWhoGrabMe.GetComponent<TITAN>().grabTF.transform.rotation;
			}
			else if (titanWhoGrabMe.GetComponent<FEMALE_TITAN>() != null)
			{
				baseTransform.position = titanWhoGrabMe.GetComponent<FEMALE_TITAN>().grabTF.transform.position;
				baseTransform.rotation = titanWhoGrabMe.GetComponent<FEMALE_TITAN>().grabTF.transform.rotation;
			}
		}
		if (!useGun)
		{
			return;
		}
		if (leftArmAim || rightArmAim)
		{
			Vector3 vector9 = gunTarget - baseTransform.position;
			float current = (0f - Mathf.Atan2(vector9.z, vector9.x)) * 57.29578f;
			float num2 = 0f - Mathf.DeltaAngle(current, baseTransform.rotation.eulerAngles.y - 90f);
			headMovement();
			if (!isLeftHandHooked && leftArmAim && num2 < 40f && num2 > -90f)
			{
				leftArmAimTo(gunTarget);
			}
			if (!isRightHandHooked && rightArmAim && num2 > -40f && num2 < 90f)
			{
				rightArmAimTo(gunTarget);
			}
		}
		else if (!grounded)
		{
			handL.localRotation = Quaternion.Euler(90f, 0f, 0f);
			handR.localRotation = Quaternion.Euler(-90f, 0f, 0f);
		}
		if (isLeftHandHooked && bulletLeft != null)
		{
			leftArmAimTo(bulletLeft.transform.position);
		}
		if (isRightHandHooked && bulletRight != null)
		{
			rightArmAimTo(bulletRight.transform.position);
		}
	}

	public void launch(Vector3 des, bool left = true, bool leviMode = false)
	{
		if (left)
		{
			isLaunchLeft = true;
			launchElapsedTimeL = 0f;
		}
		else
		{
			isLaunchRight = true;
			launchElapsedTimeR = 0f;
		}
		if (state == HERO_STATE.Grab)
		{
			return;
		}
		if (isMounted)
		{
			unmounted();
		}
		if (state != HERO_STATE.Attack)
		{
			idle();
		}
		Vector3 vector = des - base.transform.position;
		if (left)
		{
			launchPointLeft = des;
		}
		else
		{
			launchPointRight = des;
		}
		vector.Normalize();
		vector *= 20f;
		if (bulletLeft != null && bulletRight != null && bulletLeft.GetComponent<Bullet>().isHooked() && bulletRight.GetComponent<Bullet>().isHooked())
		{
			vector *= 0.8f;
		}
		leviMode = ((base.animation.IsPlaying("attack5") || base.animation.IsPlaying("special_petra")) ? true : false);
		if (!leviMode)
		{
			falseAttack();
			idle();
			if (useGun)
			{
				crossFade("AHSS_hook_forward_both", 0.1f);
			}
			else if (left && !isRightHandHooked)
			{
				crossFade("air_hook_l_just", 0.1f);
			}
			else if (!left && !isLeftHandHooked)
			{
				crossFade("air_hook_r_just", 0.1f);
			}
			else
			{
				crossFade("dash", 0.1f);
				base.animation["dash"].time = 0f;
			}
		}
		launchForce = vector;
		if (!leviMode)
		{
			if (vector.y < 30f)
			{
				launchForce += Vector3.up * (30f - vector.y);
			}
			if (des.y >= base.transform.position.y)
			{
				launchForce += Vector3.up * (des.y - base.transform.position.y) * 10f;
			}
			base.rigidbody.AddForce(launchForce);
		}
		facingDirection = Mathf.Atan2(launchForce.x, launchForce.z) * 57.29578f;
		Quaternion rotation = Quaternion.Euler(0f, facingDirection, 0f);
		base.gameObject.transform.rotation = rotation;
		base.rigidbody.rotation = rotation;
		targetRotation = rotation;
		if (leviMode)
		{
			launchElapsedTimeR = -100f;
		}
		if (base.animation.IsPlaying("special_petra"))
		{
			launchElapsedTimeR = -100f;
			launchElapsedTimeL = -100f;
			if (bulletRight != null)
			{
				bulletRight.GetComponent<Bullet>().disable();
				releaseIfIHookSb();
			}
			if (bulletLeft != null)
			{
				bulletLeft.GetComponent<Bullet>().disable();
				releaseIfIHookSb();
			}
		}
		_cancelGasDisable = true;
		sparks.enableEmission = false;
	}

	private void launchLeftRope(RaycastHit hit, bool single, int mode = 0)
	{
		if (currentGas != 0f)
		{
			useGas();
			if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
			{
				bulletLeft = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("hook"));
			}
			else if (base.photonView.isMine)
			{
				bulletLeft = PhotonNetwork.Instantiate("hook", base.transform.position, base.transform.rotation, 0);
			}
			GameObject gameObject = ((!useGun) ? hookRefL1 : hookRefL2);
			string launcher_ref = ((!useGun) ? "hookRefL1" : "hookRefL2");
			bulletLeft.transform.position = gameObject.transform.position;
			Bullet component = bulletLeft.GetComponent<Bullet>();
			float num = (single ? 0f : ((hit.distance <= 50f) ? (hit.distance * 0.05f) : (hit.distance * 0.3f)));
			Vector3 vector = hit.point - base.transform.right * num - bulletLeft.transform.position;
			vector.Normalize();
			if (mode == 1)
			{
				component.launch(vector * 3f, base.rigidbody.velocity, launcher_ref, true, base.gameObject, true);
			}
			else
			{
				component.launch(vector * 3f, base.rigidbody.velocity, launcher_ref, true, base.gameObject);
			}
			launchPointLeft = Vector3.zero;
		}
	}

	private void launchRightRope(RaycastHit hit, bool single, int mode = 0)
	{
		if (currentGas != 0f)
		{
			useGas();
			if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
			{
				bulletRight = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("hook"));
			}
			else if (base.photonView.isMine)
			{
				bulletRight = PhotonNetwork.Instantiate("hook", base.transform.position, base.transform.rotation, 0);
			}
			GameObject gameObject = ((!useGun) ? hookRefR1 : hookRefR2);
			string launcher_ref = ((!useGun) ? "hookRefR1" : "hookRefR2");
			bulletRight.transform.position = gameObject.transform.position;
			Bullet component = bulletRight.GetComponent<Bullet>();
			float num = (single ? 0f : ((hit.distance <= 50f) ? (hit.distance * 0.05f) : (hit.distance * 0.3f)));
			Vector3 vector = hit.point + base.transform.right * num - bulletRight.transform.position;
			vector.Normalize();
			if (mode == 1)
			{
				component.launch(vector * 5f, base.rigidbody.velocity, launcher_ref, false, base.gameObject, true);
			}
			else
			{
				component.launch(vector * 3f, base.rigidbody.velocity, launcher_ref, false, base.gameObject);
			}
			launchPointRight = Vector3.zero;
		}
	}

	private void leftArmAimTo(Vector3 target)
	{
		float num = target.x - upperarmL.transform.position.x;
		float y = target.y - upperarmL.transform.position.y;
		float num2 = target.z - upperarmL.transform.position.z;
		float x = Mathf.Sqrt(num * num + num2 * num2);
		handL.localRotation = Quaternion.Euler(90f, 0f, 0f);
		forearmL.localRotation = Quaternion.Euler(-90f, 0f, 0f);
		upperarmL.rotation = Quaternion.Euler(0f, 90f + Mathf.Atan2(num, num2) * 57.29578f, (0f - Mathf.Atan2(y, x)) * 57.29578f);
	}

	public void loadskin()
	{
		if ((IN_GAME_MAIN_CAMERA.gametype != 0 && !base.photonView.isMine) || !SettingsManager.CustomSkinSettings.Human.SkinsEnabled.Value)
		{
			return;
		}
		HumanCustomSkinSet humanCustomSkinSet = (HumanCustomSkinSet)SettingsManager.CustomSkinSettings.Human.GetSelectedSet();
		string text = string.Join(",", new string[19]
		{
			humanCustomSkinSet.Horse.Value,
			humanCustomSkinSet.Hair.Value,
			humanCustomSkinSet.Eye.Value,
			humanCustomSkinSet.Glass.Value,
			humanCustomSkinSet.Face.Value,
			humanCustomSkinSet.Skin.Value,
			humanCustomSkinSet.Costume.Value,
			humanCustomSkinSet.Logo.Value,
			humanCustomSkinSet.GearL.Value,
			humanCustomSkinSet.GearR.Value,
			humanCustomSkinSet.Gas.Value,
			humanCustomSkinSet.Hoodie.Value,
			humanCustomSkinSet.WeaponTrail.Value,
			humanCustomSkinSet.ThunderSpearL.Value,
			humanCustomSkinSet.ThunderSpearR.Value,
			humanCustomSkinSet.HookL.Value,
			humanCustomSkinSet.HookLTiling.Value.ToString(),
			humanCustomSkinSet.HookR.Value,
			humanCustomSkinSet.HookRTiling.Value.ToString()
		});
		if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
		{
			StartCoroutine(loadskinE(-1, text));
			return;
		}
		int num = -1;
		if (myHorse != null)
		{
			num = myHorse.GetPhotonView().viewID;
		}
		base.photonView.RPC("loadskinRPC", PhotonTargets.AllBuffered, num, text);
	}

	public IEnumerator loadskinE(int horse, string url)
	{
		while (!_hasRunStart)
		{
			yield return null;
		}
		_customSkinLoader.StartCoroutine(_customSkinLoader.LoadSkinsFromRPC(new object[2] { horse, url }));
	}

	[RPC]
	public void loadskinRPC(int horse, string url, PhotonMessageInfo info)
	{
		if (info.sender == base.photonView.owner)
		{
			HumanCustomSkinSettings human = SettingsManager.CustomSkinSettings.Human;
			if (human.SkinsEnabled.Value && (!human.SkinsLocal.Value || base.photonView.isMine))
			{
				StartCoroutine(loadskinE(horse, url));
			}
		}
	}

	public void markDie()
	{
		hasDied = true;
		state = HERO_STATE.Die;
	}

	[RPC]
	public void moveToRPC(float posX, float posY, float posZ, PhotonMessageInfo info)
	{
		if (info != null && info.sender.isMasterClient)
		{
			base.transform.position = new Vector3(posX, posY, posZ);
		}
	}

	[RPC]
	private void net3DMGSMOKE(bool ifON, PhotonMessageInfo info)
	{
		if (info != null && info.sender != base.photonView.owner && !info.sender.isLocal)
		{
			FengGameManagerMKII.instance.kickPlayerRCIfMC(info.sender, true, "hero net3dmgsmoke exploit");
		}
		else if (smoke_3dmg != null)
		{
			smoke_3dmg.enableEmission = ifON;
		}
	}

	[RPC]
	private void netContinueAnimation(PhotonMessageInfo info)
	{
		if (info != null && info.sender != base.photonView.owner)
		{
			FengGameManagerMKII.instance.kickPlayerRCIfMC(info.sender, true, "hero continueanimation exploit");
		}
		foreach (AnimationState item in base.animation)
		{
			if (item.speed == 1f)
			{
				return;
			}
			item.speed = 1f;
		}
		playAnimation(currentPlayingClipName());
	}

	[RPC]
	private void netCrossFade(string aniName, float time, PhotonMessageInfo info)
	{
		if (info != null && info.sender != base.photonView.owner)
		{
			FengGameManagerMKII.instance.kickPlayerRCIfMC(info.sender, true, "hero netCrossFade exploit");
			return;
		}
		currentAnimation = aniName;
		if (base.animation != null)
		{
			base.animation.CrossFade(aniName, time);
		}
	}

	[RPC]
	public void netDie(Vector3 v, bool isBite, int viewID = -1, string titanName = "", bool killByTitan = true, PhotonMessageInfo info = null)
	{
		if (base.photonView.isMine && info != null && IN_GAME_MAIN_CAMERA.gamemode != GAMEMODE.BOSS_FIGHT_CT)
		{
			if (FengGameManagerMKII.ignoreList.Contains(info.sender.ID))
			{
				base.photonView.RPC("backToHumanRPC", PhotonTargets.Others);
				return;
			}
			if (!info.sender.isLocal && !info.sender.isMasterClient)
			{
				if (info.sender.customProperties[PhotonPlayerProperty.name] == null || info.sender.customProperties[PhotonPlayerProperty.isTitan] == null)
				{
					FengGameManagerMKII.instance.chatRoom.addLINE("<color=#FFCC00>Unusual Kill from ID " + info.sender.ID + "</color>");
				}
				else if (viewID < 0)
				{
					if (titanName == "")
					{
						FengGameManagerMKII.instance.chatRoom.addLINE("<color=#FFCC00>Unusual Kill from ID " + info.sender.ID + " (possibly valid).</color>");
					}
					else
					{
						FengGameManagerMKII.instance.chatRoom.addLINE("<color=#FFCC00>Unusual Kill from ID " + info.sender.ID + "</color>");
					}
				}
				else if (PhotonView.Find(viewID) == null)
				{
					FengGameManagerMKII.instance.chatRoom.addLINE("<color=#FFCC00>Unusual Kill from ID " + info.sender.ID + "</color>");
				}
				else if (PhotonView.Find(viewID).owner.ID != info.sender.ID)
				{
					FengGameManagerMKII.instance.chatRoom.addLINE("<color=#FFCC00>Unusual Kill from ID " + info.sender.ID + "</color>");
				}
			}
		}
		if (PhotonNetwork.isMasterClient)
		{
			onDeathEvent(viewID, killByTitan);
			int iD = base.photonView.owner.ID;
			if (FengGameManagerMKII.heroHash.ContainsKey(iD))
			{
				FengGameManagerMKII.heroHash.Remove(iD);
			}
		}
		if (base.photonView.isMine)
		{
			Vector3 localPosition = Vector3.up * 5000f;
			if (myBomb != null)
			{
				myBomb.DestroySelf();
			}
			if (myCannon != null)
			{
				PhotonNetwork.Destroy(myCannon);
			}
			if (titanForm && eren_titan != null)
			{
				eren_titan.GetComponent<TITAN_EREN>().lifeTime = 0.1f;
			}
			if (skillCD != null)
			{
				skillCD.transform.localPosition = localPosition;
			}
		}
		if (bulletLeft != null)
		{
			bulletLeft.GetComponent<Bullet>().removeMe();
		}
		if (bulletRight != null)
		{
			bulletRight.GetComponent<Bullet>().removeMe();
		}
		meatDie.Play();
		if (!useGun && (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE || base.photonView.isMine))
		{
			leftbladetrail.Deactivate();
			rightbladetrail.Deactivate();
			leftbladetrail2.Deactivate();
			rightbladetrail2.Deactivate();
		}
		falseAttack();
		breakApart2(v, isBite);
		if (base.photonView.isMine)
		{
			currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().setSpectorMode(false);
			currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
			FengGameManagerMKII.instance.myRespawnTime = 0f;
		}
		hasDied = true;
		Transform transform = base.transform.Find("audio_die");
		if (transform != null)
		{
			transform.parent = null;
			transform.GetComponent<AudioSource>().Play();
		}
		base.gameObject.GetComponent<SmoothSyncMovement>().disabled = true;
		if (base.photonView.isMine)
		{
			PhotonNetwork.RemoveRPCs(base.photonView);
			ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
			hashtable.Add(PhotonPlayerProperty.dead, true);
			PhotonNetwork.player.SetCustomProperties(hashtable);
			hashtable = new ExitGames.Client.Photon.Hashtable();
			hashtable.Add(PhotonPlayerProperty.deaths, RCextensions.returnIntFromObject(PhotonNetwork.player.customProperties[PhotonPlayerProperty.deaths]) + 1);
			PhotonNetwork.player.SetCustomProperties(hashtable);
			object[] parameters = new object[1] { (!(titanName == string.Empty)) ? 1 : 0 };
			FengGameManagerMKII.instance.photonView.RPC("someOneIsDead", PhotonTargets.MasterClient, parameters);
			if (viewID != -1)
			{
				PhotonView photonView = PhotonView.Find(viewID);
				if (photonView != null)
				{
					FengGameManagerMKII.instance.sendKillInfo(killByTitan, "[FFC000][" + info.sender.ID + "][FFFFFF]" + RCextensions.returnStringFromObject(photonView.owner.customProperties[PhotonPlayerProperty.name]), false, RCextensions.returnStringFromObject(PhotonNetwork.player.customProperties[PhotonPlayerProperty.name]));
					hashtable = new ExitGames.Client.Photon.Hashtable();
					hashtable.Add(PhotonPlayerProperty.kills, RCextensions.returnIntFromObject(photonView.owner.customProperties[PhotonPlayerProperty.kills]) + 1);
					photonView.owner.SetCustomProperties(hashtable);
				}
			}
			else
			{
				FengGameManagerMKII.instance.sendKillInfo(!(titanName == string.Empty), "[FFC000][" + info.sender.ID + "][FFFFFF]" + titanName, false, RCextensions.returnStringFromObject(PhotonNetwork.player.customProperties[PhotonPlayerProperty.name]));
			}
		}
		if (base.photonView.isMine)
		{
			PhotonNetwork.Destroy(base.photonView);
		}
		if (viewID != -1)
		{
			PhotonView photonView2 = PhotonView.Find(viewID);
			if (photonView2 != null && photonView2.isMine && photonView2.GetComponent<TITAN>() != null)
			{
				GameProgressManager.RegisterHumanKill(photonView2.gameObject, this, KillWeapon.Titan);
			}
		}
	}

	[RPC]
	private void netDie2(int viewID = -1, string titanName = "", PhotonMessageInfo info = null)
	{
		if (base.photonView.isMine && info != null && IN_GAME_MAIN_CAMERA.gamemode != GAMEMODE.BOSS_FIGHT_CT)
		{
			if (FengGameManagerMKII.ignoreList.Contains(info.sender.ID))
			{
				base.photonView.RPC("backToHumanRPC", PhotonTargets.Others);
				return;
			}
			if (!info.sender.isLocal && !info.sender.isMasterClient)
			{
				if (info.sender.customProperties[PhotonPlayerProperty.name] == null || info.sender.customProperties[PhotonPlayerProperty.isTitan] == null)
				{
					FengGameManagerMKII.instance.chatRoom.addLINE("<color=#FFCC00>Unusual Kill from ID " + info.sender.ID + "</color>");
				}
				else if (viewID < 0)
				{
					if (titanName == "")
					{
						FengGameManagerMKII.instance.chatRoom.addLINE("<color=#FFCC00>Unusual Kill from ID " + info.sender.ID + " (possibly valid).</color>");
					}
					else if (!SettingsManager.LegacyGameSettings.BombModeEnabled.Value && !SettingsManager.LegacyGameSettings.CannonsFriendlyFire.Value)
					{
						FengGameManagerMKII.instance.chatRoom.addLINE("<color=#FFCC00>Unusual Kill from ID " + info.sender.ID + "</color>");
					}
				}
				else if (PhotonView.Find(viewID) == null)
				{
					FengGameManagerMKII.instance.chatRoom.addLINE("<color=#FFCC00>Unusual Kill from ID " + info.sender.ID + "</color>");
				}
				else if (PhotonView.Find(viewID).owner.ID != info.sender.ID)
				{
					FengGameManagerMKII.instance.chatRoom.addLINE("<color=#FFCC00>Unusual Kill from ID " + info.sender.ID + "</color>");
				}
			}
		}
		if (base.photonView.isMine)
		{
			Vector3 localPosition = Vector3.up * 5000f;
			if (myBomb != null)
			{
				myBomb.DestroySelf();
			}
			if (myCannon != null)
			{
				PhotonNetwork.Destroy(myCannon);
			}
			PhotonNetwork.RemoveRPCs(base.photonView);
			if (titanForm && eren_titan != null)
			{
				eren_titan.GetComponent<TITAN_EREN>().lifeTime = 0.1f;
			}
			if (skillCD != null)
			{
				skillCD.transform.localPosition = localPosition;
			}
		}
		meatDie.Play();
		if (bulletLeft != null)
		{
			bulletLeft.GetComponent<Bullet>().removeMe();
		}
		if (bulletRight != null)
		{
			bulletRight.GetComponent<Bullet>().removeMe();
		}
		Transform transform = base.transform.Find("audio_die");
		transform.parent = null;
		transform.GetComponent<AudioSource>().Play();
		if (base.photonView.isMine)
		{
			currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(null);
			currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().setSpectorMode(true);
			currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
			FengGameManagerMKII.instance.myRespawnTime = 0f;
		}
		falseAttack();
		hasDied = true;
		base.gameObject.GetComponent<SmoothSyncMovement>().disabled = true;
		if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && base.photonView.isMine)
		{
			PhotonNetwork.RemoveRPCs(base.photonView);
			ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
			hashtable.Add(PhotonPlayerProperty.dead, true);
			PhotonNetwork.player.SetCustomProperties(hashtable);
			hashtable = new ExitGames.Client.Photon.Hashtable();
			hashtable.Add(PhotonPlayerProperty.deaths, (int)PhotonNetwork.player.customProperties[PhotonPlayerProperty.deaths] + 1);
			PhotonNetwork.player.SetCustomProperties(hashtable);
			if (viewID != -1)
			{
				PhotonView photonView = PhotonView.Find(viewID);
				if (photonView != null)
				{
					FengGameManagerMKII.instance.sendKillInfo(true, "[FFC000][" + info.sender.ID + "][FFFFFF]" + RCextensions.returnStringFromObject(photonView.owner.customProperties[PhotonPlayerProperty.name]), false, RCextensions.returnStringFromObject(PhotonNetwork.player.customProperties[PhotonPlayerProperty.name]));
					hashtable = new ExitGames.Client.Photon.Hashtable();
					hashtable.Add(PhotonPlayerProperty.kills, RCextensions.returnIntFromObject(photonView.owner.customProperties[PhotonPlayerProperty.kills]) + 1);
					photonView.owner.SetCustomProperties(hashtable);
				}
			}
			else
			{
				FengGameManagerMKII.instance.sendKillInfo(true, "[FFC000][" + info.sender.ID + "][FFFFFF]" + titanName, false, RCextensions.returnStringFromObject(PhotonNetwork.player.customProperties[PhotonPlayerProperty.name]));
			}
			object[] parameters = new object[1] { (!(titanName == string.Empty)) ? 1 : 0 };
			FengGameManagerMKII.instance.photonView.RPC("someOneIsDead", PhotonTargets.MasterClient, parameters);
		}
		GameObject gameObject = ((IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE || !base.photonView.isMine) ? ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("hitMeat2"))) : PhotonNetwork.Instantiate("hitMeat2", base.transform.position, Quaternion.Euler(270f, 0f, 0f), 0));
		gameObject.transform.position = base.transform.position;
		if (base.photonView.isMine)
		{
			PhotonNetwork.Destroy(base.photonView);
		}
		if (PhotonNetwork.isMasterClient)
		{
			onDeathEvent(viewID, true);
			int iD = base.photonView.owner.ID;
			if (FengGameManagerMKII.heroHash.ContainsKey(iD))
			{
				FengGameManagerMKII.heroHash.Remove(iD);
			}
		}
		if (viewID != -1)
		{
			PhotonView photonView2 = PhotonView.Find(viewID);
			if (photonView2 != null && photonView2.isMine && photonView2.GetComponent<TITAN>() != null)
			{
				GameProgressManager.RegisterHumanKill(photonView2.gameObject, this, KillWeapon.Titan);
			}
		}
	}

	public void netDieLocal(Vector3 v, bool isBite, int viewID = -1, string titanName = "", bool killByTitan = true)
	{
		if (base.photonView.isMine)
		{
			Vector3 localPosition = Vector3.up * 5000f;
			if (titanForm && eren_titan != null)
			{
				eren_titan.GetComponent<TITAN_EREN>().lifeTime = 0.1f;
			}
			if (myBomb != null)
			{
				myBomb.DestroySelf();
			}
			if (myCannon != null)
			{
				PhotonNetwork.Destroy(myCannon);
			}
			if (skillCD != null)
			{
				skillCD.transform.localPosition = localPosition;
			}
		}
		if (bulletLeft != null)
		{
			bulletLeft.GetComponent<Bullet>().removeMe();
		}
		if (bulletRight != null)
		{
			bulletRight.GetComponent<Bullet>().removeMe();
		}
		meatDie.Play();
		if (!useGun && (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE || base.photonView.isMine))
		{
			leftbladetrail.Deactivate();
			rightbladetrail.Deactivate();
			leftbladetrail2.Deactivate();
			rightbladetrail2.Deactivate();
		}
		falseAttack();
		breakApart2(v, isBite);
		if (base.photonView.isMine)
		{
			currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().setSpectorMode(false);
			currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
			FengGameManagerMKII.instance.myRespawnTime = 0f;
		}
		hasDied = true;
		Transform transform = base.transform.Find("audio_die");
		transform.parent = null;
		transform.GetComponent<AudioSource>().Play();
		base.gameObject.GetComponent<SmoothSyncMovement>().disabled = true;
		if (base.photonView.isMine)
		{
			PhotonNetwork.RemoveRPCs(base.photonView);
			ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
			hashtable.Add(PhotonPlayerProperty.dead, true);
			PhotonNetwork.player.SetCustomProperties(hashtable);
			hashtable = new ExitGames.Client.Photon.Hashtable();
			hashtable.Add(PhotonPlayerProperty.deaths, RCextensions.returnIntFromObject(PhotonNetwork.player.customProperties[PhotonPlayerProperty.deaths]) + 1);
			PhotonNetwork.player.SetCustomProperties(hashtable);
			object[] parameters = new object[1] { (!(titanName == string.Empty)) ? 1 : 0 };
			FengGameManagerMKII.instance.photonView.RPC("someOneIsDead", PhotonTargets.MasterClient, parameters);
			if (viewID != -1)
			{
				PhotonView photonView = PhotonView.Find(viewID);
				if (photonView != null)
				{
					FengGameManagerMKII.instance.sendKillInfo(killByTitan, RCextensions.returnStringFromObject(photonView.owner.customProperties[PhotonPlayerProperty.name]), false, RCextensions.returnStringFromObject(PhotonNetwork.player.customProperties[PhotonPlayerProperty.name]));
					hashtable = new ExitGames.Client.Photon.Hashtable();
					hashtable.Add(PhotonPlayerProperty.kills, RCextensions.returnIntFromObject(photonView.owner.customProperties[PhotonPlayerProperty.kills]) + 1);
					photonView.owner.SetCustomProperties(hashtable);
				}
			}
			else
			{
				FengGameManagerMKII.instance.sendKillInfo(!(titanName == string.Empty), titanName, false, RCextensions.returnStringFromObject(PhotonNetwork.player.customProperties[PhotonPlayerProperty.name]));
			}
		}
		if (base.photonView.isMine)
		{
			PhotonNetwork.Destroy(base.photonView);
		}
		if (PhotonNetwork.isMasterClient)
		{
			onDeathEvent(viewID, killByTitan);
			int iD = base.photonView.owner.ID;
			if (FengGameManagerMKII.heroHash.ContainsKey(iD))
			{
				FengGameManagerMKII.heroHash.Remove(iD);
			}
		}
	}

	[RPC]
	private void netGrabbed(int id, bool leftHand, PhotonMessageInfo info)
	{
		if (info != null && !info.sender.isMasterClient && (RCextensions.returnIntFromObject(info.sender.customProperties[PhotonPlayerProperty.isTitan]) != 2 || RCextensions.returnBoolFromObject(info.sender.customProperties[PhotonPlayerProperty.dead])))
		{
			FengGameManagerMKII.instance.kickPlayerRCIfMC(info.sender, true, "hero netGrabbed exploit");
			return;
		}
		titanWhoGrabMeID = id;
		grabbed(PhotonView.Find(id).gameObject, leftHand);
	}

	[RPC]
	private void netlaughAttack(PhotonMessageInfo info)
	{
		if (info != null && info.sender != base.photonView.owner)
		{
			FengGameManagerMKII.instance.kickPlayerRCIfMC(info.sender, true, "hero netlaughattack exploit");
		}
		GameObject[] array = GameObject.FindGameObjectsWithTag("titan");
		foreach (GameObject gameObject in array)
		{
			if (Vector3.Distance(gameObject.transform.position, base.transform.position) < 50f && Vector3.Angle(gameObject.transform.forward, base.transform.position - gameObject.transform.position) < 90f && gameObject.GetComponent<TITAN>() != null)
			{
				gameObject.GetComponent<TITAN>().beLaughAttacked();
			}
		}
	}

	[RPC]
	private void netPauseAnimation(PhotonMessageInfo info)
	{
		if (info != null && info.sender != base.photonView.owner)
		{
			FengGameManagerMKII.instance.kickPlayerRCIfMC(info.sender, true, "hero netPauseAniamtion");
			return;
		}
		foreach (AnimationState item in base.animation)
		{
			item.speed = 0f;
		}
	}

	[RPC]
	private void netPlayAnimation(string aniName, PhotonMessageInfo info)
	{
		if (info != null && info.sender != base.photonView.owner && aniName != "grabbed")
		{
			FengGameManagerMKII.instance.kickPlayerRCIfMC(info.sender, true, "hero netPlayAnimation exploit");
			return;
		}
		currentAnimation = aniName;
		if (base.animation != null)
		{
			base.animation.Play(aniName);
		}
	}

	[RPC]
	private void netPlayAnimationAt(string aniName, float normalizedTime, PhotonMessageInfo info)
	{
		if (info != null && info.sender != base.photonView.owner)
		{
			FengGameManagerMKII.instance.kickPlayerRCIfMC(info.sender, true, "hero netPlayAnimationAt exploit");
			return;
		}
		currentAnimation = aniName;
		if (base.animation != null)
		{
			base.animation.Play(aniName);
			base.animation[aniName].normalizedTime = normalizedTime;
		}
	}

	[RPC]
	private void netSetIsGrabbedFalse(PhotonMessageInfo info)
	{
		if (info != null && info.sender != base.photonView.owner)
		{
			FengGameManagerMKII.instance.kickPlayerRCIfMC(info.sender, true, "hero netSetIsGrabbedFalse");
		}
		else
		{
			state = HERO_STATE.Idle;
		}
	}

	[RPC]
	private void netTauntAttack(float tauntTime, float distance, PhotonMessageInfo info)
	{
		if (info != null && info.sender != base.photonView.owner)
		{
			FengGameManagerMKII.instance.kickPlayerRCIfMC(info.sender, true, "hero netTauntAttack");
			return;
		}
		GameObject[] array = GameObject.FindGameObjectsWithTag("titan");
		foreach (GameObject gameObject in array)
		{
			if (Vector3.Distance(gameObject.transform.position, base.transform.position) < distance && gameObject.GetComponent<TITAN>() != null)
			{
				gameObject.GetComponent<TITAN>().beTauntedBy(base.gameObject, tauntTime);
			}
		}
	}

	[RPC]
	public void netUngrabbed()
	{
		ungrabbed();
		netPlayAnimation(standAnimation, null);
		falseAttack();
	}

	public void onDeathEvent(int viewID, bool isTitan)
	{
		if (isTitan)
		{
			if (FengGameManagerMKII.RCEvents.ContainsKey("OnPlayerDieByTitan"))
			{
				RCEvent rCEvent = (RCEvent)FengGameManagerMKII.RCEvents["OnPlayerDieByTitan"];
				string[] array = (string[])FengGameManagerMKII.RCVariableNames["OnPlayerDieByTitan"];
				if (FengGameManagerMKII.playerVariables.ContainsKey(array[0]))
				{
					FengGameManagerMKII.playerVariables[array[0]] = base.photonView.owner;
				}
				else
				{
					FengGameManagerMKII.playerVariables.Add(array[0], base.photonView.owner);
				}
				if (FengGameManagerMKII.titanVariables.ContainsKey(array[1]))
				{
					FengGameManagerMKII.titanVariables[array[1]] = PhotonView.Find(viewID).gameObject.GetComponent<TITAN>();
				}
				else
				{
					FengGameManagerMKII.titanVariables.Add(array[1], PhotonView.Find(viewID).gameObject.GetComponent<TITAN>());
				}
				rCEvent.checkEvent();
			}
		}
		else if (FengGameManagerMKII.RCEvents.ContainsKey("OnPlayerDieByPlayer"))
		{
			RCEvent rCEvent = (RCEvent)FengGameManagerMKII.RCEvents["OnPlayerDieByPlayer"];
			string[] array = (string[])FengGameManagerMKII.RCVariableNames["OnPlayerDieByPlayer"];
			if (FengGameManagerMKII.playerVariables.ContainsKey(array[0]))
			{
				FengGameManagerMKII.playerVariables[array[0]] = base.photonView.owner;
			}
			else
			{
				FengGameManagerMKII.playerVariables.Add(array[0], base.photonView.owner);
			}
			if (FengGameManagerMKII.playerVariables.ContainsKey(array[1]))
			{
				FengGameManagerMKII.playerVariables[array[1]] = PhotonView.Find(viewID).owner;
			}
			else
			{
				FengGameManagerMKII.playerVariables.Add(array[1], PhotonView.Find(viewID).owner);
			}
			rCEvent.checkEvent();
		}
	}

	private void OnDestroy()
	{
		if (myNetWorkName != null)
		{
			UnityEngine.Object.Destroy(myNetWorkName);
		}
		if (gunDummy != null)
		{
			UnityEngine.Object.Destroy(gunDummy);
		}
		if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER)
		{
			releaseIfIHookSb();
		}
		if (GameObject.Find("MultiplayerManager") != null)
		{
			GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().removeHero(this);
		}
		if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && base.photonView.isMine)
		{
			Vector3 localPosition = Vector3.up * 5000f;
			cross1.transform.localPosition = localPosition;
			cross2.transform.localPosition = localPosition;
			crossL1.transform.localPosition = localPosition;
			crossL2.transform.localPosition = localPosition;
			crossR1.transform.localPosition = localPosition;
			crossR2.transform.localPosition = localPosition;
			LabelDistance.transform.localPosition = localPosition;
		}
		if (setup.part_cape != null)
		{
			ClothFactory.DisposeObject(setup.part_cape);
		}
		if (setup.part_hair_1 != null)
		{
			ClothFactory.DisposeObject(setup.part_hair_1);
		}
		if (setup.part_hair_2 != null)
		{
			ClothFactory.DisposeObject(setup.part_hair_2);
		}
		if (IsMine())
		{
			GameMenu.ToggleEmoteWheel(false);
		}
	}

	public void pauseAnimation()
	{
		if (_animationStopped)
		{
			return;
		}
		_animationStopped = true;
		foreach (AnimationState item in base.animation)
		{
			item.speed = 0f;
		}
		if (IN_GAME_MAIN_CAMERA.gametype != 0 && base.photonView.isMine)
		{
			base.photonView.RPC("netPauseAnimation", PhotonTargets.Others);
		}
	}

	public void playAnimation(string aniName)
	{
		currentAnimation = aniName;
		base.animation.Play(aniName);
		if (PhotonNetwork.connected && base.photonView.isMine)
		{
			object[] parameters = new object[1] { aniName };
			base.photonView.RPC("netPlayAnimation", PhotonTargets.Others, parameters);
		}
	}

	private void playAnimationAt(string aniName, float normalizedTime)
	{
		currentAnimation = aniName;
		base.animation.Play(aniName);
		base.animation[aniName].normalizedTime = normalizedTime;
		if (PhotonNetwork.connected && base.photonView.isMine)
		{
			object[] parameters = new object[2] { aniName, normalizedTime };
			base.photonView.RPC("netPlayAnimationAt", PhotonTargets.Others, parameters);
		}
	}

	private void releaseIfIHookSb()
	{
		if (hookSomeOne && hookTarget != null)
		{
			hookTarget.GetPhotonView().RPC("badGuyReleaseMe", hookTarget.GetPhotonView().owner);
			hookTarget = null;
			hookSomeOne = false;
		}
	}

	public void resetAnimationSpeed()
	{
		foreach (AnimationState item in base.animation)
		{
			item.speed = 1f;
		}
		customAnimationSpeed();
	}

	[RPC]
	public void ReturnFromCannon(PhotonMessageInfo info)
	{
		if (info != null && info.sender == base.photonView.owner)
		{
			isCannon = false;
			base.gameObject.GetComponent<SmoothSyncMovement>().disabled = false;
		}
	}

	private void rightArmAimTo(Vector3 target)
	{
		float num = target.x - upperarmR.transform.position.x;
		float y = target.y - upperarmR.transform.position.y;
		float num2 = target.z - upperarmR.transform.position.z;
		float x = Mathf.Sqrt(num * num + num2 * num2);
		handR.localRotation = Quaternion.Euler(-90f, 0f, 0f);
		forearmR.localRotation = Quaternion.Euler(90f, 0f, 0f);
		upperarmR.rotation = Quaternion.Euler(180f, 90f + Mathf.Atan2(num, num2) * 57.29578f, Mathf.Atan2(y, x) * 57.29578f);
	}

	[RPC]
	private void RPCHookedByHuman(int hooker, Vector3 hookPosition)
	{
		hookBySomeOne = true;
		badGuy = PhotonView.Find(hooker).gameObject;
		if (Vector3.Distance(hookPosition, base.transform.position) < 15f)
		{
			launchForce = PhotonView.Find(hooker).gameObject.transform.position - base.transform.position;
			base.rigidbody.AddForce(-base.rigidbody.velocity * 0.9f, ForceMode.VelocityChange);
			float num = Mathf.Pow(launchForce.magnitude, 0.1f);
			if (grounded)
			{
				base.rigidbody.AddForce(Vector3.up * Mathf.Min(launchForce.magnitude * 0.2f, 10f), ForceMode.Impulse);
			}
			base.rigidbody.AddForce(launchForce * num * 0.1f, ForceMode.Impulse);
			if (state != HERO_STATE.Grab)
			{
				dashTime = 1f;
				crossFade("dash", 0.05f);
				base.animation["dash"].time = 0.1f;
				state = HERO_STATE.AirDodge;
				falseAttack();
				facingDirection = Mathf.Atan2(launchForce.x, launchForce.z) * 57.29578f;
				Quaternion rotation = Quaternion.Euler(0f, facingDirection, 0f);
				base.gameObject.transform.rotation = rotation;
				base.rigidbody.rotation = rotation;
				targetRotation = rotation;
			}
		}
		else
		{
			hookBySomeOne = false;
			badGuy = null;
			PhotonView.Find(hooker).RPC("hookFail", PhotonView.Find(hooker).owner);
		}
	}

	public void SetGhostSmoke(int state)
	{
		if (state == 1)
		{
			smoke_3dmg.enableEmission = true;
			smoke_3dmg.startColor = Color.cyan;
		}
		else
		{
			smoke_3dmg.enableEmission = false;
		}
	}

	private void setHookedPplDirection()
	{
		almostSingleHook = false;
		float num = facingDirection;
		if (isRightHandHooked && isLeftHandHooked)
		{
			if (bulletLeft != null && bulletRight != null)
			{
				Vector3 normal = bulletLeft.transform.position - bulletRight.transform.position;
				if (normal.sqrMagnitude < 4f)
				{
					Vector3 vector = (bulletLeft.transform.position + bulletRight.transform.position) * 0.5f - base.transform.position;
					facingDirection = Mathf.Atan2(vector.x, vector.z) * 57.29578f;
					if (useGun && state != HERO_STATE.Attack)
					{
						float current = (0f - Mathf.Atan2(base.rigidbody.velocity.z, base.rigidbody.velocity.x)) * 57.29578f;
						float target = (0f - Mathf.Atan2(vector.z, vector.x)) * 57.29578f;
						float num2 = 0f - Mathf.DeltaAngle(current, target);
						facingDirection += num2;
					}
					almostSingleHook = true;
				}
				else
				{
					Vector3 to = base.transform.position - bulletLeft.transform.position;
					Vector3 to2 = base.transform.position - bulletRight.transform.position;
					Vector3 vector2 = (bulletLeft.transform.position + bulletRight.transform.position) * 0.5f;
					Vector3 from = base.transform.position - vector2;
					if (Vector3.Angle(from, to) < 30f && Vector3.Angle(from, to2) < 30f)
					{
						almostSingleHook = true;
						Vector3 vector3 = vector2 - base.transform.position;
						facingDirection = Mathf.Atan2(vector3.x, vector3.z) * 57.29578f;
					}
					else
					{
						almostSingleHook = false;
						Vector3 tangent = base.transform.forward;
						Vector3.OrthoNormalize(ref normal, ref tangent);
						facingDirection = Mathf.Atan2(tangent.x, tangent.z) * 57.29578f;
						float current2 = Mathf.Atan2(to.x, to.z) * 57.29578f;
						if (Mathf.DeltaAngle(current2, facingDirection) > 0f)
						{
							facingDirection += 180f;
						}
					}
				}
			}
		}
		else
		{
			almostSingleHook = true;
			Vector3 zero = Vector3.zero;
			if (isRightHandHooked && bulletRight != null)
			{
				zero = bulletRight.transform.position - base.transform.position;
			}
			else
			{
				if (!isLeftHandHooked || !(bulletLeft != null))
				{
					return;
				}
				zero = bulletLeft.transform.position - base.transform.position;
			}
			facingDirection = Mathf.Atan2(zero.x, zero.z) * 57.29578f;
			if (state != HERO_STATE.Attack)
			{
				float current3 = (0f - Mathf.Atan2(base.rigidbody.velocity.z, base.rigidbody.velocity.x)) * 57.29578f;
				float target2 = (0f - Mathf.Atan2(zero.z, zero.x)) * 57.29578f;
				float num3 = 0f - Mathf.DeltaAngle(current3, target2);
				if (useGun)
				{
					facingDirection += num3;
				}
				else
				{
					float num4 = 0f;
					num4 = (((!isLeftHandHooked || !(num3 < 0f)) && (!isRightHandHooked || !(num3 > 0f))) ? 0.1f : (-0.1f));
					facingDirection += num3 * num4;
				}
			}
		}
		if (IsFiringThunderSpear())
		{
			facingDirection = num;
		}
	}

	[RPC]
	public void SetMyCannon(int viewID, PhotonMessageInfo info)
	{
		if (info.sender != base.photonView.owner || viewID < 0)
		{
			FengGameManagerMKII.instance.kickPlayerRCIfMC(info.sender, true, "hero setcannon exploit");
			return;
		}
		PhotonView photonView = PhotonView.Find(viewID);
		if (photonView.owner != info.sender)
		{
			FengGameManagerMKII.instance.kickPlayerRCIfMC(info.sender, true, "hero setcannon exploit");
		}
		else
		{
			if (info.sender != base.photonView.owner)
			{
				return;
			}
			PhotonView photonView2 = PhotonView.Find(viewID);
			if (photonView2 != null)
			{
				myCannon = photonView2.gameObject;
				if (myCannon != null)
				{
					myCannonBase = myCannon.transform;
					myCannonPlayer = myCannonBase.Find("PlayerPoint");
					isCannon = true;
				}
			}
		}
	}

	[RPC]
	public void SetMyPhotonCamera(float offset, PhotonMessageInfo info)
	{
		if (info != null && base.photonView.owner == info.sender)
		{
			CameraMultiplier = offset;
			GetComponent<SmoothSyncMovement>().PhotonCamera = true;
			isPhotonCamera = true;
		}
	}

	[RPC]
	private void setMyTeam(int val)
	{
		myTeam = val;
		if (checkBoxLeft != null)
		{
			checkBoxLeft.GetComponent<TriggerColliderWeapon>().myTeam = val;
		}
		if (checkBoxRight != null)
		{
			checkBoxRight.GetComponent<TriggerColliderWeapon>().myTeam = val;
		}
		if (IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.MULTIPLAYER || !PhotonNetwork.isMasterClient)
		{
			return;
		}
		if (SettingsManager.LegacyGameSettings.FriendlyMode.Value)
		{
			if (val != 1)
			{
				object[] parameters = new object[1] { 1 };
				base.photonView.RPC("setMyTeam", PhotonTargets.AllBuffered, parameters);
			}
		}
		else if (SettingsManager.LegacyGameSettings.BladePVP.Value == 1)
		{
			int num = 0;
			if (base.photonView.owner.customProperties[PhotonPlayerProperty.RCteam] != null)
			{
				num = RCextensions.returnIntFromObject(base.photonView.owner.customProperties[PhotonPlayerProperty.RCteam]);
			}
			if (val != num)
			{
				object[] parameters = new object[1] { num };
				base.photonView.RPC("setMyTeam", PhotonTargets.AllBuffered, parameters);
			}
		}
		else if (SettingsManager.LegacyGameSettings.BladePVP.Value == 2 && val != base.photonView.owner.ID)
		{
			object[] parameters = new object[1] { base.photonView.owner.ID };
			base.photonView.RPC("setMyTeam", PhotonTargets.AllBuffered, parameters);
		}
	}

	public void setSkillHUDPosition2()
	{
		skillCD = GameObject.Find("skill_cd_" + skillIDHUD);
		if (skillCD != null)
		{
			skillCD.transform.localPosition = GameObject.Find("skill_cd_bottom").transform.localPosition;
		}
		if (useGun && !SettingsManager.LegacyGameSettings.BombModeEnabled.Value)
		{
			skillCD.transform.localPosition = Vector3.up * 5000f;
		}
	}

	public void setStat2()
	{
		skillCDLast = 1.5f;
		skillId = setup.myCostume.stat.skillId;
		if (skillId == "levi")
		{
			skillCDLast = 3.5f;
		}
		customAnimationSpeed();
		if (skillId == "armin")
		{
			skillCDLast = 5f;
		}
		if (skillId == "marco")
		{
			skillCDLast = 10f;
		}
		if (skillId == "jean")
		{
			skillCDLast = 0.001f;
		}
		if (skillId == "eren")
		{
			skillCDLast = 120f;
			if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && (LevelInfo.getInfo(FengGameManagerMKII.level).teamTitan || LevelInfo.getInfo(FengGameManagerMKII.level).type == GAMEMODE.RACING || LevelInfo.getInfo(FengGameManagerMKII.level).type == GAMEMODE.PVP_CAPTURE || LevelInfo.getInfo(FengGameManagerMKII.level).type == GAMEMODE.TROST))
			{
				skillId = "petra";
				skillCDLast = 1f;
			}
		}
		if (skillId == "sasha")
		{
			skillCDLast = 20f;
		}
		if (skillId == "petra")
		{
			skillCDLast = 3.5f;
		}
		bombInit();
		speed = (float)setup.myCostume.stat.SPD / 10f;
		totalGas = (currentGas = setup.myCostume.stat.GAS);
		totalBladeSta = (currentBladeSta = setup.myCostume.stat.BLA);
		baseRigidBody.mass = 0.5f - (float)(setup.myCostume.stat.ACL - 100) * 0.001f;
		GameObject.Find("skill_cd_bottom").transform.localPosition = new Vector3(0f, (float)(-Screen.height) * 0.5f + 5f, 0f);
		skillCD = GameObject.Find("skill_cd_" + skillIDHUD);
		skillCD.transform.localPosition = GameObject.Find("skill_cd_bottom").transform.localPosition;
		GameObject.Find("GasUI").transform.localPosition = GameObject.Find("skill_cd_bottom").transform.localPosition;
		if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE || base.photonView.isMine)
		{
			GameObject.Find("bulletL").GetComponent<UISprite>().enabled = false;
			GameObject.Find("bulletR").GetComponent<UISprite>().enabled = false;
			GameObject.Find("bulletL1").GetComponent<UISprite>().enabled = false;
			GameObject.Find("bulletR1").GetComponent<UISprite>().enabled = false;
			GameObject.Find("bulletL2").GetComponent<UISprite>().enabled = false;
			GameObject.Find("bulletR2").GetComponent<UISprite>().enabled = false;
			GameObject.Find("bulletL3").GetComponent<UISprite>().enabled = false;
			GameObject.Find("bulletR3").GetComponent<UISprite>().enabled = false;
			GameObject.Find("bulletL4").GetComponent<UISprite>().enabled = false;
			GameObject.Find("bulletR4").GetComponent<UISprite>().enabled = false;
			GameObject.Find("bulletL5").GetComponent<UISprite>().enabled = false;
			GameObject.Find("bulletR5").GetComponent<UISprite>().enabled = false;
			GameObject.Find("bulletL6").GetComponent<UISprite>().enabled = false;
			GameObject.Find("bulletR6").GetComponent<UISprite>().enabled = false;
			GameObject.Find("bulletL7").GetComponent<UISprite>().enabled = false;
			GameObject.Find("bulletR7").GetComponent<UISprite>().enabled = false;
		}
		if (setup.myCostume.uniform_type == UNIFORM_TYPE.CasualAHSS)
		{
			standAnimation = "AHSS_stand_gun";
			useGun = true;
			gunDummy = new GameObject();
			gunDummy.name = "gunDummy";
			gunDummy.transform.position = baseTransform.position;
			gunDummy.transform.rotation = baseTransform.rotation;
			myGroup = GROUP.A;
			setTeam2(2);
			if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE || base.photonView.isMine)
			{
				GameObject.Find("bladeCL").GetComponent<UISprite>().enabled = false;
				GameObject.Find("bladeCR").GetComponent<UISprite>().enabled = false;
				GameObject.Find("bladel1").GetComponent<UISprite>().enabled = false;
				GameObject.Find("blader1").GetComponent<UISprite>().enabled = false;
				GameObject.Find("bladel2").GetComponent<UISprite>().enabled = false;
				GameObject.Find("blader2").GetComponent<UISprite>().enabled = false;
				GameObject.Find("bladel3").GetComponent<UISprite>().enabled = false;
				GameObject.Find("blader3").GetComponent<UISprite>().enabled = false;
				GameObject.Find("bladel4").GetComponent<UISprite>().enabled = false;
				GameObject.Find("blader4").GetComponent<UISprite>().enabled = false;
				GameObject.Find("bladel5").GetComponent<UISprite>().enabled = false;
				GameObject.Find("blader5").GetComponent<UISprite>().enabled = false;
				GameObject.Find("bulletL").GetComponent<UISprite>().enabled = true;
				GameObject.Find("bulletR").GetComponent<UISprite>().enabled = true;
				GameObject.Find("bulletL1").GetComponent<UISprite>().enabled = true;
				GameObject.Find("bulletR1").GetComponent<UISprite>().enabled = true;
				GameObject.Find("bulletL2").GetComponent<UISprite>().enabled = true;
				GameObject.Find("bulletR2").GetComponent<UISprite>().enabled = true;
				GameObject.Find("bulletL3").GetComponent<UISprite>().enabled = true;
				GameObject.Find("bulletR3").GetComponent<UISprite>().enabled = true;
				GameObject.Find("bulletL4").GetComponent<UISprite>().enabled = true;
				GameObject.Find("bulletR4").GetComponent<UISprite>().enabled = true;
				GameObject.Find("bulletL5").GetComponent<UISprite>().enabled = true;
				GameObject.Find("bulletR5").GetComponent<UISprite>().enabled = true;
				GameObject.Find("bulletL6").GetComponent<UISprite>().enabled = true;
				GameObject.Find("bulletR6").GetComponent<UISprite>().enabled = true;
				GameObject.Find("bulletL7").GetComponent<UISprite>().enabled = true;
				GameObject.Find("bulletR7").GetComponent<UISprite>().enabled = true;
				if (skillId != "bomb")
				{
					skillCD.transform.localPosition = Vector3.up * 5000f;
				}
			}
		}
		else if (setup.myCostume.sex == SEX.FEMALE)
		{
			standAnimation = "stand";
			setTeam2(1);
		}
		else
		{
			standAnimation = "stand_levi";
			setTeam2(1);
		}
	}

	public void setTeam2(int team)
	{
		if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && base.photonView.isMine)
		{
			object[] parameters = new object[1] { team };
			base.photonView.RPC("setMyTeam", PhotonTargets.AllBuffered, parameters);
			ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
			hashtable.Add(PhotonPlayerProperty.team, team);
			PhotonNetwork.player.SetCustomProperties(hashtable);
		}
		else
		{
			setMyTeam(team);
		}
	}

	public void shootFlare(int type)
	{
		bool flag = false;
		if (type == 1 && flare1CD == 0f)
		{
			flare1CD = flareTotalCD;
			flag = true;
		}
		if (type == 2 && flare2CD == 0f)
		{
			flare2CD = flareTotalCD;
			flag = true;
		}
		if (type == 3 && flare3CD == 0f)
		{
			flare3CD = flareTotalCD;
			flag = true;
		}
		if (flag)
		{
			if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
			{
				GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("FX/flareBullet" + type), base.transform.position, base.transform.rotation);
				gameObject.GetComponent<FlareMovement>().dontShowHint();
				UnityEngine.Object.Destroy(gameObject, 25f);
			}
			else
			{
				PhotonNetwork.Instantiate("FX/flareBullet" + type, base.transform.position, base.transform.rotation, 0).GetComponent<FlareMovement>().dontShowHint();
			}
		}
	}

	private void showAimUI2()
	{
		if (CursorManager.State == CursorState.Pointer || GameMenu.HideCrosshair)
		{
			Vector3 localPosition = Vector3.up * 10000f;
			crossL1.transform.localPosition = localPosition;
			crossL2.transform.localPosition = localPosition;
			crossR1.transform.localPosition = localPosition;
			crossR2.transform.localPosition = localPosition;
			return;
		}
		checkTitan();
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		LayerMask layerMask = 1 << PhysicsLayer.Ground;
		LayerMask layerMask2 = 1 << PhysicsLayer.EnemyBox;
		LayerMask layerMask3 = (int)layerMask2 | (int)layerMask;
		RaycastHit hitInfo;
		if (!Physics.Raycast(ray, out hitInfo, 10000000f, layerMask3.value))
		{
			return;
		}
		float magnitude = (hitInfo.point - baseTransform.position).magnitude;
		string text = string.Empty;
		if (SettingsManager.UISettings.ShowCrosshairDistance.Value)
		{
			text = ((magnitude <= 1000f) ? ((int)magnitude).ToString() : "???");
		}
		if (SettingsManager.UISettings.Speedometer.Value == 1)
		{
			if (text != string.Empty)
			{
				text += "\n";
			}
			text = text + currentSpeed.ToString("F1") + " u/s";
		}
		else if (SettingsManager.UISettings.Speedometer.Value == 2)
		{
			if (text != string.Empty)
			{
				text += "\n";
			}
			text = text + (currentSpeed / 100f).ToString("F1") + "K";
		}
		CursorManager.SetCrosshairText(text);
		if (magnitude > 120f)
		{
			CursorManager.SetCrosshairColor(false);
		}
		else
		{
			CursorManager.SetCrosshairColor(true);
		}
		if (SettingsManager.UISettings.ShowCrosshairArrows.Value)
		{
			Vector3 vector = new Vector3(0f, 0.4f, 0f);
			vector -= baseTransform.right * 0.3f;
			Vector3 vector2 = new Vector3(0f, 0.4f, 0f);
			vector2 += baseTransform.right * 0.3f;
			float num = ((hitInfo.distance <= 50f) ? (hitInfo.distance * 0.05f) : (hitInfo.distance * 0.3f));
			Vector3 vector3 = hitInfo.point - baseTransform.right * num - (baseTransform.position + vector);
			Vector3 vector4 = hitInfo.point + baseTransform.right * num - (baseTransform.position + vector2);
			vector3.Normalize();
			vector4.Normalize();
			vector3 *= 1000000f;
			vector4 *= 1000000f;
			RaycastHit hitInfo2;
			if (Physics.Linecast(baseTransform.position + vector, baseTransform.position + vector + vector3, out hitInfo2, layerMask3.value))
			{
				Transform transform = crossL1.transform;
				transform.localPosition = currentCamera.WorldToScreenPoint(hitInfo2.point);
				transform.localPosition -= new Vector3((float)Screen.width * 0.5f, (float)Screen.height * 0.5f, 0f);
				transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(transform.localPosition.y - (Input.mousePosition.y - (float)Screen.height * 0.5f), transform.localPosition.x - (Input.mousePosition.x - (float)Screen.width * 0.5f)) * 57.29578f + 180f);
				Transform transform2 = crossL2.transform;
				transform2.localPosition = transform.localPosition;
				transform2.localRotation = transform.localRotation;
				if (hitInfo2.distance > 120f)
				{
					transform.localPosition += Vector3.up * 10000f;
				}
				else
				{
					transform2.localPosition += Vector3.up * 10000f;
				}
			}
			if (Physics.Linecast(baseTransform.position + vector2, baseTransform.position + vector2 + vector4, out hitInfo2, layerMask3.value))
			{
				Transform transform3 = crossR1.transform;
				transform3.localPosition = currentCamera.WorldToScreenPoint(hitInfo2.point);
				transform3.localPosition -= new Vector3((float)Screen.width * 0.5f, (float)Screen.height * 0.5f, 0f);
				transform3.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(transform3.localPosition.y - (Input.mousePosition.y - (float)Screen.height * 0.5f), transform3.localPosition.x - (Input.mousePosition.x - (float)Screen.width * 0.5f)) * 57.29578f);
				Transform transform4 = crossR2.transform;
				transform4.localPosition = transform3.localPosition;
				transform4.localRotation = transform3.localRotation;
				if (hitInfo2.distance > 120f)
				{
					transform3.localPosition += Vector3.up * 10000f;
				}
				else
				{
					transform4.localPosition += Vector3.up * 10000f;
				}
			}
		}
		else
		{
			Vector3 localPosition = Vector3.up * 10000f;
			crossL1.transform.localPosition = localPosition;
			crossL2.transform.localPosition = localPosition;
			crossR1.transform.localPosition = localPosition;
			crossR2.transform.localPosition = localPosition;
		}
	}

	private void showFlareCD2()
	{
		if (cachedSprites["UIflare1"] != null)
		{
			cachedSprites["UIflare1"].fillAmount = (flareTotalCD - flare1CD) / flareTotalCD;
			cachedSprites["UIflare2"].fillAmount = (flareTotalCD - flare2CD) / flareTotalCD;
			cachedSprites["UIflare3"].fillAmount = (flareTotalCD - flare3CD) / flareTotalCD;
		}
	}

	private void showGas()
	{
		float num = currentGas / totalGas;
		float num2 = currentBladeSta / totalBladeSta;
		GameObject.Find("gasL1").GetComponent<UISprite>().fillAmount = currentGas / totalGas;
		GameObject.Find("gasR1").GetComponent<UISprite>().fillAmount = currentGas / totalGas;
		if (!useGun)
		{
			GameObject.Find("bladeCL").GetComponent<UISprite>().fillAmount = currentBladeSta / totalBladeSta;
			GameObject.Find("bladeCR").GetComponent<UISprite>().fillAmount = currentBladeSta / totalBladeSta;
			if (num <= 0f)
			{
				GameObject.Find("gasL").GetComponent<UISprite>().color = Color.red;
				GameObject.Find("gasR").GetComponent<UISprite>().color = Color.red;
			}
			else if (num < 0.3f)
			{
				GameObject.Find("gasL").GetComponent<UISprite>().color = Color.yellow;
				GameObject.Find("gasR").GetComponent<UISprite>().color = Color.yellow;
			}
			else
			{
				GameObject.Find("gasL").GetComponent<UISprite>().color = Color.white;
				GameObject.Find("gasR").GetComponent<UISprite>().color = Color.white;
			}
			if (num2 <= 0f)
			{
				GameObject.Find("bladel1").GetComponent<UISprite>().color = Color.red;
				GameObject.Find("blader1").GetComponent<UISprite>().color = Color.red;
			}
			else if (num2 < 0.3f)
			{
				GameObject.Find("bladel1").GetComponent<UISprite>().color = Color.yellow;
				GameObject.Find("blader1").GetComponent<UISprite>().color = Color.yellow;
			}
			else
			{
				GameObject.Find("bladel1").GetComponent<UISprite>().color = Color.white;
				GameObject.Find("blader1").GetComponent<UISprite>().color = Color.white;
			}
			if (currentBladeNum <= 4)
			{
				GameObject.Find("bladel5").GetComponent<UISprite>().enabled = false;
				GameObject.Find("blader5").GetComponent<UISprite>().enabled = false;
			}
			else
			{
				GameObject.Find("bladel5").GetComponent<UISprite>().enabled = true;
				GameObject.Find("blader5").GetComponent<UISprite>().enabled = true;
			}
			if (currentBladeNum <= 3)
			{
				GameObject.Find("bladel4").GetComponent<UISprite>().enabled = false;
				GameObject.Find("blader4").GetComponent<UISprite>().enabled = false;
			}
			else
			{
				GameObject.Find("bladel4").GetComponent<UISprite>().enabled = true;
				GameObject.Find("blader4").GetComponent<UISprite>().enabled = true;
			}
			if (currentBladeNum <= 2)
			{
				GameObject.Find("bladel3").GetComponent<UISprite>().enabled = false;
				GameObject.Find("blader3").GetComponent<UISprite>().enabled = false;
			}
			else
			{
				GameObject.Find("bladel3").GetComponent<UISprite>().enabled = true;
				GameObject.Find("blader3").GetComponent<UISprite>().enabled = true;
			}
			if (currentBladeNum <= 1)
			{
				GameObject.Find("bladel2").GetComponent<UISprite>().enabled = false;
				GameObject.Find("blader2").GetComponent<UISprite>().enabled = false;
			}
			else
			{
				GameObject.Find("bladel2").GetComponent<UISprite>().enabled = true;
				GameObject.Find("blader2").GetComponent<UISprite>().enabled = true;
			}
			if (currentBladeNum <= 0)
			{
				GameObject.Find("bladel1").GetComponent<UISprite>().enabled = false;
				GameObject.Find("blader1").GetComponent<UISprite>().enabled = false;
			}
			else
			{
				GameObject.Find("bladel1").GetComponent<UISprite>().enabled = true;
				GameObject.Find("blader1").GetComponent<UISprite>().enabled = true;
			}
		}
		else
		{
			if (leftGunHasBullet)
			{
				GameObject.Find("bulletL").GetComponent<UISprite>().enabled = true;
			}
			else
			{
				GameObject.Find("bulletL").GetComponent<UISprite>().enabled = false;
			}
			if (rightGunHasBullet)
			{
				GameObject.Find("bulletR").GetComponent<UISprite>().enabled = true;
			}
			else
			{
				GameObject.Find("bulletR").GetComponent<UISprite>().enabled = false;
			}
		}
	}

	private void showGas2()
	{
		float num = currentGas / totalGas;
		float num2 = currentBladeSta / totalBladeSta;
		cachedSprites["gasL1"].fillAmount = currentGas / totalGas;
		cachedSprites["gasR1"].fillAmount = currentGas / totalGas;
		if (!useGun)
		{
			cachedSprites["bladeCL"].fillAmount = currentBladeSta / totalBladeSta;
			cachedSprites["bladeCR"].fillAmount = currentBladeSta / totalBladeSta;
			if (num <= 0f)
			{
				cachedSprites["gasL"].color = Color.red;
				cachedSprites["gasR"].color = Color.red;
			}
			else if (num < 0.3f)
			{
				cachedSprites["gasL"].color = Color.yellow;
				cachedSprites["gasR"].color = Color.yellow;
			}
			else
			{
				cachedSprites["gasL"].color = Color.white;
				cachedSprites["gasR"].color = Color.white;
			}
			if (num2 <= 0f)
			{
				cachedSprites["bladel1"].color = Color.red;
				cachedSprites["blader1"].color = Color.red;
			}
			else if (num2 < 0.3f)
			{
				cachedSprites["bladel1"].color = Color.yellow;
				cachedSprites["blader1"].color = Color.yellow;
			}
			else
			{
				cachedSprites["bladel1"].color = Color.white;
				cachedSprites["blader1"].color = Color.white;
			}
			if (currentBladeNum <= 4)
			{
				cachedSprites["bladel5"].enabled = false;
				cachedSprites["blader5"].enabled = false;
			}
			else
			{
				cachedSprites["bladel5"].enabled = true;
				cachedSprites["blader5"].enabled = true;
			}
			if (currentBladeNum <= 3)
			{
				cachedSprites["bladel4"].enabled = false;
				cachedSprites["blader4"].enabled = false;
			}
			else
			{
				cachedSprites["bladel4"].enabled = true;
				cachedSprites["blader4"].enabled = true;
			}
			if (currentBladeNum <= 2)
			{
				cachedSprites["bladel3"].enabled = false;
				cachedSprites["blader3"].enabled = false;
			}
			else
			{
				cachedSprites["bladel3"].enabled = true;
				cachedSprites["blader3"].enabled = true;
			}
			if (currentBladeNum <= 1)
			{
				cachedSprites["bladel2"].enabled = false;
				cachedSprites["blader2"].enabled = false;
			}
			else
			{
				cachedSprites["bladel2"].enabled = true;
				cachedSprites["blader2"].enabled = true;
			}
			if (currentBladeNum <= 0)
			{
				cachedSprites["bladel1"].enabled = false;
				cachedSprites["blader1"].enabled = false;
			}
			else
			{
				cachedSprites["bladel1"].enabled = true;
				cachedSprites["blader1"].enabled = true;
			}
		}
		else
		{
			if (leftGunHasBullet)
			{
				cachedSprites["bulletL"].enabled = true;
			}
			else
			{
				cachedSprites["bulletL"].enabled = false;
			}
			if (rightGunHasBullet)
			{
				cachedSprites["bulletR"].enabled = true;
			}
			else
			{
				cachedSprites["bulletR"].enabled = false;
			}
		}
	}

	[RPC]
	private void showHitDamage(PhotonMessageInfo info)
	{
		if (info != null)
		{
			FengGameManagerMKII.instance.kickPlayerRCIfMC(info.sender, true, "hero showHitDamage exploit");
		}
	}

	private void showSkillCD()
	{
		if (skillCD != null)
		{
			skillCD.GetComponent<UISprite>().fillAmount = (skillCDLast - skillCDDuration) / skillCDLast;
		}
	}

	[RPC]
	public void SpawnCannonRPC(string settings, PhotonMessageInfo info)
	{
		if (info.sender.isMasterClient && base.photonView.isMine && myCannon == null)
		{
			if (myHorse != null && isMounted)
			{
				getOffHorse();
			}
			idle();
			if (bulletLeft != null)
			{
				bulletLeft.GetComponent<Bullet>().removeMe();
			}
			if (bulletRight != null)
			{
				bulletRight.GetComponent<Bullet>().removeMe();
			}
			if (smoke_3dmg.enableEmission && IN_GAME_MAIN_CAMERA.gametype != 0 && base.photonView.isMine)
			{
				object[] parameters = new object[1] { false };
				base.photonView.RPC("net3DMGSMOKE", PhotonTargets.Others, parameters);
			}
			smoke_3dmg.enableEmission = false;
			base.rigidbody.velocity = Vector3.zero;
			string[] array = settings.Split(',');
			if (array.Length > 15)
			{
				myCannon = PhotonNetwork.Instantiate("RCAsset/" + array[1], new Vector3(Convert.ToSingle(array[12]), Convert.ToSingle(array[13]), Convert.ToSingle(array[14])), new Quaternion(Convert.ToSingle(array[15]), Convert.ToSingle(array[16]), Convert.ToSingle(array[17]), Convert.ToSingle(array[18])), 0);
			}
			else
			{
				myCannon = PhotonNetwork.Instantiate("RCAsset/" + array[1], new Vector3(Convert.ToSingle(array[2]), Convert.ToSingle(array[3]), Convert.ToSingle(array[4])), new Quaternion(Convert.ToSingle(array[5]), Convert.ToSingle(array[6]), Convert.ToSingle(array[7]), Convert.ToSingle(array[8])), 0);
			}
			myCannonBase = myCannon.transform;
			myCannonPlayer = myCannon.transform.Find("PlayerPoint");
			isCannon = true;
			myCannon.GetComponent<Cannon>().myHero = this;
			myCannonRegion = null;
			Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(myCannon.transform.Find("Barrel").Find("FiringPoint").gameObject);
			Camera.main.fieldOfView = 55f;
			base.photonView.RPC("SetMyCannon", PhotonTargets.OthersBuffered, myCannon.GetPhotonView().viewID);
			skillCDLastCannon = skillCDLast;
			skillCDLast = 3.5f;
			skillCDDuration = 3.5f;
		}
	}

	private void Start()
	{
		FengGameManagerMKII.instance.addHero(this);
		if ((LevelInfo.getInfo(FengGameManagerMKII.level).horse || SettingsManager.LegacyGameSettings.AllowHorses.Value) && IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && base.photonView.isMine)
		{
			myHorse = PhotonNetwork.Instantiate("horse", baseTransform.position + Vector3.up * 5f, baseTransform.rotation, 0);
			myHorse.GetComponent<Horse>().myHero = base.gameObject;
			myHorse.GetComponent<TITAN_CONTROLLER>().isHorse = true;
		}
		sparks = baseTransform.Find("slideSparks").GetComponent<ParticleSystem>();
		smoke_3dmg = baseTransform.Find("3dmg_smoke").GetComponent<ParticleSystem>();
		baseTransform.localScale = new Vector3(myScale, myScale, myScale);
		facingDirection = baseTransform.rotation.eulerAngles.y;
		targetRotation = Quaternion.Euler(0f, facingDirection, 0f);
		smoke_3dmg.enableEmission = false;
		sparks.enableEmission = false;
		speedFXPS = speedFX1.GetComponent<ParticleSystem>();
		speedFXPS.enableEmission = false;
		if (IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.MULTIPLAYER)
		{
			if (Minimap.instance != null)
			{
				Minimap.instance.TrackGameObjectOnMinimap(base.gameObject, Color.green, false, true);
			}
		}
		else
		{
			if (PhotonNetwork.isMasterClient)
			{
				int iD = base.photonView.owner.ID;
				if (FengGameManagerMKII.heroHash.ContainsKey(iD))
				{
					FengGameManagerMKII.heroHash[iD] = this;
				}
				else
				{
					FengGameManagerMKII.heroHash.Add(iD, this);
				}
			}
			GameObject gameObject = GameObject.Find("UI_IN_GAME");
			myNetWorkName = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("UI/LabelNameOverHead"));
			myNetWorkName.name = "LabelNameOverHead";
			myNetWorkName.transform.parent = gameObject.GetComponent<UIReferArray>().panels[0].transform;
			myNetWorkName.transform.localScale = new Vector3(14f, 14f, 14f);
			myNetWorkName.GetComponent<UILabel>().text = string.Empty;
			if (base.photonView.isMine)
			{
				if (Minimap.instance != null)
				{
					Minimap.instance.TrackGameObjectOnMinimap(base.gameObject, Color.green, false, true);
				}
				GetComponent<SmoothSyncMovement>().PhotonCamera = true;
				base.photonView.RPC("SetMyPhotonCamera", PhotonTargets.OthersBuffered, SettingsManager.GeneralSettings.CameraDistance.Value + 0.3f);
			}
			else
			{
				bool flag = false;
				if (base.photonView.owner.customProperties[PhotonPlayerProperty.RCteam] != null)
				{
					switch (RCextensions.returnIntFromObject(base.photonView.owner.customProperties[PhotonPlayerProperty.RCteam]))
					{
					case 1:
						flag = true;
						if (Minimap.instance != null)
						{
							Minimap.instance.TrackGameObjectOnMinimap(base.gameObject, Color.cyan, false, true);
						}
						break;
					case 2:
						flag = true;
						if (Minimap.instance != null)
						{
							Minimap.instance.TrackGameObjectOnMinimap(base.gameObject, Color.magenta, false, true);
						}
						break;
					}
				}
				if (RCextensions.returnIntFromObject(base.photonView.owner.customProperties[PhotonPlayerProperty.team]) == 2)
				{
					myNetWorkName.GetComponent<UILabel>().text = "[FF0000]AHSS\n[FFFFFF]";
					if (!flag && Minimap.instance != null)
					{
						Minimap.instance.TrackGameObjectOnMinimap(base.gameObject, Color.red, false, true);
					}
				}
				else if (!flag && Minimap.instance != null)
				{
					Minimap.instance.TrackGameObjectOnMinimap(base.gameObject, Color.blue, false, true);
				}
			}
			string text = RCextensions.returnStringFromObject(base.photonView.owner.customProperties[PhotonPlayerProperty.guildName]);
			if (text != string.Empty)
			{
				UILabel component = myNetWorkName.GetComponent<UILabel>();
				string text2 = component.text;
				string[] array = new string[5]
				{
					text2,
					"[FFFF00]",
					text,
					"\n[FFFFFF]",
					RCextensions.returnStringFromObject(base.photonView.owner.customProperties[PhotonPlayerProperty.name])
				};
				component.text = string.Concat(array);
			}
			else
			{
				myNetWorkName.GetComponent<UILabel>().text += RCextensions.returnStringFromObject(base.photonView.owner.customProperties[PhotonPlayerProperty.name]);
			}
		}
		if (IN_GAME_MAIN_CAMERA.gametype != 0 && !base.photonView.isMine)
		{
			base.gameObject.layer = LayerMask.NameToLayer("NetworkObject");
			setup.init();
			setup.myCostume = new HeroCostume();
			setup.myCostume = CostumeConeveter.PhotonDataToHeroCostume2(base.photonView.owner);
			setup.setCharacterComponent();
			UnityEngine.Object.Destroy(checkBoxLeft);
			UnityEngine.Object.Destroy(checkBoxRight);
			UnityEngine.Object.Destroy(leftbladetrail);
			UnityEngine.Object.Destroy(rightbladetrail);
			UnityEngine.Object.Destroy(leftbladetrail2);
			UnityEngine.Object.Destroy(rightbladetrail2);
			hasspawn = true;
		}
		else
		{
			SetInterpolationIfEnabled(true);
			currentCamera = GameObject.Find("MainCamera").GetComponent<Camera>();
			loadskin();
			hasspawn = true;
		}
		bombImmune = false;
		if (SettingsManager.LegacyGameSettings.BombModeEnabled.Value)
		{
			bombImmune = true;
			StartCoroutine(stopImmunity());
			SetupThunderSpears();
		}
		if (_needSetupThunderspears)
		{
			CreateAndAttachThunderSpears();
		}
		_hasRunStart = true;
		SetName();
	}

	public void SetName()
	{
		if (!(myNetWorkName == null) && !(myNetWorkName.GetComponent<UILabel>() == null))
		{
			if (SettingsManager.UISettings.DisableNameColors.Value)
			{
				ForceWhiteName();
			}
			if (SettingsManager.LegacyGameSettings.GlobalHideNames.Value || SettingsManager.UISettings.HideNames.Value)
			{
				HideName();
			}
		}
	}

	public void HideName()
	{
		myNetWorkName.GetComponent<UILabel>().text = string.Empty;
	}

	public void ForceWhiteName()
	{
		UILabel component = myNetWorkName.GetComponent<UILabel>();
		component.text = component.text.StripHex();
	}

	public void SetInterpolationIfEnabled(bool interpolate)
	{
		if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE || base.photonView.isMine)
		{
			if (interpolate && SettingsManager.GraphicsSettings.InterpolationEnabled.Value)
			{
				base.rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
			}
			else
			{
				base.rigidbody.interpolation = RigidbodyInterpolation.None;
			}
		}
	}

	public IEnumerator stopImmunity()
	{
		yield return new WaitForSeconds(5f);
		bombImmune = false;
	}

	private void suicide()
	{
	}

	private void suicide2()
	{
		if (IN_GAME_MAIN_CAMERA.gametype != 0)
		{
			netDieLocal(base.rigidbody.velocity * 50f, false, -1, string.Empty);
			FengGameManagerMKII.instance.needChooseSide = true;
			FengGameManagerMKII.instance.justSuicide = true;
		}
	}

	private void throwBlades()
	{
		Transform transform = setup.part_blade_l.transform;
		Transform transform2 = setup.part_blade_r.transform;
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Character_parts/character_blade_l"), transform.position, transform.rotation);
		GameObject gameObject2 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Character_parts/character_blade_r"), transform2.position, transform2.rotation);
		gameObject.renderer.material = CharacterMaterials.materials[setup.myCostume._3dmg_texture];
		gameObject2.renderer.material = CharacterMaterials.materials[setup.myCostume._3dmg_texture];
		Vector3 force = base.transform.forward + base.transform.up * 2f - base.transform.right;
		gameObject.rigidbody.AddForce(force, ForceMode.Impulse);
		Vector3 force2 = base.transform.forward + base.transform.up * 2f + base.transform.right;
		gameObject2.rigidbody.AddForce(force2, ForceMode.Impulse);
		Vector3 torque = new Vector3(UnityEngine.Random.Range(-100, 100), UnityEngine.Random.Range(-100, 100), UnityEngine.Random.Range(-100, 100));
		torque.Normalize();
		gameObject.rigidbody.AddTorque(torque);
		torque = new Vector3(UnityEngine.Random.Range(-100, 100), UnityEngine.Random.Range(-100, 100), UnityEngine.Random.Range(-100, 100));
		torque.Normalize();
		gameObject2.rigidbody.AddTorque(torque);
		setup.part_blade_l.SetActive(false);
		setup.part_blade_r.SetActive(false);
		currentBladeNum--;
		if (currentBladeNum == 0)
		{
			currentBladeSta = 0f;
		}
		if (state == HERO_STATE.Attack)
		{
			falseAttack();
		}
	}

	public void ungrabbed()
	{
		facingDirection = 0f;
		targetRotation = Quaternion.Euler(0f, 0f, 0f);
		base.transform.parent = null;
		GetComponent<CapsuleCollider>().isTrigger = false;
		state = HERO_STATE.Idle;
	}

	private void unmounted()
	{
		myHorse.GetComponent<Horse>().unmounted();
		isMounted = false;
	}

	public void update2()
	{
		if (GameMenu.Paused && IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
		{
			return;
		}
		if (invincible > 0f)
		{
			invincible -= Time.deltaTime;
		}
		if (hasDied)
		{
			return;
		}
		if (titanForm && eren_titan != null)
		{
			baseTransform.position = eren_titan.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck").position;
			base.gameObject.GetComponent<SmoothSyncMovement>().disabled = true;
		}
		else if (isCannon && myCannon != null)
		{
			updateCannon();
			base.gameObject.GetComponent<SmoothSyncMovement>().disabled = true;
		}
		if (IN_GAME_MAIN_CAMERA.gametype != 0 && !base.photonView.isMine)
		{
			return;
		}
		UpdateInput();
		_dashCooldownLeft -= Time.deltaTime;
		if (_dashCooldownLeft < 0f)
		{
			_dashCooldownLeft = 0f;
		}
		if (myCannonRegion != null)
		{
			FengGameManagerMKII.instance.ShowHUDInfoCenter(string.Format("Press {0} to use Cannon.", SettingsManager.InputSettings.Interaction.Interact.ToString()));
			if (SettingsManager.InputSettings.Interaction.Interact.GetKeyDown())
			{
				myCannonRegion.photonView.RPC("RequestControlRPC", PhotonTargets.MasterClient, base.photonView.viewID);
			}
		}
		if (state == HERO_STATE.Grab && !useGun)
		{
			if (skillId == "jean")
			{
				if (state != HERO_STATE.Attack && (SettingsManager.InputSettings.Human.AttackDefault.GetKeyDown() || SettingsManager.InputSettings.Human.AttackSpecial.GetKeyDown()) && escapeTimes > 0 && !baseAnimation.IsPlaying("grabbed_jean"))
				{
					playAnimation("grabbed_jean");
					baseAnimation["grabbed_jean"].time = 0f;
					escapeTimes--;
				}
				if (!baseAnimation.IsPlaying("grabbed_jean") || !(baseAnimation["grabbed_jean"].normalizedTime > 0.64f) || !(titanWhoGrabMe.GetComponent<TITAN>() != null))
				{
					return;
				}
				ungrabbed();
				baseRigidBody.velocity = Vector3.up * 30f;
				if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
				{
					titanWhoGrabMe.GetComponent<TITAN>().grabbedTargetEscape(null);
					return;
				}
				base.photonView.RPC("netSetIsGrabbedFalse", PhotonTargets.All);
				if (PhotonNetwork.isMasterClient)
				{
					titanWhoGrabMe.GetComponent<TITAN>().grabbedTargetEscape(null);
				}
				else
				{
					PhotonView.Find(titanWhoGrabMeID).RPC("grabbedTargetEscape", PhotonTargets.MasterClient);
				}
			}
			else
			{
				if (!(skillId == "eren"))
				{
					return;
				}
				showSkillCD();
				if (IN_GAME_MAIN_CAMERA.gametype != 0 || (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE && !GameMenu.Paused))
				{
					calcSkillCD();
					calcFlareCD();
				}
				if (!SettingsManager.InputSettings.Human.AttackSpecial.GetKeyDown())
				{
					return;
				}
				bool flag = false;
				if (skillCDDuration > 0f || flag)
				{
					flag = true;
					return;
				}
				skillCDDuration = skillCDLast;
				if (!(skillId == "eren") || !(titanWhoGrabMe.GetComponent<TITAN>() != null))
				{
					return;
				}
				ungrabbed();
				if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
				{
					titanWhoGrabMe.GetComponent<TITAN>().grabbedTargetEscape(null);
				}
				else
				{
					base.photonView.RPC("netSetIsGrabbedFalse", PhotonTargets.All);
					if (PhotonNetwork.isMasterClient)
					{
						titanWhoGrabMe.GetComponent<TITAN>().grabbedTargetEscape(null);
					}
					else
					{
						PhotonView.Find(titanWhoGrabMeID).photonView.RPC("grabbedTargetEscape", PhotonTargets.MasterClient);
					}
				}
				erenTransform();
			}
		}
		else if (!titanForm && !isCannon)
		{
			bufferUpdate();
			UpdateThunderSpear();
			if (!GameMenu.InMenu())
			{
				if (!grounded && state != HERO_STATE.AirDodge && (!isMounted || !(myHorse != null)))
				{
					checkDashRebind();
					if (SettingsManager.InputSettings.Human.DashDoubleTap.Value)
					{
						checkDashDoubleTap();
					}
					if (dashD)
					{
						dashD = false;
						dash(0f, -1f);
						return;
					}
					if (dashU)
					{
						dashU = false;
						dash(0f, 1f);
						return;
					}
					if (dashL)
					{
						dashL = false;
						dash(-1f, 0f);
						return;
					}
					if (dashR)
					{
						dashR = false;
						dash(1f, 0f);
						return;
					}
				}
				if (grounded && (state == HERO_STATE.Idle || state == HERO_STATE.Slide))
				{
					if (SettingsManager.InputSettings.Human.Jump.GetKeyDown() && !baseAnimation.IsPlaying("jump") && !baseAnimation.IsPlaying("horse_geton"))
					{
						idle();
						crossFade("jump", 0.1f);
						sparks.enableEmission = false;
					}
					if (SettingsManager.InputSettings.Human.HorseMount.GetKeyDown() && !baseAnimation.IsPlaying("jump") && !baseAnimation.IsPlaying("horse_geton") && myHorse != null && !isMounted && Vector3.Distance(myHorse.transform.position, base.transform.position) < 15f)
					{
						getOnHorse();
					}
					if (SettingsManager.InputSettings.Human.Dodge.GetKeyDown() && !baseAnimation.IsPlaying("jump") && !baseAnimation.IsPlaying("horse_geton"))
					{
						dodge2();
						return;
					}
				}
			}
			if (state == HERO_STATE.Idle && !GameMenu.InMenu())
			{
				_flareDelayAfterEmote -= Time.deltaTime;
				if (_flareDelayAfterEmote <= 0f)
				{
					if (SettingsManager.InputSettings.Human.Flare1.GetKeyDown())
					{
						shootFlare(1);
					}
					if (SettingsManager.InputSettings.Human.Flare2.GetKeyDown())
					{
						shootFlare(2);
					}
					if (SettingsManager.InputSettings.Human.Flare3.GetKeyDown())
					{
						shootFlare(3);
					}
				}
				if (SettingsManager.InputSettings.General.ChangeCharacter.GetKeyDown())
				{
					suicide2();
				}
				if (myHorse != null && isMounted && SettingsManager.InputSettings.Human.HorseMount.GetKeyDown())
				{
					getOffHorse();
				}
				if ((base.animation.IsPlaying(standAnimation) || !grounded) && SettingsManager.InputSettings.Human.Reload.GetKeyDown() && (!useGun || SettingsManager.LegacyGameSettings.AHSSAirReload.Value || grounded))
				{
					changeBlade();
					return;
				}
				if (!isMounted && (SettingsManager.InputSettings.Human.AttackDefault.GetKeyDown() || SettingsManager.InputSettings.Human.AttackSpecial.GetKeyDown()) && !useGun)
				{
					bool flag2 = false;
					if (SettingsManager.InputSettings.Human.AttackSpecial.GetKeyDown())
					{
						if (skillCDDuration > 0f || flag2)
						{
							flag2 = true;
						}
						else
						{
							skillCDDuration = skillCDLast;
							if (skillId == "eren")
							{
								erenTransform();
								return;
							}
							if (skillId == "marco")
							{
								if (IsGrounded())
								{
									attackAnimation = ((UnityEngine.Random.Range(0, 2) != 0) ? "special_marco_1" : "special_marco_0");
									playAnimation(attackAnimation);
								}
								else
								{
									flag2 = true;
									skillCDDuration = 0f;
								}
							}
							else if (skillId == "armin")
							{
								if (IsGrounded())
								{
									attackAnimation = "special_armin";
									playAnimation("special_armin");
								}
								else
								{
									flag2 = true;
									skillCDDuration = 0f;
								}
							}
							else if (skillId == "sasha")
							{
								if (IsGrounded())
								{
									attackAnimation = "special_sasha";
									playAnimation("special_sasha");
									currentBuff = BUFF.SpeedUp;
									buffTime = 10f;
								}
								else
								{
									flag2 = true;
									skillCDDuration = 0f;
								}
							}
							else if (skillId == "mikasa")
							{
								attackAnimation = "attack3_1";
								playAnimation("attack3_1");
								baseRigidBody.velocity = Vector3.up * 10f;
							}
							else if (skillId == "levi")
							{
								attackAnimation = "attack5";
								playAnimation("attack5");
								baseRigidBody.velocity += Vector3.up * 5f;
								Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
								LayerMask layerMask = 1 << LayerMask.NameToLayer("Ground");
								LayerMask layerMask2 = 1 << LayerMask.NameToLayer("EnemyBox");
								RaycastHit hitInfo;
								if (Physics.Raycast(ray, out hitInfo, 10000000f, ((LayerMask)((int)layerMask2 | (int)layerMask)).value))
								{
									if (bulletRight != null)
									{
										bulletRight.GetComponent<Bullet>().disable();
										releaseIfIHookSb();
									}
									dashDirection = hitInfo.point - baseTransform.position;
									launchRightRope(hitInfo, true, 1);
									rope.Play();
								}
								facingDirection = Mathf.Atan2(dashDirection.x, dashDirection.z) * 57.29578f;
								targetRotation = Quaternion.Euler(0f, facingDirection, 0f);
								attackLoop = 3;
							}
							else if (skillId == "petra")
							{
								attackAnimation = "special_petra";
								playAnimation("special_petra");
								baseRigidBody.velocity += Vector3.up * 5f;
								Ray ray2 = Camera.main.ScreenPointToRay(Input.mousePosition);
								LayerMask layerMask3 = 1 << LayerMask.NameToLayer("Ground");
								LayerMask layerMask4 = 1 << LayerMask.NameToLayer("EnemyBox");
								RaycastHit hitInfo2;
								if (Physics.Raycast(ray2, out hitInfo2, 10000000f, ((LayerMask)((int)layerMask4 | (int)layerMask3)).value))
								{
									if (bulletRight != null)
									{
										bulletRight.GetComponent<Bullet>().disable();
										releaseIfIHookSb();
									}
									if (bulletLeft != null)
									{
										bulletLeft.GetComponent<Bullet>().disable();
										releaseIfIHookSb();
									}
									dashDirection = hitInfo2.point - baseTransform.position;
									launchLeftRope(hitInfo2, true);
									launchRightRope(hitInfo2, true);
									rope.Play();
								}
								facingDirection = Mathf.Atan2(dashDirection.x, dashDirection.z) * 57.29578f;
								targetRotation = Quaternion.Euler(0f, facingDirection, 0f);
								attackLoop = 3;
							}
							else
							{
								if (needLean)
								{
									if (leanLeft)
									{
										attackAnimation = ((UnityEngine.Random.Range(0, 100) >= 50) ? "attack1_hook_l1" : "attack1_hook_l2");
									}
									else
									{
										attackAnimation = ((UnityEngine.Random.Range(0, 100) >= 50) ? "attack1_hook_r1" : "attack1_hook_r2");
									}
								}
								else
								{
									attackAnimation = "attack1";
								}
								playAnimation(attackAnimation);
							}
						}
					}
					else if (SettingsManager.InputSettings.Human.AttackDefault.GetKeyDown())
					{
						if (needLean)
						{
							if (SettingsManager.InputSettings.General.Left.GetKey())
							{
								attackAnimation = ((UnityEngine.Random.Range(0, 100) >= 50) ? "attack1_hook_l1" : "attack1_hook_l2");
							}
							else if (SettingsManager.InputSettings.General.Right.GetKey())
							{
								attackAnimation = ((UnityEngine.Random.Range(0, 100) >= 50) ? "attack1_hook_r1" : "attack1_hook_r2");
							}
							else if (leanLeft)
							{
								attackAnimation = ((UnityEngine.Random.Range(0, 100) >= 50) ? "attack1_hook_l1" : "attack1_hook_l2");
							}
							else
							{
								attackAnimation = ((UnityEngine.Random.Range(0, 100) >= 50) ? "attack1_hook_r1" : "attack1_hook_r2");
							}
						}
						else if (SettingsManager.InputSettings.General.Left.GetKey())
						{
							attackAnimation = "attack2";
						}
						else if (SettingsManager.InputSettings.General.Right.GetKey())
						{
							attackAnimation = "attack1";
						}
						else if (lastHook != null)
						{
							if (lastHook.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck") != null)
							{
								attackAccordingToTarget(lastHook.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck"));
							}
							else
							{
								flag2 = true;
							}
						}
						else if (bulletLeft != null && bulletLeft.transform.parent != null)
						{
							Transform transform = bulletLeft.transform.parent.transform.root.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck");
							if (transform != null)
							{
								attackAccordingToTarget(transform);
							}
							else
							{
								attackAccordingToMouse();
							}
						}
						else if (bulletRight != null && bulletRight.transform.parent != null)
						{
							Transform transform2 = bulletRight.transform.parent.transform.root.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck");
							if (transform2 != null)
							{
								attackAccordingToTarget(transform2);
							}
							else
							{
								attackAccordingToMouse();
							}
						}
						else
						{
							GameObject gameObject = findNearestTitan();
							if (gameObject != null)
							{
								Transform transform3 = gameObject.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck");
								if (transform3 != null)
								{
									attackAccordingToTarget(transform3);
								}
								else
								{
									attackAccordingToMouse();
								}
							}
							else
							{
								attackAccordingToMouse();
							}
						}
					}
					if (!flag2)
					{
						checkBoxLeft.GetComponent<TriggerColliderWeapon>().clearHits();
						checkBoxRight.GetComponent<TriggerColliderWeapon>().clearHits();
						if (grounded)
						{
							baseRigidBody.AddForce(base.gameObject.transform.forward * 200f);
						}
						playAnimation(attackAnimation);
						baseAnimation[attackAnimation].time = 0f;
						buttonAttackRelease = false;
						state = HERO_STATE.Attack;
						if (grounded || attackAnimation == "attack3_1" || attackAnimation == "attack5" || attackAnimation == "special_petra")
						{
							attackReleased = true;
							buttonAttackRelease = true;
						}
						else
						{
							attackReleased = false;
						}
						sparks.enableEmission = false;
					}
				}
				if (useGun)
				{
					if (SettingsManager.InputSettings.Human.AttackSpecial.GetKey())
					{
						leftArmAim = true;
						rightArmAim = true;
					}
					else if (SettingsManager.InputSettings.Human.AttackDefault.GetKey())
					{
						if (leftGunHasBullet)
						{
							leftArmAim = true;
							rightArmAim = false;
						}
						else
						{
							leftArmAim = false;
							if (rightGunHasBullet)
							{
								rightArmAim = true;
							}
							else
							{
								rightArmAim = false;
							}
						}
					}
					else
					{
						leftArmAim = false;
						rightArmAim = false;
					}
					if (leftArmAim || rightArmAim)
					{
						Ray ray3 = Camera.main.ScreenPointToRay(Input.mousePosition);
						LayerMask layerMask5 = 1 << LayerMask.NameToLayer("Ground");
						LayerMask layerMask6 = 1 << LayerMask.NameToLayer("EnemyBox");
						RaycastHit hitInfo3;
						if (Physics.Raycast(ray3, out hitInfo3, 10000000f, ((LayerMask)((int)layerMask6 | (int)layerMask5)).value))
						{
							gunTarget = hitInfo3.point;
						}
					}
					bool flag3 = false;
					bool flag4 = false;
					bool flag5 = false;
					if (SettingsManager.InputSettings.Human.AttackSpecial.GetKeyUp() && skillId != "bomb")
					{
						if (leftGunHasBullet && rightGunHasBullet)
						{
							if (grounded)
							{
								attackAnimation = "AHSS_shoot_both";
							}
							else
							{
								attackAnimation = "AHSS_shoot_both_air";
							}
							flag3 = true;
						}
						else if (!leftGunHasBullet && !rightGunHasBullet)
						{
							flag4 = true;
						}
						else
						{
							flag5 = true;
						}
					}
					if (flag5 || SettingsManager.InputSettings.Human.AttackDefault.GetKeyUp())
					{
						if (grounded)
						{
							if (leftGunHasBullet && rightGunHasBullet)
							{
								if (isLeftHandHooked)
								{
									attackAnimation = "AHSS_shoot_r";
								}
								else
								{
									attackAnimation = "AHSS_shoot_l";
								}
							}
							else if (leftGunHasBullet)
							{
								attackAnimation = "AHSS_shoot_l";
							}
							else if (rightGunHasBullet)
							{
								attackAnimation = "AHSS_shoot_r";
							}
						}
						else if (leftGunHasBullet && rightGunHasBullet)
						{
							if (isLeftHandHooked)
							{
								attackAnimation = "AHSS_shoot_r_air";
							}
							else
							{
								attackAnimation = "AHSS_shoot_l_air";
							}
						}
						else if (leftGunHasBullet)
						{
							attackAnimation = "AHSS_shoot_l_air";
						}
						else if (rightGunHasBullet)
						{
							attackAnimation = "AHSS_shoot_r_air";
						}
						if (leftGunHasBullet || rightGunHasBullet)
						{
							flag3 = true;
						}
						else
						{
							flag4 = true;
						}
					}
					if (flag3)
					{
						state = HERO_STATE.Attack;
						crossFade(attackAnimation, 0.05f);
						gunDummy.transform.position = baseTransform.position;
						gunDummy.transform.rotation = baseTransform.rotation;
						gunDummy.transform.LookAt(gunTarget);
						attackReleased = false;
						facingDirection = gunDummy.transform.rotation.eulerAngles.y;
						targetRotation = Quaternion.Euler(0f, facingDirection, 0f);
					}
					else if (flag4 && (grounded || (LevelInfo.getInfo(FengGameManagerMKII.level).type != GAMEMODE.PVP_AHSS && SettingsManager.LegacyGameSettings.AHSSAirReload.Value)))
					{
						changeBlade();
					}
				}
			}
			else if (state == HERO_STATE.Attack)
			{
				if (!useGun)
				{
					if (!SettingsManager.InputSettings.Human.AttackDefault.GetKey())
					{
						buttonAttackRelease = true;
					}
					if (!attackReleased)
					{
						if (buttonAttackRelease)
						{
							continueAnimation();
							attackReleased = true;
						}
						else if (baseAnimation[attackAnimation].normalizedTime >= 0.32f)
						{
							pauseAnimation();
						}
					}
					if (attackAnimation == "attack3_1" && currentBladeSta > 0f)
					{
						if (baseAnimation[attackAnimation].normalizedTime >= 0.8f)
						{
							if (!checkBoxLeft.GetComponent<TriggerColliderWeapon>().active_me)
							{
								checkBoxLeft.GetComponent<TriggerColliderWeapon>().active_me = true;
								if (SettingsManager.GraphicsSettings.WeaponTrailEnabled.Value)
								{
									leftbladetrail2.Activate();
									rightbladetrail2.Activate();
									leftbladetrail.Activate();
									rightbladetrail.Activate();
								}
								baseRigidBody.velocity = -Vector3.up * 30f;
							}
							if (!checkBoxRight.GetComponent<TriggerColliderWeapon>().active_me)
							{
								checkBoxRight.GetComponent<TriggerColliderWeapon>().active_me = true;
								slash.Play();
							}
						}
						else if (checkBoxLeft.GetComponent<TriggerColliderWeapon>().active_me)
						{
							checkBoxLeft.GetComponent<TriggerColliderWeapon>().active_me = false;
							checkBoxRight.GetComponent<TriggerColliderWeapon>().active_me = false;
							checkBoxLeft.GetComponent<TriggerColliderWeapon>().clearHits();
							checkBoxRight.GetComponent<TriggerColliderWeapon>().clearHits();
							leftbladetrail.StopSmoothly(0.1f);
							rightbladetrail.StopSmoothly(0.1f);
							leftbladetrail2.StopSmoothly(0.1f);
							rightbladetrail2.StopSmoothly(0.1f);
						}
					}
					else
					{
						float num2;
						float num;
						if (currentBladeSta == 0f)
						{
							num2 = (num = -1f);
						}
						else if (attackAnimation == "attack5")
						{
							num2 = 0.35f;
							num = 0.5f;
						}
						else if (attackAnimation == "special_petra")
						{
							num2 = 0.35f;
							num = 0.48f;
						}
						else if (attackAnimation == "special_armin")
						{
							num2 = 0.25f;
							num = 0.35f;
						}
						else if (attackAnimation == "attack4")
						{
							num2 = 0.6f;
							num = 0.9f;
						}
						else if (attackAnimation == "special_sasha")
						{
							num2 = (num = -1f);
						}
						else
						{
							num2 = 0.5f;
							num = 0.85f;
						}
						if (baseAnimation[attackAnimation].normalizedTime > num2 && baseAnimation[attackAnimation].normalizedTime < num)
						{
							if (!checkBoxLeft.GetComponent<TriggerColliderWeapon>().active_me)
							{
								checkBoxLeft.GetComponent<TriggerColliderWeapon>().active_me = true;
								slash.Play();
								if (SettingsManager.GraphicsSettings.WeaponTrailEnabled.Value)
								{
									leftbladetrail2.Activate();
									rightbladetrail2.Activate();
									leftbladetrail.Activate();
									rightbladetrail.Activate();
								}
							}
							if (!checkBoxRight.GetComponent<TriggerColliderWeapon>().active_me)
							{
								checkBoxRight.GetComponent<TriggerColliderWeapon>().active_me = true;
							}
						}
						else if (checkBoxLeft.GetComponent<TriggerColliderWeapon>().active_me)
						{
							checkBoxLeft.GetComponent<TriggerColliderWeapon>().active_me = false;
							checkBoxRight.GetComponent<TriggerColliderWeapon>().active_me = false;
							checkBoxLeft.GetComponent<TriggerColliderWeapon>().clearHits();
							checkBoxRight.GetComponent<TriggerColliderWeapon>().clearHits();
							leftbladetrail2.StopSmoothly(0.1f);
							rightbladetrail2.StopSmoothly(0.1f);
							leftbladetrail.StopSmoothly(0.1f);
							rightbladetrail.StopSmoothly(0.1f);
						}
						if (attackLoop > 0 && baseAnimation[attackAnimation].normalizedTime > num)
						{
							attackLoop--;
							playAnimationAt(attackAnimation, num2);
						}
					}
					if (baseAnimation[attackAnimation].normalizedTime >= 1f)
					{
						if (attackAnimation == "special_marco_0" || attackAnimation == "special_marco_1")
						{
							if (IN_GAME_MAIN_CAMERA.gametype != 0)
							{
								if (!PhotonNetwork.isMasterClient)
								{
									object[] parameters = new object[2] { 5f, 100f };
									base.photonView.RPC("netTauntAttack", PhotonTargets.MasterClient, parameters);
								}
								else
								{
									netTauntAttack(5f, 100f, null);
								}
							}
							else
							{
								netTauntAttack(5f, 100f, null);
							}
							falseAttack();
							idle();
						}
						else if (attackAnimation == "special_armin")
						{
							if (IN_GAME_MAIN_CAMERA.gametype != 0)
							{
								if (!PhotonNetwork.isMasterClient)
								{
									base.photonView.RPC("netlaughAttack", PhotonTargets.MasterClient);
								}
								else
								{
									netlaughAttack(null);
								}
							}
							else
							{
								GameObject[] array = GameObject.FindGameObjectsWithTag("titan");
								foreach (GameObject gameObject2 in array)
								{
									if (Vector3.Distance(gameObject2.transform.position, baseTransform.position) < 50f && Vector3.Angle(gameObject2.transform.forward, baseTransform.position - gameObject2.transform.position) < 90f && gameObject2.GetComponent<TITAN>() != null)
									{
										gameObject2.GetComponent<TITAN>().beLaughAttacked();
									}
								}
							}
							falseAttack();
							idle();
						}
						else if (attackAnimation == "attack3_1")
						{
							baseRigidBody.velocity -= Vector3.up * Time.deltaTime * 30f;
						}
						else
						{
							falseAttack();
							idle();
						}
					}
					if (baseAnimation.IsPlaying("attack3_2") && baseAnimation["attack3_2"].normalizedTime >= 1f)
					{
						falseAttack();
						idle();
					}
				}
				else
				{
					baseTransform.rotation = Quaternion.Lerp(baseTransform.rotation, gunDummy.transform.rotation, Time.deltaTime * 30f);
					if (!attackReleased && baseAnimation[attackAnimation].normalizedTime > 0.167f)
					{
						attackReleased = true;
						bool flag6 = false;
						if (attackAnimation == "AHSS_shoot_both" || attackAnimation == "AHSS_shoot_both_air")
						{
							flag6 = true;
							leftGunHasBullet = false;
							rightGunHasBullet = false;
							baseRigidBody.AddForce(-baseTransform.forward * 1000f, ForceMode.Acceleration);
						}
						else
						{
							if (attackAnimation == "AHSS_shoot_l" || attackAnimation == "AHSS_shoot_l_air")
							{
								leftGunHasBullet = false;
							}
							else
							{
								rightGunHasBullet = false;
							}
							baseRigidBody.AddForce(-baseTransform.forward * 600f, ForceMode.Acceleration);
						}
						baseRigidBody.AddForce(Vector3.up * 200f, ForceMode.Acceleration);
						string text = "FX/shotGun";
						if (flag6)
						{
							text = "FX/shotGun 1";
						}
						if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && base.photonView.isMine)
						{
							GameObject gameObject3 = PhotonNetwork.Instantiate(text, baseTransform.position + baseTransform.up * 0.8f - baseTransform.right * 0.1f, baseTransform.rotation, 0);
							if (gameObject3.GetComponent<EnemyfxIDcontainer>() != null)
							{
								gameObject3.GetComponent<EnemyfxIDcontainer>().myOwnerViewID = base.photonView.viewID;
							}
						}
						else
						{
							GameObject gameObject3 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load(text), baseTransform.position + baseTransform.up * 0.8f - baseTransform.right * 0.1f, baseTransform.rotation);
						}
					}
					if (baseAnimation[attackAnimation].normalizedTime >= 1f)
					{
						falseAttack();
						idle();
					}
					if (!baseAnimation.IsPlaying(attackAnimation))
					{
						falseAttack();
						idle();
					}
				}
			}
			else if (state == HERO_STATE.ChangeBlade)
			{
				if (useGun)
				{
					if (baseAnimation[reloadAnimation].normalizedTime > 0.22f)
					{
						if (!leftGunHasBullet && setup.part_blade_l.activeSelf)
						{
							setup.part_blade_l.SetActive(false);
							Transform transform4 = setup.part_blade_l.transform;
							GameObject gameObject4 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Character_parts/character_gun_l"), transform4.position, transform4.rotation);
							gameObject4.renderer.material = CharacterMaterials.materials[setup.myCostume._3dmg_texture];
							Vector3 force = -baseTransform.forward * 10f + baseTransform.up * 5f - baseTransform.right;
							gameObject4.rigidbody.AddForce(force, ForceMode.Impulse);
							Vector3 torque = new Vector3(UnityEngine.Random.Range(-100, 100), UnityEngine.Random.Range(-100, 100), UnityEngine.Random.Range(-100, 100));
							gameObject4.rigidbody.AddTorque(torque, ForceMode.Acceleration);
						}
						if (!rightGunHasBullet && setup.part_blade_r.activeSelf)
						{
							setup.part_blade_r.SetActive(false);
							Transform transform5 = setup.part_blade_r.transform;
							GameObject gameObject5 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Character_parts/character_gun_r"), transform5.position, transform5.rotation);
							gameObject5.renderer.material = CharacterMaterials.materials[setup.myCostume._3dmg_texture];
							Vector3 force2 = -baseTransform.forward * 10f + baseTransform.up * 5f + baseTransform.right;
							gameObject5.rigidbody.AddForce(force2, ForceMode.Impulse);
							Vector3 torque2 = new Vector3(UnityEngine.Random.Range(-300, 300), UnityEngine.Random.Range(-300, 300), UnityEngine.Random.Range(-300, 300));
							gameObject5.rigidbody.AddTorque(torque2, ForceMode.Acceleration);
						}
					}
					if (baseAnimation[reloadAnimation].normalizedTime > 0.62f && !throwedBlades)
					{
						throwedBlades = true;
						if (leftBulletLeft > 0 && !leftGunHasBullet)
						{
							leftBulletLeft--;
							setup.part_blade_l.SetActive(true);
							leftGunHasBullet = true;
						}
						if (rightBulletLeft > 0 && !rightGunHasBullet)
						{
							setup.part_blade_r.SetActive(true);
							rightBulletLeft--;
							rightGunHasBullet = true;
						}
						updateRightMagUI();
						updateLeftMagUI();
					}
					if (baseAnimation[reloadAnimation].normalizedTime > 1f)
					{
						idle();
					}
				}
				else
				{
					if (!grounded)
					{
						if (base.animation[reloadAnimation].normalizedTime >= 0.2f && !throwedBlades)
						{
							throwedBlades = true;
							if (setup.part_blade_l.activeSelf)
							{
								throwBlades();
							}
						}
						if (base.animation[reloadAnimation].normalizedTime >= 0.56f && currentBladeNum > 0)
						{
							setup.part_blade_l.SetActive(true);
							setup.part_blade_r.SetActive(true);
							currentBladeSta = totalBladeSta;
						}
					}
					else
					{
						if (baseAnimation[reloadAnimation].normalizedTime >= 0.13f && !throwedBlades)
						{
							throwedBlades = true;
							if (setup.part_blade_l.activeSelf)
							{
								throwBlades();
							}
						}
						if (baseAnimation[reloadAnimation].normalizedTime >= 0.37f && currentBladeNum > 0)
						{
							setup.part_blade_l.SetActive(true);
							setup.part_blade_r.SetActive(true);
							currentBladeSta = totalBladeSta;
						}
					}
					if (baseAnimation[reloadAnimation].normalizedTime >= 1f)
					{
						idle();
					}
				}
			}
			else if (state == HERO_STATE.Salute)
			{
				_currentEmoteActionTime -= Time.deltaTime;
				if (_currentEmoteActionTime <= 0f)
				{
					idle();
				}
			}
			else if (state == HERO_STATE.GroundDodge)
			{
				if (baseAnimation.IsPlaying("dodge"))
				{
					if (!grounded && !(baseAnimation["dodge"].normalizedTime <= 0.6f))
					{
						idle();
					}
					if (baseAnimation["dodge"].normalizedTime >= 1f)
					{
						idle();
					}
				}
			}
			else if (state == HERO_STATE.Land)
			{
				if (baseAnimation.IsPlaying("dash_land") && baseAnimation["dash_land"].normalizedTime >= 1f)
				{
					idle();
				}
			}
			else if (state == HERO_STATE.FillGas)
			{
				if (baseAnimation.IsPlaying("supply") && baseAnimation["supply"].normalizedTime >= 1f)
				{
					if (skillId != "bomb")
					{
						currentBladeSta = totalBladeSta;
						currentBladeNum = totalBladeNum;
						if (!useGun)
						{
							setup.part_blade_l.SetActive(true);
							setup.part_blade_r.SetActive(true);
						}
						else
						{
							leftBulletLeft = (rightBulletLeft = bulletMAX);
							leftGunHasBullet = (rightGunHasBullet = true);
							setup.part_blade_l.SetActive(true);
							setup.part_blade_r.SetActive(true);
							updateRightMagUI();
							updateLeftMagUI();
						}
					}
					currentGas = totalGas;
					idle();
				}
			}
			else if (state == HERO_STATE.Slide)
			{
				if (!grounded)
				{
					idle();
				}
			}
			else if (state == HERO_STATE.AirDodge)
			{
				if (dashTime > 0f)
				{
					dashTime -= Time.deltaTime;
					if (currentSpeed > originVM)
					{
						baseRigidBody.AddForce(-baseRigidBody.velocity * Time.deltaTime * 1.7f, ForceMode.VelocityChange);
					}
				}
				else
				{
					dashTime = 0f;
					idle();
				}
			}
			if (!GameMenu.InMenu())
			{
				if (SettingsManager.InputSettings.Human.HookLeft.GetKey() && ((!baseAnimation.IsPlaying("attack3_1") && !baseAnimation.IsPlaying("attack5") && !baseAnimation.IsPlaying("special_petra") && state != HERO_STATE.Grab) || state == HERO_STATE.Idle))
				{
					if (bulletLeft != null)
					{
						QHold = true;
					}
					else
					{
						Ray ray4 = Camera.main.ScreenPointToRay(Input.mousePosition);
						LayerMask layerMask7 = 1 << LayerMask.NameToLayer("Ground");
						LayerMask layerMask8 = 1 << LayerMask.NameToLayer("EnemyBox");
						RaycastHit hitInfo4;
						if (Physics.Raycast(ray4, out hitInfo4, 10000f, ((LayerMask)((int)layerMask8 | (int)layerMask7)).value))
						{
							launchLeftRope(hitInfo4, true);
							rope.Play();
						}
					}
				}
				else
				{
					QHold = false;
				}
				if (SettingsManager.InputSettings.Human.HookRight.GetKey() && ((!baseAnimation.IsPlaying("attack3_1") && !baseAnimation.IsPlaying("attack5") && !baseAnimation.IsPlaying("special_petra") && state != HERO_STATE.Grab) || state == HERO_STATE.Idle))
				{
					if (bulletRight != null)
					{
						EHold = true;
					}
					else
					{
						Ray ray5 = Camera.main.ScreenPointToRay(Input.mousePosition);
						LayerMask layerMask9 = 1 << LayerMask.NameToLayer("Ground");
						LayerMask layerMask10 = 1 << LayerMask.NameToLayer("EnemyBox");
						RaycastHit hitInfo5;
						if (Physics.Raycast(ray5, out hitInfo5, 10000f, ((LayerMask)((int)layerMask10 | (int)layerMask9)).value))
						{
							launchRightRope(hitInfo5, true);
							rope.Play();
						}
					}
				}
				else
				{
					EHold = false;
				}
				if (SettingsManager.InputSettings.Human.HookBoth.GetKey() && ((!baseAnimation.IsPlaying("attack3_1") && !baseAnimation.IsPlaying("attack5") && !baseAnimation.IsPlaying("special_petra") && state != HERO_STATE.Grab) || state == HERO_STATE.Idle))
				{
					QHold = true;
					EHold = true;
					if (bulletLeft == null && bulletRight == null)
					{
						Ray ray6 = Camera.main.ScreenPointToRay(Input.mousePosition);
						LayerMask layerMask11 = 1 << LayerMask.NameToLayer("Ground");
						LayerMask layerMask12 = 1 << LayerMask.NameToLayer("EnemyBox");
						RaycastHit hitInfo6;
						if (Physics.Raycast(ray6, out hitInfo6, 1000000f, ((LayerMask)((int)layerMask12 | (int)layerMask11)).value))
						{
							launchLeftRope(hitInfo6, false);
							launchRightRope(hitInfo6, false);
							rope.Play();
						}
					}
				}
			}
			if (IN_GAME_MAIN_CAMERA.gametype != 0 || (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE && !GameMenu.Paused))
			{
				calcSkillCD();
				calcFlareCD();
			}
			if (!useGun)
			{
				if (leftbladetrail.gameObject.GetActive())
				{
					leftbladetrail.update();
					rightbladetrail.update();
				}
				if (leftbladetrail2.gameObject.GetActive())
				{
					leftbladetrail2.update();
					rightbladetrail2.update();
				}
				if (leftbladetrail.gameObject.GetActive())
				{
					leftbladetrail.lateUpdate();
					rightbladetrail.lateUpdate();
				}
				if (leftbladetrail2.gameObject.GetActive())
				{
					leftbladetrail2.lateUpdate();
					rightbladetrail2.lateUpdate();
				}
			}
			if (!GameMenu.Paused)
			{
				showSkillCD();
				showFlareCD2();
				showGas2();
				showAimUI2();
			}
		}
		else if (isCannon && !GameMenu.Paused)
		{
			showAimUI2();
			calcSkillCD();
			showSkillCD();
		}
	}

	public void updateCannon()
	{
		baseTransform.position = myCannonPlayer.position;
		baseTransform.rotation = myCannonBase.rotation;
	}

	private void LaunchThunderSpear()
	{
		if (myBomb != null && !myBomb.Disabled)
		{
			myBomb.Explode(bombRadius);
		}
		detonate = false;
		bombTime = 0f;
		skillCDDuration = bombCD;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		LayerMask layerMask = (1 << LayerMask.NameToLayer("Ground")) | (1 << LayerMask.NameToLayer("EnemyBox"));
		Vector3 vector = baseTransform.position + Vector3.forward * 1000f;
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, 1000000f, layerMask.value))
		{
			vector = hitInfo.point;
		}
		Vector3 vector2 = Vector3.Normalize(vector - baseTransform.position);
		float y = Vector3.Cross(baseTransform.forward, vector2).y;
		Vector3 position;
		if (y < 0f && state != HERO_STATE.Land)
		{
			position = ThunderSpearL.transform.position;
			ThunderSpearL.audio.Play();
			SetThunderSpears(false, true);
			attackAnimation = "AHSS_shoot_l";
		}
		else
		{
			position = ThunderSpearR.transform.position;
			ThunderSpearR.audio.Play();
			SetThunderSpears(true, false);
			attackAnimation = "AHSS_shoot_r";
		}
		Vector3 vector3 = Vector3.Normalize(vector - position);
		if (grounded)
		{
			position += vector3 * 1f;
		}
		if (state != HERO_STATE.Slide)
		{
			if (state == HERO_STATE.Attack)
			{
				buttonAttackRelease = true;
			}
			playAnimationAt(attackAnimation, 0.1f);
			state = HERO_STATE.Attack;
			facingDirection = Quaternion.LookRotation(vector2).eulerAngles.y;
			targetRotation = Quaternion.Euler(0f, facingDirection, 0f);
		}
		GameObject gameObject = PhotonNetwork.Instantiate("RCAsset/BombMain", position, Quaternion.LookRotation(vector3), 0);
		gameObject.rigidbody.velocity = vector3 * bombSpeed;
		myBomb = gameObject.GetComponent<Bomb>();
		myBomb.Setup(this, bombRadius);
	}

	public void UpdateThunderSpear()
	{
		if (!(skillId == "bomb"))
		{
			return;
		}
		leftArmAim = false;
		rightArmAim = false;
		bool keyDown = SettingsManager.InputSettings.Human.AttackSpecial.GetKeyDown();
		bool keyUp = SettingsManager.InputSettings.Human.AttackSpecial.GetKeyUp();
		if (skillCDDuration <= 0f && (!ThunderSpearLModel.activeSelf || !ThunderSpearRModel.activeSelf))
		{
			SetThunderSpears(true, true);
		}
		if (keyDown && skillCDDuration <= 0f)
		{
			LaunchThunderSpear();
		}
		else if (myBomb != null && !myBomb.Disabled)
		{
			bombTime += Time.deltaTime;
			bool flag = false;
			if (keyUp)
			{
				detonate = true;
			}
			else if (keyDown && detonate)
			{
				detonate = false;
				flag = true;
			}
			if (bombTime >= bombTimeMax)
			{
				flag = true;
			}
			if (flag)
			{
				myBomb.Explode(bombRadius);
				detonate = false;
			}
		}
	}

	private bool IsFiringThunderSpear()
	{
		if (skillId == "bomb")
		{
			if (!baseAnimation.IsPlaying("AHSS_shoot_r"))
			{
				return baseAnimation.IsPlaying("AHSS_shoot_l");
			}
			return true;
		}
		return false;
	}

	private void updateLeftMagUI()
	{
		for (int i = 1; i <= bulletMAX; i++)
		{
			GameObject.Find("bulletL" + i).GetComponent<UISprite>().enabled = false;
		}
		for (int j = 1; j <= leftBulletLeft; j++)
		{
			GameObject.Find("bulletL" + j).GetComponent<UISprite>().enabled = true;
		}
	}

	private void updateRightMagUI()
	{
		for (int i = 1; i <= bulletMAX; i++)
		{
			GameObject.Find("bulletR" + i).GetComponent<UISprite>().enabled = false;
		}
		for (int j = 1; j <= rightBulletLeft; j++)
		{
			GameObject.Find("bulletR" + j).GetComponent<UISprite>().enabled = true;
		}
	}

	public void useBlade(int amount = 0)
	{
		if (amount == 0)
		{
			amount = 1;
		}
		amount *= 2;
		if (!(currentBladeSta > 0f))
		{
			return;
		}
		currentBladeSta -= amount;
		if (currentBladeSta <= 0f)
		{
			if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE || base.photonView.isMine)
			{
				leftbladetrail.Deactivate();
				rightbladetrail.Deactivate();
				leftbladetrail2.Deactivate();
				rightbladetrail2.Deactivate();
				checkBoxLeft.GetComponent<TriggerColliderWeapon>().active_me = false;
				checkBoxRight.GetComponent<TriggerColliderWeapon>().active_me = false;
			}
			currentBladeSta = 0f;
			throwBlades();
		}
	}

	private void useGas(float amount = 0f)
	{
		if (SettingsManager.LegacyGameSettings.BombModeEnabled.Value && SettingsManager.LegacyGameSettings.BombModeInfiniteGas.Value)
		{
			return;
		}
		if (amount == 0f)
		{
			amount = useGasSpeed;
		}
		if (currentGas > 0f)
		{
			currentGas -= amount;
			if (currentGas < 0f)
			{
				currentGas = 0f;
			}
		}
	}

	[RPC]
	private void whoIsMyErenTitan(int id, PhotonMessageInfo info)
	{
		if (info != null && info.sender != base.photonView.owner)
		{
			FengGameManagerMKII.instance.kickPlayerRCIfMC(info.sender, true, "hero eren titan exploit");
			return;
		}
		eren_titan = PhotonView.Find(id).gameObject;
		titanForm = true;
	}
}
