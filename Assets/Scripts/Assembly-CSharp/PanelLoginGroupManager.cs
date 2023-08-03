using UnityEngine;

public class PanelLoginGroupManager : MonoBehaviour
{
	private string _loginName = string.Empty;

	private string _loginPassword = string.Empty;

	public LoginFengKAI logincomponent;

	public GameObject[] panels;

	public PanelGroupManager pgm;

	public string loginName
	{
		set
		{
			_loginName = value;
		}
	}

	public string loginPassword
	{
		set
		{
			_loginPassword = value;
		}
	}

	public void SignIn()
	{
		logincomponent.login(_loginName, _loginPassword);
	}

	private void Start()
	{
		pgm = new PanelGroupManager();
		pgm.panelGroup = panels;
	}

	public void toChangeGuildNamePanel()
	{
		pgm.ActivePanel(4);
	}

	public void toForgetPasswordPanel()
	{
		pgm.ActivePanel(3);
	}

	public void toLoginPanel()
	{
		pgm.ActivePanel(0);
	}

	public void toNewPasswordPanel()
	{
		pgm.ActivePanel(2);
	}

	public void toSignUpPanel()
	{
		pgm.ActivePanel(1);
	}

	public void toStatusPanel()
	{
		pgm.ActivePanel(5);
	}
}
