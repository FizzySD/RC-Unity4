using System;

public class AuthenticationValues
{
	public string AuthParameters;

	public CustomAuthenticationType AuthType;

	public string Secret;

	public object AuthPostData { get; private set; }

	public virtual void SetAuthParameters(string user, string token)
	{
		AuthParameters = "username=" + Uri.EscapeDataString(user) + "&token=" + Uri.EscapeDataString(token);
	}

	public virtual void SetAuthPostData(string stringData)
	{
		AuthPostData = ((!string.IsNullOrEmpty(stringData)) ? stringData : null);
	}

	public virtual void SetAuthPostData(byte[] byteData)
	{
		AuthPostData = byteData;
	}

	public override string ToString()
	{
		return AuthParameters + " s: " + Secret;
	}
}
