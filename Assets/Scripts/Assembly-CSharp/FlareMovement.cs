using UnityEngine;

public class FlareMovement : MonoBehaviour
{
	public string color;

	private GameObject hero;

	private GameObject hint;

	private bool nohint;

	private Vector3 offY;

	private float timer;

	public void dontShowHint()
	{
		Object.Destroy(hint);
		nohint = true;
	}

	private void Start()
	{
		hero = GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().main_object;
		if (!nohint && hero != null)
		{
			hint = (GameObject)Object.Instantiate(Resources.Load("UI/" + color + "FlareHint"));
			if (color == "Black")
			{
				offY = Vector3.up * 0.4f;
			}
			else
			{
				offY = Vector3.up * 0.5f;
			}
			hint.transform.parent = base.transform.root;
			hint.transform.position = hero.transform.position + offY;
			Vector3 vector = base.transform.position - hint.transform.position;
			float num = Mathf.Atan2(0f - vector.z, vector.x) * 57.29578f;
			hint.transform.rotation = Quaternion.Euler(-90f, num + 180f, 0f);
			hint.transform.localScale = Vector3.zero;
			object[] args = new object[10]
			{
				"x",
				1f,
				"y",
				1f,
				"z",
				1f,
				"easetype",
				iTween.EaseType.easeOutElastic,
				"time",
				1f
			};
			iTween.ScaleTo(hint, iTween.Hash(args));
			object[] args2 = new object[12]
			{
				"x",
				0,
				"y",
				0,
				"z",
				0,
				"easetype",
				iTween.EaseType.easeInBounce,
				"time",
				0.5f,
				"delay",
				2.5f
			};
			iTween.ScaleTo(hint, iTween.Hash(args2));
		}
	}

	private void Update()
	{
		timer += Time.deltaTime;
		if (hint != null)
		{
			if (timer < 3f)
			{
				hint.transform.position = hero.transform.position + offY;
				Vector3 vector = base.transform.position - hint.transform.position;
				float num = Mathf.Atan2(0f - vector.z, vector.x) * 57.29578f;
				hint.transform.rotation = Quaternion.Euler(-90f, num + 180f, 0f);
			}
			else if (hint != null)
			{
				Object.Destroy(hint);
			}
		}
		if (timer < 4f)
		{
			base.rigidbody.AddForce((base.transform.forward + base.transform.up * 5f) * Time.deltaTime * 5f, ForceMode.VelocityChange);
		}
		else
		{
			base.rigidbody.AddForce(-base.transform.up * Time.deltaTime * 7f, ForceMode.Acceleration);
		}
	}
}
