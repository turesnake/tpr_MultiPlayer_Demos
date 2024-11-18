using System.Collections.Generic;

using UnityEngine;
using System.Linq; 

using UnityEngine.UI;
using TMPro;
using UnityEngine.UIElements;
using System.Threading.Tasks;

using Nakama.TinyJson;
using Nakama;
using UnityEngine.SocialPlatforms;





namespace Move_05
{




[System.Serializable]
public class PersistentInputCodeDatas 
{
    public List<string> user_ids = new List<string>();
    public List<int> inputCodes = new List<int>();
}


    
public class UserPresenceData
{
    public IUserPresence userPresence;
    public QueueOperationType opType;
}




public class RelayedClient : MonoBehaviour
{
    IClient _client;
    ISocket _socket;
    ISession _session;
    string _match_id;
    
    IApiAccount _account;

    GameMgr gameMgr;

    Queue<UserPresenceData> userPresenceDataQueue = new Queue<UserPresenceData>();

    public bool isInit = false;


    public async void Init( GameMgr gameMgr_ ) 
    {
        gameMgr = gameMgr_;

        // =========== params: ==============
        const string scheme     = "http";
        string host         = GameConfigs.instance.server.Ip;
        int port            = GameConfigs.instance.server.Port;
        string serverKey    = GameConfigs.instance.server.ServerKey;
        string roomName     = GameConfigs.instance.server.RoomName;
        TprLog.Log("=== host: " + host);
        TprLog.Log("=== port: " + port);
        TprLog.Log("=== serverKey: " + serverKey);
        TprLog.Log("=== roomName: " + roomName);

        // === client:
        _client     = new Client( scheme, host, port, serverKey );

        // === start time:
        string startTime = KTool.GetCurrentTimeString();

        // === session:
        string deviceId = KTool.GetDeviceIdWithSuffix(GameConfigs.instance.DeviceIdSuffix);
        //string deviceId = SystemInfo.deviceUniqueIdentifier;
        TprLog.Log("deviceId = " + deviceId);
        _session = await _client.AuthenticateDeviceAsync(deviceId);

        TprLog.Log("after session");

        // === account:
        _account = await _client.GetAccountAsync(_session);
        IApiUser user = _account.User;
        TprLog.Log("!!! Self User ID =" + user.Id); // == _session.UserId
        //gameMgr.rolesMgr.heroRole.userId = user.Id;
        gameMgr.rolesMgr.ResetHeroRoleUserId(user.Id);

        // !!! 写入 玩家账号数据:
        await WriteToServerAsync_Role( gameMgr.rolesMgr.GetHeroRole(), RoleBH.ServerDataType.All );

        // === socket:
        _socket = _client.NewSocket();
        _socket.Connected       += () => Debug.Log("Socket connected.");
        _socket.Closed          += () => Debug.Log("Socket closed.");
        _socket.ReceivedError   += Debug.LogError;
        _socket.ReceivedMatchState      += OnReceivedMatchState;
        _socket.ReceivedMatchPresence   += OnReceivedMatchPresence;
      
        // === socket connect:
        await _socket.ConnectAsync(_session);
        TprLog.Log("After socket connected.");
        

        // 直接 创建/加入 match (room):
        await RpcCreateAndJoinMatch( roomName );

        // ==========
        isInit = true;
    }


     



    void OnDestroy()
    {
        _socket?.CloseAsync();
    }



    public void Update()
    {
        if( isInit != true ) 
        {
            return;
        }

        // ======== 
        if( userPresenceDataQueue.Count > 0 )
        {
            UserPresenceData e = userPresenceDataQueue.Dequeue();
            if( e.opType == QueueOperationType.Add )
            {
                var roleColor = KTool.GenerateColorFromHash( e.userPresence.UserId.GetHashCode() );

                Vector3 pos = new Vector3(
                    Random.Range( GlobalStates.leftBorder, GlobalStates.rightBorder ),
                    0f,
                    Random.Range( GlobalStates.bottomBorder, GlobalStates.topBorder )
                );
                RoleBH roleBH = gameMgr.rolesMgr.AddOthRole( e.userPresence.Username, e.userPresence.UserId, e.userPresence, roleColor, pos );

                ReadFromServerAsync_Role(roleBH, RoleBH.ServerDataType.All);


            }
            else if( e.opType == QueueOperationType.Remove )
            {
                gameMgr.rolesMgr.RemoveOthRole( e.userPresence.UserId );
            }
            else 
            {
                TprLog.LogError( "opType ERROR: " + e.opType );
            }
        }
    }


    // =========================================

    async Task RpcCreateAndJoinMatch( string roomName_ ) 
    {
        try
        {
            TprLog.Log("call rpc: RpcCreateAndJoinMatch");
            var payload = new Dictionary<string, string> {{ "matchName", roomName_ }};
            IApiRpc response = await _socket.RpcAsync( "RpcCreateOrFindMatch", payload.ToJson() ); // server 端的返回值存储在 Payload 中

            _match_id = response.Payload;
            TprLog.Log("RpcCreateOrFindMatch() ret: match_id = " + _match_id);

            
            // === joint match:
            Dictionary<string,string> metadatas = new Dictionary<string, string>();
            metadatas.Add( "nana", "not_yet" );
            var match = await _socket.JoinMatchAsync( _match_id, metadatas );
            TprLog.Log("joint match success");

            // 收集当前 match 中, 已经在线的 玩家账号:
            foreach (IUserPresence presence in match.Presences)
            {
                if( presence.UserId == gameMgr.rolesMgr.heroRoleUserId )
                {
                    TprLog.Log( "Find Self in match.Presences !!!!!" );
                }
                else 
                {
                    TprLog.Log("-- exist: User name = " + presence.Username + ", id = " + presence.UserId);
                    userPresenceDataQueue.Enqueue( new UserPresenceData(){userPresence = presence, opType = QueueOperationType.Add} );
                }
            }

        }
        catch (ApiResponseException ex)
        {
            Debug.LogFormat("Error: {0}", ex.Message);
        }
    }


    // =================== send OpCode ======================

    public void RpcSendInputCode(int inputCode_) 
    {
        _DoRpcSendInputCode(inputCode_);  // 发了就不管了
    }
    async void _DoRpcSendInputCode( int inputCode_ ) 
    {
        if( isInit != true ) 
        {
            TprLog.LogError("调用时机异常");
            return;
        }
        try
        {
            //TprLog.Log("call rpc: RpcSendInputCode");
            var payload = new Dictionary<string, int> {{ "inputCode", inputCode_ }};
            IApiRpc response = await _socket.RpcAsync( "RpcSendInputCode", payload.ToJson() ); // 返回值: response.Payload
        }
        catch (ApiResponseException ex)
        {
            Debug.LogFormat("Error: {0}", ex.Message);
        }
    }



    // =================== Receive OpCodes regular ======================

    void OnReceivedMatchState( IMatchState newState ) 
    {
        var content = System.Text.Encoding.UTF8.GetString(newState.State);
        switch (newState.OpCode)
        {
            case 11: 
                // ===== inputCodes: =====

                var datas = JsonUtility.FromJson<PersistentInputCodeDatas>(content);

                var rolesMgr = gameMgr.rolesMgr;

                for( int i=0; i<datas.user_ids.Count; i++ )
                {
                    string userId = datas.user_ids[i];
                    int inputCode = datas.inputCodes[i];

                    if(rolesMgr.ContainRoleUserId(userId) )
                    {   
                        rolesMgr.SetRoleInputCode( userId, inputCode );
                    }
                    else if( rolesMgr.heroRoleUserId == userId ) 
                    {
                        rolesMgr.GetHeroRole().inputCode = inputCode;
                    }
                    else 
                    {
                        TprLog.LogError("没找到 role");
                    }
                }
 
                break;
            default:
                TprLog.LogError("异常数据 = " + content);
                break;
        }
    }





    // ----------------- Storage Write/Read ---------------------
    List<WriteStorageObject> writeStorageObjects = new List<WriteStorageObject>();
    public async Task WriteToServerAsync_Role( RoleBH roleBH_, RoleBH.ServerDataType dataType_ )  
    {  
        try  
        {  
            var userId = roleBH_.userId;

            writeStorageObjects.Clear();
            if( KTool.EnumHasFlag<RoleBH.ServerDataType>( dataType_, RoleBH.ServerDataType.Persistent ) )
            {
                string datasJson = JsonUtility.ToJson(roleBH_.persistentDatas);
                writeStorageObjects.Add(new WriteStorageObject()
                    {
                        Collection = "roleDatas",
                        Key = "persistentDatas",
                        //Value = "{ \"suffix\": \"" + GameConfigs.instance.DeviceIdSuffix + "\" }",
                        Value = datasJson,
                        PermissionRead = 2, // Public Read (2), Owner Read (1), or No Read (0).
                        PermissionWrite = 1, // Owner Write (1), or No Write (0).
                    }
                );

            }
            if( KTool.EnumHasFlag<RoleBH.ServerDataType>( dataType_, RoleBH.ServerDataType.Dynamic ) )
            {
                var tmp = new RoleBH.DynamicDatas(){ posWS = roleBH_.transform.position };
                string datasJson = JsonUtility.ToJson(tmp);
                writeStorageObjects.Add(new WriteStorageObject()
                    {
                        Collection = "roleDatas",
                        Key = "dynamicDatas",
                        Value = datasJson,
                        PermissionRead = 2, // Public Read (2), Owner Read (1), or No Read (0).
                        PermissionWrite = 1, // Owner Write (1), or No Write (0).
                    }
                );
            }
            
            await _client.WriteStorageObjectsAsync( _session, writeStorageObjects.ToArray() );     
            TprLog.Log("-write-");      
        }  
        catch (System.Exception ex)  
        {  
            Debug.LogError($"Error WriteToServerAsync_Role: {ex.Message}");  
        }  
    } 

    // --------------------------------------
    List<StorageObjectId> storageObjectIds = new List<StorageObjectId>();
    async void ReadFromServerAsync_Role( RoleBH roleBH_, RoleBH.ServerDataType dataType_ )  
    {  
        try  
        {  
            var userId = roleBH_.userId;
            storageObjectIds.Clear();
            if( KTool.EnumHasFlag<RoleBH.ServerDataType>( dataType_, RoleBH.ServerDataType.Persistent ) ) 
            {
                storageObjectIds.Add(new StorageObjectId()
                    {
                        Collection = "roleDatas",
                        Key = "persistentDatas",
                        UserId = userId
                    }
                );
            }
            if( KTool.EnumHasFlag<RoleBH.ServerDataType>( dataType_, RoleBH.ServerDataType.Dynamic ) ) 
            {
                storageObjectIds.Add(new StorageObjectId()
                    {
                        Collection = "roleDatas",
                        Key = "dynamicDatas",
                        UserId = userId
                    }
                );
            }

            //---
            IApiStorageObjects result = await _client.ReadStorageObjectsAsync(_session, storageObjectIds.ToArray());
            //int count = 0;
            foreach( var e in result.Objects ) 
            {
                //count++;
                if( e.Key == "persistentDatas" )
                {
                    TprLog.Log( "read data; userId:" + userId + ", key: " + e.Key + "; data:" + e.Value );
                    roleBH_.persistentDatas = JsonUtility.FromJson<RoleBH.PersistentDatas>(e.Value);
                    roleBH_.ApplyPersistentDatas();
                }
                else if( e.Key == "dynamicDatas" ) 
                {
                    TprLog.Log( "read data; userId:" + userId + ", key: " + e.Key + "; data:" + e.Value );
                    roleBH_.dynamicDatas = JsonUtility.FromJson<RoleBH.DynamicDatas>(e.Value);
                    roleBH_.ApplyDynamicDatas();
                }
            }
            //TprLog.Log( "Read obj nums = " +  count);  
        }  
        catch (System.Exception ex)  
        {  
            Debug.LogError($"Error ReadFromServerAsync_Role: {ex.Message}");  
        }  
    } 



    // ------------------- Presence Update() --------------------------
    // --- 
    // Register a client-side presence event listener before joining or creating a match
    // -1- For batched events with both a join and leave for the same presence, 
    // -2- process the leave then join for presences already in the list and process the join then leave for presences not in the list
    //     对于具有相同状态的连接和离开的批处理事件，处理离开，然后为列表中已存在的状态连接，处理连接，然后为列表中未存在的状态离开
    // 注意, 是本次 删除/新增 的元素信息, 而不是当前全部在线的 元素信息;
    void OnReceivedMatchPresence( IMatchPresenceEvent presenceEvent_ ) 
    {
        TprLog.Log("ReceivedMatchPresence: Leaves: " + presenceEvent_.Leaves.Count() + ", Joins: " + presenceEvent_.Joins.Count() );
        
        foreach (var presence in presenceEvent_.Joins)
        {
            if( presence.UserId == gameMgr.rolesMgr.heroRoleUserId )
            {
                TprLog.Log( "Add Self !!!!!!!!!!!!!!!!!" );
            }
            else 
            {
                TprLog.Log("-- add: User name = " + presence.Username + ", id = " + presence.UserId);
                userPresenceDataQueue.Enqueue( new UserPresenceData(){userPresence = presence, opType = QueueOperationType.Add} );
            }
        }   

        foreach (var presence in presenceEvent_.Leaves)
        {
            if( presence.UserId == gameMgr.rolesMgr.heroRoleUserId )
            {
                TprLog.Log( "Remove Self !!!!!!!!!!!!!!!!!" );
            }
            else 
            {
                TprLog.Log("-- remove: User name = " + presence.Username + ", id = " + presence.UserId);
                userPresenceDataQueue.Enqueue( new UserPresenceData(){userPresence = presence, opType = QueueOperationType.Remove} );
            }
        }
    }



    






    

}

}

