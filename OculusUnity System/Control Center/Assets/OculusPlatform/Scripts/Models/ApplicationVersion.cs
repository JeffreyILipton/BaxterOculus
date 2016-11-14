namespace Oculus.Platform.Models
{
  using UnityEngine;
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Runtime.Serialization;
  using Newtonsoft.Json;

  [JsonObject(MemberSerialization.OptIn)]
  public class ApplicationVersion
  {
    public string CurrentName { get { return _CurrentName; } }
    public int CurrentCode { get { return _CurrentCode; } }
    public string LatestName { get { return _LatestName; } }
    public int LatestCode { get { return _LatestCode; } }

    private ApplicationVersion()
    {
    }

    [JsonProperty("currentVersion")]
    private string _CurrentName;

    [JsonProperty("currentVersionCode")]
    private int _CurrentCode;

    [JsonProperty("latestVersion")]
    private string _LatestName;

    [JsonProperty("latestVersionCode")]
    private int _LatestCode;
  }
}
