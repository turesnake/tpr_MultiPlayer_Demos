using System.Collections.Generic;
using Nakama.TinyJson;
using Nakama;
using UnityEngine;
using System.Linq; 

using UnityEngine.UI;
using TMPro;



public class NetKTool
{




    public static string PrintIUserPresence( IUserPresence p_ ) 
    {
        string ss = "Username: " + p_.Username +
                    ", SessionId: " + p_.SessionId + 
                    ", UserId: " + p_.UserId + 
                    ", Persistence: " + p_.Persistence + 
                    ", Status: " + p_.Status;
        return ss;
    }







}
