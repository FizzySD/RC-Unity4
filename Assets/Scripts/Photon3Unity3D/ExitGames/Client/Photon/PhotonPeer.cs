using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using ExitGames.Client.Photon.Encryption;
using Photon.SocketServer.Security;

namespace ExitGames.Client.Photon
{
	public class PhotonPeer
	{
		[Obsolete("Check QueuedOutgoingCommands and QueuedIncomingCommands on demand instead.")]
		public int WarningSize;

		public const bool NoSocket = false;

		[Obsolete("Where dynamic linking is available, this library will attempt to load it and fallback to a managed implementation. This value is always true.")]
		public const bool NativeDatagramEncrypt = true;

		public const bool DebugBuild = true;

		protected internal byte ClientSdkId = 15;

		private string clientVersion;

		private static bool checkedNativeLibs = false;

		private static bool useSocketNative;

		private static bool useDiffieHellmanCryptoProvider;

		private static bool useEncryptorNative;

		public Dictionary<ConnectionProtocol, Type> SocketImplementationConfig;

		public DebugLevel DebugOut = DebugLevel.ERROR;

		private bool reuseEventInstance;

		public bool SendInCreationOrder = true;

		private byte quickResendAttempts;

		public int RhttpMinConnections = 2;

		public int RhttpMaxConnections = 6;

		public byte ChannelCount = 2;

		public bool EnableEncryptedFlag = false;

		private bool crcEnabled;

		public int SentCountAllowance = 7;

		public int InitialResendTimeMax = 400;

		public int TimePingInterval = 1000;

		public int DisconnectTimeout = 10000;

		public static int OutgoingStreamBufferSize = 1200;

		private int mtu = 1200;

		public static bool AsyncKeyExchange = false;

		internal bool RandomizeSequenceNumbers;

		internal byte[] RandomizedSequenceNumbers;

		private Stopwatch trafficStatsStopwatch;

		private bool trafficStatsEnabled = false;

		internal PeerBase peerBase;

		private readonly object SendOutgoingLockObject = new object();

		private readonly object DispatchLockObject = new object();

		private readonly object EnqueueLock = new object();

		protected internal byte[] PayloadEncryptionSecret;

		private Type encryptorType;

		protected internal IPhotonEncryptor Encryptor;

		[Obsolete("See remarks.")]
		public int CommandBufferSize { get; set; }

		[Obsolete("See remarks.")]
		public int LimitOfUnreliableCommands { get; set; }

		[Obsolete("Should be replaced by: SupportClass.GetTickCount(). Internally this is used, too.")]
		public int LocalTimeInMilliSeconds
		{
			get
			{
				return SupportClass.GetTickCount();
			}
		}

		protected internal byte ClientSdkIdShifted
		{
			get
			{
				return (byte)((uint)(ClientSdkId << 1) | 0u);
			}
		}

		public string ClientVersion
		{
			get
			{
				if (string.IsNullOrEmpty(clientVersion))
				{
					clientVersion = string.Format("{0}.{1}.{2}.{3}", Version.clientVersion[0], Version.clientVersion[1], Version.clientVersion[2], Version.clientVersion[3]);
				}
				return clientVersion;
			}
		}

		public static bool NativeSocketLibAvailable
		{
			get
			{
				CheckNativeLibsAvailability();
				return useSocketNative;
			}
		}

		public static bool NativePayloadEncryptionLibAvailable
		{
			get
			{
				CheckNativeLibsAvailability();
				return useDiffieHellmanCryptoProvider;
			}
		}

		public static bool NativeDatagramEncryptionLibAvailable
		{
			get
			{
				CheckNativeLibsAvailability();
				return useEncryptorNative;
			}
		}

		public SerializationProtocol SerializationProtocolType { get; set; }

		public Type SocketImplementation { get; internal set; }

		public IPhotonPeerListener Listener { get; protected set; }

		public bool ReuseEventInstance
		{
			get
			{
				return reuseEventInstance;
			}
			set
			{
				lock (DispatchLockObject)
				{
					reuseEventInstance = value;
					if (!value)
					{
						peerBase.reusableEventData = null;
					}
				}
			}
		}

		public long BytesIn
		{
			get
			{
				return peerBase.BytesIn;
			}
		}

		public long BytesOut
		{
			get
			{
				return peerBase.BytesOut;
			}
		}

		public int ByteCountCurrentDispatch
		{
			get
			{
				return peerBase.ByteCountCurrentDispatch;
			}
		}

		public string CommandInfoCurrentDispatch
		{
			get
			{
				return (peerBase.CommandInCurrentDispatch != null) ? peerBase.CommandInCurrentDispatch.ToString() : string.Empty;
			}
		}

		public int ByteCountLastOperation
		{
			get
			{
				return peerBase.ByteCountLastOperation;
			}
		}

		public int CommandLogSize
		{
			get
			{
				return peerBase.CommandLogSize;
			}
			set
			{
				peerBase.CommandLogSize = value;
			}
		}

		public bool EnableServerTracing { get; set; }

		public byte QuickResendAttempts
		{
			get
			{
				return quickResendAttempts;
			}
			set
			{
				quickResendAttempts = value;
				if (quickResendAttempts > 4)
				{
					quickResendAttempts = 4;
				}
			}
		}

		public PeerStateValue PeerState
		{
			get
			{
				if (peerBase.peerConnectionState == ConnectionStateValue.Connected && !peerBase.ApplicationIsInitialized)
				{
					return PeerStateValue.InitializingApplication;
				}
				return (PeerStateValue)peerBase.peerConnectionState;
			}
		}

		public string PeerID
		{
			get
			{
				return peerBase.PeerID;
			}
		}

		public int QueuedIncomingCommands
		{
			get
			{
				return peerBase.QueuedIncomingCommandsCount;
			}
		}

		public int QueuedOutgoingCommands
		{
			get
			{
				return peerBase.QueuedOutgoingCommandsCount;
			}
		}

		public bool CrcEnabled
		{
			get
			{
				return crcEnabled;
			}
			set
			{
				if (crcEnabled != value)
				{
					if (peerBase.peerConnectionState != 0)
					{
						throw new Exception("CrcEnabled can only be set while disconnected.");
					}
					crcEnabled = value;
				}
			}
		}

		public int PacketLossByCrc
		{
			get
			{
				return peerBase.packetLossByCrc;
			}
		}

		public int PacketLossByChallenge
		{
			get
			{
				return peerBase.packetLossByChallenge;
			}
		}

		public int SentReliableCommandsCount
		{
			get
			{
				return peerBase.SentReliableCommandsCount;
			}
		}

		public int ResentReliableCommands
		{
			get
			{
				return (UsedProtocol == ConnectionProtocol.Udp) ? ((EnetPeer)peerBase).reliableCommandsRepeated : 0;
			}
		}

		public int ServerTimeInMilliSeconds
		{
			get
			{
				return peerBase.serverTimeOffsetIsAvailable ? (peerBase.serverTimeOffset + SupportClass.GetTickCount()) : 0;
			}
		}

		public SupportClass.IntegerMillisecondsDelegate LocalMsTimestampDelegate
		{
			set
			{
				if (PeerState != 0)
				{
					throw new Exception("LocalMsTimestampDelegate only settable while disconnected. State: " + PeerState);
				}
				SupportClass.IntegerMilliseconds = value;
			}
		}

		public int ConnectionTime
		{
			get
			{
				return peerBase.timeInt;
			}
		}

		public int LastSendAckTime
		{
			get
			{
				return peerBase.timeLastSendAck;
			}
		}

		public int LastSendOutgoingTime
		{
			get
			{
				return peerBase.timeLastSendOutgoing;
			}
		}

		public int LongestSentCall
		{
			get
			{
				return peerBase.longestSentCall;
			}
			set
			{
				peerBase.longestSentCall = value;
			}
		}

		public int RoundTripTime
		{
			get
			{
				return peerBase.roundTripTime;
			}
		}

		public int RoundTripTimeVariance
		{
			get
			{
				return peerBase.roundTripTimeVariance;
			}
		}

		public int LastRoundTripTime
		{
			get
			{
				return peerBase.lastRoundTripTime;
			}
		}

		public int TimestampOfLastSocketReceive
		{
			get
			{
				return peerBase.timestampOfLastReceive;
			}
		}

		public string ServerAddress
		{
			get
			{
				return peerBase.ServerAddress;
			}
			set
			{
				if ((int)DebugOut >= 1)
				{
					Listener.DebugReturn(DebugLevel.ERROR, "Failed to set ServerAddress. Can be set only when using HTTP.");
				}
			}
		}

		public string ServerIpAddress
		{
			get
			{
				return IPhotonSocket.ServerIpAddress;
			}
		}

		public ConnectionProtocol UsedProtocol
		{
			get
			{
				return peerBase.usedTransportProtocol;
			}
		}

		public ConnectionProtocol TransportProtocol { get; set; }

		public virtual bool IsSimulationEnabled
		{
			get
			{
				return NetworkSimulationSettings.IsSimulationEnabled;
			}
			set
			{
				if (value == NetworkSimulationSettings.IsSimulationEnabled)
				{
					return;
				}
				lock (SendOutgoingLockObject)
				{
					NetworkSimulationSettings.IsSimulationEnabled = value;
				}
			}
		}

		public NetworkSimulationSet NetworkSimulationSettings
		{
			get
			{
				return peerBase.NetworkSimulationSettings;
			}
		}

		public int MaximumTransferUnit
		{
			get
			{
				return mtu;
			}
			set
			{
				if (PeerState != 0)
				{
					throw new Exception("MaximumTransferUnit is only settable while disconnected. State: " + PeerState);
				}
				if (value < 576)
				{
					value = 576;
				}
				mtu = value;
			}
		}

		public bool IsEncryptionAvailable
		{
			get
			{
				return peerBase.isEncryptionAvailable;
			}
		}

		public bool IsSendingOnlyAcks { get; set; }

		public TrafficStats TrafficStatsIncoming { get; internal set; }

		public TrafficStats TrafficStatsOutgoing { get; internal set; }

		public TrafficStatsGameLevel TrafficStatsGameLevel { get; internal set; }

		public long TrafficStatsElapsedMs
		{
			get
			{
				return (trafficStatsStopwatch != null) ? trafficStatsStopwatch.ElapsedMilliseconds : 0;
			}
		}

		public bool TrafficStatsEnabled
		{
			get
			{
				return trafficStatsEnabled;
			}
			set
			{
				if (trafficStatsEnabled == value)
				{
					return;
				}
				trafficStatsEnabled = value;
				if (value)
				{
					if (trafficStatsStopwatch == null)
					{
						InitializeTrafficStats();
					}
					trafficStatsStopwatch.Start();
				}
				else if (trafficStatsStopwatch != null)
				{
					trafficStatsStopwatch.Stop();
				}
			}
		}

		public Type EncryptorType
		{
			get
			{
				return encryptorType;
			}
			set
			{
				bool flag = false;
				if (value == null || typeof(IPhotonEncryptor).IsAssignableFrom(value))
				{
					encryptorType = value;
				}
				else
				{
					Listener.DebugReturn(DebugLevel.WARNING, "Failed to set the EncryptorType. Type must implement IPhotonEncryptor.");
				}
			}
		}

		private static void CheckNativeLibsAvailability()
		{
			if (checkedNativeLibs)
			{
				return;
			}
			checkedNativeLibs = true;
			try
			{
				useDiffieHellmanCryptoProvider = false;
				Marshal.PrelinkAll(typeof(DiffieHellmanCryptoProviderNative));
				useDiffieHellmanCryptoProvider = true;
			}
			catch
			{
			}
			try
			{
				useSocketNative = false;
				Marshal.PrelinkAll(typeof(SocketNative));
				useSocketNative = true;
			}
			catch
			{
			}
			try
			{
				useEncryptorNative = false;
				Marshal.PrelinkAll(typeof(EncryptorNative));
				useEncryptorNative = true;
			}
			catch
			{
			}
		}

		public string CommandLogToString()
		{
			return peerBase.CommandLogToString();
		}

		public static void MessageBufferPoolTrim(int countOfBuffers)
		{
			lock (PeerBase.MessageBufferPool)
			{
				if (countOfBuffers <= 0)
				{
					PeerBase.MessageBufferPool.Clear();
				}
				else if (countOfBuffers < PeerBase.MessageBufferPool.Count)
				{
					while (PeerBase.MessageBufferPool.Count > countOfBuffers)
					{
						PeerBase.MessageBufferPool.Dequeue();
					}
					PeerBase.MessageBufferPool.TrimExcess();
				}
			}
		}

		public static int MessageBufferPoolSize()
		{
			return PeerBase.MessageBufferPool.Count;
		}

		public void TrafficStatsReset()
		{
			TrafficStatsEnabled = false;
			InitializeTrafficStats();
			TrafficStatsEnabled = true;
		}

		internal void InitializeTrafficStats()
		{
			TrafficStatsIncoming = new TrafficStats(peerBase.TrafficPackageHeaderSize);
			TrafficStatsOutgoing = new TrafficStats(peerBase.TrafficPackageHeaderSize);
			TrafficStatsGameLevel = new TrafficStatsGameLevel();
			if (trafficStatsStopwatch == null)
			{
				trafficStatsStopwatch = new Stopwatch();
			}
			else
			{
				trafficStatsStopwatch.Reset();
			}
			if (trafficStatsEnabled)
			{
				trafficStatsStopwatch.Start();
			}
		}

		public string VitalStatsToString(bool all)
		{
			string text = "";
			if (TrafficStatsGameLevel != null)
			{
				text = TrafficStatsGameLevel.ToStringVitalStats();
			}
			if (!all)
			{
				return string.Format("Rtt(variance): {0}({1}). Since receive: {2}ms. Longest send: {5}ms. Stats elapsed: {4}sec.\n{3}", RoundTripTime, RoundTripTimeVariance, SupportClass.GetTickCount() - TimestampOfLastSocketReceive, text, trafficStatsStopwatch.ElapsedMilliseconds / 1000, LongestSentCall);
			}
			return string.Format("Rtt(variance): {0}({1}). Since receive: {2}ms. Longest send: {7}ms. Stats elapsed: {6}sec.\n{3}\n{4}\n{5}", RoundTripTime, RoundTripTimeVariance, SupportClass.GetTickCount() - TimestampOfLastSocketReceive, text, TrafficStatsIncoming.ToString(), TrafficStatsOutgoing.ToString(), trafficStatsStopwatch.ElapsedMilliseconds / 1000, LongestSentCall);
		}

		public PhotonPeer(ConnectionProtocol protocolType)
		{
			TransportProtocol = protocolType;
			CreatePeerBase();
		}

		public PhotonPeer(IPhotonPeerListener listener, ConnectionProtocol protocolType)
			: this(protocolType)
		{
			Listener = listener;
		}

		public virtual bool Connect(string serverAddress, string applicationName)
		{
			return Connect(serverAddress, applicationName, null);
		}

		public virtual bool Connect(string serverAddress, string applicationName, object custom)
		{
			return Connect(serverAddress, null, applicationName, custom);
		}

		public virtual bool Connect(string serverAddress, string proxyServerAddress, string applicationName, object custom)
		{
			lock (DispatchLockObject)
			{
				lock (SendOutgoingLockObject)
				{
					CreatePeerBase();
					if (peerBase == null)
					{
						return false;
					}
					if (peerBase.SocketImplementation == null)
					{
						peerBase.EnqueueDebugReturn(DebugLevel.ERROR, string.Concat("Connect failed. SocketImplementationConfig is null for protocol ", TransportProtocol, ": ", SupportClass.DictionaryToString(SocketImplementationConfig)));
						return false;
					}
					if (custom == null)
					{
						Encryptor = null;
						RandomizedSequenceNumbers = null;
						RandomizeSequenceNumbers = false;
					}
					peerBase.CustomInitData = custom;
					peerBase.AppId = applicationName;
					return peerBase.Connect(serverAddress, proxyServerAddress, applicationName, custom);
				}
			}
		}

		private void CreatePeerBase()
		{
			if (SocketImplementationConfig == null)
			{
				SocketImplementationConfig = new Dictionary<ConnectionProtocol, Type>(5);
				if (NativeSocketLibAvailable)
				{
					Type typeFromHandle = typeof(SocketNative);
					SocketImplementationConfig.Add(ConnectionProtocol.Udp, typeFromHandle);
					SocketImplementationConfig.Add(ConnectionProtocol.Tcp, typeFromHandle);
					SocketImplementationConfig.Add(ConnectionProtocol.WebSocket, typeFromHandle);
					SocketImplementationConfig.Add(ConnectionProtocol.WebSocketSecure, typeFromHandle);
				}
				else
				{
					SocketImplementationConfig.Add(ConnectionProtocol.Udp, typeof(SocketUdp));
					SocketImplementationConfig.Add(ConnectionProtocol.Tcp, typeof(SocketTcp));
				}
			}
			switch (TransportProtocol)
			{
			case ConnectionProtocol.Tcp:
				peerBase = new TPeer();
				break;
			case ConnectionProtocol.WebSocket:
			case ConnectionProtocol.WebSocketSecure:
				peerBase = new TPeer
				{
					DoFraming = false
				};
				break;
			default:
				peerBase = new EnetPeer();
				break;
			}
			if (peerBase == null)
			{
				throw new Exception("No PeerBase");
			}
			peerBase.photonPeer = this;
			peerBase.usedTransportProtocol = TransportProtocol;
			Type value = null;
			SocketImplementationConfig.TryGetValue(TransportProtocol, out value);
			SocketImplementation = value;
		}

		public virtual void Disconnect()
		{
			lock (DispatchLockObject)
			{
				lock (SendOutgoingLockObject)
				{
					peerBase.Disconnect();
				}
			}
		}

		public virtual void StopThread()
		{
			lock (DispatchLockObject)
			{
				lock (SendOutgoingLockObject)
				{
					peerBase.StopConnection();
				}
			}
		}

		public virtual void FetchServerTimestamp()
		{
			peerBase.FetchServerTimestamp();
		}

		public bool EstablishEncryption()
		{
			if (AsyncKeyExchange)
			{
				SupportClass.StartBackgroundCalls(delegate
				{
					peerBase.ExchangeKeysForEncryption(SendOutgoingLockObject);
					return false;
				});
				return true;
			}
			return peerBase.ExchangeKeysForEncryption(SendOutgoingLockObject);
		}

		public bool InitDatagramEncryption(byte[] encryptionSecret, byte[] hmacSecret, bool randomizedSequenceNumbers = false, bool chaningModeGCM = false)
		{
			if (EncryptorType != null)
			{
				try
				{
					Encryptor = (IPhotonEncryptor)Activator.CreateInstance(EncryptorType);
					if (Encryptor == null)
					{
						Listener.DebugReturn(DebugLevel.WARNING, "Datagram encryptor creation by type failed, Activator.CreateInstance() returned null");
					}
				}
				catch (Exception ex)
				{
					Listener.DebugReturn(DebugLevel.WARNING, "Datagram encryptor creation by type failed: " + ex);
				}
			}
			if (Encryptor == null)
			{
				if (NativeDatagramEncryptionLibAvailable)
				{
					try
					{
						Encryptor = new EncryptorNative();
					}
					catch (Exception ex2)
					{
						Listener.DebugReturn(DebugLevel.WARNING, "Datagram encryptor creation of type EncryptorNative failed: " + ex2);
					}
				}
				if (Encryptor == null)
				{
					Encryptor = new EncryptorNet();
				}
			}
			if (Encryptor == null)
			{
				throw new NullReferenceException("Can not init datagram encryption. No suitable encryptor found or provided.");
			}
			Listener.DebugReturn(DebugLevel.INFO, string.Concat("Datagram encryptor of type ", Encryptor.GetType(), " created."));
			Listener.DebugReturn(DebugLevel.INFO, "Datagram encryptor initialization: GCM = " + chaningModeGCM + ", random seq num = " + randomizedSequenceNumbers);
			if (chaningModeGCM)
			{
				throw new NotImplementedException("DatagramEncryptionGCMRandomSequence is not supported by native Encoder API v1. Use DatagramEncryptionRandomSequence instead.");
			}
			Encryptor.Init(encryptionSecret, hmacSecret, null, chaningModeGCM);
			if (randomizedSequenceNumbers)
			{
				RandomizedSequenceNumbers = encryptionSecret;
				RandomizeSequenceNumbers = true;
			}
			return true;
		}

		public void InitPayloadEncryption(byte[] secret)
		{
			PayloadEncryptionSecret = secret;
		}

		public virtual void Service()
		{
			while (DispatchIncomingCommands())
			{
			}
			while (SendOutgoingCommands())
			{
			}
		}

		public virtual bool SendOutgoingCommands()
		{
			if (TrafficStatsEnabled)
			{
				TrafficStatsGameLevel.SendOutgoingCommandsCalled();
			}
			lock (SendOutgoingLockObject)
			{
				return peerBase.SendOutgoingCommands();
			}
		}

		public virtual bool SendAcksOnly()
		{
			if (TrafficStatsEnabled)
			{
				TrafficStatsGameLevel.SendOutgoingCommandsCalled();
			}
			lock (SendOutgoingLockObject)
			{
				return peerBase.SendAcksOnly();
			}
		}

		public virtual bool DispatchIncomingCommands()
		{
			if (TrafficStatsEnabled)
			{
				TrafficStatsGameLevel.DispatchIncomingCommandsCalled();
			}
			lock (DispatchLockObject)
			{
				peerBase.ByteCountCurrentDispatch = 0;
				return peerBase.DispatchIncomingCommands();
			}
		}

		public virtual bool SendOperation(byte operationCode, Dictionary<byte, object> operationParameters, SendOptions sendOptions)
		{
			if (sendOptions.Encrypt && !IsEncryptionAvailable && peerBase.usedTransportProtocol != ConnectionProtocol.WebSocketSecure)
			{
				throw new ArgumentException("Can't use encryption yet. Exchange keys first.");
			}
			lock (EnqueueLock)
			{
				return peerBase.EnqueueOperation(operationParameters, operationCode, sendOptions);
			}
		}

		[Obsolete("Use SendOperation() or SendMessage().")]
		public virtual bool OpCustom(byte customOpCode, Dictionary<byte, object> customOpParameters, bool sendReliable, byte channelId = 0, bool encrypt = false)
		{
			if (encrypt && !IsEncryptionAvailable && peerBase.usedTransportProtocol != ConnectionProtocol.WebSocketSecure)
			{
				throw new ArgumentException("Can't use encryption yet. Exchange keys first.");
			}
			lock (EnqueueLock)
			{
				SendOptions sendOptions = default(SendOptions);
				sendOptions.Channel = channelId;
				sendOptions.Encrypt = encrypt;
				sendOptions.DeliveryMode = (sendReliable ? DeliveryMode.Reliable : DeliveryMode.Unreliable);
				SendOptions sendParams = sendOptions;
				return peerBase.EnqueueOperation(customOpParameters, customOpCode, sendParams);
			}
		}

		[Obsolete("Use SendOperation() or SendMessage().")]
		public virtual bool OpCustom(OperationRequest operationRequest, bool sendReliable, byte channelId, bool encrypt)
		{
			if (encrypt && !IsEncryptionAvailable && peerBase.usedTransportProtocol != ConnectionProtocol.WebSocketSecure)
			{
				throw new ArgumentException("Can't use encryption yet. Exchange keys first.");
			}
			lock (EnqueueLock)
			{
				SendOptions sendOptions = default(SendOptions);
				sendOptions.Channel = channelId;
				sendOptions.Encrypt = encrypt;
				sendOptions.DeliveryMode = (sendReliable ? DeliveryMode.Reliable : DeliveryMode.Unreliable);
				SendOptions sendParams = sendOptions;
				return peerBase.EnqueueOperation(operationRequest.Parameters, operationRequest.OperationCode, sendParams);
			}
		}

		public static bool RegisterType(Type customType, byte code, SerializeMethod serializeMethod, DeserializeMethod constructor)
		{
			return Protocol.TryRegisterType(customType, code, serializeMethod, constructor);
		}

		public static bool RegisterType(Type customType, byte code, SerializeStreamMethod serializeMethod, DeserializeStreamMethod constructor)
		{
			return Protocol.TryRegisterType(customType, code, serializeMethod, constructor);
		}
	}
}
