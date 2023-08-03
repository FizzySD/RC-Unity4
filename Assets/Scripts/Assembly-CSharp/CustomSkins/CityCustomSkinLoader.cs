using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomSkins
{
	internal class CityCustomSkinLoader : LevelCustomSkinLoader
	{
		private List<GameObject> _houseObjects = new List<GameObject>();

		private List<GameObject> _groundObjects = new List<GameObject>();

		private List<GameObject> _wallObjects = new List<GameObject>();

		private List<GameObject> _gateObjects = new List<GameObject>();

		protected override string RendererIdPrefix
		{
			get
			{
				return "city";
			}
		}

		public override IEnumerator LoadSkinsFromRPC(object[] data)
		{
			FindAndIndexLevelObjects();
			char[] randomIndices = ((string)data[0]).ToCharArray();
			string[] houseUrls = ((string)data[1]).Split(',');
			string[] miscUrls = ((string)data[2]).Split(',');
			for (int i = 0; i < _houseObjects.Count; i++)
			{
				int num = int.Parse(randomIndices[i].ToString());
				BaseCustomSkinPart customSkinPart = GetCustomSkinPart(0, _houseObjects[i]);
				if (!customSkinPart.LoadCache(houseUrls[num]))
				{
					yield return StartCoroutine(customSkinPart.LoadSkin(houseUrls[num]));
				}
			}
			foreach (GameObject groundObject in _groundObjects)
			{
				BaseCustomSkinPart customSkinPart2 = GetCustomSkinPart(1, groundObject);
				if (!customSkinPart2.LoadCache(miscUrls[0]))
				{
					yield return StartCoroutine(customSkinPart2.LoadSkin(miscUrls[0]));
				}
			}
			foreach (GameObject wallObject in _wallObjects)
			{
				BaseCustomSkinPart customSkinPart3 = GetCustomSkinPart(2, wallObject);
				if (!customSkinPart3.LoadCache(miscUrls[1]))
				{
					yield return StartCoroutine(customSkinPart3.LoadSkin(miscUrls[1]));
				}
			}
			foreach (GameObject gateObject in _gateObjects)
			{
				BaseCustomSkinPart customSkinPart4 = GetCustomSkinPart(3, gateObject);
				if (!customSkinPart4.LoadCache(miscUrls[2]))
				{
					yield return StartCoroutine(customSkinPart4.LoadSkin(miscUrls[2]));
				}
			}
			FengGameManagerMKII.instance.unloadAssets();
		}

		protected BaseCustomSkinPart GetCustomSkinPart(int partId, GameObject levelObject)
		{
			List<Renderer> renderers = new List<Renderer>();
			switch ((CityCustomSkinPartId)partId)
			{
			case CityCustomSkinPartId.House:
				AddAllRenderers(renderers, levelObject);
				return new BaseCustomSkinPart(this, renderers, GetRendererId(partId), 2000000);
			case CityCustomSkinPartId.Ground:
				AddAllRenderers(renderers, levelObject);
				return new BaseCustomSkinPart(this, renderers, GetRendererId(partId), 500000);
			case CityCustomSkinPartId.Wall:
				AddAllRenderers(renderers, levelObject);
				return new BaseCustomSkinPart(this, renderers, GetRendererId(partId), 500000);
			case CityCustomSkinPartId.Gate:
				AddAllRenderers(renderers, levelObject);
				return new BaseCustomSkinPart(this, renderers, GetRendererId(partId), 2000000);
			default:
				return null;
			}
		}

		protected override void FindAndIndexLevelObjects()
		{
			_houseObjects.Clear();
			_groundObjects.Clear();
			_wallObjects.Clear();
			_gateObjects.Clear();
			Object[] array = Object.FindObjectsOfType(typeof(GameObject));
			for (int i = 0; i < array.Length; i++)
			{
				GameObject gameObject = (GameObject)array[i];
				string text = gameObject.name;
				if (gameObject != null && text.Contains("Cube_") && gameObject.transform.parent.gameObject.tag != "Player")
				{
					if (text.EndsWith("001"))
					{
						_groundObjects.Add(gameObject);
					}
					else if (text.EndsWith("006") || text.EndsWith("007") || text.EndsWith("015") || text.EndsWith("000"))
					{
						_wallObjects.Add(gameObject);
					}
					else if (text.EndsWith("002") && gameObject.transform.position == Vector3.zero)
					{
						_wallObjects.Add(gameObject);
					}
					else if (text.EndsWith("005") || text.EndsWith("003"))
					{
						_houseObjects.Add(gameObject);
					}
					else if (text.EndsWith("002") && gameObject.transform.position != Vector3.zero)
					{
						_houseObjects.Add(gameObject);
					}
					else if (text.EndsWith("019") || text.EndsWith("020"))
					{
						_gateObjects.Add(gameObject);
					}
				}
			}
		}
	}
}
