using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomSkins
{
	internal class TitanCustomSkinLoader : BaseCustomSkinLoader
	{
		protected override string RendererIdPrefix
		{
			get
			{
				return "titan";
			}
		}

		public override IEnumerator LoadSkinsFromRPC(object[] data)
		{
			if ((bool)data[0])
			{
				string url = (string)data[1];
				BaseCustomSkinPart customSkinPart = GetCustomSkinPart(0);
				if (!customSkinPart.LoadCache(url))
				{
					yield return StartCoroutine(customSkinPart.LoadSkin(url));
				}
			}
			else
			{
				string url2 = (string)data[1];
				string eyeUrl = (string)data[2];
				BaseCustomSkinPart customSkinPart2 = GetCustomSkinPart(1);
				if (!customSkinPart2.LoadCache(url2))
				{
					yield return StartCoroutine(customSkinPart2.LoadSkin(url2));
				}
				BaseCustomSkinPart customSkinPart3 = GetCustomSkinPart(2);
				if (!customSkinPart3.LoadCache(eyeUrl))
				{
					yield return StartCoroutine(customSkinPart3.LoadSkin(eyeUrl));
				}
			}
			FengGameManagerMKII.instance.unloadAssets();
		}

		protected override BaseCustomSkinPart GetCustomSkinPart(int partId)
		{
			TITAN component = _owner.GetComponent<TITAN>();
			List<Renderer> renderers = new List<Renderer>();
			switch ((TitanCustomSkinPartId)partId)
			{
			case TitanCustomSkinPartId.Hair:
				AddRendererIfExists(renderers, component.GetComponent<TITAN_SETUP>().part_hair);
				return new BaseCustomSkinPart(this, renderers, GetRendererId(partId), 500000);
			case TitanCustomSkinPartId.Body:
				AddRenderersMatchingName(renderers, _owner, "hair");
				return new BaseCustomSkinPart(this, renderers, GetRendererId(partId), 1000000);
			case TitanCustomSkinPartId.Eye:
				AddRenderersContainingName(renderers, _owner, "eye");
				return new BaseCustomSkinPart(this, renderers, GetRendererId(partId), 500000, new Vector2(4f, 8f));
			default:
				return null;
			}
		}
	}
}
