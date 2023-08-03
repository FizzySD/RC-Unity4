public class FriendInfo
{
	public bool IsInRoom
	{
		get
		{
			if (IsOnline)
			{
				return !string.IsNullOrEmpty(Room);
			}
			return false;
		}
	}

	public bool IsOnline { get; protected internal set; }

	public string Name { get; protected internal set; }

	public string Room { get; protected internal set; }

	public override string ToString()
	{
		return string.Format("{0}\t is: {1}", Name, (!IsOnline) ? "offline" : ((!IsInRoom) ? "on master" : "playing"));
	}
}
