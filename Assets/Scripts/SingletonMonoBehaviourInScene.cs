using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//シーン上に一つしか存在しないオブジェクトの抽象クラス。
//継承すると、クラス名.Instanceで参照可能になる。
public abstract class SingletonMonoBehaviorInScene<T> : MonoBehaviour where T :MonoBehaviour
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        if (Instance != null)
        {
            //シーンに2つ以上インスタンスが存在する場合は、エラーを出す
            throw new Exception("シーン内に他のインスタンスが存在します！");
        }

        Instance = this as T;
    }
}
