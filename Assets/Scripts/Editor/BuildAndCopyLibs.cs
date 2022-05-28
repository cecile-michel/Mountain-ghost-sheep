using UnityEditor;
using System.Collections.Generic;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class BuildAndCopyLibs 
{
    [MenuItem("Build/Build for Cellulo (Linux)")]
    public static void BuildGame ()
    {
        // Get filename.
        string path = EditorUtility.SaveFilePanel("Choose Location of Built Game", "", "","");
        
        List<string> Scenes = new List<string>(); 
        for(int i = 0; i<EditorBuildSettings.scenes.Length;i++){
            Scenes.Add(EditorBuildSettings.scenes[i].path);
        }
        // Build player.
        BuildReport  report = BuildPipeline.BuildPlayer(Scenes.ToArray(), path,BuildTarget.StandaloneLinux64, BuildOptions.ShowBuiltPlayer);
        BuildSummary summary = report.summary;

        // Copy a file from the project folder to the build folder, alongside the built game.
        FileUtil.CopyFileOrDirectory("Assets/Plugins/Linux/opt", path +"_Data/Plugins/opt" );
        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.LogError("Build failed");
        }
    }
}