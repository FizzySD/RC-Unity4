using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomSkins
{
	internal class AnnieCustomSkinLoader : BaseCustomSkinLoader
	{
		protected override string RendererIdPrefix
		{
			get
			{
				return "annie";
			}
		}

		public override IEnumerator LoadSkinsFromRPC(object[] data)
		{
			string url = (string)data[0];
			BaseCustomSkinPart customSkinPart = GetCustomSkinPart(0);
			if (!customSkinPart.LoadCache(url))
			{
				yield return StartCoroutine(customSkinPart.LoadSkin(url));
			}
			FengGameManagerMKII.instance.unloadAssets();
		}

		protected override BaseCustomSkinPart GetCustomSkinPart(int partId)
		{
			List<Renderer> renderers = new List<Renderer>();
			if (partId == 0)
			{
				AddAllRenderers(renderers, _owner);
				return new BaseCustomSkinPart(this, renderers, GetRendererId(partId), 2000000);
			}
			return null;
		}
	}
}
