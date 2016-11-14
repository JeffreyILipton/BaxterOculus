// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

#pragma warning disable 414
namespace Oculus.Platform
{
  public class CAPI
  {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
  #if UNITY_64 || UNITY_EDITOR_64
    public const string DLL_NAME = "LibOVRPlatform64_1";
  #else
    public const string DLL_NAME = "LibOVRPlatform32_1";
  #endif
#else
    public const string DLL_NAME = "ovrplatform";
#endif

    [StructLayout(LayoutKind.Sequential)]
    public struct ovrKeyValuePair {
      public ovrKeyValuePair(string key, string value) {
        key_ = key;
        valueType_ = KeyValuePairType.String;
        stringValue_ = value;

        intValue_ = 0;
        doubleValue_ = 0.0;
      }

      public ovrKeyValuePair(string key, int value) {
        key_ = key;
        valueType_ = KeyValuePairType.Int;
        intValue_ = value;

        stringValue_ = null;
        doubleValue_ = 0.0;
      }

      public ovrKeyValuePair(string key, double value) {
        key_ = key;
        valueType_ = KeyValuePairType.Double;
        doubleValue_ = value;

        stringValue_ = null;
        intValue_ = 0;
      }

      public string key_;
      KeyValuePairType valueType_;

      public string stringValue_;
      public int intValue_;
      public double doubleValue_;
    };

    public static IntPtr ArrayOfStructsToIntPtr(Array ar)
    {
      int totalSize = 0;
      for(int i=0; i<ar.Length; i++) {
        totalSize += Marshal.SizeOf(ar.GetValue(i));
      }

      IntPtr childrenPtr = Marshal.AllocHGlobal(totalSize);
      IntPtr curr = childrenPtr;
      for(int i=0; i<ar.Length; i++) {
        Marshal.StructureToPtr(ar.GetValue(i), curr, false);
        curr = (IntPtr)((long)curr + Marshal.SizeOf(ar.GetValue(i)));
      }
      return childrenPtr;
    }

    public static CAPI.ovrKeyValuePair[] DictionaryToOVRKeyValuePairs(Dictionary<string, object> dict)
    {
      if(dict == null || dict.Count == 0)
      {
        return null;
      }

      var nativeCustomData = new CAPI.ovrKeyValuePair[dict.Count];

      int i = 0;
      foreach(var item in dict)
      {
        if(item.Value.GetType() == typeof(int))
        {
          nativeCustomData[i] = new CAPI.ovrKeyValuePair(item.Key, (int)item.Value);
        }
        else if(item.Value.GetType() == typeof(string))
        {
          nativeCustomData[i] = new CAPI.ovrKeyValuePair(item.Key, (string)item.Value);
        }
        else if(item.Value.GetType() == typeof(double))
        {
          nativeCustomData[i] = new CAPI.ovrKeyValuePair(item.Key, (double)item.Value);
        }
        else
        {
          throw new Exception("Only int, double or string are allowed types in CustomQuery.data");
        }
        i++;
      }
      return nativeCustomData;
    }

    public static byte[] IntPtrToByteArray(IntPtr data, ulong size)
    {
      byte[] outArray = new byte[size];
      Marshal.Copy(data, outArray, 0, (int)size);
      return outArray;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ovrMatchmakingCriterion {
      public ovrMatchmakingCriterion(string key, MatchmakingCriterionImportance importance)
      {
        key_ = key;
        importance_ = importance;

        parameterArray = IntPtr.Zero;
        parameterArrayCount = 0;
      }

      public string key_;
      public MatchmakingCriterionImportance importance_;

      public IntPtr parameterArray;
      public uint parameterArrayCount;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct ovrMatchmakingCustomQueryData {
      public IntPtr dataArray;
      public uint dataArrayCount;

      public IntPtr criterionArray;
      public uint criterionArrayCount;
    };

    public static Dictionary<string, string> DataStoreFromNative(IntPtr pointer) {
      var d = new Dictionary<string, string>();
      var size = (int)CAPI.ovr_DataStore_GetNumKeys(pointer);
      for (var i = 0; i < size; i++) {
        string key = CAPI.ovr_DataStore_GetKey(pointer, i);
        d[key] = CAPI.ovr_DataStore_GetValue(pointer, key);
      }
      return d;
    }

    // Initialization
    [DllImport (DLL_NAME)]
    public static extern bool ovr_UnityInitWrapper(string appId);

    [DllImport (DLL_NAME)]
    public static extern bool ovr_UnityInitWrapperStandalone(string accessToken, IntPtr loggingCB);

    [DllImport (DLL_NAME)]
    public static extern bool ovr_UnityInitWrapperWindows(string appId, IntPtr loggingCB);

    [DllImport (DLL_NAME)]
    public static extern bool ovr_SetDeveloperAccessToken(string accessToken);


    // Message queue access

    [DllImport (DLL_NAME)]
    public static extern IntPtr ovr_PopMessage();

    [DllImport (DLL_NAME)]
    public static extern void ovr_FreeMessage(IntPtr message);

    [DllImport (DLL_NAME)]
    public static extern uint ovr_NetworkingPeer_GetSendPolicy(IntPtr networkingPeer);


    // VOIP

    [DllImport (DLL_NAME)]
    public static extern IntPtr ovr_Voip_CreateEncoder();

    [DllImport(DLL_NAME)]
    public static extern void ovr_Voip_DestroyEncoder(IntPtr encoder);

    [DllImport (DLL_NAME)]
    public static extern IntPtr ovr_Voip_CreateDecoder();

    [DllImport(DLL_NAME)]
    public static extern void ovr_Voip_DestroyDecoder(IntPtr decoder);

    [DllImport (DLL_NAME)]
    public static extern void ovr_VoipDecoder_Decode(IntPtr obj, byte[] compressedData, ulong compressedSize);

    [DllImport(DLL_NAME)]
    public static extern IntPtr ovr_Microphone_Create();

    [DllImport(DLL_NAME)]
    public static extern void ovr_Microphone_Destroy(IntPtr obj);


    // Misc

    [DllImport(DLL_NAME)]
    public static extern void ovr_UnityResetTestPlatform();

    [DllImport (DLL_NAME)]
    public static extern ulong ovr_HTTP_GetWithMessageType(string url, int messageType);

    [DllImport (DLL_NAME)]
    public static extern void ovr_CrashApplication();


    // Requests and free functions

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_Achievements_AddCount(string name, ulong count);

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_Achievements_AddFields(string name, string fields);

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_Achievements_GetAllDefinitions();

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_Achievements_GetAllProgress();

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_Achievements_GetDefinitionsByName(string[] names, int count);

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_Achievements_GetProgressByName(string[] names, int count);

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_Achievements_Unlock(string name);

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_Application_GetVersion();

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_CloudStorage_Load(string bucket, string key);

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_CloudStorage_LoadBucketMetadata(string bucket);

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_CloudStorage_LoadConflictMetadata(string bucket, string key);

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_CloudStorage_LoadHandle(IntPtr handle);

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_CloudStorage_LoadMetadata(string bucket, string key);

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_CloudStorage_ResolveKeepLocal(string bucket, string key, IntPtr remoteHandle);

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_CloudStorage_ResolveKeepRemote(string bucket, string key, IntPtr remoteHandle);

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_CloudStorage_Save(string bucket, string key, byte[] data, uint dataSize, long counter, string extraData);

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_Entitlement_GetIsViewerEntitled();

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_GraphAPI_Get(string url);

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_GraphAPI_Post(string url);

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_HTTP_Get(string url);

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_HTTP_Post(string url);

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_IAP_ConsumePurchase(string sku);

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_IAP_GetProductsBySKU(string[] skus, int count);

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_IAP_GetViewerPurchases();

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_IAP_LaunchCheckoutFlow(string sku);

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_Leaderboard_GetEntries(string leaderboardName, int limit, LeaderboardFilterType filter, LeaderboardStartAt startAt);

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_Leaderboard_GetEntriesAfterRank(string leaderboardName, int limit, ulong afterRank);

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_Leaderboard_GetNextEntries(IntPtr handle);

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_Leaderboard_GetPreviousEntries(IntPtr handle);

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_Leaderboard_WriteEntry(string leaderboardName, long score, byte[] extraData, uint extraDataLength, bool forceUpdate);

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_Matchmaking_Browse(string pool, IntPtr customQueryData);

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_Matchmaking_Cancel(string pool, string requestHash);

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_Matchmaking_Cancel2();

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_Matchmaking_CreateAndEnqueueRoom(string pool, uint maxUsers, bool subscribeToUpdates, IntPtr customQueryData);

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_Matchmaking_CreateRoom(string pool, uint maxUsers, bool subscribeToUpdates);

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_Matchmaking_Enqueue(string pool, IntPtr customQueryData);

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_Matchmaking_EnqueueRoom(UInt64 roomID, IntPtr customQueryData);

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_Matchmaking_JoinRoom(UInt64 roomID, bool subscribeToUpdates);

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_Matchmaking_ReportResultInsecure(UInt64 roomID, ovrKeyValuePair[] data, uint numItems);

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_Matchmaking_StartMatch(UInt64 roomID);

    [DllImport(DLL_NAME)]
    public static extern void ovr_Net_Accept(UInt64 peerID);

    [DllImport(DLL_NAME)]
    public static extern bool ovr_Net_AcceptForCurrentRoom();

    [DllImport(DLL_NAME)]
    public static extern void ovr_Net_Close(UInt64 peerID);

    [DllImport(DLL_NAME)]
    public static extern void ovr_Net_CloseForCurrentRoom();

    [DllImport(DLL_NAME)]
    public static extern void ovr_Net_Connect(UInt64 peerID);

    [DllImport(DLL_NAME)]
    public static extern bool ovr_Net_IsConnected(UInt64 peerID);

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_Net_Ping(UInt64 peerID);

    [DllImport(DLL_NAME)]
    public static extern IntPtr ovr_Net_ReadPacket();

    [DllImport(DLL_NAME)]
    public static extern bool ovr_Net_SendPacket(UInt64 userID, UIntPtr length, byte[] bytes, SendPolicy policy);

    [DllImport(DLL_NAME)]
    public static extern bool ovr_Net_SendPacketToCurrentRoom(UIntPtr length, byte[] bytes, SendPolicy policy);

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_Notification_GetRoomInvites();

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_Notification_MarkAsRead(UInt64 notificationID);

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_Room_CreateAndJoinPrivate(RoomJoinPolicy joinPolicy, uint maxUsers, bool subscribeToUpdates);

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_Room_Get(UInt64 roomID);

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_Room_GetCurrent();

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_Room_GetCurrentForUser(UInt64 userID);

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_Room_GetInvitableUsers();

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_Room_GetModeratedRooms();

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_Room_GetSocialRooms(UInt64 appID);

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_Room_InviteUser(UInt64 roomID, string inviteToken);

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_Room_Join(UInt64 roomID, bool subscribeToUpdates);

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_Room_KickUser(UInt64 roomID, UInt64 userID, int kickDurationSeconds);

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_Room_Leave(UInt64 roomID);

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_Room_SetDescription(UInt64 roomID, string description);

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_Room_UpdateDataStore(UInt64 roomID, ovrKeyValuePair[] data, uint numItems);

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_Room_UpdateOwner(UInt64 roomID, UInt64 userID);

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_Room_UpdatePrivateRoomJoinPolicy(UInt64 roomID, RoomJoinPolicy newJoinPolicy);

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_User_Get(UInt64 userID);

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_User_GetAccessToken();

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_User_GetLoggedInUser();

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_User_GetLoggedInUserFriends();

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_User_GetUserProof();

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_User_NewEntitledTestUser();

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_User_NewTestUser();

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_User_NewTestUserFriends();



    // data model accessors

    [DllImport(DLL_NAME)]
    public static extern uint ovr_AchievementDefinition_GetBitfieldLength(IntPtr obj);

    public static string ovr_AchievementDefinition_GetName(IntPtr obj) {
      return Marshal.PtrToStringAnsi(ovr_AchievementDefinition_GetName_Native(obj));
    }

    [DllImport(DLL_NAME, EntryPoint = "ovr_AchievementDefinition_GetName", CharSet = CharSet.Unicode)]
    private static extern IntPtr ovr_AchievementDefinition_GetName_Native(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_AchievementDefinition_GetTarget(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern AchievementType ovr_AchievementDefinition_GetType(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern IntPtr ovr_AchievementDefinitionArray_GetElement(IntPtr obj, UIntPtr index);

    [DllImport(DLL_NAME)]
    public static extern UIntPtr ovr_AchievementDefinitionArray_GetSize(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern bool ovr_AchievementDefinitionArray_HasNextPage(IntPtr obj);

    public static string ovr_AchievementProgress_GetBitfield(IntPtr obj) {
      return Marshal.PtrToStringAnsi(ovr_AchievementProgress_GetBitfield_Native(obj));
    }

    [DllImport(DLL_NAME, EntryPoint = "ovr_AchievementProgress_GetBitfield", CharSet = CharSet.Unicode)]
    private static extern IntPtr ovr_AchievementProgress_GetBitfield_Native(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_AchievementProgress_GetCount(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern bool ovr_AchievementProgress_GetIsUnlocked(IntPtr obj);

    public static string ovr_AchievementProgress_GetName(IntPtr obj) {
      return Marshal.PtrToStringAnsi(ovr_AchievementProgress_GetName_Native(obj));
    }

    [DllImport(DLL_NAME, EntryPoint = "ovr_AchievementProgress_GetName", CharSet = CharSet.Unicode)]
    private static extern IntPtr ovr_AchievementProgress_GetName_Native(IntPtr obj);

    public static DateTime ovr_AchievementProgress_GetUnlockTime(IntPtr obj) {
        var dt = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return dt.AddSeconds(ovr_AchievementProgress_GetUnlockTime_Native(obj)).ToLocalTime();
    }

    [DllImport(DLL_NAME, EntryPoint = "ovr_AchievementProgress_GetUnlockTime")]
    private static extern ulong ovr_AchievementProgress_GetUnlockTime_Native(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern IntPtr ovr_AchievementProgressArray_GetElement(IntPtr obj, UIntPtr index);

    [DllImport(DLL_NAME)]
    public static extern UIntPtr ovr_AchievementProgressArray_GetSize(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern bool ovr_AchievementProgressArray_HasNextPage(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern int ovr_ApplicationVersion_GetCurrentCode(IntPtr obj);

    public static string ovr_ApplicationVersion_GetCurrentName(IntPtr obj) {
      return Marshal.PtrToStringAnsi(ovr_ApplicationVersion_GetCurrentName_Native(obj));
    }

    [DllImport(DLL_NAME, EntryPoint = "ovr_ApplicationVersion_GetCurrentName", CharSet = CharSet.Unicode)]
    private static extern IntPtr ovr_ApplicationVersion_GetCurrentName_Native(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern int ovr_ApplicationVersion_GetLatestCode(IntPtr obj);

    public static string ovr_ApplicationVersion_GetLatestName(IntPtr obj) {
      return Marshal.PtrToStringAnsi(ovr_ApplicationVersion_GetLatestName_Native(obj));
    }

    [DllImport(DLL_NAME, EntryPoint = "ovr_ApplicationVersion_GetLatestName", CharSet = CharSet.Unicode)]
    private static extern IntPtr ovr_ApplicationVersion_GetLatestName_Native(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern IntPtr ovr_CloudStorageConflictMetadata_GetLocal(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern IntPtr ovr_CloudStorageConflictMetadata_GetRemote(IntPtr obj);

    public static string ovr_CloudStorageData_GetBucket(IntPtr obj) {
      return Marshal.PtrToStringAnsi(ovr_CloudStorageData_GetBucket_Native(obj));
    }

    [DllImport(DLL_NAME, EntryPoint = "ovr_CloudStorageData_GetBucket", CharSet = CharSet.Unicode)]
    private static extern IntPtr ovr_CloudStorageData_GetBucket_Native(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern IntPtr ovr_CloudStorageData_GetData(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern uint ovr_CloudStorageData_GetDataSize(IntPtr obj);

    public static string ovr_CloudStorageData_GetKey(IntPtr obj) {
      return Marshal.PtrToStringAnsi(ovr_CloudStorageData_GetKey_Native(obj));
    }

    [DllImport(DLL_NAME, EntryPoint = "ovr_CloudStorageData_GetKey", CharSet = CharSet.Unicode)]
    private static extern IntPtr ovr_CloudStorageData_GetKey_Native(IntPtr obj);

    public static string ovr_CloudStorageMetadata_GetBucket(IntPtr obj) {
      return Marshal.PtrToStringAnsi(ovr_CloudStorageMetadata_GetBucket_Native(obj));
    }

    [DllImport(DLL_NAME, EntryPoint = "ovr_CloudStorageMetadata_GetBucket", CharSet = CharSet.Unicode)]
    private static extern IntPtr ovr_CloudStorageMetadata_GetBucket_Native(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern long ovr_CloudStorageMetadata_GetCounter(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern uint ovr_CloudStorageMetadata_GetDataSize(IntPtr obj);

    public static string ovr_CloudStorageMetadata_GetExtraData(IntPtr obj) {
      return Marshal.PtrToStringAnsi(ovr_CloudStorageMetadata_GetExtraData_Native(obj));
    }

    [DllImport(DLL_NAME, EntryPoint = "ovr_CloudStorageMetadata_GetExtraData", CharSet = CharSet.Unicode)]
    private static extern IntPtr ovr_CloudStorageMetadata_GetExtraData_Native(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern IntPtr ovr_CloudStorageMetadata_GetHandle(IntPtr obj);

    public static string ovr_CloudStorageMetadata_GetKey(IntPtr obj) {
      return Marshal.PtrToStringAnsi(ovr_CloudStorageMetadata_GetKey_Native(obj));
    }

    [DllImport(DLL_NAME, EntryPoint = "ovr_CloudStorageMetadata_GetKey", CharSet = CharSet.Unicode)]
    private static extern IntPtr ovr_CloudStorageMetadata_GetKey_Native(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern CloudStorageDataStatus ovr_CloudStorageMetadata_GetStatus(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_CloudStorageMetadata_GetUTCSaveTime(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern IntPtr ovr_CloudStorageMetadataArray_GetElement(IntPtr obj, UIntPtr index);

    [DllImport(DLL_NAME)]
    public static extern UIntPtr ovr_CloudStorageMetadataArray_GetSize(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern bool ovr_CloudStorageMetadataArray_HasNextPage(IntPtr obj);

    public static string ovr_CloudStorageUpdateResponse_GetBucket(IntPtr obj) {
      return Marshal.PtrToStringAnsi(ovr_CloudStorageUpdateResponse_GetBucket_Native(obj));
    }

    [DllImport(DLL_NAME, EntryPoint = "ovr_CloudStorageUpdateResponse_GetBucket", CharSet = CharSet.Unicode)]
    private static extern IntPtr ovr_CloudStorageUpdateResponse_GetBucket_Native(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern IntPtr ovr_CloudStorageUpdateResponse_GetHandle(IntPtr obj);

    public static string ovr_CloudStorageUpdateResponse_GetKey(IntPtr obj) {
      return Marshal.PtrToStringAnsi(ovr_CloudStorageUpdateResponse_GetKey_Native(obj));
    }

    [DllImport(DLL_NAME, EntryPoint = "ovr_CloudStorageUpdateResponse_GetKey", CharSet = CharSet.Unicode)]
    private static extern IntPtr ovr_CloudStorageUpdateResponse_GetKey_Native(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern CloudStorageUpdateStatus ovr_CloudStorageUpdateResponse_GetStatus(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern uint ovr_DataStore_Contains(IntPtr obj, string key);

    public static string ovr_DataStore_GetKey(IntPtr obj, int index) {
      return Marshal.PtrToStringAnsi(ovr_DataStore_GetKey_Native(obj, index));
    }

    [DllImport(DLL_NAME, EntryPoint = "ovr_DataStore_GetKey", CharSet = CharSet.Unicode)]
    private static extern IntPtr ovr_DataStore_GetKey_Native(IntPtr obj, int index);

    [DllImport(DLL_NAME)]
    public static extern UIntPtr ovr_DataStore_GetNumKeys(IntPtr obj);

    public static string ovr_DataStore_GetValue(IntPtr obj, string key) {
      return Marshal.PtrToStringAnsi(ovr_DataStore_GetValue_Native(obj, key));
    }

    [DllImport(DLL_NAME, EntryPoint = "ovr_DataStore_GetValue", CharSet = CharSet.Unicode)]
    private static extern IntPtr ovr_DataStore_GetValue_Native(IntPtr obj, string key);

    [DllImport(DLL_NAME)]
    public static extern int ovr_Error_GetCode(IntPtr obj);

    public static string ovr_Error_GetDisplayableMessage(IntPtr obj) {
      return Marshal.PtrToStringAnsi(ovr_Error_GetDisplayableMessage_Native(obj));
    }

    [DllImport(DLL_NAME, EntryPoint = "ovr_Error_GetDisplayableMessage", CharSet = CharSet.Unicode)]
    private static extern IntPtr ovr_Error_GetDisplayableMessage_Native(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern int ovr_Error_GetHttpCode(IntPtr obj);

    public static string ovr_Error_GetMessage(IntPtr obj) {
      return Marshal.PtrToStringAnsi(ovr_Error_GetMessage_Native(obj));
    }

    [DllImport(DLL_NAME, EntryPoint = "ovr_Error_GetMessage", CharSet = CharSet.Unicode)]
    private static extern IntPtr ovr_Error_GetMessage_Native(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern IntPtr ovr_LeaderboardEntry_GetExtraData(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern uint ovr_LeaderboardEntry_GetExtraDataLength(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern int ovr_LeaderboardEntry_GetRank(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern long ovr_LeaderboardEntry_GetScore(IntPtr obj);

    public static DateTime ovr_LeaderboardEntry_GetTimestamp(IntPtr obj) {
        var dt = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return dt.AddSeconds(ovr_LeaderboardEntry_GetTimestamp_Native(obj)).ToLocalTime();
    }

    [DllImport(DLL_NAME, EntryPoint = "ovr_LeaderboardEntry_GetTimestamp")]
    private static extern ulong ovr_LeaderboardEntry_GetTimestamp_Native(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern IntPtr ovr_LeaderboardEntry_GetUser(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern IntPtr ovr_LeaderboardEntryArray_GetElement(IntPtr obj, UIntPtr index);

    [DllImport(DLL_NAME)]
    public static extern UIntPtr ovr_LeaderboardEntryArray_GetSize(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_LeaderboardEntryArray_GetTotalCount(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern bool ovr_LeaderboardEntryArray_HasNextPage(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern bool ovr_LeaderboardEntryArray_HasPreviousPage(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern bool ovr_LeaderboardUpdateStatus_GetDidUpdate(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern uint ovr_MatchmakingEnqueueResult_GetAverageWait(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern uint ovr_MatchmakingEnqueueResult_GetMatchesInLastHourCount(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern uint ovr_MatchmakingEnqueueResult_GetMaxExpectedWait(IntPtr obj);

    public static string ovr_MatchmakingEnqueueResult_GetPool(IntPtr obj) {
      return Marshal.PtrToStringAnsi(ovr_MatchmakingEnqueueResult_GetPool_Native(obj));
    }

    [DllImport(DLL_NAME, EntryPoint = "ovr_MatchmakingEnqueueResult_GetPool", CharSet = CharSet.Unicode)]
    private static extern IntPtr ovr_MatchmakingEnqueueResult_GetPool_Native(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern uint ovr_MatchmakingEnqueueResult_GetRecentMatchPercentage(IntPtr obj);

    public static string ovr_MatchmakingEnqueueResult_GetRequestHash(IntPtr obj) {
      return Marshal.PtrToStringAnsi(ovr_MatchmakingEnqueueResult_GetRequestHash_Native(obj));
    }

    [DllImport(DLL_NAME, EntryPoint = "ovr_MatchmakingEnqueueResult_GetRequestHash", CharSet = CharSet.Unicode)]
    private static extern IntPtr ovr_MatchmakingEnqueueResult_GetRequestHash_Native(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern IntPtr ovr_MatchmakingEnqueueResultAndRoom_GetMatchmakingEnqueueResult(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern IntPtr ovr_MatchmakingEnqueueResultAndRoom_GetRoom(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern uint ovr_MatchmakingRoom_GetPingTime(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern IntPtr ovr_MatchmakingRoom_GetRoom(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern bool ovr_MatchmakingRoom_HasPingTime(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern IntPtr ovr_MatchmakingRoomArray_GetElement(IntPtr obj, UIntPtr index);

    [DllImport(DLL_NAME)]
    public static extern UIntPtr ovr_MatchmakingRoomArray_GetSize(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern IntPtr ovr_Message_GetAchievementDefinitionArray(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern IntPtr ovr_Message_GetAchievementProgressArray(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern IntPtr ovr_Message_GetApplicationVersion(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern IntPtr ovr_Message_GetCloudStorageConflictMetadata(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern IntPtr ovr_Message_GetCloudStorageData(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern IntPtr ovr_Message_GetCloudStorageMetadata(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern IntPtr ovr_Message_GetCloudStorageMetadataArray(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern IntPtr ovr_Message_GetCloudStorageUpdateResponse(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern IntPtr ovr_Message_GetError(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern IntPtr ovr_Message_GetLeaderboardEntryArray(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern IntPtr ovr_Message_GetLeaderboardUpdateStatus(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern IntPtr ovr_Message_GetMatchmakingEnqueueResult(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern IntPtr ovr_Message_GetMatchmakingEnqueueResultAndRoom(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern IntPtr ovr_Message_GetMatchmakingRoomArray(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern IntPtr ovr_Message_GetNativeMessage(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern IntPtr ovr_Message_GetNetworkingPeer(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern IntPtr ovr_Message_GetPingResult(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern IntPtr ovr_Message_GetProductArray(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern IntPtr ovr_Message_GetPurchase(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern IntPtr ovr_Message_GetPurchaseArray(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_Message_GetRequestID(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern IntPtr ovr_Message_GetRoom(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern IntPtr ovr_Message_GetRoomArray(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern IntPtr ovr_Message_GetRoomInviteNotificationArray(IntPtr obj);

    public static string ovr_Message_GetString(IntPtr obj) {
      return Marshal.PtrToStringAnsi(ovr_Message_GetString_Native(obj));
    }

    [DllImport(DLL_NAME, EntryPoint = "ovr_Message_GetString", CharSet = CharSet.Unicode)]
    private static extern IntPtr ovr_Message_GetString_Native(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern Message.MessageType ovr_Message_GetType(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern IntPtr ovr_Message_GetUser(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern IntPtr ovr_Message_GetUserArray(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern IntPtr ovr_Message_GetUserProof(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern bool ovr_Message_IsError(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern UIntPtr ovr_Microphone_ReadData(IntPtr obj, float[] outputBuffer, UIntPtr outputBufferSize);

    [DllImport(DLL_NAME)]
    public static extern void ovr_Microphone_Start(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern void ovr_Microphone_Stop(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern UInt64 ovr_NetworkingPeer_GetID(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern PeerConnectionState ovr_NetworkingPeer_GetState(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern void ovr_Packet_Free(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern IntPtr ovr_Packet_GetBytes(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern SendPolicy ovr_Packet_GetSendPolicy(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern UInt64 ovr_Packet_GetSenderID(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern UIntPtr ovr_Packet_GetSize(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern UInt64 ovr_PingResult_GetID(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern ulong ovr_PingResult_GetPingTimeUsec(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern bool ovr_PingResult_IsTimeout(IntPtr obj);

    public static string ovr_Product_GetDescription(IntPtr obj) {
      return Marshal.PtrToStringAnsi(ovr_Product_GetDescription_Native(obj));
    }

    [DllImport(DLL_NAME, EntryPoint = "ovr_Product_GetDescription", CharSet = CharSet.Unicode)]
    private static extern IntPtr ovr_Product_GetDescription_Native(IntPtr obj);

    public static string ovr_Product_GetFormattedPrice(IntPtr obj) {
      return Marshal.PtrToStringAnsi(ovr_Product_GetFormattedPrice_Native(obj));
    }

    [DllImport(DLL_NAME, EntryPoint = "ovr_Product_GetFormattedPrice", CharSet = CharSet.Unicode)]
    private static extern IntPtr ovr_Product_GetFormattedPrice_Native(IntPtr obj);

    public static string ovr_Product_GetName(IntPtr obj) {
      return Marshal.PtrToStringAnsi(ovr_Product_GetName_Native(obj));
    }

    [DllImport(DLL_NAME, EntryPoint = "ovr_Product_GetName", CharSet = CharSet.Unicode)]
    private static extern IntPtr ovr_Product_GetName_Native(IntPtr obj);

    public static string ovr_Product_GetSKU(IntPtr obj) {
      return Marshal.PtrToStringAnsi(ovr_Product_GetSKU_Native(obj));
    }

    [DllImport(DLL_NAME, EntryPoint = "ovr_Product_GetSKU", CharSet = CharSet.Unicode)]
    private static extern IntPtr ovr_Product_GetSKU_Native(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern IntPtr ovr_ProductArray_GetElement(IntPtr obj, UIntPtr index);

    [DllImport(DLL_NAME)]
    public static extern UIntPtr ovr_ProductArray_GetSize(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern bool ovr_ProductArray_HasNextPage(IntPtr obj);

    public static DateTime ovr_Purchase_GetGrantTime(IntPtr obj) {
        var dt = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return dt.AddSeconds(ovr_Purchase_GetGrantTime_Native(obj)).ToLocalTime();
    }

    [DllImport(DLL_NAME, EntryPoint = "ovr_Purchase_GetGrantTime")]
    private static extern ulong ovr_Purchase_GetGrantTime_Native(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern UInt64 ovr_Purchase_GetPurchaseID(IntPtr obj);

    public static string ovr_Purchase_GetSKU(IntPtr obj) {
      return Marshal.PtrToStringAnsi(ovr_Purchase_GetSKU_Native(obj));
    }

    [DllImport(DLL_NAME, EntryPoint = "ovr_Purchase_GetSKU", CharSet = CharSet.Unicode)]
    private static extern IntPtr ovr_Purchase_GetSKU_Native(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern IntPtr ovr_PurchaseArray_GetElement(IntPtr obj, UIntPtr index);

    [DllImport(DLL_NAME)]
    public static extern UIntPtr ovr_PurchaseArray_GetSize(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern bool ovr_PurchaseArray_HasNextPage(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern UInt64 ovr_Room_GetApplicationID(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern IntPtr ovr_Room_GetDataStore(IntPtr obj);

    public static string ovr_Room_GetDescription(IntPtr obj) {
      return Marshal.PtrToStringAnsi(ovr_Room_GetDescription_Native(obj));
    }

    [DllImport(DLL_NAME, EntryPoint = "ovr_Room_GetDescription", CharSet = CharSet.Unicode)]
    private static extern IntPtr ovr_Room_GetDescription_Native(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern UInt64 ovr_Room_GetID(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern RoomJoinPolicy ovr_Room_GetJoinPolicy(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern RoomJoinability ovr_Room_GetJoinability(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern uint ovr_Room_GetMaxUsers(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern IntPtr ovr_Room_GetOwner(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern RoomType ovr_Room_GetType(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern IntPtr ovr_Room_GetUsers(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern IntPtr ovr_RoomArray_GetElement(IntPtr obj, UIntPtr index);

    [DllImport(DLL_NAME)]
    public static extern UIntPtr ovr_RoomArray_GetSize(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern bool ovr_RoomArray_HasNextPage(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern UInt64 ovr_RoomInviteNotification_GetID(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern UInt64 ovr_RoomInviteNotification_GetRoomID(IntPtr obj);

    public static DateTime ovr_RoomInviteNotification_GetSentTime(IntPtr obj) {
        var dt = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return dt.AddSeconds(ovr_RoomInviteNotification_GetSentTime_Native(obj)).ToLocalTime();
    }

    [DllImport(DLL_NAME, EntryPoint = "ovr_RoomInviteNotification_GetSentTime")]
    private static extern ulong ovr_RoomInviteNotification_GetSentTime_Native(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern IntPtr ovr_RoomInviteNotificationArray_GetElement(IntPtr obj, UIntPtr index);

    [DllImport(DLL_NAME)]
    public static extern UIntPtr ovr_RoomInviteNotificationArray_GetSize(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern bool ovr_RoomInviteNotificationArray_HasNextPage(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern UInt64 ovr_User_GetID(IntPtr obj);

    public static string ovr_User_GetImageUrl(IntPtr obj) {
      return Marshal.PtrToStringAnsi(ovr_User_GetImageUrl_Native(obj));
    }

    [DllImport(DLL_NAME, EntryPoint = "ovr_User_GetImageUrl", CharSet = CharSet.Unicode)]
    private static extern IntPtr ovr_User_GetImageUrl_Native(IntPtr obj);

    public static string ovr_User_GetInviteToken(IntPtr obj) {
      return Marshal.PtrToStringAnsi(ovr_User_GetInviteToken_Native(obj));
    }

    [DllImport(DLL_NAME, EntryPoint = "ovr_User_GetInviteToken", CharSet = CharSet.Unicode)]
    private static extern IntPtr ovr_User_GetInviteToken_Native(IntPtr obj);

    public static string ovr_User_GetOculusID(IntPtr obj) {
      return Marshal.PtrToStringAnsi(ovr_User_GetOculusID_Native(obj));
    }

    [DllImport(DLL_NAME, EntryPoint = "ovr_User_GetOculusID", CharSet = CharSet.Unicode)]
    private static extern IntPtr ovr_User_GetOculusID_Native(IntPtr obj);

    public static string ovr_User_GetPresence(IntPtr obj) {
      return Marshal.PtrToStringAnsi(ovr_User_GetPresence_Native(obj));
    }

    [DllImport(DLL_NAME, EntryPoint = "ovr_User_GetPresence", CharSet = CharSet.Unicode)]
    private static extern IntPtr ovr_User_GetPresence_Native(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern UserPresenceStatus ovr_User_GetPresenceStatus(IntPtr obj);

    public static string ovr_User_GetSmallImageUrl(IntPtr obj) {
      return Marshal.PtrToStringAnsi(ovr_User_GetSmallImageUrl_Native(obj));
    }

    [DllImport(DLL_NAME, EntryPoint = "ovr_User_GetSmallImageUrl", CharSet = CharSet.Unicode)]
    private static extern IntPtr ovr_User_GetSmallImageUrl_Native(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern IntPtr ovr_UserArray_GetElement(IntPtr obj, UIntPtr index);

    [DllImport(DLL_NAME)]
    public static extern UIntPtr ovr_UserArray_GetSize(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern bool ovr_UserArray_HasNextPage(IntPtr obj);

    public static string ovr_UserProof_GetNonce(IntPtr obj) {
      return Marshal.PtrToStringAnsi(ovr_UserProof_GetNonce_Native(obj));
    }

    [DllImport(DLL_NAME, EntryPoint = "ovr_UserProof_GetNonce", CharSet = CharSet.Unicode)]
    private static extern IntPtr ovr_UserProof_GetNonce_Native(IntPtr obj);

    [DllImport(DLL_NAME)]
    public static extern void ovr_VoipDecoder_Decode(IntPtr obj, byte[] compressedData, UIntPtr compressedSize);

    [DllImport(DLL_NAME)]
    public static extern UIntPtr ovr_VoipDecoder_GetDecodedPCM(IntPtr obj, float[] outputBuffer, UIntPtr outputBufferSize);

    [DllImport(DLL_NAME)]
    public static extern void ovr_VoipEncoder_AddPCM(IntPtr obj, float[] inputData, uint inputSize);

    [DllImport(DLL_NAME)]
    public static extern UIntPtr ovr_VoipEncoder_GetCompressedData(IntPtr obj, byte[] outputBuffer, UIntPtr intputSize);

    [DllImport(DLL_NAME)]
    public static extern UIntPtr ovr_VoipEncoder_GetCompressedDataSize(IntPtr obj);

  }
}
