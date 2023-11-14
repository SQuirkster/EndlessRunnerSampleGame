using System.Threading.Tasks;
using UnityEngine;
using System;
using Netflix;

#if UNITY_ANDROID
namespace Netflix
{
    class NetflixAndroidUnlockAchievementCallback : AndroidJavaProxy
    {
        private TaskCompletionSource<UnlockAchievementResult> tcs;

        public NetflixAndroidUnlockAchievementCallback(TaskCompletionSource<UnlockAchievementResult> tcs)
            : base("com.netflix.unity.api.achievements.UnlockAchievementCallback")
        {
            this.tcs = tcs;
        }

        public void onResult(AndroidJavaObject result)
        {
            var achievementUnlockResult = ToUnlockAchievementResult(result);
            tcs.SetResult(achievementUnlockResult);
        }

        internal static UnlockAchievementResult ToUnlockAchievementResult(AndroidJavaObject androidResult)
        {
            var errorAchievement = new UnlockAchievementResult(AchievementStatus.ErrorUnknown, null);
            if (androidResult == null) {
                return errorAchievement;
            }

            var status = (AchievementStatus) androidResult.Get<int>("status");

            var entryObj = androidResult.Get<AndroidJavaObject>("achievement");
            if (entryObj != null) {
                return new UnlockAchievementResult(status,  NetflixJavaConverter.ToAchievementEntry(entryObj));
            }

            return new UnlockAchievementResult(status, null); 
        }
    }
    
}
#endif