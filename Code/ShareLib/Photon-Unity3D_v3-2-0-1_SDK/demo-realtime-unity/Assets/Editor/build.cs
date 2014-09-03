using UnityEngine;
using UnityEditor;

public class build : MonoBehaviour
{
	private static string[] scenes = new string[] {"Assets/RealtimeDemoScene.unity"};

	public static void Package()
	{
		AssetDatabase.ExportPackage("Assets/Photon", "photonClient.unitypackage", ExportPackageOptions.Recurse);
	}

	public static void Webplayer()
	{
		BuildPipeline.BuildPlayer(scenes, "DemoRealtime-Web", BuildTarget.WebPlayer, BuildOptions.ShowBuiltPlayer);
	}

	public static void WindowsStandalone()
	{
		BuildPipeline.BuildPlayer(scenes, "DemoRealtime-Win", BuildTarget.StandaloneWindows, 0);
	}

	public static void MacOSStandalone()
	{
		BuildPipeline.BuildPlayer(scenes, "DemoRealtime-OSx", BuildTarget.StandaloneOSXUniversal, 0);
	}

	public static void iPhone()
	{
		#if UNITY_IPHONE
			BuildPipeline.BuildPlayer(scenes, "DemoRealtime-iOS", BuildTarget.iPhone, 0);
		#endif
	}
}