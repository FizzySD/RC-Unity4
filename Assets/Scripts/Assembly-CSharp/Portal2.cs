using Photon;
using UnityEngine;

public class Portal2 : Photon.MonoBehaviour
{
	public Vector3 teleportoffset = new Vector3(3f, 0f, 0f);

	public void OnCollisionEnter()
	{
		GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().main_object.GetComponent<HERO>().teleport(InRoomChat.Portal1GO.transform.position + teleportoffset);
	}

	private void teleport(PhotonPlayer player)
	{
		Vector3 position = default(Vector3);
		GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
		foreach (GameObject gameObject in array)
		{
			if (gameObject.GetComponent<HERO>() != null && gameObject.GetComponent<HERO>().photonView.owner == player)
			{
				position = gameObject.GetComponent<HERO>().transform.position;
				position.y += 2f;
				break;
			}
		}
		HERO component = GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().main_object.GetComponent<HERO>();
		if (component != null)
		{
			component.teleport(position);
		}
	}
}
