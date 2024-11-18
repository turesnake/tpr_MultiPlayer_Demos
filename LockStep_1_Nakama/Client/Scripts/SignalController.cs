using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;  



public class SignalController : MonoBehaviour
{

    /*
    bool signalReceived = false;  

    void Start()  
    {  
        Application.targetFrameRate = 5;
        // 开始重复调用 CheckSignal 方法  
        InvokeRepeating("CheckSignal", 0f, 0.1f);  

        // 启动自定义循环  
        StartCoroutine(CustomLoop());  
    }  

    void CheckSignal()  
    {  
        // 这里可以放置你的逻辑来决定何时发送信号  
        // 例如，某个条件满足时设置 signalReceived 为 true  
        if (SomeConditionMet())  
        {  
            signalReceived = true;  
        }  
    }  

    bool SomeConditionMet()  
    {  
        // 这里是你的条件判断逻辑  
        // 例如，简单的时间条件  
        return Time.time % 5 < 0.1f; // 每 5 秒触发一次信号  
    }  

    System.Collections.IEnumerator CustomLoop()  
    {  
        while (true)  
        {  
            // 等待信号  
            yield return new WaitUntil(() => signalReceived);  

            // 执行你的循环逻辑  
            Debug.Log("Signal received, executing loop at: " + Time.time);  

            // 重置信号  
            signalReceived = false;  
        }  
    }  
    */

    

    bool signalReceived = false;  

    void Start()  
    {  
        // 开始重复调用 CheckSignal 方法  
        InvokeRepeating("CheckSignal", 0f, 0.1f);  

        // 启动自定义循环  
        StartCustomLoop();  
    }  

    void CheckSignal()  
    {  
        // 这里可以放置你的逻辑来决定何时发送信号  
        if (SomeConditionMet())  
        {  
            signalReceived = true;  
        }  
    }  

    bool SomeConditionMet()  
    {  
        // 这里是你的条件判断逻辑  
        return Time.time % 5 < 0.1f; // 每 5 秒触发一次信号  
    }  

    async void StartCustomLoop()  
    {  
        while (true)  
        {  
            // 等待信号  
            await WaitForSignal();  

            // 执行你的循环逻辑  
            Debug.Log("Signal received, executing loop at: " + Time.time);  

            // 重置信号  
            signalReceived = false;  
        }  
    }  

    async Task WaitForSignal()  
    {  
        // 使用异步等待来降低 CPU 占用  
        while (!signalReceived)  
        {  
            await Task.Delay(10); // 每 10 毫秒检查一次  
        }  
    }



}


