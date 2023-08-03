using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	internal class EmoteEmojiPopup : EmoteTextPopup
	{
		protected RawImage _emojiImage;

		protected override Vector3 offset
		{
			get
			{
				return Vector3.up * 3f;
			}
		}

		public override void Setup(BasePanel parent = null)
		{
			_emojiImage = base.transform.Find("Panel/Emoji").GetComponent<RawImage>();
			_transform = base.transform;
		}

		protected override void SetEmote(string text)
		{
			_emojiImage.texture = GameMenu.EmojiTextures[text];
		}
	}
}
