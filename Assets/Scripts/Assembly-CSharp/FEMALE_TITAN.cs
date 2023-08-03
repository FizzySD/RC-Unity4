using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CustomSkins;
using Photon;
using Settings;
using UI;
using UnityEngine;

public class FEMALE_TITAN : Photon.MonoBehaviour
{
	[CompilerGenerated]
	private static Dictionary<string, int> fswitchSmap1;

	[CompilerGenerated]
	private static Dictionary<string, int> fswitchSmap2;

	[CompilerGenerated]
	private static Dictionary<string, int> fswitchSmap3;

	private Vector3 abnorma_jump_bite_horizon_v;

	public int AnkleLHP = 200;

	private int AnkleLHPMAX = 200;

	public int AnkleRHP = 200;

	private int AnkleRHPMAX = 200;

	private string attackAnimation;

	private float attackCheckTime;

	private float attackCheckTimeA;

	private float attackCheckTimeB;

	private bool attackChkOnce;

	public float attackDistance = 13f;

	private bool attacked;

	public float attackWait = 1f;

	private float attention = 10f;

	public GameObject bottomObject;

	public float chaseDistance = 80f;

	private Transform checkHitCapsuleEnd;

	private Vector3 checkHitCapsuleEndOld;

	private float checkHitCapsuleR;

	private Transform checkHitCapsuleStart;

	public GameObject currentCamera;

	private Transform currentGrabHand;

	private float desDeg;

	private float dieTime;

	private GameObject eren;

	[CompilerGenerated]
	private static Dictionary<string, int> fswitchsmap1;

	[CompilerGenerated]
	private static Dictionary<string, int> fswitchsmap2;

	[CompilerGenerated]
	private static Dictionary<string, int> fswitchsmap3;

	private string fxName;

	private Vector3 fxPosition;

	private Quaternion fxRotation;

	private GameObject grabbedTarget;

	public GameObject grabTF;

	private float gravity = 120f;

	private bool grounded;

	public bool hasDie;

	private bool hasDieSteam;

	public bool hasspawn;

	public GameObject healthLabel;

	public float healthTime;

	private string hitAnimation;

	private bool isAttackMoveByCore;

	private bool isGrabHandLeft;

	public float lagMax;

	public int maxHealth;

	public float maxVelocityChange = 80f;

	public static float minusDistance = 99999f;

	public static GameObject minusDistanceEnemy;

	public float myDistance;

	public GameObject myHero;

	public int NapeArmor = 1000;

	private bool needFreshCorePosition;

	private string nextAttackAnimation;

	private Vector3 oldCorePosition;

	private float sbtime;

	public float size;

	public float speed = 80f;

	private bool startJump;

	private string state = "idle";

	private int stepSoundPhase = 2;

	private float tauntTime;

	private string turnAnimation;

	private float turnDeg;

	private GameObject whoHasTauntMe;

	private AnnieCustomSkinLoader _customSkinLoader;

	private void attack(string type)
	{
		state = "attack";
		attacked = false;
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
		startJump = false;
		attackChkOnce = false;
		nextAttackAnimation = null;
		fxName = null;
		isAttackMoveByCore = false;
		attackCheckTime = 0f;
		attackCheckTimeA = 0f;
		attackCheckTimeB = 0f;
		fxRotation = Quaternion.Euler(270f, 0f, 0f);
		switch (type)
		{
		case "combo_1":
			attackCheckTimeA = 0.63f;
			attackCheckTimeB = 0.8f;
			checkHitCapsuleEnd = base.transform.Find("Amarture/Core/Controller_Body/hip/thigh_R/shin_R/foot_R");
			checkHitCapsuleStart = base.transform.Find("Amarture/Core/Controller_Body/hip/thigh_R");
			checkHitCapsuleR = 5f;
			isAttackMoveByCore = true;
			nextAttackAnimation = "combo_2";
			break;
		case "combo_2":
			attackCheckTimeA = 0.27f;
			attackCheckTimeB = 0.43f;
			checkHitCapsuleEnd = base.transform.Find("Amarture/Core/Controller_Body/hip/thigh_L/shin_L/foot_L");
			checkHitCapsuleStart = base.transform.Find("Amarture/Core/Controller_Body/hip/thigh_L");
			checkHitCapsuleR = 5f;
			isAttackMoveByCore = true;
			nextAttackAnimation = "combo_3";
			break;
		case "combo_3":
			attackCheckTimeA = 0.15f;
			attackCheckTimeB = 0.3f;
			checkHitCapsuleEnd = base.transform.Find("Amarture/Core/Controller_Body/hip/thigh_R/shin_R/foot_R");
			checkHitCapsuleStart = base.transform.Find("Amarture/Core/Controller_Body/hip/thigh_R");
			checkHitCapsuleR = 5f;
			isAttackMoveByCore = true;
			break;
		case "combo_blind_1":
			isAttackMoveByCore = true;
			attackCheckTimeA = 0.72f;
			attackCheckTimeB = 0.83f;
			checkHitCapsuleStart = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R");
			checkHitCapsuleEnd = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R/hand_R_001");
			checkHitCapsuleR = 4f;
			nextAttackAnimation = "combo_blind_2";
			break;
		case "combo_blind_2":
			isAttackMoveByCore = true;
			attackCheckTimeA = 0.5f;
			attackCheckTimeB = 0.6f;
			checkHitCapsuleStart = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R");
			checkHitCapsuleEnd = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R/hand_R_001");
			checkHitCapsuleR = 4f;
			nextAttackAnimation = "combo_blind_3";
			break;
		case "combo_blind_3":
			isAttackMoveByCore = true;
			attackCheckTimeA = 0.2f;
			attackCheckTimeB = 0.28f;
			checkHitCapsuleStart = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R");
			checkHitCapsuleEnd = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R/hand_R_001");
			checkHitCapsuleR = 4f;
			break;
		case "front":
			isAttackMoveByCore = true;
			attackCheckTimeA = 0.44f;
			attackCheckTimeB = 0.55f;
			checkHitCapsuleStart = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R");
			checkHitCapsuleEnd = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R/hand_R_001");
			checkHitCapsuleR = 4f;
			break;
		case "jumpCombo_1":
			isAttackMoveByCore = false;
			nextAttackAnimation = "jumpCombo_2";
			abnorma_jump_bite_horizon_v = Vector3.zero;
			break;
		case "jumpCombo_2":
			isAttackMoveByCore = false;
			attackCheckTimeA = 0.48f;
			attackCheckTimeB = 0.7f;
			checkHitCapsuleStart = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R");
			checkHitCapsuleEnd = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R/hand_R_001");
			checkHitCapsuleR = 4f;
			nextAttackAnimation = "jumpCombo_3";
			break;
		case "jumpCombo_3":
			isAttackMoveByCore = false;
			checkHitCapsuleEnd = base.transform.Find("Amarture/Core/Controller_Body/hip/thigh_L/shin_L/foot_L");
			checkHitCapsuleStart = base.transform.Find("Amarture/Core/Controller_Body/hip/thigh_L");
			checkHitCapsuleR = 5f;
			attackCheckTimeA = 0.22f;
			attackCheckTimeB = 0.42f;
			break;
		case "jumpCombo_4":
			isAttackMoveByCore = false;
			break;
		case "sweep":
			isAttackMoveByCore = true;
			attackCheckTimeA = 0.39f;
			attackCheckTimeB = 0.6f;
			checkHitCapsuleEnd = base.transform.Find("Amarture/Core/Controller_Body/hip/thigh_R/shin_R/foot_R");
			checkHitCapsuleStart = base.transform.Find("Amarture/Core/Controller_Body/hip/thigh_R");
			checkHitCapsuleR = 5f;
			break;
		case "sweep_back":
			isAttackMoveByCore = true;
			attackCheckTimeA = 0.41f;
			attackCheckTimeB = 0.48f;
			checkHitCapsuleStart = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R");
			checkHitCapsuleEnd = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R/hand_R_001");
			checkHitCapsuleR = 4f;
			break;
		case "sweep_front_left":
			isAttackMoveByCore = true;
			attackCheckTimeA = 0.53f;
			attackCheckTimeB = 0.63f;
			checkHitCapsuleStart = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R");
			checkHitCapsuleEnd = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R/hand_R_001");
			checkHitCapsuleR = 4f;
			break;
		case "sweep_front_right":
			isAttackMoveByCore = true;
			attackCheckTimeA = 0.5f;
			attackCheckTimeB = 0.62f;
			checkHitCapsuleStart = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L");
			checkHitCapsuleEnd = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L/forearm_L/hand_L/hand_L_001");
			checkHitCapsuleR = 4f;
			break;
		case "sweep_head_b_l":
			isAttackMoveByCore = true;
			attackCheckTimeA = 0.4f;
			attackCheckTimeB = 0.51f;
			checkHitCapsuleStart = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L");
			checkHitCapsuleEnd = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L/forearm_L/hand_L/hand_L_001");
			checkHitCapsuleR = 4f;
			break;
		case "sweep_head_b_r":
			isAttackMoveByCore = true;
			attackCheckTimeA = 0.4f;
			attackCheckTimeB = 0.51f;
			checkHitCapsuleStart = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R");
			checkHitCapsuleEnd = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R/hand_R_001");
			checkHitCapsuleR = 4f;
			break;
		}
		checkHitCapsuleEndOld = checkHitCapsuleEnd.transform.position;
		needFreshCorePosition = true;
	}

	private bool attackTarget(GameObject target)
	{
		float num = 0f;
		float num2 = 0f;
		Vector3 vector = target.transform.position - base.transform.position;
		num = (0f - Mathf.Atan2(vector.z, vector.x)) * 57.29578f;
		num2 = 0f - Mathf.DeltaAngle(num, base.gameObject.transform.rotation.eulerAngles.y - 90f);
		if (eren != null && myDistance < 35f)
		{
			attack("combo_1");
			return true;
		}
		int num3 = 0;
		string text = string.Empty;
		ArrayList arrayList = new ArrayList();
		if (myDistance < 40f)
		{
			num3 = ((Mathf.Abs(num2) < 90f) ? ((num2 > 0f) ? 1 : 2) : ((!(num2 > 0f)) ? 3 : 4));
			float num4 = target.transform.position.y - base.transform.position.y;
			if (Mathf.Abs(num2) < 90f)
			{
				if (num4 > 0f && num4 < 12f && myDistance < 22f)
				{
					arrayList.Add("attack_sweep");
				}
				if (num4 >= 55f && num4 < 90f)
				{
					arrayList.Add("attack_jumpCombo_1");
				}
			}
			if (Mathf.Abs(num2) < 90f && num4 > 12f && num4 < 40f)
			{
				arrayList.Add("attack_combo_1");
			}
			if (Mathf.Abs(num2) < 30f)
			{
				if (num4 > 0f && num4 < 12f && myDistance > 20f && myDistance < 30f)
				{
					arrayList.Add("attack_front");
				}
				if (myDistance < 12f && num4 > 33f && num4 < 51f)
				{
					arrayList.Add("grab_up");
				}
			}
			if (Mathf.Abs(num2) > 100f && myDistance < 11f && num4 >= 15f && num4 < 32f)
			{
				arrayList.Add("attack_sweep_back");
			}
			switch (num3)
			{
			case 1:
				if (myDistance >= 11f)
				{
					if (myDistance < 20f)
					{
						if (num4 >= 12f && num4 < 21f)
						{
							arrayList.Add("grab_bottom_right");
						}
						else if (num4 >= 21f && num4 < 32f)
						{
							arrayList.Add("grab_mid_right");
						}
						else if (num4 >= 32f && num4 < 47f)
						{
							arrayList.Add("grab_up_right");
						}
					}
				}
				else if (num4 >= 21f && num4 < 32f)
				{
					arrayList.Add("attack_sweep_front_right");
				}
				break;
			case 2:
				if (myDistance >= 11f)
				{
					if (myDistance < 20f)
					{
						if (num4 >= 12f && num4 < 21f)
						{
							arrayList.Add("grab_bottom_left");
						}
						else if (num4 >= 21f && num4 < 32f)
						{
							arrayList.Add("grab_mid_left");
						}
						else if (num4 >= 32f && num4 < 47f)
						{
							arrayList.Add("grab_up_left");
						}
					}
				}
				else if (num4 >= 21f && num4 < 32f)
				{
					arrayList.Add("attack_sweep_front_left");
				}
				break;
			case 3:
				if (myDistance >= 11f)
				{
					arrayList.Add("turn180");
				}
				else if (num4 >= 33f && num4 < 51f)
				{
					arrayList.Add("attack_sweep_head_b_l");
				}
				break;
			case 4:
				if (myDistance >= 11f)
				{
					arrayList.Add("turn180");
				}
				else if (num4 >= 33f && num4 < 51f)
				{
					arrayList.Add("attack_sweep_head_b_r");
				}
				break;
			}
		}
		if (arrayList.Count > 0)
		{
			text = (string)arrayList[UnityEngine.Random.Range(0, arrayList.Count)];
		}
		else if (UnityEngine.Random.Range(0, 100) < 10)
		{
			GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
			myHero = array[UnityEngine.Random.Range(0, array.Length)];
			attention = UnityEngine.Random.Range(5f, 10f);
			return true;
		}
		switch (text)
		{
		case "grab_bottom_left":
			grab("bottom_left");
			return true;
		case "grab_bottom_right":
			grab("bottom_right");
			return true;
		case "grab_mid_left":
			grab("mid_left");
			return true;
		case "grab_mid_right":
			grab("mid_right");
			return true;
		case "grab_up":
			grab("up");
			return true;
		case "grab_up_left":
			grab("up_left");
			return true;
		case "grab_up_right":
			grab("up_right");
			return true;
		case "attack_combo_1":
			attack("combo_1");
			return true;
		case "attack_front":
			attack("front");
			return true;
		case "attack_jumpCombo_1":
			attack("jumpCombo_1");
			return true;
		case "attack_sweep":
			attack("sweep");
			return true;
		case "attack_sweep_back":
			attack("sweep_back");
			return true;
		case "attack_sweep_front_left":
			attack("sweep_front_left");
			return true;
		case "attack_sweep_front_right":
			attack("sweep_front_right");
			return true;
		case "attack_sweep_head_b_l":
			attack("sweep_head_b_l");
			return true;
		case "attack_sweep_head_b_r":
			attack("sweep_head_b_r");
			return true;
		case "turn180":
			turn180();
			return true;
		default:
			return false;
		}
	}

	private void Awake()
	{
		base.rigidbody.freezeRotation = true;
		base.rigidbody.useGravity = false;
		_customSkinLoader = base.gameObject.AddComponent<AnnieCustomSkinLoader>();
	}

	public void beTauntedBy(GameObject target, float tauntTime)
	{
		whoHasTauntMe = target;
		this.tauntTime = tauntTime;
	}

	private void chase()
	{
		state = "chase";
		crossFade("run", 0.5f);
	}

	private RaycastHit[] checkHitCapsule(Vector3 start, Vector3 end, float r)
	{
		return Physics.SphereCastAll(start, r, end - start, Vector3.Distance(start, end));
	}

	private GameObject checkIfHitHand(Transform hand)
	{
		float num = 9.6f;
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
				return gameObject;
			}
			if (gameObject.GetComponent<HERO>() != null && !gameObject.GetComponent<HERO>().isInvincible())
			{
				return gameObject;
			}
		}
		return null;
	}

	private GameObject checkIfHitHead(Transform head, float rad)
	{
		float num = rad * 4f;
		GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
		foreach (GameObject gameObject in array)
		{
			if (gameObject.GetComponent<TITAN_EREN>() == null && !gameObject.GetComponent<HERO>().isInvincible())
			{
				float num2 = gameObject.GetComponent<CapsuleCollider>().height * 0.5f;
				if (Vector3.Distance(gameObject.transform.position + Vector3.up * num2, head.transform.position + Vector3.up * 1.5f * 4f) < num + num2)
				{
					return gameObject;
				}
			}
		}
		return null;
	}

	private void crossFade(string aniName, float time)
	{
		base.animation.CrossFade(aniName, time);
		if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && PhotonNetwork.isMasterClient)
		{
			object[] parameters = new object[2] { aniName, time };
			base.photonView.RPC("netCrossFade", PhotonTargets.Others, parameters);
		}
	}

	private void eatSet(GameObject grabTarget)
	{
		if (!grabTarget.GetComponent<HERO>().isGrabbed)
		{
			grabToRight();
			if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && PhotonNetwork.isMasterClient)
			{
				object[] parameters = new object[2]
				{
					base.photonView.viewID,
					false
				};
				grabTarget.GetPhotonView().RPC("netGrabbed", PhotonTargets.All, parameters);
				object[] parameters2 = new object[1] { "grabbed" };
				grabTarget.GetPhotonView().RPC("netPlayAnimation", PhotonTargets.All, parameters2);
				base.photonView.RPC("grabToRight", PhotonTargets.Others);
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
		if (!grabTarget.GetComponent<HERO>().isGrabbed)
		{
			grabToLeft();
			if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && PhotonNetwork.isMasterClient)
			{
				object[] parameters = new object[2]
				{
					base.photonView.viewID,
					true
				};
				grabTarget.GetPhotonView().RPC("netGrabbed", PhotonTargets.All, parameters);
				object[] parameters2 = new object[1] { "grabbed" };
				grabTarget.GetPhotonView().RPC("netPlayAnimation", PhotonTargets.All, parameters2);
				base.photonView.RPC("grabToLeft", PhotonTargets.Others);
			}
			else
			{
				grabTarget.GetComponent<HERO>().grabbed(base.gameObject, true);
				grabTarget.GetComponent<HERO>().animation.Play("grabbed");
			}
		}
	}

	public void erenIsHere(GameObject target)
	{
		myHero = (eren = target);
	}

	private void findNearestFacingHero()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
		GameObject gameObject = null;
		float num = float.PositiveInfinity;
		Vector3 position = base.transform.position;
		float num2 = 0f;
		float num3 = 180f;
		float num4 = 0f;
		GameObject[] array2 = array;
		foreach (GameObject gameObject2 in array2)
		{
			float sqrMagnitude = (gameObject2.transform.position - position).sqrMagnitude;
			if (sqrMagnitude < num)
			{
				Vector3 vector = gameObject2.transform.position - base.transform.position;
				num2 = (0f - Mathf.Atan2(vector.z, vector.x)) * 57.29578f;
				num4 = 0f - Mathf.DeltaAngle(num2, base.gameObject.transform.rotation.eulerAngles.y - 90f);
				if (Mathf.Abs(num4) < num3)
				{
					gameObject = gameObject2;
					num = sqrMagnitude;
				}
			}
		}
		if (gameObject != null)
		{
			myHero = gameObject;
			tauntTime = 5f;
		}
	}

	private void findNearestHero()
	{
		myHero = getNearestHero();
		attention = UnityEngine.Random.Range(5f, 10f);
	}

	private void FixedUpdate()
	{
		if ((GameMenu.Paused && IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE) || (IN_GAME_MAIN_CAMERA.gametype != 0 && !base.photonView.isMine))
		{
			return;
		}
		if (bottomObject.GetComponent<CheckHitGround>().isGrounded)
		{
			grounded = true;
			bottomObject.GetComponent<CheckHitGround>().isGrounded = false;
		}
		else
		{
			grounded = false;
		}
		if (needFreshCorePosition)
		{
			oldCorePosition = base.transform.position - base.transform.Find("Amarture/Core").position;
			needFreshCorePosition = false;
		}
		if ((state == "attack" && isAttackMoveByCore) || state == "hit" || state == "turn180" || state == "anklehurt")
		{
			Vector3 vector = base.transform.position - base.transform.Find("Amarture/Core").position - oldCorePosition;
			base.rigidbody.velocity = vector / Time.deltaTime + Vector3.up * base.rigidbody.velocity.y;
			oldCorePosition = base.transform.position - base.transform.Find("Amarture/Core").position;
		}
		else if (state == "chase")
		{
			if (myHero == null)
			{
				return;
			}
			Vector3 vector2 = base.transform.forward * speed;
			Vector3 velocity = base.rigidbody.velocity;
			Vector3 force = vector2 - velocity;
			force.y = 0f;
			base.rigidbody.AddForce(force, ForceMode.VelocityChange);
			float num = 0f;
			Vector3 vector3 = myHero.transform.position - base.transform.position;
			num = (0f - Mathf.Atan2(vector3.z, vector3.x)) * 57.29578f;
			float num2 = 0f - Mathf.DeltaAngle(num, base.gameObject.transform.rotation.eulerAngles.y - 90f);
			base.gameObject.transform.rotation = Quaternion.Lerp(base.gameObject.transform.rotation, Quaternion.Euler(0f, base.gameObject.transform.rotation.eulerAngles.y + num2, 0f), speed * Time.deltaTime);
		}
		else if (grounded && !base.animation.IsPlaying("attack_jumpCombo_1"))
		{
			base.rigidbody.AddForce(new Vector3(0f - base.rigidbody.velocity.x, 0f, 0f - base.rigidbody.velocity.z), ForceMode.VelocityChange);
		}
		base.rigidbody.AddForce(new Vector3(0f, (0f - gravity) * base.rigidbody.mass, 0f));
	}

	private void getDown()
	{
		state = "anklehurt";
		playAnimation("legHurt");
		AnkleRHP = AnkleRHPMAX;
		AnkleLHP = AnkleLHPMAX;
		needFreshCorePosition = true;
	}

	private GameObject getNearestHero()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
		GameObject result = null;
		float num = float.PositiveInfinity;
		Vector3 position = base.transform.position;
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			if ((gameObject.GetComponent<HERO>() == null || !gameObject.GetComponent<HERO>().HasDied()) && (gameObject.GetComponent<TITAN_EREN>() == null || !gameObject.GetComponent<TITAN_EREN>().hasDied))
			{
				float sqrMagnitude = (gameObject.transform.position - position).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					result = gameObject;
					num = sqrMagnitude;
				}
			}
		}
		return result;
	}

	private float getNearestHeroDistance()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
		float num = float.PositiveInfinity;
		Vector3 position = base.transform.position;
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			float magnitude = (gameObject.transform.position - position).magnitude;
			if (magnitude < num)
			{
				num = magnitude;
			}
		}
		return num;
	}

	private void grab(string type)
	{
		state = "grab";
		attacked = false;
		attackAnimation = type;
		if (base.animation.IsPlaying("attack_grab_" + type))
		{
			base.animation["attack_grab_" + type].normalizedTime = 0f;
			playAnimation("attack_grab_" + type);
		}
		else
		{
			crossFade("attack_grab_" + type, 0.1f);
		}
		isGrabHandLeft = true;
		grabbedTarget = null;
		attackCheckTime = 0f;
		switch (type)
		{
		case "bottom_left":
			attackCheckTimeA = 0.28f;
			attackCheckTimeB = 0.38f;
			attackCheckTime = 0.65f;
			isGrabHandLeft = false;
			break;
		case "bottom_right":
			attackCheckTimeA = 0.27f;
			attackCheckTimeB = 0.37f;
			attackCheckTime = 0.65f;
			break;
		case "mid_left":
			attackCheckTimeA = 0.27f;
			attackCheckTimeB = 0.37f;
			attackCheckTime = 0.65f;
			isGrabHandLeft = false;
			break;
		case "mid_right":
			attackCheckTimeA = 0.27f;
			attackCheckTimeB = 0.36f;
			attackCheckTime = 0.66f;
			break;
		case "up":
			attackCheckTimeA = 0.25f;
			attackCheckTimeB = 0.32f;
			attackCheckTime = 0.67f;
			break;
		case "up_left":
			attackCheckTimeA = 0.26f;
			attackCheckTimeB = 0.4f;
			attackCheckTime = 0.66f;
			break;
		case "up_right":
			attackCheckTimeA = 0.26f;
			attackCheckTimeB = 0.4f;
			attackCheckTime = 0.66f;
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
	public void grabbedTargetEscape()
	{
		grabbedTarget = null;
	}

	[RPC]
	public void grabToLeft()
	{
		Transform transform = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L/forearm_L/hand_L/hand_L_001");
		grabTF.transform.parent = transform;
		grabTF.transform.parent = transform;
		grabTF.transform.position = transform.GetComponent<SphereCollider>().transform.position;
		grabTF.transform.rotation = transform.GetComponent<SphereCollider>().transform.rotation;
		grabTF.transform.localPosition -= Vector3.right * transform.GetComponent<SphereCollider>().radius * 0.3f;
		grabTF.transform.localPosition -= Vector3.up * transform.GetComponent<SphereCollider>().radius * 0.51f;
		grabTF.transform.localPosition -= Vector3.forward * transform.GetComponent<SphereCollider>().radius * 0.3f;
		grabTF.transform.localRotation = Quaternion.Euler(grabTF.transform.localRotation.eulerAngles.x, grabTF.transform.localRotation.eulerAngles.y + 180f, grabTF.transform.localRotation.eulerAngles.z + 180f);
	}

	[RPC]
	public void grabToRight()
	{
		Transform transform = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R/hand_R_001");
		grabTF.transform.parent = transform;
		grabTF.transform.position = transform.GetComponent<SphereCollider>().transform.position;
		grabTF.transform.rotation = transform.GetComponent<SphereCollider>().transform.rotation;
		grabTF.transform.localPosition -= Vector3.right * transform.GetComponent<SphereCollider>().radius * 0.3f;
		grabTF.transform.localPosition += Vector3.up * transform.GetComponent<SphereCollider>().radius * 0.51f;
		grabTF.transform.localPosition -= Vector3.forward * transform.GetComponent<SphereCollider>().radius * 0.3f;
		grabTF.transform.localRotation = Quaternion.Euler(grabTF.transform.localRotation.eulerAngles.x, grabTF.transform.localRotation.eulerAngles.y + 180f, grabTF.transform.localRotation.eulerAngles.z);
	}

	public void hit(int dmg)
	{
		NapeArmor -= dmg;
		if (NapeArmor <= 0)
		{
			NapeArmor = 0;
		}
	}

	public void hitAnkleL(int dmg)
	{
		if (!hasDie && state != "anklehurt")
		{
			AnkleLHP -= dmg;
			if (AnkleLHP <= 0)
			{
				getDown();
			}
		}
	}

	[RPC]
	public void hitAnkleLRPC(int viewID, int dmg)
	{
		if (hasDie || !(state != "anklehurt"))
		{
			return;
		}
		PhotonView photonView = PhotonView.Find(viewID);
		if (!(photonView != null))
		{
			return;
		}
		if (grabbedTarget != null)
		{
			grabbedTarget.GetPhotonView().RPC("netUngrabbed", PhotonTargets.All);
		}
		if ((photonView.gameObject.transform.position - base.transform.Find("Amarture/Core/Controller_Body").transform.position).magnitude < 20f)
		{
			AnkleLHP -= dmg;
			if (AnkleLHP <= 0)
			{
				getDown();
			}
			GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().sendKillInfo(false, (string)photonView.owner.customProperties[PhotonPlayerProperty.name], true, "Female Titan's ankle", dmg);
			object[] parameters = new object[1] { dmg };
			GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().photonView.RPC("netShowDamage", photonView.owner, parameters);
		}
	}

	public void hitAnkleR(int dmg)
	{
		if (!hasDie && state != "anklehurt")
		{
			AnkleRHP -= dmg;
			if (AnkleRHP <= 0)
			{
				getDown();
			}
		}
	}

	[RPC]
	public void hitAnkleRRPC(int viewID, int dmg)
	{
		if (hasDie || !(state != "anklehurt"))
		{
			return;
		}
		PhotonView photonView = PhotonView.Find(viewID);
		if (!(photonView != null))
		{
			return;
		}
		if (grabbedTarget != null)
		{
			grabbedTarget.GetPhotonView().RPC("netUngrabbed", PhotonTargets.All);
		}
		if ((photonView.gameObject.transform.position - base.transform.Find("Amarture/Core/Controller_Body").transform.position).magnitude < 20f)
		{
			AnkleRHP -= dmg;
			if (AnkleRHP <= 0)
			{
				getDown();
			}
			GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().sendKillInfo(false, (string)photonView.owner.customProperties[PhotonPlayerProperty.name], true, "Female Titan's ankle", dmg);
			object[] parameters = new object[1] { dmg };
			GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().photonView.RPC("netShowDamage", photonView.owner, parameters);
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
		if (!hasDie)
		{
			if (grabbedTarget != null)
			{
				grabbedTarget.GetPhotonView().RPC("netUngrabbed", PhotonTargets.All);
			}
			Transform transform = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck");
			PhotonView photonView = PhotonView.Find(viewID);
			if (photonView != null && (photonView.gameObject.transform.position - transform.transform.position).magnitude < 20f)
			{
				justHitEye();
			}
		}
	}

	private void idle(float sbtime = 0f)
	{
		this.sbtime = sbtime;
		this.sbtime = Mathf.Max(0.5f, this.sbtime);
		state = "idle";
		crossFade("idle", 0.2f);
	}

	public bool IsGrounded()
	{
		return bottomObject.GetComponent<CheckHitGround>().isGrounded;
	}

	private void justEatHero(GameObject target, Transform hand)
	{
		if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && PhotonNetwork.isMasterClient)
		{
			if (!target.GetComponent<HERO>().HasDied())
			{
				target.GetComponent<HERO>().markDie();
				object[] parameters = new object[2] { -1, "Female Titan" };
				target.GetComponent<HERO>().photonView.RPC("netDie2", PhotonTargets.All, parameters);
			}
		}
		else if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
		{
			target.GetComponent<HERO>().die2(hand);
		}
	}

	private void justHitEye()
	{
		attack("combo_blind_1");
	}

	private void killPlayer(GameObject hitHero)
	{
		if (!(hitHero != null))
		{
			return;
		}
		Vector3 position = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest").position;
		if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
		{
			if (!hitHero.GetComponent<HERO>().HasDied())
			{
				hitHero.GetComponent<HERO>().die((hitHero.transform.position - position) * 15f * 4f, false);
			}
		}
		else if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && PhotonNetwork.isMasterClient && !hitHero.GetComponent<HERO>().HasDied())
		{
			hitHero.GetComponent<HERO>().markDie();
			object[] parameters = new object[5]
			{
				(hitHero.transform.position - position) * 15f * 4f,
				false,
				-1,
				"Female Titan",
				true
			};
			hitHero.GetComponent<HERO>().photonView.RPC("netDie", PhotonTargets.All, parameters);
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
			healthLabel.transform.localPosition = new Vector3(0f, 52f, 0f);
			float num = 4f;
			if (size > 0f && size < 1f)
			{
				num = 4f / size;
				num = Mathf.Min(num, 15f);
			}
			healthLabel.transform.localScale = new Vector3(num, num, num);
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
		if (base.animation.IsPlaying("run"))
		{
			if (base.animation["run"].normalizedTime % 1f > 0.1f && base.animation["run"].normalizedTime % 1f < 0.6f && stepSoundPhase == 2)
			{
				stepSoundPhase = 1;
				Transform transform = base.transform.Find("snd_titan_foot");
				transform.GetComponent<AudioSource>().Stop();
				transform.GetComponent<AudioSource>().Play();
			}
			if (base.animation["run"].normalizedTime % 1f > 0.6f && stepSoundPhase == 1)
			{
				stepSoundPhase = 2;
				Transform transform2 = base.transform.Find("snd_titan_foot");
				transform2.GetComponent<AudioSource>().Stop();
				transform2.GetComponent<AudioSource>().Play();
			}
		}
		if (IN_GAME_MAIN_CAMERA.gametype != 0)
		{
			bool isMine = base.photonView.isMine;
		}
	}

	public void lateUpdate2()
	{
		if (GameMenu.Paused && IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
		{
			return;
		}
		if (base.animation.IsPlaying("run"))
		{
			if (base.animation["run"].normalizedTime % 1f > 0.1f && base.animation["run"].normalizedTime % 1f < 0.6f && stepSoundPhase == 2)
			{
				stepSoundPhase = 1;
				Transform transform = base.transform.Find("snd_titan_foot");
				transform.GetComponent<AudioSource>().Stop();
				transform.GetComponent<AudioSource>().Play();
			}
			if (base.animation["run"].normalizedTime % 1f > 0.6f && stepSoundPhase == 1)
			{
				stepSoundPhase = 2;
				Transform transform2 = base.transform.Find("snd_titan_foot");
				transform2.GetComponent<AudioSource>().Stop();
				transform2.GetComponent<AudioSource>().Play();
			}
		}
		updateLabel();
		healthTime -= Time.deltaTime;
	}

	public void loadskin()
	{
		BaseCustomSkinSettings<ShifterCustomSkinSet> shifter = SettingsManager.CustomSkinSettings.Shifter;
		string value = ((ShifterCustomSkinSet)shifter.GetSelectedSet()).Annie.Value;
		if (shifter.SkinsEnabled.Value && TextureDownloader.ValidTextureURL(value))
		{
			base.photonView.RPC("loadskinRPC", PhotonTargets.AllBuffered, value);
		}
	}

	public IEnumerator loadskinE(string url)
	{
		while (!hasspawn)
		{
			yield return null;
		}
		yield return StartCoroutine(_customSkinLoader.LoadSkinsFromRPC(new object[1] { url }));
	}

	[RPC]
	public void loadskinRPC(string url, PhotonMessageInfo info)
	{
		if (info.sender == base.photonView.owner)
		{
			BaseCustomSkinSettings<ShifterCustomSkinSet> shifter = SettingsManager.CustomSkinSettings.Shifter;
			if (shifter.SkinsEnabled.Value && (!shifter.SkinsLocal.Value || base.photonView.isMine))
			{
				StartCoroutine(loadskinE(url));
			}
		}
	}

	[RPC]
	private void netCrossFade(string aniName, float time)
	{
		base.animation.CrossFade(aniName, time);
	}

	[RPC]
	public void netDie()
	{
		if (!hasDie)
		{
			hasDie = true;
			crossFade("die", 0.05f);
		}
	}

	[RPC]
	private void netPlayAnimation(string aniName)
	{
		base.animation.Play(aniName);
	}

	[RPC]
	private void netPlayAnimationAt(string aniName, float normalizedTime)
	{
		base.animation.Play(aniName);
		base.animation[aniName].normalizedTime = normalizedTime;
	}

	private void OnDestroy()
	{
		if (GameObject.Find("MultiplayerManager") != null)
		{
			GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().removeFT(this);
		}
	}

	private void playAnimation(string aniName)
	{
		base.animation.Play(aniName);
		if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && PhotonNetwork.isMasterClient)
		{
			object[] parameters = new object[1] { aniName };
			base.photonView.RPC("netPlayAnimation", PhotonTargets.Others, parameters);
		}
	}

	private void playAnimationAt(string aniName, float normalizedTime)
	{
		base.animation.Play(aniName);
		base.animation[aniName].normalizedTime = normalizedTime;
		if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && PhotonNetwork.isMasterClient)
		{
			object[] parameters = new object[2] { aniName, normalizedTime };
			base.photonView.RPC("netPlayAnimationAt", PhotonTargets.Others, parameters);
		}
	}

	private void playSound(string sndname)
	{
		playsoundRPC(sndname);
		if (Network.peerType == NetworkPeerType.Server)
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

	[RPC]
	public void setSize(float size, PhotonMessageInfo info)
	{
		size = Mathf.Clamp(size, 0.2f, 30f);
		if (info.sender.isMasterClient)
		{
			base.transform.localScale *= size * 0.25f;
			this.size = size;
		}
	}

	private void Start()
	{
		startMain();
		size = 4f;
		if (Minimap.instance != null)
		{
			Minimap.instance.TrackGameObjectOnMinimap(base.gameObject, Color.black, false, true);
		}
		if (base.photonView.isMine)
		{
			if (SettingsManager.LegacyGameSettings.TitanSizeEnabled.Value)
			{
				float value = SettingsManager.LegacyGameSettings.TitanSizeMin.Value;
				float value2 = SettingsManager.LegacyGameSettings.TitanSizeMax.Value;
				size = UnityEngine.Random.Range(value, value2);
				base.photonView.RPC("setSize", PhotonTargets.AllBuffered, size);
			}
			lagMax = 150f + size * 3f;
			healthTime = 0f;
			maxHealth = NapeArmor;
			if (SettingsManager.LegacyGameSettings.TitanHealthMode.Value > 0)
			{
				maxHealth = (NapeArmor = UnityEngine.Random.Range(SettingsManager.LegacyGameSettings.TitanHealthMin.Value, SettingsManager.LegacyGameSettings.TitanHealthMax.Value));
			}
			if (NapeArmor > 0)
			{
				base.photonView.RPC("labelRPC", PhotonTargets.AllBuffered, NapeArmor, maxHealth);
			}
			loadskin();
		}
		hasspawn = true;
	}

	private void startMain()
	{
		GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().addFT(this);
		base.name = "Female Titan";
		grabTF = new GameObject();
		grabTF.name = "titansTmpGrabTF";
		currentCamera = GameObject.Find("MainCamera");
		oldCorePosition = base.transform.position - base.transform.Find("Amarture/Core").position;
		if (myHero == null)
		{
			findNearestHero();
		}
		foreach (AnimationState item in base.animation)
		{
			item.speed = 0.7f;
		}
		base.animation["turn180"].speed = 0.5f;
		NapeArmor = 1000;
		AnkleLHP = 50;
		AnkleRHP = 50;
		AnkleLHPMAX = 50;
		AnkleRHPMAX = 50;
		bool flag = false;
		if (LevelInfo.getInfo(FengGameManagerMKII.level).respawnMode == RespawnMode.NEVER)
		{
			flag = true;
		}
		if (IN_GAME_MAIN_CAMERA.difficulty == 0)
		{
			NapeArmor = ((!flag) ? 1000 : 1000);
			AnkleLHP = (AnkleLHPMAX = ((!flag) ? 50 : 50));
			AnkleRHP = (AnkleRHPMAX = ((!flag) ? 50 : 50));
		}
		else if (IN_GAME_MAIN_CAMERA.difficulty == 1)
		{
			NapeArmor = ((!flag) ? 3000 : 2500);
			AnkleLHP = (AnkleLHPMAX = ((!flag) ? 200 : 100));
			AnkleRHP = (AnkleRHPMAX = ((!flag) ? 200 : 100));
			foreach (AnimationState item2 in base.animation)
			{
				item2.speed = 0.7f;
			}
			base.animation["turn180"].speed = 0.7f;
		}
		else if (IN_GAME_MAIN_CAMERA.difficulty == 2)
		{
			NapeArmor = ((!flag) ? 6000 : 4000);
			AnkleLHP = (AnkleLHPMAX = ((!flag) ? 1000 : 200));
			AnkleRHP = (AnkleRHPMAX = ((!flag) ? 1000 : 200));
			foreach (AnimationState item3 in base.animation)
			{
				item3.speed = 1f;
			}
			base.animation["turn180"].speed = 0.9f;
		}
		if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.PVP_CAPTURE)
		{
			NapeArmor = (int)((float)NapeArmor * 0.8f);
		}
		base.animation["legHurt"].speed = 1f;
		base.animation["legHurt_loop"].speed = 1f;
		base.animation["legHurt_getup"].speed = 1f;
	}

	[RPC]
	public void titanGetHit(int viewID, int speed)
	{
		Transform transform = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck");
		PhotonView photonView = PhotonView.Find(viewID);
		if (!(photonView != null) || !((photonView.gameObject.transform.position - transform.transform.position).magnitude < lagMax) || !(healthTime <= 0f))
		{
			return;
		}
		if (!SettingsManager.LegacyGameSettings.TitanArmorEnabled.Value || speed >= SettingsManager.LegacyGameSettings.TitanArmor.Value)
		{
			NapeArmor -= speed;
		}
		if ((float)maxHealth > 0f)
		{
			base.photonView.RPC("labelRPC", PhotonTargets.AllBuffered, NapeArmor, maxHealth);
		}
		if (NapeArmor <= 0)
		{
			NapeArmor = 0;
			if (!hasDie)
			{
				base.photonView.RPC("netDie", PhotonTargets.OthersBuffered);
				if (grabbedTarget != null)
				{
					grabbedTarget.GetPhotonView().RPC("netUngrabbed", PhotonTargets.All);
				}
				netDie();
				GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().titanGetKill(photonView.owner, speed, base.name);
			}
		}
		else
		{
			GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().sendKillInfo(false, (string)photonView.owner.customProperties[PhotonPlayerProperty.name], true, "Female Titan's neck", speed);
			object[] parameters = new object[1] { speed };
			GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().photonView.RPC("netShowDamage", photonView.owner, parameters);
		}
		healthTime = 0.2f;
	}

	private void turn(float d)
	{
		if (d > 0f)
		{
			turnAnimation = "turnaround1";
		}
		else
		{
			turnAnimation = "turnaround2";
		}
		playAnimation(turnAnimation);
		base.animation[turnAnimation].time = 0f;
		d = Mathf.Clamp(d, -120f, 120f);
		turnDeg = d;
		desDeg = base.gameObject.transform.rotation.eulerAngles.y + turnDeg;
		state = "turn";
	}

	private void turn180()
	{
		turnAnimation = "turn180";
		playAnimation(turnAnimation);
		base.animation[turnAnimation].time = 0f;
		state = "turn180";
		needFreshCorePosition = true;
	}

	public void update()
	{
		if ((GameMenu.Paused && IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE) || (IN_GAME_MAIN_CAMERA.gametype != 0 && !base.photonView.isMine))
		{
			return;
		}
		if (hasDie)
		{
			dieTime += Time.deltaTime;
			if (base.animation["die"].normalizedTime >= 1f)
			{
				playAnimation("die_cry");
				if (IN_GAME_MAIN_CAMERA.gamemode != GAMEMODE.PVP_CAPTURE)
				{
					for (int i = 0; i < 15; i++)
					{
						GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().randomSpawnOneTitan("titanRespawn", 50)
							.GetComponent<TITAN>()
							.beTauntedBy(base.gameObject, 20f);
					}
				}
			}
			if (dieTime > 2f && !hasDieSteam)
			{
				hasDieSteam = true;
				if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
				{
					GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("FX/FXtitanDie1"));
					gameObject.transform.position = base.transform.Find("Amarture/Core/Controller_Body/hip").position;
					gameObject.transform.localScale = base.transform.localScale;
				}
				else if (base.photonView.isMine)
				{
					PhotonNetwork.Instantiate("FX/FXtitanDie1", base.transform.Find("Amarture/Core/Controller_Body/hip").position, Quaternion.Euler(-90f, 0f, 0f), 0).transform.localScale = base.transform.localScale;
				}
			}
			if (dieTime > ((IN_GAME_MAIN_CAMERA.gamemode != GAMEMODE.PVP_CAPTURE) ? 20f : 5f))
			{
				if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
				{
					GameObject gameObject2 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("FX/FXtitanDie"));
					gameObject2.transform.position = base.transform.Find("Amarture/Core/Controller_Body/hip").position;
					gameObject2.transform.localScale = base.transform.localScale;
					UnityEngine.Object.Destroy(base.gameObject);
				}
				else if (base.photonView.isMine)
				{
					PhotonNetwork.Instantiate("FX/FXtitanDie", base.transform.Find("Amarture/Core/Controller_Body/hip").position, Quaternion.Euler(-90f, 0f, 0f), 0).transform.localScale = base.transform.localScale;
					PhotonNetwork.Destroy(base.gameObject);
				}
			}
			return;
		}
		if (attention > 0f)
		{
			attention -= Time.deltaTime;
			if (attention < 0f)
			{
				attention = 0f;
				GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
				myHero = array[UnityEngine.Random.Range(0, array.Length)];
				attention = UnityEngine.Random.Range(5f, 10f);
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
		}
		if (eren != null)
		{
			if (!eren.GetComponent<TITAN_EREN>().hasDied)
			{
				myHero = eren;
			}
			else
			{
				eren = null;
				myHero = null;
			}
		}
		if (myHero == null)
		{
			findNearestHero();
			if (myHero != null)
			{
				return;
			}
		}
		if (myHero == null)
		{
			myDistance = float.MaxValue;
		}
		else
		{
			myDistance = Mathf.Sqrt((myHero.transform.position.x - base.transform.position.x) * (myHero.transform.position.x - base.transform.position.x) + (myHero.transform.position.z - base.transform.position.z) * (myHero.transform.position.z - base.transform.position.z));
		}
		if (state == "idle")
		{
			if (!(myHero != null))
			{
				return;
			}
			float num = 0f;
			float num2 = 0f;
			Vector3 vector = myHero.transform.position - base.transform.position;
			num = (0f - Mathf.Atan2(vector.z, vector.x)) * 57.29578f;
			num2 = 0f - Mathf.DeltaAngle(num, base.gameObject.transform.rotation.eulerAngles.y - 90f);
			if (attackTarget(myHero))
			{
				return;
			}
			if (Mathf.Abs(num2) < 90f)
			{
				chase();
			}
			else if (UnityEngine.Random.Range(0, 100) < 1)
			{
				turn180();
			}
			else if (Mathf.Abs(num2) > 100f)
			{
				if (UnityEngine.Random.Range(0, 100) < 10)
				{
					turn180();
				}
			}
			else if (Mathf.Abs(num2) > 45f && UnityEngine.Random.Range(0, 100) < 30)
			{
				turn(num2);
			}
		}
		else if (state == "attack")
		{
			if (!attacked && attackCheckTime != 0f && base.animation["attack_" + attackAnimation].normalizedTime >= attackCheckTime)
			{
				attacked = true;
				fxPosition = base.transform.Find("ap_" + attackAnimation).position;
				GameObject gameObject3 = ((IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.MULTIPLAYER || !PhotonNetwork.isMasterClient) ? ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("FX/" + fxName), fxPosition, fxRotation)) : PhotonNetwork.Instantiate("FX/" + fxName, fxPosition, fxRotation, 0));
				gameObject3.transform.localScale = base.transform.localScale;
				float b = 1f - Vector3.Distance(currentCamera.transform.position, gameObject3.transform.position) * 0.05f;
				b = Mathf.Min(1f, b);
				currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().startShake(b, b);
			}
			if (attackCheckTimeA != 0f && ((base.animation["attack_" + attackAnimation].normalizedTime >= attackCheckTimeA && base.animation["attack_" + attackAnimation].normalizedTime <= attackCheckTimeB) || (!attackChkOnce && base.animation["attack_" + attackAnimation].normalizedTime >= attackCheckTimeA)))
			{
				if (!attackChkOnce)
				{
					attackChkOnce = true;
					playSound("snd_eren_swing" + UnityEngine.Random.Range(1, 3));
				}
				RaycastHit[] array2 = checkHitCapsule(checkHitCapsuleStart.position, checkHitCapsuleEnd.position, checkHitCapsuleR);
				foreach (RaycastHit raycastHit in array2)
				{
					GameObject gameObject4 = raycastHit.collider.gameObject;
					if (gameObject4.tag == "Player")
					{
						killPlayer(gameObject4);
					}
					if (!(gameObject4.tag == "erenHitbox"))
					{
						continue;
					}
					if (attackAnimation == "combo_1")
					{
						if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && PhotonNetwork.isMasterClient)
						{
							gameObject4.transform.root.gameObject.GetComponent<TITAN_EREN>().hitByFTByServer(1);
						}
					}
					else if (attackAnimation == "combo_2")
					{
						if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && PhotonNetwork.isMasterClient)
						{
							gameObject4.transform.root.gameObject.GetComponent<TITAN_EREN>().hitByFTByServer(2);
						}
					}
					else if (attackAnimation == "combo_3" && IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && PhotonNetwork.isMasterClient)
					{
						gameObject4.transform.root.gameObject.GetComponent<TITAN_EREN>().hitByFTByServer(3);
					}
				}
				RaycastHit[] array3 = checkHitCapsule(checkHitCapsuleEndOld, checkHitCapsuleEnd.position, checkHitCapsuleR);
				foreach (RaycastHit raycastHit2 in array3)
				{
					GameObject gameObject5 = raycastHit2.collider.gameObject;
					if (gameObject5.tag == "Player")
					{
						killPlayer(gameObject5);
					}
				}
				checkHitCapsuleEndOld = checkHitCapsuleEnd.position;
			}
			if (attackAnimation == "jumpCombo_1" && base.animation["attack_" + attackAnimation].normalizedTime >= 0.65f && !startJump && myHero != null)
			{
				startJump = true;
				float y = myHero.rigidbody.velocity.y;
				float num3 = -20f;
				float num4 = gravity;
				float y2 = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck").position.y;
				float num5 = (num3 - num4) * 0.5f;
				float num6 = y;
				float num7 = myHero.transform.position.y - y2;
				float num8 = Mathf.Abs((Mathf.Sqrt(num6 * num6 - 4f * num5 * num7) - num6) / (2f * num5));
				Vector3 vector2 = myHero.transform.position + myHero.rigidbody.velocity * num8 + Vector3.up * 0.5f * num3 * num8 * num8;
				float y3 = vector2.y;
				if (num7 < 0f || y3 - y2 < 0f)
				{
					idle();
					num8 = 0.5f;
					vector2 = base.transform.position + (y2 + 5f) * Vector3.up;
					y3 = vector2.y;
				}
				float num9 = y3 - y2;
				float num10 = Mathf.Sqrt(2f * num9 / gravity);
				float value = gravity * num10 + 20f;
				value = Mathf.Clamp(value, 20f, 90f);
				Vector3 vector3 = (vector2 - base.transform.position) / num8;
				abnorma_jump_bite_horizon_v = new Vector3(vector3.x, 0f, vector3.z);
				Vector3 velocity = base.rigidbody.velocity;
				Vector3 vector4 = new Vector3(abnorma_jump_bite_horizon_v.x, value, abnorma_jump_bite_horizon_v.z);
				if (vector4.magnitude > 90f)
				{
					vector4 = vector4.normalized * 90f;
				}
				Vector3 force = vector4 - velocity;
				base.rigidbody.AddForce(force, ForceMode.VelocityChange);
				float num11 = Vector2.Angle(new Vector2(base.transform.position.x, base.transform.position.z), new Vector2(myHero.transform.position.x, myHero.transform.position.z));
				num11 = Mathf.Atan2(myHero.transform.position.x - base.transform.position.x, myHero.transform.position.z - base.transform.position.z) * 57.29578f;
				base.gameObject.transform.rotation = Quaternion.Euler(0f, num11, 0f);
			}
			if (attackAnimation == "jumpCombo_3")
			{
				if (base.animation["attack_" + attackAnimation].normalizedTime >= 1f && IsGrounded())
				{
					attack("jumpCombo_4");
				}
			}
			else
			{
				if (!(base.animation["attack_" + attackAnimation].normalizedTime >= 1f))
				{
					return;
				}
				if (nextAttackAnimation != null)
				{
					attack(nextAttackAnimation);
					if (eren != null)
					{
						base.gameObject.transform.rotation = Quaternion.Euler(0f, Quaternion.LookRotation(eren.transform.position - base.transform.position).eulerAngles.y, 0f);
					}
				}
				else
				{
					findNearestHero();
					idle();
				}
			}
		}
		else if (state == "grab")
		{
			if (base.animation["attack_grab_" + attackAnimation].normalizedTime >= attackCheckTimeA && base.animation["attack_grab_" + attackAnimation].normalizedTime <= attackCheckTimeB && grabbedTarget == null)
			{
				GameObject gameObject6 = checkIfHitHand(currentGrabHand);
				if (gameObject6 != null)
				{
					if (isGrabHandLeft)
					{
						eatSetL(gameObject6);
						grabbedTarget = gameObject6;
					}
					else
					{
						eatSet(gameObject6);
						grabbedTarget = gameObject6;
					}
				}
			}
			if (base.animation["attack_grab_" + attackAnimation].normalizedTime > attackCheckTime && grabbedTarget != null)
			{
				justEatHero(grabbedTarget, currentGrabHand);
				grabbedTarget = null;
			}
			if (base.animation["attack_grab_" + attackAnimation].normalizedTime >= 1f)
			{
				idle();
			}
		}
		else if (state == "turn")
		{
			base.gameObject.transform.rotation = Quaternion.Lerp(base.gameObject.transform.rotation, Quaternion.Euler(0f, desDeg, 0f), Time.deltaTime * Mathf.Abs(turnDeg) * 0.1f);
			if (base.animation[turnAnimation].normalizedTime >= 1f)
			{
				idle();
			}
		}
		else if (state == "chase")
		{
			if ((eren == null || myDistance >= 35f || !attackTarget(myHero)) && (getNearestHeroDistance() >= 50f || UnityEngine.Random.Range(0, 100) >= 20 || !attackTarget(getNearestHero())) && myDistance < attackDistance - 15f)
			{
				idle(UnityEngine.Random.Range(0.05f, 0.2f));
			}
		}
		else if (state == "turn180")
		{
			if (base.animation[turnAnimation].normalizedTime >= 1f)
			{
				base.gameObject.transform.rotation = Quaternion.Euler(base.gameObject.transform.rotation.eulerAngles.x, base.gameObject.transform.rotation.eulerAngles.y + 180f, base.gameObject.transform.rotation.eulerAngles.z);
				idle();
				playAnimation("idle");
			}
		}
		else if (state == "anklehurt")
		{
			if (base.animation["legHurt"].normalizedTime >= 1f)
			{
				crossFade("legHurt_loop", 0.2f);
			}
			if (base.animation["legHurt_loop"].normalizedTime >= 3f)
			{
				crossFade("legHurt_getup", 0.2f);
			}
			if (base.animation["legHurt_getup"].normalizedTime >= 1f)
			{
				idle();
				playAnimation("idle");
			}
		}
	}

	public void updateLabel()
	{
		if (healthLabel != null && healthLabel.GetComponent<UILabel>().isVisible)
		{
			healthLabel.transform.LookAt(2f * healthLabel.transform.position - Camera.main.transform.position);
		}
	}
}
