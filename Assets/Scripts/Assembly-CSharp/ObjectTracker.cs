using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ObjectTracker : MonoBehaviour
{
	private static ObjectTracker instance;

	private Dictionary<GameObject, List<Component>> instantiatedObjects = new Dictionary<GameObject, List<Component>>();

	public static ObjectTracker Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake()
	{
		if (instance != null && instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			instance = this;
		}
	}

	private void OnDestroy()
	{
		instance = null;
	}

	public void AddTrackedObject(GameObject gameObject)
	{
		if (!instantiatedObjects.ContainsKey(gameObject))
		{
			instantiatedObjects.Add(gameObject, new List<Component>());
			Component[] components = gameObject.GetComponents<Component>();
			Component[] array = components;
			foreach (Component item in array)
			{
				instantiatedObjects[gameObject].Add(item);
			}
		}
	}

	public void RemoveTrackedObject(GameObject gameObject)
	{
		instantiatedObjects.Remove(gameObject);
	}


	public List<Component> GetComponentsForGameObject(GameObject gameObject)
	{
		if (instantiatedObjects.ContainsKey(gameObject))
		{
			return instantiatedObjects[gameObject];
		}
		return null;
	}
}
