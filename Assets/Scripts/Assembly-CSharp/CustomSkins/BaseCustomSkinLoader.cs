using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CustomSkins
{
	internal abstract class BaseCustomSkinLoader : MonoBehaviour
	{
		public static readonly string TransparentURL = "transparent";

		protected GameObject _owner;

		protected const int BytesPerKb = 1000;

		protected const int MaxSizeLarge = 2000000;

		protected const int MaxSizeMedium = 1000000;

		protected const int MaxSizeSmall = 500000;

		protected abstract string RendererIdPrefix { get; }

		protected void Awake()
		{
			_owner = base.gameObject;
		}

		protected virtual BaseCustomSkinPart GetCustomSkinPart(int partId)
		{
			throw new NotImplementedException();
		}

		public abstract IEnumerator LoadSkinsFromRPC(object[] data);

		protected string GetRendererId(int partId)
		{
			return RendererIdPrefix + partId;
		}

		protected void AddRendererIfExists(List<Renderer> renderers, GameObject obj)
		{
			if (obj != null)
			{
				renderers.Add(obj.renderer);
			}
		}

		protected void AddAllRenderers(List<Renderer> renderers, GameObject obj)
		{
			Renderer[] componentsInChildren = obj.GetComponentsInChildren<Renderer>();
			foreach (Renderer item in componentsInChildren)
			{
				renderers.Add(item);
			}
		}

		protected void AddRenderersContainingName(List<Renderer> renderers, GameObject obj, string name)
		{
			Renderer[] componentsInChildren = obj.GetComponentsInChildren<Renderer>();
			foreach (Renderer renderer in componentsInChildren)
			{
				if (renderer.name.Contains(name))
				{
					renderers.Add(renderer);
				}
			}
		}

		protected void AddRenderersMatchingName(List<Renderer> renderers, GameObject obj, string name)
		{
			Renderer[] componentsInChildren = obj.GetComponentsInChildren<Renderer>();
			foreach (Renderer renderer in componentsInChildren)
			{
				if (renderer.name == name)
				{
					renderers.Add(renderer);
				}
			}
		}

		protected List<int> GetCustomSkinPartIds(Type t)
		{
			return Enum.GetValues(t).Cast<int>().ToList();
		}

		private void OnDestroy()
		{
			TextureDownloader.ResetConcurrentDownloads();
		}
	}
}
