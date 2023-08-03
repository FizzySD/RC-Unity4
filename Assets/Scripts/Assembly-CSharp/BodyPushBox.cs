using UnityEngine;

public class BodyPushBox : MonoBehaviour
{
	public GameObject parent;

	private void OnTriggerStay(Collider other)
	{
		if (!(other.gameObject.tag == "bodyCollider"))
		{
			return;
		}
		BodyPushBox component = other.gameObject.GetComponent<BodyPushBox>();
		if (component != null && component.parent != null)
		{
			Vector3 vector = component.parent.transform.position - parent.transform.position;
			float radius = base.gameObject.GetComponent<CapsuleCollider>().radius;
			float radius2 = base.gameObject.GetComponent<CapsuleCollider>().radius;
			vector.y = 0f;
			if (vector.magnitude > 0f)
			{
				float num = radius + radius2 - vector.magnitude;
				vector.Normalize();
			}
			else
			{
				float num = radius + radius2;
				vector.x = 1f;
			}
			float num2 = 0.1f;
		}
	}
}
