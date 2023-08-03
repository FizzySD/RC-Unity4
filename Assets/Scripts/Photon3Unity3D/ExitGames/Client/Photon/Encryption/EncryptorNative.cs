using System;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;

namespace ExitGames.Client.Photon.Encryption
{
	public class EncryptorNative : IPhotonEncryptor
	{
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void LogCallbackDelegate(IntPtr userData, int level, [MarshalAs(UnmanagedType.LPStr)] string msg);

		private enum egDebugLevel
		{
			OFF = 0,
			ERRORS = 1,
			WARNINGS = 2,
			INFO = 3,
			ALL = 4
		}

		private const string LibName = "PhotonEncryptorPlugin";

		public static readonly int BLOCK_SIZE = 16;

		public static readonly int IV_SIZE = BLOCK_SIZE;

		public static readonly int HMAC_SIZE = 32;

		protected IntPtr encryptor;

		protected byte[] hmacHash = new byte[HMAC_SIZE];

		[DllImport("PhotonEncryptorPlugin")]
		public static extern IntPtr egconstructEncryptor(byte[] pEncryptSecret, byte[] pHmacSecret);

		[DllImport("PhotonEncryptorPlugin")]
		public static extern void egdestructEncryptor(IntPtr pEncryptor);

		[DllImport("PhotonEncryptorPlugin")]
		public static extern void egencrypt(IntPtr pEncryptor, byte[] pIn, int inSize, byte[] pOut, ref int outSize, ref int outOffset);

		[DllImport("PhotonEncryptorPlugin")]
		public static extern void egdecrypt(IntPtr pEncryptor, byte[] pIn, int inSizem, int inOffset, byte[] pOut, ref int outSize);

		[DllImport("PhotonEncryptorPlugin")]
		public static extern void egHMAC(IntPtr pEncryptor, byte[] pIn, int inSize, int inOffset, byte[] pOut, ref int outSize);

		[DllImport("PhotonEncryptorPlugin")]
		public static extern int eggetBlockSize();

		[DllImport("PhotonEncryptorPlugin")]
		public static extern int eggetIVSize();

		[DllImport("PhotonEncryptorPlugin")]
		public static extern int eggetHMACSize();

		[DllImport("PhotonEncryptorPlugin")]
		public static extern void egsetEncryptorLoggingCallback(IntPtr userData, LogCallbackDelegate callback);

		[DllImport("PhotonEncryptorPlugin")]
		public static extern bool egsetEncryptorLoggingLevel(int level);

		~EncryptorNative()
		{
			Dispose(false);
		}

		[MonoPInvokeCallback(typeof(LogCallbackDelegate))]
		private static void OnNativeLog(IntPtr userData, int debugLevel, string message)
		{
			switch ((egDebugLevel)debugLevel)
			{
			case egDebugLevel.ERRORS:
				Debug.LogError("EncryptorNative: " + message);
				break;
			case egDebugLevel.WARNINGS:
				Debug.LogWarning("EncryptorNative: " + message);
				break;
			case egDebugLevel.OFF:
			case egDebugLevel.INFO:
			case egDebugLevel.ALL:
				Debug.Log("EncryptorNative: " + message);
				break;
			}
		}

		public void Init(byte[] encryptionSecret, byte[] hmacSecret, byte[] ivBytes = null, bool chainingModeGCM = false)
		{
			egsetEncryptorLoggingCallback(IntPtr.Zero, OnNativeLog);
			egsetEncryptorLoggingLevel(1);
			encryptor = egconstructEncryptor(encryptionSecret, hmacSecret);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool dispose)
		{
			if (encryptor != IntPtr.Zero)
			{
				egdestructEncryptor(encryptor);
				encryptor = IntPtr.Zero;
			}
		}

		public void Encrypt(byte[] data, int len, byte[] output, ref int offset, bool ivPrefix = true)
		{
			int outSize = output.Length;
			egencrypt(encryptor, data, len, output, ref outSize, ref offset);
		}

		public byte[] CreateHMAC(byte[] data, int offset, int count)
		{
			lock (hmacHash)
			{
				int outSize = hmacHash.Length;
				egHMAC(encryptor, data, count, offset, hmacHash, ref outSize);
			}
			return hmacHash;
		}

		public byte[] Decrypt(byte[] data, int offset, int len, out int outLen, bool ivPrefix = true)
		{
			outLen = (len - offset) / BLOCK_SIZE * BLOCK_SIZE + BLOCK_SIZE;
			byte[] array = new byte[outLen];
			egdecrypt(encryptor, data, len, offset, array, ref outLen);
			return array;
		}

		public bool CheckHMAC(byte[] data, int len)
		{
			lock (hmacHash)
			{
				int outSize = hmacHash.Length;
				egHMAC(encryptor, data, len - HMAC_SIZE, 0, hmacHash, ref outSize);
				byte[] array = hmacHash;
				bool flag = true;
				for (int i = 0; i < 4 && flag; i++)
				{
					int num = len - HMAC_SIZE + i * 8;
					int num2 = i * 8;
					flag = data[num] == array[num2] && data[num + 1] == array[num2 + 1] && data[num + 2] == array[num2 + 2] && data[num + 3] == array[num2 + 3] && data[num + 4] == array[num2 + 4] && data[num + 5] == array[num2 + 5] && data[num + 6] == array[num2 + 6] && data[num + 7] == array[num2 + 7];
				}
				return flag;
			}
		}
	}
}
