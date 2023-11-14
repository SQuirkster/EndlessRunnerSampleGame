#if UNITY_ANDROID
using System.Threading.Tasks;
using UnityEngine;

namespace Netflix
{
    internal class NetflixAndroidCloudSaveApi: SdkApi.ICloudSave
    {
        private static AndroidJavaObject nfgSdk;
        public NetflixAndroidCloudSaveApi(AndroidJavaObject sdk)
        {
            nfgSdk = sdk;
        }

        public Task<NetflixCloudSave.GetSlotIdsResult> GetSlotIds()
        {
            NfLog.Log("GetSlotIds:");
            var tcs = new TaskCompletionSource<NetflixCloudSave.GetSlotIdsResult>();
            var callback = new NetflixAndroidGetSlotIdsCallback(tcs);
            nfgSdk.Call("getSlotIds", callback);
            return tcs.Task;
        }

        public Task<NetflixCloudSave.SaveSlotResult> SaveSlot(string slotId, NetflixCloudSave.SlotInfo slotInfo)
        {
            NfLog.Log("SaveSlot id: " + slotId);
            string dataStr = null;
            if (slotInfo.GetDataBytes() != null)
            {
                dataStr = System.Convert.ToBase64String(slotInfo.GetDataBytes());
                NfLog.Log("  base64String :" + dataStr);
            }
            var tcs = new TaskCompletionSource<NetflixCloudSave.SaveSlotResult>();
            var callback = new NetflixAndroidSaveSlotCallback(tcs);
            nfgSdk.Call("saveSlot", slotId, dataStr, callback);
            return tcs.Task;
        }

        public Task<NetflixCloudSave.ReadSlotResult> ReadSlot(string slotId)
        {
            NfLog.Log("ReadSlot id: " + slotId);
            var tcs = new TaskCompletionSource<NetflixCloudSave.ReadSlotResult>();
            var callback = new NetflixAndroidReadSlotCallback(tcs);
            nfgSdk.Call("readSlot", slotId, callback);
            return tcs.Task;
        }

        public Task<NetflixCloudSave.DeleteSlotResult> DeleteSlot(string slotId)
        {
            NfLog.Log("DeleteSlot id: " + slotId);
            var tcs = new TaskCompletionSource<NetflixCloudSave.DeleteSlotResult>();
            var callback = new NetflixAndroidDeleteSlotCallback(tcs);
            nfgSdk.Call("deleteSlot", slotId, callback);
            return tcs.Task;
        }

        public Task<NetflixCloudSave.ResolveConflictResult> ResolveConflict(string slotId, NetflixCloudSave.CloudSaveResolution resolution)
        {
            NfLog.Log("ResolveConflict id: " + slotId + ", resolution: " + resolution.ToString());
            var tcs = new TaskCompletionSource<NetflixCloudSave.ResolveConflictResult>();
            var callback = new NetflixAndroidResolveConflictCallback(tcs);
            nfgSdk.Call("resolveConflict", slotId, (int)resolution, callback);
            return tcs.Task;
        }

    }
}
#endif