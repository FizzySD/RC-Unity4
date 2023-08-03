namespace ExitGames.Client.Photon
{
	public enum ConnectionStateValue : byte
	{
		Disconnected = 0,
		Connecting = 1,
		Connected = 3,
		Disconnecting = 4,
		AcknowledgingDisconnect = 5,
		Zombie = 6
	}
}
