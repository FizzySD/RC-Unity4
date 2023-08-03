public class TypedLobby
{
	public static readonly TypedLobby Default = new TypedLobby();

	public string Name;

	public LobbyType Type;

	public bool IsDefault
	{
		get
		{
			if (Type == LobbyType.Default)
			{
				return string.IsNullOrEmpty(Name);
			}
			return false;
		}
	}

	public TypedLobby()
	{
		Name = string.Empty;
		Type = LobbyType.Default;
	}

	public TypedLobby(string name, LobbyType type)
	{
		Name = name;
		Type = type;
	}

	public override string ToString()
	{
		return string.Format("Lobby '{0}'[{1}]", Name, Type);
	}
}
