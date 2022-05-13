using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//モブの状態を管理するComponent
[RequireComponent(typeof(Animator))]
public class MobStatus : MonoBehaviour
{
    //状態を表す列挙型
    public enum StateEnum
    {
        Normal,     //通常
        Attack,     //通常攻撃中
        FallAttack, //落下攻撃中
        Syagami,    //しゃがみ状態
        Damaged,    //被弾状態（無敵）
        Die         //死亡
    }

    //移動可能かを返す
    public bool IsMovable => _state == StateEnum.Normal;

    //しゃがみ可能かを返す
    public bool IsSyagamiable => _state == StateEnum.Normal;

    //しゃがみ中かを返す
    public bool IsSyagaming => _state == StateEnum.Syagami;

    //攻撃可能かを返す
    public bool IsAttackable => _state == StateEnum.Normal;

    //攻撃中かを返す
    public bool IsAttacking => _state == StateEnum.Attack || _state == StateEnum.FallAttack;


    [SerializeField] private float _HPMax = 1.0f;
    public float HPMax => _HPMax;   //HPの最大値
    public float HP { get; private set; }   //現在のHP
    [SerializeField] private float DamagedTime; //被弾直後の無敵時間


    private StateEnum _state;   //現在の状態
    protected Animator _animator;



    // Start is called before the first frame update
    protected virtual void Start()
    {
        _state = StateEnum.Normal;
        _animator = GetComponent<Animator>();

        HP = HPMax;
    }

    //可能なら通常状態へ遷移する
    public void GoToNormalStateIfPossible()
    {
        if (_state == StateEnum.Die) return;

        _state = StateEnum.Normal;
        _animator.SetBool("syagami", false);
    }

    //可能ならしゃがみ状態へ遷移する
    public void GoToSyagamiStateIfPossible()
    {
        if (!IsSyagamiable) return;

        _state = StateEnum.Syagami;
        _animator.SetBool("syagami", true);
    }

    //可能なら通常攻撃状態へ遷移する
    public void GoToAttackStateIfPossible()
    {
        if (!IsAttackable) return;

        _state = StateEnum.Attack;
        _animator.SetTrigger("attack");
    }

    //可能なら落下攻撃状態へ遷移する
    public void GoToFallAttackStateIfPossible()
    {
        if (!IsAttackable) return;

        _state = StateEnum.FallAttack;
        _animator.SetTrigger("fallattack");
    }

    //ダメージを受ける処理
    public void Damage(float n = 1)
    {
        if (_state == StateEnum.Die) return;

        //HPを減らす
        HP -= n;

        //死亡判定
        if (HP > 0)
        {
            //生存しているなら、ダメージモーションへ遷移して戻る
            _animator.SetBool("damage", true);  //ダメージアニメーションの開始
            _state = StateEnum.Damaged; //被弾状態へ遷移
            StartCoroutine(EndDamagedTime());   //ダメージ終了処理の予約
            return;
        }
        //死亡しているときの処理
        _state = StateEnum.Die;
        OnDie();
    }

    //死亡した際の処理
    public void OnDie()
    {

    }

    //ダメージ硬直の終了
    public IEnumerator EndDamagedTime()
    {
        //無敵時間分待機する
        yield return new WaitForSeconds(DamagedTime);
        //可能なら通常状態に遷移する
        _animator.SetBool("damage", false);
        GoToNormalStateIfPossible();
    }
}
