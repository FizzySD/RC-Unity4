using ExitGames.Client.Photon;
using UnityEngine;

public class PhotonLagSimulationGui : MonoBehaviour
{
	public bool Visible = true;

	public int WindowId = 101;

	public Rect WindowRect = new Rect(0f, 100f, 120f, 100f);

	public PhotonPeer Peer { get; set; }

	private void NetSimHasNoPeerWindow(int windowId)
	{
		GUILayout.Label("No peer to communicate with. ");
	}

	private void NetSimWindow(int windowId)
	{
		GUILayout.Label(string.Format("Rtt:{0,4} +/-{1,3}", Peer.RoundTripTime, Peer.RoundTripTimeVariance));
		bool isSimulationEnabled = Peer.IsSimulationEnabled;
		bool flag = GUILayout.Toggle(isSimulationEnabled, "Simulate");
		if (flag != isSimulationEnabled)
		{
			Peer.IsSimulationEnabled = flag;
		}
		float value = Peer.NetworkSimulationSettings.IncomingLag;
		GUILayout.Label("Lag " + value);
		value = GUILayout.HorizontalSlider(value, 0f, 500f);
		Peer.NetworkSimulationSettings.IncomingLag = (int)value;
		Peer.NetworkSimulationSettings.OutgoingLag = (int)value;
		float value2 = Peer.NetworkSimulationSettings.IncomingJitter;
		GUILayout.Label("Jit " + value2);
		value2 = GUILayout.HorizontalSlider(value2, 0f, 100f);
		Peer.NetworkSimulationSettings.IncomingJitter = (int)value2;
		Peer.NetworkSimulationSettings.OutgoingJitter = (int)value2;
		float value3 = Peer.NetworkSimulationSettings.IncomingLossPercentage;
		GUILayout.Label("Loss " + value3);
		value3 = GUILayout.HorizontalSlider(value3, 0f, 10f);
		Peer.NetworkSimulationSettings.IncomingLossPercentage = (int)value3;
		Peer.NetworkSimulationSettings.OutgoingLossPercentage = (int)value3;
		if (GUI.changed)
		{
			WindowRect.height = 100f;
		}
		GUI.DragWindow();
	}

	public void OnGUI()
	{
		if (Visible)
		{
			if (Peer == null)
			{
				WindowRect = GUILayout.Window(WindowId, WindowRect, NetSimHasNoPeerWindow, "Netw. Sim.");
			}
			else
			{
				WindowRect = GUILayout.Window(WindowId, WindowRect, NetSimWindow, "Netw. Sim.");
			}
		}
	}

	public void Start()
	{
		Peer = PhotonNetwork.networkingPeer;
	}
}
