using UnityEngine;

public class BTN_ChangePassword : MonoBehaviour
{
	public GameObject logincomponent;

	public GameObject oldpassword;

	public GameObject output;

	public GameObject password;

	public GameObject password2;

	private void OnClick()
	{
		if (password.GetComponent<UIInput>().text.Length < 3)
		{
			output.GetComponent<UILabel>().text = "Password too short.";
			return;
		}
		if (password.GetComponent<UIInput>().text != password2.GetComponent<UIInput>().text)
		{
			output.GetComponent<UILabel>().text = "Password does not match the confirm password.";
			return;
		}
		output.GetComponent<UILabel>().text = "please wait...";
		logincomponent.GetComponent<LoginFengKAI>().cpassword(oldpassword.GetComponent<UIInput>().text, password.GetComponent<UIInput>().text, password2.GetComponent<UIInput>().text);
	}
}
