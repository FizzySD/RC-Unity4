using System;
using System.Net;
using System.Net.Sockets;
using System.Security;
using System.Threading;

namespace ExitGames.Client.Photon
{
	internal class SocketTcp : IPhotonSocket, IDisposable
	{
		private Socket sock;

		private readonly object syncer = new object();

		public SocketTcp(PeerBase npeer)
			: base(npeer)
		{
			if (ReportDebugOfLevel(DebugLevel.INFO))
			{
				base.Listener.DebugReturn(DebugLevel.INFO, "SocketTcp, .Net, Unity.");
			}
			PollReceive = false;
		}

		~SocketTcp()
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
				EnqueueDebugReturn(DebugLevel.INFO, "SocketTcp.Disconnect()");
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
						EnqueueDebugReturn(DebugLevel.INFO, string.Format("Cannot send to: {0} ({4}). Uptime: {1} ms. {2} {3}", base.ServerAddress, SupportClass.GetTickCount() - peerBase.timeBase, base.AddressResolvedAsIpv6 ? " IPv6" : string.Empty, text, ex));
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
					sock = new Socket(iPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
					sock.NoDelay = true;
					sock.ReceiveTimeout = peerBase.DisconnectTimeout;
					sock.SendTimeout = peerBase.DisconnectTimeout;
					sock.Connect(iPAddress, base.ServerPort);
					if (sock != null && sock.Connected)
					{
						break;
					}
				}
				catch (SecurityException ex)
				{
					if (ReportDebugOfLevel(DebugLevel.ERROR))
					{
						text = string.Concat(text, ex, " ");
						EnqueueDebugReturn(DebugLevel.WARNING, "SecurityException catched: " + ex);
					}
				}
				catch (SocketException ex2)
				{
					if (ReportDebugOfLevel(DebugLevel.WARNING))
					{
						text = string.Concat(text, ex2, " ", ex2.ErrorCode, "; ");
						EnqueueDebugReturn(DebugLevel.WARNING, string.Concat("SocketException catched: ", ex2, " ErrorCode: ", ex2.ErrorCode));
					}
				}
				catch (Exception ex3)
				{
					if (ReportDebugOfLevel(DebugLevel.WARNING))
					{
						text = string.Concat(text, ex3, "; ");
						EnqueueDebugReturn(DebugLevel.WARNING, "Exception catched: " + ex3);
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
			StreamBuffer streamBuffer = new StreamBuffer(base.MTU);
			byte[] array = new byte[9];
			while (base.State == PhotonSocketState.Connected)
			{
				streamBuffer.SetLength(0L);
				try
				{
					int num = 0;
					int num2 = 0;
					while (num < 9)
					{
						try
						{
							num2 = sock.Receive(array, num, 9 - num, SocketFlags.None);
						}
						catch (SocketException ex)
						{
							if (base.State != PhotonSocketState.Disconnecting && base.State > PhotonSocketState.Disconnected && ex.SocketErrorCode == SocketError.WouldBlock)
							{
								if (ReportDebugOfLevel(DebugLevel.ALL))
								{
									EnqueueDebugReturn(DebugLevel.ALL, "ReceiveLoop() got a WouldBlock exception. This is non-fatal. Going to continue.");
								}
								continue;
							}
							throw;
						}
						num += num2;
						if (num2 == 0)
						{
							throw new SocketException(10054);
						}
					}
					if (array[0] == 240)
					{
						HandleReceivedDatagram(array, array.Length, true);
						continue;
					}
					int num3 = (array[1] << 24) | (array[2] << 16) | (array[3] << 8) | array[4];
					if (peerBase.TrafficStatsEnabled)
					{
						if (array[5] == 0)
						{
							peerBase.TrafficStatsIncoming.CountReliableOpCommand(num3);
						}
						else
						{
							peerBase.TrafficStatsIncoming.CountUnreliableOpCommand(num3);
						}
					}
					if (ReportDebugOfLevel(DebugLevel.ALL))
					{
						EnqueueDebugReturn(DebugLevel.ALL, "TCP < " + num3);
					}
					streamBuffer.SetCapacityMinimum(num3 - 7);
					streamBuffer.Write(array, 7, num - 7);
					num = 0;
					num3 -= 9;
					while (num < num3)
					{
						try
						{
							num2 = sock.Receive(streamBuffer.GetBuffer(), streamBuffer.Position, num3 - num, SocketFlags.None);
						}
						catch (SocketException ex2)
						{
							if (base.State != PhotonSocketState.Disconnecting && base.State > PhotonSocketState.Disconnected && ex2.SocketErrorCode == SocketError.WouldBlock)
							{
								if (ReportDebugOfLevel(DebugLevel.ALL))
								{
									EnqueueDebugReturn(DebugLevel.ALL, "ReceiveLoop() got a WouldBlock exception. This is non-fatal. Going to continue.");
								}
								continue;
							}
							throw;
						}
						streamBuffer.Position += num2;
						num += num2;
						if (num2 == 0)
						{
							throw new SocketException(10054);
						}
					}
					HandleReceivedDatagram(streamBuffer.ToArray(), streamBuffer.Length, false);
					if (ReportDebugOfLevel(DebugLevel.ALL))
					{
						EnqueueDebugReturn(DebugLevel.ALL, "TCP < " + streamBuffer.Length + ((streamBuffer.Length == num3 + 2) ? " OK" : " BAD"));
					}
				}
				catch (SocketException ex3)
				{
					if (base.State != PhotonSocketState.Disconnecting && base.State != 0)
					{
						if (ReportDebugOfLevel(DebugLevel.ERROR))
						{
							EnqueueDebugReturn(DebugLevel.ERROR, "Receiving failed. SocketException: " + ex3.SocketErrorCode);
						}
						if (ex3.SocketErrorCode == SocketError.ConnectionReset || ex3.SocketErrorCode == SocketError.ConnectionAborted)
						{
							HandleException(StatusCode.DisconnectByServerTimeout);
						}
						else
						{
							HandleException(StatusCode.ExceptionOnReceive);
						}
					}
				}
				catch (Exception ex4)
				{
					if (base.State != PhotonSocketState.Disconnecting && base.State != 0)
					{
						if (ReportDebugOfLevel(DebugLevel.ERROR))
						{
							EnqueueDebugReturn(DebugLevel.ERROR, string.Concat("Receive issue. State: ", base.State, ". Server: '", base.ServerAddress, "' Exception: ", ex4));
						}
						HandleException(StatusCode.ExceptionOnReceive);
					}
				}
			}
			Disconnect();
		}
	}
}
