#if UNITY_STANDALONE_LINUX

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AOT;
using UnityEngine;
using Backtrace.Unity;
using Backtrace.Unity.Model;

namespace Netflix
{
    sealed class NetflixSdkCloudImpl : NetflixSdkImpl
    {
        internal delegate void NGPCallbackType(ulong id, string responseAsString);

        private static ulong mCurrentCallbackId = 0;
        private static Dictionary<ulong, Action<string>> mCallbacks = new Dictionary<ulong, Action<string>>();
        internal class NGPStatus
        {
            public const string DeleteSlotSuccess = "NGPProgress_DeleteSlotSuccess";
            public const string SaveSlotSuccess = "NGPProgress_SaveSlotSuccess";
            public const string ReadSlotSuccess = "NGPProgress_Slot";
            public const string SlotConflictError = "NGPProgress_SlotConflictError";
            public const string SlotNotFoundError = "NGPProgress_SlotNotFoundError";
            public const string ValidationError = "NGPProgress_ValidationError";
        }

        [Serializable]
        internal class NGPResponse
        {
            [Serializable]
            public class UserData
            {
                public string base64Data;
            }

            [Serializable]
            public class Snapshot
            {
                public string id;
                public string createTimestamp;
                public UserData directDownload;
            }

            [Serializable]
            public class Slot
            {
                public string __typename;
                public string slotClientId;
                public Snapshot currentSnapshot;
                public string description;
            }

            [Serializable]
            public class SlotId
            {
                public string slotClientId;
            }

            [Serializable]
            public class Data
            {
                public Slot ngpProgress_saveSlot;
                public Slot ngpProgress_slot;
                public Slot ngpProgress_deleteSlot;
                public List<SlotId> ngpProgress_slots;
            }

            public Data data;
        }

        internal static class NativeLibrary
        {
            public static bool Available = false;

            const string LibraryBaseName = "ngp_cloud_sdk";
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            const string LibraryExtension = "dll";
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            const string LibraryExtension = "dylib";
#else
            const string LibraryExtension = "so";
#endif
            const string LibraryName = LibraryBaseName + "." + LibraryExtension;
            [DllImport(LibraryName)]
            public static extern void ngp_cloud_init();

            [DllImport(LibraryName)]
            public static extern void ngp_cloud_release();

            [DllImport(LibraryName)]
            public static extern IntPtr ngp_cloud_get_native_sdk_version();

            [DllImport(LibraryName)]
            public static extern void ngp_cloud_check_user_auth();

            [DllImport(LibraryName)]
            public static extern void ngp_cloud_hide_netflix_menu();

            [DllImport(LibraryName)]
            public static extern void ngp_cloud_show_netflix_menu(int location);

            [DllImport(LibraryName)]
            public static extern void ngp_cloud_show_netflix_access_button();

            [DllImport(LibraryName)]
            public static extern void ngp_cloud_hide_netflix_access_button();

            [DllImport(LibraryName)]
            public static extern void ngp_cloud_set_locale(string language, string country, string variant);

            [DllImport(LibraryName)]
            public static extern void ngp_cloud_setup_bugsnag();

            [DllImport(LibraryName)]
            public static extern void ngp_cloud_publish_to_event_sink(string name, string data);

            [DllImport(LibraryName)]
            public static extern void ngp_cloud_on_push_token(string pushDeviceToken);

            [DllImport(LibraryName)]
            public static extern void ngp_cloud_on_deeplink_received(string deepLinkURL, bool processedByGame);

            [DllImport(LibraryName)]
            public static extern void ngp_cloud_on_messaging_event(NetflixMessaging.MessagingEventType eventType,
                string jsonString);

            [DllImport(LibraryName)]
            public static extern void ngp_cloud_send_cl_event(string clTypeName, string eventDataJson);

            [DllImport(LibraryName)]
            public static extern void ngp_cloud_get_slot_ids(ulong id, NGPCallbackType callback);

            [DllImport(LibraryName)]
            public static extern void ngp_cloud_save_slot(ulong id, string slotId, byte[] data, uint dataSize,
                NGPCallbackType callback);

            [DllImport(LibraryName)]
            public static extern void ngp_cloud_read_slot(ulong id, string slotId, NGPCallbackType callback);

            [DllImport(LibraryName)]
            public static extern void ngp_cloud_delete_slot(ulong id, string slotId, NGPCallbackType callback);

            [DllImport(LibraryName)]
            public static extern string ngp_cloud_resolve_slot_conflict(ulong id, string slotId,
                NetflixCloudSave.CloudSaveResolution resolution, NGPCallbackType callback);

            [DllImport(LibraryName)]
            public static extern void ngp_set_ssic_active_layout(string layoutName);

            [DllImport(LibraryName)]
            public static extern void ngp_cloud_log_exception(byte[] data, uint dataSize, string breadcrumbsPath);

            [DllImport(LibraryName)]
            public static extern IntPtr ngp_cloud_get_crash_reporter_data_path();
        }

        internal NetflixSdkCloudImpl()
        {
            try
            {
                NativeLibrary.ngp_cloud_init();
                NativeLibrary.Available = true;
            }
            catch (DllNotFoundException)
            {
                NfLog.Log("NetflixSdkCloudImpl - No native SDK available");
            }
        }

        ~NetflixSdkCloudImpl()
        {
            if (NativeLibrary.Available)
            {
                NativeLibrary.ngp_cloud_release();
            }
        }

        public override void SetupCrashReporter()
        {
            if (NativeLibrary.Available)
            {
                NfLog.Log("NetflixSdkCloudImpl SetupCrashReporter");
                IntPtr dataPath = NativeLibrary.ngp_cloud_get_crash_reporter_data_path();
                if (dataPath == IntPtr.Zero) {
                    NfLog.Log("!!! Unable to get the data path for backtrace client - Not setting up backtrace client. !!!");
                    return;
                }
                string _backtraceDatabasePath = Marshal.PtrToStringAnsi(dataPath);
                NfLog.Log("Backtrace DB Path is " + _backtraceDatabasePath);
                /* This Url is just a place holder to ensure BacktraceClient.Initialize return happy */
                string _backtraceServerUrl = string.Format("https://submit.backtrace.io/netflix/{0}/json", GetCrashReporterConfig().ID);

                var attributes = new Dictionary<String, String>();
                attributes.Add("CaptureNativeCrashes", "true");

                var configuration = ScriptableObject.CreateInstance<BacktraceConfiguration>();
                configuration.ServerUrl = _backtraceServerUrl;
                configuration.DatabasePath = _backtraceDatabasePath;
                configuration.Enabled = true;
                configuration.CreateDatabase = true;
                configuration.EnableBreadcrumbsSupport = true;

                backtraceClient = BacktraceClient.Initialize(configuration, attributes);
                backtraceClient.BeforeSend = (BacktraceData data) => {
                    string breadcrumbsPath = backtraceClient.Breadcrumbs.GetBreadcrumbLogPath();
                    byte[] exceptionJsonAsBytes = System.Text.Encoding.UTF8.GetBytes(data.ToJson());
                    NativeLibrary.ngp_cloud_log_exception(exceptionJsonAsBytes, (uint)exceptionJsonAsBytes.Length, breadcrumbsPath);
                    return null;
                };
            }
        }

        public override string GetNativeSdkVersion()
        {
            if (NativeLibrary.Available)
            {
                return Marshal.PtrToStringAnsi(NativeLibrary.ngp_cloud_get_native_sdk_version());
            }

            return "0.0.0";
        }

        protected override string GetUnitySdkVersion()
        {
            // https://docs.google.com/document/d/1iSYVFYaDLZYO_A1SvWqs_dvWCvPy8ykcapenU-BbS7s/edit#
            return "0.1.0";
        }

        public override void HideNetflixMenu()
        {
            NfLog.Log("NetflixSdkCloudImpl HideNetflixMenu.");
            if (NativeLibrary.Available)
            {
                NativeLibrary.ngp_cloud_hide_netflix_menu();
            }
        }

        public override void ShowNetflixMenu(int location)
        {
            NfLog.Log("NetflixSdkCloudImpl ShowNetflixMenu.");
            if (NativeLibrary.Available)
            {
                NativeLibrary.ngp_cloud_show_netflix_menu(location);
            }
        }

        public override void ShowNetflixAccessButton()
        {
            NfLog.Log("NetflixSdkCloudImpl ShowNetflixAccessButton.");
            if (NativeLibrary.Available)
            {
                NativeLibrary.ngp_cloud_show_netflix_access_button();
            }
        }

        public override void HideNetflixAccessButton()
        {
            NfLog.Log("NetflixSdkCloudImpl HideNetflixAccessButton.");
            if (NativeLibrary.Available)
            {
                NativeLibrary.ngp_cloud_hide_netflix_access_button();
            }
        }

        public override void SetLocale(NetflixSdk.Locale locale)
        {
            NfLog.Log("NetflixSdkCloudImpl SetLocale.");
            if (NativeLibrary.Available)
            {
                NativeLibrary.ngp_cloud_set_locale(locale.language, locale.country, locale.variant);
            }
        }

        public override SdkApi.CrashReporterConfig GetCrashReporterConfig()
        {
            SdkApi.CrashReporterConfig crashReporterConfig = new SdkApi.CrashReporterConfig
            {
                ID = "b951984649c01142cb98e28f232f31fe",
                guid = "guid1",
            };
            return crashReporterConfig;
        }

        private class CloudMessaging : SdkApi.IMessaging {
            public void OnPushToken(string pushDeviceToken)
            {
                NfLog.Log("NetflixSdkCloudImpl OnPushToken: " + pushDeviceToken);
                if (NativeLibrary.Available)
                {
                    NativeLibrary.ngp_cloud_on_push_token(pushDeviceToken);
                }
            }

            public void OnDeeplinkReceived(string deepLinkURL, bool processedByGame)
            {
                NfLog.Log("NetflixSdkCloudImpl OnDeeplinkReceived: " + deepLinkURL);
                if (NativeLibrary.Available)
                {
                    NativeLibrary.ngp_cloud_on_deeplink_received(deepLinkURL, processedByGame);
                }
            }

            public void OnMessagingEvent(NetflixMessaging.MessagingEventType eventType, string jsonString)
            {
                NfLog.Log("NetflixSdkCloudImpl OnMessagingEvent: " + eventType);
                if (NativeLibrary.Available)
                {
                    NativeLibrary.ngp_cloud_on_messaging_event(eventType, jsonString);
                }
            }
        }

        public override void SendCLEvent(string clTypeName, string eventDataJson)
        {
            NfLog.Log("NetflixSdkCloudImpl SendCLEvent: " + clTypeName + " " + eventDataJson);
            if(NativeLibrary.Available) {
                NativeLibrary.ngp_cloud_send_cl_event(clTypeName, eventDataJson);
            }
        }

        public override void LogInGameEvent(InGameEvent inGameEvent)
        {
            NfLog.Log("NetflixSdkCloudImpl LogInGameEvent: " + inGameEvent.name);
            if(NativeLibrary.Available) {
                NativeLibrary.ngp_cloud_send_cl_event(inGameEvent.name, inGameEvent.ToJson());
            }
        }

        public override Dictionary<string, string> GetTestParams()
        {
            NfLog.Log("NetflixSdkCloudImpl GetTestParams");
            return new Dictionary<string, string>();
        }

        public override void PublishToEventSink(string name, string data)
        {
            NfLog.Log("NetflixSdkCloudImpl PublishToEventSink(" + name + ", " + data + ")");
            if(NativeLibrary.Available) {
                NativeLibrary.ngp_cloud_publish_to_event_sink(name, data);
            }
        }
        
        private class CloudSave : SdkApi.ICloudSave
        {
            public Task<NetflixCloudSave.GetSlotIdsResult> GetSlotIds()
            {
                return PushTaskInternal<NetflixCloudSave.GetSlotIdsResult>((taskId)=>
                {
                    NativeLibrary.ngp_cloud_get_slot_ids(taskId, PopTaskInternal);
                },(ngpResponse)=>
                {
                    var response = ngpResponse.data.ngpProgress_slots;
                    return new NetflixCloudSave.GetSlotIdsResult
                    {
                        status = NetflixCloudSave.CloudSaveStatus.Ok,
                        slotIds = response.Select(o => o.slotClientId).ToList(),
                    };
                },()=>
                {
                    return new NetflixCloudSave.GetSlotIdsResult
                    {
                        status = NetflixCloudSave.CloudSaveStatus.ErrorInternal,
                    };
                });
            }

            public Task<NetflixCloudSave.SaveSlotResult> SaveSlot(string slotId, NetflixCloudSave.SlotInfo slotInfo)
            {
                if (slotInfo == null || slotInfo.GetDataBytes() == null)
                {
                    return new Task<NetflixCloudSave.SaveSlotResult>(() =>
                    {
                        return new NetflixCloudSave.SaveSlotResult()
                        {
                            status = NetflixCloudSave.CloudSaveStatus.ErrorValidation,
                        };
                    });
                }

                return PushTaskInternal<NetflixCloudSave.SaveSlotResult>((taskId)=>
                {
                    NativeLibrary.ngp_cloud_save_slot(taskId, slotId, slotInfo.GetDataBytes(), (uint)slotInfo.GetDataBytes().Length, PopTaskInternal);
                },(ngpResponse)=>
                {
                    NetflixCloudSave.SaveSlotResult result = null;
                    var response = ngpResponse.data.ngpProgress_saveSlot;
                    switch (response.__typename)
                    {
                        case NGPStatus.SaveSlotSuccess:
                        {
                            result = new NetflixCloudSave.SaveSlotResult
                            {
                                status = NetflixCloudSave.CloudSaveStatus.Ok,
                            };
                            break;
                        }

                        case NGPStatus.SlotConflictError:
                        {
                            result = new NetflixCloudSave.SaveSlotResult
                            {
                                status = NetflixCloudSave.CloudSaveStatus.SlotConflict,
                                errorDescription = response.description,
                                conflictResolution = new NetflixCloudSave.ConflictResolution
                                {
                                    remote = new NetflixCloudSave.SlotInfo(Convert.FromBase64String(response.currentSnapshot.directDownload.base64Data)),
                                },
                            };
                            result.conflictResolution.remote.SetServerSyncTimestamp(response.currentSnapshot.createTimestamp);
                            break;
                        }

                        case NGPStatus.ValidationError:
                        {
                            result = new NetflixCloudSave.SaveSlotResult
                            {
                                status = NetflixCloudSave.CloudSaveStatus.ErrorValidation,
                                errorDescription = response.description,
                            };
                            break;
                        }

                        default:
                            break;
                    }
                    return result;
                },()=>
                {
                    return new NetflixCloudSave.SaveSlotResult
                    {
                        status = NetflixCloudSave.CloudSaveStatus.ErrorInternal,
                    };
                });
            }

            public Task<NetflixCloudSave.ReadSlotResult> ReadSlot(string slotId)
            {
                return PushTaskInternal<NetflixCloudSave.ReadSlotResult>((taskId)=>
                {
                    NativeLibrary.ngp_cloud_read_slot(taskId, slotId, PopTaskInternal);
                },(ngpResponse)=>
                {
                    NetflixCloudSave.ReadSlotResult result = null;
                    var response = ngpResponse.data.ngpProgress_slot;
                    switch (response.__typename)
                    {
                        case NGPStatus.ReadSlotSuccess:
                        {
                            result = new NetflixCloudSave.ReadSlotResult
                            {
                                status = NetflixCloudSave.CloudSaveStatus.Ok,
                                slotInfo = new NetflixCloudSave.SlotInfo(Convert.FromBase64String(response.currentSnapshot.directDownload.base64Data)),
                            };
                            result.slotInfo.SetServerSyncTimestamp(response.currentSnapshot.createTimestamp);
                            break;
                        }

                        case NGPStatus.SlotNotFoundError:
                        {
                            result = new NetflixCloudSave.ReadSlotResult
                            {
                                status = NetflixCloudSave.CloudSaveStatus.ErrorUnknownSlotId,
                                errorDescription = response.description,
                            };
                            break;
                        }

                        case NGPStatus.ValidationError:
                        {
                            result = new NetflixCloudSave.ReadSlotResult
                            {
                                status = NetflixCloudSave.CloudSaveStatus.ErrorValidation,
                                errorDescription = response.description,
                            };
                            break;
                        }

                        default:
                            break;
                    }
                    return result;
                },()=>
                {
                    return new NetflixCloudSave.ReadSlotResult
                    {
                        status = NetflixCloudSave.CloudSaveStatus.ErrorInternal,
                    };
                });
            }

            public Task<NetflixCloudSave.DeleteSlotResult> DeleteSlot(string slotId)
            {
                return PushTaskInternal<NetflixCloudSave.DeleteSlotResult>((taskId)=>
                {
                    NativeLibrary.ngp_cloud_delete_slot(taskId, slotId, PopTaskInternal);
                },(ngpResponse)=>
                {
                    NetflixCloudSave.DeleteSlotResult result = null;
                    var response = ngpResponse.data.ngpProgress_deleteSlot;
                    switch (response.__typename)
                    {
                        case NGPStatus.DeleteSlotSuccess:
                        {
                            result = new NetflixCloudSave.DeleteSlotResult
                            {
                                status = NetflixCloudSave.CloudSaveStatus.Ok,
                                errorDescription = response.description,
                            };
                            break;
                        }

                        case NGPStatus.SlotConflictError:
                        {
                            result = new NetflixCloudSave.DeleteSlotResult
                            {
                                status = NetflixCloudSave.CloudSaveStatus.SlotConflict,
                                conflictResolution = new NetflixCloudSave.ConflictResolution
                                {
                                    remote = new NetflixCloudSave.SlotInfo(Convert.FromBase64String(response.currentSnapshot.directDownload.base64Data)),
                                },
                            };
                            result.conflictResolution.remote.SetServerSyncTimestamp(response.currentSnapshot.createTimestamp);
                            break;
                        }

                        case NGPStatus.SlotNotFoundError:
                        {
                            result = new NetflixCloudSave.DeleteSlotResult
                            {
                                status = NetflixCloudSave.CloudSaveStatus.ErrorUnknownSlotId,
                                errorDescription = response.description,
                            };
                            break;
                        }

                        case NGPStatus.ValidationError:
                        {
                            result = new NetflixCloudSave.DeleteSlotResult
                            {
                                status = NetflixCloudSave.CloudSaveStatus.ErrorValidation,
                                errorDescription = response.description,
                            };
                            break;
                        }

                        default:
                            break;
                    }
                    return result;
                },()=>
                {
                    return new NetflixCloudSave.DeleteSlotResult
                    {
                        status = NetflixCloudSave.CloudSaveStatus.ErrorInternal,
                    };
                });
            }

            public Task<NetflixCloudSave.ResolveConflictResult> ResolveConflict(string slotId,
                NetflixCloudSave.CloudSaveResolution resolution)
            {
                return PushTaskInternal<NetflixCloudSave.ResolveConflictResult>(
                    (taskId) => { NativeLibrary.ngp_cloud_resolve_slot_conflict(taskId, slotId, resolution, PopTaskInternal); },
                    (ngpResponse) =>
                    {
                        return new NetflixCloudSave.ResolveConflictResult
                        {
                            status = NetflixCloudSave.CloudSaveStatus.Ok,
                        };
                    }, () =>
                    {
                        return new NetflixCloudSave.ResolveConflictResult
                        {
                            status = NetflixCloudSave.CloudSaveStatus.ErrorInternal,
                        };
                    });
            }
        }

        protected override SdkApi.ISecondScreenInputController CreateSecondScreenInputControllerApi()
        {
            return new NetflixCloudSecondScreenInputController();
        }
        
        protected override SdkApi.IMessaging CreateMessagingApi()
        {
            return new CloudMessaging();
        }

        protected override SdkApi.ICloudSave CreateCloudSaveApi()
        {
            return new CloudSave();
        }

        private static Task<T> PushTaskInternal<T>(Action<ulong> handleNGPRequest, Func<NGPResponse, T> createResult, Func<T> fallbackResult)
        {
            if (!NativeLibrary.Available)
            {
                return Task<T>.Factory.StartNew(() =>
                {
                    return fallbackResult();
                });
            }

            var taskCompletionSource = new TaskCompletionSource<T>(TaskContinuationOptions.RunContinuationsAsynchronously);
            Action<string> callback = (responseAsString)=>
            {
                T result = default(T);
                try
                {
                    var ngpResponse = JsonUtility.FromJson<NGPResponse>(responseAsString);
                    if (ngpResponse != null)
                    {
                        result = createResult(ngpResponse);
                    }
                }
                catch (Exception e)
                {
                    NfLog.Log("NetflixSdkCloudImpl callback JsonUtility.FromJson failed: " + e.Message);
                }

                if (result == null)
                {
                    result = fallbackResult();
                }
                taskCompletionSource.SetResult(result);
            };

            ulong callbackId = 0;
            lock (mCallbacks)
            {
                callbackId = mCurrentCallbackId++;
                mCallbacks[callbackId] = callback;
            }
            handleNGPRequest(callbackId);

            return taskCompletionSource.Task;
        }

        [MonoPInvokeCallback(typeof(NGPCallbackType))]
        private static void PopTaskInternal(ulong id, string responseAsString)
        {
            Action<string> callback = null;
            lock (mCallbacks)
            {
                callback = mCallbacks[id];
                mCallbacks.Remove(id);
            }
            callback(responseAsString);
        }
    }

    internal class NetflixCloudSecondScreenInputController : SdkApi.ISecondScreenInputController
    {
        public void SetActiveLayout(string layoutName)
        {
            NfLog.Log("NetflixCloudSecondScreenInputController SetActiveLayout(" + layoutName + ")");
            Debug.Log("OCGC:VAR:controllerLayout=" + layoutName);
        }
    }
}

#endif // UNITY_STANDALONE_LINUX
