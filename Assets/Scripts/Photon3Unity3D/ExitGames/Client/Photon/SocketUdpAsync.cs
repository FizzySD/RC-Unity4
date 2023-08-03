using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ExitGames.Client.Photon
{
	public class SocketUdpAsync : IPhotonSocket, IDisposable
	{
		private Socket sock;

		private readonly object syncer = new object();

		public SocketUdpAsync(PeerBase npeer)
			: base(npeer)
		{
			if (ReportDebugOfLevel(DebugLevel.INFO))
			{
				base.Listener.DebugReturn(DebugLevel.INFO, "SocketUdpAsync, .Net, Unity.");
			}
			PollReceive = false;
		}

		~SocketUdpAsync()
		{
			Dispose();
		}

		public void Dispose()
		{
			base.State = PhotonSocketState.Disconnecting;
			if (sock != null)
			{
				try
				{
					if (sock.Connected)
					{
						sock.Close();
					}
				}
				catch (Exception ex)
				{
					EnqueueDebugReturn(DebugLevel.INFO, "Exception in Dispose(): " + ex);
				}
			}
			sock = null;
			base.State = PhotonSocketState.Disconnected;
		}

		public override bool Connect()
		{
			lock (syncer)
			{
				if (!base.Connect())
				{
					return false;
				}
				base.State = PhotonSocketState.Connecting;
			}
			Thread thread = new Thread(DnsAndConnect);
			thread.IsBackground = true;
			thread.Start();
			return true;
		}

		public override bool Disconnect()
		{
			if (ReportDebugOfLevel(DebugLevel.INFO))
			{
				EnqueueDebugReturn(DebugLevel.INFO, "SocketUdpAsync.Disconnect()");
			}
			lock (syncer)
			{
				base.State = PhotonSocketState.Disconnecting;
				if (sock != null)
				{
					try
					{
						sock.Close();
					}
					catch (Exception ex)
					{
						if (ReportDebugOfLevel(DebugLevel.INFO))
						{
							EnqueueDebugReturn(DebugLevel.INFO, "Exception in Disconnect(): " + ex);
						}
					}
				}
				base.State = PhotonSocketState.Disconnected;
			}
			return true;
		}

		public override PhotonSocketError Send(byte[] data, int length)
		{
			try
			{
				if (sock == null || !sock.Connected)
				{
					return PhotonSocketError.Skipped;
				}
				sock.Send(data, 0, length, SocketFlags.None);
			}
			catch (Exception ex)
			{
				if (base.State != PhotonSocketState.Disconnecting && base.State != 0)
				{
					if (ReportDebugOfLevel(DebugLevel.INFO))
					{
						string text = "";
						if (sock != null)
						{
							text = string.Format(" Local: {0} Remote: {1} ({2}, {3})", sock.LocalEndPoint, sock.RemoteEndPoint, sock.Connected ? "connected" : "not connected", sock.IsBound ? "bound" : "not bound");
						}
						EnqueueDebugReturn(DebugLevel.INFO, string.Format("Cannot send to: {0}. Uptime: {1} ms. {2} {3}\n{4}", base.ServerAddress, SupportClass.GetTickCount() - peerBase.timeBase, base.AddressResolvedAsIpv6 ? " IPv6" : string.Empty, text, ex));
					}
					if (!sock.Connected)
					{
						EnqueueDebugReturn(DebugLevel.INFO, "Socket got closed by the local system. Disconnecting from within Send with StatusCode.Disconnect.");
						HandleException(StatusCode.SendError);
					}
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

		internal void DnsAndConnect()
		{
			IPAddress[] ipAddresses = GetIpAddresses(base.ServerAddress);
			if (ipAddresses == null)
			{
				return;
			}
			string text = string.Empty;
			IPAddress[] array = ipAddresses;
			foreach (IPAddress iPAddress in array)
			{
				try
				{
					sock = new Socket(iPAddress.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
					sock.Connect(iPAddress, base.ServerPort);
					if (sock != null && sock.Connected)
					{
						break;
					}
				}
				catch (SocketException ex)
				{
					if (ReportDebugOfLevel(DebugLevel.WARNING))
					{
						text = string.Concat(text, ex, " ", ex.ErrorCode, "; ");
						EnqueueDebugReturn(DebugLevel.WARNING, string.Concat("SocketException catched: ", ex, " ErrorCode: ", ex.ErrorCode));
					}
				}
				catch (Exception ex2)
				{
					if (ReportDebugOfLevel(DebugLevel.WARNING))
					{
						text = string.Concat(text, ex2, "; ");
						EnqueueDebugReturn(DebugLevel.WARNING, "Exception catched: " + ex2);
					}
				}
			}
			if (sock == null || !sock.Connected)
			{
				if (ReportDebugOfLevel(DebugLevel.ERROR))
				{
					EnqueueDebugReturn(DebugLevel.ERROR, "Failed to connect to server after testing each known IP. Error(s): " + text);
				}
				HandleException(StatusCode.ExceptionOnConnect);
			}
			else
			{
				base.AddressResolvedAsIpv6 = sock.AddressFamily == AddressFamily.InterNetworkV6;
				IPhotonSocket.ServerIpAddress = sock.RemoteEndPoint.ToString();
				base.State = PhotonSocketState.Connected;
				peerBase.OnConnect();
				StartReceive();
			}
		}

		public void StartReceive()
		{
			byte[] array = new byte[base.MTU];
			try
			{
				sock.BeginReceive(array, 0, array.Length, SocketFlags.None, OnReceive, array);
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

		private void OnReceive(IAsyncResult ar)
		{
			if (base.State == PhotonSocketState.Disconnecting || base.State == PhotonSocketState.Disconnected)
			{
				return;
			}
			int length = 0;
			try
			{
				length = sock.EndReceive(ar);
			}
			catch (SocketException ex)
			{
				if (base.State != PhotonSocketState.Disconnecting && base.State != 0)
				{
					if (ReportDebugOfLevel(DebugLevel.ERROR))
					{
						EnqueueDebugReturn(DebugLevel.ERROR, string.Concat("SocketException in EndReceive. State: ", base.State, ". Server: '", base.ServerAddress, "' ErrorCode: ", ex.ErrorCode, " SocketErrorCode: ", ex.SocketErrorCode, " Message: ", ex.Message, " ", ex));
					}
					HandleException(StatusCode.ExceptionOnReceive);
				}
			}
			catch (Exception ex2)
			{
				if (base.State != PhotonSocketState.Disconnecting && base.State != 0)
				{
					if (ReportDebugOfLevel(DebugLevel.ERROR))
					{
						EnqueueDebugReturn(DebugLevel.ERROR, string.Concat("Exception in EndReceive. State: ", base.State, ". Server: '", base.ServerAddress, "' Exception: ", ex2));
					}
					HandleException(StatusCode.ExceptionOnReceive);
				}
			}
			if (base.State == PhotonSocketState.Disconnecting || base.State == PhotonSocketState.Disconnected)
			{
				return;
			}
			byte[] array = (byte[])ar.AsyncState;
			HandleReceivedDatagram(array, length, true);
			try
			{
				sock.BeginReceive(array, 0, array.Length, SocketFlags.None, OnReceive, array);
			}
			catch (SocketException ex3)
			{
				if (base.State != PhotonSocketState.Disconnecting && base.State != 0)
				{
					if (ReportDebugOfLevel(DebugLevel.ERROR))
					{
						EnqueueDebugReturn(DebugLevel.ERROR, string.Concat("SocketException in BeginReceive. State: ", base.State, ". Server: '", base.ServerAddress, "' ErrorCode: ", ex3.ErrorCode, " SocketErrorCode: ", ex3.SocketErrorCode, " Message: ", ex3.Message, " ", ex3));
					}
					HandleException(StatusCode.ExceptionOnReceive);
				}
			}
			catch (Exception ex4)
			{
				if (base.State != PhotonSocketState.Disconnecting && base.State != 0)
				{
					if (ReportDebugOfLevel(DebugLevel.ERROR))
					{
						EnqueueDebugReturn(DebugLevel.ERROR, string.Concat("Exception in BeginReceive. State: ", base.State, ". Server: '", base.ServerAddress, "' Exception: ", ex4));
					}
					HandleException(StatusCode.ExceptionOnReceive);
				}
			}
		}
	}
}
