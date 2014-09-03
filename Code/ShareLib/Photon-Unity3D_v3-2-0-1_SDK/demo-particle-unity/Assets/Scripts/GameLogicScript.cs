// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Exit Games GmbH">
//   Exit Games GmbH, 2012
// </copyright>
// <summary>
// This script must be added to game objects that describe clients
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------
using UnityEngine;
using ExitGames.Client.DemoParticle;
using System.Collections;
using ExitGames.Client.Photon.LoadBalancing;

public class GameLogicScript : MonoBehaviour {

    private GameLogic gameLogic;

    // Initialization code
    void Start () {
        gameLogic = new GameLogic(DemoGUI.ServerAddress, DemoGUI.AppId, DemoGUI.GameVersion);
        gameLogic.Start();

        // Set the client color
        float scale = DemoGUI.playgroundScale / this.gameLogic.GridSize;
        renderer.material.color = DemoGUI.IntToColor(this.gameLogic.LocalPlayer.Color);
        new Vector3(this.gameLogic.LocalPlayer.PosX * transform.localScale.x + transform.localScale.x / 2, scale / 2, this.gameLogic.LocalPlayer.PosY * transform.localScale.y + transform.localScale.y / 2);

    }

    // Update is called once per frame
    void Update () {
        gameLogic.UpdateLoop();
        Move();
    }

    // Update the position of the client
    private void Move()
    {
        if (LocalPlayerJoined())
        {
            if (!renderer.enabled)
            {
                renderer.enabled = true;
            }

            float scale = DemoGUI.playgroundScale / this.gameLogic.GridSize;
            transform.localScale = new Vector3(scale, scale, scale);
            transform.position = new Vector3(this.gameLogic.LocalPlayer.PosX * transform.localScale.x + transform.localScale.x / 2, scale / 2, this.gameLogic.LocalPlayer.PosY * transform.localScale.y + transform.localScale.y / 2);
        }
    }

    void OnGUI()
    {
        if (DemoGUI.ShowUserInfo && LocalPlayerJoined())
        {
            // Get 2D coordinates from 3D coordinates of the client
            Vector3 posVector = Camera.main.WorldToScreenPoint(transform.position);

            GUIStyle labelStyle = new GUIStyle();
            labelStyle.normal.textColor = Color.white;
            if (name == "Local Player")
            {
                labelStyle.fontStyle = FontStyle.Bold;
                labelStyle.normal.textColor = Color.white;
                renderer.material.color = DemoGUI.IntToColor(this.gameLogic.LocalPlayer.Color);
            }
            else
            {
                labelStyle.normal.textColor = Color.gray;
            }

            // Output the client's name
            GUI.Label(new Rect(posVector.x, Screen.height - posVector.y, 100, 20), this.gameLogic.PlayerName, labelStyle);
        }
    }

    /// <summary>
    /// Check if local player has joined the game
    /// </summary>
    /// <returns></returns>
    private bool LocalPlayerJoined()
    {
        GameLogic localPlayerGameLogic = DemoGUI.GetGameLogic("Local Player");
        if (localPlayerGameLogic.State == ClientState.Joined && localPlayerGameLogic.LocalRoom != null && this.gameLogic.State == ClientState.Joined)
        {
            return true;
        }
        return false;
    }

    // Get the client game logic
    public GameLogic GetClientGameLogic()
    {
        return gameLogic;
    }
}