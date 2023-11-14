#if UNITY_IOS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;

namespace Netflix
{
    internal class NGPiOSCloudSave : SdkApi.ICloudSave
    {
        [DllImport("__Internal")]
        private static extern void _ngp_set_cloud_save_dispatcher(NGPCloudSaveDispatcherFuncType dispatcher);
        [DllImport("__Internal")]
        private static extern void ngp_get_slot_ids(int tracker);
        [DllImport("__Internal")]
        private static extern void ngp_read_slot(int tracker, string slotId);
        [DllImport("__Internal")]
        private static extern void ngp_save_slot(int tracker, string slotId, byte[] data, int len);
        [DllImport("__Internal")]
        private static extern void ngp_delete_slot(int tracker, string slotId);
        [DllImport("__Internal")]
        private static extern void ngp_resolve_conflict(int tracker, string slotId, NetflixCloudSave.CloudSaveResolution resolution);

        internal NGPiOSCloudSave()
        {
            _ngp_set_cloud_save_dispatcher(NGPCloudSaveEvent);
        }
        
        [AOT.MonoPInvokeCallback(typeof(NGPCloudSaveDispatcherFuncType))]
        public static void NGPCloudSaveEvent(string resultMessage)
        {
            NfLog.Log(MethodBase.GetCurrentMethod() + " " + resultMessage);
            SerializedCloudSaveResponse parsedResponse = JsonUtility.FromJson<SerializedCloudSaveResponse>(resultMessage);
            switch (parsedResponse.type)
            {
                case "GetSlotIdsResult":
                    handleGetSlotIdsResult(parsedResponse);
                    break;
                case "ReadSlotResult":
                    handleReadSlotResult(parsedResponse);
                    break;
                case "SaveSlotResult":
                    handleSaveSlotResult(parsedResponse);
                    break;
                case "DeleteSlotResult":
                    handleDeleteSlotResult(parsedResponse);
                    break;
                case "ResolveConflictResult":
                    handleResolveConflictResult(parsedResponse);
                    break;
            }
        }

        private static void handleGetSlotIdsResult(SerializedCloudSaveResponse response)
        {
            int tracker = response.identifier;
            TaskCompletionSource<NetflixCloudSave.GetSlotIdsResult> tcs = getSlotIdsTasks[tracker];
            getSlotIdsTasks.Remove(tracker);
            var result = new NetflixCloudSave.GetSlotIdsResult
            {
                status = (NetflixCloudSave.CloudSaveStatus)response.result.status,
                slotIds = response.result.slotIds.ToList()
            };
            tcs.SetResult(result);
        }

        private static void handleReadSlotResult(SerializedCloudSaveResponse response)
        {
            int tracker = response.identifier;
            TaskCompletionSource<NetflixCloudSave.ReadSlotResult> tcs = readSlotTasks[tracker];
            readSlotTasks.Remove(tracker);
            NetflixCloudSave.ConflictResolution cr = null;
            if (response.result.status == (int)NetflixCloudSave.CloudSaveStatus.SlotConflict)
            {
                cr = response.result.conflictResolution.toNcsCr();
            }
            NetflixCloudSave.SlotInfo info = null;
            if (response.result.data != null)
            {
                info = new NetflixCloudSave.SlotInfo(System.Convert.FromBase64String(response.result.data));
            }
            var result = new NetflixCloudSave.ReadSlotResult
            {
                status = (NetflixCloudSave.CloudSaveStatus)response.result.status,
                errorDescription = response.result.errorDescription,
                slotInfo = info,
                conflictResolution = cr
            };
            tcs.SetResult(result);
        }

        private static void handleSaveSlotResult(SerializedCloudSaveResponse response)
        {
            int tracker = response.identifier;
            TaskCompletionSource<NetflixCloudSave.SaveSlotResult> tcs = saveSlotTasks[tracker];
            saveSlotTasks.Remove(tracker);
            NetflixCloudSave.ConflictResolution cr = null;
            if (response.result.status == (int)NetflixCloudSave.CloudSaveStatus.SlotConflict)
            {
                cr = response.result.conflictResolution.toNcsCr();
            }
            var result = new NetflixCloudSave.SaveSlotResult
            {
                status = (NetflixCloudSave.CloudSaveStatus)response.result.status,
                errorDescription = response.result.errorDescription,
                conflictResolution = cr
            };
            tcs.SetResult(result);
        }

        private static void handleDeleteSlotResult(SerializedCloudSaveResponse response)
        {
            int tracker = response.identifier;
            TaskCompletionSource<NetflixCloudSave.DeleteSlotResult> tcs = deleteSlotTasks[tracker];
            deleteSlotTasks.Remove(tracker);
            NetflixCloudSave.ConflictResolution cr = null;
            if (response.result.status == (int)NetflixCloudSave.CloudSaveStatus.SlotConflict)
            {
                cr = response.result.conflictResolution.toNcsCr();
            }
            var result = new NetflixCloudSave.DeleteSlotResult
            {
                status = (NetflixCloudSave.CloudSaveStatus)response.result.status,
                errorDescription = response.result.errorDescription,
                conflictResolution = cr
            };
            tcs.SetResult(result);
        }

        private static void handleResolveConflictResult(SerializedCloudSaveResponse response)
        {
            int tracker = response.identifier;
            TaskCompletionSource<NetflixCloudSave.ResolveConflictResult> tcs = resolveConflictTasks[tracker];
            resolveConflictTasks.Remove(tracker);
            var result = new NetflixCloudSave.ResolveConflictResult
            {
                status = (NetflixCloudSave.CloudSaveStatus)response.result.status,
                errorDescription = response.result.errorDescription,
            };
            tcs.SetResult(result);
        }


        private static Dictionary<int, TaskCompletionSource<NetflixCloudSave.GetSlotIdsResult>> getSlotIdsTasks =
            new Dictionary<int, TaskCompletionSource<NetflixCloudSave.GetSlotIdsResult>>();
        private static int nextGetSlotIdTask = 0;

        public Task<NetflixCloudSave.GetSlotIdsResult> GetSlotIds()
        {
            NfLog.Log("GetSlotIds: ");
            var tcs = new TaskCompletionSource<NetflixCloudSave.GetSlotIdsResult>();
            int tracker = nextGetSlotIdTask++;
            getSlotIdsTasks[tracker] = tcs;
            ngp_get_slot_ids(tracker);
            return tcs.Task;
        }

        private static Dictionary<int, TaskCompletionSource<NetflixCloudSave.SaveSlotResult>> saveSlotTasks =
            new Dictionary<int, TaskCompletionSource<NetflixCloudSave.SaveSlotResult>>();
        private static int nextSaveSlotTask = 0;

        public Task<NetflixCloudSave.SaveSlotResult> SaveSlot(string slotId, NetflixCloudSave.SlotInfo slotInfo)
        {
            NfLog.Log("SaveSlot: " + slotId);
            var tcs = new TaskCompletionSource<NetflixCloudSave.SaveSlotResult>();
            int tracker = nextSaveSlotTask++;
            saveSlotTasks[tracker] = tcs;
            var dataLength = (slotInfo.GetDataBytes() == null) ? 0 : slotInfo.GetDataBytes().Length;
            ngp_save_slot(tracker, slotId, slotInfo.GetDataBytes(), dataLength);
            return tcs.Task;
        }

        
        private static Dictionary<int, TaskCompletionSource<NetflixCloudSave.ReadSlotResult>> readSlotTasks =
            new Dictionary<int, TaskCompletionSource<NetflixCloudSave.ReadSlotResult>>();
        private static int nextReadSlotTask = 0;


        public Task<NetflixCloudSave.ReadSlotResult> ReadSlot(string slotId)
        {
            NfLog.Log("ReadSlot: " + slotId);
            var tcs = new TaskCompletionSource<NetflixCloudSave.ReadSlotResult>();
            int tracker = nextReadSlotTask++;
            readSlotTasks[tracker] = tcs;
            ngp_read_slot(tracker, slotId);
            return tcs.Task;
        }

        private static Dictionary<int, TaskCompletionSource<NetflixCloudSave.DeleteSlotResult>> deleteSlotTasks =
            new Dictionary<int, TaskCompletionSource<NetflixCloudSave.DeleteSlotResult>>();
        private static int nextDeleteSlotTask = 0;
        
        public Task<NetflixCloudSave.DeleteSlotResult> DeleteSlot(string slotId)
        {
            NfLog.Log("DeleteSlot: " + slotId);
            var tcs = new TaskCompletionSource<NetflixCloudSave.DeleteSlotResult>();
            int tracker = nextDeleteSlotTask++;
            deleteSlotTasks[tracker] = tcs;
            ngp_delete_slot(tracker, slotId);
            return tcs.Task;
        }

        private static Dictionary<int, TaskCompletionSource<NetflixCloudSave.ResolveConflictResult>> resolveConflictTasks =
            new Dictionary<int, TaskCompletionSource<NetflixCloudSave.ResolveConflictResult>>();
        private static int nextResolveConflictTask = 0;

        public Task<NetflixCloudSave.ResolveConflictResult> ResolveConflict(string slotId, NetflixCloudSave.CloudSaveResolution resolution)
        {
            NfLog.Log("ResolveConflict: " + slotId);
            var tcs = new TaskCompletionSource<NetflixCloudSave.ResolveConflictResult>();
            int tracker = nextResolveConflictTask++;
            resolveConflictTasks[tracker] = tcs;
            ngp_resolve_conflict(tracker, slotId, resolution);
            return tcs.Task;
        }
        
        [Serializable]
        public class SerializedCloudSaveResponse
        {
            public int identifier;
            public string type;

            [Serializable]
            public class Result
            {
                public int status;
                public string errorDescription;
                // for GetSlotInfo
                public string[] slotIds;
                // for ReadSlot
                public string data;

                [Serializable]
                public class ConflictResolution
                {
                    [Serializable]
                    public class SlotInfo
                    {
                        public string data;
                        public string serverSyncTimestamp;
                    }
                    public SlotInfo local;
                    public SlotInfo remote;

                    public NetflixCloudSave.ConflictResolution toNcsCr()
                    {
                        NetflixCloudSave.ConflictResolution cr = new NetflixCloudSave.ConflictResolution
                        {
                            local = new NetflixCloudSave.SlotInfo(System.Convert.FromBase64String(local.data)),
                            remote = new NetflixCloudSave.SlotInfo(System.Convert.FromBase64String(remote.data))
                        };
                        cr.local.SetServerSyncTimestamp(local.serverSyncTimestamp);
                        cr.remote.SetServerSyncTimestamp(remote.serverSyncTimestamp);
                        return cr;
                    }
                }

                public ConflictResolution conflictResolution;
            }

            public Result result;
        }
    } 
}
#endif