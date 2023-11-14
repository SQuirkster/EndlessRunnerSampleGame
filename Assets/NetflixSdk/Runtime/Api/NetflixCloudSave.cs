using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Netflix;

//TODO: Future breaking fix. Add namespace Netflix.
//namespace Netflix

public class NetflixCloudSave
{
    public static Task<SaveSlotResult> SaveSlot(string slotId, SlotInfo slotInfo)
    {
        return SdkHolder.nfsdk.GetCloudSaveApi().SaveSlot(slotId, slotInfo);
    }

    public static Task<DeleteSlotResult> DeleteSlot(string slotId)
    {
        return SdkHolder.nfsdk.GetCloudSaveApi().DeleteSlot(slotId);
    }

    public static Task<ReadSlotResult> ReadSlot(string slotId)
    {
        return SdkHolder.nfsdk.GetCloudSaveApi().ReadSlot(slotId);
    }

    public static Task<GetSlotIdsResult> GetSlotIds()
    {
        return SdkHolder.nfsdk.GetCloudSaveApi().GetSlotIds();
    }

    public static Task<ResolveConflictResult> ResolveConflict(string slotId, CloudSaveResolution resolution)
    {
        return SdkHolder.nfsdk.GetCloudSaveApi().ResolveConflict(slotId, resolution);
    }


    public enum CloudSaveResolution
    {
        KeepLocal = 0,
        KeepRemote
    }

    public enum CloudSaveStatus
    {
        Ok = 0,
        SlotConflict = 1,
        // Error start
        ErrorUnknown = 1000, 
        ErrorUnknownSlotId = 1001,
        ErrorSlotLimitExceeded = 1002,
        ErrorNetwork = 1003,
        ErrorPlatformNotInitialized = 1004,
        ErrorUserProfileNotSelected = 1005,
        ErrorInterruptedByProfileSwitch = 1006,
        ErrorSizeLimitExceeded = 1007,
        ErrorIO = 1008,
        ErrorInternal = 1009,
        ErrorValidation = 1010
    }

    public class ReadSlotResult
    {
        public CloudSaveStatus status;
        public string errorDescription;
        public SlotInfo slotInfo;
        public ConflictResolution conflictResolution;
    }

    public class SaveSlotResult
    {
        public CloudSaveStatus status;
        public string errorDescription;
        public ConflictResolution conflictResolution;
    }


    public class DeleteSlotResult
    {
        public CloudSaveStatus status;
        public string errorDescription;
        public ConflictResolution conflictResolution;
    }

    public class ResolveConflictResult
    {
        public CloudSaveStatus status;
        public string errorDescription;
    }

    public class SlotInfo
    {
        private byte[] data;
        private string serverSyncTimestamp;
        public SlotInfo(byte[] content)
        {
            data = content;
            serverSyncTimestamp = null;
        }
        public byte[] GetDataBytes()
        {
            return data;
        }
        public string GetServerSyncTimestamp()
        {
            return serverSyncTimestamp;
        }

        internal void SetServerSyncTimestamp(string timestamp) {
            this.serverSyncTimestamp = timestamp;
        }
    }

    public class ConflictResolution
    {
        public SlotInfo local;
        public SlotInfo remote;
    }

    public class GetSlotIdsResult
    {
        public CloudSaveStatus status;
        public List<string> slotIds;
    }

}
