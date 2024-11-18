using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;


namespace Move_05
{


/*
    位于 .exe 根目录下的配置文件: gameConfigs.json
*/
[System.Serializable]
public class GameConfigs
{
    [System.Serializable]
    public class Server 
    {
        public string Ip = "47.100.197.97"; // "aliyun"
        public int Port = 7350;
        public string ServerKey = "key_tapir";
        public string RoomName = "box";
    }


    [System.Serializable]
    public class Debug 
    {
        public int MaxLogLineNum = 20;
        public bool isOpenUILog = true;
    }



    // ==========================================
    public string DeviceIdSuffix = "";
    
    public Server server;
    public Debug debug;




    // ================== instance =================== 
    public static GameConfigs instance = new GameConfigs();

    public static void TryReadGameConfigs() 
    {
        var fullPath = TprIO.NormalizePathSeparator( System.IO.Path.Combine( TprIO.GetAppParentFolderPath(), "gameConfigs.json" ));
        
        var readFlag = TprIO.ReadFile( fullPath, out string jsonDataStr );
        if( readFlag ) 
        {
            // ---
            GameConfigs readGameConfig = JsonUtility.FromJson<GameConfigs>(jsonDataStr);
            instance = readGameConfig;

            // --- debug:
            // Debug.LogError( "DeviceIdSuffix = " + readGameConfig.DeviceIdSuffix );
            // Debug.LogError( "MaxLogLineNum = " + readGameConfig.MaxLogLineNum );
            // Debug.LogError( "isOpenUILog = " + readGameConfig.isOpenUILog );
        }
        else 
        {
            TprLog.LogError( "读取 gameConfigs.json 文件失败, 使用默认配置参数" );
            //-- 手写一些默认配置:
            //...
        }
        TprLog.Log("gameConfigs fullPath: " + fullPath);
    }

}



}
