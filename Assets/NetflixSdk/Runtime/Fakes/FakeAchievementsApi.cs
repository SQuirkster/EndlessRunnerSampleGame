using System.Collections.Generic;
using System.Threading.Tasks;

namespace Netflix
{
    internal sealed class FakeAchievementsApi: SdkApi.IAchievements
    {
        public Task<UnlockAchievementResult> UnlockAchievement(string achievementName)
        {
           var task = Task<UnlockAchievementResult>.Factory.StartNew(() =>
                new UnlockAchievementResult(AchievementStatus.Ok, null));
            return task;
        }

        public Task<AchievementsResult> GetAchievements()
        {
            var task = Task<AchievementsResult>.Factory.StartNew(() =>
                new AchievementsResult(AchievementStatus.Ok, new List<Achievement>()));
            return task;
        }

        public void ShowAchievementsPanel()
        {

        }
    }
}