// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Exit Games GmbH">
//   Protocol & Photon Client Lib - Copyright (C) 2010 Exit Games GmbH
// </copyright>
// <summary>
// Part of the "Demo Lobby" for Photon in Unity.
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The ChatGui will handle gui and all input of a user, which will trigger actions /use methods on the ChatPhotonClient.
/// </summary>
public class ChatGui : MonoBehaviour {
    /// <summary>The chat photon client. This is the currently controlled client for chat. The demo allows you to control more than one, for testing reasons.</summary>
    public ChatPhotonClient ChatPhotonClient;
    /// <summary>The list if chat clients that are currently available to control. Each runs autonomously as MonoBehaviour so it does not disconnect.</summary>
    public List<ChatPhotonClient> ChatClientList = new List<ChatPhotonClient>();
    private Vector2 ChatScrollPosition = Vector2.zero;      // scroll positioning for gui
    private Vector2 UsernameScrollPosition = Vector2.zero;  // scroll positioning for gui
    private string InputLine = string.Empty;        // stores input for gui
    private string UserNameInput = "my name";       // stores input for gui
    private const int MaxInput = 12;    // max number of chars for username and roomname
    public GUIStyle HiglightStyle;      // used to highlight "this" username inside room
    public bool DebugInputEnabled = true;
    private string DebugHelptext = "This demo can run multiple clients in \"debug mode\" (active). Use keys:\nInsert/Delete: add/remove users\nPageUp/PageDown: switch users";

    /// <summary>
    /// Called by Unity after this MonoBehaviour was created. This will put the initial ChatPhotonClient into the list.
    /// </summary>
    void Start()
    {
        this.ChatClientList.Add(this.ChatPhotonClient);
    }

    /// <summary>
    /// Called by Unity Engine.
    /// </summary>
    void OnGUI()
    {
        //GUI.DrawTexture(logoRect, logo);

        // here the input handling for debug features is done);
	    this.HandleDebugInput();

        // the ChatPhotonClient implements a simple state machine and the ChatGui will show the fitting screen
        switch (this.ChatPhotonClient.ChatState)
        {
            case ChatPhotonClient.ChatStateOption.Offline:
                GUILayout.BeginArea(new Rect(Screen.width / 4, Screen.height / 3, Screen.width / 2, Screen.height / 2));
                GUILayout.Label("Chatroom and Lobby Demo", HiglightStyle);
                GUILayout.Label("Offline. ServerAddress: " + this.ChatPhotonClient.ServerAddress, HiglightStyle);
                GUILayout.Label(this.ChatPhotonClient.OfflineReason);
                if (GUILayout.Button("Connect"))
                {
                    this.ChatPhotonClient.Connect();
                }
                GUILayout.EndArea();
                break;
            case ChatPhotonClient.ChatStateOption.OutsideOfRooms:
                bool joinRoom = false;

                GUILayout.BeginArea(new Rect(Screen.width / 3, Screen.height / 3, Screen.width / 3, Screen.height / 2));
                GUILayout.Label("Chatroom and Lobby Demo", HiglightStyle);
                GUILayout.Label("Enter a nickname:");
                this.UserNameInput = GUILayout.TextField(this.UserNameInput, MaxInput);
                joinRoom = GUILayout.Button("Proceed to Lobby");
                if (DebugInputEnabled)
                {
                    GUILayout.Label(DebugHelptext);
                }
                GUILayout.EndArea();

                // if the user pressed enter or the button, we proceed to the lobby. this demo always uses lobby "chat_lobby"
                if (joinRoom || Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Return)
                {
                    this.ChatPhotonClient.JoinLobby("chat_lobby");
                    this.ChatPhotonClient.UserName = this.UserNameInput;
                    this.InputLine = "my chatroom";
                }
                break;

            case ChatPhotonClient.ChatStateOption.InLobbyRoom:
                // this screen shows available chat-rooms within our current lobby. the user can select one or enter a new roomname to join
                string roomToJoin = null;
                GUILayout.BeginArea(new Rect(Screen.width / 3, Screen.height / 3, Screen.width / 3, Screen.height / 2));
                GUILayout.Label("Lobby", HiglightStyle);
                GUILayout.Label(this.ChatPhotonClient.UserName + ", select a room, or enter room name.");
                foreach (string roomName in this.ChatPhotonClient.RoomHashtable.Keys)
                {
                    string buttonText = String.Format("{0} ({1})", roomName, this.ChatPhotonClient.RoomHashtable[roomName]);
                    if (GUILayout.Button(buttonText))
                    {
                        roomToJoin = roomName;
                    }
                }
                // input for "new room"
                this.InputLine = GUILayout.TextField(this.InputLine, MaxInput);
                if (GUILayout.Button("Enter") || (Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Return))
                {
                    roomToJoin = this.InputLine;
                }
                GUILayout.EndArea();

                // we join a room when either a button was pressed or 
                if (!String.IsNullOrEmpty(roomToJoin))
                {
                    // the "wrapper" method in ChatPhotonClient calls the operation, after setting more parameters
                    this.ChatPhotonClient.JoinRoomFromLobby(roomToJoin);
                    this.InputLine = String.Empty;
                }
                break;

            case ChatPhotonClient.ChatStateOption.InChatRoom:
                // handle input via Return.key:
                if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
                {
                    this.ChatPhotonClient.SendChatMessage(this.InputLine);
                    this.InputLine = String.Empty;
                }

                // the screen is divided in two areas: left is the chat and input field, right is a list of users and a leave button
                int chatWidth = (int)(Screen.width * 0.75f);
                GUILayout.BeginArea(new Rect(0, 0, chatWidth, Screen.height));
                this.ChatScrollPosition = GUILayout.BeginScrollView(this.ChatScrollPosition);
                GUILayout.TextArea(this.ChatPhotonClient.ChatLines.ToString(), GUILayout.ExpandHeight(true));
                GUILayout.EndScrollView();
                this.InputLine = GUILayout.TextField(this.InputLine);
                GUILayout.EndArea();

                //right side: users and leave button
                GUILayout.BeginArea(new Rect(chatWidth, 5, Screen.width - chatWidth, Screen.height-5));
                GUILayout.Box("Users");
                this.UsernameScrollPosition = GUILayout.BeginScrollView(this.UsernameScrollPosition);
                if (this.ChatPhotonClient.ActorNumbersInRoom != null)
                {
                    foreach (int actorNr in this.ChatPhotonClient.ActorNumbersInRoom)
                    {
                        string name = this.ChatPhotonClient.GetActorPropertyNameOf(actorNr);
                        if (this.ChatPhotonClient.ActorNumber == actorNr)
                        {
                            GUILayout.Label(name, HiglightStyle);
                        } 
                        else
                        {
                            GUILayout.Label(name);
                        }
                    }
                }
                GUILayout.EndScrollView();
                if (GUILayout.Button(String.Format("Leave \"{0}\"", this.ChatPhotonClient.RoomName)))
                {
                    this.UserNameInput = this.ChatPhotonClient.UserName;    // prepare the input for this user once more
                    this.ChatPhotonClient.LeaveRoom();
                }
                GUILayout.EndArea();
                break;
        }
	}

    /// <summary>
    /// Handles input for "multi-client" behaviour, which is useful during development.
    /// You can connect multiple times in one client and switch through the clients with Page Up/Down keys.
    /// Insert and Delete will add and remove clients accordingly.
    /// These keys are valid all the time.
    /// All of these key events are Used at the end of this method. Any other goes through.
    /// You could turn off this with a single checkbox inside the Unity Editor in the Chat Gui Script.
    /// </summary>
    private void HandleDebugInput()
    {
        // if debug was turned off in Editor: return
        if (!DebugInputEnabled)
        {
            return;
        }

        // Currently, we don't have GUI elements to control the simulated clients.
        if (Event.current.type == EventType.KeyDown)
        {
            int i;
            bool handled = true;    //the default case below turns handled to false for any key that's not in the switch
            switch (Event.current.keyCode)
            {
                case KeyCode.Insert:
                    ChatPhotonClient newClient = (ChatPhotonClient)this.gameObject.AddComponent(typeof(ChatPhotonClient));
                    // a bit of cheating: we "copy" the current server address (set in editor) over the default address (set in code)
                    newClient.ServerAddress = this.ChatPhotonClient.ServerAddress;

                    this.ChatClientList.Add(newClient);
                    break;
                case KeyCode.Delete:
                    if (this.ChatClientList.Count <= 1)
                    {
                        //don't delete current client if only one is left
                        break;
                    }
                    i = this.ChatClientList.IndexOf(this.ChatPhotonClient);
                    this.ChatClientList.Remove(this.ChatPhotonClient);
                    this.ChatPhotonClient.Disconnect();             //disconnect is sent out immediately (includes SendOutgoing-call)
                    GameObject.Destroy(this.ChatPhotonClient);      //so we can destroy this object/client immediately
                    if (i > 0)
                    {
                        i--;
                    }
                    this.ChatPhotonClient = this.ChatClientList[i]; //if there is a "previous" element in the list, use that
                    break;
                case KeyCode.PageUp:
                    i = this.ChatClientList.IndexOf(this.ChatPhotonClient);
                    if (i > 0)
                    {
                        this.ChatPhotonClient = this.ChatClientList[i - 1];
                    }
                    break;
                case KeyCode.PageDown:
                    i = this.ChatClientList.IndexOf(this.ChatPhotonClient);
                    if (i < this.ChatClientList.Count - 1)
                    {
                        this.ChatPhotonClient = this.ChatClientList[i + 1];
                    }
                    break;
                default:
                    handled = false;
                    break;
            }

            // None of the debug keys should trigger a reaction in the gui anymore:
            if (handled)
            {
                this.ChatPhotonClient.DebugReturn("Now controlling ActorNumber: " + this.ChatPhotonClient.ActorNumber);
                Event.current.Use();
            }
        }
    }
}
