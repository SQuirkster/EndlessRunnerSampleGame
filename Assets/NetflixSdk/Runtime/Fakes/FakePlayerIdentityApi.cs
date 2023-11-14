using System.Collections.Generic;
using System.Threading.Tasks;

namespace Netflix
{
    internal sealed class FakePlayerIdentityApi: SdkApi.IPlayerIdentity
    {
        public PlayerIdentity GetCurrentPlayer()
        {
            NfLog.Log("GetCurrentPlayer");
            return null;
        }

        public Task<GetPlayerIdentitiesResult> GetPlayerIdentities(List<string> playerIds)
        {
            NfLog.Log("GetPlayerIdentities");
            return Task<GetPlayerIdentitiesResult>.Factory.StartNew(() =>
                new GetPlayerIdentitiesResult(RequestStatus.ErrorUnknown, null, null));
        }
    }
}