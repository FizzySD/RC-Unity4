using System.Collections.Generic;
using UnityEngine;
using Xft;

namespace CustomSkins
{
	internal class WeaponTrailCustomSkinPart : BaseCustomSkinPart
	{
		private List<XWeaponTrail> _weaponTrails;

		public WeaponTrailCustomSkinPart(BaseCustomSkinLoader loader, List<XWeaponTrail> weaponTrails, string rendererId, int maxSize, Vector2? textureScale = null)
			: base(loader, null, rendererId, maxSize, textureScale)
		{
			_weaponTrails = weaponTrails;
		}

		protected override bool IsValidPart()
		{
			if (_weaponTrails.Count > 0)
			{
				return _weaponTrails[0] != null;
			}
			return false;
		}

		protected override void DisableRenderers()
		{
			foreach (XWeaponTrail weaponTrail in _weaponTrails)
			{
				weaponTrail.enabled = false;
			}
		}

		protected override void SetMaterial(Material material)
		{
			foreach (XWeaponTrail weaponTrail in _weaponTrails)
			{
				weaponTrail.MyMaterial = material;
			}
		}

		protected override Material SetNewTexture(Texture2D texture)
		{
			_weaponTrails[0].MyMaterial.mainTexture = texture;
			if (_textureScale != _defaultTextureScale)
			{
				Vector2 mainTextureScale = _weaponTrails[0].MyMaterial.mainTextureScale;
				_weaponTrails[0].MyMaterial.mainTextureScale = new Vector2(mainTextureScale.x * _textureScale.x, mainTextureScale.y * _textureScale.y);
			}
			SetMaterial(_weaponTrails[0].MyMaterial);
			return _weaponTrails[0].MyMaterial;
		}
	}
}
