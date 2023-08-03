using System.Collections;
using System.Collections.Generic;
using Settings;
using UnityEngine;
using Xft;

namespace CustomSkins
{
	internal class HumanCustomSkinLoader : BaseCustomSkinLoader
	{
		private int _horseViewId;

		public HookCustomSkinPart HookL;

		public HookCustomSkinPart HookR;

		public float HookLTiling = 1f;

		public float HookRTiling = 1f;

		protected override string RendererIdPrefix
		{
			get
			{
				return "human";
			}
		}

		public override IEnumerator LoadSkinsFromRPC(object[] data)
		{
			_horseViewId = (int)data[0];
			string[] skinUrls = ((string)data[1]).Split(',');
			foreach (int partId in GetCustomSkinPartIds(typeof(HumanCustomSkinPartId)))
			{
				if ((partId == 0 && _horseViewId < 0) || (partId == 12 && !_owner.GetComponent<HERO>().IsMine()) || (partId == 10 && !SettingsManager.CustomSkinSettings.Human.GasEnabled.Value))
				{
					continue;
				}
				if (partId == 16 && skinUrls.Length > partId)
				{
					float.TryParse(skinUrls[partId], out HookLTiling);
				}
				else if (partId == 18 && skinUrls.Length > partId)
				{
					float.TryParse(skinUrls[partId], out HookRTiling);
				}
				else if ((partId != 15 || SettingsManager.CustomSkinSettings.Human.HookEnabled.Value) && (partId != 17 || SettingsManager.CustomSkinSettings.Human.HookEnabled.Value))
				{
					BaseCustomSkinPart part = GetCustomSkinPart(partId);
					if (skinUrls.Length > partId && !part.LoadCache(skinUrls[partId]))
					{
						yield return StartCoroutine(part.LoadSkin(skinUrls[partId]));
					}
					switch (partId)
					{
					case 15:
						HookL = (HookCustomSkinPart)part;
						break;
					case 17:
						HookR = (HookCustomSkinPart)part;
						break;
					}
				}
			}
			FengGameManagerMKII.instance.unloadAssets();
		}

		protected override BaseCustomSkinPart GetCustomSkinPart(int partId)
		{
			HERO component = _owner.GetComponent<HERO>();
			List<Renderer> renderers = new List<Renderer>();
			switch ((HumanCustomSkinPartId)partId)
			{
			case HumanCustomSkinPartId.Horse:
				AddRenderersMatchingName(renderers, PhotonView.Find(_horseViewId).gameObject, "HORSE");
				return new BaseCustomSkinPart(this, renderers, GetRendererId(partId), 1000000);
			case HumanCustomSkinPartId.Hair:
				AddRendererIfExists(renderers, component.setup.part_hair);
				AddRendererIfExists(renderers, component.setup.part_hair_1);
				return new HumanHairCustomSkinPart(this, renderers, GetRendererId(partId), 1000000, component.setup.myCostume.hairInfo);
			case HumanCustomSkinPartId.Eye:
				AddRendererIfExists(renderers, component.setup.part_eye);
				return new BaseCustomSkinPart(this, renderers, GetRendererId(partId), 500000, new Vector2(8f, 8f));
			case HumanCustomSkinPartId.Glass:
				AddRendererIfExists(renderers, component.setup.part_glass);
				return new BaseCustomSkinPart(this, renderers, GetRendererId(partId), 500000, new Vector2(8f, 8f));
			case HumanCustomSkinPartId.Face:
				AddRendererIfExists(renderers, component.setup.part_face);
				return new BaseCustomSkinPart(this, renderers, GetRendererId(partId), 500000, new Vector2(8f, 8f));
			case HumanCustomSkinPartId.Skin:
				AddRendererIfExists(renderers, component.setup.part_hand_l);
				AddRendererIfExists(renderers, component.setup.part_hand_r);
				AddRendererIfExists(renderers, component.setup.part_head);
				AddRendererIfExists(renderers, component.setup.part_chest);
				return new BaseCustomSkinPart(this, renderers, GetRendererId(partId), 1000000);
			case HumanCustomSkinPartId.Costume:
				AddRendererIfExists(renderers, component.setup.part_arm_l);
				AddRendererIfExists(renderers, component.setup.part_arm_r);
				AddRendererIfExists(renderers, component.setup.part_leg);
				AddRendererIfExists(renderers, component.setup.part_chest_2);
				AddRendererIfExists(renderers, component.setup.part_chest_3);
				AddRendererIfExists(renderers, component.setup.part_upper_body);
				return new BaseCustomSkinPart(this, renderers, GetRendererId(partId), 2000000, null, true);
			case HumanCustomSkinPartId.Logo:
				AddRendererIfExists(renderers, component.setup.part_cape);
				AddRendererIfExists(renderers, component.setup.part_brand_1);
				AddRendererIfExists(renderers, component.setup.part_brand_2);
				AddRendererIfExists(renderers, component.setup.part_brand_3);
				AddRendererIfExists(renderers, component.setup.part_brand_4);
				return new BaseCustomSkinPart(this, renderers, GetRendererId(partId), 500000);
			case HumanCustomSkinPartId.GearL:
				AddRendererIfExists(renderers, component.setup.part_3dmg);
				AddRendererIfExists(renderers, component.setup.part_3dmg_belt);
				AddRendererIfExists(renderers, component.setup.part_3dmg_gas_l);
				AddRendererIfExists(renderers, component.setup.part_blade_l);
				return new BaseCustomSkinPart(this, renderers, GetRendererId(partId), 1000000);
			case HumanCustomSkinPartId.GearR:
				AddRendererIfExists(renderers, component.setup.part_3dmg_gas_r);
				AddRendererIfExists(renderers, component.setup.part_blade_r);
				return new BaseCustomSkinPart(this, renderers, GetRendererId(partId), 1000000);
			case HumanCustomSkinPartId.Gas:
				AddRendererIfExists(renderers, component.transform.Find("3dmg_smoke").gameObject);
				return new BaseCustomSkinPart(this, renderers, GetRendererId(partId), 500000);
			case HumanCustomSkinPartId.Hoodie:
				if (component.setup.part_chest_1 != null && component.setup.part_chest_1.name.Contains("character_cap"))
				{
					AddRendererIfExists(renderers, component.setup.part_chest_1);
				}
				return new BaseCustomSkinPart(this, renderers, GetRendererId(partId), 500000);
			case HumanCustomSkinPartId.WeaponTrail:
			{
				List<XWeaponTrail> list = new List<XWeaponTrail>();
				list.Add(component.leftbladetrail);
				list.Add(component.leftbladetrail2);
				list.Add(component.rightbladetrail);
				list.Add(component.rightbladetrail2);
				return new WeaponTrailCustomSkinPart(this, list, GetRendererId(partId), 500000);
			}
			case HumanCustomSkinPartId.ThunderspearL:
				if (component.ThunderSpearLModel != null)
				{
					AddRendererIfExists(renderers, component.ThunderSpearLModel);
				}
				return new BaseCustomSkinPart(this, renderers, GetRendererId(partId), 1000000);
			case HumanCustomSkinPartId.ThunderspearR:
				if (component.ThunderSpearRModel != null)
				{
					AddRendererIfExists(renderers, component.ThunderSpearRModel);
				}
				return new BaseCustomSkinPart(this, renderers, GetRendererId(partId), 1000000);
			case HumanCustomSkinPartId.HookL:
			case HumanCustomSkinPartId.HookR:
				return new HookCustomSkinPart(this, GetRendererId(partId), 500000);
			default:
				return null;
			}
		}
	}
}
