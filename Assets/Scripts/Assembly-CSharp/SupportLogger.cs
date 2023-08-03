using UnityEngine;

public class SupportLogger : MonoBehaviour
{
	public bool LogTrafficStats = true;

	public void Start()
	{
		if (GameObject.Find("PunSupportLogger") == null)
		{
			GameObject gameObject = new GameObject("PunSupportLogger");
			Object.DontDestroyOnLoad(gameObject);
			gameObject.AddComponent<SupportLogging>().LogTrafficStats = LogTrafficStats;
		}
	}
}
