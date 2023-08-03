namespace ExitGames.Client.Photon
{
	public enum StatusCode
	{
		Connect = 1024,
		Disconnect = 1025,
		Exception = 1026,
		ExceptionOnConnect = 1023,
		SecurityExceptionOnConnect = 1022,
		SendError = 1030,
		ExceptionOnReceive = 1039,
		TimeoutDisconnect = 1040,
		DisconnectByServerTimeout = 1041,
		DisconnectByServerUserLimit = 1042,
		DisconnectByServerLogic = 1043,
		DisconnectByServerReasonUnknown = 1044,
		EncryptionEstablished = 1048,
		EncryptionFailedToEstablish = 1049
	}
}
