using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 設定速度に瞬間的に加速する
/// </summary>
public class TransformMove : IMove
{
    private Transform _transform;
    private Params _params;

    public  TransformMove( Params moveParams , Transform tr)
    {
        _transform = tr;
        _params = moveParams;
    }

    public void  Move(Vector2 moveDirection)
    {
        if (moveDirection == Vector2.zero) return;
        _transform.position +=  new  Vector3(moveDirection.x, 0, moveDirection.y).normalized * _params.MoveSpeed * Time.deltaTime;
    }

    public void Stop()
    {
    }

    [Serializable]
    public class Params
    {
        public float MoveSpeed = 5f;
    }
}
