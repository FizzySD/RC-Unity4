using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CustomSkins
{
	internal class ForestCustomSkinLoader : LevelCustomSkinLoader
	{
		private List<GameObject> _treeObjects = new List<GameObject>();

		private List<GameObject> _groundObjects = new List<GameObject>();

		protected override string RendererIdPrefix
		{
			get
			{
				return "forest";
			}
		}

		public override IEnumerator LoadSkinsFromRPC(object[] data)
		{
			FindAndIndexLevelObjects();
			char[] randomIndices = ((string)data[0]).ToCharArray();
			int[] trunkRandomIndices = SplitRandomIndices(randomIndices, 0);
			int[] leafRandomIndices = SplitRandomIndices(randomIndices, 1);
			string[] trunkUrls = ((string)data[1]).Split(',');
			string[] leafUrls = ((string)data[2]).Split(',');
			string groundUrl = leafUrls[8];
			for (int i = 0; i < _treeObjects.Count; i++)
			{
				int num = trunkRandomIndices[i];
				int num2 = leafRandomIndices[i];
				string url = trunkUrls[num];
				string leafUrl = leafUrls[num2];
				BaseCustomSkinPart customSkinPart = GetCustomSkinPart(0, _treeObjects[i]);
				BaseCustomSkinPart leafPart = GetCustomSkinPart(1, _treeObjects[i]);
				if (!customSkinPart.LoadCache(url))
				{
					yield return StartCoroutine(customSkinPart.LoadSkin(url));
				}
				if (!leafPart.LoadCache(leafUrl))
				{
					yield return StartCoroutine(leafPart.LoadSkin(leafUrl));
				}
			}
			foreach (GameObject groundObject in _groundObjects)
			{
				BaseCustomSkinPart customSkinPart2 = GetCustomSkinPart(2, groundObject);
				if (!customSkinPart2.LoadCache(groundUrl))
				{
					yield return StartCoroutine(customSkinPart2.LoadSkin(groundUrl));
				}
			}
			FengGameManagerMKII.instance.unloadAssets();
		}

		protected BaseCustomSkinPart GetCustomSkinPart(int partId, GameObject levelObject)
		{
			List<Renderer> renderers = new List<Renderer>();
			switch ((ForestCustomSkinPartId)partId)
			{
			case ForestCustomSkinPartId.TreeTrunk:
				AddRenderersContainingName(renderers, levelObject, "Cube");
				return new BaseCustomSkinPart(this, renderers, GetRendererId(partId), 2000000);
			case ForestCustomSkinPartId.TreeLeaf:
				AddRenderersContainingName(renderers, levelObject, "Plane_031");
				return new BaseCustomSkinPart(this, renderers, GetRendererId(partId), 500000);
			case ForestCustomSkinPartId.Ground:
				AddAllRenderers(renderers, levelObject);
				return new BaseCustomSkinPart(this, renderers, GetRendererId(partId), 500000);
			default:
				return null;
			}
		}

		protected override void FindAndIndexLevelObjects()
		{
			_treeObjects.Clear();
			_groundObjects.Clear();
			Object[] array = Object.FindObjectsOfType(typeof(GameObject));
			for (int i = 0; i < array.Length; i++)
			{
				GameObject gameObject = (GameObject)array[i];
				if (gameObject != null)
				{
					if (gameObject.name.Contains("TREE"))
					{
						_treeObjects.Add(gameObject);
					}
					else if (gameObject.name.Contains("Cube_001") && gameObject.transform.parent.gameObject.tag != "Player")
					{
						_groundObjects.Add(gameObject);
					}
				}
			}
		}

		private int[] SplitRandomIndices(char[] randomIndices, int offset)
		{
			List<int> list = new List<int>();
			for (int i = offset; i < randomIndices.Length; i += 2)
			{
				if (i < randomIndices.Length)
				{
					list.Add(int.Parse(randomIndices[i].ToString()));
				}
			}
			return Enumerable.ToArray(list);
		}
	}
}
