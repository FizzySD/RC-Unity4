using Settings;
using UnityEngine;

public class TITAN_CONTROLLER : MonoBehaviour
{
	public bool bite;

	public bool bitel;

	public bool biter;

	public bool chopl;

	public bool chopr;

	public bool choptl;

	public bool choptr;

	public bool cover;

	public Camera currentCamera;

	public float currentDirection;

	public bool grabbackl;

	public bool grabbackr;

	public bool grabfrontl;

	public bool grabfrontr;

	public bool grabnapel;

	public bool grabnaper;

	public bool isAttackDown;

	public bool isAttackIIDown;

	public bool isHorse;

	public bool isJumpDown;

	public bool isSuicide;

	public bool isWALKDown;

	public bool sit;

	public float targetDirection;

	private void Start()
	{
		currentCamera = GameObject.Find("MainCamera").GetComponent<Camera>();
		if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
		{
			base.enabled = false;
		}
	}

	private void Update()
	{
		int num;
		int num2;
		float num5;
		if (isHorse)
		{
			num = (SettingsManager.InputSettings.General.Forward.GetKey() ? 1 : (SettingsManager.InputSettings.General.Back.GetKey() ? (-1) : 0));
			num2 = (SettingsManager.InputSettings.General.Left.GetKey() ? (-1) : (SettingsManager.InputSettings.General.Right.GetKey() ? 1 : 0));
			if (num2 != 0 || num != 0)
			{
				float y = currentCamera.transform.rotation.eulerAngles.y;
				float num3 = Mathf.Atan2(num, num2) * 57.29578f;
				num3 = 0f - num3 + 90f;
				float num4 = y + num3;
				targetDirection = num4;
			}
			else
			{
				targetDirection = -874f;
			}
			isAttackDown = false;
			isAttackIIDown = false;
			if (targetDirection != -874f)
			{
				currentDirection = targetDirection;
			}
			num5 = currentCamera.transform.rotation.eulerAngles.y - currentDirection;
			if (num5 >= 180f)
			{
				num5 -= 360f;
			}
			if (SettingsManager.InputSettings.Human.HorseJump.GetKey())
			{
				isAttackDown = true;
			}
			isWALKDown = SettingsManager.InputSettings.Human.HorseWalk.GetKey();
			return;
		}
		num = (SettingsManager.InputSettings.General.Forward.GetKey() ? 1 : (SettingsManager.InputSettings.General.Back.GetKey() ? (-1) : 0));
		num2 = (SettingsManager.InputSettings.General.Left.GetKey() ? (-1) : (SettingsManager.InputSettings.General.Right.GetKey() ? 1 : 0));
		if (num2 != 0 || num != 0)
		{
			float y = currentCamera.transform.rotation.eulerAngles.y;
			float num3 = Mathf.Atan2(num, num2) * 57.29578f;
			num3 = 0f - num3 + 90f;
			float num4 = y + num3;
			targetDirection = num4;
		}
		else
		{
			targetDirection = -874f;
		}
		isAttackDown = false;
		isJumpDown = false;
		isAttackIIDown = false;
		isSuicide = false;
		grabbackl = false;
		grabbackr = false;
		grabfrontl = false;
		grabfrontr = false;
		grabnapel = false;
		grabnaper = false;
		choptl = false;
		chopr = false;
		chopl = false;
		choptr = false;
		bite = false;
		bitel = false;
		biter = false;
		cover = false;
		sit = false;
		if (targetDirection != -874f)
		{
			currentDirection = targetDirection;
		}
		num5 = currentCamera.transform.rotation.eulerAngles.y - currentDirection;
		if (num5 >= 180f)
		{
			num5 -= 360f;
		}
		if (SettingsManager.InputSettings.Titan.AttackPunch.GetKey())
		{
			isAttackDown = true;
		}
		if (SettingsManager.InputSettings.Titan.AttackSlam.GetKey())
		{
			isAttackIIDown = true;
		}
		if (SettingsManager.InputSettings.Titan.Jump.GetKey())
		{
			isJumpDown = true;
		}
		if (SettingsManager.InputSettings.General.ChangeCharacter.GetKey())
		{
			isSuicide = true;
		}
		if (SettingsManager.InputSettings.Titan.CoverNape.GetKey())
		{
			cover = true;
		}
		if (SettingsManager.InputSettings.Titan.Sit.GetKey())
		{
			sit = true;
		}
		if (SettingsManager.InputSettings.Titan.AttackGrabFront.GetKey() && num5 >= 0f)
		{
			grabfrontr = true;
		}
		if (SettingsManager.InputSettings.Titan.AttackGrabFront.GetKey() && num5 < 0f)
		{
			grabfrontl = true;
		}
		if (SettingsManager.InputSettings.Titan.AttackGrabBack.GetKey() && num5 >= 0f)
		{
			grabbackr = true;
		}
		if (SettingsManager.InputSettings.Titan.AttackGrabBack.GetKey() && num5 < 0f)
		{
			grabbackl = true;
		}
		if (SettingsManager.InputSettings.Titan.AttackGrabNape.GetKey() && num5 >= 0f)
		{
			grabnaper = true;
		}
		if (SettingsManager.InputSettings.Titan.AttackGrabNape.GetKey() && num5 < 0f)
		{
			grabnapel = true;
		}
		if (SettingsManager.InputSettings.Titan.AttackSlap.GetKey() && num5 >= 0f)
		{
			choptr = true;
		}
		if (SettingsManager.InputSettings.Titan.AttackSlap.GetKey() && num5 < 0f)
		{
			choptl = true;
		}
		if (SettingsManager.InputSettings.Titan.AttackBite.GetKey() && num5 > 7.5f)
		{
			biter = true;
		}
		if (SettingsManager.InputSettings.Titan.AttackBite.GetKey() && num5 < -7.5f)
		{
			bitel = true;
		}
		if (SettingsManager.InputSettings.Titan.AttackBite.GetKey() && num5 >= -7.5f && num5 <= 7.5f)
		{
			bite = true;
		}
		isWALKDown = SettingsManager.InputSettings.Titan.Walk.GetKey();
	}
}
