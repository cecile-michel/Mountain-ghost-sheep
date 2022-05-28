
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.IO;

public class ScriptBatch : IPostprocessBuildWithReport
{ public int callbackOrder{ get { return 0; } }
[MenuItem("Build/Build for Cellulo (MacOS)")]
public static void BuildGame ()
{
    // Get filename.
    string path = EditorUtility.SaveFilePanel("Choose Location of Built Game", "", "","app");
    List<string> Scenes = new List<string>(); 
    for(int i = 0; i<EditorBuildSettings.scenes.Length;i++)
            Scenes.Add(EditorBuildSettings.scenes[i].path);
    // Build player.
    BuildPipeline.BuildPlayer(Scenes.ToArray(), path, BuildTarget.StandaloneOSX, BuildOptions.ShowBuiltPlayer);
}

public void OnPostprocessBuild(BuildReport report)
{
#if UNITY_STANDALONE_OSX
    // Copy a file from the project folder to the build folder, alongside the built game.
    FileUtil.ReplaceDirectory("Assets/Plugins/macOS/Frameworks", report.summary.outputPath + "/Contents/PlugIns/Frameworks" );
    UnityEngine.Debug.Log("Signing files for MacOS Build");
    //UnityEditor.OSXStandalone.MacOSCodeSigning.CodeSignAppBundle(report.summary.outputPath + "/Contents/PlugIns/Frameworks");
    UnityEditor.OSXStandalone.MacOSCodeSigning.CodeSignAppBundle(report.summary.outputPath);
#endif
    UnityEngine.Debug.Log("MyCustomBuildProcessor.OnPostprocessBuild for target " + report.summary.platform + " at path " + report.summary.outputPath);
    }
}
