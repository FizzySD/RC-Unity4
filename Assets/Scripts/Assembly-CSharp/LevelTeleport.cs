using UnityEngine;

public class LevelTeleport : MonoBehaviour
{
	public string levelname = string.Empty;

	public GameObject link;

	private void OnTriggerStay(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			if (levelname != string.Empty)
			{
				Application.LoadLevel(levelname);
			}
			else
			{
				other.gameObject.transform.position = link.transform.position;
			}
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
