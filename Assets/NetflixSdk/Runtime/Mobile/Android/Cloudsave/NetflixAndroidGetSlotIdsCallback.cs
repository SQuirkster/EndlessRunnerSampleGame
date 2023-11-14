using UnityEngine;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

#if UNITY_ANDROID
namespace Netflix
{
    class NetflixAndroidGetSlotIdsCallback : AndroidJavaProxy
    {
        private TaskCompletionSource<NetflixCloudSave.GetSlotIdsResult> tcs;
        public NetflixAndroidGetSlotIdsCallback(TaskCompletionSource<NetflixCloudSave.GetSlotIdsResult> tcs)
            : base("com.netflix.unity.api.cloudsave.GetSlotIdsCallback")
        {
            this.tcs = tcs;
        }

        public void onResult(int status, AndroidJavaObject slotIdsList)
        {
            List<string> slotIds = new List<string>();
            if (slotIdsList != null)
            {
                int size = slotIdsList.Call<int>("size");
                for (int i = 0; i < size; i++)
                {
                    slotIds.Add(slotIdsList.Call<string>("get", i));
                }
            }
            var sb = new System.Text.StringBuilder();
            sb.Append("onResult status : " + status);
            sb.Append("         slotIds: ");
            foreach(var slotId in slotIds)
            {
                sb.Append(slotId);
                sb.Append(", ");
            }
            NfLog.Log(sb.ToString());
            var result = new NetflixCloudSave.GetSlotIdsResult
            {
                status = (NetflixCloudSave.CloudSaveStatus)status,
                slotIds = slotIds
            };
            tcs.SetResult(result);
        }
    }
}
#endif
