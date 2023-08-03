using System.Collections;
using UnityEngine;

public class LoginFengKAI : MonoBehaviour
{
	private string ChangeGuildURL = "http://aotskins.com/version/guild.php";

	private string ChangePasswordURL = "http://fenglee.com/game/aog/change_password.php";

	private string CheckUserURL = "http://aotskins.com/version/login.php";

	private string ForgetPasswordURL = "http://fenglee.com/game/aog/forget_password.php";

	public string formText = string.Empty;

	private string GetInfoURL = "http://aotskins.com/version/getinfo.php";

	public PanelLoginGroupManager loginGroup;

	public GameObject output;

	public GameObject output2;

	public GameObject panelChangeGUILDNAME;

	public GameObject panelChangePassword;

	public GameObject panelForget;

	public GameObject panelLogin;

	public GameObject panelRegister;

	public GameObject panelStatus;

	public static PlayerInfoPHOTON player;

	private static string playerGUILDName = string.Empty;

	private static string playerName = string.Empty;

	private static string playerPassword = string.Empty;

	private string RegisterURL = "http://fenglee.com/game/aog/signup_check.php";

	public void cGuild(string name)
	{
		if (playerName == string.Empty)
		{
			logout();
			NGUITools.SetActive(panelChangeGUILDNAME, false);
			NGUITools.SetActive(panelLogin, true);
			output.GetComponent<UILabel>().text = "Please sign in.";
		}
		else
		{
			StartCoroutine(changeGuild(name));
		}
	}

	private IEnumerator changeGuild(string name)
	{
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("name", playerName);
		wWWForm.AddField("guildname", name);
		WWW wWW = new WWW(ChangeGuildURL, wWWForm);
		yield return wWW;
		if (wWW.error == null)
		{
			output.GetComponent<UILabel>().text = wWW.text;
			if (wWW.text.Contains("Guild name set."))
			{
				NGUITools.SetActive(panelChangeGUILDNAME, false);
				NGUITools.SetActive(panelStatus, true);
				StartCoroutine(getInfo());
			}
			wWW.Dispose();
		}
		else
		{
			MonoBehaviour.print(wWW.error);
		}
	}

	private IEnumerator changePassword(string oldpassword, string password, string password2)
	{
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("userid", playerName);
		wWWForm.AddField("old_password", oldpassword);
		wWWForm.AddField("password", password);
		wWWForm.AddField("password2", password2);
		WWW wWW = new WWW(ChangePasswordURL, wWWForm);
		yield return wWW;
		if (wWW.error == null)
		{
			output.GetComponent<UILabel>().text = wWW.text;
			if (wWW.text.Contains("Thanks, Your password changed successfully"))
			{
				NGUITools.SetActive(panelChangePassword, false);
				NGUITools.SetActive(panelLogin, true);
			}
			wWW.Dispose();
		}
		else
		{
			MonoBehaviour.print(wWW.error);
		}
	}

	private void clearCOOKIE()
	{
		playerName = string.Empty;
		playerPassword = string.Empty;
	}

	public void cpassword(string oldpassword, string password, string password2)
	{
		if (playerName == string.Empty)
		{
			logout();
			NGUITools.SetActive(panelChangePassword, false);
			NGUITools.SetActive(panelLogin, true);
			output.GetComponent<UILabel>().text = "Please sign in.";
		}
		else
		{
			StartCoroutine(changePassword(oldpassword, password, password2));
		}
	}

	private IEnumerator ForgetPassword(string email)
	{
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("email", email);
		WWW wWW = new WWW(ForgetPasswordURL, wWWForm);
		yield return wWW;
		if (wWW.error == null)
		{
			output.GetComponent<UILabel>().text = wWW.text;
			wWW.Dispose();
			NGUITools.SetActive(panelForget, false);
			NGUITools.SetActive(panelLogin, true);
		}
		else
		{
			MonoBehaviour.print(wWW.error);
		}
		clearCOOKIE();
	}

	private IEnumerator getInfo()
	{
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("userid", playerName);
		wWWForm.AddField("password", playerPassword);
		WWW wWW = new WWW(GetInfoURL, wWWForm);
		yield return wWW;
		if (wWW.error == null)
		{
			if (wWW.text.Contains("Error,please sign in again."))
			{
				NGUITools.SetActive(panelLogin, true);
				NGUITools.SetActive(panelStatus, false);
				output.GetComponent<UILabel>().text = wWW.text;
				playerName = string.Empty;
				playerPassword = string.Empty;
			}
			else
			{
				char[] separator = new char[1] { '|' };
				string[] array = wWW.text.Split(separator);
				playerGUILDName = array[0];
				output2.GetComponent<UILabel>().text = array[1];
				player.name = playerName;
				player.guildname = playerGUILDName;
			}
			wWW.Dispose();
		}
		else
		{
			MonoBehaviour.print(wWW.error);
		}
	}

	public void login(string name, string password)
	{
		StartCoroutine(Login(name, password));
	}

	private IEnumerator Login(string name, string password)
	{
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("userid", name);
		wWWForm.AddField("password", password);
		wWWForm.AddField("version", UIMainReferences.Version);
		WWW wWW = new WWW(CheckUserURL, wWWForm);
		yield return wWW;
		clearCOOKIE();
		if (wWW.error == null)
		{
			output.GetComponent<UILabel>().text = wWW.text;
			formText = wWW.text;
			wWW.Dispose();
			if (formText.Contains("Welcome back") && formText.Contains("(^o^)/~"))
			{
				NGUITools.SetActive(panelLogin, false);
				NGUITools.SetActive(panelStatus, true);
				playerName = name;
				playerPassword = password;
				StartCoroutine(getInfo());
			}
		}
		else
		{
			MonoBehaviour.print(wWW.error);
		}
	}

	public void logout()
	{
		clearCOOKIE();
		player = new PlayerInfoPHOTON();
		player.initAsGuest();
		output.GetComponent<UILabel>().text = "Welcome," + player.name;
	}

	private IEnumerator Register(string name, string password, string password2, string email)
	{
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("userid", name);
		wWWForm.AddField("password", password);
		wWWForm.AddField("password2", password2);
		wWWForm.AddField("email", email);
		WWW wWW = new WWW(RegisterURL, wWWForm);
		yield return wWW;
		if (wWW.error == null)
		{
			output.GetComponent<UILabel>().text = wWW.text;
			if (wWW.text.Contains("Final step,to activate your account, please click the link in the activation email"))
			{
				NGUITools.SetActive(panelRegister, false);
				NGUITools.SetActive(panelLogin, true);
			}
			wWW.Dispose();
		}
		else
		{
			MonoBehaviour.print(wWW.error);
		}
		clearCOOKIE();
	}

	public void resetPassword(string email)
	{
		StartCoroutine(ForgetPassword(email));
	}

	public void signup(string name, string password, string password2, string email)
	{
		StartCoroutine(Register(name, password, password2, email));
	}

	private void Start()
	{
		if (player == null)
		{
			player = new PlayerInfoPHOTON();
			player.initAsGuest();
		}
		if (playerName != string.Empty)
		{
			NGUITools.SetActive(panelLogin, false);
			NGUITools.SetActive(panelStatus, true);
			StartCoroutine(getInfo());
		}
		else
		{
			output.GetComponent<UILabel>().text = "Welcome," + player.name;
		}
	}
}
