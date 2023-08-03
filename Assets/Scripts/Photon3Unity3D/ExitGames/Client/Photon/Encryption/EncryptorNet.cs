using System;
using System.IO;
using System.Security.Cryptography;

namespace ExitGames.Client.Photon.Encryption
{
	public class EncryptorNet : IPhotonEncryptor
	{
		public const int BLOCK_SIZE = 16;

		public const int IV_SIZE = 16;

		public const int HMAC_SIZE = 32;

		protected Aes encryptorIn;

		protected Aes encryptorOut;

		protected HMACSHA256 hmacsha256In;

		protected HMACSHA256 hmacsha256Out;

		private readonly byte[] reusedIvBuffer = new byte[16];

		private readonly byte[] reusedReadBuffer = new byte[16];

		public void Init(byte[] encryptionSecret, byte[] hmacSecret, byte[] ivBytes = null, bool chainingModeGCM = false)
		{
			encryptorIn = new AesManaged
			{
				Key = encryptionSecret
			};
			encryptorOut = new AesManaged
			{
				Key = encryptionSecret
			};
			hmacsha256In = new HMACSHA256(hmacSecret);
			hmacsha256Out = new HMACSHA256(hmacSecret);
		}

		public void Encrypt(byte[] data, int len, byte[] output, ref int offset, bool ivPrefix = true)
		{
			if (ivPrefix)
			{
				encryptorOut.GenerateIV();
			}
			using (ICryptoTransform transform = encryptorOut.CreateEncryptor())
			{
				using (MemoryStream memoryStream = new MemoryStream(output, offset, output.Length - offset))
				{
					if (ivPrefix)
					{
						byte[] iV = encryptorOut.IV;
						memoryStream.Write(iV, 0, iV.Length);
					}
					using (CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write))
					{
						cryptoStream.Write(data, 0, len);
						cryptoStream.FlushFinalBlock();
						offset += (int)memoryStream.Position;
					}
				}
			}
		}

		public byte[] Decrypt(byte[] data, int offset, int len, out int outLen, bool ivPrefix = true)
		{
			if (ivPrefix)
			{
				Buffer.BlockCopy(data, offset, reusedIvBuffer, 0, 16);
				offset += 16;
				len -= 16;
			}
			encryptorIn.IV = reusedIvBuffer;
			using (ICryptoTransform transform = encryptorIn.CreateDecryptor())
			{
				using (MemoryStream stream = new MemoryStream(data, offset, len))
				{
					using (CryptoStream cryptoStream = new CryptoStream(stream, transform, CryptoStreamMode.Read))
					{
						using (MemoryStream memoryStream = new MemoryStream(len))
						{
							int num;
							do
							{
								num = cryptoStream.Read(reusedReadBuffer, 0, 16);
								if (num != 0)
								{
									memoryStream.Write(reusedReadBuffer, 0, num);
								}
							}
							while (num != 0);
							outLen = (int)memoryStream.Length;
							return memoryStream.ToArray();
						}
					}
				}
			}
		}

		public byte[] CreateHMAC(byte[] data, int offset, int count)
		{
			hmacsha256Out.TransformFinalBlock(data, offset, count);
			byte[] hash = hmacsha256Out.Hash;
			hmacsha256Out.Initialize();
			return hash;
		}

		public bool CheckHMAC(byte[] data, int len)
		{
			hmacsha256In.ComputeHash(data, 0, len - 32);
			byte[] hash = hmacsha256In.Hash;
			bool flag = true;
			for (int i = 0; i < 4 && flag; i++)
			{
				int num = len - 32 + i * 8;
				int num2 = i * 8;
				flag = data[num] == hash[num2] && data[num + 1] == hash[num2 + 1] && data[num + 2] == hash[num2 + 2] && data[num + 3] == hash[num2 + 3] && data[num + 4] == hash[num2 + 4] && data[num + 5] == hash[num2 + 5] && data[num + 6] == hash[num2 + 6] && data[num + 7] == hash[num2 + 7];
			}
			return flag;
		}
	}
}
