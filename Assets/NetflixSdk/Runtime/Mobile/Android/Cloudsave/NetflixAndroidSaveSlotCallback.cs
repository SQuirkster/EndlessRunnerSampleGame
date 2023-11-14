using UnityEngine;
using System;
using System.Threading.Tasks;
using static NetflixCloudSave;

#if UNITY_ANDROID
namespace Netflix
{
    class NetflixAndroidSaveSlotCallback : NetflixAndroidCloudSaveCallback
    {
        private TaskCompletionSource<SaveSlotResult> tcs;
        public NetflixAndroidSaveSlotCallback(TaskCompletionSource<SaveSlotResult> tcs): base()
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

            SaveSlotResult result = new SaveSlotResult 
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
