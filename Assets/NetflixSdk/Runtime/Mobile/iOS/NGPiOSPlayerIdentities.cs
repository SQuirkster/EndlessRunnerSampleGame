#if UNITY_IOS

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;

namespace Netflix
{
    internal class NGPiOSPlayerIdentities : SdkApi.IPlayerIdentity
    {
        [DllImport("__Internal")]
        private static extern void ngp_current_player_identity(NGPCurrentPlayerCallbackFuncType dispatcher);
        [DllImport("__Internal")]
        private static extern void ngp_get_player_identities(int tracker, string json_string, NGPGetPlayerIdentitiesCallbackFuncType dispatcher);
        
        static PlayerIdentity playerIdentity;
        [AOT.MonoPInvokeCallback(typeof(NGPCurrentPlayerCallbackFuncType))]
        public static void NGPCurrentPlayer(string playerMessage)
        {
            NfLog.Log(MethodBase.GetCurrentMethod() + " " + playerMessage);
            SerializedPlayerIdentitiesResponse.SerializedPlayerIdentityResponse.SerializedPlayerIdentity sPlayerID = JsonUtility.FromJson<SerializedPlayerIdentitiesResponse.SerializedPlayerIdentityResponse.SerializedPlayerIdentity>(playerMessage);
            playerIdentity = new PlayerIdentity(sPlayerID.playerId, sPlayerID.handle);
        }

        public PlayerIdentity GetCurrentPlayer()
        {
            NfLog.Log("GetCurrentPlayer");
            ngp_current_player_identity(NGPCurrentPlayer);
            return playerIdentity;
        }

        [Serializable]
        public class SerializedPlayerIdentitiesResponse
        {
            [Serializable]
            public class SerializedPlayerIdentityResponse
            {
                [Serializable]
                public class SerializedPlayerIdentity
                {
                    public string playerId;
                    public string handle;
                }
                public int status;
                public string id;
                public SerializedPlayerIdentity playerIdentity;
            }

            public int tracker;
            public int resultStatus;
            public string description;
            public List<SerializedPlayerIdentityResponse> identities;
        }

        private static Dictionary<int, TaskCompletionSource<GetPlayerIdentitiesResult>> getPlayerIdentitiesTasks =
            new Dictionary<int, TaskCompletionSource<GetPlayerIdentitiesResult>>();
        private static int nextGetPlayerIdentitiesTask = 0;

        [AOT.MonoPInvokeCallback(typeof(NGPGetPlayerIdentitiesCallbackFuncType))]
        public static void NGPGetPlayerIdentities(string resultMessage)
        {
            NfLog.Log(MethodBase.GetCurrentMethod() + " " + resultMessage);
            SerializedPlayerIdentitiesResponse response = JsonUtility.FromJson<SerializedPlayerIdentitiesResponse>(resultMessage);
            int tracker = response.tracker;
            TaskCompletionSource<GetPlayerIdentitiesResult> tcs = getPlayerIdentitiesTasks[tracker];
            getPlayerIdentitiesTasks.Remove(tracker);

            RequestStatus status = (RequestStatus)response.resultStatus;
            string description = response.description;
            Dictionary<string, PlayerIdentityResult> identities = new Dictionary<string, PlayerIdentityResult>();

            foreach (SerializedPlayerIdentitiesResponse.SerializedPlayerIdentityResponse IDResponse in response.identities)
            {
                PlayerIdentityStatus pidStatus = (PlayerIdentityStatus)IDResponse.status;
                PlayerIdentity identity = new PlayerIdentity(IDResponse.playerIdentity.playerId, IDResponse.playerIdentity.handle);
                PlayerIdentityResult pidResult = new PlayerIdentityResult(pidStatus, identity);
                identities[IDResponse.id] = pidResult;
            }

            GetPlayerIdentitiesResult result = new GetPlayerIdentitiesResult(status, description, identities);
            tcs.SetResult(result);
        }

        public Task<GetPlayerIdentitiesResult> GetPlayerIdentities(List<string> playerIds)
        {
            NfLog.Log("GetPlayerIdentities");
            var tcs = new TaskCompletionSource<GetPlayerIdentitiesResult>();
            string ids = String.Join(",", playerIds.ToArray());
            int tracker = nextGetPlayerIdentitiesTask++;
            getPlayerIdentitiesTasks[tracker] = tcs;
            ngp_get_player_identities(tracker, ids, NGPGetPlayerIdentities);
            return tcs.Task;
        }

    }
}
#endif