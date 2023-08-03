using System;
using System.Runtime.InteropServices;
using System.Threading;
using AOT;

namespace ExitGames.Client.Photon
{
	public class SocketNative : IPhotonSocket
	{
		public delegate void LogCallbackDelegate(IntPtr pUserData, int level, [MarshalAs(UnmanagedType.LPStr)] string msg);

		private enum NativeSocketState : byte
		{
			Disconnected = 0,
			Connecting = 1,
			Connected = 2,
			ConnectionError = 3,
			SendError = 4,
			ReceiveError = 5,
			Disconnecting = 6
		}

		private enum NativeDebugLevel : byte
		{
			OFF = 0,
			ERRORS = 1,
			WARNINGS = 2,
			INFO = 3,
			ALL = 4
		}

		private const string LibName = "PhotonSocketPlugin";

		private IntPtr pConnectionHandler = IntPtr.Zero;

		private GCHandle handle;

		internal static readonly object syncer = new object();

		private string serializationProtocolType;

		private byte[] serializationProtocolBytes = new byte[2];

		private string appId;

		private NativeDebugLevel debugLevel;

		[DllImport("PhotonSocketPlugin")]
		public static extern IntPtr egconnect([MarshalAs(UnmanagedType.LPStr)] string address);

		[DllImport("PhotonSocketPlugin")]
		public static extern IntPtr egconnectWithProtocol([MarshalAs(UnmanagedType.LPStr)] string address, byte connectionProtocol);

		[DllImport("PhotonSocketPlugin")]
		public static extern IntPtr egconnectWithProtocols([MarshalAs(UnmanagedType.LPStr)] string address, byte connectionProtocol, [MarshalAs(UnmanagedType.LPStr)] string appID, [MarshalAs(UnmanagedType.LPStr)] string serializationProtocolString, byte serializationProtocolMajor, byte serializationProtocolMinor);

		[DllImport("PhotonSocketPlugin")]
		public static extern byte eggetState(IntPtr pConnectionHandler);

		[DllImport("PhotonSocketPlugin")]
		public static extern void egdisconnect(IntPtr pConnectionHandler);

		[DllImport("PhotonSocketPlugin")]
		public static extern uint egservice(IntPtr pConnectionHandler);

		[DllImport("PhotonSocketPlugin")]
		public static extern bool egsend(IntPtr pConnectionHandler, byte[] arr, uint size);

		[DllImport("PhotonSocketPlugin")]
		public static extern uint egread(IntPtr pConnectionHandler, byte[] arr, ref uint size);

		[DllImport("PhotonSocketPlugin")]
		public static extern void egsetSocketLoggingCallback(IntPtr pConnectionHandler, IntPtr userData, LogCallbackDelegate callback);

		[DllImport("PhotonSocketPlugin")]
		public static extern bool egsetSocketLoggingLevel(IntPtr pConnectionHandler, int level);

		[DllImport("PhotonSocketPlugin")]
		public static extern bool eggetUsingIPv6(IntPtr pConnectionHandler);

		public SocketNative(PeerBase npeer)
			: base(npeer)
		{
			handle = GCHandle.Alloc(this);
			base.ServerAddress = npeer.ServerAddress;
			serializationProtocolType = npeer.SerializationProtocol.ProtocolType;
			serializationProtocolBytes = npeer.SerializationProtocol.VersionBytes;
			appId = npeer.AppId;
			debugLevel = (NativeDebugLevel)((npeer.debugOut == DebugLevel.ALL) ? ((DebugLevel)4) : npeer.debugOut);
			if (ReportDebugOfLevel(DebugLevel.INFO))
			{
				base.Listener.DebugReturn(DebugLevel.INFO, "new SocketNative() for Unity. Server: " + base.ServerAddress);
			}
			PollReceive = false;
		}

		~SocketNative()
		{
			Dispose();
		}

		public void Dispose()
		{
			lock (syncer)
			{
				if (pConnectionHandler != IntPtr.Zero)
				{
					egdisconnect(pConnectionHandler);
				}
				pConnectionHandler = IntPtr.Zero;
				handle.Free();
				base.State = PhotonSocketState.Disconnected;
			}
			GC.SuppressFinalize(this);
		}

		public override bool Connect()
		{
			if (base.State == PhotonSocketState.Connected)
			{
				EnqueueDebugReturn(DebugLevel.ERROR, "Already connected!");
				return false;
			}
			if (base.State == PhotonSocketState.Connecting)
			{
				EnqueueDebugReturn(DebugLevel.ERROR, "Connect already in process!");
				return false;
			}
			if (!base.Connect())
			{
				return false;
			}
			base.State = PhotonSocketState.Connecting;
			Thread thread = new Thread(DnsAndConnect);
			thread.IsBackground = true;
			thread.Start();
			return true;
		}

		[MonoPInvokeCallback(typeof(LogCallbackDelegate))]
		private static void DebugReturn(IntPtr pUserData, int debugLevel, string str)
		{
			((SocketNative)((GCHandle)pUserData).Target).EnqueueDebugReturn((DebugLevel)debugLevel, str);
		}

		internal void DnsAndConnect()
		{
			lock (syncer)
			{
				try
				{
					pConnectionHandler = egconnectWithProtocols(base.ConnectAddress, (byte)Protocol, appId, serializationProtocolType, serializationProtocolBytes[0], serializationProtocolBytes[1]);
					egsetSocketLoggingCallback(pConnectionHandler, (IntPtr)handle, DebugReturn);
					egsetSocketLoggingLevel(pConnectionHandler, (int)debugLevel);
				}
				catch (EntryPointNotFoundException ex)
				{
					EnqueueDebugReturn(DebugLevel.ERROR, "EntryPointNotFoundException: " + ex.Message);
					HandleException(StatusCode.ExceptionOnConnect);
					return;
				}
				catch (DllNotFoundException ex2)
				{
					EnqueueDebugReturn(DebugLevel.ERROR, "DllNotFoundException: " + ex2);
					HandleException(StatusCode.ExceptionOnConnect);
					return;
				}
			}
			while (true)
			{
				lock (syncer)
				{
					if (pConnectionHandler != IntPtr.Zero && eggetState(pConnectionHandler) == 1)
					{
						egservice(pConnectionHandler);
						continue;
					}
				}
				break;
			}
			lock (syncer)
			{
				if (pConnectionHandler != IntPtr.Zero)
				{
					NativeSocketState nativeSocketState = (NativeSocketState)eggetState(pConnectionHandler);
					if (nativeSocketState == NativeSocketState.Connected)
					{
						base.AddressResolvedAsIpv6 = eggetUsingIPv6(pConnectionHandler);
						base.State = PhotonSocketState.Connected;
						peerBase.OnConnect();
					}
					else
					{
						if (ReportDebugOfLevel(DebugLevel.ERROR))
						{
							EnqueueDebugReturn(DebugLevel.ERROR, "Connect() to '" + base.ServerAddress + "' failed: native socket state = " + nativeSocketState);
						}
						HandleException(StatusCode.ExceptionOnConnect);
					}
				}
			}
			Thread thread = new Thread(ReceiveLoop);
			thread.IsBackground = true;
			thread.Start();
		}

		public override bool Disconnect()
		{
			if (ReportDebugOfLevel(DebugLevel.INFO))
			{
				EnqueueDebugReturn(DebugLevel.INFO, "SocketNative.Disconnect()");
			}
			base.State = PhotonSocketState.Disconnecting;
			lock (syncer)
			{
				if (pConnectionHandler != IntPtr.Zero)
				{
					try
					{
						egdisconnect(pConnectionHandler);
						pConnectionHandler = IntPtr.Zero;
					}
					catch (Exception ex)
					{
						EnqueueDebugReturn(DebugLevel.INFO, "Exception in Disconnect(): " + ex);
					}
				}
			}
			base.State = PhotonSocketState.Disconnected;
			return true;
		}

		public override PhotonSocketError Send(byte[] data, int length)
		{
			if (base.State != PhotonSocketState.Connected)
			{
				return PhotonSocketError.Skipped;
			}
			try
			{
				if (ReportDebugOfLevel(DebugLevel.ALL))
				{
					base.Listener.DebugReturn(DebugLevel.ALL, "Sending: " + length);
				}
				lock (syncer)
				{
					if (pConnectionHandler != IntPtr.Zero)
					{
						egsend(pConnectionHandler, data, (uint)length);
					}
				}
			}
			catch (Exception ex)
			{
				if (base.State != PhotonSocketState.Disconnecting && base.State != 0)
				{
					if (ReportDebugOfLevel(DebugLevel.INFO))
					{
						base.Listener.DebugReturn(DebugLevel.INFO, "Cannot send to: " + base.ServerAddress + ". " + ex.Message);
					}
					HandleException(StatusCode.SendError);
				}
				return PhotonSocketError.Exception;
			}
			return PhotonSocketError.Success;
		}

		public override PhotonSocketError Receive(out byte[] data)
		{
			data = null;
			return PhotonSocketError.NoData;
		}

		public void ReceiveLoop()
		{
			while (base.State == PhotonSocketState.Connecting || base.State == PhotonSocketState.Connected)
			{
				try
				{
					uint num = 0u;
					byte[] array = null;
					lock (syncer)
					{
						if (pConnectionHandler == IntPtr.Zero)
						{
							throw new Exception("Pointer to unmanaged socket is Zero. Can't read.");
						}
						NativeSocketState nativeSocketState = (NativeSocketState)eggetState(pConnectionHandler);
						if (nativeSocketState != NativeSocketState.Connected && nativeSocketState != NativeSocketState.Connecting)
						{
							EnqueueDebugReturn(DebugLevel.ERROR, "Disconnecting cause native socket's state during read is: " + nativeSocketState);
							int statusCode;
							switch (nativeSocketState)
							{
							default:
								statusCode = 1026;
								break;
							case NativeSocketState.ReceiveError:
								statusCode = 1039;
								break;
							case NativeSocketState.SendError:
								statusCode = 1030;
								break;
							}
							HandleException((StatusCode)statusCode);
						}
						uint size = egservice(pConnectionHandler);
						if (size != 0)
						{
							array = new byte[size];
							num = egread(pConnectionHandler, array, ref size);
						}
					}
					if (array != null)
					{
						try
						{
							HandleReceivedDatagram(array, array.Length, false);
						}
						catch (Exception ex)
						{
							if (base.State != PhotonSocketState.Disconnecting && base.State != 0)
							{
								if (ReportDebugOfLevel(DebugLevel.ERROR))
								{
									EnqueueDebugReturn(DebugLevel.ERROR, string.Concat("Receive issue. State: ", base.State, ". Server: '", base.ServerAddress, "' Exception: ", ex));
								}
								HandleException(StatusCode.ExceptionOnReceive);
							}
						}
					}
					if (num == 0)
					{
						Thread.Sleep(15);
					}
				}
				catch (ObjectDisposedException ex2)
				{
					if (base.State != PhotonSocketState.Disconnecting && base.State != 0)
					{
						if (ReportDebugOfLevel(DebugLevel.ERROR))
						{
							EnqueueDebugReturn(DebugLevel.ERROR, string.Concat("Receive issue (ObjectDisposedException). State: ", base.State, " Exception: ", ex2));
						}
						HandleException(StatusCode.ExceptionOnReceive);
						break;
					}
				}
				catch (Exception ex3)
				{
					if (base.State != PhotonSocketState.Disconnecting && base.State != 0)
					{
						if (ReportDebugOfLevel(DebugLevel.ERROR))
						{
							EnqueueDebugReturn(DebugLevel.ERROR, string.Concat("Receive issue. State: ", base.State, ". Server: '", base.ServerAddress, "' Exception: ", ex3));
						}
						HandleException(StatusCode.ExceptionOnReceive);
						break;
					}
				}
			}
			Disconnect();
		}
	}
}
