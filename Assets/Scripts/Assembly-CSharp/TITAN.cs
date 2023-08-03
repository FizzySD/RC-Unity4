using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Constants;
using CustomSkins;
using ExitGames.Client.Photon;
using Photon;
using Settings;
using UI;
using UnityEngine;

internal class TITAN : Photon.MonoBehaviour
{
	[CompilerGenerated]
	private static Dictionary<string, int> fswitchSmap5;

	[CompilerGenerated]
	private static Dictionary<string, int> fswitchSmap6;

	[CompilerGenerated]
	private static Dictionary<string, int> fswitchSmap7;

	private Vector3 abnorma_jump_bite_horizon_v;

	public AbnormalType abnormalType;

	public int activeRad = int.MaxValue;

	private float angle;

	public bool asClientLookTarget;

	private string attackAnimation;

	private float attackCheckTime;

	private float attackCheckTimeA;

	private float attackCheckTimeB;

	private int attackCount;

	public float attackDistance = 13f;

	private bool attacked;

	private float attackEndWait;

	public float attackWait = 1f;

	public Animation baseAnimation;

	public AudioSource baseAudioSource;

	public List<Collider> baseColliders;

	public Transform baseGameObjectTransform;

	public Rigidbody baseRigidBody;

	public Transform baseTransform;

	private float between2;

	public float chaseDistance = 80f;

	public ArrayList checkPoints = new ArrayList();

	public bool colliderEnabled;

	public TITAN_CONTROLLER controller;

	public GameObject currentCamera;

	private Transform currentGrabHand;

	public int currentHealth;

	private float desDeg;

	private float dieTime;

	public bool eye;

	[CompilerGenerated]
	private static Dictionary<string, int> fswitchmap5;

	[CompilerGenerated]
	private static Dictionary<string, int> fswitchmap6;

	[CompilerGenerated]
	private static Dictionary<string, int> fswitchmap7;

	private string fxName;

	private Vector3 fxPosition;

	private Quaternion fxRotation;

	private float getdownTime;

	private GameObject grabbedTarget;

	public GameObject grabTF;

	private float gravity = 120f;

	private bool grounded;

	public bool hasDie;

	private bool hasDieSteam;

	public bool hasExplode;

	public bool hasload;

	public bool hasSetLevel;

	public bool hasSpawn;

	private Transform head;

	private Vector3 headscale = Vector3.one;

	public GameObject healthLabel;

	public bool healthLabelEnabled;

	public float healthTime;

	private string hitAnimation;

	private float hitPause;

	public bool isAlarm;

	private bool isAttackMoveByCore;

	private bool isGrabHandLeft;

	public bool isHooked;

	public bool isLook;

	public bool isThunderSpear;

	public float lagMax;

	private bool leftHandAttack;

	public GameObject mainMaterial;

	public int maxHealth;

	private float maxStamina = 320f;

	public float maxVelocityChange = 10f;

	public static float minusDistance = 99999f;

	public static GameObject minusDistanceEnemy;

	public FengGameManagerMKII MultiplayerManager;

	public int myDifficulty;

	public float myDistance;

	public GROUP myGroup = GROUP.T;

	public GameObject myHero;

	public float myLevel = 1f;

	public TitanTrigger myTitanTrigger;

	private Transform neck;

	private bool needFreshCorePosition;

	private string nextAttackAnimation;

	public bool nonAI;

	private bool nonAIcombo;

	private Vector3 oldCorePosition;

	private Quaternion oldHeadRotation;

	public PVPcheckPoint PVPfromCheckPt;

	private float random_run_time;

	private float rockInterval;

	public bool rockthrow;

	private string runAnimation;

	private float sbtime;

	public int skin;

	private Vector3 spawnPt;

	public float speed = 7f;

	private float stamina = 320f;

	private TitanState state;

	private int stepSoundPhase = 2;

	private bool stuck;

	private float stuckTime;

	private float stuckTurnAngle;

	private Vector3 targetCheckPt;

	private Quaternion targetHeadRotation;

	private float targetR;

	private float tauntTime;

	private GameObject throwRock;

	private string turnAnimation;

	private float turnDeg;

	private GameObject whoHasTauntMe;

	private TitanCustomSkinLoader _customSkinLoader;

	private bool _hasRunStart;

	private HashSet<string> _ignoreLookTargetAnimations;

	private HashSet<string> _fastHeadRotationAnimations;

	private bool _ignoreLookTarget;

	private bool _fastHeadRotation;

	private void HideTitanIfBomb()
	{
		if (IN_GAME_MAIN_CAMERA.gametype != 0 && PhotonNetwork.isMasterClient && SettingsManager.LegacyGameSettings.BombModeEnabled.Value && SettingsManager.LegacyGameSettings.BombModeDisableTitans.Value)
		{
			base.transform.position = new Vector3(-10000f, -10000f, -10000f);
		}
	}

	public bool WillDIe(int damage)
	{
		if (!hasDie)
		{
			if (!SettingsManager.LegacyGameSettings.TitanArmorEnabled.Value || damage >= SettingsManager.LegacyGameSettings.TitanArmor.Value)
			{
				return (float)(currentHealth - damage) <= 0f;
			}
			if (abnormalType == AbnormalType.TYPE_CRAWLER && !SettingsManager.LegacyGameSettings.TitanArmorCrawlerEnabled.Value)
			{
				return (float)(currentHealth - damage) <= 0f;
			}
		}
		return false;
	}

	private void attack(string type)
	{
		state = TitanState.attack;
		attacked = false;
		isAlarm = true;
		if (attackAnimation == type)
		{
			attackAnimation = type;
			playAnimationAt("attack_" + type, 0f);
		}
		else
		{
			attackAnimation = type;
			playAnimationAt("attack_" + type, 0f);
		}
		nextAttackAnimation = null;
		fxName = null;
		isAttackMoveByCore = false;
		attackCheckTime = 0f;
		attackCheckTimeA = 0f;
		attackCheckTimeB = 0f;
		attackEndWait = 0f;
		fxRotation = Quaternion.Euler(270f, 0f, 0f);
		switch (type)
		{
		case "abnormal_getup":
			attackCheckTime = 0f;
			fxName = string.Empty;
			break;
		case "abnormal_jump":
			nextAttackAnimation = "abnormal_getup";
			if (!nonAI)
			{
				attackEndWait = ((myDifficulty <= 0) ? UnityEngine.Random.Range(1f, 4f) : UnityEngine.Random.Range(0f, 1f));
			}
			else
			{
				attackEndWait = 0f;
			}
			attackCheckTime = 0.75f;
			fxName = "boom4";
			fxRotation = Quaternion.Euler(270f, base.transform.rotation.eulerAngles.y, 0f);
			break;
		case "combo_1":
			nextAttackAnimation = "combo_2";
			attackCheckTimeA = 0.54f;
			attackCheckTimeB = 0.76f;
			nonAIcombo = false;
			isAttackMoveByCore = true;
			leftHandAttack = false;
			break;
		case "combo_2":
			if (abnormalType != AbnormalType.TYPE_PUNK)
			{
				nextAttackAnimation = "combo_3";
			}
			attackCheckTimeA = 0.37f;
			attackCheckTimeB = 0.57f;
			nonAIcombo = false;
			isAttackMoveByCore = true;
			leftHandAttack = true;
			break;
		case "combo_3":
			nonAIcombo = false;
			isAttackMoveByCore = true;
			attackCheckTime = 0.21f;
			fxName = "boom1";
			break;
		case "front_ground":
			fxName = "boom1";
			attackCheckTime = 0.45f;
			break;
		case "kick":
			fxName = "boom5";
			fxRotation = base.transform.rotation;
			attackCheckTime = 0.43f;
			break;
		case "slap_back":
			fxName = "boom3";
			attackCheckTime = 0.66f;
			break;
		case "slap_face":
			fxName = "boom3";
			attackCheckTime = 0.655f;
			break;
		case "stomp":
			fxName = "boom2";
			attackCheckTime = 0.42f;
			break;
		case "bite":
			fxName = "bite";
			attackCheckTime = 0.6f;
			break;
		case "bite_l":
			fxName = "bite";
			attackCheckTime = 0.4f;
			break;
		case "bite_r":
			fxName = "bite";
			attackCheckTime = 0.4f;
			break;
		case "jumper_0":
			abnorma_jump_bite_horizon_v = Vector3.zero;
			break;
		case "crawler_jump_0":
			abnorma_jump_bite_horizon_v = Vector3.zero;
			break;
		case "anti_AE_l":
			attackCheckTimeA = 0.31f;
			attackCheckTimeB = 0.4f;
			leftHandAttack = true;
			break;
		case "anti_AE_r":
			attackCheckTimeA = 0.31f;
			attackCheckTimeB = 0.4f;
			leftHandAttack = false;
			break;
		case "anti_AE_low_l":
			attackCheckTimeA = 0.31f;
			attackCheckTimeB = 0.4f;
			leftHandAttack = true;
			break;
		case "anti_AE_low_r":
			attackCheckTimeA = 0.31f;
			attackCheckTimeB = 0.4f;
			leftHandAttack = false;
			break;
		case "quick_turn_l":
			attackCheckTimeA = 2f;
			attackCheckTimeB = 2f;
			isAttackMoveByCore = true;
			break;
		case "quick_turn_r":
			attackCheckTimeA = 2f;
			attackCheckTimeB = 2f;
			isAttackMoveByCore = true;
			break;
		case "throw":
			isAlarm = true;
			chaseDistance = 99999f;
			break;
		}
		needFreshCorePosition = true;
	}

	private void attack2(string type)
	{
		state = TitanState.attack;
		attacked = false;
		isAlarm = true;
		if (attackAnimation == type)
		{
			attackAnimation = type;
			playAnimationAt("attack_" + type, 0f);
		}
		else
		{
			attackAnimation = type;
			playAnimationAt("attack_" + type, 0f);
		}
		nextAttackAnimation = null;
		fxName = null;
		isAttackMoveByCore = false;
		attackCheckTime = 0f;
		attackCheckTimeA = 0f;
		attackCheckTimeB = 0f;
		attackEndWait = 0f;
		fxRotation = Quaternion.Euler(270f, 0f, 0f);
		switch (type)
		{
		case "abnormal_getup":
			attackCheckTime = 0f;
			fxName = string.Empty;
			break;
		case "abnormal_jump":
			nextAttackAnimation = "abnormal_getup";
			if (nonAI)
			{
				attackEndWait = 0f;
			}
			else
			{
				attackEndWait = ((myDifficulty <= 0) ? UnityEngine.Random.Range(1f, 4f) : UnityEngine.Random.Range(0f, 1f));
			}
			attackCheckTime = 0.75f;
			fxName = "boom4";
			fxRotation = Quaternion.Euler(270f, baseTransform.rotation.eulerAngles.y, 0f);
			break;
		case "combo_1":
			nextAttackAnimation = "combo_2";
			attackCheckTimeA = 0.54f;
			attackCheckTimeB = 0.76f;
			nonAIcombo = false;
			isAttackMoveByCore = true;
			leftHandAttack = false;
			break;
		case "combo_2":
			if (abnormalType != AbnormalType.TYPE_PUNK && !nonAI)
			{
				nextAttackAnimation = "combo_3";
			}
			attackCheckTimeA = 0.37f;
			attackCheckTimeB = 0.57f;
			nonAIcombo = false;
			isAttackMoveByCore = true;
			leftHandAttack = true;
			break;
		case "combo_3":
			nonAIcombo = false;
			isAttackMoveByCore = true;
			attackCheckTime = 0.21f;
			fxName = "boom1";
			break;
		case "front_ground":
			fxName = "boom1";
			attackCheckTime = 0.45f;
			break;
		case "kick":
			fxName = "boom5";
			fxRotation = baseTransform.rotation;
			attackCheckTime = 0.43f;
			break;
		case "slap_back":
			fxName = "boom3";
			attackCheckTime = 0.66f;
			break;
		case "slap_face":
			fxName = "boom3";
			attackCheckTime = 0.655f;
			break;
		case "stomp":
			fxName = "boom2";
			attackCheckTime = 0.42f;
			break;
		case "bite":
			fxName = "bite";
			attackCheckTime = 0.6f;
			break;
		case "bite_l":
			fxName = "bite";
			attackCheckTime = 0.4f;
			break;
		case "bite_r":
			fxName = "bite";
			attackCheckTime = 0.4f;
			break;
		case "jumper_0":
			abnorma_jump_bite_horizon_v = Vector3.zero;
			break;
		case "crawler_jump_0":
			abnorma_jump_bite_horizon_v = Vector3.zero;
			break;
		case "anti_AE_l":
			attackCheckTimeA = 0.31f;
			attackCheckTimeB = 0.4f;
			leftHandAttack = true;
			break;
		case "anti_AE_r":
			attackCheckTimeA = 0.31f;
			attackCheckTimeB = 0.4f;
			leftHandAttack = false;
			break;
		case "anti_AE_low_l":
			attackCheckTimeA = 0.31f;
			attackCheckTimeB = 0.4f;
			leftHandAttack = true;
			break;
		case "anti_AE_low_r":
			attackCheckTimeA = 0.31f;
			attackCheckTimeB = 0.4f;
			leftHandAttack = false;
			break;
		case "quick_turn_l":
			attackCheckTimeA = 2f;
			attackCheckTimeB = 2f;
			isAttackMoveByCore = true;
			break;
		case "quick_turn_r":
			attackCheckTimeA = 2f;
			attackCheckTimeB = 2f;
			isAttackMoveByCore = true;
			break;
		case "throw":
			isAlarm = true;
			chaseDistance = 99999f;
			break;
		}
		needFreshCorePosition = true;
	}

	private void Awake()
	{
		cache();
		baseRigidBody.freezeRotation = true;
		baseRigidBody.useGravity = false;
		_customSkinLoader = base.gameObject.AddComponent<TitanCustomSkinLoader>();
		_ignoreLookTargetAnimations = new HashSet<string> { "sit_hunt_down", "hit_eren_L", "hit_eren_R", "idle_recovery", "eat_l", "eat_r", "sit_hit_eye", "hit_eye" };
		_fastHeadRotationAnimations = new HashSet<string> { "hit_eren_L", "hit_eren_R", "sit_hit_eye", "hit_eye" };
		foreach (AnimationState item in base.animation)
		{
			if (item.name.StartsWith("attack_"))
			{
				_ignoreLookTargetAnimations.Add(item.name);
				_fastHeadRotationAnimations.Add(item.name);
			}
		}
		HideTitanIfBomb();
	}

	private void CheckAnimationLookTarget(string animation)
	{
		_ignoreLookTarget = _ignoreLookTargetAnimations.Contains(animation);
		_fastHeadRotation = _fastHeadRotationAnimations.Contains(animation);
	}

	private IEnumerator HandleSpawnCollisionCoroutine(float time, float maxSpeed)
	{
		while (time > 0f)
		{
			if (baseRigidBody.velocity.magnitude > maxSpeed)
			{
				baseRigidBody.velocity = baseRigidBody.velocity.normalized * maxSpeed;
			}
			time -= Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate();
		}
	}

	public void beLaughAttacked()
	{
		if (!hasDie && abnormalType != AbnormalType.TYPE_CRAWLER)
		{
			if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER)
			{
				object[] parameters = new object[1] { 0f };
				base.photonView.RPC("laugh", PhotonTargets.All, parameters);
			}
			else if (state == TitanState.idle || state == TitanState.turn || state == TitanState.chase)
			{
				laugh();
			}
		}
	}

	public void beTauntedBy(GameObject target, float tauntTime)
	{
		whoHasTauntMe = target;
		this.tauntTime = tauntTime;
		isAlarm = true;
	}

	public void cache()
	{
		baseAudioSource = base.transform.Find("snd_titan_foot").GetComponent<AudioSource>();
		baseAnimation = base.animation;
		baseTransform = base.transform;
		baseRigidBody = base.rigidbody;
		baseColliders = new List<Collider>();
		Collider[] componentsInChildren = GetComponentsInChildren<Collider>();
		foreach (Collider collider in componentsInChildren)
		{
			if (collider.name != "AABB")
			{
				baseColliders.Add(collider);
			}
		}
		GameObject gameObject = new GameObject
		{
			name = "PlayerDetectorRC"
		};
		CapsuleCollider capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
		CapsuleCollider component = baseTransform.Find("AABB").GetComponent<CapsuleCollider>();
		capsuleCollider.center = component.center;
		capsuleCollider.radius = Math.Abs(baseTransform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head").position.y - baseTransform.position.y);
		capsuleCollider.height = component.height * 1.2f;
		capsuleCollider.material = component.material;
		capsuleCollider.isTrigger = true;
		capsuleCollider.name = "PlayerDetectorRC";
		myTitanTrigger = gameObject.AddComponent<TitanTrigger>();
		myTitanTrigger.isCollide = false;
		gameObject.layer = PhysicsLayer.PlayerAttackBox;
		gameObject.transform.parent = baseTransform.Find("AABB");
		gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
		MultiplayerManager = FengGameManagerMKII.instance;
		if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE || base.photonView.isMine)
		{
			baseGameObjectTransform = base.gameObject.transform;
		}
	}

	private void chase()
	{
		state = TitanState.chase;
		isAlarm = true;
		crossFade(runAnimation, 0.5f);
	}

	private GameObject checkIfHitCrawlerMouth(Transform head, float rad)
	{
		float num = rad * myLevel;
		GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
		foreach (GameObject gameObject in array)
		{
			if (gameObject.GetComponent<TITAN_EREN>() == null && (gameObject.GetComponent<HERO>() == null || !gameObject.GetComponent<HERO>().isInvincible()))
			{
				float num2 = gameObject.GetComponent<CapsuleCollider>().height * 0.5f;
				if (Vector3.Distance(gameObject.transform.position + Vector3.up * num2, head.position - Vector3.up * 1.5f * myLevel) < num + num2)
				{
					return gameObject;
				}
			}
		}
		return null;
	}

	private GameObject checkIfHitHand(Transform hand)
	{
		float num = 2.4f * myLevel;
		Collider[] array = Physics.OverlapSphere(hand.GetComponent<SphereCollider>().transform.position, num + 1f);
		foreach (Collider collider in array)
		{
			if (!(collider.transform.root.tag == "Player"))
			{
				continue;
			}
			GameObject gameObject = collider.transform.root.gameObject;
			if (gameObject.GetComponent<TITAN_EREN>() != null)
			{
				if (!gameObject.GetComponent<TITAN_EREN>().isHit)
				{
					gameObject.GetComponent<TITAN_EREN>().hitByTitan();
				}
			}
			else if (gameObject.GetComponent<HERO>() != null && !gameObject.GetComponent<HERO>().isInvincible())
			{
				return gameObject;
			}
		}
		return null;
	}

	private GameObject checkIfHitHead(Transform head, float rad)
	{
		float num = rad * myLevel;
		GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
		foreach (GameObject gameObject in array)
		{
			if (gameObject.GetComponent<TITAN_EREN>() == null && (gameObject.GetComponent<HERO>() == null || !gameObject.GetComponent<HERO>().isInvincible()))
			{
				float num2 = gameObject.GetComponent<CapsuleCollider>().height * 0.5f;
				if (Vector3.Distance(gameObject.transform.position + Vector3.up * num2, head.position + Vector3.up * 1.5f * myLevel) < num + num2)
				{
					return gameObject;
				}
			}
		}
		return null;
	}

	private void crossFadeIfNotPlaying(string aniName, float time)
	{
		if (!base.animation.IsPlaying(aniName) || IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE || PhotonNetwork.offlineMode)
		{
			crossFade(aniName, time);
		}
	}

	private void crossFade(string aniName, float time)
	{
		base.animation.CrossFade(aniName, time);
		CheckAnimationLookTarget(aniName);
		if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && base.photonView.isMine)
		{
			object[] parameters = new object[2] { aniName, time };
			base.photonView.RPC("netCrossFade", PhotonTargets.Others, parameters);
		}
	}

	public bool die()
	{
		if (hasDie)
		{
			return false;
		}
		hasDie = true;
		GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().oneTitanDown(string.Empty, false);
		dieAnimation();
		return true;
	}

	private void dieAnimation()
	{
		if (base.animation.IsPlaying("sit_idle") || base.animation.IsPlaying("sit_hit_eye"))
		{
			crossFade("sit_die", 0.1f);
		}
		else if (abnormalType == AbnormalType.TYPE_CRAWLER)
		{
			crossFade("crawler_die", 0.2f);
		}
		else if (abnormalType == AbnormalType.NORMAL)
		{
			crossFade("die_front", 0.05f);
		}
		else if ((base.animation.IsPlaying("attack_abnormal_jump") && base.animation["attack_abnormal_jump"].normalizedTime > 0.7f) || (base.animation.IsPlaying("attack_abnormal_getup") && base.animation["attack_abnormal_getup"].normalizedTime < 0.7f) || base.animation.IsPlaying("tired"))
		{
			crossFade("die_ground", 0.2f);
		}
		else
		{
			crossFade("die_back", 0.05f);
		}
	}

	public void dieBlow(Vector3 attacker, float hitPauseTime)
	{
		if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
		{
			dieBlowFunc(attacker, hitPauseTime);
			if (GameObject.FindGameObjectsWithTag("titan").Length <= 1)
			{
				GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
			}
		}
		else
		{
			object[] parameters = new object[2] { attacker, hitPauseTime };
			base.photonView.RPC("dieBlowRPC", PhotonTargets.All, parameters);
		}
	}

	public void dieBlowFunc(Vector3 attacker, float hitPauseTime)
	{
		if (hasDie)
		{
			return;
		}
		base.transform.rotation = Quaternion.Euler(0f, Quaternion.LookRotation(attacker - base.transform.position).eulerAngles.y, 0f);
		hasDie = true;
		hitAnimation = "die_blow";
		hitPause = hitPauseTime;
		playAnimation(hitAnimation);
		base.animation[hitAnimation].time = 0f;
		base.animation[hitAnimation].speed = 0f;
		needFreshCorePosition = true;
		GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().oneTitanDown(string.Empty, false);
		if (base.photonView.isMine)
		{
			if (grabbedTarget != null)
			{
				grabbedTarget.GetPhotonView().RPC("netUngrabbed", PhotonTargets.All);
			}
			if (nonAI)
			{
				currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(null);
				currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().setSpectorMode(true);
				currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
				ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
				hashtable.Add(PhotonPlayerProperty.dead, true);
				PhotonNetwork.player.SetCustomProperties(hashtable);
				hashtable = new ExitGames.Client.Photon.Hashtable();
				hashtable.Add(PhotonPlayerProperty.deaths, (int)PhotonNetwork.player.customProperties[PhotonPlayerProperty.deaths] + 1);
				PhotonNetwork.player.SetCustomProperties(hashtable);
			}
		}
	}

	[RPC]
	private void dieBlowRPC(Vector3 attacker, float hitPauseTime)
	{
		if (base.photonView.isMine && (attacker - base.transform.position).magnitude < 80f)
		{
			dieBlowFunc(attacker, hitPauseTime);
		}
	}

	[RPC]
	public void DieByCannon(int viewID)
	{
		PhotonView photonView = PhotonView.Find(viewID);
		if (photonView != null)
		{
			int damage = 0;
			if (PhotonNetwork.isMasterClient)
			{
				OnTitanDie(photonView);
			}
			if (nonAI)
			{
				FengGameManagerMKII.instance.titanGetKill(photonView.owner, damage, (string)PhotonNetwork.player.customProperties[PhotonPlayerProperty.name]);
			}
			else
			{
				FengGameManagerMKII.instance.titanGetKill(photonView.owner, damage, base.name);
			}
		}
		else
		{
			FengGameManagerMKII.instance.photonView.RPC("netShowDamage", photonView.owner, speed);
		}
	}

	public void dieHeadBlow(Vector3 attacker, float hitPauseTime)
	{
		if (abnormalType == AbnormalType.TYPE_CRAWLER)
		{
			return;
		}
		if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
		{
			dieHeadBlowFunc(attacker, hitPauseTime);
			if (GameObject.FindGameObjectsWithTag("titan").Length <= 1)
			{
				GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
			}
		}
		else
		{
			object[] parameters = new object[2] { attacker, hitPauseTime };
			base.photonView.RPC("dieHeadBlowRPC", PhotonTargets.All, parameters);
		}
	}

	public void dieHeadBlowFunc(Vector3 attacker, float hitPauseTime)
	{
		if (hasDie)
		{
			return;
		}
		playSound("snd_titan_head_blow");
		base.transform.rotation = Quaternion.Euler(0f, Quaternion.LookRotation(attacker - base.transform.position).eulerAngles.y, 0f);
		hasDie = true;
		hitAnimation = "die_headOff";
		hitPause = hitPauseTime;
		playAnimation(hitAnimation);
		base.animation[hitAnimation].time = 0f;
		base.animation[hitAnimation].speed = 0f;
		GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().oneTitanDown(string.Empty, false);
		needFreshCorePosition = true;
		GameObject gameObject = ((IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.MULTIPLAYER || !base.photonView.isMine) ? ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("bloodExplore"), head.position + Vector3.up * 1f * myLevel, Quaternion.Euler(270f, 0f, 0f))) : PhotonNetwork.Instantiate("bloodExplore", head.position + Vector3.up * 1f * myLevel, Quaternion.Euler(270f, 0f, 0f), 0));
		gameObject.transform.localScale = base.transform.localScale;
		gameObject = ((IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.MULTIPLAYER || !base.photonView.isMine) ? ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("bloodsplatter"), head.position, Quaternion.Euler(270f + neck.rotation.eulerAngles.x, neck.rotation.eulerAngles.y, neck.rotation.eulerAngles.z))) : PhotonNetwork.Instantiate("bloodsplatter", head.position, Quaternion.Euler(270f + neck.rotation.eulerAngles.x, neck.rotation.eulerAngles.y, neck.rotation.eulerAngles.z), 0));
		gameObject.transform.localScale = base.transform.localScale;
		gameObject.transform.parent = neck;
		gameObject = ((IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.MULTIPLAYER || !base.photonView.isMine) ? ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("FX/justSmoke"), neck.position, Quaternion.Euler(270f, 0f, 0f))) : PhotonNetwork.Instantiate("FX/justSmoke", neck.position, Quaternion.Euler(270f, 0f, 0f), 0));
		gameObject.transform.parent = neck;
		if (base.photonView.isMine)
		{
			if (grabbedTarget != null)
			{
				grabbedTarget.GetPhotonView().RPC("netUngrabbed", PhotonTargets.All);
			}
			if (nonAI)
			{
				currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(null);
				currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().setSpectorMode(true);
				currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
				ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
				hashtable.Add(PhotonPlayerProperty.dead, true);
				PhotonNetwork.player.SetCustomProperties(hashtable);
				hashtable = new ExitGames.Client.Photon.Hashtable();
				hashtable.Add(PhotonPlayerProperty.deaths, (int)PhotonNetwork.player.customProperties[PhotonPlayerProperty.deaths] + 1);
				PhotonNetwork.player.SetCustomProperties(hashtable);
			}
		}
	}

	[RPC]
	private void dieHeadBlowRPC(Vector3 attacker, float hitPauseTime)
	{
		if (base.photonView.isMine && (attacker - neck.position).magnitude < lagMax)
		{
			dieHeadBlowFunc(attacker, hitPauseTime);
		}
	}

	private void eat()
	{
		state = TitanState.eat;
		attacked = false;
		if (isGrabHandLeft)
		{
			attackAnimation = "eat_l";
			crossFade("eat_l", 0.1f);
		}
		else
		{
			attackAnimation = "eat_r";
			crossFade("eat_r", 0.1f);
		}
	}

	private void eatSet(GameObject grabTarget)
	{
		if ((IN_GAME_MAIN_CAMERA.gametype != 0 && (IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.MULTIPLAYER || !base.photonView.isMine)) || !grabTarget.GetComponent<HERO>().isGrabbed)
		{
			grabToRight(null);
			if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && base.photonView.isMine)
			{
				base.photonView.RPC("grabToRight", PhotonTargets.Others);
				object[] parameters = new object[1] { "grabbed" };
				grabTarget.GetPhotonView().RPC("netPlayAnimation", PhotonTargets.All, parameters);
				object[] parameters2 = new object[2]
				{
					base.photonView.viewID,
					false
				};
				grabTarget.GetPhotonView().RPC("netGrabbed", PhotonTargets.All, parameters2);
			}
			else
			{
				grabTarget.GetComponent<HERO>().grabbed(base.gameObject, false);
				grabTarget.GetComponent<HERO>().animation.Play("grabbed");
			}
		}
	}

	private void eatSetL(GameObject grabTarget)
	{
		if ((IN_GAME_MAIN_CAMERA.gametype != 0 && (IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.MULTIPLAYER || !base.photonView.isMine)) || !grabTarget.GetComponent<HERO>().isGrabbed)
		{
			grabToLeft(null);
			if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && base.photonView.isMine)
			{
				base.photonView.RPC("grabToLeft", PhotonTargets.Others);
				object[] parameters = new object[1] { "grabbed" };
				grabTarget.GetPhotonView().RPC("netPlayAnimation", PhotonTargets.All, parameters);
				object[] parameters2 = new object[2]
				{
					base.photonView.viewID,
					true
				};
				grabTarget.GetPhotonView().RPC("netGrabbed", PhotonTargets.All, parameters2);
			}
			else
			{
				grabTarget.GetComponent<HERO>().grabbed(base.gameObject, true);
				grabTarget.GetComponent<HERO>().animation.Play("grabbed");
			}
		}
	}

	private bool executeAttack(string decidedAction)
	{
		switch (decidedAction)
		{
		case "grab_ground_front_l":
			grab("ground_front_l");
			return true;
		case "grab_ground_front_r":
			grab("ground_front_r");
			return true;
		case "grab_ground_back_l":
			grab("ground_back_l");
			return true;
		case "grab_ground_back_r":
			grab("ground_back_r");
			return true;
		case "grab_head_front_l":
			grab("head_front_l");
			return true;
		case "grab_head_front_r":
			grab("head_front_r");
			return true;
		case "grab_head_back_l":
			grab("head_back_l");
			return true;
		case "grab_head_back_r":
			grab("head_back_r");
			return true;
		case "attack_abnormal_jump":
			attack("abnormal_jump");
			return true;
		case "attack_combo":
			attack("combo_1");
			return true;
		case "attack_front_ground":
			attack("front_ground");
			return true;
		case "attack_kick":
			attack("kick");
			return true;
		case "attack_slap_back":
			attack("slap_back");
			return true;
		case "attack_slap_face":
			attack("slap_face");
			return true;
		case "attack_stomp":
			attack("stomp");
			return true;
		case "attack_bite":
			attack("bite");
			return true;
		case "attack_bite_l":
			attack("bite_l");
			return true;
		case "attack_bite_r":
			attack("bite_r");
			return true;
		default:
			return false;
		}
	}

	private bool executeAttack2(string decidedAction)
	{
		switch (decidedAction)
		{
		case "grab_ground_front_l":
			grab("ground_front_l");
			return true;
		case "grab_ground_front_r":
			grab("ground_front_r");
			return true;
		case "grab_ground_back_l":
			grab("ground_back_l");
			return true;
		case "grab_ground_back_r":
			grab("ground_back_r");
			return true;
		case "grab_head_front_l":
			grab("head_front_l");
			return true;
		case "grab_head_front_r":
			grab("head_front_r");
			return true;
		case "grab_head_back_l":
			grab("head_back_l");
			return true;
		case "grab_head_back_r":
			grab("head_back_r");
			return true;
		case "attack_abnormal_jump":
			attack2("abnormal_jump");
			return true;
		case "attack_combo":
			attack2("combo_1");
			return true;
		case "attack_front_ground":
			attack2("front_ground");
			return true;
		case "attack_kick":
			attack2("kick");
			return true;
		case "attack_slap_back":
			attack2("slap_back");
			return true;
		case "attack_slap_face":
			attack2("slap_face");
			return true;
		case "attack_stomp":
			attack2("stomp");
			return true;
		case "attack_bite":
			attack2("bite");
			return true;
		case "attack_bite_l":
			attack2("bite_l");
			return true;
		case "attack_bite_r":
			attack2("bite_r");
			return true;
		default:
			return false;
		}
	}

	public void explode()
	{
		if (!SettingsManager.LegacyGameSettings.TitanExplodeEnabled.Value || !hasDie || !(dieTime >= 1f) || hasExplode)
		{
			return;
		}
		int num = 0;
		float num2 = myLevel * 10f;
		if (abnormalType == AbnormalType.TYPE_CRAWLER)
		{
			if (dieTime >= 2f)
			{
				hasExplode = true;
				num2 = 0f;
				num = 1;
			}
		}
		else
		{
			num = 1;
			hasExplode = true;
		}
		if (num != 1)
		{
			return;
		}
		Vector3 vector = baseTransform.position + Vector3.up * num2;
		PhotonNetwork.Instantiate("FX/Thunder", vector, Quaternion.Euler(270f, 0f, 0f), 0);
		PhotonNetwork.Instantiate("FX/boom1", vector, Quaternion.Euler(270f, 0f, 0f), 0);
		GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
		foreach (GameObject gameObject in array)
		{
			if (Vector3.Distance(gameObject.transform.position, vector) < (float)SettingsManager.LegacyGameSettings.TitanExplodeRadius.Value)
			{
				gameObject.GetComponent<HERO>().markDie();
				gameObject.GetComponent<HERO>().photonView.RPC("netDie2", PhotonTargets.All, -1, "Server ");
			}
		}
	}

	private void findNearestFacingHero2()
	{
		GameObject gameObject = null;
		float num = float.PositiveInfinity;
		Vector3 position = baseTransform.position;
		float num2 = 0f;
		float num3 = ((abnormalType != 0) ? 180f : 100f);
		float num4 = 0f;
		foreach (HERO player in MultiplayerManager.getPlayers())
		{
			float num5 = Vector3.Distance(player.transform.position, position);
			if (num5 < num)
			{
				Vector3 vector = player.transform.position - baseTransform.position;
				num2 = (0f - Mathf.Atan2(vector.z, vector.x)) * 57.29578f;
				num4 = 0f - Mathf.DeltaAngle(num2, baseGameObjectTransform.rotation.eulerAngles.y - 90f);
				if (Mathf.Abs(num4) < num3)
				{
					gameObject = player.gameObject;
					num = num5;
				}
			}
		}
		foreach (TITAN_EREN eren in MultiplayerManager.getErens())
		{
			float num6 = Vector3.Distance(eren.transform.position, position);
			if (num6 < num)
			{
				Vector3 vector2 = eren.transform.position - baseTransform.position;
				num2 = (0f - Mathf.Atan2(vector2.z, vector2.x)) * 57.29578f;
				num4 = 0f - Mathf.DeltaAngle(num2, baseGameObjectTransform.rotation.eulerAngles.y - 90f);
				if (Mathf.Abs(num4) < num3)
				{
					gameObject = eren.gameObject;
					num = num6;
				}
			}
		}
		if (!(gameObject != null))
		{
			return;
		}
		GameObject gameObject2 = myHero;
		myHero = gameObject;
		if (gameObject2 != myHero && IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && PhotonNetwork.isMasterClient)
		{
			if (myHero == null)
			{
				object[] parameters = new object[1] { -1 };
				base.photonView.RPC("setMyTarget", PhotonTargets.Others, parameters);
			}
			else
			{
				object[] parameters2 = new object[1] { myHero.GetPhotonView().viewID };
				base.photonView.RPC("setMyTarget", PhotonTargets.Others, parameters2);
			}
		}
		tauntTime = 5f;
	}

	private void findNearestHero2()
	{
		GameObject gameObject = myHero;
		myHero = getNearestHero2();
		if (myHero != gameObject && IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && PhotonNetwork.isMasterClient)
		{
			if (myHero == null)
			{
				object[] parameters = new object[1] { -1 };
				base.photonView.RPC("setMyTarget", PhotonTargets.Others, parameters);
			}
			else
			{
				object[] parameters2 = new object[1] { myHero.GetPhotonView().viewID };
				base.photonView.RPC("setMyTarget", PhotonTargets.Others, parameters2);
			}
		}
		oldHeadRotation = head.rotation;
	}

	private void FixedUpdate()
	{
		if ((GameMenu.Paused && IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE) || (IN_GAME_MAIN_CAMERA.gametype != 0 && !base.photonView.isMine))
		{
			return;
		}
		baseRigidBody.AddForce(new Vector3(0f, (0f - gravity) * baseRigidBody.mass, 0f));
		if (needFreshCorePosition)
		{
			oldCorePosition = baseTransform.position - baseTransform.Find("Amarture/Core").position;
			needFreshCorePosition = false;
		}
		if (hasDie)
		{
			if (hitPause <= 0f && baseAnimation.IsPlaying("die_headOff"))
			{
				Vector3 vector = baseTransform.position - baseTransform.Find("Amarture/Core").position - oldCorePosition;
				baseRigidBody.velocity = vector / Time.deltaTime + Vector3.up * baseRigidBody.velocity.y;
			}
			oldCorePosition = baseTransform.position - baseTransform.Find("Amarture/Core").position;
		}
		else if ((state == TitanState.attack && isAttackMoveByCore) || state == TitanState.hit)
		{
			Vector3 vector2 = baseTransform.position - baseTransform.Find("Amarture/Core").position - oldCorePosition;
			baseRigidBody.velocity = vector2 / Time.deltaTime + Vector3.up * baseRigidBody.velocity.y;
			oldCorePosition = baseTransform.position - baseTransform.Find("Amarture/Core").position;
		}
		if (hasDie)
		{
			if (hitPause > 0f)
			{
				hitPause -= Time.deltaTime;
				if (hitPause <= 0f)
				{
					baseAnimation[hitAnimation].speed = 1f;
					hitPause = 0f;
				}
			}
			else if (baseAnimation.IsPlaying("die_blow"))
			{
				if (baseAnimation["die_blow"].normalizedTime < 0.55f)
				{
					baseRigidBody.velocity = -baseTransform.forward * 300f + Vector3.up * baseRigidBody.velocity.y;
				}
				else if (baseAnimation["die_blow"].normalizedTime < 0.83f)
				{
					baseRigidBody.velocity = -baseTransform.forward * 100f + Vector3.up * baseRigidBody.velocity.y;
				}
				else
				{
					baseRigidBody.velocity = Vector3.up * baseRigidBody.velocity.y;
				}
			}
			return;
		}
		if (nonAI && !GameMenu.Paused && (state == TitanState.idle || (state == TitanState.attack && attackAnimation == "jumper_1")))
		{
			Vector3 vector3 = Vector3.zero;
			if (controller.targetDirection != -874f)
			{
				bool flag = false;
				if (stamina < 5f)
				{
					flag = true;
				}
				else if (!(stamina >= 40f) && !baseAnimation.IsPlaying("run_abnormal") && !baseAnimation.IsPlaying("crawler_run"))
				{
					flag = true;
				}
				vector3 = ((!(controller.isWALKDown || flag)) ? (baseTransform.forward * speed * Mathf.Sqrt(myLevel)) : (baseTransform.forward * speed * Mathf.Sqrt(myLevel) * 0.2f));
				baseGameObjectTransform.rotation = Quaternion.Lerp(baseGameObjectTransform.rotation, Quaternion.Euler(0f, controller.targetDirection, 0f), speed * 0.15f * Time.deltaTime);
				if (state == TitanState.idle)
				{
					if (controller.isWALKDown || flag)
					{
						if (abnormalType == AbnormalType.TYPE_CRAWLER)
						{
							if (!baseAnimation.IsPlaying("crawler_run"))
							{
								crossFade("crawler_run", 0.1f);
							}
						}
						else if (!baseAnimation.IsPlaying("run_walk"))
						{
							crossFade("run_walk", 0.1f);
						}
					}
					else if (abnormalType == AbnormalType.TYPE_CRAWLER)
					{
						if (!baseAnimation.IsPlaying("crawler_run"))
						{
							crossFade("crawler_run", 0.1f);
						}
						GameObject gameObject = checkIfHitCrawlerMouth(head, 2.2f);
						if (gameObject != null)
						{
							Vector3 position = baseTransform.Find("Amarture/Core/Controller_Body/hip/spine/chest").position;
							if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
							{
								gameObject.GetComponent<HERO>().die((gameObject.transform.position - position) * 15f * myLevel, false);
							}
							else if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && base.photonView.isMine && !gameObject.GetComponent<HERO>().HasDied())
							{
								gameObject.GetComponent<HERO>().markDie();
								object[] parameters = new object[5]
								{
									(gameObject.transform.position - position) * 15f * myLevel,
									true,
									(!nonAI) ? (-1) : base.photonView.viewID,
									base.name,
									true
								};
								gameObject.GetComponent<HERO>().photonView.RPC("netDie", PhotonTargets.All, parameters);
							}
						}
					}
					else if (!baseAnimation.IsPlaying("run_abnormal"))
					{
						crossFade("run_abnormal", 0.1f);
					}
				}
			}
			else if (state == TitanState.idle)
			{
				if (abnormalType == AbnormalType.TYPE_CRAWLER)
				{
					if (!baseAnimation.IsPlaying("crawler_idle"))
					{
						crossFade("crawler_idle", 0.1f);
					}
				}
				else if (!baseAnimation.IsPlaying("idle"))
				{
					crossFade("idle", 0.1f);
				}
				vector3 = Vector3.zero;
			}
			if (state == TitanState.idle)
			{
				Vector3 velocity = baseRigidBody.velocity;
				Vector3 force = vector3 - velocity;
				force.x = Mathf.Clamp(force.x, 0f - maxVelocityChange, maxVelocityChange);
				force.z = Mathf.Clamp(force.z, 0f - maxVelocityChange, maxVelocityChange);
				force.y = 0f;
				baseRigidBody.AddForce(force, ForceMode.VelocityChange);
			}
			else if (state == TitanState.attack && attackAnimation == "jumper_0")
			{
				Vector3 velocity2 = baseRigidBody.velocity;
				Vector3 force2 = vector3 * 0.8f - velocity2;
				force2.x = Mathf.Clamp(force2.x, 0f - maxVelocityChange, maxVelocityChange);
				force2.z = Mathf.Clamp(force2.z, 0f - maxVelocityChange, maxVelocityChange);
				force2.y = 0f;
				baseRigidBody.AddForce(force2, ForceMode.VelocityChange);
			}
		}
		if ((abnormalType == AbnormalType.TYPE_I || abnormalType == AbnormalType.TYPE_JUMPER) && !nonAI && state == TitanState.attack && attackAnimation == "jumper_0")
		{
			Vector3 vector4 = baseTransform.forward * speed * myLevel * 0.5f;
			Vector3 velocity3 = baseRigidBody.velocity;
			if (baseAnimation["attack_jumper_0"].normalizedTime <= 0.28f || baseAnimation["attack_jumper_0"].normalizedTime >= 0.8f)
			{
				vector4 = Vector3.zero;
			}
			Vector3 force3 = vector4 - velocity3;
			force3.x = Mathf.Clamp(force3.x, 0f - maxVelocityChange, maxVelocityChange);
			force3.z = Mathf.Clamp(force3.z, 0f - maxVelocityChange, maxVelocityChange);
			force3.y = 0f;
			baseRigidBody.AddForce(force3, ForceMode.VelocityChange);
		}
		if (state != TitanState.chase && state != TitanState.wander && state != TitanState.to_check_point && state != TitanState.to_pvp_pt && state != TitanState.random_run)
		{
			return;
		}
		Vector3 vector5 = baseTransform.forward * speed;
		Vector3 velocity4 = baseRigidBody.velocity;
		Vector3 force4 = vector5 - velocity4;
		force4.x = Mathf.Clamp(force4.x, 0f - maxVelocityChange, maxVelocityChange);
		force4.z = Mathf.Clamp(force4.z, 0f - maxVelocityChange, maxVelocityChange);
		force4.y = 0f;
		baseRigidBody.AddForce(force4, ForceMode.VelocityChange);
		if (!stuck && abnormalType != AbnormalType.TYPE_CRAWLER && !nonAI)
		{
			if (baseAnimation.IsPlaying(runAnimation) && baseRigidBody.velocity.magnitude < speed * 0.5f)
			{
				stuck = true;
				stuckTime = 2f;
				stuckTurnAngle = (float)UnityEngine.Random.Range(0, 2) * 140f - 70f;
			}
			if (state == TitanState.chase && myHero != null && myDistance > attackDistance && myDistance < 150f)
			{
				float num = 0.05f;
				if (myDifficulty > 1)
				{
					num += 0.05f;
				}
				if (abnormalType != 0)
				{
					num += 0.1f;
				}
				if (UnityEngine.Random.Range(0f, 1f) < num)
				{
					stuck = true;
					stuckTime = 1f;
					float num2 = UnityEngine.Random.Range(20f, 50f);
					stuckTurnAngle = (float)UnityEngine.Random.Range(0, 2) * num2 * 2f - num2;
				}
			}
		}
		float num3 = 0f;
		if (state == TitanState.wander)
		{
			num3 = baseTransform.rotation.eulerAngles.y - 90f;
		}
		else if (state == TitanState.to_check_point || state == TitanState.to_pvp_pt || state == TitanState.random_run)
		{
			Vector3 vector6 = targetCheckPt - baseTransform.position;
			num3 = (0f - Mathf.Atan2(vector6.z, vector6.x)) * 57.29578f;
		}
		else
		{
			if (myHero == null)
			{
				return;
			}
			Vector3 vector7 = myHero.transform.position - baseTransform.position;
			num3 = (0f - Mathf.Atan2(vector7.z, vector7.x)) * 57.29578f;
		}
		if (stuck)
		{
			stuckTime -= Time.deltaTime;
			if (stuckTime < 0f)
			{
				stuck = false;
			}
			if (stuckTurnAngle > 0f)
			{
				stuckTurnAngle -= Time.deltaTime * 10f;
			}
			else
			{
				stuckTurnAngle += Time.deltaTime * 10f;
			}
			num3 += stuckTurnAngle;
		}
		float num4 = 0f - Mathf.DeltaAngle(num3, baseGameObjectTransform.rotation.eulerAngles.y - 90f);
		if (abnormalType == AbnormalType.TYPE_CRAWLER)
		{
			baseGameObjectTransform.rotation = Quaternion.Lerp(baseGameObjectTransform.rotation, Quaternion.Euler(0f, baseGameObjectTransform.rotation.eulerAngles.y + num4, 0f), speed * 0.3f * Time.deltaTime / myLevel);
		}
		else
		{
			baseGameObjectTransform.rotation = Quaternion.Lerp(baseGameObjectTransform.rotation, Quaternion.Euler(0f, baseGameObjectTransform.rotation.eulerAngles.y + num4, 0f), speed * 0.5f * Time.deltaTime / myLevel);
		}
	}

	private string[] GetAttackStrategy()
	{
		string[] array = null;
		int num = 0;
		if (isAlarm || myHero.transform.position.y + 3f <= neck.position.y + 10f * myLevel)
		{
			if (myHero.transform.position.y > neck.position.y - 3f * myLevel)
			{
				if (myDistance < attackDistance * 0.5f)
				{
					if (Vector3.Distance(myHero.transform.position, base.transform.Find("chkOverHead").position) < 3.6f * myLevel)
					{
						array = ((!(between2 > 0f)) ? new string[1] { "grab_head_front_l" } : new string[1] { "grab_head_front_r" });
					}
					else if (Mathf.Abs(between2) < 90f)
					{
						if (Mathf.Abs(between2) < 30f)
						{
							if (Vector3.Distance(myHero.transform.position, base.transform.Find("chkFront").position) < 2.5f * myLevel)
							{
								array = new string[3] { "attack_bite", "attack_bite", "attack_slap_face" };
							}
						}
						else if (between2 > 0f)
						{
							if (Vector3.Distance(myHero.transform.position, base.transform.Find("chkFrontRight").position) < 2.5f * myLevel)
							{
								array = new string[1] { "attack_bite_r" };
							}
						}
						else if (Vector3.Distance(myHero.transform.position, base.transform.Find("chkFrontLeft").position) < 2.5f * myLevel)
						{
							array = new string[1] { "attack_bite_l" };
						}
					}
					else if (between2 > 0f)
					{
						if (Vector3.Distance(myHero.transform.position, base.transform.Find("chkBackRight").position) < 2.8f * myLevel)
						{
							array = new string[3] { "grab_head_back_r", "grab_head_back_r", "attack_slap_back" };
						}
					}
					else if (Vector3.Distance(myHero.transform.position, base.transform.Find("chkBackLeft").position) < 2.8f * myLevel)
					{
						array = new string[3] { "grab_head_back_l", "grab_head_back_l", "attack_slap_back" };
					}
				}
				if (array != null)
				{
					return array;
				}
				if (abnormalType == AbnormalType.NORMAL || abnormalType == AbnormalType.TYPE_PUNK)
				{
					if (myDifficulty <= 0 && UnityEngine.Random.Range(0, 1000) >= 3)
					{
						return array;
					}
					if (Mathf.Abs(between2) >= 60f)
					{
						return array;
					}
					return new string[1] { "attack_combo" };
				}
				if (abnormalType != AbnormalType.TYPE_I && abnormalType != AbnormalType.TYPE_JUMPER)
				{
					return array;
				}
				if (myDifficulty > 0 || UnityEngine.Random.Range(0, 100) < 50)
				{
					return new string[1] { "attack_abnormal_jump" };
				}
				return array;
			}
			switch ((Mathf.Abs(between2) < 90f) ? ((between2 > 0f) ? 1 : 2) : ((!(between2 > 0f)) ? 3 : 4))
			{
			case 1:
				if (myDistance >= attackDistance * 0.25f)
				{
					if (myDistance < attackDistance * 0.5f)
					{
						if (abnormalType != AbnormalType.TYPE_PUNK && abnormalType == AbnormalType.NORMAL)
						{
							return new string[3] { "grab_ground_front_r", "grab_ground_front_r", "attack_stomp" };
						}
						return new string[3] { "grab_ground_front_r", "grab_ground_front_r", "attack_abnormal_jump" };
					}
					if (abnormalType != AbnormalType.TYPE_PUNK)
					{
						if (abnormalType == AbnormalType.NORMAL)
						{
							if (myDifficulty <= 0)
							{
								return new string[5] { "attack_front_ground", "attack_front_ground", "attack_front_ground", "attack_front_ground", "attack_combo" };
							}
							return new string[3] { "attack_front_ground", "attack_combo", "attack_combo" };
						}
						return new string[1] { "attack_abnormal_jump" };
					}
					return new string[3] { "attack_combo", "attack_combo", "attack_abnormal_jump" };
				}
				if (abnormalType != AbnormalType.TYPE_PUNK)
				{
					if (abnormalType != 0)
					{
						return new string[1] { "attack_kick" };
					}
					return new string[2] { "attack_front_ground", "attack_stomp" };
				}
				return new string[2] { "attack_kick", "attack_stomp" };
			case 2:
				if (myDistance >= attackDistance * 0.25f)
				{
					if (myDistance < attackDistance * 0.5f)
					{
						if (abnormalType != AbnormalType.TYPE_PUNK && abnormalType == AbnormalType.NORMAL)
						{
							return new string[3] { "grab_ground_front_l", "grab_ground_front_l", "attack_stomp" };
						}
						return new string[3] { "grab_ground_front_l", "grab_ground_front_l", "attack_abnormal_jump" };
					}
					if (abnormalType != AbnormalType.TYPE_PUNK)
					{
						if (abnormalType == AbnormalType.NORMAL)
						{
							if (myDifficulty <= 0)
							{
								return new string[5] { "attack_front_ground", "attack_front_ground", "attack_front_ground", "attack_front_ground", "attack_combo" };
							}
							return new string[3] { "attack_front_ground", "attack_combo", "attack_combo" };
						}
						return new string[1] { "attack_abnormal_jump" };
					}
					return new string[3] { "attack_combo", "attack_combo", "attack_abnormal_jump" };
				}
				if (abnormalType != AbnormalType.TYPE_PUNK)
				{
					if (abnormalType != 0)
					{
						return new string[1] { "attack_kick" };
					}
					return new string[2] { "attack_front_ground", "attack_stomp" };
				}
				return new string[2] { "attack_kick", "attack_stomp" };
			case 3:
			{
				if (myDistance >= attackDistance * 0.5f)
				{
					return array;
				}
				AbnormalType abnormalType2 = abnormalType;
				return new string[1] { "grab_ground_back_l" };
			}
			case 4:
			{
				if (myDistance >= attackDistance * 0.5f)
				{
					return array;
				}
				AbnormalType abnormalType3 = abnormalType;
				return new string[1] { "grab_ground_back_r" };
			}
			}
		}
		return array;
	}

	private void getDown()
	{
		state = TitanState.down;
		isAlarm = true;
		playAnimation("sit_hunt_down");
		getdownTime = UnityEngine.Random.Range(3f, 5f);
	}

	private GameObject getNearestHero2()
	{
		GameObject result = null;
		float num = float.PositiveInfinity;
		Vector3 position = baseTransform.position;
		foreach (HERO player in MultiplayerManager.getPlayers())
		{
			float num2 = Vector3.Distance(base.gameObject.transform.position, position);
			if (num2 < num)
			{
				result = player.gameObject;
				num = num2;
			}
		}
		foreach (TITAN_EREN eren in MultiplayerManager.getErens())
		{
			float num3 = Vector3.Distance(base.gameObject.transform.position, position);
			if (num3 < num)
			{
				result = eren.gameObject;
				num = num3;
			}
		}
		return result;
	}

	private int getPunkNumber()
	{
		int num = 0;
		GameObject[] array = GameObject.FindGameObjectsWithTag("titan");
		foreach (GameObject gameObject in array)
		{
			if (gameObject.GetComponent<TITAN>() != null && gameObject.GetComponent<TITAN>().name == "Punk")
			{
				num++;
			}
		}
		return num;
	}

	private void grab(string type)
	{
		state = TitanState.grab;
		attacked = false;
		isAlarm = true;
		attackAnimation = type;
		crossFade("grab_" + type, 0.1f);
		isGrabHandLeft = true;
		grabbedTarget = null;
		switch (type)
		{
		case "ground_back_l":
			attackCheckTimeA = 0.34f;
			attackCheckTimeB = 0.49f;
			break;
		case "ground_back_r":
			attackCheckTimeA = 0.34f;
			attackCheckTimeB = 0.49f;
			isGrabHandLeft = false;
			break;
		case "ground_front_l":
			attackCheckTimeA = 0.37f;
			attackCheckTimeB = 0.6f;
			break;
		case "ground_front_r":
			attackCheckTimeA = 0.37f;
			attackCheckTimeB = 0.6f;
			isGrabHandLeft = false;
			break;
		case "head_back_l":
			attackCheckTimeA = 0.45f;
			attackCheckTimeB = 0.5f;
			isGrabHandLeft = false;
			break;
		case "head_back_r":
			attackCheckTimeA = 0.45f;
			attackCheckTimeB = 0.5f;
			break;
		case "head_front_l":
			attackCheckTimeA = 0.38f;
			attackCheckTimeB = 0.55f;
			break;
		case "head_front_r":
			attackCheckTimeA = 0.38f;
			attackCheckTimeB = 0.55f;
			isGrabHandLeft = false;
			break;
		}
		if (isGrabHandLeft)
		{
			currentGrabHand = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L/forearm_L/hand_L/hand_L_001");
		}
		else
		{
			currentGrabHand = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R/hand_R_001");
		}
	}

	[RPC]
	public void grabbedTargetEscape(PhotonMessageInfo info)
	{
		if (info != null && info.sender != grabbedTarget.GetComponent<PhotonView>().owner && PhotonNetwork.isMasterClient)
		{
			FengGameManagerMKII.instance.kickPlayerRCIfMC(info.sender, true, "titan grabbedTargetEscape");
		}
		else
		{
			grabbedTarget = null;
		}
	}

	[RPC]
	public void grabToLeft(PhotonMessageInfo info)
	{
		if (PhotonNetwork.isMasterClient && info != null && info.sender != base.photonView.owner)
		{
			FengGameManagerMKII.instance.kickPlayerRCIfMC(info.sender, true, "titan grabToLeft");
			return;
		}
		Transform transform = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L/forearm_L/hand_L/hand_L_001");
		grabTF.transform.parent = transform;
		grabTF.transform.position = transform.GetComponent<SphereCollider>().transform.position;
		grabTF.transform.rotation = transform.GetComponent<SphereCollider>().transform.rotation;
		grabTF.transform.localPosition -= Vector3.right * transform.GetComponent<SphereCollider>().radius * 0.3f;
		grabTF.transform.localPosition -= Vector3.up * transform.GetComponent<SphereCollider>().radius * 0.51f;
		grabTF.transform.localPosition -= Vector3.forward * transform.GetComponent<SphereCollider>().radius * 0.3f;
		grabTF.transform.localRotation = Quaternion.Euler(grabTF.transform.localRotation.eulerAngles.x, grabTF.transform.localRotation.eulerAngles.y + 180f, grabTF.transform.localRotation.eulerAngles.z + 180f);
	}

	[RPC]
	public void grabToRight(PhotonMessageInfo info)
	{
		if (PhotonNetwork.isMasterClient && info != null && info.sender != base.photonView.owner)
		{
			FengGameManagerMKII.instance.kickPlayerRCIfMC(info.sender, true, "titan grabToLeft");
			return;
		}
		Transform transform = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R/hand_R_001");
		grabTF.transform.parent = transform;
		grabTF.transform.position = transform.GetComponent<SphereCollider>().transform.position;
		grabTF.transform.rotation = transform.GetComponent<SphereCollider>().transform.rotation;
		grabTF.transform.localPosition -= Vector3.right * transform.GetComponent<SphereCollider>().radius * 0.3f;
		grabTF.transform.localPosition += Vector3.up * transform.GetComponent<SphereCollider>().radius * 0.51f;
		grabTF.transform.localPosition -= Vector3.forward * transform.GetComponent<SphereCollider>().radius * 0.3f;
		grabTF.transform.localRotation = Quaternion.Euler(grabTF.transform.localRotation.eulerAngles.x, grabTF.transform.localRotation.eulerAngles.y + 180f, grabTF.transform.localRotation.eulerAngles.z);
	}

	public void headMovement()
	{
		if (!hasDie)
		{
			if (IN_GAME_MAIN_CAMERA.gametype != 0)
			{
				if (base.photonView.isMine)
				{
					targetHeadRotation = head.rotation;
					bool flag = false;
					if (abnormalType != AbnormalType.TYPE_CRAWLER && state != TitanState.attack && state != TitanState.down && state != TitanState.hit && state != TitanState.recover && state != TitanState.eat && state != TitanState.hit_eye && !hasDie && myDistance < 100f && myHero != null)
					{
						Vector3 vector = myHero.transform.position - base.transform.position;
						angle = (0f - Mathf.Atan2(vector.z, vector.x)) * 57.29578f;
						float value = 0f - Mathf.DeltaAngle(angle, base.transform.rotation.eulerAngles.y - 90f);
						value = Mathf.Clamp(value, -40f, 40f);
						float y = neck.position.y + myLevel * 2f - myHero.transform.position.y;
						float value2 = Mathf.Atan2(y, myDistance) * 57.29578f;
						value2 = Mathf.Clamp(value2, -40f, 30f);
						targetHeadRotation = Quaternion.Euler(head.rotation.eulerAngles.x + value2, head.rotation.eulerAngles.y + value, head.rotation.eulerAngles.z);
						if (!asClientLookTarget)
						{
							asClientLookTarget = true;
							object[] parameters = new object[1] { true };
							base.photonView.RPC("setIfLookTarget", PhotonTargets.Others, parameters);
						}
						flag = true;
					}
					if (!flag && asClientLookTarget)
					{
						asClientLookTarget = false;
						object[] parameters2 = new object[1] { false };
						base.photonView.RPC("setIfLookTarget", PhotonTargets.Others, parameters2);
					}
					if (state == TitanState.attack || state == TitanState.hit || state == TitanState.hit_eye)
					{
						oldHeadRotation = Quaternion.Lerp(oldHeadRotation, targetHeadRotation, Time.deltaTime * 20f);
					}
					else
					{
						oldHeadRotation = Quaternion.Lerp(oldHeadRotation, targetHeadRotation, Time.deltaTime * 10f);
					}
				}
				else
				{
					targetHeadRotation = head.rotation;
					if (asClientLookTarget && myHero != null)
					{
						Vector3 vector2 = myHero.transform.position - base.transform.position;
						angle = (0f - Mathf.Atan2(vector2.z, vector2.x)) * 57.29578f;
						float value3 = 0f - Mathf.DeltaAngle(angle, base.transform.rotation.eulerAngles.y - 90f);
						value3 = Mathf.Clamp(value3, -40f, 40f);
						float y2 = neck.position.y + myLevel * 2f - myHero.transform.position.y;
						float value4 = Mathf.Atan2(y2, myDistance) * 57.29578f;
						value4 = Mathf.Clamp(value4, -40f, 30f);
						targetHeadRotation = Quaternion.Euler(head.rotation.eulerAngles.x + value4, head.rotation.eulerAngles.y + value3, head.rotation.eulerAngles.z);
					}
					if (!hasDie)
					{
						oldHeadRotation = Quaternion.Lerp(oldHeadRotation, targetHeadRotation, Time.deltaTime * 10f);
					}
				}
			}
			else
			{
				targetHeadRotation = head.rotation;
				if (abnormalType != AbnormalType.TYPE_CRAWLER && state != TitanState.attack && state != TitanState.down && state != TitanState.hit && state != TitanState.recover && state != TitanState.hit_eye && !hasDie && myDistance < 100f && myHero != null)
				{
					Vector3 vector3 = myHero.transform.position - base.transform.position;
					angle = (0f - Mathf.Atan2(vector3.z, vector3.x)) * 57.29578f;
					float value5 = 0f - Mathf.DeltaAngle(angle, base.transform.rotation.eulerAngles.y - 90f);
					value5 = Mathf.Clamp(value5, -40f, 40f);
					float y3 = neck.position.y + myLevel * 2f - myHero.transform.position.y;
					float value6 = Mathf.Atan2(y3, myDistance) * 57.29578f;
					value6 = Mathf.Clamp(value6, -40f, 30f);
					targetHeadRotation = Quaternion.Euler(head.rotation.eulerAngles.x + value6, head.rotation.eulerAngles.y + value5, head.rotation.eulerAngles.z);
				}
				if (state == TitanState.attack || state == TitanState.hit || state == TitanState.hit_eye)
				{
					oldHeadRotation = Quaternion.Lerp(oldHeadRotation, targetHeadRotation, Time.deltaTime * 20f);
				}
				else
				{
					oldHeadRotation = Quaternion.Lerp(oldHeadRotation, targetHeadRotation, Time.deltaTime * 10f);
				}
			}
			head.rotation = oldHeadRotation;
		}
		if (!base.animation.IsPlaying("die_headOff"))
		{
			head.localScale = headscale;
		}
	}

	public void headMovement2()
	{
		if (!hasDie)
		{
			targetHeadRotation = head.rotation;
			if (!_ignoreLookTarget && abnormalType != AbnormalType.TYPE_CRAWLER && !hasDie && myDistance < 100f && myHero != null)
			{
				Vector3 vector = myHero.transform.position - base.transform.position;
				angle = (0f - Mathf.Atan2(vector.z, vector.x)) * 57.29578f;
				float value = 0f - Mathf.DeltaAngle(angle, base.transform.rotation.eulerAngles.y - 90f);
				value = Mathf.Clamp(value, -40f, 40f);
				float y = neck.position.y + myLevel * 2f - myHero.transform.position.y;
				float value2 = Mathf.Atan2(y, myDistance) * 57.29578f;
				value2 = Mathf.Clamp(value2, -40f, 30f);
				targetHeadRotation = Quaternion.Euler(head.rotation.eulerAngles.x + value2, head.rotation.eulerAngles.y + value, head.rotation.eulerAngles.z);
			}
			if (_fastHeadRotation)
			{
				oldHeadRotation = Quaternion.Lerp(oldHeadRotation, targetHeadRotation, Time.deltaTime * 20f);
			}
			else
			{
				oldHeadRotation = Quaternion.Lerp(oldHeadRotation, targetHeadRotation, Time.deltaTime * 10f);
			}
			head.rotation = oldHeadRotation;
		}
		if (!base.animation.IsPlaying("die_headOff"))
		{
			head.localScale = headscale;
		}
	}

	private void hit(string animationName, Vector3 attacker, float hitPauseTime)
	{
		state = TitanState.hit;
		hitAnimation = animationName;
		hitPause = hitPauseTime;
		playAnimation(hitAnimation);
		base.animation[hitAnimation].time = 0f;
		base.animation[hitAnimation].speed = 0f;
		base.transform.rotation = Quaternion.Euler(0f, Quaternion.LookRotation(attacker - base.transform.position).eulerAngles.y, 0f);
		needFreshCorePosition = true;
		if (base.photonView.isMine && grabbedTarget != null)
		{
			grabbedTarget.GetPhotonView().RPC("netUngrabbed", PhotonTargets.All);
		}
	}

	public void hitAnkle()
	{
		if (!hasDie && state != TitanState.down)
		{
			if (grabbedTarget != null)
			{
				grabbedTarget.GetPhotonView().RPC("netUngrabbed", PhotonTargets.All);
			}
			getDown();
		}
	}

	[RPC]
	public void hitAnkleRPC(int viewID)
	{
		if (hasDie || state == TitanState.down)
		{
			return;
		}
		PhotonView photonView = PhotonView.Find(viewID);
		if (photonView != null && (photonView.gameObject.transform.position - base.transform.position).magnitude < 20f)
		{
			if (base.photonView.isMine && grabbedTarget != null)
			{
				grabbedTarget.GetPhotonView().RPC("netUngrabbed", PhotonTargets.All);
			}
			getDown();
		}
	}

	public void hitEye()
	{
		if (!hasDie)
		{
			justHitEye();
		}
	}

	[RPC]
	public void hitEyeRPC(int viewID)
	{
		if (!hasDie && (PhotonView.Find(viewID).gameObject.transform.position - neck.position).magnitude < 20f)
		{
			if (base.photonView.isMine && grabbedTarget != null)
			{
				grabbedTarget.GetPhotonView().RPC("netUngrabbed", PhotonTargets.All);
			}
			if (!hasDie)
			{
				justHitEye();
			}
		}
	}

	public void hitL(Vector3 attacker, float hitPauseTime)
	{
		if (abnormalType != AbnormalType.TYPE_CRAWLER)
		{
			if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
			{
				hit("hit_eren_L", attacker, hitPauseTime);
				return;
			}
			object[] parameters = new object[2] { attacker, hitPauseTime };
			base.photonView.RPC("hitLRPC", PhotonTargets.All, parameters);
		}
	}

	[RPC]
	private void hitLRPC(Vector3 attacker, float hitPauseTime)
	{
		if (base.photonView.isMine && (attacker - base.transform.position).magnitude < 80f)
		{
			hit("hit_eren_L", attacker, hitPauseTime);
		}
	}

	public void hitR(Vector3 attacker, float hitPauseTime)
	{
		if (abnormalType != AbnormalType.TYPE_CRAWLER)
		{
			if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
			{
				hit("hit_eren_R", attacker, hitPauseTime);
				return;
			}
			object[] parameters = new object[2] { attacker, hitPauseTime };
			base.photonView.RPC("hitRRPC", PhotonTargets.All, parameters);
		}
	}

	[RPC]
	private void hitRRPC(Vector3 attacker, float hitPauseTime)
	{
		if (base.photonView.isMine && !hasDie && (attacker - base.transform.position).magnitude < 80f)
		{
			hit("hit_eren_R", attacker, hitPauseTime);
		}
	}

	private void idle(float sbtime = 0f)
	{
		stuck = false;
		this.sbtime = sbtime;
		if (myDifficulty == 2 && (abnormalType == AbnormalType.TYPE_JUMPER || abnormalType == AbnormalType.TYPE_I))
		{
			this.sbtime = UnityEngine.Random.Range(0f, 1.5f);
		}
		else if (myDifficulty >= 1)
		{
			this.sbtime = 0f;
		}
		this.sbtime = Mathf.Max(0.5f, this.sbtime);
		if (abnormalType == AbnormalType.TYPE_PUNK)
		{
			this.sbtime = 0.1f;
			if (myDifficulty == 1)
			{
				this.sbtime += 0.4f;
			}
		}
		state = TitanState.idle;
		if (abnormalType == AbnormalType.TYPE_CRAWLER)
		{
			crossFadeIfNotPlaying("crawler_idle", 0.2f);
		}
		else
		{
			crossFadeIfNotPlaying("idle", 0.2f);
		}
	}

	public bool IsGrounded()
	{
		LayerMask layerMask = 1 << LayerMask.NameToLayer("Ground");
		LayerMask layerMask2 = 1 << LayerMask.NameToLayer("EnemyAABB");
		LayerMask layerMask3 = (int)layerMask2 | (int)layerMask;
		return Physics.Raycast(base.gameObject.transform.position + Vector3.up * 0.1f, -Vector3.up, 0.3f, layerMask3.value);
	}

	private void justEatHero(GameObject target, Transform hand)
	{
		if (!(target != null))
		{
			return;
		}
		if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && base.photonView.isMine)
		{
			if (!target.GetComponent<HERO>().HasDied())
			{
				target.GetComponent<HERO>().markDie();
				if (nonAI)
				{
					object[] parameters = new object[2]
					{
						base.photonView.viewID,
						base.name
					};
					target.GetComponent<HERO>().photonView.RPC("netDie2", PhotonTargets.All, parameters);
				}
				else
				{
					object[] parameters2 = new object[2] { -1, base.name };
					target.GetComponent<HERO>().photonView.RPC("netDie2", PhotonTargets.All, parameters2);
				}
			}
		}
		else if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
		{
			target.GetComponent<HERO>().die2(hand);
		}
	}

	private void justHitEye()
	{
		if (state != TitanState.hit_eye)
		{
			if (state == TitanState.down || state == TitanState.sit)
			{
				playAnimation("sit_hit_eye");
			}
			else
			{
				playAnimation("hit_eye");
			}
			state = TitanState.hit_eye;
		}
	}

	[RPC]
	public void labelRPC(int health, int maxHealth)
	{
		if (health < 0)
		{
			if (healthLabel != null)
			{
				UnityEngine.Object.Destroy(healthLabel);
			}
			return;
		}
		if (healthLabel == null)
		{
			healthLabel = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("UI/LabelNameOverHead"));
			healthLabel.name = "LabelNameOverHead";
			healthLabel.transform.parent = base.transform;
			healthLabel.transform.localPosition = new Vector3(0f, 20f + 1f / myLevel, 0f);
			if (abnormalType == AbnormalType.TYPE_CRAWLER)
			{
				healthLabel.transform.localPosition = new Vector3(0f, 10f + 1f / myLevel, 0f);
			}
			float num = 1f;
			if (myLevel < 1f)
			{
				num = 1f / myLevel;
			}
			healthLabel.transform.localScale = new Vector3(num, num, num);
			healthLabelEnabled = true;
		}
		string text = "[7FFF00]";
		float num2 = (float)health / (float)maxHealth;
		if (num2 < 0.75f && num2 >= 0.5f)
		{
			text = "[f2b50f]";
		}
		else if (num2 < 0.5f && num2 >= 0.25f)
		{
			text = "[ff8100]";
		}
		else if (num2 < 0.25f)
		{
			text = "[ff3333]";
		}
		healthLabel.GetComponent<UILabel>().text = text + Convert.ToString(health);
	}

	public void lateUpdate()
	{
		if (GameMenu.Paused && IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
		{
			return;
		}
		if (base.animation.IsPlaying("run_walk"))
		{
			if (base.animation["run_walk"].normalizedTime % 1f > 0.1f && base.animation["run_walk"].normalizedTime % 1f < 0.6f && stepSoundPhase == 2)
			{
				stepSoundPhase = 1;
				Transform transform = base.transform.Find("snd_titan_foot");
				transform.GetComponent<AudioSource>().Stop();
				transform.GetComponent<AudioSource>().Play();
			}
			if (base.animation["run_walk"].normalizedTime % 1f > 0.6f && stepSoundPhase == 1)
			{
				stepSoundPhase = 2;
				Transform transform2 = base.transform.Find("snd_titan_foot");
				transform2.GetComponent<AudioSource>().Stop();
				transform2.GetComponent<AudioSource>().Play();
			}
		}
		if (base.animation.IsPlaying("crawler_run"))
		{
			if (base.animation["crawler_run"].normalizedTime % 1f > 0.1f && base.animation["crawler_run"].normalizedTime % 1f < 0.56f && stepSoundPhase == 2)
			{
				stepSoundPhase = 1;
				Transform transform3 = base.transform.Find("snd_titan_foot");
				transform3.GetComponent<AudioSource>().Stop();
				transform3.GetComponent<AudioSource>().Play();
			}
			if (base.animation["crawler_run"].normalizedTime % 1f > 0.56f && stepSoundPhase == 1)
			{
				stepSoundPhase = 2;
				Transform transform4 = base.transform.Find("snd_titan_foot");
				transform4.GetComponent<AudioSource>().Stop();
				transform4.GetComponent<AudioSource>().Play();
			}
		}
		if (base.animation.IsPlaying("run_abnormal"))
		{
			if (base.animation["run_abnormal"].normalizedTime % 1f > 0.47f && base.animation["run_abnormal"].normalizedTime % 1f < 0.95f && stepSoundPhase == 2)
			{
				stepSoundPhase = 1;
				Transform transform5 = base.transform.Find("snd_titan_foot");
				transform5.GetComponent<AudioSource>().Stop();
				transform5.GetComponent<AudioSource>().Play();
			}
			if ((base.animation["run_abnormal"].normalizedTime % 1f > 0.95f || base.animation["run_abnormal"].normalizedTime % 1f < 0.47f) && stepSoundPhase == 1)
			{
				stepSoundPhase = 2;
				Transform transform6 = base.transform.Find("snd_titan_foot");
				transform6.GetComponent<AudioSource>().Stop();
				transform6.GetComponent<AudioSource>().Play();
			}
		}
		headMovement();
		grounded = false;
	}

	public void lateUpdate2()
	{
		if (GameMenu.Paused && IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
		{
			return;
		}
		if (baseAnimation.IsPlaying("run_walk"))
		{
			if (baseAnimation["run_walk"].normalizedTime % 1f > 0.1f && baseAnimation["run_walk"].normalizedTime % 1f < 0.6f && stepSoundPhase == 2)
			{
				stepSoundPhase = 1;
				baseAudioSource.Stop();
				baseAudioSource.Play();
			}
			else if (baseAnimation["run_walk"].normalizedTime % 1f > 0.6f && stepSoundPhase == 1)
			{
				stepSoundPhase = 2;
				baseAudioSource.Stop();
				baseAudioSource.Play();
			}
		}
		else if (baseAnimation.IsPlaying("crawler_run"))
		{
			if (baseAnimation["crawler_run"].normalizedTime % 1f > 0.1f && baseAnimation["crawler_run"].normalizedTime % 1f < 0.56f && stepSoundPhase == 2)
			{
				stepSoundPhase = 1;
				baseAudioSource.Stop();
				baseAudioSource.Play();
			}
			else if (baseAnimation["crawler_run"].normalizedTime % 1f > 0.56f && stepSoundPhase == 1)
			{
				stepSoundPhase = 2;
				baseAudioSource.Stop();
				baseAudioSource.Play();
			}
		}
		else if (baseAnimation.IsPlaying("run_abnormal"))
		{
			if (baseAnimation["run_abnormal"].normalizedTime % 1f > 0.47f && baseAnimation["run_abnormal"].normalizedTime % 1f < 0.95f && stepSoundPhase == 2)
			{
				stepSoundPhase = 1;
				baseAudioSource.Stop();
				baseAudioSource.Play();
			}
			else if ((baseAnimation["run_abnormal"].normalizedTime % 1f > 0.95f || baseAnimation["run_abnormal"].normalizedTime % 1f < 0.47f) && stepSoundPhase == 1)
			{
				stepSoundPhase = 2;
				baseAudioSource.Stop();
				baseAudioSource.Play();
			}
		}
		headMovement2();
		grounded = false;
		updateLabel();
		updateCollider();
	}

	[RPC]
	private void laugh(float sbtime = 0f)
	{
		if (state == TitanState.idle || state == TitanState.turn || state == TitanState.chase)
		{
			this.sbtime = sbtime;
			state = TitanState.laugh;
			crossFade("laugh", 0.2f);
		}
	}

	public void loadskin()
	{
		skin = 0;
		eye = false;
		BaseCustomSkinSettings<TitanCustomSkinSet> titan = SettingsManager.CustomSkinSettings.Titan;
		if ((IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE || base.photonView.isMine) && titan.SkinsEnabled.Value)
		{
			int num = (int)UnityEngine.Random.Range(0f, 5f);
			int index = num;
			TitanCustomSkinSet titanCustomSkinSet = (TitanCustomSkinSet)titan.GetSelectedSet();
			if (titanCustomSkinSet.RandomizedPairs.Value)
			{
				index = (int)UnityEngine.Random.Range(0f, 5f);
			}
			string value = ((StringSetting)titanCustomSkinSet.Bodies.GetItemAt(num)).Value;
			string value2 = ((StringSetting)titanCustomSkinSet.Eyes.GetItemAt(index)).Value;
			skin = num;
			if (value2.EndsWith(".jpg") || value2.EndsWith(".png") || value2.EndsWith(".jpeg"))
			{
				eye = true;
			}
			GetComponent<TITAN_SETUP>().setVar(skin, eye);
			if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
			{
				StartCoroutine(loadskinE(value, value2));
				return;
			}
			base.photonView.RPC("loadskinRPC", PhotonTargets.AllBuffered, value, value2);
		}
	}

	public IEnumerator loadskinE(string body, string eye)
	{
		while (!hasSpawn)
		{
			yield return null;
		}
		yield return StartCoroutine(_customSkinLoader.LoadSkinsFromRPC(new object[3] { false, body, eye }));
	}

	[RPC]
	public void loadskinRPC(string body, string eye, PhotonMessageInfo info)
	{
		if (info.sender == base.photonView.owner)
		{
			BaseCustomSkinSettings<TitanCustomSkinSet> titan = SettingsManager.CustomSkinSettings.Titan;
			if (titan.SkinsEnabled.Value && (!titan.SkinsLocal.Value || base.photonView.isMine))
			{
				StartCoroutine(loadskinE(body, eye));
			}
		}
	}

	private bool longRangeAttackCheck()
	{
		if (abnormalType == AbnormalType.TYPE_PUNK && myHero != null && myHero.rigidbody != null)
		{
			Vector3 vector = myHero.rigidbody.velocity * Time.deltaTime * 30f;
			if (vector.sqrMagnitude > 10f)
			{
				if (simpleHitTestLineAndBall(vector, base.transform.Find("chkAeLeft").position - myHero.transform.position, 5f * myLevel))
				{
					attack("anti_AE_l");
					return true;
				}
				if (simpleHitTestLineAndBall(vector, base.transform.Find("chkAeLLeft").position - myHero.transform.position, 5f * myLevel))
				{
					attack("anti_AE_low_l");
					return true;
				}
				if (simpleHitTestLineAndBall(vector, base.transform.Find("chkAeRight").position - myHero.transform.position, 5f * myLevel))
				{
					attack("anti_AE_r");
					return true;
				}
				if (simpleHitTestLineAndBall(vector, base.transform.Find("chkAeLRight").position - myHero.transform.position, 5f * myLevel))
				{
					attack("anti_AE_low_r");
					return true;
				}
			}
			Vector3 vector2 = myHero.transform.position - base.transform.position;
			float current = (0f - Mathf.Atan2(vector2.z, vector2.x)) * 57.29578f;
			float f = 0f - Mathf.DeltaAngle(current, base.gameObject.transform.rotation.eulerAngles.y - 90f);
			if (rockInterval > 0f)
			{
				rockInterval -= Time.deltaTime;
			}
			else if (Mathf.Abs(f) < 5f)
			{
				Vector3 vector3 = myHero.transform.position + vector;
				float sqrMagnitude = (vector3 - base.transform.position).sqrMagnitude;
				if (sqrMagnitude > 8000f && sqrMagnitude < 90000f)
				{
					attack("throw");
					rockInterval = 2f;
					return true;
				}
			}
		}
		return false;
	}

	private bool longRangeAttackCheck2()
	{
		if (abnormalType == AbnormalType.TYPE_PUNK && myHero != null)
		{
			Vector3 vector = myHero.rigidbody.velocity * Time.deltaTime * 30f;
			if (vector.sqrMagnitude > 10f)
			{
				if (simpleHitTestLineAndBall(vector, baseTransform.Find("chkAeLeft").position - myHero.transform.position, 5f * myLevel))
				{
					attack2("anti_AE_l");
					return true;
				}
				if (simpleHitTestLineAndBall(vector, baseTransform.Find("chkAeLLeft").position - myHero.transform.position, 5f * myLevel))
				{
					attack2("anti_AE_low_l");
					return true;
				}
				if (simpleHitTestLineAndBall(vector, baseTransform.Find("chkAeRight").position - myHero.transform.position, 5f * myLevel))
				{
					attack2("anti_AE_r");
					return true;
				}
				if (simpleHitTestLineAndBall(vector, baseTransform.Find("chkAeLRight").position - myHero.transform.position, 5f * myLevel))
				{
					attack2("anti_AE_low_r");
					return true;
				}
			}
			Vector3 vector2 = myHero.transform.position - baseTransform.position;
			float current = (0f - Mathf.Atan2(vector2.z, vector2.x)) * 57.29578f;
			float f = 0f - Mathf.DeltaAngle(current, baseGameObjectTransform.rotation.eulerAngles.y - 90f);
			if (rockInterval > 0f)
			{
				rockInterval -= Time.deltaTime;
			}
			else if (Mathf.Abs(f) < 5f)
			{
				Vector3 vector3 = myHero.transform.position + vector;
				float sqrMagnitude = (vector3 - baseTransform.position).sqrMagnitude;
				if (sqrMagnitude > 8000f && sqrMagnitude < 90000f && SettingsManager.LegacyGameSettings.RockThrowEnabled.Value)
				{
					attack2("throw");
					rockInterval = 2f;
					return true;
				}
			}
		}
		return false;
	}

	public void moveTo(float posX, float posY, float posZ)
	{
		base.transform.position = new Vector3(posX, posY, posZ);
	}

	[RPC]
	public void moveToRPC(float posX, float posY, float posZ, PhotonMessageInfo info)
	{
		if (info.sender.isMasterClient)
		{
			base.transform.position = new Vector3(posX, posY, posZ);
		}
	}

	[RPC]
	private void netCrossFade(string aniName, float time)
	{
		base.animation.CrossFade(aniName, time);
		CheckAnimationLookTarget(aniName);
	}

	[RPC]
	private void netDie()
	{
		asClientLookTarget = false;
		if (!hasDie)
		{
			hasDie = true;
			if (nonAI)
			{
				currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(null);
				currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().setSpectorMode(true);
				currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
				ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
				hashtable.Add(PhotonPlayerProperty.dead, true);
				PhotonNetwork.player.SetCustomProperties(hashtable);
				hashtable = new ExitGames.Client.Photon.Hashtable();
				hashtable.Add(PhotonPlayerProperty.deaths, (int)PhotonNetwork.player.customProperties[PhotonPlayerProperty.deaths] + 1);
				PhotonNetwork.player.SetCustomProperties(hashtable);
			}
			dieAnimation();
		}
	}

	[RPC]
	private void netPlayAnimation(string aniName)
	{
		base.animation.Play(aniName);
		CheckAnimationLookTarget(aniName);
	}

	[RPC]
	private void netPlayAnimationAt(string aniName, float normalizedTime)
	{
		base.animation.Play(aniName);
		CheckAnimationLookTarget(aniName);
		base.animation[aniName].normalizedTime = normalizedTime;
	}

	[RPC]
	private void netSetAbnormalType(int type)
	{
		if (!hasload)
		{
			hasload = true;
			loadskin();
		}
		switch (type)
		{
		case 0:
			abnormalType = AbnormalType.NORMAL;
			base.name = "Titan";
			runAnimation = "run_walk";
			GetComponent<TITAN_SETUP>().setHair2();
			break;
		case 1:
			abnormalType = AbnormalType.TYPE_I;
			base.name = "Aberrant";
			runAnimation = "run_abnormal";
			GetComponent<TITAN_SETUP>().setHair2();
			break;
		case 2:
			abnormalType = AbnormalType.TYPE_JUMPER;
			base.name = "Jumper";
			runAnimation = "run_abnormal";
			GetComponent<TITAN_SETUP>().setHair2();
			break;
		case 3:
			abnormalType = AbnormalType.TYPE_CRAWLER;
			base.name = "Crawler";
			runAnimation = "crawler_run";
			GetComponent<TITAN_SETUP>().setHair2();
			break;
		case 4:
			abnormalType = AbnormalType.TYPE_PUNK;
			base.name = "Punk";
			runAnimation = "run_abnormal_1";
			GetComponent<TITAN_SETUP>().setHair2();
			break;
		}
		if (abnormalType == AbnormalType.TYPE_I || abnormalType == AbnormalType.TYPE_JUMPER || abnormalType == AbnormalType.TYPE_PUNK)
		{
			speed = 18f;
			if (myLevel > 1f)
			{
				speed *= Mathf.Sqrt(myLevel);
			}
			if (myDifficulty == 1)
			{
				speed *= 1.4f;
			}
			if (myDifficulty == 2)
			{
				speed *= 1.6f;
			}
			baseAnimation["turnaround1"].speed = 2f;
			baseAnimation["turnaround2"].speed = 2f;
		}
		if (abnormalType == AbnormalType.TYPE_CRAWLER)
		{
			chaseDistance += 50f;
			speed = 25f;
			if (myLevel > 1f)
			{
				speed *= Mathf.Sqrt(myLevel);
			}
			if (myDifficulty == 1)
			{
				speed *= 2f;
			}
			if (myDifficulty == 2)
			{
				speed *= 2.2f;
			}
			baseTransform.Find("AABB").gameObject.GetComponent<CapsuleCollider>().height = 10f;
			baseTransform.Find("AABB").gameObject.GetComponent<CapsuleCollider>().radius = 5f;
			baseTransform.Find("AABB").gameObject.GetComponent<CapsuleCollider>().center = new Vector3(0f, 5.05f, 0f);
		}
		if (nonAI)
		{
			if (abnormalType == AbnormalType.TYPE_CRAWLER)
			{
				speed = Mathf.Min(70f, speed);
			}
			else
			{
				speed = Mathf.Min(60f, speed);
			}
			baseAnimation["attack_jumper_0"].speed = 7f;
			baseAnimation["attack_crawler_jump_0"].speed = 4f;
		}
		baseAnimation["attack_combo_1"].speed = 1f;
		baseAnimation["attack_combo_2"].speed = 1f;
		baseAnimation["attack_combo_3"].speed = 1f;
		baseAnimation["attack_quick_turn_l"].speed = 1f;
		baseAnimation["attack_quick_turn_r"].speed = 1f;
		baseAnimation["attack_anti_AE_l"].speed = 1.1f;
		baseAnimation["attack_anti_AE_low_l"].speed = 1.1f;
		baseAnimation["attack_anti_AE_r"].speed = 1.1f;
		baseAnimation["attack_anti_AE_low_r"].speed = 1.1f;
		idle();
	}

	[RPC]
	private void netSetLevel(float level, int AI, int skinColor, PhotonMessageInfo info)
	{
		if (!info.sender.isMasterClient && !info.sender.isLocal && base.photonView.owner != info.sender)
		{
			FengGameManagerMKII.instance.kickPlayerRCIfMC(info.sender, true, "titan netSetLevel");
		}
		setLevel2(level, AI, skinColor);
		if (level > 5f)
		{
			headscale = new Vector3(1f, 1f, 1f);
		}
		else if (level < 1f && FengGameManagerMKII.level.StartsWith("Custom"))
		{
			myTitanTrigger.GetComponent<CapsuleCollider>().radius *= 2.5f - level;
		}
	}

	private void OnCollisionStay()
	{
		grounded = true;
	}

	private void OnDestroy()
	{
		if (GameObject.Find("MultiplayerManager") != null)
		{
			GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().removeTitan(this);
		}
	}

	public void OnTitanDie(PhotonView view)
	{
		if (FengGameManagerMKII.logicLoaded && FengGameManagerMKII.RCEvents.ContainsKey("OnTitanDie"))
		{
			RCEvent rCEvent = (RCEvent)FengGameManagerMKII.RCEvents["OnTitanDie"];
			string[] array = (string[])FengGameManagerMKII.RCVariableNames["OnTitanDie"];
			if (FengGameManagerMKII.titanVariables.ContainsKey(array[0]))
			{
				FengGameManagerMKII.titanVariables[array[0]] = this;
			}
			else
			{
				FengGameManagerMKII.titanVariables.Add(array[0], this);
			}
			if (FengGameManagerMKII.playerVariables.ContainsKey(array[1]))
			{
				FengGameManagerMKII.playerVariables[array[1]] = view.owner;
			}
			else
			{
				FengGameManagerMKII.playerVariables.Add(array[1], view.owner);
			}
			rCEvent.checkEvent();
		}
	}

	private void playAnimation(string aniName)
	{
		base.animation.Play(aniName);
		CheckAnimationLookTarget(aniName);
		if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && base.photonView.isMine)
		{
			object[] parameters = new object[1] { aniName };
			base.photonView.RPC("netPlayAnimation", PhotonTargets.Others, parameters);
		}
	}

	private void playAnimationAt(string aniName, float normalizedTime)
	{
		base.animation.Play(aniName);
		CheckAnimationLookTarget(aniName);
		base.animation[aniName].normalizedTime = normalizedTime;
		if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && base.photonView.isMine)
		{
			object[] parameters = new object[2] { aniName, normalizedTime };
			base.photonView.RPC("netPlayAnimationAt", PhotonTargets.Others, parameters);
		}
	}

	private void playSound(string sndname)
	{
		playsoundRPC(sndname);
		if (base.photonView.isMine)
		{
			object[] parameters = new object[1] { sndname };
			base.photonView.RPC("playsoundRPC", PhotonTargets.Others, parameters);
		}
	}

	[RPC]
	private void playsoundRPC(string sndname)
	{
		base.transform.Find(sndname).GetComponent<AudioSource>().Play();
	}

	public void pt()
	{
		if (controller.bite)
		{
			attack2("bite");
		}
		if (controller.bitel)
		{
			attack2("bite_l");
		}
		if (controller.biter)
		{
			attack2("bite_r");
		}
		if (controller.chopl)
		{
			attack2("anti_AE_low_l");
		}
		if (controller.chopr)
		{
			attack2("anti_AE_low_r");
		}
		if (controller.choptl)
		{
			attack2("anti_AE_l");
		}
		if (controller.choptr)
		{
			attack2("anti_AE_r");
		}
		if (controller.cover && stamina > 75f)
		{
			recoverpt();
			stamina -= 75f;
		}
		if (controller.grabbackl)
		{
			grab("ground_back_l");
		}
		if (controller.grabbackr)
		{
			grab("ground_back_r");
		}
		if (controller.grabfrontl)
		{
			grab("ground_front_l");
		}
		if (controller.grabfrontr)
		{
			grab("ground_front_r");
		}
		if (controller.grabnapel)
		{
			grab("head_back_l");
		}
		if (controller.grabnaper)
		{
			grab("head_back_r");
		}
	}

	public void randomRun(Vector3 targetPt, float r)
	{
		state = TitanState.random_run;
		targetCheckPt = targetPt;
		targetR = r;
		random_run_time = UnityEngine.Random.Range(1f, 2f);
		crossFade(runAnimation, 0.5f);
	}

	private void recover()
	{
		state = TitanState.recover;
		playAnimation("idle_recovery");
		getdownTime = UnityEngine.Random.Range(2f, 5f);
	}

	private void recoverpt()
	{
		state = TitanState.recover;
		playAnimation("idle_recovery");
		getdownTime = UnityEngine.Random.Range(1.8f, 2.5f);
	}

	private void remainSitdown()
	{
		state = TitanState.sit;
		playAnimation("sit_idle");
		getdownTime = UnityEngine.Random.Range(10f, 30f);
	}

	public void resetLevel(float level)
	{
		myLevel = level;
		setmyLevel();
	}

	public void setAbnormalType(AbnormalType type, bool forceCrawler = false)
	{
		int num = 0;
		float num2 = 0.02f * (float)(IN_GAME_MAIN_CAMERA.difficulty + 1);
		if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.PVP_AHSS)
		{
			num2 = 100f;
		}
		switch (type)
		{
		case AbnormalType.NORMAL:
			num = ((UnityEngine.Random.Range(0f, 1f) < num2) ? 4 : 0);
			break;
		case AbnormalType.TYPE_I:
			num = ((!(UnityEngine.Random.Range(0f, 1f) < num2)) ? 1 : 4);
			break;
		case AbnormalType.TYPE_JUMPER:
			num = ((!(UnityEngine.Random.Range(0f, 1f) < num2)) ? 2 : 4);
			break;
		case AbnormalType.TYPE_CRAWLER:
			num = 3;
			if (GameObject.Find("Crawler") != null && UnityEngine.Random.Range(0, 1000) > 5)
			{
				num = 2;
			}
			break;
		case AbnormalType.TYPE_PUNK:
			num = 4;
			break;
		}
		if (forceCrawler)
		{
			num = 3;
		}
		if (num == 4)
		{
			if (!LevelInfo.getInfo(FengGameManagerMKII.level).punk)
			{
				num = 1;
			}
			else
			{
				if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE && getPunkNumber() >= 3)
				{
					num = 1;
				}
				if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.SURVIVE_MODE)
				{
					int wave = GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().wave;
					if (wave != 5 && wave != 10 && wave != 15 && wave != 20)
					{
						num = 1;
					}
				}
			}
		}
		if (IN_GAME_MAIN_CAMERA.gametype != 0 && base.photonView.isMine)
		{
			object[] parameters = new object[1] { num };
			base.photonView.RPC("netSetAbnormalType", PhotonTargets.AllBuffered, parameters);
		}
		else if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
		{
			netSetAbnormalType(num);
		}
	}

	public void setAbnormalType2(AbnormalType type, bool forceCrawler)
	{
		bool flag = false;
		if (SettingsManager.LegacyGameSettings.TitanSpawnEnabled.Value)
		{
			flag = true;
		}
		if (FengGameManagerMKII.level.StartsWith("Custom"))
		{
			flag = true;
		}
		int num = 0;
		float num2 = 0.02f * (float)(IN_GAME_MAIN_CAMERA.difficulty + 1);
		if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.PVP_AHSS)
		{
			num2 = 100f;
		}
		switch (type)
		{
		case AbnormalType.NORMAL:
			num = ((UnityEngine.Random.Range(0f, 1f) < num2) ? 4 : 0);
			if (flag)
			{
				num = 0;
			}
			break;
		case AbnormalType.TYPE_I:
			num = ((!(UnityEngine.Random.Range(0f, 1f) < num2)) ? 1 : 4);
			if (flag)
			{
				num = 1;
			}
			break;
		case AbnormalType.TYPE_JUMPER:
			num = ((!(UnityEngine.Random.Range(0f, 1f) < num2)) ? 2 : 4);
			if (flag)
			{
				num = 2;
			}
			break;
		case AbnormalType.TYPE_CRAWLER:
			num = 3;
			if (GameObject.Find("Crawler") != null && UnityEngine.Random.Range(0, 1000) > 5)
			{
				num = 2;
			}
			if (flag)
			{
				num = 3;
			}
			break;
		case AbnormalType.TYPE_PUNK:
			num = 4;
			break;
		}
		if (forceCrawler)
		{
			num = 3;
		}
		if (num == 4)
		{
			if (!LevelInfo.getInfo(FengGameManagerMKII.level).punk)
			{
				num = 1;
			}
			else
			{
				if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE && getPunkNumber() >= 3)
				{
					num = 1;
				}
				if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.SURVIVE_MODE)
				{
					int wave = FengGameManagerMKII.instance.wave;
					if (wave != 5 && wave != 10 && wave != 15 && wave != 20)
					{
						num = 1;
					}
				}
			}
			if (flag)
			{
				num = 4;
			}
		}
		if (IN_GAME_MAIN_CAMERA.gametype != 0 && base.photonView.isMine)
		{
			object[] parameters = new object[1] { num };
			base.photonView.RPC("netSetAbnormalType", PhotonTargets.AllBuffered, parameters);
		}
		else if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
		{
			netSetAbnormalType(num);
		}
	}

	[RPC]
	private void setIfLookTarget(bool bo)
	{
		asClientLookTarget = bo;
	}

	private void setLevel(float level, int AI, int skinColor)
	{
		myLevel = level;
		myLevel = Mathf.Clamp(myLevel, 0.7f, 3f);
		attackWait += UnityEngine.Random.Range(0f, 2f);
		chaseDistance += myLevel * 10f;
		base.transform.localScale = new Vector3(myLevel, myLevel, myLevel);
		float num = Mathf.Min(Mathf.Pow(2f / myLevel, 0.35f), 1.25f);
		headscale = new Vector3(num, num, num);
		head = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head");
		head.localScale = headscale;
		if (skinColor != 0)
		{
			Material material = mainMaterial.GetComponent<SkinnedMeshRenderer>().material;
			Color color;
			switch (skinColor)
			{
			case 1:
				color = FengColor.titanSkin1;
				break;
			case 2:
				color = FengColor.titanSkin2;
				break;
			default:
				color = FengColor.titanSkin3;
				break;
			}
			material.color = color;
		}
		float value = 1.4f - (myLevel - 0.7f) * 0.15f;
		value = Mathf.Clamp(value, 0.9f, 1.5f);
		foreach (AnimationState item in base.animation)
		{
			item.speed = value;
		}
		base.rigidbody.mass *= myLevel;
		base.rigidbody.rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0, 360), 0f);
		if (myLevel > 1f)
		{
			speed *= Mathf.Sqrt(myLevel);
		}
		myDifficulty = AI;
		if (myDifficulty == 1 || myDifficulty == 2)
		{
			foreach (AnimationState item2 in base.animation)
			{
				item2.speed = value * 1.05f;
			}
			if (nonAI)
			{
				speed *= 1.1f;
			}
			else
			{
				speed *= 1.4f;
			}
			chaseDistance *= 1.15f;
		}
		if (myDifficulty == 2)
		{
			foreach (AnimationState item3 in base.animation)
			{
				item3.speed = value * 1.05f;
			}
			if (nonAI)
			{
				speed *= 1.1f;
			}
			else
			{
				speed *= 1.5f;
			}
			chaseDistance *= 1.3f;
		}
		if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.ENDLESS_TITAN || IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.SURVIVE_MODE)
		{
			chaseDistance = 999999f;
		}
		if (nonAI)
		{
			if (abnormalType == AbnormalType.TYPE_CRAWLER)
			{
				speed = Mathf.Min(70f, speed);
			}
			else
			{
				speed = Mathf.Min(60f, speed);
			}
		}
		attackDistance = Vector3.Distance(base.transform.position, base.transform.Find("ap_front_ground").position) * 1.65f;
	}

	private void setLevel2(float level, int AI, int skinColor)
	{
		myLevel = level;
		myLevel = Mathf.Clamp(myLevel, 0.1f, 50f);
		attackWait += UnityEngine.Random.Range(0f, 2f);
		chaseDistance += myLevel * 10f;
		base.transform.localScale = new Vector3(myLevel, myLevel, myLevel);
		float num = Mathf.Min(Mathf.Pow(2f / myLevel, 0.35f), 1.25f);
		headscale = new Vector3(num, num, num);
		head = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head");
		head.localScale = headscale;
		if (skinColor != 0)
		{
			Material material = mainMaterial.GetComponent<SkinnedMeshRenderer>().material;
			Color color;
			switch (skinColor)
			{
			case 1:
				color = FengColor.titanSkin1;
				break;
			case 2:
				color = FengColor.titanSkin2;
				break;
			default:
				color = FengColor.titanSkin3;
				break;
			}
			material.color = color;
		}
		float value = 1.4f - (myLevel - 0.7f) * 0.15f;
		value = Mathf.Clamp(value, 0.9f, 1.5f);
		foreach (AnimationState item in base.animation)
		{
			item.speed = value;
		}
		base.rigidbody.mass *= myLevel;
		base.rigidbody.rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0, 360), 0f);
		if (myLevel > 1f)
		{
			speed *= Mathf.Sqrt(myLevel);
		}
		myDifficulty = AI;
		if (myDifficulty == 1 || myDifficulty == 2)
		{
			foreach (AnimationState item2 in base.animation)
			{
				item2.speed = value * 1.05f;
			}
			if (nonAI)
			{
				speed *= 1.1f;
			}
			else
			{
				speed *= 1.4f;
			}
			chaseDistance *= 1.15f;
		}
		if (myDifficulty == 2)
		{
			foreach (AnimationState item3 in base.animation)
			{
				item3.speed = value * 1.05f;
			}
			if (nonAI)
			{
				speed *= 1.1f;
			}
			else
			{
				speed *= 1.5f;
			}
			chaseDistance *= 1.3f;
		}
		if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.ENDLESS_TITAN || IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.SURVIVE_MODE)
		{
			chaseDistance = 999999f;
		}
		if (nonAI)
		{
			if (abnormalType == AbnormalType.TYPE_CRAWLER)
			{
				speed = Mathf.Min(70f, speed);
			}
			else
			{
				speed = Mathf.Min(60f, speed);
			}
		}
		attackDistance = Vector3.Distance(base.transform.position, base.transform.Find("ap_front_ground").position) * 1.65f;
	}

	private void setmyLevel()
	{
		base.animation.cullingType = AnimationCullingType.BasedOnRenderers;
		if (IN_GAME_MAIN_CAMERA.gametype != 0 && base.photonView.isMine)
		{
			object[] parameters = new object[3]
			{
				myLevel,
				GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().difficulty,
				UnityEngine.Random.Range(0, 4)
			};
			base.photonView.RPC("netSetLevel", PhotonTargets.AllBuffered, parameters);
			base.animation.cullingType = AnimationCullingType.AlwaysAnimate;
		}
		else if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
		{
			setLevel2(myLevel, IN_GAME_MAIN_CAMERA.difficulty, UnityEngine.Random.Range(0, 4));
		}
	}

	[RPC]
	private void setMyTarget(int ID)
	{
		if (ID == -1)
		{
			myHero = null;
		}
		PhotonView photonView = PhotonView.Find(ID);
		if (photonView != null)
		{
			myHero = photonView.gameObject;
		}
	}

	public void setRoute(GameObject route)
	{
		checkPoints = new ArrayList();
		for (int i = 1; i <= 10; i++)
		{
			checkPoints.Add(route.transform.Find("r" + i).position);
		}
		checkPoints.Add("end");
	}

	private bool simpleHitTestLineAndBall(Vector3 line, Vector3 ball, float R)
	{
		Vector3 vector = Vector3.Project(ball, line);
		if ((ball - vector).magnitude > R)
		{
			return false;
		}
		if (Vector3.Dot(line, vector) < 0f)
		{
			return false;
		}
		if (vector.sqrMagnitude > line.sqrMagnitude)
		{
			return false;
		}
		return true;
	}

	private void sitdown()
	{
		state = TitanState.sit;
		playAnimation("sit_down");
		getdownTime = UnityEngine.Random.Range(10f, 30f);
	}

	private void Start()
	{
		MultiplayerManager.addTitan(this);
		if (Minimap.instance != null)
		{
			Minimap.instance.TrackGameObjectOnMinimap(base.gameObject, Color.yellow, false, true);
		}
		currentCamera = GameObject.Find("MainCamera");
		runAnimation = "run_walk";
		grabTF = new GameObject();
		grabTF.name = "titansTmpGrabTF";
		head = baseTransform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head");
		neck = baseTransform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck");
		oldHeadRotation = head.rotation;
		if (IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.MULTIPLAYER || base.photonView.isMine)
		{
			if (!hasSetLevel)
			{
				myLevel = UnityEngine.Random.Range(0.7f, 3f);
				if (SettingsManager.LegacyGameSettings.TitanSizeEnabled.Value)
				{
					float value = SettingsManager.LegacyGameSettings.TitanSizeMin.Value;
					float value2 = SettingsManager.LegacyGameSettings.TitanSizeMax.Value;
					myLevel = UnityEngine.Random.Range(value, value2);
				}
				hasSetLevel = true;
			}
			spawnPt = baseTransform.position;
			setmyLevel();
			setAbnormalType2(abnormalType, false);
			if (myHero == null)
			{
				findNearestHero2();
			}
			controller = base.gameObject.GetComponent<TITAN_CONTROLLER>();
			StartCoroutine(HandleSpawnCollisionCoroutine(2f, 20f));
		}
		if (maxHealth == 0 && SettingsManager.LegacyGameSettings.TitanHealthMode.Value > 0)
		{
			if (SettingsManager.LegacyGameSettings.TitanHealthMode.Value == 1)
			{
				maxHealth = (currentHealth = UnityEngine.Random.Range(SettingsManager.LegacyGameSettings.TitanHealthMin.Value, SettingsManager.LegacyGameSettings.TitanHealthMax.Value + 1));
			}
			else if (SettingsManager.LegacyGameSettings.TitanHealthMode.Value == 2)
			{
				maxHealth = (currentHealth = Mathf.Clamp(Mathf.RoundToInt(myLevel / 4f * (float)UnityEngine.Random.Range(SettingsManager.LegacyGameSettings.TitanHealthMin.Value, SettingsManager.LegacyGameSettings.TitanHealthMax.Value + 1)), SettingsManager.LegacyGameSettings.TitanHealthMin.Value, SettingsManager.LegacyGameSettings.TitanHealthMax.Value));
			}
		}
		lagMax = 150f + myLevel * 3f;
		healthTime = Time.time;
		if (currentHealth > 0 && base.photonView.isMine)
		{
			base.photonView.RPC("labelRPC", PhotonTargets.AllBuffered, currentHealth, maxHealth);
		}
		hasExplode = false;
		colliderEnabled = true;
		isHooked = false;
		isLook = false;
		isThunderSpear = false;
		hasSpawn = true;
		_hasRunStart = true;
	}

	public void suicide()
	{
		netDie();
		if (nonAI)
		{
			GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().sendKillInfo(false, string.Empty, true, (string)PhotonNetwork.player.customProperties[PhotonPlayerProperty.name]);
		}
		GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().needChooseSide = true;
		GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().justSuicide = true;
	}

	public void testVisual(bool setCollider)
	{
		if (setCollider)
		{
			Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>();
			foreach (Renderer renderer in componentsInChildren)
			{
				renderer.material.color = Color.white;
			}
		}
		else
		{
			Renderer[] componentsInChildren2 = GetComponentsInChildren<Renderer>();
			foreach (Renderer renderer2 in componentsInChildren2)
			{
				renderer2.material.color = Color.black;
			}
		}
	}

	[RPC]
	public void titanGetHit(int viewID, int speed)
	{
		PhotonView photonView = PhotonView.Find(viewID);
		if (!(photonView != null) || !((photonView.gameObject.transform.position - neck.position).magnitude < lagMax) || hasDie || !(Time.time - healthTime > 0.2f))
		{
			return;
		}
		healthTime = Time.time;
		if (!SettingsManager.LegacyGameSettings.TitanArmorEnabled.Value || speed >= SettingsManager.LegacyGameSettings.TitanArmor.Value)
		{
			currentHealth -= speed;
		}
		else if (abnormalType == AbnormalType.TYPE_CRAWLER && !SettingsManager.LegacyGameSettings.TitanArmorCrawlerEnabled.Value)
		{
			currentHealth -= speed;
		}
		if ((float)maxHealth > 0f)
		{
			base.photonView.RPC("labelRPC", PhotonTargets.AllBuffered, currentHealth, maxHealth);
		}
		if ((float)currentHealth < 0f)
		{
			if (PhotonNetwork.isMasterClient)
			{
				OnTitanDie(photonView);
			}
			base.photonView.RPC("netDie", PhotonTargets.OthersBuffered);
			if (grabbedTarget != null)
			{
				grabbedTarget.GetPhotonView().RPC("netUngrabbed", PhotonTargets.All);
			}
			netDie();
			if (nonAI)
			{
				FengGameManagerMKII.instance.titanGetKill(photonView.owner, speed, (string)PhotonNetwork.player.customProperties[PhotonPlayerProperty.name]);
			}
			else
			{
				FengGameManagerMKII.instance.titanGetKill(photonView.owner, speed, base.name);
			}
		}
		else
		{
			FengGameManagerMKII.instance.photonView.RPC("netShowDamage", photonView.owner, speed);
		}
	}

	public void toCheckPoint(Vector3 targetPt, float r)
	{
		state = TitanState.to_check_point;
		targetCheckPt = targetPt;
		targetR = r;
		crossFade(runAnimation, 0.5f);
	}

	public void toPVPCheckPoint(Vector3 targetPt, float r)
	{
		state = TitanState.to_pvp_pt;
		targetCheckPt = targetPt;
		targetR = r;
		crossFade(runAnimation, 0.5f);
	}

	private void turn(float d)
	{
		if (abnormalType == AbnormalType.TYPE_CRAWLER)
		{
			if (d > 0f)
			{
				turnAnimation = "crawler_turnaround_R";
			}
			else
			{
				turnAnimation = "crawler_turnaround_L";
			}
		}
		else if (d > 0f)
		{
			turnAnimation = "turnaround2";
		}
		else
		{
			turnAnimation = "turnaround1";
		}
		playAnimation(turnAnimation);
		base.animation[turnAnimation].time = 0f;
		d = Mathf.Clamp(d, -120f, 120f);
		turnDeg = d;
		desDeg = base.gameObject.transform.rotation.eulerAngles.y + turnDeg;
		state = TitanState.turn;
	}

	public void UpdateHeroDistance()
	{
		if ((!GameMenu.Paused || IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER) && myDifficulty >= 0 && !nonAI)
		{
			if (myHero == null)
			{
				myDistance = float.MaxValue;
				return;
			}
			Vector2 a = new Vector2(myHero.transform.position.x, myHero.transform.position.z);
			Vector2 b = new Vector2(baseTransform.position.x, baseTransform.position.z);
			myDistance = Vector2.Distance(a, b);
		}
	}

	public void update2()
	{
		UpdateHeroDistance();
		if ((GameMenu.Paused && IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE) || myDifficulty < 0 || (IN_GAME_MAIN_CAMERA.gametype != 0 && !base.photonView.isMine))
		{
			return;
		}
		explode();
		if (!nonAI)
		{
			if (activeRad < int.MaxValue && (state == TitanState.idle || state == TitanState.wander || state == TitanState.chase))
			{
				if (checkPoints.Count > 1)
				{
					if (Vector3.Distance((Vector3)checkPoints[0], baseTransform.position) > (float)activeRad)
					{
						toCheckPoint((Vector3)checkPoints[0], 10f);
					}
				}
				else if (Vector3.Distance(spawnPt, baseTransform.position) > (float)activeRad)
				{
					toCheckPoint(spawnPt, 10f);
				}
			}
			if (whoHasTauntMe != null)
			{
				tauntTime -= Time.deltaTime;
				if (tauntTime <= 0f)
				{
					whoHasTauntMe = null;
				}
				myHero = whoHasTauntMe;
				if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && PhotonNetwork.isMasterClient)
				{
					object[] parameters = new object[1] { myHero.GetPhotonView().viewID };
					base.photonView.RPC("setMyTarget", PhotonTargets.Others, parameters);
				}
			}
		}
		if (hasDie)
		{
			dieTime += Time.deltaTime;
			if (dieTime > 2f && !hasDieSteam)
			{
				hasDieSteam = true;
				if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
				{
					GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("FX/FXtitanDie1"));
					gameObject.transform.position = baseTransform.Find("Amarture/Core/Controller_Body/hip").position;
					gameObject.transform.localScale = baseTransform.localScale;
				}
				else if (base.photonView.isMine)
				{
					PhotonNetwork.Instantiate("FX/FXtitanDie1", baseTransform.Find("Amarture/Core/Controller_Body/hip").position, Quaternion.Euler(-90f, 0f, 0f), 0).transform.localScale = baseTransform.localScale;
				}
			}
			if (dieTime > 5f)
			{
				if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
				{
					GameObject gameObject2 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("FX/FXtitanDie"));
					gameObject2.transform.position = baseTransform.Find("Amarture/Core/Controller_Body/hip").position;
					gameObject2.transform.localScale = baseTransform.localScale;
					UnityEngine.Object.Destroy(base.gameObject);
				}
				else if (base.photonView.isMine)
				{
					PhotonNetwork.Instantiate("FX/FXtitanDie", baseTransform.Find("Amarture/Core/Controller_Body/hip").position, Quaternion.Euler(-90f, 0f, 0f), 0).transform.localScale = baseTransform.localScale;
					PhotonNetwork.Destroy(base.gameObject);
					myDifficulty = -1;
				}
			}
			return;
		}
		if (state == TitanState.hit)
		{
			if (hitPause > 0f)
			{
				hitPause -= Time.deltaTime;
				if (hitPause <= 0f)
				{
					baseAnimation[hitAnimation].speed = 1f;
					hitPause = 0f;
				}
			}
			if (baseAnimation[hitAnimation].normalizedTime >= 1f)
			{
				idle();
			}
		}
		if (!nonAI)
		{
			if (myHero == null)
			{
				findNearestHero2();
			}
			if ((state == TitanState.idle || state == TitanState.chase || state == TitanState.wander) && whoHasTauntMe == null && UnityEngine.Random.Range(0, 100) < 10)
			{
				findNearestFacingHero2();
			}
		}
		else
		{
			if (stamina < maxStamina)
			{
				if (baseAnimation.IsPlaying("idle"))
				{
					stamina += Time.deltaTime * 30f;
				}
				if (baseAnimation.IsPlaying("crawler_idle"))
				{
					stamina += Time.deltaTime * 35f;
				}
				if (baseAnimation.IsPlaying("run_walk"))
				{
					stamina += Time.deltaTime * 10f;
				}
			}
			if (baseAnimation.IsPlaying("run_abnormal_1"))
			{
				stamina -= Time.deltaTime * 5f;
			}
			if (baseAnimation.IsPlaying("crawler_run"))
			{
				stamina -= Time.deltaTime * 15f;
			}
			if (stamina < 0f)
			{
				stamina = 0f;
			}
			if (!GameMenu.Paused)
			{
				GameObject.Find("stamina_titan").transform.localScale = new Vector3(stamina, 16f);
			}
		}
		if (state == TitanState.laugh)
		{
			if (baseAnimation["laugh"].normalizedTime >= 1f)
			{
				idle(2f);
			}
		}
		else if (state == TitanState.idle)
		{
			if (nonAI)
			{
				if (GameMenu.Paused)
				{
					return;
				}
				pt();
				if (abnormalType != AbnormalType.TYPE_CRAWLER)
				{
					if (controller.isAttackDown && stamina > 25f)
					{
						stamina -= 25f;
						attack2("combo_1");
					}
					else if (controller.isAttackIIDown && stamina > 50f)
					{
						stamina -= 50f;
						attack2("abnormal_jump");
					}
					else if (controller.isJumpDown && stamina > 15f)
					{
						stamina -= 15f;
						attack2("jumper_0");
					}
				}
				else if (controller.isAttackDown && stamina > 40f)
				{
					stamina -= 40f;
					attack2("crawler_jump_0");
				}
				if (controller.isSuicide)
				{
					suicide();
				}
				return;
			}
			if (sbtime > 0f)
			{
				sbtime -= Time.deltaTime;
				return;
			}
			if (!isAlarm)
			{
				if (abnormalType != AbnormalType.TYPE_PUNK && abnormalType != AbnormalType.TYPE_CRAWLER && UnityEngine.Random.Range(0f, 1f) < 0.005f)
				{
					sitdown();
					return;
				}
				if (UnityEngine.Random.Range(0f, 1f) < 0.02f)
				{
					wander();
					return;
				}
				if (UnityEngine.Random.Range(0f, 1f) < 0.01f)
				{
					turn(UnityEngine.Random.Range(30, 120));
					return;
				}
				if (UnityEngine.Random.Range(0f, 1f) < 0.01f)
				{
					turn(UnityEngine.Random.Range(-30, -120));
					return;
				}
			}
			angle = 0f;
			between2 = 0f;
			if (myDistance < chaseDistance || whoHasTauntMe != null)
			{
				Vector3 vector = myHero.transform.position - baseTransform.position;
				angle = (0f - Mathf.Atan2(vector.z, vector.x)) * 57.29578f;
				between2 = 0f - Mathf.DeltaAngle(angle, baseGameObjectTransform.rotation.eulerAngles.y - 90f);
				if (myDistance >= attackDistance)
				{
					if (isAlarm || Mathf.Abs(between2) < 90f)
					{
						chase();
						return;
					}
					if (!isAlarm && !(myDistance >= chaseDistance * 0.1f))
					{
						chase();
						return;
					}
				}
			}
			if (longRangeAttackCheck2())
			{
				return;
			}
			if (myDistance < chaseDistance)
			{
				if (abnormalType == AbnormalType.TYPE_JUMPER && (myDistance > attackDistance || myHero.transform.position.y > head.position.y + 4f * myLevel) && Mathf.Abs(between2) < 120f && Vector3.Distance(baseTransform.position, myHero.transform.position) < 1.5f * myHero.transform.position.y)
				{
					attack2("jumper_0");
					return;
				}
				if (abnormalType == AbnormalType.TYPE_CRAWLER && myDistance < attackDistance * 3f && Mathf.Abs(between2) < 90f && myHero.transform.position.y < neck.position.y + 30f * myLevel && myHero.transform.position.y > neck.position.y + 10f * myLevel)
				{
					attack2("crawler_jump_0");
					return;
				}
			}
			if (abnormalType == AbnormalType.TYPE_PUNK && myDistance < 90f && Mathf.Abs(between2) > 90f)
			{
				if (UnityEngine.Random.Range(0f, 1f) < 0.4f)
				{
					randomRun(baseTransform.position + new Vector3(UnityEngine.Random.Range(-50f, 50f), UnityEngine.Random.Range(-50f, 50f), UnityEngine.Random.Range(-50f, 50f)), 10f);
				}
				if (UnityEngine.Random.Range(0f, 1f) < 0.2f)
				{
					recover();
				}
				else if (UnityEngine.Random.Range(0, 2) == 0)
				{
					attack2("quick_turn_l");
				}
				else
				{
					attack2("quick_turn_r");
				}
				return;
			}
			if (myDistance < attackDistance)
			{
				if (abnormalType == AbnormalType.TYPE_CRAWLER)
				{
					if (myHero.transform.position.y + 3f <= neck.position.y + 20f * myLevel && UnityEngine.Random.Range(0f, 1f) < 0.1f)
					{
						chase();
					}
					return;
				}
				string text = string.Empty;
				string[] attackStrategy = GetAttackStrategy();
				if (attackStrategy != null)
				{
					text = attackStrategy[UnityEngine.Random.Range(0, attackStrategy.Length)];
				}
				if ((abnormalType == AbnormalType.TYPE_JUMPER || abnormalType == AbnormalType.TYPE_I) && Mathf.Abs(between2) > 40f)
				{
					if (text.Contains("grab") || text.Contains("kick") || text.Contains("slap") || text.Contains("bite"))
					{
						if (UnityEngine.Random.Range(0, 100) < 30)
						{
							turn(between2);
							return;
						}
					}
					else if (UnityEngine.Random.Range(0, 100) < 90)
					{
						turn(between2);
						return;
					}
				}
				if (executeAttack2(text))
				{
					return;
				}
				if (abnormalType == AbnormalType.NORMAL)
				{
					if (UnityEngine.Random.Range(0, 100) < 30 && Mathf.Abs(between2) > 45f)
					{
						turn(between2);
						return;
					}
				}
				else if (Mathf.Abs(between2) > 45f)
				{
					turn(between2);
					return;
				}
			}
			if (!(PVPfromCheckPt != null))
			{
				return;
			}
			if (PVPfromCheckPt.state == CheckPointState.Titan)
			{
				if (UnityEngine.Random.Range(0, 100) > 48)
				{
					GameObject chkPtNext = PVPfromCheckPt.chkPtNext;
					if (chkPtNext != null && (chkPtNext.GetComponent<PVPcheckPoint>().state != CheckPointState.Titan || UnityEngine.Random.Range(0, 100) < 20))
					{
						toPVPCheckPoint(chkPtNext.transform.position, 5 + UnityEngine.Random.Range(0, 10));
						PVPfromCheckPt = chkPtNext.GetComponent<PVPcheckPoint>();
					}
				}
				else
				{
					GameObject chkPtNext = PVPfromCheckPt.chkPtPrevious;
					if (chkPtNext != null && (chkPtNext.GetComponent<PVPcheckPoint>().state != CheckPointState.Titan || UnityEngine.Random.Range(0, 100) < 5))
					{
						toPVPCheckPoint(chkPtNext.transform.position, 5 + UnityEngine.Random.Range(0, 10));
						PVPfromCheckPt = chkPtNext.GetComponent<PVPcheckPoint>();
					}
				}
			}
			else
			{
				toPVPCheckPoint(PVPfromCheckPt.transform.position, 5 + UnityEngine.Random.Range(0, 10));
			}
		}
		else if (state == TitanState.attack)
		{
			if (attackAnimation == "combo")
			{
				if (nonAI)
				{
					if (controller.isAttackDown)
					{
						nonAIcombo = true;
					}
					if (!nonAIcombo && !(baseAnimation["attack_" + attackAnimation].normalizedTime < 0.385f))
					{
						idle();
						return;
					}
				}
				if (baseAnimation["attack_" + attackAnimation].normalizedTime >= 0.11f && baseAnimation["attack_" + attackAnimation].normalizedTime <= 0.16f)
				{
					GameObject gameObject3 = checkIfHitHand(baseTransform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R/hand_R_001"));
					if (gameObject3 != null)
					{
						Vector3 position = baseTransform.Find("Amarture/Core/Controller_Body/hip/spine/chest").position;
						if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
						{
							gameObject3.GetComponent<HERO>().die((gameObject3.transform.position - position) * 15f * myLevel, false);
						}
						else if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && base.photonView.isMine && !gameObject3.GetComponent<HERO>().HasDied())
						{
							gameObject3.GetComponent<HERO>().markDie();
							object[] parameters2 = new object[5]
							{
								(gameObject3.transform.position - position) * 15f * myLevel,
								false,
								(!nonAI) ? (-1) : base.photonView.viewID,
								base.name,
								true
							};
							gameObject3.GetComponent<HERO>().photonView.RPC("netDie", PhotonTargets.All, parameters2);
						}
					}
				}
				if (baseAnimation["attack_" + attackAnimation].normalizedTime >= 0.27f && baseAnimation["attack_" + attackAnimation].normalizedTime <= 0.32f)
				{
					GameObject gameObject4 = checkIfHitHand(baseTransform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L/forearm_L/hand_L/hand_L_001"));
					if (gameObject4 != null)
					{
						Vector3 position2 = baseTransform.Find("Amarture/Core/Controller_Body/hip/spine/chest").position;
						if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
						{
							gameObject4.GetComponent<HERO>().die((gameObject4.transform.position - position2) * 15f * myLevel, false);
						}
						else if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && base.photonView.isMine && !gameObject4.GetComponent<HERO>().HasDied())
						{
							gameObject4.GetComponent<HERO>().markDie();
							object[] parameters3 = new object[5]
							{
								(gameObject4.transform.position - position2) * 15f * myLevel,
								false,
								(!nonAI) ? (-1) : base.photonView.viewID,
								base.name,
								true
							};
							gameObject4.GetComponent<HERO>().photonView.RPC("netDie", PhotonTargets.All, parameters3);
						}
					}
				}
			}
			if (attackCheckTimeA != 0f && baseAnimation["attack_" + attackAnimation].normalizedTime >= attackCheckTimeA && baseAnimation["attack_" + attackAnimation].normalizedTime <= attackCheckTimeB)
			{
				if (leftHandAttack)
				{
					GameObject gameObject5 = checkIfHitHand(baseTransform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L/forearm_L/hand_L/hand_L_001"));
					if (gameObject5 != null)
					{
						Vector3 position3 = baseTransform.Find("Amarture/Core/Controller_Body/hip/spine/chest").position;
						if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
						{
							gameObject5.GetComponent<HERO>().die((gameObject5.transform.position - position3) * 15f * myLevel, false);
						}
						else if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && base.photonView.isMine && !gameObject5.GetComponent<HERO>().HasDied())
						{
							gameObject5.GetComponent<HERO>().markDie();
							object[] parameters4 = new object[5]
							{
								(gameObject5.transform.position - position3) * 15f * myLevel,
								false,
								(!nonAI) ? (-1) : base.photonView.viewID,
								base.name,
								true
							};
							gameObject5.GetComponent<HERO>().photonView.RPC("netDie", PhotonTargets.All, parameters4);
						}
					}
				}
				else
				{
					GameObject gameObject6 = checkIfHitHand(baseTransform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R/hand_R_001"));
					if (gameObject6 != null)
					{
						Vector3 position4 = baseTransform.Find("Amarture/Core/Controller_Body/hip/spine/chest").position;
						if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
						{
							gameObject6.GetComponent<HERO>().die((gameObject6.transform.position - position4) * 15f * myLevel, false);
						}
						else if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && base.photonView.isMine && !gameObject6.GetComponent<HERO>().HasDied())
						{
							gameObject6.GetComponent<HERO>().markDie();
							object[] parameters5 = new object[5]
							{
								(gameObject6.transform.position - position4) * 15f * myLevel,
								false,
								(!nonAI) ? (-1) : base.photonView.viewID,
								base.name,
								true
							};
							gameObject6.GetComponent<HERO>().photonView.RPC("netDie", PhotonTargets.All, parameters5);
						}
					}
				}
			}
			if (!attacked && attackCheckTime != 0f && baseAnimation["attack_" + attackAnimation].normalizedTime >= attackCheckTime)
			{
				attacked = true;
				fxPosition = baseTransform.Find("ap_" + attackAnimation).position;
				GameObject gameObject7 = ((IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.MULTIPLAYER || !base.photonView.isMine) ? ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("FX/" + fxName), fxPosition, fxRotation)) : PhotonNetwork.Instantiate("FX/" + fxName, fxPosition, fxRotation, 0));
				if (nonAI)
				{
					gameObject7.transform.localScale = baseTransform.localScale * 1.5f;
					if (gameObject7.GetComponent<EnemyfxIDcontainer>() != null)
					{
						gameObject7.GetComponent<EnemyfxIDcontainer>().myOwnerViewID = base.photonView.viewID;
					}
				}
				else
				{
					gameObject7.transform.localScale = baseTransform.localScale;
				}
				if (gameObject7.GetComponent<EnemyfxIDcontainer>() != null)
				{
					gameObject7.GetComponent<EnemyfxIDcontainer>().titanName = base.name;
				}
				float b = 1f - Vector3.Distance(currentCamera.transform.position, gameObject7.transform.position) * 0.05f;
				b = Mathf.Min(1f, b);
				currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().startShake(b, b);
			}
			if (attackAnimation == "throw")
			{
				if (!attacked && baseAnimation["attack_" + attackAnimation].normalizedTime >= 0.11f)
				{
					attacked = true;
					Transform transform = baseTransform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R/hand_R_001");
					if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && base.photonView.isMine)
					{
						throwRock = PhotonNetwork.Instantiate("FX/rockThrow", transform.position, transform.rotation, 0);
					}
					else
					{
						throwRock = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("FX/rockThrow"), transform.position, transform.rotation);
					}
					throwRock.transform.localScale = baseTransform.localScale;
					throwRock.transform.position -= throwRock.transform.forward * 2.5f * myLevel;
					if (throwRock.GetComponent<EnemyfxIDcontainer>() != null)
					{
						if (nonAI)
						{
							throwRock.GetComponent<EnemyfxIDcontainer>().myOwnerViewID = base.photonView.viewID;
						}
						throwRock.GetComponent<EnemyfxIDcontainer>().titanName = base.name;
					}
					throwRock.transform.parent = transform;
					if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && base.photonView.isMine)
					{
						object[] parameters6 = new object[4]
						{
							base.photonView.viewID,
							baseTransform.localScale,
							throwRock.transform.localPosition,
							myLevel
						};
						throwRock.GetPhotonView().RPC("initRPC", PhotonTargets.Others, parameters6);
					}
				}
				if (baseAnimation["attack_" + attackAnimation].normalizedTime >= 0.11f)
				{
					float y = Mathf.Atan2(myHero.transform.position.x - baseTransform.position.x, myHero.transform.position.z - baseTransform.position.z) * 57.29578f;
					baseGameObjectTransform.rotation = Quaternion.Euler(0f, y, 0f);
				}
				if (throwRock != null && baseAnimation["attack_" + attackAnimation].normalizedTime >= 0.62f)
				{
					float num = 1f;
					float num2 = -20f;
					Vector3 vector2;
					if (myHero != null)
					{
						vector2 = (myHero.transform.position - throwRock.transform.position) / num + myHero.rigidbody.velocity;
						float num3 = myHero.transform.position.y + 2f * myLevel;
						float num4 = num3 - throwRock.transform.position.y;
						vector2 = new Vector3(vector2.x, num4 / num - 0.5f * num2 * num, vector2.z);
					}
					else
					{
						vector2 = baseTransform.forward * 60f + Vector3.up * 10f;
					}
					throwRock.GetComponent<RockThrow>().launch(vector2);
					throwRock.transform.parent = null;
					throwRock = null;
				}
			}
			if (attackAnimation == "jumper_0" || attackAnimation == "crawler_jump_0")
			{
				if (!attacked)
				{
					if (baseAnimation["attack_" + attackAnimation].normalizedTime >= 0.68f)
					{
						attacked = true;
						if (myHero == null || nonAI)
						{
							float num5 = 120f;
							Vector3 velocity = baseTransform.forward * speed + Vector3.up * num5;
							if (nonAI && abnormalType == AbnormalType.TYPE_CRAWLER)
							{
								num5 = 100f;
								float a = speed * 2.5f;
								a = Mathf.Min(a, 100f);
								velocity = baseTransform.forward * a + Vector3.up * num5;
							}
							baseRigidBody.velocity = velocity;
						}
						else
						{
							float y2 = myHero.rigidbody.velocity.y;
							float num6 = -20f;
							float num7 = gravity;
							float y3 = neck.position.y;
							float num8 = (num6 - num7) * 0.5f;
							float num9 = y2;
							float num10 = myHero.transform.position.y - y3;
							float num11 = Mathf.Abs((Mathf.Sqrt(num9 * num9 - 4f * num8 * num10) - num9) / (2f * num8));
							Vector3 vector3 = myHero.transform.position + myHero.rigidbody.velocity * num11 + Vector3.up * 0.5f * num6 * num11 * num11;
							float y4 = vector3.y;
							float num12;
							if (num10 < 0f || y4 - y3 < 0f)
							{
								num12 = 60f;
								float a2 = speed * 2.5f;
								a2 = Mathf.Min(a2, 100f);
								Vector3 velocity2 = baseTransform.forward * a2 + Vector3.up * num12;
								baseRigidBody.velocity = velocity2;
								return;
							}
							float num13 = y4 - y3;
							float num14 = Mathf.Sqrt(2f * num13 / gravity);
							num12 = gravity * num14;
							num12 = Mathf.Max(30f, num12);
							Vector3 vector4 = (vector3 - baseTransform.position) / num11;
							abnorma_jump_bite_horizon_v = new Vector3(vector4.x, 0f, vector4.z);
							Vector3 velocity3 = baseRigidBody.velocity;
							Vector3 force = new Vector3(abnorma_jump_bite_horizon_v.x, velocity3.y, abnorma_jump_bite_horizon_v.z) - velocity3;
							baseRigidBody.AddForce(force, ForceMode.VelocityChange);
							baseRigidBody.AddForce(Vector3.up * num12, ForceMode.VelocityChange);
							float num15 = Vector2.Angle(new Vector2(baseTransform.position.x, baseTransform.position.z), new Vector2(myHero.transform.position.x, myHero.transform.position.z));
							num15 = Mathf.Atan2(myHero.transform.position.x - baseTransform.position.x, myHero.transform.position.z - baseTransform.position.z) * 57.29578f;
							baseGameObjectTransform.rotation = Quaternion.Euler(0f, num15, 0f);
						}
					}
					else
					{
						baseRigidBody.velocity = Vector3.zero;
					}
				}
				if (!(baseAnimation["attack_" + attackAnimation].normalizedTime >= 1f))
				{
					return;
				}
				Debug.DrawLine(baseTransform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head").position + Vector3.up * 1.5f * myLevel, baseTransform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head").position + Vector3.up * 1.5f * myLevel + Vector3.up * 3f * myLevel, Color.green);
				Debug.DrawLine(baseTransform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head").position + Vector3.up * 1.5f * myLevel, baseTransform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head").position + Vector3.up * 1.5f * myLevel + Vector3.forward * 3f * myLevel, Color.green);
				GameObject gameObject8 = checkIfHitHead(head, 3f);
				if (gameObject8 != null)
				{
					Vector3 position5 = baseTransform.Find("Amarture/Core/Controller_Body/hip/spine/chest").position;
					if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
					{
						gameObject8.GetComponent<HERO>().die((gameObject8.transform.position - position5) * 15f * myLevel, false);
					}
					else if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && base.photonView.isMine && !gameObject8.GetComponent<HERO>().HasDied())
					{
						gameObject8.GetComponent<HERO>().markDie();
						object[] parameters7 = new object[5]
						{
							(gameObject8.transform.position - position5) * 15f * myLevel,
							true,
							(!nonAI) ? (-1) : base.photonView.viewID,
							base.name,
							true
						};
						gameObject8.GetComponent<HERO>().photonView.RPC("netDie", PhotonTargets.All, parameters7);
					}
					if (abnormalType == AbnormalType.TYPE_CRAWLER)
					{
						attackAnimation = "crawler_jump_1";
					}
					else
					{
						attackAnimation = "jumper_1";
					}
					playAnimation("attack_" + attackAnimation);
				}
				if (Mathf.Abs(baseRigidBody.velocity.y) < 0.5f || baseRigidBody.velocity.y < 0f || IsGrounded())
				{
					if (abnormalType == AbnormalType.TYPE_CRAWLER)
					{
						attackAnimation = "crawler_jump_1";
					}
					else
					{
						attackAnimation = "jumper_1";
					}
					playAnimation("attack_" + attackAnimation);
				}
			}
			else if (attackAnimation == "jumper_1" || attackAnimation == "crawler_jump_1")
			{
				if (baseAnimation["attack_" + attackAnimation].normalizedTime >= 1f && grounded)
				{
					if (abnormalType == AbnormalType.TYPE_CRAWLER)
					{
						attackAnimation = "crawler_jump_2";
					}
					else
					{
						attackAnimation = "jumper_2";
					}
					crossFade("attack_" + attackAnimation, 0.1f);
					fxPosition = baseTransform.position;
					GameObject gameObject9 = ((IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.MULTIPLAYER || !base.photonView.isMine) ? ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("FX/boom2"), fxPosition, fxRotation)) : PhotonNetwork.Instantiate("FX/boom2", fxPosition, fxRotation, 0));
					gameObject9.transform.localScale = baseTransform.localScale * 1.6f;
					float b2 = 1f - Vector3.Distance(currentCamera.transform.position, gameObject9.transform.position) * 0.05f;
					b2 = Mathf.Min(1f, b2);
					currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().startShake(b2, b2);
				}
			}
			else if (attackAnimation == "jumper_2" || attackAnimation == "crawler_jump_2")
			{
				if (baseAnimation["attack_" + attackAnimation].normalizedTime >= 1f)
				{
					idle();
				}
			}
			else if (baseAnimation.IsPlaying("tired"))
			{
				if (baseAnimation["tired"].normalizedTime >= 1f + Mathf.Max(attackEndWait * 2f, 3f))
				{
					idle(UnityEngine.Random.Range(attackWait - 1f, 3f));
				}
			}
			else
			{
				if (!(baseAnimation["attack_" + attackAnimation].normalizedTime >= 1f + attackEndWait))
				{
					return;
				}
				if (nextAttackAnimation != null)
				{
					attack2(nextAttackAnimation);
				}
				else if (attackAnimation == "quick_turn_l" || attackAnimation == "quick_turn_r")
				{
					baseTransform.rotation = Quaternion.Euler(baseTransform.rotation.eulerAngles.x, baseTransform.rotation.eulerAngles.y + 180f, baseTransform.rotation.eulerAngles.z);
					idle(UnityEngine.Random.Range(0.5f, 1f));
					playAnimation("idle");
				}
				else if (abnormalType == AbnormalType.TYPE_I || abnormalType == AbnormalType.TYPE_JUMPER)
				{
					attackCount++;
					if (attackCount > 3 && attackAnimation == "abnormal_getup")
					{
						attackCount = 0;
						crossFade("tired", 0.5f);
					}
					else
					{
						idle(UnityEngine.Random.Range(attackWait - 1f, 3f));
					}
				}
				else
				{
					idle(UnityEngine.Random.Range(attackWait - 1f, 3f));
				}
			}
		}
		else if (state == TitanState.grab)
		{
			if (baseAnimation["grab_" + attackAnimation].normalizedTime >= attackCheckTimeA && baseAnimation["grab_" + attackAnimation].normalizedTime <= attackCheckTimeB && grabbedTarget == null)
			{
				GameObject gameObject10 = checkIfHitHand(currentGrabHand);
				if (gameObject10 != null)
				{
					if (isGrabHandLeft)
					{
						eatSetL(gameObject10);
						grabbedTarget = gameObject10;
					}
					else
					{
						eatSet(gameObject10);
						grabbedTarget = gameObject10;
					}
				}
			}
			if (baseAnimation["grab_" + attackAnimation].normalizedTime >= 1f)
			{
				if (grabbedTarget != null)
				{
					eat();
				}
				else
				{
					idle(UnityEngine.Random.Range(attackWait - 1f, 2f));
				}
			}
		}
		else if (state == TitanState.eat)
		{
			if (!attacked && !(baseAnimation[attackAnimation].normalizedTime < 0.48f))
			{
				attacked = true;
				justEatHero(grabbedTarget, currentGrabHand);
			}
			bool flag = grabbedTarget == null;
			if (baseAnimation[attackAnimation].normalizedTime >= 1f)
			{
				idle();
			}
		}
		else if (state == TitanState.chase)
		{
			if (myHero == null)
			{
				idle();
			}
			else
			{
				if (longRangeAttackCheck2())
				{
					return;
				}
				if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.PVP_CAPTURE && PVPfromCheckPt != null && myDistance > chaseDistance)
				{
					idle();
				}
				else if (abnormalType == AbnormalType.TYPE_CRAWLER)
				{
					Vector3 vector5 = myHero.transform.position - baseTransform.position;
					float current = (0f - Mathf.Atan2(vector5.z, vector5.x)) * 57.29578f;
					float f = 0f - Mathf.DeltaAngle(current, baseGameObjectTransform.rotation.eulerAngles.y - 90f);
					if (myDistance < attackDistance * 3f && UnityEngine.Random.Range(0f, 1f) < 0.1f && Mathf.Abs(f) < 90f && myHero.transform.position.y < neck.position.y + 30f * myLevel && myHero.transform.position.y > neck.position.y + 10f * myLevel)
					{
						attack2("crawler_jump_0");
						return;
					}
					GameObject gameObject11 = checkIfHitCrawlerMouth(head, 2.2f);
					if (gameObject11 != null)
					{
						Vector3 position6 = baseTransform.Find("Amarture/Core/Controller_Body/hip/spine/chest").position;
						if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
						{
							gameObject11.GetComponent<HERO>().die((gameObject11.transform.position - position6) * 15f * myLevel, false);
						}
						else if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && base.photonView.isMine)
						{
							if (gameObject11.GetComponent<TITAN_EREN>() != null)
							{
								gameObject11.GetComponent<TITAN_EREN>().hitByTitan();
							}
							else if (!gameObject11.GetComponent<HERO>().HasDied())
							{
								gameObject11.GetComponent<HERO>().markDie();
								object[] parameters8 = new object[5]
								{
									(gameObject11.transform.position - position6) * 15f * myLevel,
									true,
									(!nonAI) ? (-1) : base.photonView.viewID,
									base.name,
									true
								};
								gameObject11.GetComponent<HERO>().photonView.RPC("netDie", PhotonTargets.All, parameters8);
							}
						}
					}
					if (myDistance < attackDistance && UnityEngine.Random.Range(0f, 1f) < 0.02f)
					{
						idle(UnityEngine.Random.Range(0.05f, 0.2f));
					}
				}
				else if (abnormalType == AbnormalType.TYPE_JUMPER && ((myDistance > attackDistance && myHero.transform.position.y > head.position.y + 4f * myLevel) || myHero.transform.position.y > head.position.y + 4f * myLevel) && Vector3.Distance(baseTransform.position, myHero.transform.position) < 1.5f * myHero.transform.position.y)
				{
					attack2("jumper_0");
				}
				else if (myDistance < attackDistance)
				{
					idle(UnityEngine.Random.Range(0.05f, 0.2f));
				}
			}
		}
		else if (state == TitanState.wander)
		{
			float num16 = 0f;
			float num17 = 0f;
			if (myDistance < chaseDistance || whoHasTauntMe != null)
			{
				Vector3 vector6 = myHero.transform.position - baseTransform.position;
				num16 = (0f - Mathf.Atan2(vector6.z, vector6.x)) * 57.29578f;
				num17 = 0f - Mathf.DeltaAngle(num16, baseGameObjectTransform.rotation.eulerAngles.y - 90f);
				if (isAlarm || Mathf.Abs(num17) < 90f)
				{
					chase();
					return;
				}
				if (!isAlarm && !(myDistance >= chaseDistance * 0.1f))
				{
					chase();
					return;
				}
			}
			if (UnityEngine.Random.Range(0f, 1f) < 0.01f)
			{
				idle();
			}
		}
		else if (state == TitanState.turn)
		{
			baseGameObjectTransform.rotation = Quaternion.Lerp(baseGameObjectTransform.rotation, Quaternion.Euler(0f, desDeg, 0f), Time.deltaTime * Mathf.Abs(turnDeg) * 0.015f);
			if (baseAnimation[turnAnimation].normalizedTime >= 1f)
			{
				idle();
			}
		}
		else if (state == TitanState.hit_eye)
		{
			if (baseAnimation.IsPlaying("sit_hit_eye") && baseAnimation["sit_hit_eye"].normalizedTime >= 1f)
			{
				remainSitdown();
			}
			else if (baseAnimation.IsPlaying("hit_eye") && baseAnimation["hit_eye"].normalizedTime >= 1f)
			{
				if (nonAI)
				{
					idle();
				}
				else
				{
					attack2("combo_1");
				}
			}
		}
		else if (state == TitanState.to_check_point)
		{
			if (checkPoints.Count <= 0 && myDistance < attackDistance)
			{
				string decidedAction = string.Empty;
				string[] attackStrategy2 = GetAttackStrategy();
				if (attackStrategy2 != null)
				{
					decidedAction = attackStrategy2[UnityEngine.Random.Range(0, attackStrategy2.Length)];
				}
				if (executeAttack2(decidedAction))
				{
					return;
				}
			}
			if (!(Vector3.Distance(baseTransform.position, targetCheckPt) < targetR))
			{
				return;
			}
			if (checkPoints.Count > 0)
			{
				if (checkPoints.Count == 1)
				{
					if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.BOSS_FIGHT_CT)
					{
						MultiplayerManager.gameLose2();
						checkPoints = new ArrayList();
						idle();
					}
					return;
				}
				if (checkPoints.Count == 4)
				{
					MultiplayerManager.sendChatContentInfo("<color=#A8FF24>*WARNING!* An abnormal titan is approaching the north gate!</color>");
				}
				Vector3 vector7 = (Vector3)checkPoints[0];
				targetCheckPt = vector7;
				checkPoints.RemoveAt(0);
			}
			else
			{
				idle();
			}
		}
		else if (state == TitanState.to_pvp_pt)
		{
			if (myDistance < chaseDistance * 0.7f)
			{
				chase();
			}
			if (Vector3.Distance(baseTransform.position, targetCheckPt) < targetR)
			{
				idle();
			}
		}
		else if (state == TitanState.random_run)
		{
			random_run_time -= Time.deltaTime;
			if (Vector3.Distance(baseTransform.position, targetCheckPt) < targetR || random_run_time <= 0f)
			{
				idle();
			}
		}
		else if (state == TitanState.down)
		{
			getdownTime -= Time.deltaTime;
			if (baseAnimation.IsPlaying("sit_hunt_down") && baseAnimation["sit_hunt_down"].normalizedTime >= 1f)
			{
				playAnimation("sit_idle");
			}
			if (getdownTime <= 0f)
			{
				crossFadeIfNotPlaying("sit_getup", 0.1f);
			}
			if (baseAnimation.IsPlaying("sit_getup") && baseAnimation["sit_getup"].normalizedTime >= 1f)
			{
				idle();
			}
		}
		else if (state == TitanState.sit)
		{
			getdownTime -= Time.deltaTime;
			angle = 0f;
			between2 = 0f;
			if (myDistance < chaseDistance || whoHasTauntMe != null)
			{
				if (myDistance < 50f)
				{
					isAlarm = true;
				}
				else
				{
					Vector3 vector8 = myHero.transform.position - baseTransform.position;
					angle = (0f - Mathf.Atan2(vector8.z, vector8.x)) * 57.29578f;
					between2 = 0f - Mathf.DeltaAngle(angle, baseGameObjectTransform.rotation.eulerAngles.y - 90f);
					if (Mathf.Abs(between2) < 100f)
					{
						isAlarm = true;
					}
				}
			}
			if (baseAnimation.IsPlaying("sit_down") && baseAnimation["sit_down"].normalizedTime >= 1f)
			{
				playAnimation("sit_idle");
			}
			if ((getdownTime <= 0f || isAlarm) && baseAnimation.IsPlaying("sit_idle"))
			{
				crossFadeIfNotPlaying("sit_getup", 0.1f);
			}
			if (baseAnimation.IsPlaying("sit_getup") && baseAnimation["sit_getup"].normalizedTime >= 1f)
			{
				idle();
			}
		}
		else if (state == TitanState.recover)
		{
			getdownTime -= Time.deltaTime;
			if (getdownTime <= 0f)
			{
				idle();
			}
			if (baseAnimation.IsPlaying("idle_recovery") && baseAnimation["idle_recovery"].normalizedTime >= 1f)
			{
				idle();
			}
		}
	}

	public void updateCollider()
	{
		if (colliderEnabled)
		{
			if (isHooked || myTitanTrigger.isCollide || isLook || isThunderSpear)
			{
				return;
			}
			foreach (Collider baseCollider in baseColliders)
			{
				if (baseCollider != null)
				{
					baseCollider.enabled = false;
				}
			}
			colliderEnabled = false;
		}
		else
		{
			if (!isHooked && !myTitanTrigger.isCollide && !isLook && !isThunderSpear)
			{
				return;
			}
			foreach (Collider baseCollider2 in baseColliders)
			{
				if (baseCollider2 != null)
				{
					baseCollider2.enabled = true;
				}
			}
			colliderEnabled = true;
		}
	}

	public void updateLabel()
	{
		if (healthLabel != null && healthLabel.GetComponent<UILabel>().isVisible)
		{
			healthLabel.transform.LookAt(2f * healthLabel.transform.position - Camera.main.transform.position);
		}
	}

	private void wander(float sbtime = 0f)
	{
		state = TitanState.wander;
		crossFade(runAnimation, 0.5f);
	}
}
