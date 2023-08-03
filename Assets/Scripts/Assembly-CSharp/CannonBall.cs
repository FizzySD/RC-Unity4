using System.Collections;
using System.Collections.Generic;
using Photon;
using Settings;
using UnityEngine;

internal class CannonBall : Photon.MonoBehaviour
{
	private Vector3 correctPos;

	private Vector3 correctVelocity;

	public bool disabled;

	public Transform firingPoint;

	public bool isCollider;

	public HERO myHero;

	public List<TitanTrigger> myTitanTriggers;

	public float SmoothingDelay = 10f;

	private void Awake()
	{
		if (base.photonView != null)
		{
			base.photonView.observed = this;
			correctPos = base.transform.position;
			correctVelocity = Vector3.zero;
			GetComponent<SphereCollider>().enabled = false;
			if (base.photonView.isMine)
			{
				StartCoroutine(WaitAndDestroy(10f));
				myTitanTriggers = new List<TitanTrigger>();
			}
		}
	}

	public void destroyMe()
	{
		if (disabled)
		{
			return;
		}
		disabled = true;
		GameObject gameObject = PhotonNetwork.Instantiate("FX/boom4", base.transform.position, base.transform.rotation, 0);
		EnemyCheckCollider[] componentsInChildren = gameObject.GetComponentsInChildren<EnemyCheckCollider>();
		foreach (EnemyCheckCollider enemyCheckCollider in componentsInChildren)
		{
			enemyCheckCollider.dmg = 0;
		}
		if (SettingsManager.LegacyGameSettings.CannonsFriendlyFire.Value)
		{
			foreach (HERO player in FengGameManagerMKII.instance.getPlayers())
			{
				if (!(player != null) || !(Vector3.Distance(player.transform.position, base.transform.position) <= 20f) || player.photonView.isMine)
				{
					continue;
				}
				GameObject gameObject2 = player.gameObject;
				PhotonPlayer owner = gameObject2.GetPhotonView().owner;
				if (SettingsManager.LegacyGameSettings.TeamMode.Value > 0 && PhotonNetwork.player.customProperties[PhotonPlayerProperty.RCteam] != null && owner.customProperties[PhotonPlayerProperty.RCteam] != null)
				{
					int num = RCextensions.returnIntFromObject(PhotonNetwork.player.customProperties[PhotonPlayerProperty.RCteam]);
					int num2 = RCextensions.returnIntFromObject(owner.customProperties[PhotonPlayerProperty.RCteam]);
					if (num == 0 || num != num2)
					{
						gameObject2.GetComponent<HERO>().markDie();
						gameObject2.GetComponent<HERO>().photonView.RPC("netDie2", PhotonTargets.All, -1, RCextensions.returnStringFromObject(PhotonNetwork.player.customProperties[PhotonPlayerProperty.name]) + " ");
						FengGameManagerMKII.instance.playerKillInfoUpdate(PhotonNetwork.player, 0);
					}
				}
				else
				{
					gameObject2.GetComponent<HERO>().markDie();
					gameObject2.GetComponent<HERO>().photonView.RPC("netDie2", PhotonTargets.All, -1, RCextensions.returnStringFromObject(PhotonNetwork.player.customProperties[PhotonPlayerProperty.name]) + " ");
					FengGameManagerMKII.instance.playerKillInfoUpdate(PhotonNetwork.player, 0);
				}
			}
		}
		if (myTitanTriggers != null)
		{
			for (int j = 0; j < myTitanTriggers.Count; j++)
			{
				if (myTitanTriggers[j] != null)
				{
					myTitanTriggers[j].isCollide = false;
				}
			}
		}
		PhotonNetwork.Destroy(base.gameObject);
	}

	public void FixedUpdate()
	{
		if (!base.photonView.isMine || disabled)
		{
			return;
		}
		LayerMask layerMask = 1 << LayerMask.NameToLayer("PlayerAttackBox");
		LayerMask layerMask2 = 1 << LayerMask.NameToLayer("EnemyBox");
		LayerMask layerMask3 = (int)layerMask | (int)layerMask2;
		if (!isCollider)
		{
			LayerMask layerMask4 = 1 << LayerMask.NameToLayer("Ground");
			layerMask3 = (int)layerMask3 | (int)layerMask4;
		}
		Collider[] array = Physics.OverlapSphere(base.transform.position, 0.6f, layerMask3.value);
		bool flag = false;
		for (int i = 0; i < array.Length; i++)
		{
			GameObject gameObject = array[i].gameObject;
			if (gameObject.layer == 16)
			{
				TitanTrigger component = gameObject.GetComponent<TitanTrigger>();
				if (!(component == null) && !myTitanTriggers.Contains(component))
				{
					component.isCollide = true;
					myTitanTriggers.Add(component);
				}
			}
			else if (gameObject.layer == 10)
			{
				TITAN component2 = gameObject.transform.root.gameObject.GetComponent<TITAN>();
				if (!(component2 != null))
				{
					continue;
				}
				if (component2.abnormalType == AbnormalType.TYPE_CRAWLER)
				{
					if (gameObject.name == "head")
					{
						component2.photonView.RPC("DieByCannon", component2.photonView.owner, myHero.photonView.viewID);
						component2.dieBlow(base.transform.position, 0.2f);
						i = array.Length;
					}
				}
				else if (gameObject.name == "head")
				{
					component2.photonView.RPC("DieByCannon", component2.photonView.owner, myHero.photonView.viewID);
					component2.dieHeadBlow(base.transform.position, 0.2f);
					i = array.Length;
				}
				else if (Random.Range(0f, 1f) < 0.5f)
				{
					component2.hitL(base.transform.position, 0.05f);
				}
				else
				{
					component2.hitR(base.transform.position, 0.05f);
				}
				destroyMe();
			}
			else if (gameObject.layer == 9 && (gameObject.transform.root.name.Contains("CannonWall") || gameObject.transform.root.name.Contains("CannonGround")))
			{
				flag = true;
			}
		}
		if (!(isCollider || flag))
		{
			isCollider = true;
			GetComponent<SphereCollider>().enabled = true;
		}
	}

	public void OnCollisionEnter(Collision myCollision)
	{
		if (base.photonView.isMine)
		{
			destroyMe();
		}
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			stream.SendNext(base.transform.position);
			stream.SendNext(base.rigidbody.velocity);
		}
		else
		{
			correctPos = (Vector3)stream.ReceiveNext();
			correctVelocity = (Vector3)stream.ReceiveNext();
		}
	}

	public void Update()
	{
		if (!base.photonView.isMine)
		{
			base.transform.position = Vector3.Lerp(base.transform.position, correctPos, Time.deltaTime * SmoothingDelay);
			base.rigidbody.velocity = correctVelocity;
		}
	}

	public IEnumerator WaitAndDestroy(float time)
	{
		yield return new WaitForSeconds(time);
		destroyMe();
	}
}
