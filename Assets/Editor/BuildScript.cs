
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System;
using System.IO;

public class BuildScript
{
    public static void PerformBuildAAB()
    {
        PerformBuild();
    }

    public static void PerformBuildAPK()
    {
        PerformBuild();
    }
    
    public static void PerformBuild()
    {

        string[] scenes = {
            "Assets/Scenes/MainMenuScene.unity",
            "Assets/Scenes/GamePlayScene.unity",
        };

        string aabPath = "Sky Tower.aab";
        string apkPath = "Sky Tower.apk";

        string keystoreBase64 = "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";
        string keystorePass = "tower1";
        string keyAlias = "tower1";
        string keyPass = "tower1";


        string tempKeystorePath = null;

        if (!string.IsNullOrEmpty(keystoreBase64))
        {


            tempKeystorePath = Path.Combine(Path.GetTempPath(), "TempKeystore.jks");
            File.WriteAllBytes(tempKeystorePath, Convert.FromBase64String(keystoreBase64));

            PlayerSettings.Android.useCustomKeystore = true;
            PlayerSettings.Android.keystoreName = tempKeystorePath;
            PlayerSettings.Android.keystorePass = keystorePass;
            PlayerSettings.Android.keyaliasName = keyAlias;
            PlayerSettings.Android.keyaliasPass = keyPass;

            Debug.Log("Android signing configured from Base64 keystore.");
        }
        else
        {
            Debug.LogWarning("Keystore Base64 not set. APK/AAB will be unsigned.");
        }

        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = scenes,
            target = BuildTarget.Android,
            options = BuildOptions.None
        };

        EditorUserBuildSettings.buildAppBundle = true;
        options.locationPathName = aabPath;

        Debug.Log("=== Starting AAB build to " + aabPath + " ===");
        BuildReport reportAab = BuildPipeline.BuildPlayer(options);
        if (reportAab.summary.result == BuildResult.Succeeded)
            Debug.Log("AAB build succeeded! File: " + aabPath);
        else
            Debug.LogError("AAB build failed!");

        EditorUserBuildSettings.buildAppBundle = false;
        options.locationPathName = apkPath;

        Debug.Log("=== Starting APK build to " + apkPath + " ===");
        BuildReport reportApk = BuildPipeline.BuildPlayer(options);
        if (reportApk.summary.result == BuildResult.Succeeded)
            Debug.Log("APK build succeeded! File: " + apkPath);
        else
            Debug.LogError("APK build failed!");

        Debug.Log("=== Build script finished ===");

        if (!string.IsNullOrEmpty(tempKeystorePath) && File.Exists(tempKeystorePath))
        {
            File.Delete(tempKeystorePath);
            Debug.Log("Temporary keystore deleted.");
        }
    }
}

