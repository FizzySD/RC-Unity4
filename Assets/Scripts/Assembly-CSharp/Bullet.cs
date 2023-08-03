using System.Collections;
using CustomSkins;
using Photon;
using UnityEngine;

internal class Bullet : Photon.MonoBehaviour
{
	private Vector3 heightOffSet = Vector3.up * 0.48f;

	private bool isdestroying;

	private float killTime;

	private float killTime2;

	private Vector3 launchOffSet = Vector3.zero;

	private bool left = true;

	public bool leviMode;

	public float leviShootTime;

	private LineRenderer lineRenderer;

	private GameObject master;

	private GameObject myRef;

	public TITAN myTitan;

	private ArrayList nodes = new ArrayList();

	private int phase;

	private GameObject rope;

	private int spiralcount;

	private ArrayList spiralNodes;

	private Vector3 velocity = Vector3.zero;

	private Vector3 velocity2 = Vector3.zero;

	private bool _hasSkin;

	private float _lastLength;

	private float _skinTiling;

	private bool _isTransparent;

	private void SetSkin()
	{
		HumanCustomSkinLoader customSkinLoader = master.GetComponent<HERO>()._customSkinLoader;
		HookCustomSkinPart hookCustomSkinPart = (left ? customSkinLoader.HookL : customSkinLoader.HookR);
		if (hookCustomSkinPart != null)
		{
			if (hookCustomSkinPart.HookMaterial != null)
			{
				_hasSkin = true;
				lineRenderer.material = hookCustomSkinPart.HookMaterial;
				_skinTiling = (left ? customSkinLoader.HookLTiling : customSkinLoader.HookRTiling);
			}
			if (hookCustomSkinPart.Transparent)
			{
				_hasSkin = true;
				_isTransparent = true;
				lineRenderer.enabled = false;
			}
		}
	}

	public LineRenderer GetHook()
	{
		return lineRenderer;
	}

	private void UpdateSkin()
	{
		if (_hasSkin && !_isTransparent)
		{
			float magnitude = (base.transform.position - myRef.transform.position).magnitude;
			if (magnitude != _lastLength)
			{
				_lastLength = magnitude;
				lineRenderer.material.mainTextureScale = new Vector2(_skinTiling * magnitude, 1f);
			}
		}
	}

	public void checkTitan()
	{
		GameObject main_object = Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().main_object;
		if (!(main_object != null) || !(master != null) || !(master == main_object))
		{
			return;
		}
		LayerMask layerMask = 1 << LayerMask.NameToLayer("PlayerAttackBox");
		RaycastHit hitInfo;
		if (!Physics.Raycast(base.transform.position, velocity, out hitInfo, 10f, layerMask.value))
		{
			return;
		}
		Collider collider = hitInfo.collider;
		if (!collider.name.Contains("PlayerDetectorRC"))
		{
			return;
		}
		TITAN component = collider.transform.root.gameObject.GetComponent<TITAN>();
		if (component != null)
		{
			if (myTitan == null)
			{
				myTitan = component;
				myTitan.isHooked = true;
			}
			else if (myTitan != component)
			{
				myTitan.isHooked = false;
				myTitan = component;
				myTitan.isHooked = true;
			}
		}
	}

	public void disable()
	{
		phase = 2;
		killTime = 0f;
		if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER)
		{
			object[] parameters = new object[1] { 2 };
			base.photonView.RPC("setPhase", PhotonTargets.Others, parameters);
		}
	}

	private void FixedUpdate()
	{
		if ((phase == 2 || phase == 1) && leviMode)
		{
			spiralcount++;
			if (spiralcount >= 60)
			{
				isdestroying = true;
				removeMe();
				return;
			}
		}
		if (IN_GAME_MAIN_CAMERA.gametype != 0 && !base.photonView.isMine)
		{
			if (phase == 0)
			{
				base.gameObject.transform.position += velocity * Time.deltaTime * 50f + velocity2 * Time.deltaTime;
				nodes.Add(new Vector3(base.gameObject.transform.position.x, base.gameObject.transform.position.y, base.gameObject.transform.position.z));
			}
		}
		else
		{
			if (phase != 0)
			{
				return;
			}
			checkTitan();
			base.gameObject.transform.position += velocity * Time.deltaTime * 50f + velocity2 * Time.deltaTime;
			LayerMask layerMask = 1 << LayerMask.NameToLayer("EnemyBox");
			LayerMask layerMask2 = 1 << LayerMask.NameToLayer("Ground");
			LayerMask layerMask3 = 1 << LayerMask.NameToLayer("NetworkObject");
			LayerMask layerMask4 = (int)layerMask | (int)layerMask2 | (int)layerMask3;
			bool flag = false;
			bool flag2 = false;
			RaycastHit hitInfo;
			if ((nodes.Count <= 1) ? Physics.Linecast((Vector3)nodes[nodes.Count - 1], base.gameObject.transform.position, out hitInfo, layerMask4.value) : Physics.Linecast((Vector3)nodes[nodes.Count - 2], base.gameObject.transform.position, out hitInfo, layerMask4.value))
			{
				bool flag3 = true;
				if (hitInfo.collider.transform.gameObject.layer == LayerMask.NameToLayer("EnemyBox"))
				{
					if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER)
					{
						object[] parameters = new object[1] { hitInfo.collider.transform.root.gameObject.GetPhotonView().viewID };
						base.photonView.RPC("tieMeToOBJ", PhotonTargets.Others, parameters);
					}
					master.GetComponent<HERO>().lastHook = hitInfo.collider.transform.root;
					base.transform.parent = hitInfo.collider.transform;
				}
				else if (hitInfo.collider.transform.gameObject.layer == LayerMask.NameToLayer("Ground"))
				{
					master.GetComponent<HERO>().lastHook = null;
				}
				else if (hitInfo.collider.transform.gameObject.layer == LayerMask.NameToLayer("NetworkObject") && hitInfo.collider.transform.gameObject.tag == "Player" && !leviMode)
				{
					if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER)
					{
						object[] parameters2 = new object[1] { hitInfo.collider.transform.root.gameObject.GetPhotonView().viewID };
						base.photonView.RPC("tieMeToOBJ", PhotonTargets.Others, parameters2);
					}
					master.GetComponent<HERO>().hookToHuman(hitInfo.collider.transform.root.gameObject, base.transform.position);
					base.transform.parent = hitInfo.collider.transform;
					master.GetComponent<HERO>().lastHook = null;
				}
				else
				{
					flag3 = false;
				}
				if (phase == 2)
				{
					flag3 = false;
				}
				if (flag3)
				{
					master.GetComponent<HERO>().launch(hitInfo.point, left, leviMode);
					base.transform.position = hitInfo.point;
					if (phase != 2)
					{
						phase = 1;
						if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER)
						{
							object[] parameters3 = new object[1] { 1 };
							base.photonView.RPC("setPhase", PhotonTargets.Others, parameters3);
							object[] parameters4 = new object[1] { base.transform.position };
							base.photonView.RPC("tieMeTo", PhotonTargets.Others, parameters4);
						}
						if (leviMode)
						{
							getSpiral(master.transform.position, master.transform.rotation.eulerAngles);
						}
						flag = true;
					}
				}
			}
			nodes.Add(new Vector3(base.gameObject.transform.position.x, base.gameObject.transform.position.y, base.gameObject.transform.position.z));
			if (flag)
			{
				return;
			}
			killTime2 += Time.deltaTime;
			if (killTime2 > 0.8f)
			{
				phase = 4;
				if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER)
				{
					object[] parameters5 = new object[1] { 4 };
					base.photonView.RPC("setPhase", PhotonTargets.Others, parameters5);
				}
			}
		}
	}

	private void getSpiral(Vector3 masterposition, Vector3 masterrotation)
	{
		float num = 1.2f;
		float num2 = 30f;
		float num3 = 2f;
		float num4 = 0.5f;
		num = 30f;
		num3 = 0.05f + (float)spiralcount * 0.03f;
		num = ((spiralcount >= 5) ? (1.2f + (float)(60 - spiralcount) * 0.1f) : Vector2.Distance(new Vector2(masterposition.x, masterposition.z), new Vector2(base.gameObject.transform.position.x, base.gameObject.transform.position.z)));
		num4 -= (float)spiralcount * 0.06f;
		float num5 = num / num2;
		float num6 = num3 / num2;
		float num7 = num6 * 2f * 3.141593f;
		num4 *= 6.283185f;
		spiralNodes = new ArrayList();
		for (int i = 1; (float)i <= num2; i++)
		{
			float num8 = (float)i * num5 * (1f + 0.05f * (float)i);
			float f = (float)i * num7 + num4 + 1.256637f + masterrotation.y * 0.0173f;
			float x = Mathf.Cos(f) * num8;
			float z = (0f - Mathf.Sin(f)) * num8;
			spiralNodes.Add(new Vector3(x, 0f, z));
		}
	}

	public bool isHooked()
	{
		return phase == 1;
	}

	private void killObject()
	{
		Object.Destroy(rope);
		Object.Destroy(base.gameObject);
	}

	public void launch(Vector3 v, Vector3 v2, string launcher_ref, bool isLeft, GameObject hero, bool leviMode = false)
	{
		if (phase != 2)
		{
			master = hero;
			velocity = v;
			float f = Mathf.Acos(Vector3.Dot(v.normalized, v2.normalized)) * 57.29578f;
			if (Mathf.Abs(f) > 90f)
			{
				velocity2 = Vector3.zero;
			}
			else
			{
				velocity2 = Vector3.Project(v2, v);
			}
			if (launcher_ref == "hookRefL1")
			{
				myRef = hero.GetComponent<HERO>().hookRefL1;
			}
			if (launcher_ref == "hookRefL2")
			{
				myRef = hero.GetComponent<HERO>().hookRefL2;
			}
			if (launcher_ref == "hookRefR1")
			{
				myRef = hero.GetComponent<HERO>().hookRefR1;
			}
			if (launcher_ref == "hookRefR2")
			{
				myRef = hero.GetComponent<HERO>().hookRefR2;
			}
			nodes = new ArrayList();
			nodes.Add(myRef.transform.position);
			phase = 0;
			this.leviMode = leviMode;
			left = isLeft;
			if (IN_GAME_MAIN_CAMERA.gametype != 0 && base.photonView.isMine)
			{
				object[] parameters = new object[2]
				{
					hero.GetComponent<HERO>().photonView.viewID,
					launcher_ref
				};
				base.photonView.RPC("myMasterIs", PhotonTargets.Others, parameters);
				object[] parameters2 = new object[3] { v, velocity2, left };
				base.photonView.RPC("setVelocityAndLeft", PhotonTargets.Others, parameters2);
			}
			base.transform.position = myRef.transform.position;
			base.transform.rotation = Quaternion.LookRotation(v.normalized);
			SetSkin();
		}
	}

	[RPC]
	private void myMasterIs(int id, string launcherRef, PhotonMessageInfo info)
	{
		if (info.sender != base.photonView.owner || id < 0)
		{
			FengGameManagerMKII.instance.kickPlayerRCIfMC(info.sender, true, "bullet myMasterIs");
			return;
		}
		PhotonView photonView = PhotonView.Find(id);
		if (photonView.owner != info.sender)
		{
			FengGameManagerMKII.instance.kickPlayerRCIfMC(info.sender, true, "bullet myMasterIs");
		}
		else if (!(photonView == null) && !(photonView.gameObject == null))
		{
			master = PhotonView.Find(id).gameObject;
			if (launcherRef == "hookRefL1")
			{
				myRef = master.GetComponent<HERO>().hookRefL1;
			}
			if (launcherRef == "hookRefL2")
			{
				myRef = master.GetComponent<HERO>().hookRefL2;
			}
			if (launcherRef == "hookRefR1")
			{
				myRef = master.GetComponent<HERO>().hookRefR1;
			}
			if (launcherRef == "hookRefR2")
			{
				myRef = master.GetComponent<HERO>().hookRefR2;
			}
		}
	}

	private void netLaunch(Vector3 newPosition)
	{
		nodes = new ArrayList();
		nodes.Add(newPosition);
	}

	private void netUpdateLeviSpiral(Vector3 newPosition, Vector3 masterPosition, Vector3 masterrotation)
	{
		phase = 2;
		leviMode = true;
		getSpiral(masterPosition, masterrotation);
		Vector3 vector = masterPosition - (Vector3)spiralNodes[0];
		lineRenderer.SetVertexCount(spiralNodes.Count - (int)((float)spiralcount * 0.5f));
		for (int i = 0; (float)i <= (float)(spiralNodes.Count - 1) - (float)spiralcount * 0.5f; i++)
		{
			if (spiralcount < 5)
			{
				Vector3 vector2 = (Vector3)spiralNodes[i] + vector;
				float num = (float)(spiralNodes.Count - 1) - (float)spiralcount * 0.5f;
				vector2 = new Vector3(vector2.x, vector2.y * ((num - (float)i) / num) + newPosition.y * ((float)i / num), vector2.z);
				lineRenderer.SetPosition(i, vector2);
			}
			else
			{
				lineRenderer.SetPosition(i, (Vector3)spiralNodes[i] + vector);
			}
		}
	}

	private void netUpdatePhase1(Vector3 newPosition, Vector3 masterPosition)
	{
		lineRenderer.SetVertexCount(2);
		lineRenderer.SetPosition(0, newPosition);
		lineRenderer.SetPosition(1, masterPosition);
		base.transform.position = newPosition;
	}

	private void OnDestroy()
	{
		if (FengGameManagerMKII.instance != null)
		{
			FengGameManagerMKII.instance.removeHook(this);
		}
		if (myTitan != null)
		{
			myTitan.isHooked = false;
		}
		Object.Destroy(rope);
	}

	public void removeMe()
	{
		isdestroying = true;
		if (IN_GAME_MAIN_CAMERA.gametype != 0 && base.photonView.isMine)
		{
			PhotonNetwork.Destroy(base.photonView);
			PhotonNetwork.RemoveRPCs(base.photonView);
		}
		else if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
		{
			Object.Destroy(rope);
			Object.Destroy(base.gameObject);
		}
	}

	private void setLinePhase0()
	{
		if (master == null)
		{
			Object.Destroy(rope);
			Object.Destroy(base.gameObject);
		}
		else if (nodes.Count > 0)
		{
			Vector3 vector = myRef.transform.position - (Vector3)nodes[0];
			lineRenderer.SetVertexCount(nodes.Count);
			for (int i = 0; i <= nodes.Count - 1; i++)
			{
				lineRenderer.SetPosition(i, (Vector3)nodes[i] + vector * Mathf.Pow(0.75f, i));
			}
			if (nodes.Count > 1)
			{
				lineRenderer.SetPosition(1, myRef.transform.position);
			}
		}
	}

	[RPC]
	private void setPhase(int value, PhotonMessageInfo info)
	{
		if (info.sender != base.photonView.owner)
		{
			FengGameManagerMKII.instance.kickPlayerRCIfMC(info.sender, true, "bullet setphase");
		}
		else
		{
			phase = value;
		}
	}

	[RPC]
	private void setVelocityAndLeft(Vector3 value, Vector3 v2, bool l, PhotonMessageInfo info)
	{
		if (info.sender != base.photonView.owner)
		{
			FengGameManagerMKII.instance.kickPlayerRCIfMC(info.sender, true, "bullet setvelocity");
			return;
		}
		velocity = value;
		velocity2 = v2;
		left = l;
		base.transform.rotation = Quaternion.LookRotation(value.normalized);
		SetSkin();
	}

	private void Awake()
	{
		rope = (GameObject)Object.Instantiate(Resources.Load("rope"));
		lineRenderer = rope.GetComponent<LineRenderer>();
		GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().addHook(this);
	}

	[RPC]
	private void tieMeTo(Vector3 p, PhotonMessageInfo info)
	{
		if (info.sender != base.photonView.owner)
		{
			FengGameManagerMKII.instance.kickPlayerRCIfMC(info.sender, true, "bullet tieMeTo");
		}
		else
		{
			base.transform.position = p;
		}
	}

	[RPC]
	private void tieMeToOBJ(int id, PhotonMessageInfo info)
	{
		if (info.sender != base.photonView.owner)
		{
			FengGameManagerMKII.instance.kickPlayerRCIfMC(info.sender, true, "bullet TieMeToObj");
		}
		else
		{
			base.transform.parent = PhotonView.Find(id).gameObject.transform;
		}
	}

	public void update()
	{
		if (master == null)
		{
			removeMe();
		}
		else
		{
			if (isdestroying)
			{
				return;
			}
			if (leviMode)
			{
				leviShootTime += Time.deltaTime;
				if (leviShootTime > 0.4f)
				{
					phase = 2;
					base.gameObject.GetComponent<MeshRenderer>().enabled = false;
				}
			}
			if (phase == 0)
			{
				setLinePhase0();
			}
			else if (phase == 1)
			{
				Vector3 vector = base.transform.position - myRef.transform.position;
				Vector3 vector2 = base.transform.position + myRef.transform.position;
				Vector3 vector3 = master.rigidbody.velocity;
				float magnitude = vector3.magnitude;
				float magnitude2 = vector.magnitude;
				int value = (int)((magnitude2 + magnitude) / 5f);
				value = Mathf.Clamp(value, 2, 6);
				lineRenderer.SetVertexCount(value);
				lineRenderer.SetPosition(0, myRef.transform.position);
				int i = 1;
				float num = Mathf.Pow(magnitude2, 0.3f);
				for (; i < value; i++)
				{
					int num2 = value / 2;
					float num3 = Mathf.Abs(i - num2);
					float f = ((float)num2 - num3) / (float)num2;
					f = Mathf.Pow(f, 0.5f);
					float num4 = (num + magnitude) * 0.0015f * f;
					lineRenderer.SetPosition(i, new Vector3(Random.Range(0f - num4, num4), Random.Range(0f - num4, num4), Random.Range(0f - num4, num4)) + myRef.transform.position + vector * ((float)i / (float)value) - Vector3.up * num * 0.05f * f - vector3 * 0.001f * f * num);
				}
				lineRenderer.SetPosition(value - 1, base.transform.position);
			}
			else if (phase == 2)
			{
				if (!leviMode)
				{
					lineRenderer.SetVertexCount(2);
					lineRenderer.SetPosition(0, base.transform.position);
					lineRenderer.SetPosition(1, myRef.transform.position);
					killTime += Time.deltaTime * 0.2f;
					lineRenderer.SetWidth(0.1f - killTime, 0.1f - killTime);
					if (killTime > 0.1f)
					{
						removeMe();
					}
				}
				else
				{
					getSpiral(master.transform.position, master.transform.rotation.eulerAngles);
					Vector3 vector4 = myRef.transform.position - (Vector3)spiralNodes[0];
					lineRenderer.SetVertexCount(spiralNodes.Count - (int)((float)spiralcount * 0.5f));
					for (int j = 0; (float)j <= (float)(spiralNodes.Count - 1) - (float)spiralcount * 0.5f; j++)
					{
						if (spiralcount < 5)
						{
							Vector3 vector5 = (Vector3)spiralNodes[j] + vector4;
							float num5 = (float)(spiralNodes.Count - 1) - (float)spiralcount * 0.5f;
							vector5 = new Vector3(vector5.x, vector5.y * ((num5 - (float)j) / num5) + base.gameObject.transform.position.y * ((float)j / num5), vector5.z);
							lineRenderer.SetPosition(j, vector5);
						}
						else
						{
							lineRenderer.SetPosition(j, (Vector3)spiralNodes[j] + vector4);
						}
					}
				}
			}
			else if (phase == 4)
			{
				base.gameObject.transform.position += velocity + velocity2 * Time.deltaTime;
				nodes.Add(new Vector3(base.gameObject.transform.position.x, base.gameObject.transform.position.y, base.gameObject.transform.position.z));
				Vector3 vector6 = myRef.transform.position - (Vector3)nodes[0];
				for (int k = 0; k <= nodes.Count - 1; k++)
				{
					lineRenderer.SetVertexCount(nodes.Count);
					lineRenderer.SetPosition(k, (Vector3)nodes[k] + vector6 * Mathf.Pow(0.5f, k));
				}
				killTime2 += Time.deltaTime;
				if (killTime2 > 0.8f)
				{
					killTime += Time.deltaTime * 0.2f;
					lineRenderer.SetWidth(0.1f - killTime, 0.1f - killTime);
					if (killTime > 0.1f)
					{
						removeMe();
					}
				}
			}
			UpdateSkin();
		}
	}
}
