using System.Collections.Generic;
using System.Threading.Tasks;

namespace Netflix
{
    public class NetflixPlayerIdentity
    {
        public static PlayerIdentity GetCurrentPlayer()
        {
            return SdkHolder.nfsdk.GetPlayerIdentityApi().GetCurrentPlayer();
        }

        public static Task<GetPlayerIdentitiesResult> GetPlayerIdentities(List<string> playerIds)
        {
            return SdkHolder.nfsdk.GetPlayerIdentityApi().GetPlayerIdentities(playerIds);
        }
    }

    public class PlayerIdentity
    {
        public PlayerIdentity(string playerId, string handle)
        {
            this.playerId = playerId;
            this.handle = handle;
        }

        /**
         * Returns the unique player ID. This ID is not intended for UI display purposes
         * but for using as a primary key for any player data lookup or storage.
         */
        public readonly string playerId;

        /**
         * Returns a handle that can be used to publicly identify the player in social contexts
         * for example, game lobbies, leaderboards, etc.
         */
        public readonly string handle;
    }

    public class GetPlayerIdentitiesResult
    {
        public GetPlayerIdentitiesResult(RequestStatus status, string description, Dictionary<string, PlayerIdentityResult> identities)
        {
            this.status = status;
            this.description = description;
            this.identities = identities;
        }

        /**
         * The status of the request
         */
        public readonly RequestStatus status;

        /**
         * The description of the status (optional, nullable)
         */
        public readonly string description;

        /**
         *  If successful, contains the map of unique playerIds to PlayerIdentityResult objects
         *  Otherwise, value is null
         */
        public readonly Dictionary<string, PlayerIdentityResult> identities;
    }


    public class PlayerIdentityResult
    {
        public PlayerIdentityResult(PlayerIdentityStatus status, PlayerIdentity player)
        {
            this.status = status;
            this.playerIdentity = player;
        }

        /**
         * The individual status of retrieving the player identity.
         */
        public readonly PlayerIdentityStatus status;

        /**
         * If successful, contains the player identity object
         * Otherwise, value is null
         */
        public readonly PlayerIdentity playerIdentity;
    }

    public enum RequestStatus
    {
        /**
         *  Success
         */
        Ok = 0,

        /**
         *  Unknown error
         */
        ErrorUnknown = 1000,

        /**
         *  Size limit exceeded
         *  Maximum size limit for getting player identities is 25
         */
        ErrorLimitExceeded = 1002,


        /**
         * Error due to network. Caller can retry the operation at a later time
         */
        ErrorNetwork = 1003,

        /**
         * Error returned when the API call is made before platform is initialized
         */
        ErrorPlatformNotInitialized = 1004
    }

    public enum PlayerIdentityStatus
    {
        /** 
         * Success
         */
        Ok = 0,

        /**
         * Player identity is not found. This can happen when the player profile has been deleted.
         * The game can remove this entry.
        */
        NotFound = 1,

        /**
         * Player identity is not available at this time. This can happen when the player has not set
         * a handle or if there was an error retrieving it.
         * The game can either hide this entry or use a placeholder identity for this player.
         */
        Unavailable = 2
    }
}
