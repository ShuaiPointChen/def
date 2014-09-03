// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Exit Games GmbH">
//   Exit Games GmbH, 2012
// </copyright>
// <summary>
// Show the dialog for connection to a master server
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;

public class ConnectionDialog : MonoBehaviour {
    public static int MenuWidth { get; private set; }
    public static int MenuHeight { get; private set; }

    public static int TextBoxWidth { get; private set; }
    public static int TextBoxHeight { get; private set; }

    public static int LabelWidth { get; private set; }
    public static int LabelHeight { get; private set; }

    public static int ButtonWidth { get; private set; }
    public static int ButtonHeight { get; private set; }

    public static int LabelOffset { get; private set; }
    public static int TextBoxOffset { get; private set; }

    public static int guiControlInterval { get; private set; }

    private string masterServerAddress;
    private string appId;
    private string gameVersion;

    private bool showConnectionDialog;

    void Start()
    {
        Application.runInBackground = true;
        masterServerAddress = "app.exitgamescloud.com:5055";
        appId = "<your AppId>";
        gameVersion = "1.0";

        showConnectionDialog = true;

        #if UNITY_ANDROID
            MenuWidth = 400;
            MenuHeight = 200;
            TextBoxWidth = 200;
            TextBoxHeight = 30;
            LabelWidth = 200;
            LabelHeight = TextBoxHeight;
            ButtonWidth = 160;
            ButtonHeight = TextBoxHeight;
            LabelOffset = 20;
            TextBoxOffset = -40;
            guiControlInterval = LabelHeight + 10;
        #else
            MenuWidth = 300;
            MenuHeight = 150;
            TextBoxWidth = 150;
            TextBoxHeight = 20;
            LabelWidth = 150;
            LabelHeight = TextBoxHeight;
            ButtonWidth = 80;
            ButtonHeight = TextBoxHeight;
            LabelOffset = 20;
            TextBoxOffset = -20;
            guiControlInterval = LabelHeight + 10;
        #endif
    }


    // Use this for initialization
    void OnGUI()
    {
        if (showConnectionDialog)
        {
            int i = 1;
            int top = 30;

            GUI.Box(new Rect((Screen.width - MenuWidth) / 2, top, MenuWidth, MenuHeight), "Connect to Master Server");

            GUI.Label(new Rect(Screen.width / 2 - LabelWidth + LabelOffset, top + (guiControlInterval * i++), LabelWidth, LabelHeight), "Server address:");

            GUI.Label(new Rect(Screen.width / 2 - LabelWidth + LabelOffset, top + (guiControlInterval * i++), LabelWidth, LabelHeight), "App ID:");

            GUI.Label(new Rect(Screen.width / 2 - LabelWidth + LabelOffset, top + (guiControlInterval * i++), LabelWidth, LabelHeight), "Game version:");

            i = 1;

            masterServerAddress = GUI.TextField(new Rect(Screen.width / 2 + TextBoxOffset, top + (guiControlInterval * i++), TextBoxWidth, TextBoxHeight), masterServerAddress);

            appId = GUI.TextField(new Rect(Screen.width / 2 + TextBoxOffset, top + (guiControlInterval * i++), TextBoxWidth, TextBoxHeight), appId);

            gameVersion = GUI.TextField(new Rect(Screen.width / 2 + TextBoxOffset, top + (guiControlInterval * i++), TextBoxWidth, TextBoxHeight), gameVersion);

            if (GUI.Button(new Rect((Screen.width - ButtonWidth) / 2, top + (guiControlInterval * i++), ButtonWidth, ButtonHeight), "Connect"))
            {
                GameObject mainCamera = GameObject.Find("Main Camera");
                mainCamera.GetComponent<DemoGUI>().StartGame(masterServerAddress, appId, gameVersion);

                showConnectionDialog = false;
            }
        }

    }
}
