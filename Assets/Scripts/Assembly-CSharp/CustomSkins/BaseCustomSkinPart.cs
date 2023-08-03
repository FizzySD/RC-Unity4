using System.Collections;
using System.Collections.Generic;
using Settings;
using UnityEngine;
using Utility;

namespace CustomSkins
{
	internal class BaseCustomSkinPart
	{
		protected BaseCustomSkinLoader _loader;

		protected List<Renderer> _renderers;

		protected string _rendererId;

		protected int _maxSize;

		protected Vector2 _textureScale;

		protected readonly Vector2 _defaultTextureScale = new Vector2(1f, 1f);

		protected bool _useTransparentMaterial;

		public BaseCustomSkinPart(BaseCustomSkinLoader loader, List<Renderer> renderers, string rendererId, int maxSize, Vector2? textureScale = null, bool useTransparentMaterial = false)
		{
			_loader = loader;
			_renderers = renderers;
			_rendererId = rendererId;
			_maxSize = maxSize;
			if (!textureScale.HasValue)
			{
				_textureScale = _defaultTextureScale;
			}
			else
			{
				_textureScale = textureScale.Value;
			}
			_useTransparentMaterial = useTransparentMaterial;
		}

		public bool LoadCache(string url)
		{
			if (url.ToLower() == BaseCustomSkinLoader.TransparentURL)
			{
				DisableRenderers();
				return true;
			}
			if (!IsValidPart() || !TextureDownloader.ValidTextureURL(url))
			{
				return true;
			}
			if (MaterialCache.ContainsKey(_rendererId, url))
			{
				SetMaterial(MaterialCache.GetMaterial(_rendererId, url));
				return true;
			}
			return false;
		}

		public IEnumerator LoadSkin(string url)
		{
			url = url.Trim();
			if (IsValidPart() && TextureDownloader.ValidTextureURL(url))
			{
				bool value = SettingsManager.GraphicsSettings.MipmapEnabled.Value;
				CoroutineWithData cwd = new CoroutineWithData(_loader, TextureDownloader.DownloadTexture(_loader, url, value, _maxSize));
				yield return cwd.Coroutine;
				if (IsValidPart())
				{
					Material material = SetNewTexture((Texture2D)cwd.Result);
					MaterialCache.SetMaterial(_rendererId, url, material);
				}
			}
		}

		protected virtual bool IsValidPart()
		{
			if (_renderers.Count > 0)
			{
				return _renderers[0] != null;
			}
			return false;
		}

		protected virtual void DisableRenderers()
		{
			if (_useTransparentMaterial)
			{
				SetMaterial(MaterialCache.TransparentMaterial);
				return;
			}
			foreach (Renderer renderer in _renderers)
			{
				renderer.enabled = false;
			}
		}

		protected virtual void SetMaterial(Material material)
		{
			foreach (Renderer renderer in _renderers)
			{
				renderer.material = material;
			}
		}

		protected virtual Material SetNewTexture(Texture2D texture)
		{
			_renderers[0].material.mainTexture = texture;
			if (_textureScale != _defaultTextureScale)
			{
				Vector2 mainTextureScale = _renderers[0].material.mainTextureScale;
				_renderers[0].material.mainTextureScale = new Vector2(mainTextureScale.x * _textureScale.x, mainTextureScale.y * _textureScale.y);
				_renderers[0].material.mainTextureOffset = new Vector2(0f, 0f);
			}
			SetMaterial(_renderers[0].material);
			return _renderers[0].material;
		}
	}
}
