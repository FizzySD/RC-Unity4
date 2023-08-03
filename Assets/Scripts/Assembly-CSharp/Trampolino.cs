using Photon;
using UnityEngine;

public class Trampolino : Photon.MonoBehaviour
{
	private int a;

	private void OnCollisionEnter(Collision collision)
	{
		if (a < 120)
		{
			if (base.collider.gameObject.name == "Titan" || base.collider.gameObject.name == "Aberrant")
			{
				collision.gameObject.transform.position = new Vector3(base.gameObject.transform.position.x, base.gameObject.transform.position.y + 1000f, base.gameObject.transform.position.z);
			}
			a++;
		}
	}
}
