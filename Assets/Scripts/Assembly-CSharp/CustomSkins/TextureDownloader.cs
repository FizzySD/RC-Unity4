using System;
using System.Collections;
using UnityEngine;
using Utility;

namespace CustomSkins
{
	internal class TextureDownloader
	{
		private static readonly string[] ValidHosts = new string[21]
		{
			"i.imgur.com/", "imgur.com/", "image.ibb.co/", "i.reddit.it/", "cdn.discordapp.com/attachments/", "media.discordapp.net/attachments/", "images-ext-2.discordapp.net/external/", "i.reddit.it/", "gyazo.com/", "puu.sh/",
			"i.postimg.cc/", "postimg./", "deviantart.com/", "photobucket.com/", "aotcorehome.files.wordpress.com/", "s1.ax1x.com/", "s27.postimg.io/", "1.bp.blogspot.com/", "tiebapic.baidu.com/", "s25.postimg.gg/",
			"aotcorehome.files.wordpress.com/"
		};

		private static readonly string[] ValidFileEndings = new string[3] { ".jpg", ".png", ".jpeg" };

		private static readonly string[] URLPrefixes = new string[3] { "https://", "http://", "www." };

		private const int MaxConcurrentDownloads = 1;

		private static int CurrentConcurrentDownloads = 0;

		public static void ResetConcurrentDownloads()
		{
			CurrentConcurrentDownloads = 0;
		}

		public static bool ValidTextureURL(string url)
		{
			url = url.ToLower();
			if (url == string.Empty)
			{
				return false;
			}
			if (url == BaseCustomSkinLoader.TransparentURL)
			{
				return true;
			}
			if (CheckFileEnding(url))
			{
				return CheckValidHost(url);
			}
			return false;
		}

		private static bool CheckFileEnding(string url)
		{
			string[] validFileEndings = ValidFileEndings;
			foreach (string value in validFileEndings)
			{
				if (url.EndsWith(value))
				{
					return true;
				}
			}
			return false;
		}

		private static bool CheckValidHost(string url)
		{
			if (url.StartsWith("file://"))
			{
				return true;
			}
			string[] uRLPrefixes = URLPrefixes;
			foreach (string text in uRLPrefixes)
			{
				if (url.StartsWith(text))
				{
					url = url.Remove(0, text.Length);
				}
			}
			string[] validHosts = ValidHosts;
			foreach (string value in validHosts)
			{
				if (url.StartsWith(value))
				{
					return true;
				}
			}
			return false;
		}

		public static IEnumerator DownloadTexture(BaseCustomSkinLoader obj, string url, bool mipmap, int maxSize)
		{
			Texture2D blankTexture = CreateBlankTexture(mipmap);
			yield return blankTexture;
			if (!ValidTextureURL(url))
			{
				yield break;
			}
			while (!CanStartTextureDownload())
			{
				yield return blankTexture;
			}
			OnStartTextureDownload();
			using (WWW www = new WWW(url))
			{
				yield return www;
				if (www.error != null || www.bytesDownloaded > maxSize)
				{
					OnStopTextureDownload();
					yield return blankTexture;
					yield break;
				}
				OnStopTextureDownload();
				CoroutineWithData cwd = new CoroutineWithData(obj, CreateTextureFromData(obj, www, mipmap));
				yield return cwd.Coroutine;
				yield return cwd.Result;
			}
		}

		private static bool CanStartTextureDownload()
		{
			return CurrentConcurrentDownloads < 1;
		}

		private static void OnStartTextureDownload()
		{
			CurrentConcurrentDownloads++;
			CurrentConcurrentDownloads = Math.Min(CurrentConcurrentDownloads, 1);
		}

		private static void OnStopTextureDownload()
		{
			CurrentConcurrentDownloads--;
			CurrentConcurrentDownloads = Math.Max(CurrentConcurrentDownloads, 0);
		}

		private static bool IsPowerOfTwo(int num)
		{
			if (num >= 4)
			{
				return (num & (num - 1)) == 0;
			}
			return false;
		}

		private static int GetClosestPowerOfTwo(int num)
		{
			int num2 = 4;
			num = Math.Min(num, 2047);
			while (num2 < num)
			{
				num2 *= 2;
			}
			return num2;
		}

		private static Texture2D CreateBlankTexture(bool mipmap, bool compressed = false)
		{
			if (compressed)
			{
				return new Texture2D(4, 4, TextureFormat.DXT5, mipmap);
			}
			return new Texture2D(4, 4, TextureFormat.RGBA32, mipmap);
		}

		private static Texture2D DecodeTexture(WWW www, bool mipmap)
		{
			Texture2D texture2D = CreateBlankTexture(mipmap);
			try
			{
				texture2D.LoadImage(www.bytes);
				return texture2D;
			}
			catch
			{
				texture2D = CreateBlankTexture(false);
				texture2D.LoadImage(www.bytes);
				return texture2D;
			}
		}

		private static IEnumerator CreateTextureFromData(BaseCustomSkinLoader obj, WWW www, bool mipmap)
		{
			int resizedSize = 0;
			Texture2D texture = DecodeTexture(www, mipmap);
			yield return obj.StartCoroutine(Util.WaitForFrames(2));
			int width = texture.width;
			int height = texture.height;
			if (!IsPowerOfTwo(width))
			{
				resizedSize = GetClosestPowerOfTwo(width);
			}
			else if (!IsPowerOfTwo(height))
			{
				resizedSize = GetClosestPowerOfTwo(height);
			}
			if (resizedSize == 0)
			{
				texture.Compress(true);
				yield return obj.StartCoroutine(Util.WaitForFrames(2));
				yield return texture;
			}
			else
			{
				yield return obj.StartCoroutine(TextureScaler.Scale(texture, resizedSize, resizedSize));
				yield return obj.StartCoroutine(Util.WaitForFrames(2));
				texture.Compress(true);
				yield return obj.StartCoroutine(Util.WaitForFrames(2));
				yield return texture;
			}
		}
	}
}
