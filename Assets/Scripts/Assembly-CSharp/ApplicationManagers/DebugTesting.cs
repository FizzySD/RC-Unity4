using UnityEngine;
using Utility;

namespace ApplicationManagers
{
	internal class DebugTesting : MonoBehaviour
	{
		private static DebugTesting _instance;

		public static void Init()
		{
			_instance = SingletonFactory.CreateSingleton(_instance);
		}

		public static void RunTests()
		{
			bool developmentMode = ApplicationConfig.DevelopmentMode;
		}

		public static void Log(object message)
		{
			Debug.Log(message);
		}

		private void Update()
		{
		}

		public static void RunDebugCommand(string command)
		{
			if (!ApplicationConfig.DevelopmentMode)
			{
				Debug.Log("Debug commands are not available in release mode.");
				return;
			}
			string[] array = command.Split(' ');
			string text = array[0];
			if (text == "spawnasset")
			{
				string text2 = array[1];
				string[] array2 = array[2].Split(',');
				Vector3 position = new Vector3(float.Parse(array2[0]), float.Parse(array2[1]), float.Parse(array2[2]));
				string[] array3 = array[3].Split(',');
				Object.Instantiate(rotation: new Quaternion(float.Parse(array3[0]), float.Parse(array3[1]), float.Parse(array3[2]), float.Parse(array3[3])), original: FengGameManagerMKII.RCassets.Load(text2), position: position);
			}
			else
			{
				Debug.Log("Invalid debug command.");
			}
		}
	}
}
