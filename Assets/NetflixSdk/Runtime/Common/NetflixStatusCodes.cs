using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Netflix
{
    internal enum StatusCode
    {
        Ok = 0,
        // 1 to 999 reserved for per-feature status codes

        // Common error codes

        ErrorUnknown = 1000,
        ErrorNotFound = 1001,
        ErrorSlotLimitExceeded = 1002,
        ErrorNetwork = 1003,
        ErrorPlatformNotInitialized = 1004,
        ErrorUserProfileNotSelected = 1005,
        ErrorInterruptedByProfileSwitch = 1006,
        ErrorSizeLimitExceeded = 1007,
        ErrorIO = 1008,
        ErrorInternal = 1009,
        ErrorValidation = 1010,
        ErrorTimedOut = 1011,
        ErrorUnavailable = 1012,

        // Start of per-feature custom error codes
        ErrorCustomStart = 2000,
    }
}
