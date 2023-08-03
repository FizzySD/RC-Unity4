using System;
using UnityEngine;

namespace Utility
{
	internal class SingletonFactory : MonoBehaviour
	{
		public static T CreateSingleton<T>(T instance) where T : Component
		{
			if ((UnityEngine.Object)instance != (UnityEngine.Object)null)
			{
				Type typeFromHandle = typeof(T);
				throw new Exception(string.Format("Attempting to create duplicate singleton of {0}", typeFromHandle.Name));
			}
			GameObject gameObject = new GameObject();
			instance = gameObject.AddComponent<T>();
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			return instance;
		}
	}
}
