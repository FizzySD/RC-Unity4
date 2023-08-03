using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace ExitGames.Client.Photon
{
	public abstract class IPhotonSocket
	{
		protected internal PeerBase peerBase;

		protected readonly ConnectionProtocol Protocol;

		public bool PollReceive;

		protected IPhotonPeerListener Listener
		{
			get
			{
				return peerBase.Listener;
			}
		}

		protected internal int MTU
		{
			get
			{
				return peerBase.mtu;
			}
		}

		public PhotonSocketState State { get; protected set; }

		public bool Connected
		{
			get
			{
				return State == PhotonSocketState.Connected;
			}
		}

		public string ConnectAddress
		{
			get
			{
				return peerBase.ServerAddress;
			}
		}

		public string ServerAddress { get; protected set; }

		public string ProxyServerAddress { get; protected set; }

		public static string ServerIpAddress { get; protected set; }

		public int ServerPort { get; protected set; }

		public bool AddressResolvedAsIpv6 { get; protected internal set; }

		public string UrlProtocol { get; protected set; }

		public string UrlPath { get; protected set; }

		protected internal string SerializationProtocol
		{
			get
			{
				if (peerBase == null || peerBase.photonPeer == null)
				{
					return "GpBinaryV18";
				}
				return Enum.GetName(typeof(SerializationProtocol), peerBase.photonPeer.SerializationProtocolType);
			}
		}

		public IPhotonSocket(PeerBase peerBase)
		{
			if (peerBase == null)
			{
				throw new Exception("Can't init without peer");
			}
			Protocol = peerBase.usedTransportProtocol;
			this.peerBase = peerBase;
		}

		public virtual bool Connect()
		{
			if (State != 0)
			{
				if ((int)peerBase.debugOut >= 1)
				{
					peerBase.Listener.DebugReturn(DebugLevel.ERROR, "Connect() failed: connection in State: " + State);
				}
				return false;
			}
			if (peerBase == null || Protocol != peerBase.usedTransportProtocol)
			{
				return false;
			}
			string address;
			ushort port;
			string urlProtocol;
			string urlPath;
			if (!TryParseAddress(peerBase.ServerAddress, out address, out port, out urlProtocol, out urlPath))
			{
				if ((int)peerBase.debugOut >= 1)
				{
					peerBase.Listener.DebugReturn(DebugLevel.ERROR, "Failed parsing address: " + peerBase.ServerAddress);
				}
				return false;
			}
			ServerIpAddress = string.Empty;
			ServerAddress = address;
			ServerPort = port;
			UrlProtocol = urlProtocol;
			UrlPath = urlPath;
			if ((int)peerBase.debugOut >= 5)
			{
				Listener.DebugReturn(DebugLevel.ALL, "IPhotonSocket.Connect() " + ServerAddress + ":" + ServerPort + " this.Protocol: " + Protocol);
			}
			return true;
		}

		public abstract bool Disconnect();

		public abstract PhotonSocketError Send(byte[] data, int length);

		public abstract PhotonSocketError Receive(out byte[] data);

		public void HandleReceivedDatagram(byte[] inBuffer, int length, bool willBeReused)
		{
			if (peerBase.NetworkSimulationSettings.IsSimulationEnabled)
			{
				if (willBeReused)
				{
					byte[] array = new byte[length];
					Buffer.BlockCopy(inBuffer, 0, array, 0, length);
					peerBase.ReceiveNetworkSimulated(array);
				}
				else
				{
					peerBase.ReceiveNetworkSimulated(inBuffer);
				}
			}
			else
			{
				peerBase.ReceiveIncomingCommands(inBuffer, length);
			}
		}

		public bool ReportDebugOfLevel(DebugLevel levelOfMessage)
		{
			return (int)peerBase.debugOut >= (int)levelOfMessage;
		}

		public void EnqueueDebugReturn(DebugLevel debugLevel, string message)
		{
			peerBase.EnqueueDebugReturn(debugLevel, message);
		}

		protected internal void HandleException(StatusCode statusCode)
		{
			State = PhotonSocketState.Disconnecting;
			peerBase.EnqueueStatusCallback(statusCode);
			peerBase.EnqueueActionForDispatch(delegate
			{
				peerBase.Disconnect();
			});
		}

		protected internal bool TryParseAddress(string url, out string address, out ushort port, out string urlProtocol, out string urlPath)
		{
			address = string.Empty;
			port = 0;
			urlProtocol = string.Empty;
			urlPath = string.Empty;
			string text = url;
			if (string.IsNullOrEmpty(text))
			{
				return false;
			}
			int num = text.IndexOf("://");
			if (num >= 0)
			{
				urlProtocol = text.Substring(0, num);
				text = text.Substring(num + 3);
			}
			num = text.IndexOf("/");
			if (num >= 0)
			{
				urlPath = text.Substring(num);
				text = text.Substring(0, num);
			}
			num = text.LastIndexOf(':');
			if (num < 0)
			{
				return false;
			}
			if (text.IndexOf(':') != num && (!text.Contains("[") || !text.Contains("]")))
			{
				return false;
			}
			address = text.Substring(0, num);
			string s = text.Substring(num + 1);
			return ushort.TryParse(s, out port);
		}

		protected internal bool IsIpv6SimpleCheck(IPAddress address)
		{
			return address != null && address.ToString().Contains(":");
		}

		protected internal IPAddress[] GetIpAddresses(string hostname)
		{
			IPAddress address = null;
			if (IPAddress.TryParse(hostname, out address))
			{
				return new IPAddress[1] { address };
			}
			IPAddress[] addressList;
			try
			{
				IPHostEntry hostEntry = Dns.GetHostEntry(ServerAddress);
				addressList = hostEntry.AddressList;
			}
			catch (Exception ex)
			{
				if (ReportDebugOfLevel(DebugLevel.ERROR))
				{
					EnqueueDebugReturn(DebugLevel.ERROR, "DNS.GetHostEntry() failed for: " + ServerAddress + ". Exception: " + ex);
				}
				HandleException(StatusCode.ExceptionOnConnect);
				return null;
			}
			Array.Sort(addressList, AddressSortComparer);
			if (ReportDebugOfLevel(DebugLevel.INFO))
			{
				string[] value = addressList.Select((IPAddress x) => string.Concat(x.ToString(), " (", x.AddressFamily, ")")).ToArray();
				string text = string.Join(", ", value);
				if (ReportDebugOfLevel(DebugLevel.INFO))
				{
					EnqueueDebugReturn(DebugLevel.INFO, ServerAddress + " resolved to these addresses: " + text);
				}
			}
			return addressList;
		}

		private int AddressSortComparer(IPAddress x, IPAddress y)
		{
			if (x.AddressFamily == y.AddressFamily)
			{
				return 0;
			}
			return (x.AddressFamily != AddressFamily.InterNetworkV6) ? 1 : (-1);
		}

		[Obsolete("Use GetIpAddresses instead.")]
		protected internal static IPAddress GetIpAddress(string address)
		{
			IPAddress address2 = null;
			if (IPAddress.TryParse(address, out address2))
			{
				return address2;
			}
			IPHostEntry hostEntry = Dns.GetHostEntry(address);
			IPAddress[] addressList = hostEntry.AddressList;
			IPAddress[] array = addressList;
			foreach (IPAddress iPAddress in array)
			{
				if (iPAddress.AddressFamily == AddressFamily.InterNetworkV6)
				{
					ServerIpAddress = iPAddress.ToString();
					return iPAddress;
				}
				if (address2 == null && iPAddress.AddressFamily == AddressFamily.InterNetwork)
				{
					address2 = iPAddress;
				}
			}
			ServerIpAddress = ((address2 != null) ? address2.ToString() : (address + " not resolved"));
			return address2;
		}
	}
}
