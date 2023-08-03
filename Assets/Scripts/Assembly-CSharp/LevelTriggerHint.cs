using Settings;
using UnityEngine;

public class LevelTriggerHint : MonoBehaviour
{
	public string content;

	public HintType myhint;

	private bool on;

	private void OnTriggerStay(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			on = true;
		}
	}

	private void Start()
	{
		if (!LevelInfo.getInfo(FengGameManagerMKII.level).hint)
		{
			base.enabled = false;
		}
		if (content == string.Empty)
		{
			switch (myhint)
			{
			case HintType.MOVE:
			{
				string[] array5 = new string[6]
				{
					"Hello soldier!\nWelcome to Attack On Titan Tribute Game!\n Press [F7D358]",
					SettingsManager.InputSettings.General.Forward.ToString(),
					SettingsManager.InputSettings.General.Left.ToString(),
					SettingsManager.InputSettings.General.Back.ToString(),
					SettingsManager.InputSettings.General.Right.ToString(),
					"[-] to Move."
				};
				content = string.Concat(array5);
				break;
			}
			case HintType.TELE:
				content = "Move to [82FA58]green warp point[-] to proceed.";
				break;
			case HintType.CAMA:
			{
				string[] array4 = new string[5]
				{
					"Press [F7D358]",
					SettingsManager.InputSettings.General.ChangeCamera.ToString(),
					"[-] to change camera mode\nPress [F7D358]",
					SettingsManager.InputSettings.General.HideUI.ToString(),
					"[-] to hide or show the cursor."
				};
				content = string.Concat(array4);
				break;
			}
			case HintType.JUMP:
				content = "Press [F7D358]" + SettingsManager.InputSettings.Human.Jump.ToString() + "[-] to Jump.";
				break;
			case HintType.JUMP2:
				content = "Press [F7D358]" + SettingsManager.InputSettings.General.Forward.ToString() + "[-] towards a wall to perform a wall-run.";
				break;
			case HintType.HOOK:
			{
				string[] array3 = new string[5]
				{
					"Press and Hold[F7D358] ",
					SettingsManager.InputSettings.Human.HookLeft.ToString(),
					"[-] or [F7D358]",
					SettingsManager.InputSettings.Human.HookRight.ToString(),
					"[-] to launch your grapple.\nNow Try hooking to the [>3<] box. "
				};
				content = string.Concat(array3);
				break;
			}
			case HintType.HOOK2:
			{
				string[] array2 = new string[5]
				{
					"Press and Hold[F7D358] ",
					SettingsManager.InputSettings.Human.HookBoth.ToString(),
					"[-] to launch both of your grapples at the same Time.\n\nNow aim between the two black blocks. \nYou will see the mark '<' and '>' appearing on the blocks. \nThen press ",
					SettingsManager.InputSettings.Human.HookBoth.ToString(),
					" to hook the blocks."
				};
				content = string.Concat(array2);
				break;
			}
			case HintType.SUPPLY:
				content = "Press [F7D358]" + SettingsManager.InputSettings.Human.Reload.ToString() + "[-] to reload your blades.\n Move to the supply station to refill your gas and blades.";
				break;
			case HintType.DODGE:
				content = "Press [F7D358]" + SettingsManager.InputSettings.Human.Dodge.ToString() + "[-] to Dodge.";
				break;
			case HintType.ATTACK:
			{
				string[] array = new string[5]
				{
					"Press [F7D358]",
					SettingsManager.InputSettings.Human.AttackDefault.ToString(),
					"[-] to Attack. \nPress [F7D358]",
					SettingsManager.InputSettings.Human.AttackSpecial.ToString(),
					"[-] to use special attack.\n***You can only kill a titan by slashing his [FA5858]NAPE[-].***\n\n"
				};
				content = string.Concat(array);
				break;
			}
			}
		}
	}

	private void Update()
	{
		if (on)
		{
			GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().ShowHUDInfoCenter(content + "\n\n\n\n\n");
			on = false;
		}
	}
}
