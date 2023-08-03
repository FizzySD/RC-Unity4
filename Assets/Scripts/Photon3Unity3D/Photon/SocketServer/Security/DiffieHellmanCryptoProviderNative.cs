using System;
using System.Runtime.InteropServices;

namespace Photon.SocketServer.Security
{
	internal class DiffieHellmanCryptoProviderNative : ICryptoProvider, IDisposable
	{
		private IntPtr cryptor;

		private byte[] sharedKeyHash;

		public bool IsInitialized
		{
			get
			{
				return sharedKeyHash != null || egCryptorIsEncryptionAvailable(cryptor);
			}
		}

		public byte[] PublicKey
		{
			get
			{
				if (sharedKeyHash != null)
				{
					throw new Exception("Can't get PublicKey on DiffieHellmanCryptoProviderNative object initialized with shared key hash");
				}
				IntPtr key;
				int keySize;
				egCryptorPublicKey(cryptor, out key, out keySize);
				byte[] array = new byte[keySize];
				Marshal.Copy(key, array, 0, keySize);
				return array;
			}
		}

		[DllImport("PhotonCryptoPlugin")]
		public static extern IntPtr egCryptorCreate();

		[DllImport("PhotonCryptoPlugin")]
		public static extern int egCryptorPublicKey(IntPtr cryptor, out IntPtr key, out int keySize);

		[DllImport("PhotonCryptoPlugin")]
		public static extern int egCryptorDeriveSharedKey(IntPtr cryptor, byte[] serverPublicKey, int keySize);

		[DllImport("PhotonCryptoPlugin")]
		public static extern int egCryptorEncrypt(IntPtr cryptor, byte[] plainData, int plainDataOffset, int plainDataSize, byte[] sharedKeyHash, out IntPtr encodedData, out int encodedDataSize);

		[DllImport("PhotonCryptoPlugin")]
		public static extern int egCryptorDecrypt(IntPtr cryptor, byte[] encodedData, int encodedDataOffset, int encodedDataSize, byte[] sharedKeyHash, out IntPtr plainData, out int plainDataSize);

		[DllImport("PhotonCryptoPlugin")]
		public static extern bool egCryptorIsEncryptionAvailable(IntPtr cryptor);

		[DllImport("PhotonCryptoPlugin")]
		public static extern void egCryptorDispose(IntPtr cryptor);

		public DiffieHellmanCryptoProviderNative()
		{
			cryptor = egCryptorCreate();
		}

		public DiffieHellmanCryptoProviderNative(byte[] sharedKeyHash)
		{
			if (sharedKeyHash == null)
			{
				throw new ArgumentNullException("sharedKeyHash");
			}
			this.sharedKeyHash = sharedKeyHash;
		}

		public void DeriveSharedKey(byte[] otherPartyPublicKey)
		{
			if (sharedKeyHash != null)
			{
				throw new Exception("Can't call DeriveSharedKey on DiffieHellmanCryptoProviderNative object initialized with shared key hash");
			}
			egCryptorDeriveSharedKey(cryptor, otherPartyPublicKey, otherPartyPublicKey.Length);
		}

		public byte[] Encrypt(byte[] data)
		{
			return Encrypt(data, 0, data.Length);
		}

		public byte[] Encrypt(byte[] data, int offset, int count)
		{
			IntPtr encodedData;
			int encodedDataSize;
			if (egCryptorEncrypt(cryptor, data, offset, count, sharedKeyHash, out encodedData, out encodedDataSize) == 0)
			{
				byte[] array = new byte[encodedDataSize];
				Marshal.Copy(encodedData, array, 0, encodedDataSize);
				return array;
			}
			return null;
		}

		public byte[] Decrypt(byte[] data)
		{
			return Decrypt(data, 0, data.Length);
		}

		public byte[] Decrypt(byte[] data, int offset, int count)
		{
			IntPtr plainData;
			int plainDataSize;
			if (egCryptorDecrypt(cryptor, data, offset, count, sharedKeyHash, out plainData, out plainDataSize) == 0)
			{
				byte[] array = new byte[plainDataSize];
				Marshal.Copy(plainData, array, 0, plainDataSize);
				return array;
			}
			return null;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected void Dispose(bool disposing)
		{
			if (disposing)
			{
				IntPtr cryptor2 = cryptor;
				if (true)
				{
					egCryptorDispose(cryptor);
				}
			}
		}
	}
}
