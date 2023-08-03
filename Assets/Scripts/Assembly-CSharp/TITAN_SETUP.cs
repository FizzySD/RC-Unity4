using System.Collections;
using CustomSkins;
using Photon;
using Settings;
using UnityEngine;

public class TITAN_SETUP : Photon.MonoBehaviour
{
	public GameObject eye;

	private CostumeHair hair;

	private GameObject hair_go_ref;

	private int hairType;

	public bool haseye;

	public GameObject part_hair;

	public int skin;

	private TitanCustomSkinLoader _customSkinLoader;

	private void Awake()
	{
		CostumeHair.init();
		CharacterMaterials.init();
		HeroCostume.init2();
		hair_go_ref = new GameObject();
		eye.transform.parent = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head").transform;
		hair_go_ref.transform.position = eye.transform.position + Vector3.up * 3.5f + base.transform.forward * 5.2f;
		hair_go_ref.transform.rotation = eye.transform.rotation;
		hair_go_ref.transform.RotateAround(eye.transform.position, base.transform.right, -20f);
		hair_go_ref.transform.localScale = new Vector3(210f, 210f, 210f);
		hair_go_ref.transform.parent = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head").transform;
		_customSkinLoader = base.gameObject.AddComponent<TitanCustomSkinLoader>();
	}

	public IEnumerator loadskinE(int hair, int eye, string hairlink)
	{
		Object.Destroy(part_hair);
		this.hair = CostumeHair.hairsM[hair];
		hairType = hair;
		if (this.hair.hair != string.Empty)
		{
			GameObject gameObject = (GameObject)Object.Instantiate(Resources.Load("Character/" + this.hair.hair));
			gameObject.transform.parent = hair_go_ref.transform.parent;
			gameObject.transform.position = hair_go_ref.transform.position;
			gameObject.transform.rotation = hair_go_ref.transform.rotation;
			gameObject.transform.localScale = hair_go_ref.transform.localScale;
			gameObject.renderer.material = CharacterMaterials.materials[this.hair.texture];
			part_hair = gameObject;
			yield return StartCoroutine(_customSkinLoader.LoadSkinsFromRPC(new object[2] { true, hairlink }));
		}
		if (eye >= 0)
		{
			setFacialTexture(this.eye, eye);
		}
		yield return null;
	}

	public void setFacialTexture(GameObject go, int id)
	{
		if (id >= 0)
		{
			float num = 0.25f;
			float num2 = 0.125f;
			float x = num2 * (float)(int)((float)id / 8f);
			float y = (0f - num) * (float)(id % 4);
			go.renderer.material.mainTextureOffset = new Vector2(x, y);
		}
	}

	public void setHair2()
	{
		BaseCustomSkinSettings<TitanCustomSkinSet> titan = SettingsManager.CustomSkinSettings.Titan;
		if (titan.SkinsEnabled.Value && (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE || base.photonView.isMine))
		{
			TitanCustomSkinSet titanCustomSkinSet = (TitanCustomSkinSet)titan.GetSelectedSet();
			int num = Random.Range(0, 9);
			if (num == 3)
			{
				num = 9;
			}
			int index = skin;
			if (titanCustomSkinSet.RandomizedPairs.Value)
			{
				index = Random.Range(0, 5);
			}
			int num2 = ((IntSetting)titanCustomSkinSet.HairModels.GetItemAt(index)).Value - 1;
			if (num2 >= 0)
			{
				num = num2;
			}
			string value = ((StringSetting)titanCustomSkinSet.Hairs.GetItemAt(index)).Value;
			int num3 = Random.Range(1, 8);
			if (haseye)
			{
				num3 = 0;
			}
			bool flag = false;
			if (value.EndsWith(".jpg") || value.EndsWith(".png") || value.EndsWith(".jpeg"))
			{
				flag = true;
			}
			if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && base.photonView.isMine)
			{
				if (flag)
				{
					object[] parameters = new object[3] { num, num3, value };
					base.photonView.RPC("setHairRPC2", PhotonTargets.AllBuffered, parameters);
				}
				else
				{
					Color hair_color = HeroCostume.costume[Random.Range(0, HeroCostume.costume.Length - 5)].hair_color;
					object[] parameters = new object[5] { num, num3, hair_color.r, hair_color.g, hair_color.b };
					base.photonView.RPC("setHairPRC", PhotonTargets.AllBuffered, parameters);
				}
			}
			else if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
			{
				if (flag)
				{
					StartCoroutine(loadskinE(num, num3, value));
					return;
				}
				Color hair_color = HeroCostume.costume[Random.Range(0, HeroCostume.costume.Length - 5)].hair_color;
				setHairPRC(num, num3, hair_color.r, hair_color.g, hair_color.b);
			}
		}
		else
		{
			int num = Random.Range(0, CostumeHair.hairsM.Length);
			if (num == 3)
			{
				num = 9;
			}
			Object.Destroy(part_hair);
			hairType = num;
			hair = CostumeHair.hairsM[num];
			if (hair.hair == string.Empty)
			{
				hair = CostumeHair.hairsM[9];
				hairType = 9;
			}
			part_hair = (GameObject)Object.Instantiate(Resources.Load("Character/" + hair.hair));
			part_hair.transform.parent = hair_go_ref.transform.parent;
			part_hair.transform.position = hair_go_ref.transform.position;
			part_hair.transform.rotation = hair_go_ref.transform.rotation;
			part_hair.transform.localScale = hair_go_ref.transform.localScale;
			part_hair.renderer.material = CharacterMaterials.materials[hair.texture];
			part_hair.renderer.material.color = HeroCostume.costume[Random.Range(0, HeroCostume.costume.Length - 5)].hair_color;
			int num4 = Random.Range(1, 8);
			setFacialTexture(eye, num4);
			if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && base.photonView.isMine)
			{
				object[] parameters = new object[5]
				{
					hairType,
					num4,
					part_hair.renderer.material.color.r,
					part_hair.renderer.material.color.g,
					part_hair.renderer.material.color.b
				};
				base.photonView.RPC("setHairPRC", PhotonTargets.OthersBuffered, parameters);
			}
		}
	}

	[RPC]
	private void setHairPRC(int type, int eye_type, float c1, float c2, float c3)
	{
		Object.Destroy(part_hair);
		hair = CostumeHair.hairsM[type];
		hairType = type;
		if (hair.hair != string.Empty)
		{
			GameObject gameObject = (GameObject)Object.Instantiate(Resources.Load("Character/" + hair.hair));
			gameObject.transform.parent = hair_go_ref.transform.parent;
			gameObject.transform.position = hair_go_ref.transform.position;
			gameObject.transform.rotation = hair_go_ref.transform.rotation;
			gameObject.transform.localScale = hair_go_ref.transform.localScale;
			gameObject.renderer.material = CharacterMaterials.materials[hair.texture];
			gameObject.renderer.material.color = new Color(c1, c2, c3);
			part_hair = gameObject;
		}
		setFacialTexture(eye, eye_type);
	}

	[RPC]
	public void setHairRPC2(int hair, int eye, string hairlink, PhotonMessageInfo info)
	{
		BaseCustomSkinSettings<TitanCustomSkinSet> titan = SettingsManager.CustomSkinSettings.Titan;
		if (info.sender == base.photonView.owner && titan.SkinsEnabled.Value && (!titan.SkinsLocal.Value || base.photonView.isMine))
		{
			StartCoroutine(loadskinE(hair, eye, hairlink));
		}
	}

	public void setPunkHair()
	{
		Object.Destroy(part_hair);
		hair = CostumeHair.hairsM[3];
		hairType = 3;
		GameObject gameObject = (GameObject)Object.Instantiate(Resources.Load("Character/" + hair.hair));
		gameObject.transform.parent = hair_go_ref.transform.parent;
		gameObject.transform.position = hair_go_ref.transform.position;
		gameObject.transform.rotation = hair_go_ref.transform.rotation;
		gameObject.transform.localScale = hair_go_ref.transform.localScale;
		gameObject.renderer.material = CharacterMaterials.materials[hair.texture];
		switch (Random.Range(1, 4))
		{
		case 1:
			gameObject.renderer.material.color = FengColor.hairPunk1;
			break;
		case 2:
			gameObject.renderer.material.color = FengColor.hairPunk2;
			break;
		case 3:
			gameObject.renderer.material.color = FengColor.hairPunk3;
			break;
		}
		part_hair = gameObject;
		setFacialTexture(eye, 0);
		if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && base.photonView.isMine)
		{
			object[] parameters = new object[5]
			{
				hairType,
				0,
				part_hair.renderer.material.color.r,
				part_hair.renderer.material.color.g,
				part_hair.renderer.material.color.b
			};
			base.photonView.RPC("setHairPRC", PhotonTargets.OthersBuffered, parameters);
		}
	}

	public void setVar(int skin, bool haseye)
	{
		this.skin = skin;
		this.haseye = haseye;
	}
}
