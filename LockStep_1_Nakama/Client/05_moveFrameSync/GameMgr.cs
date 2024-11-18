using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Move_05
{

/*
    游戏主流程 总管理
*/
public class GameMgr : MonoBehaviour
{
    public RelayedClient relayedClient; 
    public InputMgr inputMgr;
    public RolesMgr rolesMgr;
    public MainDialog mainDialog;
    public GameObject rolePrefab;
    

    int frameCount = 0;




    void Awake()
    {
        Debug.Assert(rolesMgr);
        Debug.Assert(inputMgr);
        Debug.Assert(relayedClient);
        Debug.Assert(mainDialog);
        Debug.Assert(rolePrefab);

        
    }


    void Start() 
    {
        TprLog.Log("======================== Game Start ========================", TprLogMode.LogFile);
        // !!! must read first:
        GameConfigs.TryReadGameConfigs();


        inputMgr.Init();
        rolesMgr.Init(this);
        mainDialog.Init();
        relayedClient.Init(this);
        
        
    }



    void Update()
    {
        if( !relayedClient.isInit )
        {
            return;
        }

        inputMgr.SelfUpdate();

        // 只要 inputCode 发送变化就send, 不考虑周期;
        if( frameCount%5==0 && inputMgr.isInputCodeChange) 
        {
            relayedClient.RpcSendInputCode(inputMgr.inputCode);
            inputMgr.isInputCodeChange = false;
        }

        rolesMgr.SelfUpdate();
        mainDialog.SelfUpdate();


        //---
        frameCount++;
    }


    // ==============================

    





}

}
