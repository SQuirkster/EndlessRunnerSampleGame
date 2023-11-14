using UnityEngine;
using System;
using System.Threading.Tasks;
using static NetflixCloudSave;

#if UNITY_ANDROID
namespace Netflix
{
    class NetflixAndroidReadSlotCallback : NetflixAndroidCloudSaveCallback
    {
        private TaskCompletionSource<ReadSlotResult> tcs;
        public NetflixAndroidReadSlotCallback(TaskCompletionSource<ReadSlotResult> tcs) : base()
        {
            this.tcs = tcs;
        }

        public override void OnDecodedResult(CloudSaveStatus status, String errorDescription, byte[] localData, string localTimestamp, byte[] remoteData, string remoteTimestamp)
        {
            SlotInfo remoteSlotInfo = null;
            ConflictResolution conflictResolution = null;

            if (status == CloudSaveStatus.SlotConflict) {
                conflictResolution = new ConflictResolution
                {
                    local = new SlotInfo(localData),
                    remote = new SlotInfo(remoteData)
                };
                conflictResolution.local.SetServerSyncTimestamp(localTimestamp);
                conflictResolution.remote.SetServerSyncTimestamp(remoteTimestamp);
                remoteSlotInfo = new SlotInfo(null);
            } else {
                /* regular read is server data */
                remoteSlotInfo = new SlotInfo(remoteData);
                remoteSlotInfo.SetServerSyncTimestamp(remoteTimestamp);
            }

            ReadSlotResult result = new ReadSlotResult 
            {
                status = status,
                errorDescription = errorDescription,
                slotInfo = remoteSlotInfo,
                conflictResolution = conflictResolution
            };

            tcs.SetResult(result);
        }
    }

}
#endif
