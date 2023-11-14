using UnityEngine;
using System;
using System.Threading.Tasks;
using static NetflixCloudSave;
using System.Text;

#if UNITY_ANDROID
namespace Netflix
{
    abstract class NetflixAndroidCloudSaveCallback : AndroidJavaProxy
    {
        private const int TEST_LOG_PAYLOAD_LEN = 100;

        public abstract void OnDecodedResult(CloudSaveStatus status, String errorDescription, byte[] localData, String localTimestamp, byte[] remoteData, String remoteTimestamp);

        public NetflixAndroidCloudSaveCallback() : base("com.netflix.unity.api.cloudsave.CloudSaveCallback")
        {
        }

        public void onResult(AndroidJavaObject cloudSaveResultObj)
        {
            CloudSaveStatus status = CloudSaveStatus.ErrorUnknown;
            string errorDescription = null;
            string localDataTimestamp = null, remoteDataTimestamp = null;
            byte[] localData = null, remoteData = null;

            if (cloudSaveResultObj != null) {
                string localDataString = null, remoteDataString = null;
                status = (CloudSaveStatus) cloudSaveResultObj.Get<int>("status");
                errorDescription = cloudSaveResultObj.Get<string>("description");

                AndroidJavaObject localObj = cloudSaveResultObj.Get<AndroidJavaObject>("local");
                if (localObj != null) {
                    localDataTimestamp = localObj.Get<string>("serverTimestamp");

                    localDataString = localObj.Get<string>("data");
                    localData = Base64DecodeToBytes(localDataString);
                }

                AndroidJavaObject remoteObj = cloudSaveResultObj.Get<AndroidJavaObject>("remote");
                if (remoteObj != null) {
                    remoteDataTimestamp = remoteObj.Get<string>("serverTimestamp");
                    
                    remoteDataString = remoteObj.Get<string>("data");
                    remoteData = Base64DecodeToBytes(remoteDataString);
                }
                LogBase64String(status, localDataString, remoteDataString);
            }

            LogDecoded(status, errorDescription, localData, localDataTimestamp, remoteData, remoteDataTimestamp);
            OnDecodedResult(status, errorDescription, localData, localDataTimestamp, remoteData, remoteDataTimestamp);
        }

        private void LogBase64String(CloudSaveStatus status, string data, string remoteData)
        {
            var sb = new System.Text.StringBuilder();
            sb.Append(" LogBase64String onResult status : " + status);

            if (data != null && data.Length > 0) {
                 sb.Append("\n data: (" + data.Length + ") " + data.Substring(0, Math.Min(data.Length, TEST_LOG_PAYLOAD_LEN)));
            }  else {
                sb.Append("\n data   : " + data);
            }

            if (remoteData != null && remoteData.Length > 0) {
                 sb.Append("\n remoteData: (" + remoteData.Length + ") " + remoteData.Substring(0, Math.Min(remoteData.Length, TEST_LOG_PAYLOAD_LEN)));
            }  else {
                sb.Append("\n remoteData: " + remoteData);
            }

            NfLog.Log(sb.ToString());
        }

        private void LogDecoded(CloudSaveStatus status, String errorDescription, byte[] localData, String localTimestamp, byte[] remoteData, String remoteTimestamp)
        {
            var sb = new System.Text.StringBuilder();
            sb.Append(" LogDecoded onResult status : " + status + ", errorDescription: " + errorDescription);

            string localString = BytesAsString(localData);
            sb.Append("\n local:: timestamp: " + localTimestamp);
            if (localString != null) {
                sb.Append(", (" + localString.Length + ") " + localString.Substring(0, Math.Min(localString.Length, TEST_LOG_PAYLOAD_LEN)));
            }

            string remoteString = BytesAsString(remoteData);
            sb.Append("\n remote:: timestamp: " + remoteTimestamp);
            if (remoteString != null) {
                sb.Append(", (" + remoteString.Length + ") " + remoteString.Substring(0, Math.Min(remoteString.Length, TEST_LOG_PAYLOAD_LEN)));
            }

            NfLog.Log(sb.ToString());
        }


        private static string BytesAsString(byte[] bytes)
        {
            if (bytes == null)
            {
                return "null";
            }
            else
            {
                return BitConverter.ToString(bytes);
            }
        }

        private static string FromAndroidString(AndroidJavaObject javaObj)
        {
            if (javaObj == null)
            {
                return null;
            }
            return javaObj.Call<string>("toString");
        }


        private static string Base64Decode(string base64EncodedData)
        {
            if (base64EncodedData == null)
            {
                return null;
            }
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        private static byte[] Base64DecodeToBytes(string base64EncodedData)
        {
            if (base64EncodedData == null)
            {
                return null;
            }
            return System.Convert.FromBase64String(base64EncodedData);
        }

        private byte[] Base64DecodeToBytes(AndroidJavaObject javaObj)
        {
            if (javaObj == null)
            {
                return null;
            }
            string data = javaObj.Call<string>("toString");
            return System.Convert.FromBase64String(data);
        }
    }
}
#endif
