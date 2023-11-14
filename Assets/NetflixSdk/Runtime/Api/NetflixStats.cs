using System;
using System.Threading.Tasks;
using Netflix.SerializableStatModels;

namespace Netflix
{
    public class Stats
    {
        public class StatItem
        {
            private string _name;
            private Int64 _value;

            public StatItem(string name, Int64 value)
            {
                _name = name;
                _value = value;
            }

            /**
             * The stat definition name which uniquely identifies the stat to be updated in the context of
             * the game. This name is provided by the game developer when setting up the stat definition in
             * Netflix Games Developer Portal.
             */
            public string name => _name;


            /**
             * The value of the stat
             */
            public Int64 value => _value;
        }

        public class AggregatedStat
        {
            private string _name;
            private Int64 _value;

            public AggregatedStat(string name, Int64 value)
            {
                _name = name;
                _value = value;
            }

            /**
             * The stat definition name which uniquely identifies the stat to be updated in the context of
             * the game. This name is provided by the game developer when setting up the stat definition in
             * Netflix Games Developer Portal.
             */
            public string name => _name;


            /**
             * The value of the stat
             */
            public Int64 value => _value;
        }

        [Serializable]
        public enum StatsStatus
        {
            // Operation was successful
            Ok = 0,

            // An unexpected error occurred
            ErrorUnknown = StatusCode.ErrorUnknown,

            ErrorStatValueNotFound = StatusCode.ErrorNotFound,

            // Error due to network. Caller can retry the operation at a later time
            ErrorNetwork = StatusCode.ErrorNetwork,

            // Error returned when Stats API call is made before platform is initialized
            ErrorPlatformNotInitialized = StatusCode.ErrorPlatformNotInitialized,

            // Error returned when Stats API call is made without a user profile selected
            ErrorUserProfileNotSelected = StatusCode.ErrorUserProfileNotSelected,

            // Error returned when an operation is interrupted by a profile switch. Caller
            // can retry the operation upon returning to the corresponding profile
            ErrorInterruptedByProfileSwitch = StatusCode.ErrorInterruptedByProfileSwitch,

            // Error due to client-server interactions
            ErrorInternal = StatusCode.ErrorInternal,

            // Error due to API call timed out
            ErrorTimedOut = StatusCode.ErrorTimedOut,

            // Error due to service unavailable
            ErrorUnavailable = StatusCode.ErrorUnavailable,

            // Error due to stat name not found
            ErrorUnknownStat = StatusCode.ErrorCustomStart,

            // Error due to submitting a value for an archived stat
            ErrorArchivedStat = StatusCode.ErrorCustomStart + 1
        }

        /**
         *  SubmitStatResult contains the result of the SubmitStat API call
         */
        public class SubmitStatResult
        {
            public StatsStatus status;
            public StatItem submittedStat;
            public AggregatedStat aggregatedStat;

            internal SubmitStatResult()
            {

            }

            internal SubmitStatResult(SerializedSubmitStatResponse response)
            {
                this.status = response.status;


                bool hasAggregatedStat = response.aggregatedStat != null && response.aggregatedStat.name != null && response.aggregatedStat.name != "";

                if (hasAggregatedStat)
                {
                    aggregatedStat = new AggregatedStat(response.aggregatedStat.name, response.aggregatedStat.value);
                }

                bool hasSubmittedStat = response.submittedStat != null && response.submittedStat.name != null && response.submittedStat.name != "";

                if (hasSubmittedStat)
                {
                    submittedStat = new StatItem(response.submittedStat.name, response.submittedStat.value);
                }
            }
        }

        [Serializable]
        /**
         *  AggregatedStatResult contains the result of the GetAggregatedStat API call
         */
        public class AggregatedStatResult
        {
            public StatsStatus status;
            public AggregatedStat aggregatedStat;

            internal AggregatedStatResult()
            {

            }

            internal AggregatedStatResult(SerializedAggregatedStatResult response)
            {
                this.status = response.status;


                bool hasAggregatedStat = response.aggregatedStat != null && response.aggregatedStat.name != null && response.aggregatedStat.name != "";

                if (hasAggregatedStat)
                {
                    aggregatedStat = new AggregatedStat(response.aggregatedStat.name, response.aggregatedStat.value);
                }
            }
        }


        /**
         * Asynchronously submits a stat for the current active user profile within a pre-determined
         * time frame. If the device is offline or if the submission results in an error, an error
         * status is returned.  If the SDK is unable to submit the stat in timely matter, a timeout
         * error will be returned.  The stat will not be cached in local storage for later delivery.
         *
         * Use this API if the stat submission is required in order to advance the game state.
         *
         * @param statItem - The stat item to submit
         * 
         * Returns a task object that will eventually return the result
         */
        public static Task<SubmitStatResult> SubmitStatNow(StatItem statItem)
        {
            return SdkHolder.nfsdk.GetStatsApi().SubmitStatNow(statItem);
        }

        /**
         * Fire and forget API to submit a stat for the current active user profile.
         * The SDK will make a best effort in syncing the stat with the server. If the device is
         * offline or encounters a transient error, the statItem will be cached in local storage
         * to be retried at a later time.
         *
         * Use this API if the stat submission can be done in the background without affecting the game
         * state.
         *
         * If using the releaseWithLogs version of the SDK, errors will be logged via the android logger.
         * Sample error log:
         *   E nf_stats: submitStat("coins", 214) failed with status ERROR_UNAVAILABLE
         *
         * @param statItem - The stat item to submit
         */
        public static void SubmitStat(StatItem statItem)
        {
            SdkHolder.nfsdk.GetStatsApi().SubmitStat(statItem);
        }

        /**
         * Asynchronously retrieves the latest stat aggregation value of a stat for the current active
         * user profile within a pre-determined time frame. If the device is offline or if the
         * submission results in an error, an error status is returned. If the SDK is unable to
         * return the aggregated stat in timely matter, a timeout error will be returned.
         *
         * This API flushes any pending stat submissions on the client before returning the aggregated
         * stat value.
         *
         * @param statName the stat name as defined when setting up the stat
         * 
         * Returns a task object that will eventually return the result
         */
        public static Task<AggregatedStatResult> GetAggregatedStat(String statName)
        {
            return SdkHolder.nfsdk.GetStatsApi().GetAggregatedStat(statName);
        }
    }
}

