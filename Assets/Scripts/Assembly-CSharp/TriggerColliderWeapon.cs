using System.Collections;
using GameProgress;
using Settings;
using UnityEngine;

public class TriggerColliderWeapon : MonoBehaviour
{
	public bool active_me;

	public GameObject currentCamera;

	public ArrayList currentHits = new ArrayList();

	public ArrayList currentHitsII = new ArrayList();

	public AudioSource meatDie;

	public int myTeam = 1;

	public float scoreMulti = 1f;

	private bool checkIfBehind(GameObject titan)
	{
		Transform transform = titan.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head");
		Vector3 to = base.transform.position - transform.transform.position;
		return Vector3.Angle(-transform.transform.forward, to) < 70f;
	}

	public void clearHits()
	{
		currentHitsII = new ArrayList();
		currentHits = new ArrayList();
	}

	private void napeMeat(Vector3 vkill, Transform titan)
	{
		Transform transform = titan.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck");
		GameObject gameObject = (GameObject)Object.Instantiate(Resources.Load("titanNapeMeat"), transform.position, transform.rotation);
		gameObject.transform.localScale = titan.localScale;
		gameObject.rigidbody.AddForce(vkill.normalized * 15f, ForceMode.Impulse);
		gameObject.rigidbody.AddForce(-titan.forward * 10f, ForceMode.Impulse);
		gameObject.rigidbody.AddTorque(new Vector3(Random.Range(-100, 100), Random.Range(-100, 100), Random.Range(-100, 100)), ForceMode.Impulse);
	}

	private void OnTriggerStay(Collider other)
	{
		if (!active_me)
		{
			return;
		}
		HERO component = base.transform.root.GetComponent<HERO>();
		if (!currentHitsII.Contains(other.gameObject))
		{
			currentHitsII.Add(other.gameObject);
			currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().startShake(0.1f, 0.1f);
			if (other.transform.root.gameObject.tag == "titan" && other.gameObject.layer != 16)
			{
				component.slashHit.Play();
				GameObject gameObject = ((IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE) ? ((GameObject)Object.Instantiate(Resources.Load("hitMeat"))) : PhotonNetwork.Instantiate("hitMeat", base.transform.position, Quaternion.Euler(270f, 0f, 0f), 0));
				gameObject.transform.position = base.transform.position;
				component.useBlade();
			}
		}
		if (other.gameObject.tag == "playerHitbox")
		{
			if (!LevelInfo.getInfo(FengGameManagerMKII.level).pvp)
			{
				return;
			}
			float b = 1f - Vector3.Distance(other.gameObject.transform.position, base.transform.position) * 0.05f;
			b = Mathf.Min(1f, b);
			HitBox component2 = other.gameObject.GetComponent<HitBox>();
			if (!(component2 != null) || !(component2.transform.root != null) || component2.transform.root.GetComponent<HERO>().myTeam == myTeam || component2.transform.root.GetComponent<HERO>().isInvincible())
			{
				return;
			}
			HERO component3 = component2.transform.root.GetComponent<HERO>();
			if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
			{
				if (!component3.isGrabbed)
				{
					component3.die((component3.transform.position - base.transform.position).normalized * b * 1000f + Vector3.up * 50f, false);
					GameProgressManager.RegisterHumanKill(component.gameObject, component3, KillWeapon.Blade);
				}
			}
			else if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && !component3.HasDied() && !component3.isGrabbed)
			{
				component3.markDie();
				object[] parameters = new object[5]
				{
					(component2.transform.root.position - base.transform.position).normalized * b * 1000f + Vector3.up * 50f,
					false,
					base.transform.root.gameObject.GetPhotonView().viewID,
					PhotonView.Find(base.transform.root.gameObject.GetPhotonView().viewID).owner.customProperties[PhotonPlayerProperty.name],
					false
				};
				component3.photonView.RPC("netDie", PhotonTargets.All, parameters);
				GameProgressManager.RegisterHumanKill(component.gameObject, component3, KillWeapon.Blade);
			}
		}
		else if (other.gameObject.tag == "titanneck")
		{
			HitBox component4 = other.gameObject.GetComponent<HitBox>();
			if (!(component4 != null) || !checkIfBehind(component4.transform.root.gameObject) || currentHits.Contains(component4))
			{
				return;
			}
			component4.hitPosition = (base.transform.position + component4.transform.position) * 0.5f;
			currentHits.Add(component4);
			meatDie.Play();
			TITAN component5 = component4.transform.root.GetComponent<TITAN>();
			if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
			{
				if (component5 != null && !component5.hasDie)
				{
					int b2 = (int)((component.rigidbody.velocity - component4.transform.root.rigidbody.velocity).magnitude * 10f * scoreMulti);
					b2 = Mathf.Max(10, b2);
					if (SettingsManager.GeneralSettings.SnapshotsEnabled.Value)
					{
						GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().startSnapShot2(component4.transform.position, b2, component4.transform.root.gameObject, 0.02f);
					}
					component5.die();
					napeMeat(currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().main_object.rigidbody.velocity, component4.transform.root);
					GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().netShowDamage(b2);
					GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().ReportKillToChatFeed("You", "Titan", b2);
					GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().playerKillInfoSingleUpdate(b2);
					GameProgressManager.RegisterDamage(component.gameObject, component5.gameObject, KillWeapon.Blade, b2);
					GameProgressManager.RegisterTitanKill(component.gameObject, component5, KillWeapon.Blade);
				}
			}
			else if (!PhotonNetwork.isMasterClient)
			{
				if (component5 != null)
				{
					if (!component5.hasDie)
					{
						int b3 = (int)((component.rigidbody.velocity - component4.transform.root.rigidbody.velocity).magnitude * 10f * scoreMulti);
						b3 = Mathf.Max(10, b3);
						if (SettingsManager.GeneralSettings.SnapshotsEnabled.Value)
						{
							GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().startSnapShot2(component4.transform.position, b3, component4.transform.root.gameObject, 0.02f);
							component4.transform.root.GetComponent<TITAN>().asClientLookTarget = false;
						}
						object[] parameters2 = new object[2]
						{
							component.gameObject.GetPhotonView().viewID,
							b3
						};
						component4.transform.root.GetComponent<TITAN>().photonView.RPC("titanGetHit", component5.photonView.owner, parameters2);
						GameProgressManager.RegisterDamage(component.gameObject, component5.gameObject, KillWeapon.Blade, b3);
						if (component5.WillDIe(b3))
						{
							GameProgressManager.RegisterTitanKill(component.gameObject, component5, KillWeapon.Blade);
						}
					}
				}
				else if (component4.transform.root.GetComponent<FEMALE_TITAN>() != null)
				{
					base.transform.root.GetComponent<HERO>().useBlade(int.MaxValue);
					int b4 = (int)((currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().main_object.rigidbody.velocity - component4.transform.root.rigidbody.velocity).magnitude * 10f * scoreMulti);
					b4 = Mathf.Max(10, b4);
					if (!component4.transform.root.GetComponent<FEMALE_TITAN>().hasDie)
					{
						object[] parameters3 = new object[2]
						{
							base.transform.root.gameObject.GetPhotonView().viewID,
							b4
						};
						component4.transform.root.GetComponent<FEMALE_TITAN>().photonView.RPC("titanGetHit", component4.transform.root.GetComponent<FEMALE_TITAN>().photonView.owner, parameters3);
					}
				}
				else if (component4.transform.root.GetComponent<COLOSSAL_TITAN>() != null)
				{
					base.transform.root.GetComponent<HERO>().useBlade(int.MaxValue);
					if (!component4.transform.root.GetComponent<COLOSSAL_TITAN>().hasDie)
					{
						int b5 = (int)((currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().main_object.rigidbody.velocity - component4.transform.root.rigidbody.velocity).magnitude * 10f * scoreMulti);
						b5 = Mathf.Max(10, b5);
						object[] parameters4 = new object[2]
						{
							base.transform.root.gameObject.GetPhotonView().viewID,
							b5
						};
						component4.transform.root.GetComponent<COLOSSAL_TITAN>().photonView.RPC("titanGetHit", component4.transform.root.GetComponent<COLOSSAL_TITAN>().photonView.owner, parameters4);
					}
				}
			}
			else if (component5 != null)
			{
				if (!component5.hasDie)
				{
					int b6 = (int)((component.rigidbody.velocity - component4.transform.root.rigidbody.velocity).magnitude * 10f * scoreMulti);
					b6 = Mathf.Max(10, b6);
					if (SettingsManager.GeneralSettings.SnapshotsEnabled.Value)
					{
						GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().startSnapShot2(component4.transform.position, b6, component4.transform.root.gameObject, 0.02f);
					}
					component5.titanGetHit(component.gameObject.GetPhotonView().viewID, b6);
					GameProgressManager.RegisterDamage(component.gameObject, component5.gameObject, KillWeapon.Blade, b6);
					if (component5.WillDIe(b6))
					{
						GameProgressManager.RegisterTitanKill(component.gameObject, component5, KillWeapon.Blade);
					}
				}
			}
			else if (component4.transform.root.GetComponent<FEMALE_TITAN>() != null)
			{
				base.transform.root.GetComponent<HERO>().useBlade(int.MaxValue);
				if (!component4.transform.root.GetComponent<FEMALE_TITAN>().hasDie)
				{
					int b7 = (int)((currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().main_object.rigidbody.velocity - component4.transform.root.rigidbody.velocity).magnitude * 10f * scoreMulti);
					b7 = Mathf.Max(10, b7);
					if (SettingsManager.GeneralSettings.SnapshotsEnabled.Value)
					{
						GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().startSnapShot2(component4.transform.position, b7, null, 0.02f);
					}
					component4.transform.root.GetComponent<FEMALE_TITAN>().titanGetHit(base.transform.root.gameObject.GetPhotonView().viewID, b7);
				}
			}
			else if (component4.transform.root.GetComponent<COLOSSAL_TITAN>() != null)
			{
				base.transform.root.GetComponent<HERO>().useBlade(int.MaxValue);
				if (!component4.transform.root.GetComponent<COLOSSAL_TITAN>().hasDie)
				{
					int b8 = (int)((currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().main_object.rigidbody.velocity - component4.transform.root.rigidbody.velocity).magnitude * 10f * scoreMulti);
					b8 = Mathf.Max(10, b8);
					if (SettingsManager.GeneralSettings.SnapshotsEnabled.Value)
					{
						GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().startSnapShot2(component4.transform.position, b8, null, 0.02f);
					}
					component4.transform.root.GetComponent<COLOSSAL_TITAN>().titanGetHit(base.transform.root.gameObject.GetPhotonView().viewID, b8);
				}
			}
			showCriticalHitFX();
		}
		else if (other.gameObject.tag == "titaneye")
		{
			if (currentHits.Contains(other.gameObject))
			{
				return;
			}
			currentHits.Add(other.gameObject);
			GameObject gameObject2 = other.gameObject.transform.root.gameObject;
			if (gameObject2.GetComponent<FEMALE_TITAN>() != null)
			{
				if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
				{
					if (!gameObject2.GetComponent<FEMALE_TITAN>().hasDie)
					{
						gameObject2.GetComponent<FEMALE_TITAN>().hitEye();
					}
				}
				else if (!PhotonNetwork.isMasterClient)
				{
					if (!gameObject2.GetComponent<FEMALE_TITAN>().hasDie)
					{
						object[] parameters5 = new object[1] { base.transform.root.gameObject.GetPhotonView().viewID };
						gameObject2.GetComponent<FEMALE_TITAN>().photonView.RPC("hitEyeRPC", PhotonTargets.MasterClient, parameters5);
					}
				}
				else if (!gameObject2.GetComponent<FEMALE_TITAN>().hasDie)
				{
					gameObject2.GetComponent<FEMALE_TITAN>().hitEyeRPC(base.transform.root.gameObject.GetPhotonView().viewID);
				}
			}
			else
			{
				if (gameObject2.GetComponent<TITAN>().abnormalType == AbnormalType.TYPE_CRAWLER)
				{
					return;
				}
				if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
				{
					if (!gameObject2.GetComponent<TITAN>().hasDie)
					{
						gameObject2.GetComponent<TITAN>().hitEye();
					}
				}
				else if (!PhotonNetwork.isMasterClient)
				{
					if (!gameObject2.GetComponent<TITAN>().hasDie)
					{
						object[] parameters6 = new object[1] { base.transform.root.gameObject.GetPhotonView().viewID };
						gameObject2.GetComponent<TITAN>().photonView.RPC("hitEyeRPC", PhotonTargets.MasterClient, parameters6);
					}
				}
				else if (!gameObject2.GetComponent<TITAN>().hasDie)
				{
					gameObject2.GetComponent<TITAN>().hitEyeRPC(base.transform.root.gameObject.GetPhotonView().viewID);
				}
				showCriticalHitFX();
			}
		}
		else
		{
			if (!(other.gameObject.tag == "titanankle") || currentHits.Contains(other.gameObject))
			{
				return;
			}
			currentHits.Add(other.gameObject);
			GameObject gameObject3 = other.gameObject.transform.root.gameObject;
			int b9 = (int)((currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().main_object.rigidbody.velocity - gameObject3.rigidbody.velocity).magnitude * 10f * scoreMulti);
			b9 = Mathf.Max(10, b9);
			if (gameObject3.GetComponent<TITAN>() != null && gameObject3.GetComponent<TITAN>().abnormalType != AbnormalType.TYPE_CRAWLER)
			{
				if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
				{
					if (!gameObject3.GetComponent<TITAN>().hasDie)
					{
						gameObject3.GetComponent<TITAN>().hitAnkle();
					}
					return;
				}
				if (!PhotonNetwork.isMasterClient)
				{
					if (!gameObject3.GetComponent<TITAN>().hasDie)
					{
						object[] parameters7 = new object[1] { base.transform.root.gameObject.GetPhotonView().viewID };
						gameObject3.GetComponent<TITAN>().photonView.RPC("hitAnkleRPC", PhotonTargets.MasterClient, parameters7);
					}
				}
				else if (!gameObject3.GetComponent<TITAN>().hasDie)
				{
					gameObject3.GetComponent<TITAN>().hitAnkle();
				}
				showCriticalHitFX();
			}
			else
			{
				if (!(gameObject3.GetComponent<FEMALE_TITAN>() != null))
				{
					return;
				}
				if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
				{
					if (other.gameObject.name == "ankleR")
					{
						if (gameObject3.GetComponent<FEMALE_TITAN>() != null && !gameObject3.GetComponent<FEMALE_TITAN>().hasDie)
						{
							gameObject3.GetComponent<FEMALE_TITAN>().hitAnkleR(b9);
						}
					}
					else if (gameObject3.GetComponent<FEMALE_TITAN>() != null && !gameObject3.GetComponent<FEMALE_TITAN>().hasDie)
					{
						gameObject3.GetComponent<FEMALE_TITAN>().hitAnkleL(b9);
					}
				}
				else if (other.gameObject.name == "ankleR")
				{
					if (!PhotonNetwork.isMasterClient)
					{
						if (!gameObject3.GetComponent<FEMALE_TITAN>().hasDie)
						{
							object[] parameters8 = new object[2]
							{
								base.transform.root.gameObject.GetPhotonView().viewID,
								b9
							};
							gameObject3.GetComponent<FEMALE_TITAN>().photonView.RPC("hitAnkleRRPC", PhotonTargets.MasterClient, parameters8);
						}
					}
					else if (!gameObject3.GetComponent<FEMALE_TITAN>().hasDie)
					{
						gameObject3.GetComponent<FEMALE_TITAN>().hitAnkleRRPC(base.transform.root.gameObject.GetPhotonView().viewID, b9);
					}
				}
				else if (!PhotonNetwork.isMasterClient)
				{
					if (!gameObject3.GetComponent<FEMALE_TITAN>().hasDie)
					{
						object[] parameters9 = new object[2]
						{
							base.transform.root.gameObject.GetPhotonView().viewID,
							b9
						};
						gameObject3.GetComponent<FEMALE_TITAN>().photonView.RPC("hitAnkleLRPC", PhotonTargets.MasterClient, parameters9);
					}
				}
				else if (!gameObject3.GetComponent<FEMALE_TITAN>().hasDie)
				{
					gameObject3.GetComponent<FEMALE_TITAN>().hitAnkleLRPC(base.transform.root.gameObject.GetPhotonView().viewID, b9);
				}
				showCriticalHitFX();
			}
		}
	}

	private void showCriticalHitFX()
	{
		currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().startShake(0.2f, 0.3f);
		GameObject gameObject = ((IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE) ? ((GameObject)Object.Instantiate(Resources.Load("redCross"))) : PhotonNetwork.Instantiate("redCross", base.transform.position, Quaternion.Euler(270f, 0f, 0f), 0));
		gameObject.transform.position = base.transform.position;
	}

	private void Start()
	{
		currentCamera = GameObject.Find("MainCamera");
	}
}
