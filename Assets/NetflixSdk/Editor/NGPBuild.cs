using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Callbacks;
using UnityEngine;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif
using Debug = UnityEngine.Debug;
using LogType = UnityEngine.LogType;
using Environment = System.Environment;

public sealed class NGPBuild : IPreprocessBuildWithReport
{
    public int callbackOrder => 1;

    public static bool FlattenXCFramework = true;

    public static readonly string NGPFlattenXCFramework = "NGPFlattenXCFramework";
    public static readonly string NGPIgnoreBuildErrors = "NGPIgnoreBuildErrors";

    public void OnPreprocessBuild(BuildReport report)
    {
        Debug.Log(MethodBase.GetCurrentMethod() + " " + report);
        List<LogType> logTypes = null;
        string flattenOverride = Environment.GetEnvironmentVariable(NGPFlattenXCFramework);
        string ignoreBuildErrors = Environment.GetEnvironmentVariable(NGPIgnoreBuildErrors);
        if (flattenOverride != null)
        {
            FlattenXCFramework = Convert.ToBoolean(flattenOverride);
            Debug.Log(NGPFlattenXCFramework + " overriden to " + FlattenXCFramework);
        }
        if (ignoreBuildErrors != null)
        {
            logTypes = new List<LogType> { LogType.Error, LogType.Exception };
        }
        NGPBuildListener.BuildDidStart(logTypes);
        NGPBuildTool.NGPProcessAllXCFrameworks(FlattenXCFramework);
    }

    [PostProcessBuildAttribute(1)]
    public static void OnPostprocessBuild(BuildTarget buildTarget, string outputPath)
    {
        Debug.Log(MethodBase.GetCurrentMethod() + " " + buildTarget);
#if UNITY_IOS
        if (buildTarget.Equals(BuildTarget.iOS))
        {
            string xcodeProjectPath = outputPath;
            string xcProjectPath = PBXProject.GetPBXProjectPath(xcodeProjectPath);
            PBXProject pbxProject = new PBXProject();
            pbxProject.ReadFromFile(xcProjectPath);

            Debug.Log("Xcode project " + xcProjectPath);

            NGPBuildTool.EmbedPluginFramework(pbxProject, xcodeProjectPath);
            NGPBuildTool.NGPProcessBundles(pbxProject, xcodeProjectPath);
            NGPBuildTool.NGPProcessRecaptcha(pbxProject, xcodeProjectPath);
        }
#endif
        NGPBuildTool.NGPCleanFrameworkLibraries();
    }
}
