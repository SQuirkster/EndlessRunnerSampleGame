using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

#if UNITY_ANDROID
namespace Netflix
{
    internal class NetflixAndroidPlayerIdentitiesApi : SdkApi.IPlayerIdentity
    {
        private static AndroidJavaObject nfgSdk;
        public NetflixAndroidPlayerIdentitiesApi(AndroidJavaObject sdk)
        {
            nfgSdk = sdk;
        }

        public PlayerIdentity GetCurrentPlayer()
        {
            NfLog.Log("GetCurrentPlayer");
            var currentPlayerJava = nfgSdk.Call<AndroidJavaObject>("getCurrentPlayer");
            if (currentPlayerJava == null)
            {
                return null;
            }
            var playerId = currentPlayerJava.Call<string>("getPlayerId");
            var handle = currentPlayerJava.Call<string>("getHandle");
            return new PlayerIdentity(playerId, handle);
        }

        public Task<GetPlayerIdentitiesResult> GetPlayerIdentities(List<string> playerIds)
        {
            var playerIdArray = playerIds.ToArray();
            NfLog.Log("calling GetPlayerIdentities " + string.Join(", ", playerIdArray));
            var tcs = new TaskCompletionSource<GetPlayerIdentitiesResult>();
            var callback = new NetflixAndroidGetPlayerIdentitiesCallback(tcs);

            nfgSdk.Call("getPlayerIdentities", playerIdArray, callback);
            return tcs.Task;
        }
    }
}
#endif