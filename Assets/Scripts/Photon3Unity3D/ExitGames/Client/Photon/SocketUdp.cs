using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ExitGames.Client.Photon
{
	internal class SocketUdp : IPhotonSocket, IDisposable
	{
		private Socket sock;

		private readonly object syncer = new object();

		public SocketUdp(PeerBase npeer)
			: base(npeer)
		{
			if (ReportDebugOfLevel(DebugLevel.INFO))
			{
				base.Listener.DebugReturn(DebugLevel.INFO, "SocketUdp, .Net, Unity.");
			}
			PollReceive = false;
		}

		~SocketUdp()
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
				EnqueueDebugReturn(DebugLevel.INFO, "SocketUdp.Disconnect()");
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
				Thread thread = new Thread(ReceiveLoop);
				thread.IsBackground = true;
				thread.Start();
			}
		}

		public void ReceiveLoop()
		{
			byte[] array = new byte[base.MTU];
			while (base.State == PhotonSocketState.Connected)
			{
				try
				{
					int length = sock.Receive(array);
					HandleReceivedDatagram(array, length, true);
				}
				catch (SocketException ex)
				{
					if (base.State != PhotonSocketState.Disconnecting && base.State != 0)
					{
						if (ReportDebugOfLevel(DebugLevel.ERROR))
						{
							EnqueueDebugReturn(DebugLevel.ERROR, string.Concat("Receive issue. State: ", base.State, ". Server: '", base.ServerAddress, "' ErrorCode: ", ex.ErrorCode, " SocketErrorCode: ", ex.SocketErrorCode, " Message: ", ex.Message, " ", ex));
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
							EnqueueDebugReturn(DebugLevel.ERROR, string.Concat("Receive issue. State: ", base.State, ". Server: '", base.ServerAddress, "' Message: ", ex2.Message, " Exception: ", ex2));
						}
						HandleException(StatusCode.ExceptionOnReceive);
					}
				}
			}
			Disconnect();
		}
	}
}
