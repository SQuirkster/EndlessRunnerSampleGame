#if UNITY_IOS
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Threading.Tasks;
using Netflix;
using UnityEngine;

namespace Netflix
{
    internal class NGPiOSAchievements : SdkApi.IAchievements
    {
        [DllImport("__Internal")]
        private static extern void ngp_unlock_achievement(int correlationId, string achievementName, NGPUnlockAchievementCallbackFuncType callback);
        [DllImport("__Internal")]
        private static extern void ngp_get_achievements(int correlationId, NGPGetAchievementsCallbackFuncType callback);
        [DllImport("__Internal")]
        private static extern void ngp_show_achievements_panel();

        public Task<UnlockAchievementResult> UnlockAchievement(string achievementName)
        {
            TaskCompletionSource<UnlockAchievementResult> completionSource = new TaskCompletionSource<UnlockAchievementResult>();

            var correlationId = NetflixBridgeCallbackManager.shared.AddCompletionSource(completionSource);

            NfLog.Log(MethodBase.GetCurrentMethod() + " correlationId " + correlationId + " name " + achievementName);

            ngp_unlock_achievement(correlationId, achievementName, NGPUnlockAchievementDispatchResult);

            return completionSource.Task;
        }

        public Task<AchievementsResult> GetAchievements()
        {
            var completionSource = new TaskCompletionSource<AchievementsResult>();

            var correlationId = NetflixBridgeCallbackManager.shared.AddCompletionSource(completionSource);

            NfLog.Log(MethodBase.GetCurrentMethod() + " correlationId " + correlationId);

            ngp_get_achievements(correlationId, NGPGetAchievementsDispatchResult);

            return completionSource.Task;
        }

        public void ShowAchievementsPanel()
        {
            NfLog.Log(MethodBase.GetCurrentMethod());

            ngp_show_achievements_panel();
        }


        [AOT.MonoPInvokeCallback(typeof(NGPUnlockAchievementCallbackFuncType))]
        public static void NGPUnlockAchievementDispatchResult(int correlationId, string resultMessage)
        {
            var completionSource = NetflixBridgeCallbackManager.shared.CompletionSourceForId<UnlockAchievementResult>(correlationId);

            if (completionSource != null)
            {
                UnlockAchievementResult result = iOSSerialization.AchievementsResponseParser.parseUnlockAchievementResult(resultMessage);

                NfLog.Log(MethodBase.GetCurrentMethod() + " correlationId " + correlationId + " UnlockAchievementResult " + JsonUtility.ToJson(result));

                if (result != null)
                {
                    completionSource.SetResult(result);
                    NetflixBridgeCallbackManager.shared.RemoveCompletionSourceForId(correlationId);
                }
            }
        }

        [AOT.MonoPInvokeCallback(typeof(NGPGetAchievementsCallbackFuncType))]
        public static void NGPGetAchievementsDispatchResult(int correlationId, string resultMessage)
        {
            var completionSource = NetflixBridgeCallbackManager.shared.CompletionSourceForId<AchievementsResult>(correlationId);

            if (completionSource != null)
            {
                AchievementsResult result = iOSSerialization.AchievementsResponseParser.parseAchievementsResult(resultMessage);

                NfLog.Log(MethodBase.GetCurrentMethod() + " correlationId " + correlationId + " AchievementsResult " + JsonUtility.ToJson(result));

                if (result != null)
                {
                    completionSource.SetResult(result);
                    NetflixBridgeCallbackManager.shared.RemoveCompletionSourceForId(correlationId);
                }
            }
        }
    }

    namespace iOSSerialization
    {
        internal class AchievementsResponseParser
        {
            internal static UnlockAchievementResult parseUnlockAchievementResult(string resultMessage)
            {

                var result = JsonUtility.FromJson<SerializableUnlockAchievementResult>(resultMessage);
                Achievement achievement = null;

                if (result.achievement.name != null)
                {
                    achievement = new Achievement(result.achievement.name, result.achievement.isLocked);
                }

                return new UnlockAchievementResult(result.status, achievement);
            }

            internal static AchievementsResult parseAchievementsResult(string resultMessage)
            {

                var result = JsonUtility.FromJson<SerializableAchievementsResult>(resultMessage);
                var achievements = new List<Achievement>();

                foreach(var serializedAchievement in result.achievements)
                {
                    var achievement = new Achievement(serializedAchievement.name, serializedAchievement.isLocked);
                    achievements.Add(achievement);
                }

                return new AchievementsResult(result.status, achievements);
            }
        }

        [Serializable]
        public class SerializableAchievementsResult
        {
            public AchievementStatus status;
            public List<SerializableAchievement> achievements;
        }

        [Serializable]
        public class SerializableUnlockAchievementResult
        {
            public AchievementStatus status;
            public SerializableAchievement achievement;
        }

        [Serializable]
        public class SerializableAchievement
        {
            public String name;
            public bool isLocked;
        }
    }
}


#endif