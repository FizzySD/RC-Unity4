namespace ExitGames.Client.Photon
{
	internal static class SerializationProtocolFactory
	{
		internal static IProtocol Create(SerializationProtocol serializationProtocol)
		{
			if (serializationProtocol == SerializationProtocol.GpBinaryV18)
			{
				return new Protocol18();
			}
			return new Protocol16();
		}
	}
}
