using Constants;
using UnityEngine;

public class TitanTrigger : MonoBehaviour
{
	public bool isCollide;

	private void OnTriggerEnter(Collider other)
	{
		if (isCollide)
		{
			return;
		}
		GameObject gameObject = other.transform.root.gameObject;
		if (gameObject.layer != PhysicsLayer.Players)
		{
			return;
		}
		if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER)
		{
			if (gameObject.GetPhotonView().isMine)
			{
				isCollide = true;
			}
		}
		else if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
		{
			GameObject main_object = Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().main_object;
			if (main_object != null && main_object == gameObject)
			{
				isCollide = true;
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (!isCollide)
		{
			return;
		}
		GameObject gameObject = other.transform.root.gameObject;
		if (gameObject.layer != PhysicsLayer.Players)
		{
			return;
		}
		if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER)
		{
			if (gameObject.GetPhotonView().isMine)
			{
				isCollide = false;
			}
		}
		else if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
		{
			GameObject main_object = Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().main_object;
			if (main_object != null && main_object == gameObject)
			{
				isCollide = false;
			}
		}
	}
}
