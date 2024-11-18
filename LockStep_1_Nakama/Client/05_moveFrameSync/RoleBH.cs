using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using Nakama;
using System.Data.Common;


namespace Move_05
{


/*
    绑定在一个 role prefab 上, 控制这个 role 在场景中行动;
    同时支持 hero 和 othPlayer;
*/
public class RoleBH : MonoBehaviour
{
    public enum ServerDataType 
    {
        Persistent = 1,
        Dynamic = 2,
        //---
        All = 3
    }




    [System.Serializable]
    public class PersistentDatas 
    {
        public Color color;
    }


    [System.Serializable]
    public class DynamicDatas 
    {
        public Vector3 posWS; 
    }





    public string userId;

    public int inputCode = 0;
    
    public PersistentDatas persistentDatas = new PersistentDatas();
    public DynamicDatas    dynamicDatas = new DynamicDatas();


    //Color color;
    Material material;    
    bool isInit = false;



    public void Init( string userId_, Color color_ )
    {
        material = transform.GetComponent<Renderer>().material;

        userId = userId_;
        //userPresence = userPresence_;
        persistentDatas.color = color_;

        ApplyPersistentDatas();
        
        // ---
        isInit = true;
    }


    public void Move( Vector2 move2_ )
    {
        if(isInit == false) 
        {
            return;
        }
        Vector3 move3 = new Vector3( move2_.x, 0f, move2_.y );
        Vector3 newPos = transform.position + move3;
        newPos.x = Mathf.Clamp( newPos.x, GlobalStates.leftBorder, GlobalStates.rightBorder );
        newPos.z = Mathf.Clamp( newPos.z, GlobalStates.bottomBorder, GlobalStates.topBorder );
        transform.position = newPos;
    }


    public void MoveTo( Vector3 tgtPos_, float lerpW_ = 1f )
    {
        transform.position = Vector3.Lerp( transform.position, tgtPos_, lerpW_ );
    }


    public void ApplyPersistentDatas()
    {
        material.SetColor("_BaseColor", persistentDatas.color);
    }

    public void ApplyDynamicDatas()
    {
        transform.position = dynamicDatas.posWS;
    }




}

}
