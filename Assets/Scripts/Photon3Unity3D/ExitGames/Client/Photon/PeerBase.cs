using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Photon.SocketServer.Security;

namespace ExitGames.Client.Photon
{
	public abstract class PeerBase
	{
		internal delegate void MyAction();

		internal PhotonPeer photonPeer;

		public IProtocol SerializationProtocol;

		internal ConnectionProtocol usedTransportProtocol;

		internal IPhotonSocket PhotonSocket;

		internal ConnectionStateValue peerConnectionState;

		internal int ByteCountLastOperation;

		internal int ByteCountCurrentDispatch;

		internal NCommand CommandInCurrentDispatch;

		internal int packetLossByCrc;

		internal int packetLossByChallenge;

		internal readonly Queue<MyAction> ActionQueue = new Queue<MyAction>();

		internal short peerID = -1;

		internal int serverTimeOffset;

		internal bool serverTimeOffsetIsAvailable;

		internal int roundTripTime;

		internal int roundTripTimeVariance;

		internal int lastRoundTripTime;

		internal int lowestRoundTripTime;

		internal int highestRoundTripTimeVariance;

		internal int timestampOfLastReceive;

		internal static short peerCount;

		internal long bytesOut;

		internal long bytesIn;

		internal object CustomInitData;

		public string AppId;

		internal EventData reusableEventData;

		internal int timeBase;

		internal int timeoutInt;

		internal int timeLastAckReceive;

		internal int longestSentCall;

		internal int timeLastSendAck;

		internal int timeLastSendOutgoing;

		internal bool ApplicationIsInitialized;

		internal bool isEncryptionAvailable;

		internal int outgoingCommandsInStream = 0;

		protected internal static Queue<StreamBuffer> MessageBufferPool = new Queue<StreamBuffer>(32);

		internal ICryptoProvider CryptoProvider;

		private readonly Random lagRandomizer = new Random();

		internal readonly LinkedList<SimulationItem> NetSimListOutgoing = new LinkedList<SimulationItem>();

		internal readonly LinkedList<SimulationItem> NetSimListIncoming = new LinkedList<SimulationItem>();

		private readonly NetworkSimulationSet networkSimulationSettings = new NetworkSimulationSet();

		internal int TrafficPackageHeaderSize;

		private int commandLogSize;

		internal Queue<CmdLogItem> CommandLog;

		internal Queue<CmdLogItem> InReliableLog;

		internal Type SocketImplementation
		{
			get
			{
				return photonPeer.SocketImplementation;
			}
		}

		public string ServerAddress { get; internal set; }

		public string ProxyServerAddress { get; internal set; }

		internal IPhotonPeerListener Listener
		{
			get
			{
				return photonPeer.Listener;
			}
		}

		public DebugLevel debugOut
		{
			get
			{
				return photonPeer.DebugOut;
			}
		}

		internal int DisconnectTimeout
		{
			get
			{
				return photonPeer.DisconnectTimeout;
			}
		}

		internal int timePingInterval
		{
			get
			{
				return photonPeer.TimePingInterval;
			}
		}

		internal byte ChannelCount
		{
			get
			{
				return photonPeer.ChannelCount;
			}
		}

		internal long BytesOut
		{
			get
			{
				return bytesOut;
			}
		}

		internal long BytesIn
		{
			get
			{
				return bytesIn;
			}
		}

		internal abstract int QueuedIncomingCommandsCount { get; }

		internal abstract int QueuedOutgoingCommandsCount { get; }

		internal virtual int SentReliableCommandsCount
		{
			get
			{
				return 0;
			}
		}

		public virtual string PeerID
		{
			get
			{
				return ((ushort)peerID).ToString();
			}
		}

		internal int timeInt
		{
			get
			{
				return SupportClass.GetTickCount() - timeBase;
			}
		}

		internal static int outgoingStreamBufferSize
		{
			get
			{
				return PhotonPeer.OutgoingStreamBufferSize;
			}
		}

		internal bool IsSendingOnlyAcks
		{
			get
			{
				return photonPeer.IsSendingOnlyAcks;
			}
		}

		internal int mtu
		{
			get
			{
				return photonPeer.MaximumTransferUnit;
			}
		}

		protected internal bool IsIpv6
		{
			get
			{
				return PhotonSocket != null && PhotonSocket.AddressResolvedAsIpv6;
			}
		}

		public NetworkSimulationSet NetworkSimulationSettings
		{
			get
			{
				return networkSimulationSettings;
			}
		}

		internal bool TrafficStatsEnabled
		{
			get
			{
				return photonPeer.TrafficStatsEnabled;
			}
		}

		internal TrafficStats TrafficStatsIncoming
		{
			get
			{
				return photonPeer.TrafficStatsIncoming;
			}
		}

		internal TrafficStats TrafficStatsOutgoing
		{
			get
			{
				return photonPeer.TrafficStatsOutgoing;
			}
		}

		internal TrafficStatsGameLevel TrafficStatsGameLevel
		{
			get
			{
				return photonPeer.TrafficStatsGameLevel;
			}
		}

		internal int CommandLogSize
		{
			get
			{
				return commandLogSize;
			}
			set
			{
				commandLogSize = value;
				CommandLogResize();
			}
		}

		protected PeerBase()
		{
			networkSimulationSettings.peerBase = this;
			peerCount++;
		}

		public static StreamBuffer MessageBufferPoolGet()
		{
			lock (MessageBufferPool)
			{
				if (MessageBufferPool.Count > 0)
				{
					return MessageBufferPool.Dequeue();
				}
				return new StreamBuffer(75);
			}
		}

		public static void MessageBufferPoolPut(StreamBuffer buff)
		{
			buff.Position = 0;
			buff.SetLength(0L);
			lock (MessageBufferPool)
			{
				MessageBufferPool.Enqueue(buff);
			}
		}

		internal virtual void InitPeerBase()
		{
			SerializationProtocol = SerializationProtocolFactory.Create(photonPeer.SerializationProtocolType);
			photonPeer.InitializeTrafficStats();
			ByteCountLastOperation = 0;
			ByteCountCurrentDispatch = 0;
			bytesIn = 0L;
			bytesOut = 0L;
			packetLossByCrc = 0;
			packetLossByChallenge = 0;
			networkSimulationSettings.LostPackagesIn = 0;
			networkSimulationSettings.LostPackagesOut = 0;
			lock (NetSimListOutgoing)
			{
				NetSimListOutgoing.Clear();
			}
			lock (NetSimListIncoming)
			{
				NetSimListIncoming.Clear();
			}
			peerConnectionState = ConnectionStateValue.Disconnected;
			timeBase = SupportClass.GetTickCount();
			isEncryptionAvailable = false;
			ApplicationIsInitialized = false;
			roundTripTime = 200;
			roundTripTimeVariance = 5;
			serverTimeOffsetIsAvailable = false;
			serverTimeOffset = 0;
		}

		internal abstract bool Connect(string serverAddress, string appID, object customData = null);

		internal abstract bool Connect(string serverAddress, string proxyServerAddress, string appID, object customData);

		private string GetHttpKeyValueString(Dictionary<string, string> dic)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, string> item in dic)
			{
				stringBuilder.Append(item.Key).Append("=").Append(item.Value)
					.Append("&");
			}
			return stringBuilder.ToString();
		}

		internal byte[] PrepareConnectData(string serverAddress, string appID, object custom)
		{
			if (PhotonSocket == null || !PhotonSocket.Connected)
			{
				EnqueueDebugReturn(DebugLevel.WARNING, "The peer attempts to prepare an Init-Request but the socket is not connected!?");
			}
			if (custom == null)
			{
				byte[] array = new byte[41];
				byte[] clientVersion = Version.clientVersion;
				array[0] = 243;
				array[1] = 0;
				array[2] = SerializationProtocol.VersionBytes[0];
				array[3] = SerializationProtocol.VersionBytes[1];
				array[4] = photonPeer.ClientSdkIdShifted;
				array[5] = (byte)((byte)(clientVersion[0] << 4) | clientVersion[1]);
				array[6] = clientVersion[2];
				array[7] = clientVersion[3];
				array[8] = 0;
				if (string.IsNullOrEmpty(appID))
				{
					appID = "LoadBalancing";
				}
				for (int i = 0; i < 32; i++)
				{
                    array[i + 9] = (byte)((i < appID.Length) ? ((byte)appID[i]) : (byte)0);

                }
                if (IsIpv6)
				{
					array[5] |= 128;
				}
				else
				{
					array[5] &= 127;
				}
				return array;
			}
			if (custom != null)
			{
				byte[] array2 = null;
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary["init"] = null;
				dictionary["app"] = appID;
				dictionary["clientversion"] = photonPeer.ClientVersion;
				dictionary["protocol"] = SerializationProtocol.ProtocolType;
				dictionary["sid"] = photonPeer.ClientSdkIdShifted.ToString();
				byte[] array3 = null;
				int num = 0;
				if (custom != null)
				{
					array3 = SerializationProtocol.Serialize(custom);
					num += array3.Length;
				}
				string text = GetHttpKeyValueString(dictionary);
				if (IsIpv6)
				{
					text += "&IPv6";
				}
				string text2 = string.Format("POST /?{0} HTTP/1.1\r\nHost: {1}\r\nContent-Length: {2}\r\n\r\n", text, serverAddress, num);
				array2 = new byte[text2.Length + num];
				if (array3 != null)
				{
					Buffer.BlockCopy(array3, 0, array2, text2.Length, array3.Length);
				}
				Buffer.BlockCopy(Encoding.UTF8.GetBytes(text2), 0, array2, 0, text2.Length);
				return array2;
			}
			return null;
		}

		internal string PepareWebSocketUrl(string serverAddress, string appId, object customData)
		{
			StringBuilder stringBuilder = new StringBuilder(1024);
			string empty = string.Empty;
			if (customData != null)
			{
				byte[] array = SerializationProtocol.Serialize(customData);
				if (array == null)
				{
					EnqueueDebugReturn(DebugLevel.ERROR, "Can not deserialize custom data");
					return null;
				}
			}
			stringBuilder.AppendFormat("app={0}&clientver={1}&sid={2}&{3}&initobj={4}", appId, photonPeer.ClientVersion, photonPeer.ClientSdkIdShifted, IsIpv6 ? "IPv6" : string.Empty, empty);
			if (customData != null)
			{
				stringBuilder.Append("&xInit=");
			}
			return stringBuilder.ToString();
		}

		public abstract void OnConnect();

		internal void InitCallback()
		{
			if (peerConnectionState == ConnectionStateValue.Connecting)
			{
				peerConnectionState = ConnectionStateValue.Connected;
			}
			ApplicationIsInitialized = true;
			FetchServerTimestamp();
			Listener.OnStatusChanged(StatusCode.Connect);
		}

		internal abstract void Disconnect();

		internal abstract void StopConnection();

		internal abstract void FetchServerTimestamp();

		internal abstract bool EnqueueOperation(Dictionary<byte, object> parameters, byte opCode, SendOptions sendParams, EgMessageType messageType = EgMessageType.Operation);

		internal abstract StreamBuffer SerializeOperationToMessage(byte opCode, Dictionary<byte, object> parameters, EgMessageType messageType, bool encrypt);

		internal abstract bool EnqueueMessage(object message, SendOptions sendOptions);

		internal StreamBuffer SerializeMessageToMessage(object message, bool encrypt, byte[] messageHeader, bool writeLength = true)
		{
			bool flag = encrypt && usedTransportProtocol != ConnectionProtocol.WebSocketSecure;
			StreamBuffer streamBuffer = MessageBufferPoolGet();
			streamBuffer.SetLength(0L);
			if (!flag)
			{
				streamBuffer.Write(messageHeader, 0, messageHeader.Length);
			}
			if (message is byte[])
			{
				byte[] array = message as byte[];
				streamBuffer.Write(array, 0, array.Length);
			}
			else
			{
				SerializationProtocol.SerializeMessage(streamBuffer, message);
			}
			if (flag)
			{
				byte[] array2 = CryptoProvider.Encrypt(streamBuffer.GetBuffer(), 0, streamBuffer.Length);
				streamBuffer.SetLength(0L);
				streamBuffer.Write(messageHeader, 0, messageHeader.Length);
				streamBuffer.Write(array2, 0, array2.Length);
			}
			byte[] buffer = streamBuffer.GetBuffer();
			buffer[messageHeader.Length - 1] = (byte)((message is byte[]) ? 9 : 8);
			if (flag)
			{
				buffer[messageHeader.Length - 1] = (byte)(buffer[messageHeader.Length - 1] | 0x80u);
			}
			if (writeLength)
			{
				int targetOffset = 1;
				Protocol.Serialize(streamBuffer.Length, buffer, ref targetOffset);
			}
			return streamBuffer;
		}

		internal abstract bool SendOutgoingCommands();

		internal virtual bool SendAcksOnly()
		{
			return false;
		}

		internal abstract void ReceiveIncomingCommands(byte[] inBuff, int dataLength);

		internal abstract bool DispatchIncomingCommands();

		internal virtual bool DeserializeMessageAndCallback(StreamBuffer stream)
		{
			if (stream.Length < 2)
			{
				if ((int)debugOut >= 1)
				{
					Listener.DebugReturn(DebugLevel.ERROR, "Incoming UDP data too short! " + stream.Length);
				}
				return false;
			}
			byte b = stream.ReadByte();
			if (b != 243 && b != 253)
			{
				if ((int)debugOut >= 1)
				{
					Listener.DebugReturn(DebugLevel.ALL, "No regular operation UDP message: " + b);
				}
				return false;
			}
			byte b2 = stream.ReadByte();
			byte b3 = (byte)(b2 & 0x7Fu);
			bool flag = (b2 & 0x80) > 0;
			if (b3 != 1)
			{
				try
				{
					if (flag)
					{
						byte[] buf = CryptoProvider.Decrypt(stream.GetBuffer(), 2, stream.Length - 2);
						stream = new StreamBuffer(buf);
					}
					else
					{
						stream.Seek(2L, SeekOrigin.Begin);
					}
				}
				catch (Exception ex)
				{
					if ((int)debugOut >= 1)
					{
						Listener.DebugReturn(DebugLevel.ERROR, "msgType: " + b3 + " exception: " + ex.ToString());
					}
					SupportClass.WriteStackTrace(ex);
					return false;
				}
			}
			int num = 0;
			switch (b3)
			{
			case 3:
			{
				OperationResponse operationResponse = null;
				try
				{
					operationResponse = SerializationProtocol.DeserializeOperationResponse(stream);
				}
				catch (Exception ex4)
				{
					EnqueueDebugReturn(DebugLevel.ERROR, "Deserialization failed for Operation Response. " + ex4);
					return false;
				}
				if (TrafficStatsEnabled)
				{
					TrafficStatsGameLevel.CountResult(ByteCountCurrentDispatch);
					num = SupportClass.GetTickCount();
				}
				Listener.OnOperationResponse(operationResponse);
				if (TrafficStatsEnabled)
				{
					TrafficStatsGameLevel.TimeForResponseCallback(operationResponse.OperationCode, SupportClass.GetTickCount() - num);
				}
				break;
			}
			case 4:
			{
				EventData eventData = null;
				try
				{
					eventData = SerializationProtocol.DeserializeEventData(stream, reusableEventData);
				}
				catch (Exception ex2)
				{
					EnqueueDebugReturn(DebugLevel.ERROR, "Deserialization failed for Event. " + ex2);
					return false;
				}
				if (TrafficStatsEnabled)
				{
					TrafficStatsGameLevel.CountEvent(ByteCountCurrentDispatch);
					num = SupportClass.GetTickCount();
				}
				Listener.OnEvent(eventData);
				if (TrafficStatsEnabled)
				{
					TrafficStatsGameLevel.TimeForEventCallback(eventData.Code, SupportClass.GetTickCount() - num);
				}
				if (photonPeer.ReuseEventInstance)
				{
					reusableEventData = eventData;
				}
				break;
			}
			case 1:
				InitCallback();
				break;
			case 7:
			{
				OperationResponse operationResponse;
				try
				{
					operationResponse = SerializationProtocol.DeserializeOperationResponse(stream);
				}
				catch (Exception ex3)
				{
					EnqueueDebugReturn(DebugLevel.ERROR, "Deserialization failed for internal Operation Response. " + ex3);
					return false;
				}
				if (TrafficStatsEnabled)
				{
					TrafficStatsGameLevel.CountResult(ByteCountCurrentDispatch);
					num = SupportClass.GetTickCount();
				}
				if (operationResponse.OperationCode == PhotonCodes.InitEncryption)
				{
					DeriveSharedKey(operationResponse);
				}
				else if (operationResponse.OperationCode == PhotonCodes.Ping)
				{
					TPeer tPeer = this as TPeer;
					if (tPeer != null)
					{
						tPeer.ReadPingResult(operationResponse);
					}
				}
				else
				{
					EnqueueDebugReturn(DebugLevel.ERROR, "Received unknown internal operation. " + operationResponse.ToStringFull());
				}
				if (TrafficStatsEnabled)
				{
					TrafficStatsGameLevel.TimeForResponseCallback(operationResponse.OperationCode, SupportClass.GetTickCount() - num);
				}
				break;
			}
			case 8:
			{
				object obj = SerializationProtocol.DeserializeMessage(stream);
				if (TrafficStatsEnabled)
				{
					TrafficStatsGameLevel.CountEvent(ByteCountCurrentDispatch);
					num = SupportClass.GetTickCount();
				}
				if (TrafficStatsEnabled)
				{
					TrafficStatsGameLevel.TimeForMessageCallback(SupportClass.GetTickCount() - num);
				}
				break;
			}
			case 9:
			{
				if (TrafficStatsEnabled)
				{
					TrafficStatsGameLevel.CountEvent(ByteCountCurrentDispatch);
					num = SupportClass.GetTickCount();
				}
				byte[] array = stream.ToArrayFromPos();
				if (TrafficStatsEnabled)
				{
					TrafficStatsGameLevel.TimeForRawMessageCallback(SupportClass.GetTickCount() - num);
				}
				break;
			}
			default:
				EnqueueDebugReturn(DebugLevel.ERROR, "unexpected msgType " + b3);
				break;
			}
			return true;
		}

		internal void UpdateRoundTripTimeAndVariance(int lastRoundtripTime)
		{
			if (lastRoundtripTime >= 0)
			{
				roundTripTimeVariance -= roundTripTimeVariance / 4;
				if (lastRoundtripTime >= roundTripTime)
				{
					roundTripTime += (lastRoundtripTime - roundTripTime) / 8;
					roundTripTimeVariance += (lastRoundtripTime - roundTripTime) / 4;
				}
				else
				{
					roundTripTime += (lastRoundtripTime - roundTripTime) / 8;
					roundTripTimeVariance -= (lastRoundtripTime - roundTripTime) / 4;
				}
				if (roundTripTime < lowestRoundTripTime)
				{
					lowestRoundTripTime = roundTripTime;
				}
				if (roundTripTimeVariance > highestRoundTripTimeVariance)
				{
					highestRoundTripTimeVariance = roundTripTimeVariance;
				}
			}
		}

		internal bool ExchangeKeysForEncryption(object lockObject)
		{
			isEncryptionAvailable = false;
			if (CryptoProvider != null)
			{
				CryptoProvider.Dispose();
				CryptoProvider = null;
			}
			if (PhotonPeer.NativePayloadEncryptionLibAvailable)
			{
				CryptoProvider = new DiffieHellmanCryptoProviderNative();
			}
			if (CryptoProvider == null)
			{
				CryptoProvider = new DiffieHellmanCryptoProvider();
			}
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>(1);
			dictionary[PhotonCodes.ClientKey] = CryptoProvider.PublicKey;
			SendOptions sendOptions;
			if (lockObject != null)
			{
				lock (lockObject)
				{
					sendOptions = default(SendOptions);
					sendOptions.Channel = 0;
					sendOptions.Encrypt = false;
					sendOptions.DeliveryMode = DeliveryMode.Reliable;
					SendOptions sendParams = sendOptions;
					return EnqueueOperation(dictionary, PhotonCodes.InitEncryption, sendParams, EgMessageType.InternalOperationRequest);
				}
			}
			sendOptions = default(SendOptions);
			sendOptions.Channel = 0;
			sendOptions.Encrypt = false;
			sendOptions.DeliveryMode = DeliveryMode.Reliable;
			SendOptions sendParams2 = sendOptions;
			return EnqueueOperation(dictionary, PhotonCodes.InitEncryption, sendParams2, EgMessageType.InternalOperationRequest);
		}

		internal void DeriveSharedKey(OperationResponse operationResponse)
		{
			if (operationResponse.ReturnCode != 0)
			{
				EnqueueDebugReturn(DebugLevel.ERROR, "Establishing encryption keys failed. " + operationResponse.ToStringFull());
				EnqueueStatusCallback(StatusCode.EncryptionFailedToEstablish);
				return;
			}
			byte[] array = (byte[])operationResponse[PhotonCodes.ServerKey];
			if (array == null || array.Length == 0)
			{
				EnqueueDebugReturn(DebugLevel.ERROR, "Establishing encryption keys failed. Server's public key is null or empty. " + operationResponse.ToStringFull());
				EnqueueStatusCallback(StatusCode.EncryptionFailedToEstablish);
			}
			else
			{
				CryptoProvider.DeriveSharedKey(array);
				isEncryptionAvailable = true;
				EnqueueStatusCallback(StatusCode.EncryptionEstablished);
			}
		}

		internal virtual void InitEncryption(byte[] secret)
		{
			if (PhotonPeer.NativePayloadEncryptionLibAvailable)
			{
				CryptoProvider = new DiffieHellmanCryptoProviderNative(secret);
				isEncryptionAvailable = true;
			}
			if (CryptoProvider == null)
			{
				CryptoProvider = new DiffieHellmanCryptoProvider(secret);
				isEncryptionAvailable = true;
			}
		}

		internal void EnqueueActionForDispatch(MyAction action)
		{
			lock (ActionQueue)
			{
				ActionQueue.Enqueue(action);
			}
		}

		internal void EnqueueDebugReturn(DebugLevel level, string debugReturn)
		{
			lock (ActionQueue)
			{
				ActionQueue.Enqueue(delegate
				{
					Listener.DebugReturn(level, debugReturn);
				});
			}
		}

		internal void EnqueueStatusCallback(StatusCode statusValue)
		{
			lock (ActionQueue)
			{
				ActionQueue.Enqueue(delegate
				{
					Listener.OnStatusChanged(statusValue);
				});
			}
		}

		internal void SendNetworkSimulated(byte[] dataToSend)
		{
			if (!NetworkSimulationSettings.IsSimulationEnabled)
			{
				throw new NotImplementedException("SendNetworkSimulated was called, despite NetworkSimulationSettings.IsSimulationEnabled == false.");
			}
			if (usedTransportProtocol == ConnectionProtocol.Udp && NetworkSimulationSettings.OutgoingLossPercentage > 0 && lagRandomizer.Next(101) < NetworkSimulationSettings.OutgoingLossPercentage)
			{
				networkSimulationSettings.LostPackagesOut++;
				return;
			}
			int num = ((networkSimulationSettings.OutgoingJitter > 0) ? (lagRandomizer.Next(networkSimulationSettings.OutgoingJitter * 2) - networkSimulationSettings.OutgoingJitter) : 0);
			int num2 = networkSimulationSettings.OutgoingLag + num;
			int num3 = SupportClass.GetTickCount() + num2;
			SimulationItem value = new SimulationItem
			{
				DelayedData = dataToSend,
				TimeToExecute = num3,
				Delay = num2
			};
			lock (NetSimListOutgoing)
			{
				if (NetSimListOutgoing.Count == 0 || usedTransportProtocol == ConnectionProtocol.Tcp)
				{
					NetSimListOutgoing.AddLast(value);
					return;
				}
				LinkedListNode<SimulationItem> linkedListNode = NetSimListOutgoing.First;
				while (linkedListNode != null && linkedListNode.Value.TimeToExecute < num3)
				{
					linkedListNode = linkedListNode.Next;
				}
				if (linkedListNode == null)
				{
					NetSimListOutgoing.AddLast(value);
				}
				else
				{
					NetSimListOutgoing.AddBefore(linkedListNode, value);
				}
			}
		}

		internal void ReceiveNetworkSimulated(byte[] dataReceived)
		{
			if (!networkSimulationSettings.IsSimulationEnabled)
			{
				throw new NotImplementedException("ReceiveNetworkSimulated was called, despite NetworkSimulationSettings.IsSimulationEnabled == false.");
			}
			if (usedTransportProtocol == ConnectionProtocol.Udp && networkSimulationSettings.IncomingLossPercentage > 0 && lagRandomizer.Next(101) < networkSimulationSettings.IncomingLossPercentage)
			{
				networkSimulationSettings.LostPackagesIn++;
				return;
			}
			int num = ((networkSimulationSettings.IncomingJitter > 0) ? (lagRandomizer.Next(networkSimulationSettings.IncomingJitter * 2) - networkSimulationSettings.IncomingJitter) : 0);
			int num2 = networkSimulationSettings.IncomingLag + num;
			int num3 = SupportClass.GetTickCount() + num2;
			SimulationItem value = new SimulationItem
			{
				DelayedData = dataReceived,
				TimeToExecute = num3,
				Delay = num2
			};
			lock (NetSimListIncoming)
			{
				if (NetSimListIncoming.Count == 0 || usedTransportProtocol == ConnectionProtocol.Tcp)
				{
					NetSimListIncoming.AddLast(value);
					return;
				}
				LinkedListNode<SimulationItem> linkedListNode = NetSimListIncoming.First;
				while (linkedListNode != null && linkedListNode.Value.TimeToExecute < num3)
				{
					linkedListNode = linkedListNode.Next;
				}
				if (linkedListNode == null)
				{
					NetSimListIncoming.AddLast(value);
				}
				else
				{
					NetSimListIncoming.AddBefore(linkedListNode, value);
				}
			}
		}

		protected internal void NetworkSimRun()
		{
			while (true)
			{
				bool flag = false;
				lock (networkSimulationSettings.NetSimManualResetEvent)
				{
					flag = networkSimulationSettings.IsSimulationEnabled;
				}
				if (!flag)
				{
					networkSimulationSettings.NetSimManualResetEvent.WaitOne();
					continue;
				}
				lock (NetSimListIncoming)
				{
					SimulationItem simulationItem = null;
					while (NetSimListIncoming.First != null)
					{
						simulationItem = NetSimListIncoming.First.Value;
						if (simulationItem.stopw.ElapsedMilliseconds < simulationItem.Delay)
						{
							break;
						}
						ReceiveIncomingCommands(simulationItem.DelayedData, simulationItem.DelayedData.Length);
						NetSimListIncoming.RemoveFirst();
					}
				}
				lock (NetSimListOutgoing)
				{
					SimulationItem simulationItem2 = null;
					while (NetSimListOutgoing.First != null)
					{
						simulationItem2 = NetSimListOutgoing.First.Value;
						if (simulationItem2.stopw.ElapsedMilliseconds < simulationItem2.Delay)
						{
							break;
						}
						if (PhotonSocket != null && PhotonSocket.Connected)
						{
							PhotonSocket.Send(simulationItem2.DelayedData, simulationItem2.DelayedData.Length);
						}
						NetSimListOutgoing.RemoveFirst();
					}
				}
				Thread.Sleep(0);
			}
		}

		internal void CommandLogResize()
		{
			if (CommandLogSize <= 0)
			{
				CommandLog = null;
				InReliableLog = null;
				return;
			}
			if (CommandLog == null || InReliableLog == null)
			{
				CommandLogInit();
			}
			while (CommandLog.Count > 0 && CommandLog.Count > CommandLogSize)
			{
				CommandLog.Dequeue();
			}
			while (InReliableLog.Count > 0 && InReliableLog.Count > CommandLogSize)
			{
				InReliableLog.Dequeue();
			}
		}

		internal void CommandLogInit()
		{
			if (CommandLogSize <= 0)
			{
				CommandLog = null;
				InReliableLog = null;
			}
			else if (CommandLog == null || InReliableLog == null)
			{
				CommandLog = new Queue<CmdLogItem>(CommandLogSize);
				InReliableLog = new Queue<CmdLogItem>(CommandLogSize);
			}
			else
			{
				CommandLog.Clear();
				InReliableLog.Clear();
			}
		}

		public string CommandLogToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = ((usedTransportProtocol == ConnectionProtocol.Udp) ? ((EnetPeer)this).reliableCommandsRepeated : 0);
			stringBuilder.AppendFormat("PeerId: {0} Now: {1} Server: {2} State: {3} Total Resends: {4} Received {5}ms ago.\n", PeerID, timeInt, ServerAddress, peerConnectionState, num, SupportClass.GetTickCount() - timestampOfLastReceive);
			if (CommandLog == null)
			{
				return stringBuilder.ToString();
			}
			foreach (CmdLogItem item in CommandLog)
			{
				stringBuilder.AppendLine(item.ToString());
			}
			stringBuilder.AppendLine("Received Reliable Log: ");
			foreach (CmdLogItem item2 in InReliableLog)
			{
				stringBuilder.AppendLine(item2.ToString());
			}
			return stringBuilder.ToString();
		}
	}
}
