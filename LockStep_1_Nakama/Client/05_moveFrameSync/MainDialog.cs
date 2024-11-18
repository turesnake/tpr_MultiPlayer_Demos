using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;



namespace Move_05
{


public class MainDialog : MonoBehaviour
{
    public static MainDialog instance; // 简易版 singletom


    Queue<string> textLineQueue = new Queue<string>();

    TextMeshProUGUI textLines;

    string tmpText = "";

    void Awake()
    {   
        instance = this;
        //---
        var textLines_tf = transform.Find("textLines");
        textLines = textLines_tf.GetComponent<TextMeshProUGUI>();
        textLines.text = "---";
    }

    public void Init()
    {
        
    
    }


    public void SelfUpdate()
    {
        if( textLines.text.Length != tmpText.Length )
        {
            textLines.text = tmpText;
        }    
    }



    public void AddLogLine( string newLine_ ) 
    {        
        if( GameConfigs.instance.debug.isOpenUILog == false )
        {
            return;
        }
        // ===
        textLineQueue.Enqueue(newLine_);
        while( textLineQueue.Count > GameConfigs.instance.debug.MaxLogLineNum )
        {
            textLineQueue.Dequeue();
        }
        // ---
        string[] strs = new string[GameConfigs.instance.debug.MaxLogLineNum];
        textLineQueue.CopyTo(strs, 0);
        // ---
        string ss = "";
        for( int i=0; i<strs.Length; i++ )
        {
            ss += strs[i] + "\n";
        }
        tmpText = ss;
    }

}

}
