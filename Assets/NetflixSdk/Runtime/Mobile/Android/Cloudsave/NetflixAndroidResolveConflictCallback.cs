using UnityEngine;
using System;
using System.Threading.Tasks;
using static NetflixCloudSave;

#if UNITY_ANDROID
namespace Netflix
{
    class NetflixAndroidResolveConflictCallback : NetflixAndroidCloudSaveCallback
    {
        private TaskCompletionSource<ResolveConflictResult> tcs;
        public NetflixAndroidResolveConflictCallback(TaskCompletionSource<ResolveConflictResult> tcs) : base()
        {
            this.tcs = tcs;
        }

        public override void OnDecodedResult(CloudSaveStatus status, String errorDescription, byte[] localData, string localTimestamp, byte[] remoteData, string remoteTimestamp)
        {
            ResolveConflictResult result = new ResolveConflictResult
            {
                status = status,
                errorDescription = errorDescription,
            };

            tcs.SetResult(result);
        }
    }
}
#endif
