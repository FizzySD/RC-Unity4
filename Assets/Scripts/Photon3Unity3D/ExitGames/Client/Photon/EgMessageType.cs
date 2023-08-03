namespace ExitGames.Client.Photon
{
	internal enum EgMessageType : byte
	{
		Init = 0,
		InitResponse = 1,
		Operation = 2,
		OperationResponse = 3,
		Event = 4,
		InternalOperationRequest = 6,
		InternalOperationResponse = 7,
		Message = 8,
		RawMessage = 9
	}
}
