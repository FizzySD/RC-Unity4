using System.Collections;
using Photon;
using UnityEngine;

public class PVPcheckPoint : Photon.MonoBehaviour
{
	private bool annie;

	public GameObject[] chkPtNextArr;

	public GameObject[] chkPtPreviousArr;

	public static ArrayList chkPts;

	private float getPtsInterval = 20f;

	private float getPtsTimer;

	public bool hasAnnie;

	private float hitTestR = 15f;

	public GameObject humanCyc;

	public float humanPt;

	public float humanPtMax = 40f;

	public int id;

	public bool isBase;

	public int normalTitanRate = 70;

	private bool playerOn;

	public float size = 1f;

	private float spawnTitanTimer;

	public CheckPointState state;

	private GameObject supply;

	private float syncInterval = 0.6f;

	private float syncTimer;

	public GameObject titanCyc;

	public float titanInterval = 30f;

	private bool titanOn;

	public float titanPt;

	public float titanPtMax = 40f;

	private float _lastTitanPt;

	private float _lastHumanPt;

	public GameObject chkPtNext
	{
		get
		{
			if (chkPtNextArr.Length == 0)
			{
				return null;
			}
			return chkPtNextArr[Random.Range(0, chkPtNextArr.Length)];
		}
	}

	public GameObject chkPtPrevious
	{
		get
		{
			if (chkPtPreviousArr.Length == 0)
			{
				return null;
			}
			return chkPtPreviousArr[Random.Range(0, chkPtPreviousArr.Length)];
		}
	}

	[RPC]
	private void changeHumanPt(float pt)
	{
		humanPt = pt;
	}

	[RPC]
	private void changeState(int num)
	{
		if (num == 0)
		{
			state = CheckPointState.Non;
		}
		if (num == 1)
		{
			state = CheckPointState.Human;
		}
		if (num == 2)
		{
			state = CheckPointState.Titan;
		}
	}

	[RPC]
	private void changeTitanPt(float pt)
	{
		titanPt = pt;
	}

	private void checkIfBeingCapture()
	{
		playerOn = false;
		titanOn = false;
		GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
		GameObject[] array2 = GameObject.FindGameObjectsWithTag("titan");
		for (int i = 0; i < array.Length; i++)
		{
			if (!(Vector3.Distance(array[i].transform.position, base.transform.position) < hitTestR))
			{
				continue;
			}
			playerOn = true;
			if (state == CheckPointState.Human && array[i].GetPhotonView().isMine)
			{
				if (GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().checkpoint != base.gameObject)
				{
					GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().checkpoint = base.gameObject;
					GameObject.Find("Chatroom").GetComponent<InRoomChat>().addLINE("<color=#A8FF24>Respawn point changed to point" + id + "</color>");
				}
				break;
			}
		}
		for (int i = 0; i < array2.Length; i++)
		{
			if (!(Vector3.Distance(array2[i].transform.position, base.transform.position) < hitTestR + 5f) || (!(array2[i].GetComponent<TITAN>() == null) && array2[i].GetComponent<TITAN>().hasDie))
			{
				continue;
			}
			titanOn = true;
			if (state == CheckPointState.Titan && array2[i].GetPhotonView().isMine && array2[i].GetComponent<TITAN>() != null && array2[i].GetComponent<TITAN>().nonAI)
			{
				if (GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().checkpoint != base.gameObject)
				{
					GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().checkpoint = base.gameObject;
					GameObject.Find("Chatroom").GetComponent<InRoomChat>().addLINE("<color=#A8FF24>Respawn point changed to point" + id + "</color>");
				}
				break;
			}
		}
	}

	private bool checkIfHumanWins()
	{
		for (int i = 0; i < chkPts.Count; i++)
		{
			if ((chkPts[i] as PVPcheckPoint).state != CheckPointState.Human)
			{
				return false;
			}
		}
		return true;
	}

	private bool checkIfTitanWins()
	{
		for (int i = 0; i < chkPts.Count; i++)
		{
			if ((chkPts[i] as PVPcheckPoint).state != CheckPointState.Titan)
			{
				return false;
			}
		}
		return true;
	}

	private float getHeight(Vector3 pt)
	{
		LayerMask layerMask = 1 << LayerMask.NameToLayer("Ground");
		RaycastHit hitInfo;
		if (Physics.Raycast(pt, -Vector3.up, out hitInfo, 1000f, layerMask.value))
		{
			return hitInfo.point.y;
		}
		return 0f;
	}

	public string getStateString()
	{
		if (state == CheckPointState.Human)
		{
			return "[" + ColorSet.color_human + "]H[-]";
		}
		if (state == CheckPointState.Titan)
		{
			return "[" + ColorSet.color_titan_player + "]T[-]";
		}
		return "[" + ColorSet.color_D + "]_[-]";
	}

	private void humanGetsPoint()
	{
		if (humanPt >= humanPtMax)
		{
			humanPt = humanPtMax;
			titanPt = 0f;
			syncPts();
			state = CheckPointState.Human;
			object[] parameters = new object[1] { 1 };
			base.photonView.RPC("changeState", PhotonTargets.All, parameters);
			if (LevelInfo.getInfo(FengGameManagerMKII.level).mapName != "The City I")
			{
				supply = PhotonNetwork.Instantiate("aot_supply", base.transform.position - Vector3.up * (base.transform.position.y - getHeight(base.transform.position)), base.transform.rotation, 0);
			}
			FengGameManagerMKII component = GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>();
			component.PVPhumanScore += 2;
			GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().checkPVPpts();
			if (checkIfHumanWins())
			{
				GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().gameWin2();
			}
		}
		else
		{
			humanPt += Time.deltaTime;
		}
	}

	private void humanLosePoint()
	{
		if (!(humanPt > 0f))
		{
			return;
		}
		humanPt -= Time.deltaTime * 3f;
		if (humanPt <= 0f)
		{
			humanPt = 0f;
			syncPts();
			if (state != CheckPointState.Titan)
			{
				state = CheckPointState.Non;
				object[] parameters = new object[1] { 0 };
				base.photonView.RPC("changeState", PhotonTargets.Others, parameters);
			}
		}
	}

	private void newTitan()
	{
		GameObject gameObject = GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().spawnTitan(normalTitanRate, base.transform.position - Vector3.up * (base.transform.position.y - getHeight(base.transform.position)), base.transform.rotation);
		if (LevelInfo.getInfo(FengGameManagerMKII.level).mapName == "The City I")
		{
			gameObject.GetComponent<TITAN>().chaseDistance = 120f;
		}
		else
		{
			gameObject.GetComponent<TITAN>().chaseDistance = 200f;
		}
		gameObject.GetComponent<TITAN>().PVPfromCheckPt = this;
	}

	private void Start()
	{
		if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		if (IN_GAME_MAIN_CAMERA.gamemode != GAMEMODE.PVP_CAPTURE)
		{
			if (base.photonView.isMine)
			{
				Object.Destroy(base.gameObject);
			}
			Object.Destroy(base.gameObject);
			return;
		}
		chkPts.Add(this);
		IComparer comparer = new IComparerPVPchkPtID();
		chkPts.Sort(comparer);
		if (humanPt == humanPtMax)
		{
			state = CheckPointState.Human;
			if (base.photonView.isMine && LevelInfo.getInfo(FengGameManagerMKII.level).mapName != "The City I")
			{
				supply = PhotonNetwork.Instantiate("aot_supply", base.transform.position - Vector3.up * (base.transform.position.y - getHeight(base.transform.position)), base.transform.rotation, 0);
			}
		}
		else if (base.photonView.isMine && !hasAnnie)
		{
			if (Random.Range(0, 100) < 50)
			{
				int num = Random.Range(1, 2);
				for (int i = 0; i < num; i++)
				{
					newTitan();
				}
			}
			if (isBase)
			{
				newTitan();
			}
		}
		if (titanPt == titanPtMax)
		{
			state = CheckPointState.Titan;
		}
		hitTestR = 15f * size;
		base.transform.localScale = new Vector3(size, size, size);
	}

	private void syncPts()
	{
		if (titanPt != _lastTitanPt)
		{
			object[] parameters = new object[1] { titanPt };
			base.photonView.RPC("changeTitanPt", PhotonTargets.Others, parameters);
			_lastTitanPt = titanPt;
		}
		if (humanPt != _lastHumanPt)
		{
			object[] parameters2 = new object[1] { humanPt };
			base.photonView.RPC("changeHumanPt", PhotonTargets.Others, parameters2);
			_lastHumanPt = humanPt;
		}
	}

	public void OnPhotonPlayerConnected(PhotonPlayer player)
	{
		if (PhotonNetwork.isMasterClient)
		{
			object[] parameters = new object[1] { titanPt };
			base.photonView.RPC("changeTitanPt", player, parameters);
			parameters = new object[1] { humanPt };
			base.photonView.RPC("changeHumanPt", player, parameters);
		}
	}

	private void titanGetsPoint()
	{
		if (titanPt >= titanPtMax)
		{
			titanPt = titanPtMax;
			humanPt = 0f;
			syncPts();
			if (state == CheckPointState.Human && supply != null)
			{
				PhotonNetwork.Destroy(supply);
			}
			state = CheckPointState.Titan;
			object[] parameters = new object[1] { 2 };
			base.photonView.RPC("changeState", PhotonTargets.All, parameters);
			FengGameManagerMKII component = GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>();
			component.PVPtitanScore += 2;
			GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().checkPVPpts();
			if (checkIfTitanWins())
			{
				GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().gameLose2();
			}
			if (hasAnnie)
			{
				if (!annie)
				{
					annie = true;
					PhotonNetwork.Instantiate("FEMALE_TITAN", base.transform.position - Vector3.up * (base.transform.position.y - getHeight(base.transform.position)), base.transform.rotation, 0);
				}
				else
				{
					newTitan();
				}
			}
			else
			{
				newTitan();
			}
		}
		else
		{
			titanPt += Time.deltaTime;
		}
	}

	private void titanLosePoint()
	{
		if (!(titanPt > 0f))
		{
			return;
		}
		titanPt -= Time.deltaTime * 3f;
		if (titanPt <= 0f)
		{
			titanPt = 0f;
			syncPts();
			if (state != CheckPointState.Human)
			{
				state = CheckPointState.Non;
				object[] parameters = new object[1] { 0 };
				base.photonView.RPC("changeState", PhotonTargets.All, parameters);
			}
		}
	}

	private void Update()
	{
		float num = humanPt / humanPtMax;
		float num2 = titanPt / titanPtMax;
		if (!base.photonView.isMine)
		{
			num = humanPt / humanPtMax;
			num2 = titanPt / titanPtMax;
			humanCyc.transform.localScale = new Vector3(num, num, 1f);
			titanCyc.transform.localScale = new Vector3(num2, num2, 1f);
			syncTimer += Time.deltaTime;
			if (syncTimer > syncInterval)
			{
				syncTimer = 0f;
				checkIfBeingCapture();
			}
			return;
		}
		if (state == CheckPointState.Non)
		{
			if (playerOn && !titanOn)
			{
				humanGetsPoint();
				titanLosePoint();
			}
			else if (titanOn && !playerOn)
			{
				titanGetsPoint();
				humanLosePoint();
			}
			else
			{
				humanLosePoint();
				titanLosePoint();
			}
		}
		else if (state == CheckPointState.Human)
		{
			if (titanOn && !playerOn)
			{
				titanGetsPoint();
			}
			else
			{
				titanLosePoint();
			}
			getPtsTimer += Time.deltaTime;
			if (getPtsTimer > getPtsInterval)
			{
				getPtsTimer = 0f;
				if (!isBase)
				{
					FengGameManagerMKII component = GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>();
					component.PVPhumanScore++;
				}
				GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().checkPVPpts();
			}
		}
		else if (state == CheckPointState.Titan)
		{
			if (playerOn && !titanOn)
			{
				humanGetsPoint();
			}
			else
			{
				humanLosePoint();
			}
			getPtsTimer += Time.deltaTime;
			if (getPtsTimer > getPtsInterval)
			{
				getPtsTimer = 0f;
				if (!isBase)
				{
					FengGameManagerMKII component2 = GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>();
					component2.PVPtitanScore++;
				}
				GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().checkPVPpts();
			}
			spawnTitanTimer += Time.deltaTime;
			if (spawnTitanTimer > titanInterval)
			{
				spawnTitanTimer = 0f;
				if (LevelInfo.getInfo(FengGameManagerMKII.level).mapName == "The City I")
				{
					if (GameObject.FindGameObjectsWithTag("titan").Length < 12)
					{
						newTitan();
					}
				}
				else if (GameObject.FindGameObjectsWithTag("titan").Length < 20)
				{
					newTitan();
				}
			}
		}
		syncTimer += Time.deltaTime;
		if (syncTimer > syncInterval)
		{
			syncTimer = 0f;
			checkIfBeingCapture();
			syncPts();
		}
		num = humanPt / humanPtMax;
		num2 = titanPt / titanPtMax;
		humanCyc.transform.localScale = new Vector3(num, num, 1f);
		titanCyc.transform.localScale = new Vector3(num2, num2, 1f);
	}
}
