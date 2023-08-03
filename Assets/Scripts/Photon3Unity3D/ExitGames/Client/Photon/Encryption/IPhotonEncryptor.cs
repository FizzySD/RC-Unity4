namespace ExitGames.Client.Photon.Encryption
{
	public interface IPhotonEncryptor
	{
		void Init(byte[] encryptionSecret, byte[] hmacSecret, byte[] ivBytes = null, bool chainingModeGCM = false);

		void Encrypt(byte[] data, int len, byte[] output, ref int offset, bool ivPrefix = true);

		byte[] Decrypt(byte[] data, int offset, int len, out int outLen, bool ivPrefix = true);

		byte[] CreateHMAC(byte[] data, int offset, int count);

		bool CheckHMAC(byte[] data, int len);
	}
}
