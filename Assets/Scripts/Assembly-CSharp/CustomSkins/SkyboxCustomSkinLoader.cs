using System.Collections;
using UnityEngine;

namespace CustomSkins
{
	internal class SkyboxCustomSkinLoader : BaseCustomSkinLoader
	{
		public static Material SkyboxMaterial;

		protected override string RendererIdPrefix
		{
			get
			{
				return "skybox";
			}
		}

		public override IEnumerator LoadSkinsFromRPC(object[] data)
		{
			SkyboxMaterial = new Material(Shader.Find("RenderFX/Skybox"));
			SkyboxMaterial.CopyPropertiesFromMaterial(Camera.main.GetComponent<Skybox>().material);
			foreach (int customSkinPartId in GetCustomSkinPartIds(typeof(SkyboxCustomSkinPartId)))
			{
				BaseCustomSkinPart customSkinPart = GetCustomSkinPart(customSkinPartId);
				string url = (string)data[customSkinPartId];
				if (!customSkinPart.LoadCache(url))
				{
					yield return StartCoroutine(customSkinPart.LoadSkin(url));
				}
			}
		}

		protected override BaseCustomSkinPart GetCustomSkinPart(int partId)
		{
			return new SkyboxCustomSkinPart(this, SkyboxMaterial, PartIdToTextureName((SkyboxCustomSkinPartId)partId), GetRendererId(partId), 2000000);
		}

		public string PartIdToTextureName(SkyboxCustomSkinPartId partId)
		{
			return "_" + partId.ToString() + "Tex";
		}
	}
}
