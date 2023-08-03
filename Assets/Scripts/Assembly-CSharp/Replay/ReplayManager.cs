using UnityEngine;
using Utility;

namespace Replay
{
	internal class ReplayManager : MonoBehaviour
	{
		private static ReplayManager _instance;

		public static void Init()
		{
			_instance = SingletonFactory.CreateSingleton(_instance);
		}
	}
}
