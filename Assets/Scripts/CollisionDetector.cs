using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

//衝突管理Component。衝突が起こった際に、インスペクタで指定したメソッド（引数はCollider2D）を呼び出す。
[RequireComponent(typeof(Collider2D))]
public class CollisionDetector : MonoBehaviour
{
    [SerializeField] private TriggerEvent onTriggerEnter = new TriggerEvent();
    [SerializeField] private TriggerEvent onTriggerStay = new TriggerEvent();

    private void OnTriggerEnter2D(Collider2D other)
    {
        onTriggerEnter.Invoke(other);
    }

    //Is TriggerがONで他のColliderと重なっているときは、このメソッドが常にコールされる
    private void OnTriggerStay2D(Collider2D other)
    {
        //onTriggerStayで指定された処理を実行する
        onTriggerStay.Invoke(other);
    }

    //UnityEventを継承したクラスに[Serialize]属性を付与することで、
    //Inspectorウィンドウ上に表示できるようになる
    [Serializable]
    public class TriggerEvent : UnityEvent<Collider2D>
    {
    }
}
