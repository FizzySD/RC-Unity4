using System.Collections;
using System.Collections.Generic;
using Constants;
using GameProgress;
using Photon;
using Settings;
using UnityEngine;

internal class Bomb : Photon.MonoBehaviour
{
	private Vector3 correctPlayerPos = Vector3.zero;

	private Quaternion correctPlayerRot = Quaternion.identity;

	private Vector3 correctPlayerVelocity = Vector3.zero;

	public bool Disabled;

	public float SmoothingDelay = 10f;

	public float BombRadius;

	private TITAN _collidedTitan;

	private SphereCollider _sphereCollider;

	private List<GameObject> _hideUponDestroy;

	private ParticleSystem _trail;

	private ParticleSystem _flame;

	private float _DisabledTrailFadeMultiplier = 0.6f;

	private HERO _owner;

	private bool _receivedNonZeroVelocity;

	private bool _ownerIsUpdated;

	public void Setup(HERO owner, float bombRadius)
	{
		_owner = owner;
		BombRadius = bombRadius;
	}

	public void Awake()
	{
		if (base.photonView != null)
		{
			base.photonView.observed = this;
			correctPlayerPos = base.transform.position;
			correctPlayerRot = base.transform.rotation;
			PhotonPlayer owner = base.photonView.owner;
			_trail = base.transform.Find("Trail").GetComponent<ParticleSystem>();
			_flame = base.transform.Find("Flame").GetComponent<ParticleSystem>();
			_sphereCollider = GetComponent<SphereCollider>();
			_hideUponDestroy = new List<GameObject>();
			_hideUponDestroy.Add(base.transform.Find("Flame").gameObject);
			_hideUponDestroy.Add(base.transform.Find("ThunderSpearModel").gameObject);
			if (SettingsManager.AbilitySettings.ShowBombColors.Value)
			{
				Color bombColor = BombUtil.GetBombColor(owner, 1f);
				_trail.startColor = bombColor;
				_flame.startColor = bombColor;
			}
			if (base.photonView.isMine)
			{
				base.photonView.RPC("IsUpdatedRPC", PhotonTargets.All);
			}
		}
	}

	[RPC]
	private void IsUpdatedRPC(PhotonMessageInfo info)
	{
		if (info.sender == base.photonView.owner)
		{
			_ownerIsUpdated = true;
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (base.photonView.isMine && !Disabled)
		{
			Explode(BombRadius);
		}
	}

	public void DestroySelf()
	{
		if (base.photonView.isMine && !Disabled)
		{
			base.photonView.RPC("DisableRPC", PhotonTargets.All);
			StartCoroutine(WaitAndFinishDestroyCoroutine(1.5f));
		}
	}

	private IEnumerator WaitAndFinishDestroyCoroutine(float time)
	{
		yield return new WaitForSeconds(time);
		if (_collidedTitan != null)
		{
			_collidedTitan.isThunderSpear = false;
		}
		PhotonNetwork.Destroy(base.gameObject);
	}

	[RPC]
	public void DisableRPC(PhotonMessageInfo info = null)
	{
		if (Disabled || (info != null && info.sender != base.photonView.owner))
		{
			return;
		}
		foreach (GameObject item in _hideUponDestroy)
		{
			item.SetActive(false);
		}
		_sphereCollider.enabled = false;
		SetDisabledTrailFade();
		base.rigidbody.velocity = Vector3.zero;
		Disabled = true;
	}

	private void SetDisabledTrailFade()
	{
		int particleCount = _trail.particleCount;
		float startLifetime = _trail.startLifetime * _DisabledTrailFadeMultiplier;
		ParticleSystem.Particle[] array = new ParticleSystem.Particle[particleCount];
		_trail.GetParticles(array);
		for (int i = 0; i < particleCount; i++)
		{
			array[i].lifetime *= _DisabledTrailFadeMultiplier;
			array[i].startLifetime = startLifetime;
		}
		_trail.SetParticles(array, particleCount);
	}

	public void Explode(float radius)
	{
		if (!Disabled)
		{
			PhotonNetwork.Instantiate("RCAsset/BombExplodeMain", base.transform.position, Quaternion.Euler(0f, 0f, 0f), 0);
			KillPlayersInRadius(radius);
			DestroySelf();
		}
	}

	private void KillPlayersInRadius(float radius)
	{
		foreach (HERO player in FengGameManagerMKII.instance.getPlayers())
		{
			GameObject gameObject = player.gameObject;
			if (!(Vector3.Distance(gameObject.transform.position, base.transform.position) < radius) || gameObject.GetPhotonView().isMine || player.bombImmune)
			{
				continue;
			}
			PhotonPlayer owner = gameObject.GetPhotonView().owner;
			if (SettingsManager.LegacyGameSettings.TeamMode.Value > 0 && PhotonNetwork.player.customProperties[PhotonPlayerProperty.RCteam] != null && owner.customProperties[PhotonPlayerProperty.RCteam] != null)
			{
				int num = RCextensions.returnIntFromObject(PhotonNetwork.player.customProperties[PhotonPlayerProperty.RCteam]);
				int num2 = RCextensions.returnIntFromObject(owner.customProperties[PhotonPlayerProperty.RCteam]);
				if (num == 0 || num != num2)
				{
					KillPlayer(player);
				}
			}
			else
			{
				KillPlayer(player);
			}
		}
	}

	private void KillPlayer(HERO hero)
	{
		hero.markDie();
		hero.photonView.RPC("netDie2", PhotonTargets.All, -1, RCextensions.returnStringFromObject(PhotonNetwork.player.customProperties[PhotonPlayerProperty.name]) + " ");
		FengGameManagerMKII.instance.playerKillInfoUpdate(PhotonNetwork.player, 0);
		GameProgressManager.RegisterHumanKill(_owner.gameObject, hero, KillWeapon.ThunderSpear);
	}

	private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			stream.SendNext(base.transform.position);
			stream.SendNext(base.transform.rotation);
			stream.SendNext(base.rigidbody.velocity);
		}
		else
		{
			correctPlayerPos = (Vector3)stream.ReceiveNext();
			correctPlayerRot = (Quaternion)stream.ReceiveNext();
			correctPlayerVelocity = (Vector3)stream.ReceiveNext();
		}
	}

	private void Update()
	{
		if (base.photonView.isMine)
		{
			return;
		}
		base.transform.position = Vector3.Lerp(base.transform.position, correctPlayerPos, Time.deltaTime * SmoothingDelay);
		base.transform.rotation = Quaternion.Lerp(base.transform.rotation, correctPlayerRot, Time.deltaTime * SmoothingDelay);
		base.rigidbody.velocity = correctPlayerVelocity;
		if (base.rigidbody.velocity != Vector3.zero)
		{
			_receivedNonZeroVelocity = true;
		}
		else
		{
			if (_ownerIsUpdated || !_receivedNonZeroVelocity || Disabled)
			{
				return;
			}
			Disabled = true;
			foreach (GameObject item in _hideUponDestroy)
			{
				item.SetActive(false);
			}
		}
	}

	private void FixedUpdate()
	{
		if (!Disabled && base.photonView.isMine)
		{
			CheckCollide();
		}
	}

	private void CheckCollide()
	{
		LayerMask layerMask = (1 << PhysicsLayer.PlayerAttackBox) | (1 << PhysicsLayer.PlayerHitBox);
		Collider[] array = Physics.OverlapSphere(base.transform.position + _sphereCollider.center, _sphereCollider.radius, layerMask);
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			if (collider.name.Contains("PlayerDetectorRC"))
			{
				TITAN component = collider.transform.root.gameObject.GetComponent<TITAN>();
				if (component != null)
				{
					if (_collidedTitan == null)
					{
						_collidedTitan = component;
						_collidedTitan.isThunderSpear = true;
					}
					else if (_collidedTitan != component)
					{
						_collidedTitan.isThunderSpear = false;
						_collidedTitan = component;
						_collidedTitan.isThunderSpear = true;
					}
				}
			}
			else if (collider.gameObject.layer == PhysicsLayer.PlayerHitBox)
			{
				HERO component2 = collider.transform.root.gameObject.GetComponent<HERO>();
				if (!component2.photonView.isMine)
				{
					Explode(BombRadius);
				}
			}
		}
	}
}
