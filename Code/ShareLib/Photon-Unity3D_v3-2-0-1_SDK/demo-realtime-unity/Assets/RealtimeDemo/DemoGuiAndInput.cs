// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DemoGuiAndInput.cs" company="Exit Games GmbH">
// Copyright (C) 2010 Exit Games GmbH
// </copyright>
// <summary>
// This is the "main" script of the Photon Realtime Demo for Unity.
// The demo simply shows how to get a player's position across to other players in the same room. 
// Server side, it uses the Lite application.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using ExitGames.RealtimeDemo;
using UnityEngine;

/// <summary>
/// This class is the melting point between the Unity Engine and the demo's logic, implemented in 
/// the classes Game and Player. Attached to any active GameObject, this script will create and 
/// update a Game instance. There, the actual game logic is implemented. This class does not use
/// the Photon library's classes.
/// 
/// The Game class uses a LitePeer and manages the players but neither handles input nor graphics.
/// The Player class represents each user, position, name and color. It sends it's info when someone joins.
/// </summary>
/// <remarks>
/// The actual handling of Photon is done in the Game and Player classes.
/// </remarks>
public class DemoGuiAndInput : MonoBehaviour
{
    /// <summary>The GameInstance is where Photon is actually handled.</summary>
    public Game GameInstance;

    /// <summary>The model for all players (not just "this" one) which includes a Playername object as well.</summary>
    public Transform playerModel;

    /// <summary>List of PlayerModel instances (each representing a player).</summary>
    private Dictionary<int, Transform> playerTransformDict = new Dictionary<int, Transform>();

    /// <summary>This player's name. In this demo it's simply to display over a player model.</summary>
    public string LocalPlayerName = "Unity";

    /// <summary>time for next input reaction (used for iphone only)</summary>
    private float nextMove;

    /// <summary>
    /// Shows or hides additional options as buttons. These options are just to 
    /// showcase features of Photon but they are not useful in a real game.
    /// </summary>
    private bool showOptions = false;

    /// <summary>
    /// Called by Unity Engine.
    /// On start, we instantiate a Game and make it connect. On iOS devices, we add "iOS" to the 
    /// username that's sent in the player's info.
    /// </summary>
    public void Start()
    {
        // This is essential to keep connections without having focus
        Application.runInBackground = true;

        // This makes clients on iOS easy to distinguish (but it has no real effect aside from the visual one)
        #if UNITY_IPHONE
        LocalPlayerName = LocalPlayerName + " iOS";
        #endif

        // The Game instance handles Photon and encapsulates the game "logic", which is also used by the 
        // DotNet demo client with Windows Forms as GUI.)
        if (this.GameInstance == null)
        {
            DebugReturn("DemoGuiAndInput.GameInstance can't be null.");
        }

        // We will call GameInstance.Update() in this script's Update method but at start, we need to connect.
        // In Connect, we also instantiate the local player.
        this.GameInstance.Connect();

        // This demo makes players send their username and color in addition to the position. 
        // The local player's color is random, the name is set here.
        this.GameInstance.LocalPlayer.name = this.LocalPlayerName;

        // Turn on the light!
        Light l = (Light)GameObject.FindObjectOfType(typeof(Light));
        l.enabled = true;
    }

    /// <summary>
    /// Called by Unity Engine. 
    /// When a client is closed, disconnect from Photon. If this is not done, the server will timeout the connection.
    /// </summary>
    public void OnDisable()
    {
        this.GameInstance.Disconnect();
    }

    /// <summary>
    /// Called by Unity Engine.
    /// Processes the input (which might move the local player), updates the GameInstance and then applies 
    /// positions to the player objects (updating the visual representation).
    /// </summary>
    public void Update()
    {
        if (!this.showOptions)
        {
            this.ProcessInput();
            this.ProcessInputOnMobiles();
        }

        // For security, we will return here if there's no GameInstance (for whatever reason).
        if (this.GameInstance == null)
        {
            return;
        }

        // Update the game and then apply the new positions to the player models this script knows.
        this.GameInstance.Update();
        this.UpdatePositions();
    }

    /// <summary>
    /// Input handling on non-mobile platforms
    /// </summary>
    public void ProcessInput()
    {
        int x = 0, z = 0;
        if (GameInstance == null || GameInstance.Players == null)
        {
            return;
        }

        if (Input.GetKeyDown("e"))
        {
            Debug.Log("E : Exchange (Crypto) Keys");
            GameInstance.OpExchangeKeysForEncryption();
        }
        if (Input.GetKeyDown("r"))
        {
            Game.RaiseEncrypted = !Game.RaiseEncrypted;
            Debug.Log("R : Toggled RaiseEncrypted to: " + Game.RaiseEncrypted);
        }

        if (Input.GetKeyDown("a") || Input.GetKeyDown("left")) x -= 1;
        if (Input.GetKeyDown("d") || Input.GetKeyDown("right")) x += 1;
        if (Input.GetKeyDown("w") || Input.GetKeyDown("up")) z -= 1;
        if (Input.GetKeyDown("s") || Input.GetKeyDown("down")) z += 1;

        GameInstance.LocalPlayer.Move(x, z);	// moves the player x,y steps and checks boundaries
    }

    /// <summary>
    /// Input handling on mobile platforms
    /// </summary>
    public void ProcessInputOnMobiles()
    {
        #if UNITY_IPHONE
        float x = 0, z = 0;

		if (Application.platform != RuntimePlatform.IPhonePlayer)
		{
			x = Input.GetAxisRaw("Horizontal") * Time.deltaTime;
			z = Input.GetAxisRaw("Vertical") * Time.deltaTime;
		}

		// activate accelerometer control only for OSXEditor (via Unity Remote control) and iPhone device
		x = -Input.acceleration.y * Time.deltaTime;
		z = -Input.acceleration.x * Time.deltaTime;

		if(x >  0.01)
			x = 1;
		if(x < -0.01)
			x = -1;
		if(z >  0.01)
			z = 1;
		if(z < -0.01)
			z = -1;
			
		if(Time.time > nextMove)
		{
			nextMove = Time.time + 0.1f;
		}

		GameInstance.LocalPlayer.Move((int)x, (int)z);	// moves the player x,y steps and checks boundaries
        #endif
    }

    public void UpdatePositions()
    {
        if (GameInstance == null || GameInstance.Players == null)
        {
            return;
        }

        // check all instanced player models and remove those without a corresponding player
        int[] transformKeys = new int[playerTransformDict.Count];
        playerTransformDict.Keys.CopyTo(transformKeys, 0);

        foreach (int playerId in transformKeys)
        {
            if (!GameInstance.Players.ContainsKey(playerId))
            {
                DebugReturn("removing " + playerId + " = " + playerTransformDict[playerId].gameObject);
                Destroy(playerTransformDict[playerId].gameObject);
                playerTransformDict.Remove(playerId);
            }
        }

        foreach (Player player in GameInstance.Players.Values)
        {
            Transform playerTransform;

            if (!playerTransformDict.ContainsKey(player.actorNr))
            {
                // create a player Transform instance 
                DebugReturn("Instantiate player #" + player.actorNr);
                playerTransformDict[player.actorNr] = (Transform)Instantiate(playerModel, new Vector3(player.posX, 0, 15 - player.posY), Quaternion.identity);
            }

            playerTransform = playerTransformDict[player.actorNr];

            // set this Transform position
            playerTransform.transform.position = new Vector3(player.posX, 0, 15 - player.posY);
            // set the color and name 
            playerTransform.renderer.material.color = IntToColor(player.color);
            // get the corresponding TextMesh (used to display the name per player)
            TextMesh tm = (TextMesh)playerTransform.GetComponentInChildren(typeof(TextMesh));
            if (tm != null)
            {
                tm.text = player.name + " (" + player.actorNr + ")";
            }
        }
    }

    /// <summary>
    /// Called by Unity Engine.
    /// We display some information here at runtime. The player cubes are handled in UpdatePositions().
    /// A button for "Showcase Options" toggles a window overlay, which is OnGUIWindowOptions().
    /// </summary>
    void OnGUI()
    {
        if (GameInstance == null || GameInstance.Players == null)
        {
            return;
        }

        if (GameInstance.State == PhotonClient.ClientState.InRoom)
        {
            GUILayout.BeginArea(new Rect(2, 10, 200, 150));
            GUILayout.Label(String.Format("State: {0}\nServer: {1}\nRountriptime: {2}[ms]\nPlayers: {3}",
                this.GameInstance.State, this.GameInstance.ServerAddress, this.GameInstance.PeerRoundTripTime, this.GameInstance.Players.Count));
            GUILayout.EndArea();

            if (!this.showOptions)
            {
                this.showOptions = GUI.Button(new Rect(Screen.width / 2 - 70, 10, 140, 24), "Options");
            }
        }
        else
        {
            GUILayout.BeginArea(new Rect(2, 30, Screen.width / 2, Screen.height - 30));
            GUILayout.Label(String.Format("State: {0}\nServer: {1}\n{2}", this.GameInstance.State, this.GameInstance.ServerAddress, this.GameInstance.OfflineReason));
            GUILayout.EndArea();
        }

        if (this.showOptions)
        {
            GUI.Window(1, new Rect(Screen.width / 3, 10, Screen.width / 3, Screen.height / 3), this.OnGUIWindowOptions, "Showcase Options");
        }
    }

    /// <summary>
    /// This Unity window wraps up buttons for additional feature showcases. As such they 
    /// would not be part of a regular game.
    /// </summary>
    /// <param name="id">the window's id. we only use one window.</param>
    private void OnGUIWindowOptions(int id)
    {
        GUILayout.BeginArea(new Rect(2, 20, Screen.width / 3, Screen.height / 3));

        if (GUILayout.Button("Close Options"))
        {
            this.showOptions = false;
        }

        // This button controls the encryption showcase. First off, encryption keys must be exchanged.
        if (!this.GameInstance.PeerIsEncryptionAvailable)
        {
            if (GUILayout.Button("Encryption: Exchange keys"))
            {
                this.GameInstance.OpExchangeKeysForEncryption();
            }
        }
        else
        {
            // After encryption keys were exchanged, you can use encryption. As demo, this is used on positon data.
            if (GUILayout.Button(Game.RaiseEncrypted ? "Enrcyption: Dont" : "Encryption: Do use"))
            {
                Game.RaiseEncrypted = !Game.RaiseEncrypted;
            }
        }

        // Toggle between reliable and unreliable sending of the position updates.
        if (GUILayout.Button(Player.isSendReliable ? "Send pos unreliable" : "Send pos reliable"))
        {
            Player.isSendReliable = !Player.isSendReliable;
        }

        // Toggle visibility of PhotonStatsGui
        PhotonNetSimSettingsGui pnssg = FindObjectOfType(typeof(PhotonNetSimSettingsGui)) as PhotonNetSimSettingsGui;   // better cache this to avoid Find
        if (pnssg != null && GUILayout.Button(pnssg.Visible ? "Hide Network Simulation Settings" : "Show Network Simulation Settings"))
        {
            pnssg.Visible = !pnssg.Visible;
        }

        // Toggle visibility of PhotonNetSimSettingsGui
        PhotonStatsGui psg = FindObjectOfType(typeof(PhotonStatsGui)) as PhotonStatsGui;    // better cache this to avoid Find
        if (psg != null && GUILayout.Button(psg.Visible ? "Hide Photon statistics" : "Show Photon statistics"))
        {
            psg.Visible = !psg.Visible;
        }

        GUILayout.EndArea();
    }

    public void DebugReturn(string debug)
    {
        Debug.Log(debug);
    }

    Int32 ColorToInt(Color c)
    {
        return (((int)c.r) << 16) + (((int)c.g) << 8) + ((int)c.b);
    }

    Color IntToColor(Int32 col)
    {
        if (col == 0)
        {
            return new Color(0f, 0f, 0f, 0.3f);
        }

        return new Color((float)(((double)(col & 0X00FF0000) / 256 / 256) / 256), (float)(((double)(col & 0X0000FF00) / 256) / 256), (float)(((double)(col & 0x000000FF)) / 256));
    }
}