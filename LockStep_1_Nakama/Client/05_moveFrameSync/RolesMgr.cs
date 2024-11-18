using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cinemachine;

using Nakama;
using System.Threading.Tasks;


namespace Move_05
{

/*
    同时管理 hero 和 othPlayers;

*/
public class RolesMgr : MonoBehaviour
{
    public CinemachineVirtualCamera followvcam1; 
   
    GameMgr gameMgr;

    Transform othRolesParentTF;


    public HeroData heroData = new HeroData(); // !! 暂未使用


    public string heroRoleUserId;
    
    List<RoleBH> roles = new List<RoleBH>(); // todo: 删除的元素直接写 null, 最简版本
    Dictionary<string,int> roleNameMap = new Dictionary<string, int>(); // k:user_id, v:othRoles_idx



    bool isInit = false;
    long frameIdx = 0;


    void Awake()
    {
        Debug.Assert(followvcam1);
    }
    
    public void Init( GameMgr gameMgr_ )
    {
        gameMgr = gameMgr_;

        // --- create scene nodes:
        othRolesParentTF = new GameObject("othRoles").transform;
        othRolesParentTF.SetParent( transform, false );


        // --- create heroRole:
        string deviceId = KTool.GetDeviceIdWithSuffix(GameConfigs.instance.DeviceIdSuffix);
        var heroColor = KTool.GenerateColorFromHash(deviceId.GetHashCode());

        heroRoleUserId = "000"; // 先写个假的;
        RoleBH newHeroRole = CreateNewRole( "Hero", heroRoleUserId, heroColor, transform );
        followvcam1.Follow = newHeroRole.transform;

        // ---
        frameIdx = 0;
        isInit = true;
    }

    
    public void ResetHeroRoleUserId( string newUserId_ )
    {
        Debug.Assert( roleNameMap.ContainsKey(newUserId_) == false );
        roleNameMap.Add( newUserId_, roleNameMap[heroRoleUserId] );
        roleNameMap.Remove( heroRoleUserId );
        heroRoleUserId = newUserId_;
    }

    public RoleBH GetHeroRole() 
    {
        return roles[roleNameMap[heroRoleUserId]];
    }

    public bool ContainRoleUserId( string userId_ ) 
    {
        return roleNameMap.ContainsKey(userId_);
    }

    public void SetRoleInputCode( string userId_, int inputCode_ ) 
    {
        int idx = roleNameMap[userId_];
        roles[idx].inputCode = inputCode_;
    }


    public void SelfUpdate()
    {
        if(isInit == false) 
        {
            return;
        }
        frameIdx++;
        

        //--- 基于自身现有的 inputCode, 执行运动:
        float moveSpeed = gameMgr.inputMgr.moveSpeed * Time.deltaTime;
        for( int i=0; i<roles.Count; i++ )
        {
            var othRole = roles[i];
            othRole.Move( InputMgr.InputCode2RawMove( othRole.inputCode, moveSpeed ) );
        }
    }
    


    RoleBH CreateNewRole( string name_, string userId_, Color color_, Transform parentTF_ ) 
    {
        var newgo = GameObject.Instantiate( gameMgr.rolePrefab, parentTF_);
        var tf = newgo.transform;
        tf.name = name_;
        tf.localScale = Vector3.one;
        tf.localRotation = Quaternion.identity;

        var roleBH = tf.GetComponent<RoleBH>();
        roleBH.Init(userId_, color_);

        //--
        int idx = roles.Count;
        roles.Add(roleBH);
        roleNameMap.Add( userId_, idx );
        //---
        return roleBH;
    }


    public RoleBH AddOthRole( string name_, string userId_, IUserPresence userPresence_, Color color_, Vector3 posWS_ ) 
    {
        if( roleNameMap.ContainsKey(userId_) )
        {
            TprLog.LogError("发现目标 othRole 已经存在, name = " + name_);
            return null;
        }
        //---
        RoleBH newRole = CreateNewRole( name_, userPresence_.UserId, color_, othRolesParentTF );
        var tf = newRole.transform;
        tf.position = posWS_;

        //---
        return newRole;
    }


    public bool RemoveOthRole( string userId_ ) 
    {
        if( roleNameMap.ContainsKey(userId_) == false )
        {
            TprLog.LogError("未找到目标 othRole, userId = " + userId_);
            return false;
        }
        int idx = roleNameMap[userId_];
        if( !(idx >= 0 && idx < roles.Count) )
        {
            TprLog.LogError("roleNameMap 内索引越界; idx = " + idx);
            return false;
        }
        //---
        var tgtRole = roles[idx];
        roleNameMap.Remove(userId_); // todo: 也许可做返回值检查...
        roles[idx] = null; // todo: tmp, 不节省空间的做法...
        //---
        GameObject.Destroy( tgtRole.gameObject );
        //---
        return true;
    }




}

}
