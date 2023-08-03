using System.Collections.Generic;
using UI;
using UnityEngine;
using Weather;

namespace GameManagers
{
	internal class CustomRPCManager : MonoBehaviour
	{
		public static PhotonView PhotonView;

		[RPC]
		public void SetWeatherRPC(string currentWeatherJson, string startWeatherJson, string targetWeatherJson, Dictionary<int, float> targetWeatherStartTimes, Dictionary<int, float> targetWeatherEndTimes, float currentTime, PhotonMessageInfo info)
		{
			WeatherManager.OnSetWeatherRPC(currentWeatherJson, startWeatherJson, targetWeatherJson, targetWeatherStartTimes, targetWeatherEndTimes, currentTime, info);
		}

		[RPC]
		public void EmoteEmojiRPC(int viewId, string emoji, PhotonMessageInfo info)
		{
			GameMenu.OnEmoteEmojiRPC(viewId, emoji, info);
		}

		[RPC]
		public void EmoteTextRPC(int viewId, string text, PhotonMessageInfo info)
		{
			GameMenu.OnEmoteTextRPC(viewId, text, info);
		}

		private void Awake()
		{
			PhotonView = GetComponent<PhotonView>();
		}
	}
}
