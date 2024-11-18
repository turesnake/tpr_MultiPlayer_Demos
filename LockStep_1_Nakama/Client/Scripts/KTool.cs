using System.Collections.Generic;
//using Nakama.TinyJson;
//using Nakama;
using UnityEngine;
using System.Linq; 

using UnityEngine.UI;
using TMPro;



public class KTool
{




    public static string GetCurrentTimeString( string path_ = null )  
    {  
        if( path_ == null )
        {
            path_ = "yyyyMMddHHmmss"; // 格式: "yyyyMMddHHmmss"  
        }
        // 获取当前时间  
        System.DateTime now = System.DateTime.Now;   
        string timeString = now.ToString(path_);
        return timeString;  
    }


    /// <summary>
    /// 判断游戏对象是否为空
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool IsNull(object obj)
    {
        var unityObj = obj as UnityEngine.Object;

        if (!ReferenceEquals(unityObj, null))
        {
            return unityObj == null;
        }

        return obj == null;
    }


    /// <summary>
    /// 判断对象是否为空
    /// </summary>
    /// <param name="instance"></param>
    /// <returns></returns>
    public static bool IsActive(GameObject instance)
    {
        return instance && instance.activeSelf;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="instance"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool IsActive<T>(T instance) where T : Component
    {
        return instance && instance.gameObject.activeSelf;
    }




    public static Color GenerateColorFromHash(int hash_)  
    {          
        // 使用哈希值生成RGB颜色  
        float r = (hash_ & 0xFF) / 255f; // 取哈希值的低8位作为红色分量  
        float g = ((hash_ >> 8) & 0xFF) / 255f; // 取哈希值的中间8位作为绿色分量  
        float b = ((hash_ >> 16) & 0xFF) / 255f; // 取哈希值的高8位作为蓝色分量  
        return new Color(r, g, b);  
    } 



    public static string GetDeviceIdWithSuffix( string suffix_ ) 
    {
        return SystemInfo.deviceUniqueIdentifier + "_" + suffix_;
    }


    public static bool EnumHasFlag<T>(T enumValue, T flag) where T : System.Enum  
    {  
        // 将枚举值转换为整数进行位运算  
        var value = System.Convert.ToInt32(enumValue);  // 若参数为 null, ToInt32() 返回 0 而不是抛出异常;
        var flagValue = System.Convert.ToInt32(flag);  
        return (value & flagValue) == flagValue;  
    }  



    public static int SetBitMask(int value_, int bitPos_, bool bitValue_)  
    {  
        bitPos_ = Mathf.Clamp( bitPos_, 0, 31 );
        if (bitValue_)  
        {   
            return value_ | (1 << bitPos_);   // Set the bit to 1  
        }  
        else  
        {  
            return value_ & ~(1 << bitPos_);  // Set the bit to 0  
        }  
    } 


    public static bool GetBitMask(int value_, int bitPos_)  
    {  
        bitPos_ = Mathf.Clamp( bitPos_, 0, 31 );
        return (value_ & (1 << bitPos_)) != 0;
        /*
        if (bitValue_)  
        {   
            return value_ | (1 << bitPos_);   // Set the bit to 1  
        }  
        else  
        {  
            return value_ & ~(1 << bitPos_);  // Set the bit to 0  
        } 
        */ 
    } 





}
