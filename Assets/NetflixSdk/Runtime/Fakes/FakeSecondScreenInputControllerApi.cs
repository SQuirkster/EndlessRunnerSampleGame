namespace Netflix
{
    internal sealed class FakeSecondScreenInputController: SdkApi.ISecondScreenInputController
    {
        public void SetActiveLayout(string layoutName)
        {
            NfLog.Log("SetActiveLayout: " + layoutName);
        }
    }
}