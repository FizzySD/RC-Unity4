using System.Threading;

namespace ExitGames.Client.Photon
{
	public class NetworkSimulationSet
	{
		private bool isSimulationEnabled = false;

		private int outgoingLag = 100;

		private int outgoingJitter = 0;

		private int outgoingLossPercentage = 1;

		private int incomingLag = 100;

		private int incomingJitter = 0;

		private int incomingLossPercentage = 1;

		internal PeerBase peerBase;

		private Thread netSimThread;

		protected internal readonly ManualResetEvent NetSimManualResetEvent = new ManualResetEvent(false);

		protected internal bool IsSimulationEnabled
		{
			get
			{
				return isSimulationEnabled;
			}
			set
			{
				lock (NetSimManualResetEvent)
				{
					if (value == isSimulationEnabled)
					{
						return;
					}
					if (!value)
					{
						lock (peerBase.NetSimListIncoming)
						{
							foreach (SimulationItem item in peerBase.NetSimListIncoming)
							{
								if (peerBase.PhotonSocket != null && peerBase.PhotonSocket.Connected)
								{
									peerBase.ReceiveIncomingCommands(item.DelayedData, item.DelayedData.Length);
								}
							}
							peerBase.NetSimListIncoming.Clear();
						}
						lock (peerBase.NetSimListOutgoing)
						{
							foreach (SimulationItem item2 in peerBase.NetSimListOutgoing)
							{
								if (peerBase.PhotonSocket != null && peerBase.PhotonSocket.Connected)
								{
									peerBase.PhotonSocket.Send(item2.DelayedData, item2.DelayedData.Length);
								}
							}
							peerBase.NetSimListOutgoing.Clear();
						}
					}
					isSimulationEnabled = value;
					if (isSimulationEnabled)
					{
						if (netSimThread == null)
						{
							netSimThread = new Thread(peerBase.NetworkSimRun);
							netSimThread.IsBackground = true;
							netSimThread.Name = "netSim" + SupportClass.GetTickCount();
							netSimThread.Start();
						}
						NetSimManualResetEvent.Set();
					}
					else
					{
						NetSimManualResetEvent.Reset();
					}
				}
			}
		}

		public int OutgoingLag
		{
			get
			{
				return outgoingLag;
			}
			set
			{
				outgoingLag = value;
			}
		}

		public int OutgoingJitter
		{
			get
			{
				return outgoingJitter;
			}
			set
			{
				outgoingJitter = value;
			}
		}

		public int OutgoingLossPercentage
		{
			get
			{
				return outgoingLossPercentage;
			}
			set
			{
				outgoingLossPercentage = value;
			}
		}

		public int IncomingLag
		{
			get
			{
				return incomingLag;
			}
			set
			{
				incomingLag = value;
			}
		}

		public int IncomingJitter
		{
			get
			{
				return incomingJitter;
			}
			set
			{
				incomingJitter = value;
			}
		}

		public int IncomingLossPercentage
		{
			get
			{
				return incomingLossPercentage;
			}
			set
			{
				incomingLossPercentage = value;
			}
		}

		public int LostPackagesOut { get; internal set; }

		public int LostPackagesIn { get; internal set; }

		public override string ToString()
		{
			return string.Format("NetworkSimulationSet {6}.  Lag in={0} out={1}. Jitter in={2} out={3}. Loss in={4} out={5}.", incomingLag, outgoingLag, incomingJitter, outgoingJitter, incomingLossPercentage, outgoingLossPercentage, IsSimulationEnabled);
		}
	}
}
