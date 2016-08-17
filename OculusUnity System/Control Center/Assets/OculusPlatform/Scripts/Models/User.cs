namespace Oculus.Platform.Models
{
  using UnityEngine;
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using Newtonsoft.Json;
  using System.Runtime.Serialization;

  [JsonObject(MemberSerialization.OptIn)]
  public class User
  {
    //Public interface
    public string OculusID { get {return _OculusID;} }
    public UInt64 ID { get {return _ID;} }
	  public string InviteToken { get {return _InviteToken;} }
	  public string Presence { get {return _Presence;} }
    public UserPresenceStatus PresenceStatus { get {return _PresenceStatus;} }
    public string ImageURL { get {return _ProfileURL;} }

    //Internal
    [JsonProperty("alias")]
    private string _OculusID;

    [JsonProperty("id")]
    private UInt64 _ID;

    [JsonProperty("token")]
    private string _InviteToken;

    [JsonProperty("presence")]
    private string _Presence;

    [JsonProperty("presence_status")]
    private string _PresenceStatusRaw;
    private UserPresenceStatus _PresenceStatus;

    [JsonProperty("profile_url")]
    private string _ProfileURL;


    [OnDeserialized]
    private void OnDeserializedMethod(StreamingContext context)
    {
      //Handle PresenceStatus
      if ("ONLINE".Equals(_PresenceStatusRaw)) {
        _PresenceStatus = UserPresenceStatus.Online;
      } else if ("OFFLINE".Equals(_PresenceStatusRaw)) {
        _PresenceStatus = UserPresenceStatus.Offline;
      } else {
        _PresenceStatus = UserPresenceStatus.Unknown;
      }
      _PresenceStatusRaw = null;
    }
  }

  public class UserList : DeserializableList<User> {}
}
