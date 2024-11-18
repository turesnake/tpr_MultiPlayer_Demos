using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Move_05
{

//[DefaultExecutionOrder(-100)]
public class InputMgr : MonoBehaviour
{

    public enum OpCodeBitPos 
    {
        A = 1,
        D = 2,
        W = 3,
        S = 4
    }



    public float moveSpeed = 1f;
    Vector2 rawMove, smoothMove;

    bool isInit = false;

    public int inputCode = 0; // 32-bit 操作码
    int lastInputCode = 0;
    public bool isInputCodeChange = false;

    public void Init()
    {
        rawMove = Vector2.zero;
        smoothMove = Vector2.zero;
        isInit = true;
    }


    public void SelfUpdate()
    {
        if(isInit == false) 
        {
            return;
        }

        inputCode = 0;


        rawMove = Vector2.zero;
        float val = moveSpeed * Time.deltaTime;
        if( Input.GetKey(KeyCode.W) )
        {
            rawMove.y = val;
            inputCode = KTool.SetBitMask( inputCode, (int)OpCodeBitPos.W, true );
        }
        else if( Input.GetKey(KeyCode.S) ) 
        {
            rawMove.y = -val;
            inputCode = KTool.SetBitMask( inputCode, (int)OpCodeBitPos.S, true );
        }

        if( Input.GetKey(KeyCode.A) )
        {
            rawMove.x = -val;
            inputCode = KTool.SetBitMask( inputCode, (int)OpCodeBitPos.A, true );
        }
        else if( Input.GetKey(KeyCode.D) ) 
        {
            rawMove.x = val;
            inputCode = KTool.SetBitMask( inputCode, (int)OpCodeBitPos.D, true );
        }
        //---
        smoothMove = Vector2.Lerp( smoothMove, rawMove, 0.3f );

        //---
        //isInputCodeChange = inputCode != lastInputCode;

        if( inputCode != lastInputCode ) 
        {
            isInputCodeChange = true;
        }

        lastInputCode = inputCode;

        

    }


    public Vector2 GetMove() 
    {
        return smoothMove;
    }




    public static Vector2 InputCode2RawMove( int inputCode, float moveSpeed_ ) 
    {
        Vector2 _rawMove = Vector2.zero;
        if( KTool.GetBitMask(inputCode, (int)OpCodeBitPos.W) ) 
        {
            _rawMove.y = 1f;
        }
        else if( KTool.GetBitMask(inputCode, (int)OpCodeBitPos.S) ) 
        {
            _rawMove.y = -1f;
        }   

        if( KTool.GetBitMask(inputCode, (int)OpCodeBitPos.D) ) 
        {
            _rawMove.x = 1f;
        }
        else if( KTool.GetBitMask(inputCode, (int)OpCodeBitPos.A) ) 
        {
            _rawMove.x = -1f;
        }
        _rawMove = _rawMove.normalized * moveSpeed_;
        return _rawMove;
    }


}

}
