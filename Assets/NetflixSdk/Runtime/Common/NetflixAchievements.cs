using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Netflix
{
    public class Achievements
    {

        /**
         * Unlock an achievement for the current active user profile.
         * 
         * The SDK will make a best effort in syncing the achievement with the server. If the device is
         * offline or encounters a transient error, the achievement will be cached in local storage
         * to be retried at a later time.
         *
         * @param achievementName - The name of the achievement to unlock.
         */
        public static Task<UnlockAchievementResult> UnlockAchievement(string achievementName)
        {
            return SdkHolder.nfsdk.GetAchievementsApi().UnlockAchievement(achievementName);
        }

        /**
         * Get the list of achievements for the current active user profile. 
         */
        public static Task<AchievementsResult> GetAchievements()
        {
            return SdkHolder.nfsdk.GetAchievementsApi().GetAchievements();
        }


        /**
         * Shows the SDK UI for the current active user profile's achievements.  
         */
        public static void ShowAchievementsPanel()
        {
            SdkHolder.nfsdk.GetAchievementsApi().ShowAchievementsPanel();
        }

    }

    public class AchievementsResult
    {
        public readonly AchievementStatus status;
        public readonly List<Achievement> achievements;

        internal AchievementsResult(AchievementStatus status, List<Achievement> achievements)
        {
            this.status = status;
            this.achievements = achievements;
        }
    }

    public class UnlockAchievementResult
    {
        public readonly AchievementStatus status;
        public readonly Achievement achievement;

        internal UnlockAchievementResult(AchievementStatus status, Achievement achievement)
        {
            this.status = status;
            this.achievement = achievement;
        }
    }

    public class Achievement
    {
        public readonly String name;
        public readonly bool isLocked;

        internal Achievement(String name, bool isLocked)
        {
            this.name = name;
            this.isLocked = isLocked;
        }

    }

    [Serializable]
    public enum AchievementStatus : int
    {
        Ok = Netflix.StatusCode.Ok,

        /**
         * An unexpected error occurred
         */
        ErrorUnknown = Netflix.StatusCode.ErrorUnknown,

        /**
         * Error due to network. Caller can retry the operation at a later time
         */
        ErrorNetwork = Netflix.StatusCode.ErrorNetwork,

        /**
         * Error returned when Achievements API call is made before platform is initialized
         */
        ErrorPlatformNotInitialized = Netflix.StatusCode.ErrorPlatformNotInitialized,

        /**
         * Error returned when Achievements API call is made without a user profile selected
         */
        ErrorUserProfileNotSelected = Netflix.StatusCode.ErrorUserProfileNotSelected,

        /**
         * Error returned when an operation is interrupted by a profile switch. Caller
         * can retry the operation upon returning to the corresponding profile
         */
        ErrorInterruptedByProfileSwitch = Netflix.StatusCode.ErrorInterruptedByProfileSwitch,


        /**
         * Error due to client-server interactions
         */
        ErrorInternal = Netflix.StatusCode.ErrorInternal,

        /**
          * Error due to timeout
          */
        ErrorTimedOut = Netflix.StatusCode.ErrorTimedOut,

        /**
          * Error due to timeout
          */
        ErrorUnavailable = Netflix.StatusCode.ErrorUnavailable,


        /**
         * Achievement Not Found
         */
        ErrorUnknownAchievement = Netflix.StatusCode.ErrorCustomStart,

        /**
         * Achievement Archived
         */
        ErrorArchivedAchievement = Netflix.StatusCode.ErrorCustomStart + 1
    }
}