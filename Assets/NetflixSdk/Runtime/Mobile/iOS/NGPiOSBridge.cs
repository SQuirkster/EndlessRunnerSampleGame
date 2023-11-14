#if UNITY_IOS

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;

namespace Netflix
{
    delegate void NGPEventDispatcherFuncType(string eventMessage);
    delegate void NGPConfigurationCallbackFuncType(string configMessage);
    delegate void NGPCloudSaveDispatcherFuncType(string resultMessage);
    delegate void NGPLeaderboardCallbackFuncType(int correlationId, string resultMessage);
    delegate void NGPUnlockAchievementCallbackFuncType(int correlationId, string resultMessage);
    delegate void NGPGetAchievementsCallbackFuncType(int correlationId, string resultMessage);
    delegate void NGPCurrentPlayerCallbackFuncType(string playerMessage);
    delegate void NGPGetPlayerIdentitiesCallbackFuncType(string resultMessage);

    internal sealed class NGPiOSBridge : NetflixSdkImpl
    {
        class Config
        {
            public string ngpUnitySdkVer;
            public string ngpNativeSdkVer;
            public string unitySdkVersion;
        }

        static SdkApi.CrashReporterConfig CrashReporterConfig;
        static Config iOSConfig;

        protected override SdkApi.ILeaderboards CreateLeaderboardsApi()
        {
            return new NGPiOSLeaderboards();
        }

        protected override SdkApi.IStats CreateStatsApi()
        {
            return new NGPiOSStats();
        }

        protected override SdkApi.IAchievements CreateAchievementsApi()
        {
            return new NGPiOSAchievements();
        }

        [MonoPInvokeCallback(typeof(NGPEventDispatcherFuncType))]
        public static void NGPDispatchEvent(string eventMessage)
        {
            NfLog.Log(MethodBase.GetCurrentMethod() + " " + eventMessage);
            NetflixEventSenderImpl.AddNetflixEvent(eventMessage);
        }

        [MonoPInvokeCallback(typeof(NGPConfigurationCallbackFuncType))]
        public static void NGPConfig(string configMessage)
        {
            NfLog.Log(MethodBase.GetCurrentMethod() + " " + configMessage);
            CrashReporterConfig = JsonUtility.FromJson<SdkApi.CrashReporterConfig>(configMessage);
            iOSConfig = JsonUtility.FromJson<Config>(configMessage);

            if (CrashReporterConfig.guid != null && backtraceClient != null)
            {
                backtraceClient.Breadcrumbs.Info("guid_updated", new Dictionary<string, string> { { "GUID", CrashReporterConfig.guid } });
            }
        }

        protected override SdkApi.IMessaging CreateMessagingApi()
        {
            return new NGPiOSMessaging();
        }
        
        protected override SdkApi.ICloudSave CreateCloudSaveApi()
        {
            return new NGPiOSCloudSave();
        }

        protected override SdkApi.IPlayerIdentity CreatePlayerIdentityApi()
        {
            return new NGPiOSPlayerIdentities();
        }
        
        internal NGPiOSBridge()
        {
            _ngp_set_config_dispatcher(NGPConfig);
            _ngp_set_event_dispatcher(NGPDispatchEvent);
        }

        [DllImport("__Internal")]
        private static extern void _ngp_set_config_dispatcher(NGPConfigurationCallbackFuncType dispatcher);
        [DllImport("__Internal")]
        private static extern void _ngp_set_event_dispatcher(NGPEventDispatcherFuncType dispatcher);
        [DllImport("__Internal")]
        private static extern void ngp_check_user_authentication();
        [DllImport("__Internal")]
        private static extern void ngp_hide_netflix_menu();
        [DllImport("__Internal")]
        private static extern void ngp_show_netflix_access_button();
        [DllImport("__Internal")]
        private static extern void ngp_hide_netflix_access_button();
        [DllImport("__Internal")]
        private static extern void ngp_on_game_state_saved(string state);
        [DllImport("__Internal")]
        private static extern void ngp_set_locale(string locale);
        [DllImport("__Internal")]
        private static extern void ngp_show_netflix_menu(int location);
        [DllImport("__Internal")]
        private static extern void ngp_send_cl_event(string cl_type_name, string event_data_json);

        public override void CheckUserAuth()
        {
            NfLog.Log(MethodBase.GetCurrentMethod());
            ngp_check_user_authentication();
        }

        public override void HideNetflixMenu()
        {
            NfLog.Log(MethodBase.GetCurrentMethod());
            ngp_hide_netflix_menu();
        }

        public override void ShowNetflixAccessButton()
        {
            NfLog.Log(MethodBase.GetCurrentMethod());
            ngp_show_netflix_access_button();
        }

        public override void HideNetflixAccessButton()
        {
            NfLog.Log(MethodBase.GetCurrentMethod());
            ngp_hide_netflix_access_button();
        }

        public override void ShowNetflixMenu(int location)
        {
            NfLog.Log(MethodBase.GetCurrentMethod());
            ngp_show_netflix_menu(location);
        }

        public override void SetLocale(NetflixSdk.Locale locale)
        {
            string localeJSON = JsonUtility.ToJson(locale);
            NfLog.Log(MethodBase.GetCurrentMethod() + " " + localeJSON);
            ngp_set_locale(localeJSON);
        }

        public override SdkApi.CrashReporterConfig GetCrashReporterConfig()
        {
            return CrashReporterConfig;
        }

        public override string GetNativeSdkVersion()
        {
            return iOSConfig.ngpNativeSdkVer;
        }

        protected override string GetUnitySdkVersion()
        {
            return iOSConfig.ngpUnitySdkVer;
        }
        
        [Obsolete("SendCLEvent is deprecated, please use LogInGameEvent instead.")]
        public override void SendCLEvent(string clTypeName, string eventDataJson)
        {
            // TODO: Add iOS logging support
            NfLog.Log("NGPiOSBridge SendCLEvent.");
            ngp_send_cl_event(clTypeName, eventDataJson);
        }

        public override void LogInGameEvent(InGameEvent inGameEvent)
        {
            NfLog.Log("NGPiOSBridge LogInGameEvent.");
            ngp_send_cl_event(inGameEvent.name, inGameEvent.ToJson());
        }
        
        public override void PublishToEventSink(string name, string data)
        {
            //TODO
        }

        public override Dictionary<string, string> GetTestParams()
        {
            return new Dictionary<string, string>();
        }
    }
}
#endif
