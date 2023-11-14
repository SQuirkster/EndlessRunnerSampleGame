using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

#if UNITY_ANDROID
namespace Netflix
{
    internal class NetflixAndroidAchievementsApi : SdkApi.IAchievements
    {
        private static AndroidJavaObject nfgSdk;
        public NetflixAndroidAchievementsApi(AndroidJavaObject sdk)
        {
            nfgSdk = sdk;
        }

        public Task<AchievementsResult> GetAchievements()
        {
            NfLog.Log(" NetflixAndroidAchievements GetAchievements ");
            var tcs = new TaskCompletionSource<AchievementsResult>();
            NetflixAndroidGetAchievementsCallback callback = new NetflixAndroidGetAchievementsCallback(tcs);
            nfgSdk.Call("getAchievements", callback);
            return tcs.Task;   
        }

        public Task<UnlockAchievementResult> UnlockAchievement(string achievementName)
        {
            NfLog.Log(" NetflixAndroidAchievements UnlockAchievement: " + achievementName);
            var tcs = new TaskCompletionSource<UnlockAchievementResult>();
            var callback = new NetflixAndroidUnlockAchievementCallback(tcs);
            nfgSdk.Call("unlockAchievement", achievementName, callback);
            return tcs.Task;

        }

        public void ShowAchievementsPanel() {
             NfLog.Log(" NetflixAndroidAchievements ShowAchievementsPanel: ");
              nfgSdk.Call("showAchievementsPanel");
        }
    
    }
}
#endif