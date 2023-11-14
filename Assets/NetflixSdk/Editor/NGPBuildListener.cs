using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class NGPBuildListener : MonoBehaviour
{
    private static bool DidCleanIOSPlugin = false;
    private static List<LogType> ObservedLogTypes = new List<LogType> { };

    static NGPBuildListener()
    {
        Application.logMessageReceived += OnLogMesssageReceived;
    }

    public static void BuildDidStart(List<LogType> logTypes = null)
    {
        if (logTypes != null)
        {
            ObservedLogTypes = logTypes;
        }
        DidCleanIOSPlugin = false;

        Debug.Log(MethodBase.GetCurrentMethod() + " observed log types:");
        foreach (LogType logType in ObservedLogTypes)
        {
            Debug.Log("logType " + logType);
        }
    }

    private static void OnLogMesssageReceived(string message, string stackTrace, LogType logType)
    {
        List<LogType> observedLogTypes = ObservedLogTypes;
        if (observedLogTypes.Contains(logType))
        {
            Debug.Log(MethodBase.GetCurrentMethod() + "message " + message + " stackTrace " + stackTrace + " type " + logType);

            if (!message.Contains("CocoaPods installation failure") && DidCleanIOSPlugin == false)
            {
                DidCleanIOSPlugin = true;
                NGPBuildTool.NGPCleanIOSPlugin();
            }
        }
    }
}
