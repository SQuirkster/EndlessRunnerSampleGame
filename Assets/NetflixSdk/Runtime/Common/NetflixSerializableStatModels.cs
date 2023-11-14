using System;
using static Netflix.Stats;

namespace Netflix
{
    namespace SerializableStatModels
    {
        [Serializable]
        /**
         *  AggregatedStatResult contains the result of the GetAggregatedStat API call
         */
        class SerializedAggregatedStatResult
        {
            public StatsStatus status;
            public SerializedAggregatedStat aggregatedStat;
        }

        [Serializable]
        class SerializedStatItem
        {
            public string name;
            public Int64 value;
        }

        [Serializable]
        class SerializedAggregatedStat
        {
            public string name;
            public Int64 value;
        }

        [Serializable]
        class SerializedSubmitStatResponse
        {
            public StatsStatus status;
            public SerializedStatItem submittedStat;
            public SerializedAggregatedStat aggregatedStat;
        }
    }
}

