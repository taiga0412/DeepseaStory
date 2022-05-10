using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//モブの攻撃を管理するコンポーネント
[RequireComponent(typeof(MobStatus))]
public class MobAttack : MonoBehaviour
{

    private MobStatus _status;

    private void Start()
    {
        _status = GetComponent<MobStatus>();

    }


    //可能なら通常攻撃に移る
    public void AttackIfPossible()
    {
        if (!_status.IsAttackable) return;

        _status.GoToAttackStateIfPossible();
    }

    //可能なら落下攻撃に移る
    public void FallAttackIfPossible()
    {
        if (!_status.IsAttackable) return;

        _status.GoToFallAttackStateIfPossible();
    }


    //TODO：攻撃が当たった際の処理


    //攻撃が終了した際に呼ばれる
    public void AttackFinished()
    {
        //可能なら通常状態へ遷移する
        _status.GoToNormalStateIfPossible();
    }
}
