using UnityEngine;
using System;
using System.Threading.Tasks;
using static NetflixCloudSave;

#if UNITY_ANDROID
namespace Netflix
{
    class NetflixAndroidDeleteSlotCallback : NetflixAndroidCloudSaveCallback
    {
        private TaskCompletionSource<DeleteSlotResult> tcs;
        public NetflixAndroidDeleteSlotCallback(TaskCompletionSource<DeleteSlotResult> tcs) : base()
        {
            this.tcs = tcs;
        }

        public override void OnDecodedResult(CloudSaveStatus status, String errorDescription, byte[] localData, string localTimestamp, byte[] remoteData, string remoteTimestamp)
        {
            ConflictResolution conflictResolution = null;
            if (status == CloudSaveStatus.SlotConflict) {
                conflictResolution = new ConflictResolution
                {
                    local = new SlotInfo(localData),
                    remote = new SlotInfo(remoteData)
                };
                conflictResolution.local.SetServerSyncTimestamp(localTimestamp);
                conflictResolution.remote.SetServerSyncTimestamp(remoteTimestamp);
            }

            DeleteSlotResult result = new DeleteSlotResult 
            {
                status = status,
                errorDescription = errorDescription,
                conflictResolution = conflictResolution
            };

            tcs.SetResult(result);
        }
    }
}
#endif
