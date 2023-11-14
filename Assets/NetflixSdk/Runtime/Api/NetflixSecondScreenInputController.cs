using System;
using UnityEngine;

namespace Netflix
{
    public class SecondScreenInputController
    {
        // Sets the active second screen layout
        public static void SetActiveLayout(string layoutName)
        {
            try
            {
                var controller = SdkHolder.nfsdk.GetSecondScreenInputControllerApi();
                controller.SetActiveLayout(layoutName);
            }
            catch (NotImplementedException)
            {
                NfLog.Log("SecondScreenInputController - Not supported on current platform");
            }
        }
    }
}
