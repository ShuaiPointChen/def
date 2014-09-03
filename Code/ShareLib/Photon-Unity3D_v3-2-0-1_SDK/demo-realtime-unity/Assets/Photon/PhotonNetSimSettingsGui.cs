using ExitGames.Client.Photon.Lite;
using UnityEngine;


/// <summary>
/// This MonoBehaviour is a basic GUI for the Photon client's network-simulation feature.
/// It can modify lag (fixed delay), jitter (random lag) and packet loss.
/// </summary>
public class PhotonNetSimSettingsGui : MonoBehaviour
{
    public Rect WindowRect = new Rect(0, 0, 200, 50);
    public bool Visible = false;
    public LitePeer Peer { get; set; }

    void Start()
    {
        this.WindowRect = new Rect(0, 133, 200, this.WindowRect.height);
    }

    public void OnGUI()
    {
        if (!this.Visible)
        {
            return;
        }

        if (this.Peer == null)
        {
            this.WindowRect = GUILayout.Window(0, this.WindowRect, this.NetSimHasNoPeerWindow, "Netw. Sim.");
        }
        else
        {
            this.WindowRect = GUILayout.Window(0, this.WindowRect, this.NetSimWindow, "Netw. Sim.");
        }
    }

    private void NetSimHasNoPeerWindow(int windowId)
    {
        GUILayout.Label("No peer to communicate with. ");
    }

    private void NetSimWindow(int windowId)
    {
        GUILayout.Label(string.Format("Rtt: {0,3} +/- {1,3}", this.Peer.RoundTripTime, this.Peer.RoundTripTimeVariance));

        bool simEnabled = this.Peer.IsSimulationEnabled;
        bool newSimEnabled = GUILayout.Toggle(simEnabled, "sim");
        if (newSimEnabled != simEnabled)
        {
            this.Peer.IsSimulationEnabled = newSimEnabled;
        }

        float inOutLag = this.Peer.NetworkSimulationSettings.IncomingLag;
        GUILayout.Label("lag " + inOutLag);
        inOutLag = GUILayout.HorizontalSlider(inOutLag, 0, 500);

        this.Peer.NetworkSimulationSettings.IncomingLag = (int)inOutLag;
        this.Peer.NetworkSimulationSettings.OutgoingLag = (int)inOutLag;

        float inOutJitter = this.Peer.NetworkSimulationSettings.IncomingJitter;
        GUILayout.Label("jit " + inOutJitter);
        inOutJitter = GUILayout.HorizontalSlider(inOutJitter, 0, 100);

        this.Peer.NetworkSimulationSettings.IncomingJitter = (int)inOutJitter;
        this.Peer.NetworkSimulationSettings.OutgoingJitter = (int)inOutJitter;

        float loss = this.Peer.NetworkSimulationSettings.IncomingLossPercentage;
        GUILayout.Label("loss " + loss);
        loss = GUILayout.HorizontalSlider(loss, 0, 10);

        this.Peer.NetworkSimulationSettings.IncomingLossPercentage = (int)loss;
        this.Peer.NetworkSimulationSettings.OutgoingLossPercentage = (int)loss;

        GUI.DragWindow();
    }
}