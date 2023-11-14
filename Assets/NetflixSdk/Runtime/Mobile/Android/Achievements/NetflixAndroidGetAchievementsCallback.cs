using System.Threading.Tasks;
using UnityEngine;
using System;
using System.Collections.Generic;


#if UNITY_ANDROID
namespace Netflix
{
    internal class NetflixAndroidGetAchievementsCallback : AndroidJavaProxy
    {
        private TaskCompletionSource<AchievementsResult> tcs;

        public NetflixAndroidGetAchievementsCallback(TaskCompletionSource<AchievementsResult> tcs) :
            base("com.netflix.unity.api.achievements.GetAchievementsCallback")
        {
            this.tcs = tcs;
        }

        public void onResult(AndroidJavaObject result)
        {
            var achievementEntriesResult =  ToAchievementsResult(result);
            tcs.SetResult(achievementEntriesResult);
        }

        internal static AchievementsResult ToAchievementsResult(AndroidJavaObject androidResult)
        {
            if (androidResult == null) {
                return new AchievementsResult(AchievementStatus.ErrorUnknown, null);
            }

            var status = (AchievementStatus) androidResult.Get<int>("status");
            NfLog.Log("ToAchievementsResult status: " +status);
            if (status != AchievementStatus.Ok) {
                return new AchievementsResult(status, null);
            }

            var achievements  = new List<Achievement>();
            var entries = androidResult.Get<AndroidJavaObject>("achievements");
            if (entries != null) {

                int size = entries.Call<int>("size");
                AndroidJavaObject entryObj;

                for (int i = 0; i < size; ++i) {
                    NfLog.Log("[" + i + "]");
                    entryObj = entries.Call<AndroidJavaObject>("get", i);
                    if (entryObj != null) {
                        achievements.Add(NetflixJavaConverter.ToAchievementEntry(entryObj));
                    }
                }
            }
            NfLog.Log("ToAchievementsResult achievements: " +achievements);
            return new AchievementsResult(status, achievements);
        }
        
    }
}
#endif