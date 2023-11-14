using System.Collections.Generic;
using System.Threading.Tasks;

namespace Netflix
{
    internal sealed class FakeCloudSaveApi : SdkApi.ICloudSave
    {
        public Task<NetflixCloudSave.GetSlotIdsResult> GetSlotIds()
        {
            NfLog.Log("GetSlotIds: " ); 
            return Task<NetflixCloudSave.GetSlotIdsResult>.Factory.StartNew(() => new NetflixCloudSave.GetSlotIdsResult
            {
                status = NetflixCloudSave.CloudSaveStatus.Ok,
                slotIds = new List<string>()
            });
        }
        public Task<NetflixCloudSave.SaveSlotResult> SaveSlot(string slotId, NetflixCloudSave.SlotInfo slotInfo)
        {
            NfLog.Log("SaveSlot: " + slotId);
            return Task<NetflixCloudSave.SaveSlotResult>.Factory.StartNew(() => new NetflixCloudSave.SaveSlotResult
                { status = NetflixCloudSave.CloudSaveStatus.Ok });
        }

        public Task<NetflixCloudSave.ReadSlotResult> ReadSlot(string slotId)
        {
            NfLog.Log("ReadSlot: " + slotId);
            return Task<NetflixCloudSave.ReadSlotResult>.Factory.StartNew(() => new NetflixCloudSave.ReadSlotResult
                { status = NetflixCloudSave.CloudSaveStatus.Ok });
        }

        public Task<NetflixCloudSave.DeleteSlotResult> DeleteSlot(string slotId)
        {
            NfLog.Log("DeleteSlot: " + slotId);
            return Task<NetflixCloudSave.DeleteSlotResult>.Factory.StartNew(() =>
                new NetflixCloudSave.DeleteSlotResult { status = NetflixCloudSave.CloudSaveStatus.Ok });
        }

        public Task<NetflixCloudSave.ResolveConflictResult> ResolveConflict(string slotId, NetflixCloudSave.CloudSaveResolution resolution)
        {
            NfLog.Log("ResolveConflict: " + slotId);
            return Task<NetflixCloudSave.ResolveConflictResult>.Factory.StartNew(() =>
                new NetflixCloudSave.ResolveConflictResult { status = NetflixCloudSave.CloudSaveStatus.Ok });
        }
    }
}