using UnityEngine;
using System.Collections;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.Lite;


/// <summary>
/// This MonoBehaviour is a basic GUI for the Photon client's statistics features.
/// The shown health values can help identify problems with connection losses or performance.
/// Example: 
/// If the time delta between two consecutive SendOutgoingCommands calls is a second or more,
/// chances rise for a disconnect being caused by this (because acknowledgements to the server
/// need to be sent in due time).
/// </summary>
public class PhotonStatsGui : MonoBehaviour
{
    public Rect WindowRect = new Rect(0, 0, 200, 50);
    public float WidthWithText = 400;
    public bool healthStatsOn;
    public bool trafficStatsOn;
    public bool buttonsOn;

    public bool Visible = false;
    public PhotonPeer Peer { get; set; }

    void Start()
    {
        float width = this.WindowRect.width;
        if (this.trafficStatsOn)
        {
            width = this.WidthWithText;
        }

        this.WindowRect = new Rect(Screen.width - width, 133, width, this.WindowRect.height);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && Input.GetKey(KeyCode.LeftShift))
        {
            this.Visible = !this.Visible;
        }
    }

    void OnGUI()
    {
        if (!this.Visible)
        {
            return;
        }

        if (this.Peer == null)
        {
            this.WindowRect = GUILayout.Window(10, this.WindowRect, this.NetSimHasNoPeerWindow, "Messages (shift+tab)");
        }
        else
        {
            this.WindowRect = GUILayout.Window(10, this.WindowRect, this.TrafficStatsWindow, "Messages (shift+tab)");
        }
    }

    void NetSimHasNoPeerWindow(int windowId)
    {
        GUILayout.Label("No peer to communicate with. ");
    }

    void TrafficStatsWindow(int windowID)
    {
        bool statsToLog = false;
        TrafficStatsGameLevel gls = this.Peer.TrafficStatsGameLevel;
        long elapsedMs = this.Peer.TrafficStatsElapsedMs / 1000;
        if (elapsedMs == 0)
        {
            elapsedMs = 1;
        }

        GUILayout.BeginHorizontal();
        this.buttonsOn = GUILayout.Toggle(this.buttonsOn, "buttons");
        if (this.Peer.TrafficStatsEnabled != this.Visible)
        {
            this.Peer.TrafficStatsEnabled = this.Visible;
        }

        this.trafficStatsOn = GUILayout.Toggle(this.trafficStatsOn, "traffic");
        this.healthStatsOn = GUILayout.Toggle(this.healthStatsOn, "health");
        GUILayout.EndHorizontal();

        string total = string.Format("Out|In|Sum:\t{0,4} | {1,4} | {2,4}", gls.TotalOutgoingMessageCount, gls.TotalIncomingMessageCount, gls.TotalMessageCount);
        string elapsedTime = string.Format("{0}sec average:", elapsedMs);
        string average = string.Format("Out|In|Sum:\t{0,4} | {1,4} | {2,4}", gls.TotalOutgoingMessageCount / elapsedMs, gls.TotalIncomingMessageCount / elapsedMs, gls.TotalMessageCount / elapsedMs);
        GUILayout.Label(total);
        GUILayout.Label(elapsedTime);
        GUILayout.Label(average);

        if (this.buttonsOn)
        {
            GUILayout.BeginHorizontal();
            this.Visible = GUILayout.Toggle(this.Visible, "stats on");
            if (GUILayout.Button("Reset"))
            {
                this.Peer.TrafficStatsReset();
                this.Peer.TrafficStatsEnabled = true;
            }
            statsToLog = GUILayout.Button("To Log");
            GUILayout.EndHorizontal();
        }

        string trafficStatsIn = string.Empty;
        string trafficStatsOut = string.Empty;
        if (this.trafficStatsOn)
        {
            trafficStatsIn = "Incoming: " + this.Peer.TrafficStatsIncoming.ToString();
            trafficStatsOut = "Outgoing: " + this.Peer.TrafficStatsOutgoing.ToString();
            GUILayout.Label(trafficStatsIn);
            GUILayout.Label(trafficStatsOut);
        }

        string healthStats = string.Empty;
        if (this.healthStatsOn)
        {
            healthStats = string.Format(
                "longest delta between\nsend: {0,4}ms disp: {1,4}ms\nlongest time for:\nev({3}):{2,3}ms op({5}):{4,3}ms",
                gls.LongestDeltaBetweenSending,
                gls.LongestDeltaBetweenDispatching,
                gls.LongestEventCallback,
                gls.LongestEventCallbackCode,
                gls.LongestOpResponseCallback,
                gls.LongestOpResponseCallbackOpCode);
            GUILayout.Label(healthStats);
        }

        if (statsToLog)
        {
            string complete = string.Format("{0}\n{1}\n{2}\n{3}\n{4}\n{5}", total, elapsedTime, average, trafficStatsIn, trafficStatsOut, healthStats);
            Debug.Log(complete);
        }

        GUI.DragWindow();
    }
}
