using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ExitGames.Client.Photon
{
	internal class EnetPeer : PeerBase
	{
		private const int CRC_LENGTH = 4;

		protected internal const int HMAC_SIZE = 32;

		protected internal const int BLOCK_SIZE = 16;

		protected internal const int IV_SIZE = 16;

		private const int EncryptedDataGramHeaderSize = 7;

		private const int EncryptedHeaderSize = 5;

		private List<NCommand> sentReliableCommands = new List<NCommand>();

		private StreamBuffer outgoingAcknowledgementsPool;

		internal const int UnsequencedWindowSize = 128;

		internal readonly int[] unsequencedWindow = new int[4];

		internal int outgoingUnsequencedGroupNumber;

		internal int incomingUnsequencedGroupNumber;

		private byte udpCommandCount;

		private byte[] udpBuffer;

		private int udpBufferIndex;

		private int udpBufferLength;

		private byte[] bufferForEncryption;

		private int commandBufferSize = 100;

		internal int challenge;

		internal int reliableCommandsRepeated;

		internal int reliableCommandsSent;

		internal int serverSentTime;

		internal static readonly byte[] udpHeader0xF3 = new byte[2] { 243, 2 };

		internal static readonly byte[] messageHeader = udpHeader0xF3;

		protected bool datagramEncryptedConnection;

		private EnetChannel[] channelArray = new EnetChannel[0];

		private const byte ControlChannelNumber = byte.MaxValue;

		protected internal const short PeerIdForConnect = -1;

		protected internal const short PeerIdForConnectTrace = -2;

		private Queue<int> commandsToRemove = new Queue<int>();

		private int fragmentLength = 0;

		private int fragmentLengthDatagramEncrypt = 0;

		private int fragmentLengthMtuValue = 0;

		private Queue<NCommand> commandsToResend = new Queue<NCommand>();

		private Queue<NCommand> CommandQueue = new Queue<NCommand>();

		internal override int QueuedIncomingCommandsCount
		{
			get
			{
				int num = 0;
				lock (channelArray)
				{
					for (int i = 0; i < channelArray.Length; i++)
					{
						EnetChannel enetChannel = channelArray[i];
						num += enetChannel.incomingReliableCommandsList.Count;
						num += enetChannel.incomingUnreliableCommandsList.Count;
					}
				}
				return num;
			}
		}

		internal override int QueuedOutgoingCommandsCount
		{
			get
			{
				int num = 0;
				lock (channelArray)
				{
					for (int i = 0; i < channelArray.Length; i++)
					{
						EnetChannel enetChannel = channelArray[i];
						num += enetChannel.outgoingReliableCommandsList.Count;
						num += enetChannel.outgoingUnreliableCommandsList.Count;
					}
				}
				return num;
			}
		}

		internal override int SentReliableCommandsCount
		{
			get
			{
				return sentReliableCommands.Count;
			}
		}

		internal EnetPeer()
		{
			TrafficPackageHeaderSize = 12;
		}

		internal override void InitPeerBase()
		{
			base.InitPeerBase();
			if (photonPeer.PayloadEncryptionSecret != null && usedTransportProtocol == ConnectionProtocol.Udp)
			{
				InitEncryption(photonPeer.PayloadEncryptionSecret);
			}
			if (photonPeer.Encryptor != null)
			{
				isEncryptionAvailable = true;
			}
			peerID = (short)(photonPeer.EnableServerTracing ? (-2) : (-1));
			challenge = SupportClass.ThreadSafeRandom.Next();
			if (udpBuffer == null || udpBuffer.Length != base.mtu)
			{
				udpBuffer = new byte[base.mtu];
			}
			reliableCommandsSent = 0;
			reliableCommandsRepeated = 0;
			lock (channelArray)
			{
				EnetChannel[] array = channelArray;
				if (array.Length != base.ChannelCount + 1)
				{
					array = new EnetChannel[base.ChannelCount + 1];
				}
				for (byte b = 0; b < base.ChannelCount; b = (byte)(b + 1))
				{
					array[b] = new EnetChannel(b, commandBufferSize);
				}
				array[base.ChannelCount] = new EnetChannel(byte.MaxValue, commandBufferSize);
				channelArray = array;
			}
			lock (sentReliableCommands)
			{
				sentReliableCommands = new List<NCommand>(commandBufferSize);
			}
			outgoingAcknowledgementsPool = new StreamBuffer();
			CommandLogInit();
		}

		internal void ApplyRandomizedSequenceNumbers()
		{
			if (!photonPeer.RandomizeSequenceNumbers)
			{
				return;
			}
			lock (channelArray)
			{
				EnetChannel[] array = channelArray;
				foreach (EnetChannel enetChannel in array)
				{
					int num = photonPeer.RandomizedSequenceNumbers[(int)enetChannel.ChannelNumber % photonPeer.RandomizedSequenceNumbers.Length];
					string debugReturn = string.Format("Channel {0} seqNr in: {1} out: {2}. randomize value: {3}", enetChannel.ChannelNumber, enetChannel.incomingReliableSequenceNumber, enetChannel.outgoingReliableSequenceNumber, num);
					EnqueueDebugReturn(DebugLevel.INFO, debugReturn);
					enetChannel.incomingReliableSequenceNumber = num;
					enetChannel.outgoingReliableSequenceNumber = num;
					enetChannel.outgoingReliableUnsequencedNumber = num;
				}
			}
		}

		internal override bool Connect(string ipport, string appID, object custom = null)
		{
			return Connect(ipport, null, appID, custom);
		}

		internal override bool Connect(string ipport, string proxyServerAddress, string appID, object custom)
		{
			if (peerConnectionState != 0)
			{
				base.Listener.DebugReturn(DebugLevel.WARNING, "Connect() can't be called if peer is not Disconnected. Not connecting. peerConnectionState: " + peerConnectionState);
				return false;
			}
			if ((int)base.debugOut >= 5)
			{
				base.Listener.DebugReturn(DebugLevel.ALL, "Connect()");
			}
			base.ServerAddress = ipport;
			base.ProxyServerAddress = proxyServerAddress;
			InitPeerBase();
			if (base.SocketImplementation != null)
			{
				PhotonSocket = (IPhotonSocket)Activator.CreateInstance(base.SocketImplementation, this);
			}
			else
			{
				PhotonSocket = new SocketUdp(this);
			}
			if (PhotonSocket == null)
			{
				base.Listener.DebugReturn(DebugLevel.ERROR, "Connect() failed, because SocketImplementation or socket was null. Set PhotonPeer.SocketImplementation before Connect().");
				return false;
			}
			if (PhotonSocket.Connect())
			{
				if (base.TrafficStatsEnabled)
				{
					base.TrafficStatsOutgoing.ControlCommandBytes += 44;
					base.TrafficStatsOutgoing.ControlCommandCount++;
				}
				peerConnectionState = ConnectionStateValue.Connecting;
				return true;
			}
			return false;
		}

		public override void OnConnect()
		{
			QueueOutgoingReliableCommand(new NCommand(this, 2, null, byte.MaxValue));
		}

		internal override void Disconnect()
		{
			if (peerConnectionState == ConnectionStateValue.Disconnected || peerConnectionState == ConnectionStateValue.Disconnecting)
			{
				return;
			}
			if (sentReliableCommands != null)
			{
				lock (sentReliableCommands)
				{
					sentReliableCommands.Clear();
				}
			}
			lock (channelArray)
			{
				EnetChannel[] array = channelArray;
				foreach (EnetChannel enetChannel in array)
				{
					enetChannel.clearAll();
				}
			}
			bool isSimulationEnabled = base.NetworkSimulationSettings.IsSimulationEnabled;
			base.NetworkSimulationSettings.IsSimulationEnabled = false;
			NCommand nCommand = new NCommand(this, 4, null, byte.MaxValue);
			QueueOutgoingReliableCommand(nCommand);
			SendOutgoingCommands();
			if (base.TrafficStatsEnabled)
			{
				base.TrafficStatsOutgoing.CountControlCommand(nCommand.Size);
			}
			base.NetworkSimulationSettings.IsSimulationEnabled = isSimulationEnabled;
			PhotonSocket.Disconnect();
			peerConnectionState = ConnectionStateValue.Disconnected;
			EnqueueStatusCallback(StatusCode.Disconnect);
			datagramEncryptedConnection = false;
		}

		internal override void StopConnection()
		{
			if (PhotonSocket != null)
			{
				PhotonSocket.Disconnect();
			}
			peerConnectionState = ConnectionStateValue.Disconnected;
			if (base.Listener != null)
			{
				base.Listener.OnStatusChanged(StatusCode.Disconnect);
			}
		}

		internal override void FetchServerTimestamp()
		{
			if (peerConnectionState != ConnectionStateValue.Connected || !ApplicationIsInitialized)
			{
				if ((int)base.debugOut >= 3)
				{
					EnqueueDebugReturn(DebugLevel.INFO, "FetchServerTimestamp() was skipped, as the client is not connected. Current ConnectionState: " + peerConnectionState);
				}
			}
			else
			{
				CreateAndEnqueueCommand(12, null, byte.MaxValue);
			}
		}

		internal override bool DispatchIncomingCommands()
		{
			int count = CommandQueue.Count;
			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					lock (CommandQueue)
					{
						NCommand command = CommandQueue.Dequeue();
						ExecuteCommand(command);
					}
				}
			}
			while (true)
			{
				MyAction myAction;
				lock (ActionQueue)
				{
					if (ActionQueue.Count <= 0)
					{
						break;
					}
					myAction = ActionQueue.Dequeue();
					goto IL_00a8;
				}
				IL_00a8:
				myAction();
			}
			NCommand value = null;
			lock (channelArray)
			{
				for (int j = 0; j < channelArray.Length; j++)
				{
					EnetChannel enetChannel = channelArray[j];
					if (enetChannel.incomingUnsequencedCommandsList.Count > 0)
					{
						value = enetChannel.incomingUnsequencedCommandsList.Dequeue();
						break;
					}
					if (enetChannel.incomingUnreliableCommandsList.Count > 0)
					{
						int num = int.MaxValue;
						foreach (int key in enetChannel.incomingUnreliableCommandsList.Keys)
						{
							NCommand nCommand = enetChannel.incomingUnreliableCommandsList[key];
							if (key < enetChannel.incomingUnreliableSequenceNumber || nCommand.reliableSequenceNumber < enetChannel.incomingReliableSequenceNumber)
							{
								commandsToRemove.Enqueue(key);
							}
							else if (key < num && nCommand.reliableSequenceNumber <= enetChannel.incomingReliableSequenceNumber)
							{
								num = key;
							}
						}
						while (commandsToRemove.Count > 0)
						{
							enetChannel.incomingUnreliableCommandsList.Remove(commandsToRemove.Dequeue());
						}
						if (num < int.MaxValue)
						{
							value = enetChannel.incomingUnreliableCommandsList[num];
						}
						if (value != null)
						{
							enetChannel.incomingUnreliableCommandsList.Remove(value.unreliableSequenceNumber);
							enetChannel.incomingUnreliableSequenceNumber = value.unreliableSequenceNumber;
							break;
						}
					}
					if (value != null || enetChannel.incomingReliableCommandsList.Count <= 0)
					{
						continue;
					}
					enetChannel.incomingReliableCommandsList.TryGetValue(enetChannel.incomingReliableSequenceNumber + 1, out value);
					if (value != null)
					{
						if (value.commandType != 8)
						{
							enetChannel.incomingReliableSequenceNumber = value.reliableSequenceNumber;
							enetChannel.incomingReliableCommandsList.Remove(value.reliableSequenceNumber);
						}
						else if (value.fragmentsRemaining > 0)
						{
							value = null;
						}
						else
						{
							enetChannel.incomingReliableSequenceNumber = value.reliableSequenceNumber + value.fragmentCount - 1;
							enetChannel.incomingReliableCommandsList.Remove(value.reliableSequenceNumber);
						}
						break;
					}
				}
			}
			if (value != null && value.Payload != null)
			{
				ByteCountCurrentDispatch = value.Size;
				CommandInCurrentDispatch = value;
				bool flag = DeserializeMessageAndCallback(value.Payload);
				value.FreePayload();
				CommandInCurrentDispatch = null;
				return true;
			}
			return false;
		}

		private int GetFragmentLength()
		{
			if (fragmentLength == 0 || base.mtu != fragmentLengthMtuValue)
			{
				fragmentLengthMtuValue = base.mtu;
				fragmentLength = base.mtu - 12 - 36;
				int num = base.mtu;
				num = num - 7 - 32 - 16;
				num = num / 16 * 16;
				num = num - 5 - 36;
				fragmentLengthDatagramEncrypt = num;
			}
			return datagramEncryptedConnection ? fragmentLengthDatagramEncrypt : fragmentLength;
		}

		private int CalculateBufferLen()
		{
			int num = base.mtu;
			if (datagramEncryptedConnection)
			{
				num = num - 7 - 32 - 16;
				num = num / 16 * 16;
				return num - 1;
			}
			return num;
		}

		private int CalculateInitialOffset()
		{
			if (datagramEncryptedConnection)
			{
				return 5;
			}
			int num = 12;
			if (photonPeer.CrcEnabled)
			{
				num += 4;
			}
			return num;
		}

		internal override bool SendAcksOnly()
		{
			if (peerConnectionState == ConnectionStateValue.Disconnected)
			{
				return false;
			}
			if (PhotonSocket == null || !PhotonSocket.Connected)
			{
				return false;
			}
			lock (udpBuffer)
			{
				int num = 0;
				udpBufferIndex = CalculateInitialOffset();
				udpBufferLength = CalculateBufferLen();
				udpCommandCount = 0;
				lock (outgoingAcknowledgementsPool)
				{
					num = SerializeAckToBuffer();
					timeLastSendAck = base.timeInt;
				}
				if (base.timeInt > timeoutInt && sentReliableCommands.Count > 0)
				{
					lock (sentReliableCommands)
					{
						foreach (NCommand sentReliableCommand in sentReliableCommands)
						{
							if (sentReliableCommand != null && sentReliableCommand.roundTripTimeout != 0 && base.timeInt - sentReliableCommand.commandSentTime > sentReliableCommand.roundTripTimeout)
							{
								sentReliableCommand.commandSentCount = 1;
								sentReliableCommand.roundTripTimeout = 0;
								sentReliableCommand.timeoutTime = int.MaxValue;
								sentReliableCommand.commandSentTime = base.timeInt;
							}
						}
					}
				}
				if (udpCommandCount <= 0)
				{
					return false;
				}
				if (base.TrafficStatsEnabled)
				{
					base.TrafficStatsOutgoing.TotalPacketCount++;
					base.TrafficStatsOutgoing.TotalCommandsInPackets += udpCommandCount;
				}
				SendData(udpBuffer, udpBufferIndex);
				return num > 0;
			}
		}

		internal override bool SendOutgoingCommands()
		{
			if (peerConnectionState == ConnectionStateValue.Disconnected)
			{
				return false;
			}
			if (!PhotonSocket.Connected)
			{
				return false;
			}
			lock (udpBuffer)
			{
				int num = 0;
				udpBufferIndex = CalculateInitialOffset();
				udpBufferLength = CalculateBufferLen();
				udpCommandCount = 0;
				timeLastSendOutgoing = base.timeInt;
				lock (outgoingAcknowledgementsPool)
				{
					if (outgoingAcknowledgementsPool.Length > 0)
					{
						num = SerializeAckToBuffer();
						timeLastSendAck = base.timeInt;
					}
				}
				if (!base.IsSendingOnlyAcks && base.timeInt > timeoutInt && sentReliableCommands.Count > 0)
				{
					lock (sentReliableCommands)
					{
						commandsToResend.Clear();
						for (int i = 0; i < sentReliableCommands.Count; i++)
						{
							NCommand nCommand = sentReliableCommands[i];
							if (nCommand == null || base.timeInt - nCommand.commandSentTime <= nCommand.roundTripTimeout)
							{
								continue;
							}
							if (nCommand.commandSentCount > photonPeer.SentCountAllowance || base.timeInt > nCommand.timeoutTime)
							{
								if ((int)base.debugOut >= 2)
								{
									base.Listener.DebugReturn(DebugLevel.WARNING, string.Concat("Timeout-disconnect! Command: ", nCommand, " now: ", base.timeInt, " challenge: ", Convert.ToString(challenge, 16)));
								}
								if (CommandLog != null)
								{
									CommandLog.Enqueue(new CmdLogSentReliable(nCommand, base.timeInt, roundTripTime, roundTripTimeVariance, true));
									CommandLogResize();
								}
								peerConnectionState = ConnectionStateValue.Zombie;
								EnqueueStatusCallback(StatusCode.TimeoutDisconnect);
								Disconnect();
								return false;
							}
							commandsToResend.Enqueue(nCommand);
						}
						while (commandsToResend.Count > 0)
						{
							NCommand nCommand2 = commandsToResend.Dequeue();
							QueueOutgoingReliableCommand(nCommand2);
							sentReliableCommands.Remove(nCommand2);
							reliableCommandsRepeated++;
							if ((int)base.debugOut >= 3)
							{
								base.Listener.DebugReturn(DebugLevel.INFO, string.Format("Resending: {0}. times out after: {1} sent: {3} now: {2} rtt/var: {4}/{5} last recv: {6}", nCommand2, nCommand2.roundTripTimeout, base.timeInt, nCommand2.commandSentTime, roundTripTime, roundTripTimeVariance, SupportClass.GetTickCount() - timestampOfLastReceive));
							}
						}
					}
				}
				if (!base.IsSendingOnlyAcks && peerConnectionState == ConnectionStateValue.Connected && base.timePingInterval > 0 && sentReliableCommands.Count == 0 && base.timeInt - timeLastAckReceive > base.timePingInterval && !AreReliableCommandsInTransit() && udpBufferIndex + 12 < udpBufferLength)
				{
					NCommand nCommand3 = new NCommand(this, 5, null, byte.MaxValue);
					QueueOutgoingReliableCommand(nCommand3);
					if (base.TrafficStatsEnabled)
					{
						base.TrafficStatsOutgoing.CountControlCommand(nCommand3.Size);
					}
				}
				if (!base.IsSendingOnlyAcks)
				{
					lock (channelArray)
					{
						for (int j = 0; j < channelArray.Length; j++)
						{
							EnetChannel enetChannel = channelArray[j];
							num += SerializeToBuffer(enetChannel.outgoingReliableCommandsList);
							num += SerializeToBuffer(enetChannel.outgoingUnreliableCommandsList);
						}
					}
				}
				if (udpCommandCount <= 0)
				{
					return false;
				}
				if (base.TrafficStatsEnabled)
				{
					base.TrafficStatsOutgoing.TotalPacketCount++;
					base.TrafficStatsOutgoing.TotalCommandsInPackets += udpCommandCount;
				}
				SendData(udpBuffer, udpBufferIndex);
				return num > 0;
			}
		}

		private bool AreReliableCommandsInTransit()
		{
			lock (channelArray)
			{
				for (int i = 0; i < channelArray.Length; i++)
				{
					EnetChannel enetChannel = channelArray[i];
					if (enetChannel.outgoingReliableCommandsList.Count > 0)
					{
						return true;
					}
				}
			}
			return false;
		}

		internal override bool EnqueueOperation(Dictionary<byte, object> parameters, byte opCode, SendOptions sendParams, EgMessageType messageType = EgMessageType.Operation)
		{
			if (peerConnectionState != ConnectionStateValue.Connected)
			{
				if ((int)base.debugOut >= 1)
				{
					base.Listener.DebugReturn(DebugLevel.ERROR, "Cannot send op: " + opCode + " Not connected. PeerState: " + peerConnectionState);
				}
				base.Listener.OnStatusChanged(StatusCode.SendError);
				return false;
			}
			if (sendParams.Channel >= base.ChannelCount)
			{
				if ((int)base.debugOut >= 1)
				{
					base.Listener.DebugReturn(DebugLevel.ERROR, "Cannot send op: Selected channel (" + sendParams.Channel + ")>= channelCount (" + base.ChannelCount + ").");
				}
				base.Listener.OnStatusChanged(StatusCode.SendError);
				return false;
			}
			byte commandType = 7;
			if (sendParams.DeliveryMode == DeliveryMode.UnreliableUnsequenced)
			{
				commandType = 11;
			}
			else if (sendParams.DeliveryMode == DeliveryMode.ReliableUnsequenced)
			{
				commandType = 14;
			}
			else if (sendParams.DeliveryMode == DeliveryMode.Reliable)
			{
				commandType = 6;
			}
			StreamBuffer payload = SerializeOperationToMessage(opCode, parameters, messageType, sendParams.Encrypt);
			return CreateAndEnqueueCommand(commandType, payload, sendParams.Channel);
		}

		internal override bool EnqueueMessage(object message, SendOptions sendOptions)
		{
			if (peerConnectionState != ConnectionStateValue.Connected)
			{
				if ((int)base.debugOut >= 1)
				{
					base.Listener.DebugReturn(DebugLevel.ERROR, "Cannot send message! Not connected. PeerState: " + peerConnectionState);
				}
				base.Listener.OnStatusChanged(StatusCode.SendError);
				return false;
			}
			byte channel = sendOptions.Channel;
			if (channel >= base.ChannelCount)
			{
				if ((int)base.debugOut >= 1)
				{
					base.Listener.DebugReturn(DebugLevel.ERROR, "Cannot send op: Selected channel (" + channel + ")>= channelCount (" + base.ChannelCount + ").");
				}
				base.Listener.OnStatusChanged(StatusCode.SendError);
				return false;
			}
			byte commandType = 7;
			if (sendOptions.DeliveryMode == DeliveryMode.UnreliableUnsequenced)
			{
				commandType = 11;
			}
			else if (sendOptions.DeliveryMode == DeliveryMode.ReliableUnsequenced)
			{
				commandType = 14;
			}
			else if (sendOptions.DeliveryMode == DeliveryMode.Reliable)
			{
				commandType = 6;
			}
			StreamBuffer payload = SerializeMessageToMessage(message, sendOptions.Encrypt, messageHeader, false);
			return CreateAndEnqueueCommand(commandType, payload, channel);
		}

		private EnetChannel GetChannel(byte channelNumber)
		{
			return (channelNumber == byte.MaxValue) ? channelArray[channelArray.Length - 1] : channelArray[channelNumber];
		}

		internal bool CreateAndEnqueueCommand(byte commandType, StreamBuffer payload, byte channelNumber)
		{
			EnetChannel channel = GetChannel(channelNumber);
			ByteCountLastOperation = 0;
			int num = GetFragmentLength();
			if (num == 0)
			{
				num = 1000;
				EnqueueDebugReturn(DebugLevel.WARNING, "Value of currentFragmentSize should not be 0. Corrected to 1000.");
			}
			if (payload == null || payload.Length <= num)
			{
				NCommand nCommand = new NCommand(this, commandType, payload, channel.ChannelNumber);
				if (nCommand.IsFlaggedReliable)
				{
					QueueOutgoingReliableCommand(nCommand);
					ByteCountLastOperation = nCommand.Size;
					if (base.TrafficStatsEnabled)
					{
						base.TrafficStatsOutgoing.CountReliableOpCommand(nCommand.Size);
						base.TrafficStatsGameLevel.CountOperation(nCommand.Size);
					}
				}
				else
				{
					QueueOutgoingUnreliableCommand(nCommand);
					ByteCountLastOperation = nCommand.Size;
					if (base.TrafficStatsEnabled)
					{
						base.TrafficStatsOutgoing.CountUnreliableOpCommand(nCommand.Size);
						base.TrafficStatsGameLevel.CountOperation(nCommand.Size);
					}
				}
			}
			else
			{
				bool flag = commandType == 14 || commandType == 11;
				int fragmentCount = (payload.Length + num - 1) / num;
				int startSequenceNumber = (flag ? channel.outgoingReliableUnsequencedNumber : channel.outgoingReliableSequenceNumber) + 1;
				byte[] buffer = payload.GetBuffer();
				int num2 = 0;
				for (int i = 0; i < payload.Length; i += num)
				{
					if (payload.Length - i < num)
					{
						num = payload.Length - i;
					}
					StreamBuffer streamBuffer = PeerBase.MessageBufferPoolGet();
					streamBuffer.Write(buffer, i, num);
					NCommand nCommand2 = new NCommand(this, (byte)(flag ? 15 : 8), streamBuffer, channel.ChannelNumber);
					nCommand2.fragmentNumber = num2;
					nCommand2.startSequenceNumber = startSequenceNumber;
					nCommand2.fragmentCount = fragmentCount;
					nCommand2.totalLength = payload.Length;
					nCommand2.fragmentOffset = i;
					QueueOutgoingReliableCommand(nCommand2);
					ByteCountLastOperation += nCommand2.Size;
					if (base.TrafficStatsEnabled)
					{
						base.TrafficStatsOutgoing.CountFragmentOpCommand(nCommand2.Size);
						base.TrafficStatsGameLevel.CountOperation(nCommand2.Size);
					}
					num2++;
				}
				PeerBase.MessageBufferPoolPut(payload);
			}
			return true;
		}

		internal override StreamBuffer SerializeOperationToMessage(byte opCode, Dictionary<byte, object> parameters, EgMessageType messageType, bool encrypt)
		{
			bool flag = encrypt && !datagramEncryptedConnection;
			StreamBuffer streamBuffer = PeerBase.MessageBufferPoolGet();
			streamBuffer.SetLength(0L);
			if (!flag)
			{
				streamBuffer.Write(messageHeader, 0, messageHeader.Length);
			}
			SerializationProtocol.SerializeOperationRequest(streamBuffer, opCode, parameters, false);
			if (flag)
			{
				byte[] array = CryptoProvider.Encrypt(streamBuffer.GetBuffer(), 0, streamBuffer.Length);
				streamBuffer.SetLength(0L);
				streamBuffer.Write(messageHeader, 0, messageHeader.Length);
				streamBuffer.Write(array, 0, array.Length);
			}
			byte[] buffer = streamBuffer.GetBuffer();
			if (messageType != EgMessageType.Operation)
			{
				buffer[messageHeader.Length - 1] = (byte)messageType;
			}
			if (flag || (encrypt && photonPeer.EnableEncryptedFlag))
			{
				buffer[messageHeader.Length - 1] = (byte)(buffer[messageHeader.Length - 1] | 0x80u);
			}
			return streamBuffer;
		}

		internal int SerializeAckToBuffer()
		{
			outgoingAcknowledgementsPool.Seek(0L, SeekOrigin.Begin);
			while (outgoingAcknowledgementsPool.Position + 20 <= outgoingAcknowledgementsPool.Length)
			{
				if (udpBufferIndex + 20 > udpBufferLength)
				{
					if ((int)base.debugOut >= 3)
					{
						base.Listener.DebugReturn(DebugLevel.INFO, "UDP package is full. Commands in Package: " + udpCommandCount + ". bytes left in queue: " + outgoingAcknowledgementsPool.Position);
					}
					break;
				}
				int offset;
				byte[] bufferAndAdvance = outgoingAcknowledgementsPool.GetBufferAndAdvance(20, out offset);
				Buffer.BlockCopy(bufferAndAdvance, offset, udpBuffer, udpBufferIndex, 20);
				udpBufferIndex += 20;
				udpCommandCount++;
			}
			outgoingAcknowledgementsPool.Compact();
			outgoingAcknowledgementsPool.Position = outgoingAcknowledgementsPool.Length;
			return outgoingAcknowledgementsPool.Length / 20;
		}

		internal int SerializeToBuffer(Queue<NCommand> commandList)
		{
			while (commandList.Count > 0)
			{
				NCommand nCommand = commandList.Peek();
				if (nCommand == null)
				{
					commandList.Dequeue();
					continue;
				}
				if (udpBufferIndex + nCommand.Size > udpBufferLength)
				{
					if ((int)base.debugOut >= 3)
					{
						base.Listener.DebugReturn(DebugLevel.INFO, "UDP package is full. Commands in Package: " + udpCommandCount + ". Commands left in queue: " + commandList.Count);
					}
					break;
				}
				nCommand.SerializeHeader(udpBuffer, ref udpBufferIndex);
				if (nCommand.SizeOfPayload > 0)
				{
					Buffer.BlockCopy(nCommand.Serialize(), 0, udpBuffer, udpBufferIndex, nCommand.SizeOfPayload);
					udpBufferIndex += nCommand.SizeOfPayload;
				}
				udpCommandCount++;
				if (nCommand.IsFlaggedReliable)
				{
					QueueSentCommand(nCommand);
					if (CommandLog != null)
					{
						CommandLog.Enqueue(new CmdLogSentReliable(nCommand, base.timeInt, roundTripTime, roundTripTimeVariance));
						CommandLogResize();
					}
				}
				else
				{
					nCommand.FreePayload();
				}
				commandList.Dequeue();
			}
			return commandList.Count;
		}

		internal void SendData(byte[] data, int length)
		{
			try
			{
				if (datagramEncryptedConnection)
				{
					SendDataEncrypted(data, length);
					return;
				}
				int targetOffset = 0;
				Protocol.Serialize(peerID, data, ref targetOffset);
				data[2] = (byte)(photonPeer.CrcEnabled ? 204 : 0);
				data[3] = udpCommandCount;
				targetOffset = 4;
				Protocol.Serialize(base.timeInt, data, ref targetOffset);
				Protocol.Serialize(challenge, data, ref targetOffset);
				if (photonPeer.CrcEnabled)
				{
					Protocol.Serialize(0, data, ref targetOffset);
					uint value = SupportClass.CalculateCrc(data, length);
					targetOffset -= 4;
					Protocol.Serialize((int)value, data, ref targetOffset);
				}
				bytesOut += length;
				SendToSocket(data, length);
			}
			catch (Exception ex)
			{
				if ((int)base.debugOut >= 1)
				{
					base.Listener.DebugReturn(DebugLevel.ERROR, ex.ToString());
				}
				SupportClass.WriteStackTrace(ex);
			}
		}

		private void SendToSocket(byte[] data, int length)
		{
			if (base.NetworkSimulationSettings.IsSimulationEnabled)
			{
				byte[] array = new byte[length];
				Buffer.BlockCopy(data, 0, array, 0, length);
				SendNetworkSimulated(array);
				return;
			}
			int tickCount = SupportClass.GetTickCount();
			PhotonSocket.Send(data, length);
			int num = SupportClass.GetTickCount() - tickCount;
			if (num > longestSentCall)
			{
				longestSentCall = num;
			}
		}

		private void SendDataEncrypted(byte[] data, int length)
		{
			if (bufferForEncryption == null || bufferForEncryption.Length != base.mtu)
			{
				bufferForEncryption = new byte[base.mtu];
			}
			byte[] array = bufferForEncryption;
			int targetOffset = 0;
			Protocol.Serialize(peerID, array, ref targetOffset);
			array[2] = 1;
			targetOffset++;
			Protocol.Serialize(challenge, array, ref targetOffset);
			data[0] = udpCommandCount;
			int targetOffset2 = 1;
			Protocol.Serialize(base.timeInt, data, ref targetOffset2);
			photonPeer.Encryptor.Encrypt(data, length, array, ref targetOffset);
			Buffer.BlockCopy(photonPeer.Encryptor.CreateHMAC(array, 0, targetOffset), 0, array, targetOffset, 32);
			SendToSocket(array, targetOffset + 32);
		}

		internal void QueueSentCommand(NCommand command)
		{
			command.commandSentTime = base.timeInt;
			command.commandSentCount++;
			if (command.roundTripTimeout == 0)
			{
				command.roundTripTimeout = Math.Min(roundTripTime + 4 * roundTripTimeVariance, photonPeer.InitialResendTimeMax);
				command.timeoutTime = base.timeInt + base.DisconnectTimeout;
			}
			else if (command.commandSentCount > photonPeer.QuickResendAttempts + 1)
			{
				command.roundTripTimeout *= 2;
			}
			lock (sentReliableCommands)
			{
				if (sentReliableCommands.Count == 0)
				{
					int num = command.commandSentTime + command.roundTripTimeout;
					if (num < timeoutInt)
					{
						timeoutInt = num;
					}
				}
				reliableCommandsSent++;
				sentReliableCommands.Add(command);
			}
		}

		internal void QueueOutgoingReliableCommand(NCommand command)
		{
			EnetChannel channel = GetChannel(command.commandChannelID);
			lock (channel)
			{
				if (command.reliableSequenceNumber == 0)
				{
					if (command.IsFlaggedUnsequenced)
					{
						command.reliableSequenceNumber = ++channel.outgoingReliableUnsequencedNumber;
					}
					else
					{
						command.reliableSequenceNumber = ++channel.outgoingReliableSequenceNumber;
					}
				}
				channel.outgoingReliableCommandsList.Enqueue(command);
			}
		}

		internal void QueueOutgoingUnreliableCommand(NCommand command)
		{
			EnetChannel channel = GetChannel(command.commandChannelID);
			lock (channel)
			{
				if (command.IsFlaggedUnsequenced)
				{
					command.reliableSequenceNumber = 0;
					command.unsequencedGroupNumber = ++outgoingUnsequencedGroupNumber;
				}
				else
				{
					command.reliableSequenceNumber = channel.outgoingReliableSequenceNumber;
					command.unreliableSequenceNumber = ++channel.outgoingUnreliableSequenceNumber;
				}
				if (!photonPeer.SendInCreationOrder)
				{
					channel.outgoingUnreliableCommandsList.Enqueue(command);
				}
				else
				{
					channel.outgoingReliableCommandsList.Enqueue(command);
				}
			}
		}

		internal void QueueOutgoingAcknowledgement(NCommand readCommand, int sendTime)
		{
			lock (outgoingAcknowledgementsPool)
			{
				int offset;
				byte[] bufferAndAdvance = outgoingAcknowledgementsPool.GetBufferAndAdvance(20, out offset);
				NCommand.CreateAck(bufferAndAdvance, offset, readCommand, sendTime);
			}
		}

		internal override void ReceiveIncomingCommands(byte[] inBuff, int dataLength)
		{
			timestampOfLastReceive = SupportClass.GetTickCount();
			try
			{
				int offset = 0;
				short value;
				Protocol.Deserialize(out value, inBuff, ref offset);
				byte b = inBuff[offset++];
				int value2;
				byte b2;
				if (b == 1)
				{
					if (photonPeer.Encryptor == null)
					{
						EnqueueDebugReturn(DebugLevel.ERROR, "Got encrypted packet, but encryption is not set up. Packet ignored");
						return;
					}
					datagramEncryptedConnection = true;
					if (!photonPeer.Encryptor.CheckHMAC(inBuff, dataLength))
					{
						packetLossByCrc++;
						if (peerConnectionState != 0 && (int)base.debugOut >= 3)
						{
							EnqueueDebugReturn(DebugLevel.INFO, "Ignored package due to wrong HMAC.");
						}
						return;
					}
					Protocol.Deserialize(out value2, inBuff, ref offset);
					inBuff = photonPeer.Encryptor.Decrypt(inBuff, offset, dataLength - offset - 32, out dataLength);
					dataLength = inBuff.Length;
					offset = 0;
					b2 = inBuff[offset++];
					Protocol.Deserialize(out serverSentTime, inBuff, ref offset);
					bytesIn += 60 + dataLength + (16 - dataLength % 16);
				}
				else
				{
					if (datagramEncryptedConnection)
					{
						EnqueueDebugReturn(DebugLevel.WARNING, "Got not encrypted packet, but expected only encrypted. Packet ignored");
						return;
					}
					b2 = inBuff[offset++];
					Protocol.Deserialize(out serverSentTime, inBuff, ref offset);
					Protocol.Deserialize(out value2, inBuff, ref offset);
					if (b == 204)
					{
						int value3;
						Protocol.Deserialize(out value3, inBuff, ref offset);
						bytesIn += 4L;
						offset -= 4;
						Protocol.Serialize(0, inBuff, ref offset);
						uint num = SupportClass.CalculateCrc(inBuff, dataLength);
						if (value3 != (int)num)
						{
							packetLossByCrc++;
							if (peerConnectionState != 0 && (int)base.debugOut >= 3)
							{
								EnqueueDebugReturn(DebugLevel.INFO, string.Format("Ignored package due to wrong CRC. Incoming:  {0:X} Local: {1:X}", (uint)value3, num));
							}
							return;
						}
					}
					bytesIn += 12L;
				}
				if (base.TrafficStatsEnabled)
				{
					base.TrafficStatsIncoming.TotalPacketCount++;
					base.TrafficStatsIncoming.TotalCommandsInPackets += b2;
				}
				if (b2 > commandBufferSize || b2 <= 0)
				{
					EnqueueDebugReturn(DebugLevel.ERROR, "too many/few incoming commands in package: " + b2 + " > " + commandBufferSize);
				}
				if (value2 != challenge)
				{
					packetLossByChallenge++;
					if (peerConnectionState != 0 && (int)base.debugOut >= 5)
					{
						EnqueueDebugReturn(DebugLevel.ALL, "Info: Ignoring received package due to wrong challenge. Challenge in-package!=local:" + value2 + "!=" + challenge + " Commands in it: " + b2);
					}
					return;
				}
				for (int i = 0; i < b2; i++)
				{
					NCommand nCommand = new NCommand(this, inBuff, ref offset);
					if (nCommand.commandType != 1 && nCommand.commandType != 16)
					{
						lock (CommandQueue)
						{
							CommandQueue.Enqueue(nCommand);
						}
					}
					else
					{
						ExecuteCommand(nCommand);
					}
					if (nCommand.IsFlaggedReliable)
					{
						if (InReliableLog != null)
						{
							InReliableLog.Enqueue(new CmdLogReceivedReliable(nCommand, base.timeInt, roundTripTime, roundTripTimeVariance, base.timeInt - timeLastSendOutgoing, base.timeInt - timeLastSendAck));
							CommandLogResize();
						}
						QueueOutgoingAcknowledgement(nCommand, serverSentTime);
						if (base.TrafficStatsEnabled)
						{
							base.TrafficStatsIncoming.TimestampOfLastReliableCommand = SupportClass.GetTickCount();
							base.TrafficStatsOutgoing.CountControlCommand(20);
						}
					}
				}
			}
			catch (Exception ex)
			{
				if ((int)base.debugOut >= 1)
				{
					EnqueueDebugReturn(DebugLevel.ERROR, string.Format("Exception while reading commands from incoming data: {0}", ex));
				}
				SupportClass.WriteStackTrace(ex);
			}
		}

		internal bool ExecuteCommand(NCommand command)
		{
			bool result = true;
			switch (command.commandType)
			{
			case 2:
			case 5:
				if (base.TrafficStatsEnabled)
				{
					base.TrafficStatsIncoming.CountControlCommand(command.Size);
				}
				break;
			case 4:
			{
				if (base.TrafficStatsEnabled)
				{
					base.TrafficStatsIncoming.CountControlCommand(command.Size);
				}
				StatusCode statusValue = StatusCode.DisconnectByServerReasonUnknown;
				if (command.reservedByte == 1)
				{
					statusValue = StatusCode.DisconnectByServerLogic;
				}
				else if (command.reservedByte == 2)
				{
					statusValue = StatusCode.DisconnectByServerTimeout;
				}
				else if (command.reservedByte == 3)
				{
					statusValue = StatusCode.DisconnectByServerUserLimit;
				}
				if ((int)base.debugOut >= 3)
				{
					base.Listener.DebugReturn(DebugLevel.INFO, "Server " + base.ServerAddress + " sent disconnect. PeerId: " + (ushort)peerID + " RTT/Variance:" + roundTripTime + "/" + roundTripTimeVariance + " reason byte: " + command.reservedByte + " peerConnectionState: " + peerConnectionState);
				}
				if (peerConnectionState != 0 && peerConnectionState != ConnectionStateValue.Disconnecting)
				{
					EnqueueStatusCallback(statusValue);
					Disconnect();
				}
				break;
			}
			case 1:
			case 16:
			{
				if (base.TrafficStatsEnabled)
				{
					base.TrafficStatsIncoming.TimestampOfLastAck = SupportClass.GetTickCount();
					base.TrafficStatsIncoming.CountControlCommand(command.Size);
				}
				timeLastAckReceive = base.timeInt;
				lastRoundTripTime = base.timeInt - command.ackReceivedSentTime;
				if (lastRoundTripTime < 0 || lastRoundTripTime > 10000)
				{
					if ((int)base.debugOut >= 3)
					{
						EnqueueDebugReturn(DebugLevel.INFO, "Measured lastRoundtripTime is suspicious: " + lastRoundTripTime + " for command: " + command);
					}
					lastRoundTripTime = roundTripTime * 4;
				}
				NCommand nCommand = RemoveSentReliableCommand(command.ackReceivedReliableSequenceNumber, command.commandChannelID, command.commandType == 16);
				if (CommandLog != null)
				{
					CommandLog.Enqueue(new CmdLogReceivedAck(command, base.timeInt, roundTripTime, roundTripTimeVariance));
					CommandLogResize();
				}
				if (nCommand == null)
				{
					break;
				}
				nCommand.FreePayload();
				if (nCommand.commandType == 12)
				{
					if (lastRoundTripTime <= roundTripTime)
					{
						serverTimeOffset = serverSentTime + (lastRoundTripTime >> 1) - SupportClass.GetTickCount();
						serverTimeOffsetIsAvailable = true;
					}
					else
					{
						FetchServerTimestamp();
					}
					break;
				}
				UpdateRoundTripTimeAndVariance(lastRoundTripTime);
				if (nCommand.commandType == 4 && peerConnectionState == ConnectionStateValue.Disconnecting)
				{
					if ((int)base.debugOut >= 3)
					{
						EnqueueDebugReturn(DebugLevel.INFO, "Received disconnect ACK by server");
					}
					EnqueueActionForDispatch(delegate
					{
						PhotonSocket.Disconnect();
					});
				}
				else if (nCommand.commandType == 2 && lastRoundTripTime >= 0)
				{
					if (lastRoundTripTime <= 15)
					{
						roundTripTime = 15;
						roundTripTimeVariance = 5;
					}
					else
					{
						roundTripTime = lastRoundTripTime;
					}
				}
				break;
			}
			case 6:
				if (base.TrafficStatsEnabled)
				{
					base.TrafficStatsIncoming.CountReliableOpCommand(command.Size);
				}
				if (peerConnectionState == ConnectionStateValue.Connected)
				{
					result = QueueIncomingCommand(command);
				}
				break;
			case 14:
				if (base.TrafficStatsEnabled)
				{
					base.TrafficStatsIncoming.CountReliableOpCommand(command.Size);
				}
				if (peerConnectionState == ConnectionStateValue.Connected)
				{
					result = QueueIncomingCommand(command);
				}
				break;
			case 7:
				if (base.TrafficStatsEnabled)
				{
					base.TrafficStatsIncoming.CountUnreliableOpCommand(command.Size);
				}
				if (peerConnectionState == ConnectionStateValue.Connected)
				{
					result = QueueIncomingCommand(command);
				}
				break;
			case 11:
				if (base.TrafficStatsEnabled)
				{
					base.TrafficStatsIncoming.CountUnreliableOpCommand(command.Size);
				}
				if (peerConnectionState == ConnectionStateValue.Connected)
				{
					result = QueueIncomingCommand(command);
				}
				break;
			case 8:
			case 15:
			{
				if (peerConnectionState != ConnectionStateValue.Connected)
				{
					break;
				}
				if (base.TrafficStatsEnabled)
				{
					base.TrafficStatsIncoming.CountFragmentOpCommand(command.Size);
				}
				if (command.fragmentNumber > command.fragmentCount || command.fragmentOffset >= command.totalLength || command.fragmentOffset + command.Payload.Length > command.totalLength)
				{
					if ((int)base.debugOut >= 1)
					{
						base.Listener.DebugReturn(DebugLevel.ERROR, "Received fragment has bad size: " + command);
					}
					break;
				}
				bool flag = command.commandType == 8;
				EnetChannel channel = GetChannel(command.commandChannelID);
				NCommand fragment = null;
				bool flag2 = channel.TryGetFragment(command.startSequenceNumber, flag, out fragment);
				if ((flag2 && fragment.fragmentsRemaining <= 0) || !QueueIncomingCommand(command))
				{
					break;
				}
				if (command.reliableSequenceNumber != command.startSequenceNumber)
				{
					if (flag2)
					{
						fragment.fragmentsRemaining--;
					}
				}
				else
				{
					fragment = command;
					fragment.fragmentsRemaining--;
					NCommand fragment2 = null;
					int num = command.startSequenceNumber + 1;
					while (fragment.fragmentsRemaining > 0 && num < fragment.startSequenceNumber + fragment.fragmentCount)
					{
						if (channel.TryGetFragment(num++, flag, out fragment2))
						{
							fragment.fragmentsRemaining--;
						}
					}
				}
				if (fragment == null || fragment.fragmentsRemaining > 0)
				{
					break;
				}
				StreamBuffer streamBuffer = PeerBase.MessageBufferPoolGet();
				streamBuffer.Position = 0;
				streamBuffer.SetCapacityMinimum(fragment.totalLength);
				byte[] buffer = streamBuffer.GetBuffer();
				for (int i = fragment.startSequenceNumber; i < fragment.startSequenceNumber + fragment.fragmentCount; i++)
				{
					NCommand fragment3;
					if (channel.TryGetFragment(i, flag, out fragment3))
					{
						Buffer.BlockCopy(fragment3.Payload.GetBuffer(), 0, buffer, fragment3.fragmentOffset, fragment3.Payload.Length);
						fragment3.FreePayload();
						channel.RemoveFragment(fragment3.reliableSequenceNumber, flag);
						continue;
					}
					throw new Exception("startCommand.fragmentsRemaining was 0 but not all fragments were found to be combined!");
				}
				streamBuffer.SetLength(fragment.totalLength);
				fragment.Payload = streamBuffer;
				fragment.Size = 12 * fragment.fragmentCount + fragment.totalLength;
				if (!flag)
				{
					channel.incomingUnsequencedCommandsList.Enqueue(fragment);
				}
				else
				{
					channel.incomingReliableCommandsList.Add(fragment.startSequenceNumber, fragment);
				}
				break;
			}
			case 3:
				if (base.TrafficStatsEnabled)
				{
					base.TrafficStatsIncoming.CountControlCommand(command.Size);
				}
				if (peerConnectionState == ConnectionStateValue.Connecting)
				{
					byte[] buf = PrepareConnectData(base.ServerAddress, AppId, CustomInitData);
					CreateAndEnqueueCommand(6, new StreamBuffer(buf), 0);
					if (photonPeer.RandomizeSequenceNumbers)
					{
						ApplyRandomizedSequenceNumbers();
					}
					peerConnectionState = ConnectionStateValue.Connected;
				}
				break;
			}
			return result;
		}

		internal bool QueueIncomingCommand(NCommand command)
		{
			EnetChannel channel = GetChannel(command.commandChannelID);
			if (channel == null)
			{
				if ((int)base.debugOut >= 1)
				{
					base.Listener.DebugReturn(DebugLevel.ERROR, "Received command for non-existing channel: " + command.commandChannelID);
				}
				return false;
			}
			if ((int)base.debugOut >= 5)
			{
				base.Listener.DebugReturn(DebugLevel.ALL, string.Concat("queueIncomingCommand() ", command, " channel seq# r/u: ", channel.incomingReliableSequenceNumber, "/", channel.incomingUnreliableSequenceNumber));
			}
			if (command.IsFlaggedReliable)
			{
				if (command.IsFlaggedUnsequenced)
				{
					return channel.QueueIncomingReliableUnsequenced(command);
				}
				if (command.reliableSequenceNumber <= channel.incomingReliableSequenceNumber)
				{
					if ((int)base.debugOut >= 3)
					{
						base.Listener.DebugReturn(DebugLevel.INFO, string.Concat("incoming command ", command, " is old (not saving it). Dispatched incomingReliableSequenceNumber: ", channel.incomingReliableSequenceNumber));
					}
					return false;
				}
				if (channel.ContainsReliableSequenceNumber(command.reliableSequenceNumber))
				{
					if ((int)base.debugOut >= 3)
					{
						base.Listener.DebugReturn(DebugLevel.INFO, string.Concat("Info: command was received before! Old/New: ", channel.FetchReliableSequenceNumber(command.reliableSequenceNumber), "/", command, " inReliableSeq#: ", channel.incomingReliableSequenceNumber));
					}
					return false;
				}
				channel.incomingReliableCommandsList.Add(command.reliableSequenceNumber, command);
				return true;
			}
			if (command.commandFlags == 0)
			{
				if (command.reliableSequenceNumber < channel.incomingReliableSequenceNumber)
				{
					if ((int)base.debugOut >= 3)
					{
						base.Listener.DebugReturn(DebugLevel.INFO, "incoming reliable-seq# < Dispatched-rel-seq#. not saved.");
					}
					return true;
				}
				if (command.unreliableSequenceNumber <= channel.incomingUnreliableSequenceNumber)
				{
					if ((int)base.debugOut >= 3)
					{
						base.Listener.DebugReturn(DebugLevel.INFO, "incoming unreliable-seq# < Dispatched-unrel-seq#. not saved.");
					}
					return true;
				}
				if (channel.ContainsUnreliableSequenceNumber(command.unreliableSequenceNumber))
				{
					if ((int)base.debugOut >= 3)
					{
						base.Listener.DebugReturn(DebugLevel.INFO, string.Concat("command was received before! Old/New: ", channel.incomingUnreliableCommandsList[command.unreliableSequenceNumber], "/", command));
					}
					return false;
				}
				channel.incomingUnreliableCommandsList.Add(command.unreliableSequenceNumber, command);
				return true;
			}
			if (command.commandFlags == 2)
			{
				int unsequencedGroupNumber = command.unsequencedGroupNumber;
				int num = command.unsequencedGroupNumber % 128;
				if (unsequencedGroupNumber >= incomingUnsequencedGroupNumber + 128)
				{
					incomingUnsequencedGroupNumber = unsequencedGroupNumber - num;
					for (int i = 0; i < unsequencedWindow.Length; i++)
					{
						unsequencedWindow[i] = 0;
					}
				}
				else if (unsequencedGroupNumber < incomingUnsequencedGroupNumber || (unsequencedWindow[num / 32] & (1 << num % 32)) != 0)
				{
					return false;
				}
				unsequencedWindow[num / 32] |= 1 << num % 32;
				channel.incomingUnsequencedCommandsList.Enqueue(command);
				return true;
			}
			return false;
		}

		internal NCommand RemoveSentReliableCommand(int ackReceivedReliableSequenceNumber, int ackReceivedChannel, bool isUnsequenced)
		{
			NCommand nCommand = null;
			lock (sentReliableCommands)
			{
				foreach (NCommand sentReliableCommand in sentReliableCommands)
				{
					if (sentReliableCommand != null && sentReliableCommand.reliableSequenceNumber == ackReceivedReliableSequenceNumber && sentReliableCommand.IsFlaggedUnsequenced == isUnsequenced && sentReliableCommand.commandChannelID == ackReceivedChannel)
					{
						nCommand = sentReliableCommand;
						break;
					}
				}
				if (nCommand != null)
				{
					sentReliableCommands.Remove(nCommand);
					if (sentReliableCommands.Count > 0)
					{
						timeoutInt = base.timeInt + 25;
					}
				}
				else if ((int)base.debugOut >= 5 && peerConnectionState != ConnectionStateValue.Connected && peerConnectionState != ConnectionStateValue.Disconnecting)
				{
					EnqueueDebugReturn(DebugLevel.ALL, string.Format("No sent command for ACK (Ch: {0} Sq#: {1}). PeerState: {2}.", ackReceivedReliableSequenceNumber, ackReceivedChannel, peerConnectionState));
				}
			}
			return nCommand;
		}

		internal string CommandListToString(NCommand[] list)
		{
			if ((int)base.debugOut < 5)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < list.Length; i++)
			{
				stringBuilder.Append(i + "=");
				stringBuilder.Append(list[i]);
				stringBuilder.Append(" # ");
			}
			return stringBuilder.ToString();
		}
	}
}
