using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_ANDROID
namespace Netflix
{
internal class NetflixAndroidCallback : AndroidJavaProxy
{

    public NetflixAndroidCallback() : base("com.netflix.unity.api.NetflixPluginCallback")
    {
    }

    public void onNfEvent(string eventMessage)
    {
        NfLog.Log("NetflixAndroidCallback onNfEvent " + eventMessage);
        Netflix.NetflixEventSenderImpl.AddNetflixEvent(eventMessage);
    }
}
}
#endif