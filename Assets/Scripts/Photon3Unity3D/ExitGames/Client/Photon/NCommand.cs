using System;

namespace ExitGames.Client.Photon
{
	internal class NCommand : IComparable<NCommand>
	{
		internal const byte FV_UNRELIABLE = 0;

		internal const byte FV_RELIABLE = 1;

		internal const byte FV_UNRELIABLE_UNSEQUENCED = 2;

		internal const byte FV_RELIBALE_UNSEQUENCED = 3;

		internal const byte CT_NONE = 0;

		internal const byte CT_ACK = 1;

		internal const byte CT_CONNECT = 2;

		internal const byte CT_VERIFYCONNECT = 3;

		internal const byte CT_DISCONNECT = 4;

		internal const byte CT_PING = 5;

		internal const byte CT_SENDRELIABLE = 6;

		internal const byte CT_SENDUNRELIABLE = 7;

		internal const byte CT_SENDFRAGMENT = 8;

		internal const byte CT_SENDUNSEQUENCED = 11;

		internal const byte CT_EG_SERVERTIME = 12;

		internal const byte CT_EG_SEND_UNRELIABLE_PROCESSED = 13;

		internal const byte CT_EG_SEND_RELIABLE_UNSEQUENCED = 14;

		internal const byte CT_EG_SEND_FRAGMENT_UNSEQUENCED = 15;

		internal const byte CT_EG_ACK_UNSEQUENCED = 16;

		internal const int HEADER_UDP_PACK_LENGTH = 12;

		internal const int CmdSizeMinimum = 12;

		internal const int CmdSizeAck = 20;

		internal const int CmdSizeConnect = 44;

		internal const int CmdSizeVerifyConnect = 44;

		internal const int CmdSizeDisconnect = 12;

		internal const int CmdSizePing = 12;

		internal const int CmdSizeReliableHeader = 12;

		internal const int CmdSizeUnreliableHeader = 16;

		internal const int CmdSizeUnsequensedHeader = 16;

		internal const int CmdSizeFragmentHeader = 32;

		internal const int CmdSizeMaxHeader = 36;

		internal byte commandFlags;

		internal byte commandType;

		internal byte commandChannelID;

		internal int reliableSequenceNumber;

		internal int unreliableSequenceNumber;

		internal int unsequencedGroupNumber;

		internal byte reservedByte = 4;

		internal int startSequenceNumber;

		internal int fragmentCount;

		internal int fragmentNumber;

		internal int totalLength;

		internal int fragmentOffset;

		internal int fragmentsRemaining;

		internal int commandSentTime;

		internal byte commandSentCount;

		internal int roundTripTimeout;

		internal int timeoutTime;

		internal int ackReceivedReliableSequenceNumber;

		internal int ackReceivedSentTime;

		internal int Size;

		private byte[] commandHeader;

		internal int SizeOfHeader;

		internal StreamBuffer Payload;

		protected internal int SizeOfPayload
		{
			get
			{
				return (Payload != null) ? Payload.Length : 0;
			}
		}

		protected internal bool IsFlaggedUnsequenced
		{
			get
			{
				return (commandFlags & 2) > 0;
			}
		}

		protected internal bool IsFlaggedReliable
		{
			get
			{
				return (commandFlags & 1) > 0;
			}
		}

		internal NCommand(EnetPeer peer, byte commandType, StreamBuffer payload, byte channel)
		{
			this.commandType = commandType;
			commandFlags = 1;
			commandChannelID = channel;
			Payload = payload;
			Size = 12;
			switch (this.commandType)
			{
			case 2:
			{
				Size = 44;
				byte[] array = new byte[32];
				array[0] = 0;
				array[1] = 0;
				int targetOffset = 2;
				Protocol.Serialize((short)peer.mtu, array, ref targetOffset);
				array[4] = 0;
				array[5] = 0;
				array[6] = 128;
				array[7] = 0;
				array[11] = peer.ChannelCount;
				array[15] = 0;
				array[19] = 0;
				array[22] = 19;
				array[23] = 136;
				array[27] = 2;
				array[31] = 2;
				Payload = new StreamBuffer(array);
				break;
			}
			case 4:
				Size = 12;
				if (peer.peerConnectionState != ConnectionStateValue.Connected)
				{
					commandFlags = 2;
					if (peer.peerConnectionState == ConnectionStateValue.Zombie)
					{
						reservedByte = 2;
					}
				}
				break;
			case 6:
				Size = 12 + payload.Length;
				break;
			case 14:
				Size = 12 + payload.Length;
				commandFlags = 3;
				break;
			case 7:
				Size = 16 + payload.Length;
				commandFlags = 0;
				break;
			case 11:
				Size = 16 + payload.Length;
				commandFlags = 2;
				break;
			case 8:
				Size = 32 + payload.Length;
				break;
			case 15:
				Size = 32 + payload.Length;
				commandFlags = 3;
				break;
			case 3:
			case 5:
			case 9:
			case 10:
			case 12:
			case 13:
				break;
			}
		}

		internal static void CreateAck(byte[] buffer, int offset, NCommand commandToAck, int sentTime)
		{
			buffer[offset++] = (byte)((!commandToAck.IsFlaggedUnsequenced) ? 1 : 16);
			buffer[offset++] = commandToAck.commandChannelID;
			buffer[offset++] = 0;
			buffer[offset++] = commandToAck.reservedByte;
			Protocol.Serialize(20, buffer, ref offset);
			Protocol.Serialize(0, buffer, ref offset);
			Protocol.Serialize(commandToAck.reliableSequenceNumber, buffer, ref offset);
			Protocol.Serialize(sentTime, buffer, ref offset);
		}

		internal NCommand(EnetPeer peer, byte[] inBuff, ref int readingOffset)
		{
			commandType = inBuff[readingOffset++];
			commandChannelID = inBuff[readingOffset++];
			commandFlags = inBuff[readingOffset++];
			reservedByte = inBuff[readingOffset++];
			Protocol.Deserialize(out Size, inBuff, ref readingOffset);
			Protocol.Deserialize(out reliableSequenceNumber, inBuff, ref readingOffset);
			peer.bytesIn += Size;
			int num = 0;
			switch (commandType)
			{
			case 1:
			case 16:
				Protocol.Deserialize(out ackReceivedReliableSequenceNumber, inBuff, ref readingOffset);
				Protocol.Deserialize(out ackReceivedSentTime, inBuff, ref readingOffset);
				break;
			case 6:
			case 14:
				num = Size - 12;
				break;
			case 7:
				Protocol.Deserialize(out unreliableSequenceNumber, inBuff, ref readingOffset);
				num = Size - 16;
				break;
			case 11:
				Protocol.Deserialize(out unsequencedGroupNumber, inBuff, ref readingOffset);
				num = Size - 16;
				break;
			case 8:
			case 15:
				Protocol.Deserialize(out startSequenceNumber, inBuff, ref readingOffset);
				Protocol.Deserialize(out fragmentCount, inBuff, ref readingOffset);
				Protocol.Deserialize(out fragmentNumber, inBuff, ref readingOffset);
				Protocol.Deserialize(out totalLength, inBuff, ref readingOffset);
				Protocol.Deserialize(out fragmentOffset, inBuff, ref readingOffset);
				num = Size - 32;
				fragmentsRemaining = fragmentCount;
				break;
			case 3:
			{
				short value;
				Protocol.Deserialize(out value, inBuff, ref readingOffset);
				readingOffset += 30;
				if (peer.peerID == -1 || peer.peerID == -2)
				{
					peer.peerID = value;
				}
				break;
			}
			}
			if (num != 0)
			{
				StreamBuffer streamBuffer = PeerBase.MessageBufferPoolGet();
				streamBuffer.Write(inBuff, readingOffset, num);
				Payload = streamBuffer;
				Payload.Position = 0;
				readingOffset += num;
			}
		}

		internal void SerializeHeader(byte[] buffer, ref int bufferIndex)
		{
			if (commandHeader == null)
			{
				SizeOfHeader = 12;
				if (commandType == 7)
				{
					SizeOfHeader = 16;
				}
				else if (commandType == 11)
				{
					SizeOfHeader = 16;
				}
				else if (commandType == 8 || commandType == 15)
				{
					SizeOfHeader = 32;
				}
				buffer[bufferIndex++] = commandType;
				buffer[bufferIndex++] = commandChannelID;
				buffer[bufferIndex++] = commandFlags;
				buffer[bufferIndex++] = reservedByte;
				Protocol.Serialize(Size, buffer, ref bufferIndex);
				Protocol.Serialize(reliableSequenceNumber, buffer, ref bufferIndex);
				if (commandType == 7)
				{
					Protocol.Serialize(unreliableSequenceNumber, buffer, ref bufferIndex);
				}
				else if (commandType == 11)
				{
					Protocol.Serialize(unsequencedGroupNumber, buffer, ref bufferIndex);
				}
				else if (commandType == 8 || commandType == 15)
				{
					Protocol.Serialize(startSequenceNumber, buffer, ref bufferIndex);
					Protocol.Serialize(fragmentCount, buffer, ref bufferIndex);
					Protocol.Serialize(fragmentNumber, buffer, ref bufferIndex);
					Protocol.Serialize(totalLength, buffer, ref bufferIndex);
					Protocol.Serialize(fragmentOffset, buffer, ref bufferIndex);
				}
			}
		}

		internal byte[] Serialize()
		{
			return Payload.GetBuffer();
		}

		public void FreePayload()
		{
			if (Payload != null)
			{
				PeerBase.MessageBufferPoolPut(Payload);
			}
			Payload = null;
		}

		public int CompareTo(NCommand other)
		{
			if (other == null)
			{
				return 1;
			}
			int num = reliableSequenceNumber - other.reliableSequenceNumber;
			if (IsFlaggedReliable || num != 0)
			{
				return num;
			}
			return unreliableSequenceNumber - other.unreliableSequenceNumber;
		}

		public override string ToString()
		{
			if (commandType == 1 || commandType == 16)
			{
				return string.Format("CMD({1} ack for ch#/sq#/time: {0}/{2}/{3})", commandChannelID, commandType, ackReceivedReliableSequenceNumber, ackReceivedSentTime);
			}
			return string.Format("CMD({1} ch#/sq#/usq#: {0}/{2}/{3} r#/st/tt:{5}/{4}/{6})", commandChannelID, commandType, reliableSequenceNumber, unreliableSequenceNumber, commandSentTime, commandSentCount, timeoutTime);
		}
	}
}
