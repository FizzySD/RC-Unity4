using Settings;
using UnityEngine;

public class SpectatorMovement : MonoBehaviour
{
	public bool disable;

	private float speed = 100f;

	private void Start()
	{
	}

	private void Update()
	{
		if (!disable)
		{
			float num = speed;
			if (SettingsManager.InputSettings.Human.Jump.GetKey())
			{
				num *= 3f;
			}
			float num2 = (SettingsManager.InputSettings.General.Forward.GetKey() ? 1f : ((!SettingsManager.InputSettings.General.Back.GetKey()) ? 0f : (-1f)));
			float num3 = (SettingsManager.InputSettings.General.Left.GetKey() ? (-1f) : ((!SettingsManager.InputSettings.General.Right.GetKey()) ? 0f : 1f));
			Transform transform = base.transform;
			if (num2 > 0f)
			{
				transform.position += base.transform.forward * num * Time.deltaTime;
			}
			else if (num2 < 0f)
			{
				transform.position -= base.transform.forward * num * Time.deltaTime;
			}
			if (num3 > 0f)
			{
				transform.position += base.transform.right * num * Time.deltaTime;
			}
			else if (num3 < 0f)
			{
				transform.position -= base.transform.right * num * Time.deltaTime;
			}
			if (SettingsManager.InputSettings.Human.HookLeft.GetKey())
			{
				transform.position -= base.transform.up * num * Time.deltaTime;
			}
			else if (SettingsManager.InputSettings.Human.HookRight.GetKey())
			{
				transform.position += base.transform.up * num * Time.deltaTime;
			}
		}
	}
}
