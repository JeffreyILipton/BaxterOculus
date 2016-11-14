namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Runtime.Serialization;
  using Newtonsoft.Json;

  [JsonObject(MemberSerialization.OptIn)]
  public sealed class MatchmakingEnqueueResult
  {
    public uint AverageWait { get { return averageWait; } }
    public uint MaxExpectedWait { get { return maxExpectedWait; } }
    public string RequestHash { get { return requestHash; } }
    public uint MatchesInLastHourCount { get { return matchesInLastHourCount; } }
    public uint RecentMatchPercentage { get { return recentMatchPercentage; } }
    private MatchmakingEnqueueResult()
    {
    }

    [JsonProperty("average_wait_s")]
    private uint averageWait;

    [JsonProperty("max_expected_wait_s")]
    private uint maxExpectedWait;

    [JsonProperty("trace_id")]
    private string requestHash;

    [JsonProperty("matches_in_last_hour")]
    private uint matchesInLastHourCount;

    [JsonProperty("recent_match_percentage")]
    private uint recentMatchPercentage;
  }
}
