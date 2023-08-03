using System.Collections.Generic;

namespace ExitGames.Client.Photon
{
	internal class EnetChannel
	{
		internal byte ChannelNumber;

		internal Dictionary<int, NCommand> incomingReliableCommandsList;

		internal Dictionary<int, NCommand> incomingUnreliableCommandsList;

		internal Queue<NCommand> incomingUnsequencedCommandsList;

		internal Dictionary<int, NCommand> incomingUnsequencedFragments;

		internal Queue<NCommand> outgoingReliableCommandsList;

		internal Queue<NCommand> outgoingUnreliableCommandsList;

		internal int incomingReliableSequenceNumber;

		internal int incomingUnreliableSequenceNumber;

		internal int outgoingReliableSequenceNumber;

		internal int outgoingUnreliableSequenceNumber;

		internal int outgoingReliableUnsequencedNumber;

		private int reliableUnsequencedNumbersCompletelyReceived;

		private HashSet<int> reliableUnsequencedNumbersReceived = new HashSet<int>();

		public EnetChannel(byte channelNumber, int commandBufferSize)
		{
			ChannelNumber = channelNumber;
			incomingReliableCommandsList = new Dictionary<int, NCommand>(commandBufferSize);
			incomingUnreliableCommandsList = new Dictionary<int, NCommand>(commandBufferSize);
			incomingUnsequencedCommandsList = new Queue<NCommand>();
			incomingUnsequencedFragments = new Dictionary<int, NCommand>();
			outgoingReliableCommandsList = new Queue<NCommand>(commandBufferSize);
			outgoingUnreliableCommandsList = new Queue<NCommand>(commandBufferSize);
		}

		public bool ContainsUnreliableSequenceNumber(int unreliableSequenceNumber)
		{
			return incomingUnreliableCommandsList.ContainsKey(unreliableSequenceNumber);
		}

		public NCommand FetchUnreliableSequenceNumber(int unreliableSequenceNumber)
		{
			return incomingUnreliableCommandsList[unreliableSequenceNumber];
		}

		public bool ContainsReliableSequenceNumber(int reliableSequenceNumber)
		{
			return incomingReliableCommandsList.ContainsKey(reliableSequenceNumber);
		}

		public NCommand FetchReliableSequenceNumber(int reliableSequenceNumber)
		{
			return incomingReliableCommandsList[reliableSequenceNumber];
		}

		public bool TryGetFragment(int reliableSequenceNumber, bool isSequenced, out NCommand fragment)
		{
			if (isSequenced)
			{
				return incomingReliableCommandsList.TryGetValue(reliableSequenceNumber, out fragment);
			}
			return incomingUnsequencedFragments.TryGetValue(reliableSequenceNumber, out fragment);
		}

		public void RemoveFragment(int reliableSequenceNumber, bool isSequenced)
		{
			if (isSequenced)
			{
				incomingReliableCommandsList.Remove(reliableSequenceNumber);
			}
			else
			{
				incomingUnsequencedFragments.Remove(reliableSequenceNumber);
			}
		}

		public void clearAll()
		{
			lock (this)
			{
				incomingReliableCommandsList.Clear();
				incomingUnreliableCommandsList.Clear();
				incomingUnsequencedCommandsList.Clear();
				incomingUnsequencedFragments.Clear();
				outgoingReliableCommandsList.Clear();
				outgoingUnreliableCommandsList.Clear();
			}
		}

		public bool QueueIncomingReliableUnsequenced(NCommand command)
		{
			if (command.reliableSequenceNumber <= reliableUnsequencedNumbersCompletelyReceived)
			{
				return false;
			}
			if (reliableUnsequencedNumbersReceived.Contains(command.reliableSequenceNumber))
			{
				return false;
			}
			if (command.reliableSequenceNumber == reliableUnsequencedNumbersCompletelyReceived + 1)
			{
				reliableUnsequencedNumbersCompletelyReceived++;
			}
			else
			{
				reliableUnsequencedNumbersReceived.Add(command.reliableSequenceNumber);
			}
			while (reliableUnsequencedNumbersReceived.Contains(reliableUnsequencedNumbersCompletelyReceived + 1))
			{
				reliableUnsequencedNumbersCompletelyReceived++;
				reliableUnsequencedNumbersReceived.Remove(reliableUnsequencedNumbersCompletelyReceived);
			}
			if (command.commandType == 15)
			{
				incomingUnsequencedFragments.Add(command.reliableSequenceNumber, command);
			}
			else
			{
				incomingUnsequencedCommandsList.Enqueue(command);
			}
			return true;
		}
	}
}
