public class PhotonMessageInfo
{
	public PhotonView photonView;

	public PhotonPlayer sender;

	private int timeInt;

	public double timestamp
	{
		get
		{
			return (double)timeInt / 1000.0;
		}
	}

	public PhotonMessageInfo()
	{
		sender = PhotonNetwork.player;
		timeInt = (int)(PhotonNetwork.time * 1000.0);
		photonView = null;
	}

	public PhotonMessageInfo(PhotonPlayer player, int timestamp, PhotonView view)
	{
		sender = player;
		timeInt = timestamp;
		photonView = view;
	}

	public override string ToString()
	{
		return string.Format("[PhotonMessageInfo: player='{1}' timestamp={0}]", timestamp, sender);
	}
}
