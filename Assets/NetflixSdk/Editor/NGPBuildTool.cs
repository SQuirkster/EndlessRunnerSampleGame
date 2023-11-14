using System.IO;
using System.Reflection;
using UnityEditor;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;
#endif
using UnityEngine;

public class NGPBuildTool
{
    public static readonly string DefaultBuildPath = "build";
    public static readonly string AppleSimulatorUniversalLibraryIdentifier = "ios-arm64_x86_64-simulator";
    public static readonly string DeviceLibraryIdentifier = "ios-arm64";
    public static readonly string IntelSimulatorLibraryIdentifier = "ios-x86_64-simulator";
    public static readonly string IOSPluginPath = "NetflixSdk/Plugins/iOS";
    public static readonly string IOSPluginDir = Path.Combine(Application.dataPath, IOSPluginPath);
    public static readonly string IOSAssetPluginPath = Path.Combine("Assets", IOSPluginPath);
    public static readonly string IOSAssetPluginCachePath = Path.Combine(IOSAssetPluginPath, ".cache");
    public static readonly string IOSEditorPluginPath = Path.Combine(Application.dataPath, "NetflixSdk/Editor");
    public static readonly string IOSPluginCacheDir = Path.Combine(IOSPluginDir, ".cache");
    public static readonly string IOSRecaptchaRelativePath = "NetflixSdk/Recaptcha/iOS";
    public static readonly string IOSAssetRecaptchaPath = Path.Combine(Application.dataPath, IOSRecaptchaRelativePath);
    public static readonly string IOSRecaptchaLibraryPath = Path.Combine("Libraries", IOSRecaptchaRelativePath);
    public static readonly string IOSAssetFirebaseEDMConfigFilePath = Path.Combine(IOSEditorPluginPath, "NetflixDependencies-firebase.xml");
    public static readonly string IOSBuildFrameworkPath = Path.Combine("Frameworks", IOSPluginPath);
    public static readonly string IOSInfoPlistPath = Path.Combine(IOSBuildFrameworkPath, "NGP.framework/Info.plist");

    // Lifted from https://docs.microsoft.com/en-us/dotnet/standard/io/how-to-copy-directories
    private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
    {
        // Get the subdirectories for the specified directory.
        DirectoryInfo dir = new DirectoryInfo(sourceDirName);

        if (!dir.Exists)
        {
            throw new DirectoryNotFoundException(
                "Source directory does not exist or could not be found: "
                + sourceDirName);
        }

        DirectoryInfo[] dirs = dir.GetDirectories();

        // If the destination directory doesn't exist, create it.
        Directory.CreateDirectory(destDirName);

        // Get the files in the directory and copy them to the new location.
        FileInfo[] files = dir.GetFiles();
        foreach (FileInfo file in files)
        {
            string tempPath = Path.Combine(destDirName, file.Name);
            file.CopyTo(tempPath, false);
        }

        // If copying subdirectories, copy them and their contents to new location.
        if (copySubDirs)
        {
            foreach (DirectoryInfo subdir in dirs)
            {
                string tempPath = Path.Combine(destDirName, subdir.Name);
                DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
            }
        }
    }

#if UNITY_IOS
    public static void BuildiOS()
    {
        Debug.Log(MethodBase.GetCurrentMethod());
        BuildiOSWithPlayerOptions(DefaultBuildPlayerOptions());
    }

    public static void BuildiOSWithPlayerOptions(BuildPlayerOptions buildPlayerOptions)
    {
        Debug.Log(MethodBase.GetCurrentMethod());

        NGPProcessAllXCFrameworks(NGPBuild.FlattenXCFramework);

        buildPlayerOptions.targetGroup = BuildTargetGroup.iOS;
        buildPlayerOptions.target = BuildTarget.iOS;
        BuildPipeline.BuildPlayer(buildPlayerOptions);

        string xcProjectPath = PBXProject.GetPBXProjectPath(buildPlayerOptions.locationPathName);
        PBXProject pbxProject = new PBXProject();
        pbxProject.ReadFromFile(xcProjectPath);

        EmbedPluginFramework(pbxProject, buildPlayerOptions.locationPathName);
        NGPProcessBundles(pbxProject, buildPlayerOptions.locationPathName);
        NGPProcessRecaptcha(pbxProject, buildPlayerOptions.locationPathName);
        NGPCleanFrameworkLibraries();
    }
#endif

    public static void BuildPlugin()
    {
        Debug.Log(MethodBase.GetCurrentMethod());
        throw new System.NotImplementedException();
    }

    public static BuildPlayerOptions DefaultBuildPlayerOptions(bool isDebug = true)
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.locationPathName = DefaultBuildPath;
        if (isDebug)
        {
            buildPlayerOptions.options = BuildOptions.Development;
        }
        return buildPlayerOptions;
    }

#if UNITY_IOS
    public static void EmbedPluginFramework(PBXProject pbxProject, string iOSBuildPath = "build")
    {
        string xcProjectPath = PBXProject.GetPBXProjectPath(iOSBuildPath);
        Debug.Log(MethodBase.GetCurrentMethod() + " " + xcProjectPath);
#if (UNITY_EDITOR == UNITY_2019_3_OR_NEWER)
        string unityTargetGUID = pbxProject.GetUnityMainTargetGuid();
        string unityFramework = pbxProject.GetUnityFrameworkTargetGuid();
#else
        string unityTargetGUID = pbxProject.TargetGuidByName(PBXProject.GetUnityTargetName());
        // Hardcode this since soon it won't be needed after bitcode is turned on
        string unityFramework = pbxProject.TargetGuidByName("UnityFramework");
#endif
        pbxProject.SetBuildProperty(unityTargetGUID, "ENABLE_BITCODE", "NO");
        pbxProject.SetBuildProperty(unityFramework, "ENABLE_BITCODE", "NO");
        string frameworksPluginPath = Path.Combine("Frameworks", IOSPluginPath);
        foreach (string framework in NGPFindFrameworks())
        {
            string embedFramework = Path.Combine(frameworksPluginPath, Path.GetFileName(framework));
            Debug.Log("Embedding " + embedFramework);
            string fileGUID = pbxProject.FindFileGuidByProjectPath(embedFramework);
            if (fileGUID != null)
            {
                PBXProjectExtensions.AddFileToEmbedFrameworks(pbxProject, unityTargetGUID, fileGUID);
                Debug.Log("Embedded " + framework);
                pbxProject.WriteToFile(xcProjectPath);
            }
        }
    }
#endif

    public static void ExportPlugin()
    {
        Debug.Log(MethodBase.GetCurrentMethod());

        //AssetDatabase.ExportPackage("Assets/NetflixSdk", "Builds/NetflixPlugin.unitypackage", ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies);
        throw new System.NotImplementedException();
    }

    public static void NGPCleanFrameworkLibraries()
    {
        MethodBase currentMethod = MethodBase.GetCurrentMethod();

        if (Directory.Exists(IOSPluginDir))
        {
            foreach (string frameworkPath in NGPFindFrameworks())
            {
                string frameworkAsset = Path.Combine(IOSAssetPluginPath, Path.GetFileName(frameworkPath));
                AssetDatabase.DeleteAsset(frameworkAsset);
                Debug.Log(currentMethod + " Removed " + frameworkAsset);
            }
        }

        if (Directory.Exists(IOSPluginCacheDir))
        {
            foreach (string cachedXCFrameworkPath in Directory.GetDirectories(IOSPluginCacheDir, "*.xcframework"))
            {
                string ngpXCFrameworkPath = Path.Combine(IOSPluginDir, Path.GetFileName(cachedXCFrameworkPath));
                if (Directory.Exists(cachedXCFrameworkPath) && !Directory.Exists(ngpXCFrameworkPath))
                {
                    Directory.Move(cachedXCFrameworkPath, ngpXCFrameworkPath);
                }
                Debug.Log("Restored " + ngpXCFrameworkPath);
            }
            Directory.Delete(IOSPluginCacheDir, true);
            Debug.Log("Removed cache " + IOSPluginCacheDir);
        }

        AssetDatabase.Refresh();
        Debug.Log("Assets refreshed");
    }

    [MenuItem("NGP/Clean iOS Plugin")]
    public static void NGPCleanIOSPlugin()
    {
        MethodBase currentMethod = MethodBase.GetCurrentMethod();
        Debug.Log(currentMethod + " Clean iOS plugin starting");
        NGPCleanFrameworkLibraries();
        Debug.Log(currentMethod + " Clean iOS plugin complete");
    }

    public static void NGPFilterMetaFilesInDirectory(string dir)
    {
        MethodBase currentMethod = MethodBase.GetCurrentMethod();
        DirectoryInfo frameworkDirInfo = new DirectoryInfo(dir);
        if (frameworkDirInfo.Exists)
        {
            Debug.Log(currentMethod + " Filtering meta files");
            FileInfo[] metaFiles = frameworkDirInfo.GetFiles("*.meta", SearchOption.AllDirectories);
            foreach (FileInfo metaFile in metaFiles)
            {
                metaFile.Delete();
            }
        }
    }

    public static string NGPFrameworkNameFromXCFramework(string xcframeworkPath)
    {
        return Path.GetFileNameWithoutExtension(Path.GetFileName(xcframeworkPath)) + ".framework";
    }

    public static string[] NGPFindBundles()
    {
        return Directory.GetDirectories(IOSPluginDir, "*.bundle");
    }

    public static string[] NGPFindFrameworks()
    {
        return Directory.GetDirectories(IOSPluginDir, "*.framework");
    }

    public static string[] NGPFindXCFrameworks()
    {
        return Directory.GetDirectories(IOSPluginDir, "*.xcframework");
    }

    public static string[] NGPFindFrameworksInXCFramework(string ngpXCFrameworkSlicePath)
    {
        return Directory.GetDirectories(ngpXCFrameworkSlicePath, "*.framework", SearchOption.AllDirectories);
    }

    public static void NGPProcessAllXCFrameworks(bool flatten = false)
    {
        if (Directory.Exists(IOSPluginDir))
        {
            foreach (string xcframework in NGPFindXCFrameworks())
            {
                NGPProcessXCFrameworkLibrary(xcframework, flatten);
            }
        }
    }

#if UNITY_IOS
    public static void NGPProcessBundles(PBXProject pbxProject, string iOSBuildPath = "build")
    {
        MethodBase currentMethod = MethodBase.GetCurrentMethod();
        string xcProjectPath = PBXProject.GetPBXProjectPath(iOSBuildPath);
        Debug.Log(currentMethod + " " + xcProjectPath);
#if (UNITY_EDITOR == UNITY_2019_3_OR_NEWER)
        string unityTargetGUID = pbxProject.GetUnityMainTargetGuid();
#else
        string unityTargetGUID = pbxProject.TargetGuidByName(PBXProject.GetUnityTargetName());
#endif

        foreach (string bundlePath in NGPFindBundles())
        {
            string buildBundlePath = Path.Combine(iOSBuildPath, Path.GetFileName(bundlePath));
            Debug.Log(currentMethod + " bundlePath " + bundlePath);
            Debug.Log(currentMethod + " buildBundlePath " + buildBundlePath);

            if (Directory.Exists(buildBundlePath))
            {
                Directory.Delete(buildBundlePath, true);
                Debug.Log(currentMethod + " deleted existing " + buildBundlePath);
            }

            DirectoryCopy(bundlePath, buildBundlePath, true);

            string bundlePhaseGUID = pbxProject.GetResourcesBuildPhaseByTarget(unityTargetGUID);
            string fileGUID = pbxProject.AddFolderReference(buildBundlePath, buildBundlePath, PBXSourceTree.Source);
            pbxProject.AddFileToBuildSection(unityTargetGUID, bundlePhaseGUID, fileGUID);

            pbxProject.WriteToFile(xcProjectPath);
            Debug.Log(currentMethod + " processed bundle " + bundlePath);
        }
    }

    public static void NGPProcessRecaptcha(PBXProject pbxProject, string xcodeProjectPath)
    {
        MethodBase currentMethod = MethodBase.GetCurrentMethod();
        Debug.Log(currentMethod + " xcodeProjectPath " + xcodeProjectPath);

        if (PlayerSettings.iOS.sdkVersion.Equals(iOSSdkVersion.SimulatorSDK))
        {
            Debug.Log(currentMethod + " skip recaptcha processing on simulator");
            return;
        }

        if (!NGPShouldProcessRecaptcha(xcodeProjectPath))
        {
            Debug.Log(currentMethod + " skip recaptcha processing.");
            return;
        }

        string IOSNetflixDependenciesPath = Path.Combine(IOSEditorPluginPath, "NetflixDependencies.xml");
        Debug.Log(currentMethod + " copying " + IOSAssetFirebaseEDMConfigFilePath + " to " + IOSNetflixDependenciesPath);
        File.Copy(IOSAssetFirebaseEDMConfigFilePath, IOSNetflixDependenciesPath, true);

#if (UNITY_EDITOR == UNITY_2019_3_OR_NEWER)
        string unityFrameworkGUID = pbxProject.GetUnityFrameworkTargetGuid();
        string unityTargetGUID = pbxProject.GetUnityMainTargetGuid();

#else
        string unityFrameworkGUID = pbxProject.TargetGuidByName(PBXProject.GetUnityFrameworkName());
        string unityTargetGUID = pbxProject.TargetGuidByName(PBXProject.GetUnityTargetName());

#endif
        string buildFrameworksPath = Path.Combine(xcodeProjectPath, IOSBuildFrameworkPath);
        Debug.Log(currentMethod + " buildFrameworksPath " + buildFrameworksPath);
        pbxProject.AddBuildProperty(unityFrameworkGUID, "FRAMEWORK_SEARCH_PATHS", buildFrameworksPath);
        pbxProject.AddFrameworkToProject(unityFrameworkGUID, "JavaScriptCore.framework", false);
        pbxProject.AddFrameworkToProject(unityFrameworkGUID, "GLKit.framework", false);

        string recaptchaPluginPath = Path.Combine(IOSAssetRecaptchaPath, "recaptcha.framework");
        string projectFrameworkPath = Path.Combine(xcodeProjectPath, IOSBuildFrameworkPath);
        string targetFrameworkRecaptchaPath = Path.Combine(projectFrameworkPath, "recaptcha.framework");

        pbxProject.SetBuildProperty(unityTargetGUID, "OTHER_LDFLAGS", "");
        pbxProject.AddBuildProperty(unityTargetGUID, "FRAMEWORK_SEARCH_PATHS", buildFrameworksPath);

        Debug.Log(currentMethod + " copying " + recaptchaPluginPath + " to " + targetFrameworkRecaptchaPath);
        DirectoryCopy(recaptchaPluginPath, targetFrameworkRecaptchaPath, true);

        NGPFilterMetaFilesInDirectory(projectFrameworkPath);

        pbxProject.AddFileToBuild(unityFrameworkGUID, pbxProject.AddFile(Path.Combine(IOSBuildFrameworkPath, "recaptcha.framework"), targetFrameworkRecaptchaPath, PBXSourceTree.Source));
        pbxProject.SetBuildProperty(unityTargetGUID, "SWIFT_VERSION", "5.0");

        pbxProject.WriteToFile(PBXProject.GetPBXProjectPath(xcodeProjectPath));
    }
#endif

    public static void NGPProcessXCFrameworkLibrary(string ngpXCFrameworkPath, bool flatten = false)
    {
        Debug.Log(MethodBase.GetCurrentMethod());
        if (Directory.Exists(ngpXCFrameworkPath))
        {
            Debug.Log("Found XCFramework " + ngpXCFrameworkPath);

            bool isSimulatorTarget = (PlayerSettings.iOS.sdkVersion == iOSSdkVersion.SimulatorSDK);
            bool isAppleSilicon = SystemInfo.processorType.StartsWith("Apple");

            string simulatorLibraryIdentifier = isAppleSilicon ? AppleSimulatorUniversalLibraryIdentifier : IntelSimulatorLibraryIdentifier;
            if (isSimulatorTarget && !isAppleSilicon)
            {
                if (Directory.Exists(Path.Combine(Path.Combine(ngpXCFrameworkPath, AppleSimulatorUniversalLibraryIdentifier), NGPFrameworkNameFromXCFramework(ngpXCFrameworkPath))))
                {
                    //Use AppleSimulatorUniversalLibraryIdentifier when present
                    simulatorLibraryIdentifier = AppleSimulatorUniversalLibraryIdentifier;
                }
            }

            string libaryIdentifierDir = isSimulatorTarget ? simulatorLibraryIdentifier : DeviceLibraryIdentifier;
            string ngpXCFrameworkPathCache = Path.Combine(IOSPluginCacheDir, Path.GetFileName(ngpXCFrameworkPath));

            if (!Directory.Exists(IOSPluginCacheDir))
            {
                Debug.Log("Creating cache dir " + IOSPluginCacheDir);
                Directory.CreateDirectory(IOSAssetPluginCachePath);
            }
            else
            {
                if (Directory.Exists(ngpXCFrameworkPathCache))
                {
                    Debug.Log("Removing previous cached framework " + ngpXCFrameworkPathCache);
                    Directory.Delete(ngpXCFrameworkPathCache, true);
                }
            }

            Debug.Log("Caching " + ngpXCFrameworkPath);
            DirectoryCopy(ngpXCFrameworkPath, ngpXCFrameworkPathCache, true);

            Debug.Log("Deleting asset " + ngpXCFrameworkPath);
            AssetDatabase.DeleteAsset(Path.Combine(IOSAssetPluginPath, Path.GetFileName(ngpXCFrameworkPath)));

            string[] frameworks;
            if (flatten)
            {
                frameworks = NGPFindFrameworksInXCFramework(Path.Combine(ngpXCFrameworkPathCache, libaryIdentifierDir));
            }
            else
            {
                frameworks = new string[] { (Path.Combine(Path.Combine(ngpXCFrameworkPathCache, libaryIdentifierDir), NGPFrameworkNameFromXCFramework(ngpXCFrameworkPath))) };
            }

            foreach (string frameworkPath in frameworks)
            {
                Debug.Log("Processing frameworkPath " + frameworkPath);

                if (!Directory.Exists(frameworkPath))
                {
                    throw new System.PlatformNotSupportedException(frameworkPath + " not included in " + ngpXCFrameworkPath);
                }

                string pluginFrameworkPath = Path.Combine(IOSPluginDir, Path.GetFileName(frameworkPath));
                if (Directory.Exists(pluginFrameworkPath))
                {
                    Debug.Log("Removing previous framework " + pluginFrameworkPath);
                    _ = AssetDatabase.DeleteAsset(Path.Combine(IOSAssetPluginPath, Path.GetFileName(pluginFrameworkPath)));
                }

                Debug.Log("Staging framework " + frameworkPath + ", copying " + frameworkPath + " to  " + pluginFrameworkPath);
                DirectoryCopy(frameworkPath, pluginFrameworkPath, true);

                if (flatten)
                {
                    string embeddedFrameworksPath = Path.Combine(pluginFrameworkPath, "Frameworks");
                    string embeddedFrameworksMetaPath = Path.Combine(pluginFrameworkPath, "Frameworks.meta");
                    if (Directory.Exists(embeddedFrameworksPath))
                    {
                        Debug.Log("Removing embedded frameworks " + embeddedFrameworksPath);
                        Directory.Delete(embeddedFrameworksPath, true);
                        if (File.Exists(embeddedFrameworksMetaPath))
                        {
                            File.Delete(embeddedFrameworksMetaPath);
                        }
                    }
                }

                Debug.Log("Importing asset " + pluginFrameworkPath);
                AssetDatabase.ImportAsset(Path.Combine(IOSAssetPluginPath, Path.GetFileName(pluginFrameworkPath)));
            }
        }
    }
#if UNITY_IOS
    public static bool NGPShouldProcessRecaptcha(string xcodeProjectPath)
    {
        MethodBase currentMethod = MethodBase.GetCurrentMethod();

        string infoPListPath = Path.Combine(xcodeProjectPath, IOSInfoPlistPath);
        Debug.Log(currentMethod + " plist: " + infoPListPath);

        PlistDocument plistDoc = new PlistDocument();
        plistDoc.ReadFromFile(infoPListPath);

        bool processRecaptcha = false;
        PlistElementDict rootDict = plistDoc.root;
        PlistElement prop = rootDict["unityShouldLinkRecaptcha"];
        if (prop != null)
        {
            processRecaptcha = prop.AsBoolean();
        }

        Debug.Log(currentMethod + ": " + processRecaptcha);
        return processRecaptcha;
    }
#endif
}
