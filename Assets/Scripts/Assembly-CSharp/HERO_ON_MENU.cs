using UnityEngine;

public class HERO_ON_MENU : MonoBehaviour
{
	private Vector3 cameraOffset;

	private Transform cameraPref;

	public int costumeId;

	private Transform head;

	public float headRotationX;

	public float headRotationY;

	private void LateUpdate()
	{
		head.rotation = Quaternion.Euler(head.rotation.eulerAngles.x + headRotationX, head.rotation.eulerAngles.y + headRotationY, head.rotation.eulerAngles.z);
		if (costumeId == 9)
		{
			GameObject.Find("MainCamera_Mono").transform.position = cameraPref.position + cameraOffset;
		}
	}

	private void Start()
	{
		HERO_SETUP component = base.gameObject.GetComponent<HERO_SETUP>();
		HeroCostume.init2();
		component.init();
		component.myCostume = HeroCostume.costume[costumeId];
		component.setCharacterComponent();
		head = base.transform.Find("Amarture/Controller_Body/hip/spine/chest/neck/head");
		cameraPref = base.transform.Find("Amarture/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R");
		if (costumeId == 9)
		{
			cameraOffset = GameObject.Find("MainCamera_Mono").transform.position - cameraPref.position;
		}
		if (component.myCostume.sex == SEX.FEMALE)
		{
			base.animation.Play("stand");
			base.animation["stand"].normalizedTime = Random.Range(0f, 1f);
		}
		else
		{
			base.animation.Play("stand_levi");
			base.animation["stand_levi"].normalizedTime = Random.Range(0f, 1f);
		}
		float speed = 0.5f;
		base.animation["stand"].speed = speed;
		base.animation["stand_levi"].speed = speed;
	}
}
