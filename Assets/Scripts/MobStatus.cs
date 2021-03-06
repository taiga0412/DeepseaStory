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

    //しゃがみ中かを返す
    public bool IsSyagaming => _state == StateEnum.Syagami;

    //攻撃可能かを返す
    public bool IsAttackable => _state == StateEnum.Normal;

    //被弾硬直中かを返す
    public bool IsDamaged => _state == StateEnum.Damaged;

    [SerializeField] private float _HPMax = 1.0f;
    public float HPMax => _HPMax;   //HPの最大値
    public float HP { get; private set; }   //現在のHP
    [SerializeField] private float FrozenTime;  //被弾後の硬直時間
    [SerializeField] private float DamagedMutekiTime;   //被弾から復帰した後の無敵時間
    public bool IsMuteki { get; private set; }  //無敵かどうかのフラグ



    private const float FlickDuration = 0.1f;   //点滅間隔

    private StateEnum _state;   //現在の状態
    private SpriteRenderer _spriteRenderer;
    protected Animator _animator;



    // Start is called before the first frame update
    protected virtual void Start()
    {
        _state = StateEnum.Normal;
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        HP = HPMax;
    }

    //可能なら通常状態へ遷移する
    public void GoToNormalStateIfPossible()
    {
        if (_state == StateEnum.Die) return;

        if (_state == StateEnum.Syagami) _animator.SetBool("syagami", false);
        _state = StateEnum.Normal;
    }

    //可能ならしゃがみ状態へ遷移する
    public void GoToSyagamiStateIfPossible()
    {
        if (!IsMovable) return;

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


    //可能なら被弾状態へ遷移する
    public void GoToDamagedStateIfPossible()
    {
        if (_state == StateEnum.Die) return;

        _state = StateEnum.Damaged;
        _animator.SetBool("damage", true);
        IsMuteki = true;
        StartCoroutine(Flicker());
        StartCoroutine(EndDamagedTime());
    }

    //ダメージを受ける処理
    public void Damage(float n = 1)
    {
        if (_state == StateEnum.Die || IsMuteki) return;

        //HPを減らす
        HP -= n;

        //生存判定
        if (HP > 0)
        {
            //被弾状態へ遷移する
            GoToDamagedStateIfPossible();
            return;
        }
        //死亡しているときの処理
        OnDie();
    }

    //死亡した際の処理
    public virtual void OnDie()
    {
        _animator.SetTrigger("die");
        _state = StateEnum.Die;
    }

    //ダメージ硬直の終了
    public IEnumerator EndDamagedTime()
    {
        //硬直時間分待機する
        yield return new WaitForSeconds(FrozenTime);
        //可能なら通常状態に遷移する
        _animator.SetBool("damage", false);
        GoToNormalStateIfPossible();
        //さらに無敵時間分待機する
        yield return new WaitForSeconds(DamagedMutekiTime);
        //無敵時間を終わらせる
        IsMuteki = false;
    }

    //無敵時間中はスプライトを点滅させる
    private IEnumerator Flicker()
    {
        //基本色
        Color baseColor = new Color(255, 255, 255, 255);
        float alpha_Sin = 255;
        while (true)
        {
            //透明度を計算する
            alpha_Sin = 255 - alpha_Sin;
            //透明度を設定する
            baseColor.a = alpha_Sin;
            _spriteRenderer.color = baseColor;

            if (!IsMuteki)
            {
                baseColor.a = 255;
                _spriteRenderer.color = baseColor;
                yield break;
            }
            yield return new WaitForSeconds(FlickDuration);
        }
    }
}
