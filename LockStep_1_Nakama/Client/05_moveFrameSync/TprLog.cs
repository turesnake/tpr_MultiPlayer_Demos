using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;


namespace Move_05
{


public enum TprLogMode 
{
    Console = 1,
    UI = 2,
    LogFile = 4,
    //---
    NoUI = 5,
    All = 7,
} 



public class TprLog
{
    static string logFileFullPath;

    //public static Queue<string> logQue = new Queue<string>();



    static void TryDelayInit() 
    {
        if( logFileFullPath != null )
        {
            return;
        }
        var folderFullPath = TprIO.GetAppParentFolderPath();
        logFileFullPath = TprIO.NormalizePathSeparator( System.IO.Path.Combine( TprIO.GetAppParentFolderPath(), "gameLogs.md" ));
        TprIO.CheckAndCreateDirectory(folderFullPath);
        // if( File.Exists(logFileFullPath) == false ) 
        // {
        //     File.Create(logFileFullPath); // !!! 可能引发下方 TprIO.WriteToFile() 的 IOException: Sharing violation on path 问题; 未来再改;
        // }
    }



    // 普通 log: 可选配写入三个 output 端口;
    public static void Log( string msg_, TprLogMode mode_ = TprLogMode.All ) 
    {        
        TryDelayInit();

        // -1-:
        if( ((int)mode_ & (int)TprLogMode.Console) == (int)TprLogMode.Console )
        { 
            Debug.Log(msg_);
        }
        
        // -2-:
        if( ((int)mode_ & (int)TprLogMode.UI) == (int)TprLogMode.UI )
        { 
            var mainDialog = MainDialog.instance;
            if( mainDialog != null )
            {
                mainDialog.AddLogLine(msg_);
            }
        }
        // -3-:
        if( ((int)mode_ & (int)TprLogMode.LogFile) == (int)TprLogMode.LogFile )
        { 
            string logData = KTool.GetCurrentTimeString("\nyyyy_MMdd_HH:mm:ss.fff: ") + msg_;
            TprIO.WriteToFile( logData, logFileFullPath, FileMode.Append );
        }
        
    }


    // 报错: 可选配写入三个 output 端口;
    public static void LogError( string msg_, TprLogMode mode = TprLogMode.NoUI ) 
    {
        TryDelayInit();

        // -1-:
        if( ((int)mode & (int)TprLogMode.Console) == (int)TprLogMode.Console )
        { 
            //Debug.LogError("jjjjjj");
            Debug.LogError(msg_);
        }
        
        // -2-:
        if( ((int)mode & (int)TprLogMode.UI) == (int)TprLogMode.UI )
        { 
            var mainDialog = MainDialog.instance;
            if( mainDialog != null )
            {
                mainDialog.AddLogLine(msg_);
            }
        }
        // -3-:
        if( ((int)mode & (int)TprLogMode.LogFile) == (int)TprLogMode.LogFile )
        { 
            string logData = KTool.GetCurrentTimeString("\nyyyy_MMdd_HH:mm:ss.fff: [-ERROR-]: ") + msg_;
            TprIO.WriteToFile( logData, logFileFullPath, FileMode.Append );
        }
        
    }






}

}
