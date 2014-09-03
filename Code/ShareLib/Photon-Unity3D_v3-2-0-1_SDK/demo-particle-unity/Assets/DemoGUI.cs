// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Exit Games GmbH">
//   Exit Games GmbH, 2012
// </copyright>
// <summary>
//  Creates and controls the behaviour of the interface elements
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------
using UnityEngine;
using System.Collections.Generic;
using ExitGames.Client.Photon.LoadBalancing;
using ExitGames.Client.DemoParticle;
using System.Threading;

public class DemoGUI : MonoBehaviour {

    // Connection parameters
    public static string ServerAddress;
    public static string AppId;
    public static string GameVersion;

    // Main menu parameters
    public static bool ShowUserInfo { get; private set; }
    public static bool ShowMainMenu { get; private set; }

    private static int DefaultButtonWidth { get; set; }
    private static int DefaultButtonHeight { get; set; }
    private static int IntervalBetweenButtons { get; set; }
    
    // Game logic parameters
    public GameObject client;
    private GameObject localPlayer;
    private Queue<GameObject> backgroundGames;
    private string localPlayerName;

    private static bool AutomoveEnabled { get; set; }
    private static bool MultipleRoomsEnabled { get; set; }
    private bool IsGameStarted { get; set; }

    public static int playgroundScale { get; private set; }

    // Initialization code 
    void Start () {

        // Queue for background games
        backgroundGames = new Queue<GameObject>();

        ShowMainMenu = true;
        ShowUserInfo = true;

        AutomoveEnabled = true;

        #if UNITY_ANDROID
            DefaultButtonWidth = 100;
            DefaultButtonHeight = 60;
        #else
            DefaultButtonWidth = 40;
            DefaultButtonHeight = 40;
        #endif

        IntervalBetweenButtons = 10;

        DemoGUI.playgroundScale = (int) GameObject.Find("Playground").transform.localScale.x;
            
	}

    // 
    /// <summary>
    /// This function is called when the user clicks the 'Connection' button 
    /// </summary>
    /// <seealso cref="ConnectionDialog.cs"/>
    public void StartGame(string serverAddress, string appId, string gameVersion)
    {
        DemoGUI.ServerAddress = serverAddress;
        DemoGUI.AppId = appId;
        DemoGUI.GameVersion = gameVersion;

        // Create local player
        localPlayer = (GameObject)Instantiate(client);
        localPlayer.name = "Local Player";
        localPlayer.renderer.enabled = false; // don't show the local player until he joined the game
        IsGameStarted = true;   
    }
	
	// Update is called once per frame
	void Update () {

        // Show or hide main menu when 'Tab' is pressed
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            ShowMainMenu = !ShowMainMenu;
        }        
	}

    // Draw the GUI
    void OnGUI()
    {

        if (ShowMainMenu && IsGameStarted)
        {
            // Get local's player game logic
            GameLogic localPlayerGameLogic = GetGameLogic("Local Player");

            #region menu buttons
            int i = 0;
            int buttonSpace = IntervalBetweenButtons + DefaultButtonWidth;
            // Automove button
            if (AutomoveEnabled)
            {
                if (GUI.Button(new Rect(buttonSpace * (i++) + IntervalBetweenButtons, 10, DefaultButtonWidth, DefaultButtonHeight), Resources.Load("automove off") as Texture))
                {
                    AutomoveEnabled = !AutomoveEnabled;
                    localPlayerGameLogic.MoveInterval.IsEnabled = !localPlayerGameLogic.MoveInterval.IsEnabled;
                }
            }
            else
            {
                if (GUI.Button(new Rect(buttonSpace * (i++) + IntervalBetweenButtons, 10, DefaultButtonWidth, DefaultButtonHeight), Resources.Load("automove on") as Texture))
                {
                    AutomoveEnabled = !AutomoveEnabled;
                    localPlayerGameLogic.MoveInterval.IsEnabled = !localPlayerGameLogic.MoveInterval.IsEnabled;
                }
            }

            // Interest management button
            if (GUI.Button(new Rect(buttonSpace * (i++) + IntervalBetweenButtons, 10, DefaultButtonWidth, DefaultButtonHeight), Resources.Load("interest management icon") as Texture))
            {
                localPlayerGameLogic.SetUseInterestGroups(!localPlayerGameLogic.UseInterestGroups);

                foreach (GameObject backgroundGame in backgroundGames)
                {
                    GameLogic gameLogic = GetGameLogic(backgroundGame);
                    gameLogic.SetUseInterestGroups(gameLogic.UseInterestGroups);
                }

                SetPlaygroundTexture();
            }

            // Change color button
            if (GUI.Button(new Rect(buttonSpace * (i++) + IntervalBetweenButtons, 10, DefaultButtonWidth, DefaultButtonHeight), Resources.Load("change color icon") as Texture))
            {
                localPlayerGameLogic.ChangeLocalPlayercolor();    
            }

            // Show user information button
            if (ShowUserInfo)
            {
                if (GUI.Button(new Rect(buttonSpace * (i++) + IntervalBetweenButtons, 10, DefaultButtonWidth, DefaultButtonHeight), Resources.Load("client info icon") as Texture))
                {
                    ShowUserInfo =! ShowUserInfo;
                }
            } 
            else
            {
                if (GUI.Button(new Rect(buttonSpace * (i++) + IntervalBetweenButtons, 10, DefaultButtonWidth, DefaultButtonHeight), Resources.Load("client info icon") as Texture))
                {
                    ShowUserInfo =! ShowUserInfo;
                }
            }

            // Add client button
            if (GUI.Button(new Rect(buttonSpace * (i++) + IntervalBetweenButtons, 10, DefaultButtonWidth, DefaultButtonHeight), Resources.Load("add client icon") as Texture))
            {
                AddClient();
            }

            // Remove client button
            if (GUI.Button(new Rect(buttonSpace * (i++) + IntervalBetweenButtons, 10, DefaultButtonWidth, DefaultButtonHeight), Resources.Load("remove client icon") as Texture))
            {
                RemoveClient();
            }

            // Change grid size button
            if (GUI.Button(new Rect(buttonSpace * (i++) + IntervalBetweenButtons, 10, DefaultButtonWidth, DefaultButtonHeight), Resources.Load("grid icon") as Texture))
            {
                localPlayerGameLogic.ChangeGridSize();

                 foreach (GameObject backgroundGame in backgroundGames)
                {
                    GameLogic gameLogic = GetGameLogic(backgroundGame);
                    gameLogic.ChangeGridSize();
                }

                 SetPlaygroundTexture();
            }
            #endregion
        }
    }

    // Add new client to the scene
    void AddClient()
    {
        if (!IsLocalPlayerInGame())
        {
            return;
        }
        GameObject addedClient = (GameObject) Instantiate(client);
        addedClient.renderer.enabled = false;
        backgroundGames.Enqueue(addedClient);
    }

    // Remove last added client
    void RemoveClient()
    {
        if (backgroundGames.Count > 0)
        {
            GameObject removedClient = backgroundGames.Dequeue();
            DemoGUI.GetGameLogic(removedClient).Disconnect();
            Destroy(removedClient);    
        }
    }

    // Set the playground texture
    // Current texture depends on interest management setting 
    private void SetPlaygroundTexture()
    {
        GameObject playground = GameObject.Find("Playground");

        Material playgroundMaterial;
        Texture texture;
        float textureScale;

        Material playgroundGridMaterial;
        Texture gridTexture;
        float gridTextureScale;

        Material[] materials;

        materials = playground.renderer.materials;
        
        playgroundMaterial = new Material(Shader.Find("Diffuse"));

        playgroundGridMaterial = new Material(Shader.Find("Transparent/Diffuse"));
        gridTexture = Resources.Load("interest management grid") as Texture;
        gridTextureScale = DemoGUI.GetGameLogic("Local Player").GridSize;
        playgroundGridMaterial.mainTexture = gridTexture;

        if (GetGameLogic("Local Player").UseInterestGroups)
        {
            texture = Resources.Load("interest groups enabled texture") as Texture;
            textureScale = 1.0f;

            materials[0] = playgroundMaterial;
            materials[0].mainTexture = texture;
            materials[0].mainTextureScale = new Vector2(textureScale, textureScale);
            
        }
        else
        {
            texture = Resources.Load("interest groups disabled texture") as Texture;
            textureScale = DemoGUI.GetGameLogic("Local Player").GridSize;
            
            materials[0] = playgroundMaterial;
            materials[0].mainTexture = texture;
            materials[0].mainTextureScale = new Vector2(textureScale, textureScale);
        }

        materials[1] = playgroundGridMaterial;
        materials[1].mainTexture = gridTexture;
        materials[1].mainTextureScale = new Vector2(gridTextureScale, gridTextureScale);

        texture.wrapMode = TextureWrapMode.Repeat;
        gridTexture.wrapMode = TextureWrapMode.Repeat;

        playground.renderer.materials = materials;
    }

    // Check if the local player joined the game
    private bool IsLocalPlayerInGame()
    {
        return DemoGUI.GetGameLogic("Local Player").LocalPlayer != null ? true : false;
    }
    
    // Get the client game by name
    public static GameLogic GetGameLogic(string clientName)
    {
        return GameObject.Find(clientName).GetComponent<GameLogicScript>().GetClientGameLogic();
    }

    // Get the client game by object reference
    public static GameLogic GetGameLogic(GameObject clientObject)
    {
        return clientObject.GetComponent<GameLogicScript>().GetClientGameLogic();
    }

    public static Color IntToColor(int colorValue)
    {     
        float r = (byte)(colorValue >> 16) / 255.0f; 
        float g = (byte)(colorValue >> 8) / 255.0f; 
        float b = (byte)colorValue / 255.0f;

        return new Color(r, g, b);
    }
}