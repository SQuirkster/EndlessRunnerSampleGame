using System.Threading.Tasks;
using Netflix;
using UnityEngine;

#if UNITY_ANDROID
namespace Netflix
{
    internal class NetflixAndroidGetPlayerIdentitiesCallback : AndroidJavaProxy
    {
        private TaskCompletionSource<GetPlayerIdentitiesResult> tcs;

        public NetflixAndroidGetPlayerIdentitiesCallback(TaskCompletionSource<GetPlayerIdentitiesResult> tcs) :
            base("com.netflix.unity.api.player.GetPlayerIdentitiesCallback")
        {
            this.tcs = tcs;
        }

        public void onGetPlayerIdentities(AndroidJavaObject result)
        {
            var playerIdentitiesResult =  NetflixJavaConverter.ToGetPlayerIdentitiesResult(result);
            tcs.SetResult(playerIdentitiesResult);
        }
    }
}
#endif
