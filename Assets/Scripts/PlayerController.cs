using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//プレイヤー操作を管理するComponent
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(MobStatus))]
[RequireComponent (typeof(MobAttack))]
public class PlayerController : MonoBehaviour
{
    #region//インスペクターで設定する
    public float speed;         //移動速度
    public float runForceAlpha; //移動時に加える力の計算で使用する係数（大きい程力が強くなる）
    public float runForceLimit; //移動時に加える力の大きさの限界値

    public float jumpPower;     //ジャンプ力（上ボタンを一瞬押した際のジャンプ力が決まる）
    public float jumpAirPower;  //空中ジャンプ力（上ボタンを押し続けた際の跳躍力の上がり幅が決まる）
    public float jumpHeight;    //最大ジャンプ高度（過ぎると自動で落下開始）
    public float jumpLimitTime; //ジャンプ上昇の時間制限（過ぎると自動で落下開始）

    public float damagedAnimSpeed;  //被弾時の横移動速度
    public float damagedAnimTime;   //被弾時の横移動時間
    public GroundCheck ground;
    public GroundCheck head;
    #endregion

    [SerializeField] private Animator animator;
    private float gravity;          //重力加速度（rigidbodyから取得する）


    private bool IsJumping;         //ジャンプ上昇中かのフラグ
    private float jumpTime = 0.0f;  //上方向の入力時間
    private float jumpPos = 0.0f;   //ジャンプ開始した際のy座標
    private float damagedPassedTime = 0.0f;   //被弾してからの経過時間


    private Rigidbody2D _rb;
    private MobStatus _status;
    private MobAttack _attack;
    private bool isGround;          //床の接地判定情報
    private float horizontalKey;    //横キー入力情報
    private float verticalKey;      //縦キー入力情報

    // Start is called before the first frame update
    void Start()
    {
        _status = GetComponent<MobStatus>();
        _attack = GetComponent<MobAttack>();
        _rb = GetComponent<Rigidbody2D>();
        gravity = _rb.gravityScale;
        
    }

    // Update is called once per frame
    void Update()
    {
        //移動キー入力取得
        horizontalKey = Input.GetAxisRaw("Horizontal");
        verticalKey = Input.GetAxisRaw("Vertical");

        //デバッグ用。ボタンを押すとダメージを受ける
        if (Input.GetKeyDown(KeyCode.Z))
        {
            _status.Damage();
        }

        //攻撃処理
        if (Input.GetKeyDown(KeyCode.C))
        {
            //通常攻撃処理へ
            _attack.AttackIfPossible();
        }
        else if (Input.GetKeyDown(KeyCode.X) && !isGround)
        {
            //落下攻撃処理へ
            _attack.FallAttackIfPossible();
        }

        //しゃがみ関連処理
        if (isGround)
        {
            if (_status.IsSyagaming && verticalKey >= 0)
            {
                //しゃがみ解除
                _status.GoToNormalStateIfPossible();
                _rb.gravityScale = gravity;
            }
            else if (_status.IsMovable && verticalKey < 0)
            {
                //しゃがみ状態へ移行
                _status.GoToSyagamiStateIfPossible();
                _rb.gravityScale = gravity * 10;
            }
        }else if (_status.IsSyagaming)
        {
            //しゃがんだ状態で空中にいるならしゃがみ状態を解除
            _status.GoToNormalStateIfPossible();
        }

        //Animatorに移動速度情報から得られるパラメータを適用する
        SetAnimatorParams(_rb.velocity, isGround);
    }

    //力を計算し加える
    private void FixedUpdate()
    {
        //地表との接触判定の取得
        isGround = ground.IsGround();
        bool isHead = head.IsGround();

        //X軸速度の処理
        if (_status.IsDamaged)
        {
            //被弾した直後は横に移動する
            damagedPassedTime += Time.deltaTime;
            if (damagedPassedTime <= damagedAnimTime)
            {
                _rb.AddForce(Vector2.left * transform.localScale.x * 100);
            }
        }
        else
        {
            damagedPassedTime = 0;
            if (_status.IsMovable)
            {
                //プレイヤーの向きを変える
                if (horizontalKey > 0) transform.localScale = new Vector3(1, 1, 1);
                else if(horizontalKey < 0) transform.localScale = new Vector3(-1, 1, 1);
                //横入力に応じて力を加える
                float runForcePower = CalcForce(_rb.velocity.x, horizontalKey * speed, runForceLimit, -runForceLimit, runForceAlpha);
                _rb.AddForce(Vector2.right * runForcePower);
            }
        }

        //Y軸速度の処理
        if (isGround && !IsJumping)
        {
            //※地面にいる場合
            //ジャンプ・しゃがみ状態への遷移
            if (_status.IsMovable && verticalKey > 0)
            {
                //上入力でジャンプ上昇へ
                _rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
                jumpPos = transform.position.y; //ジャンプ開始時の座標の記録
                jumpTime = 0.0f;
                IsJumping = true;
            }
        }
        else if (IsJumping)
        {
            //※空中でジャンプしている場合
            //上昇終了判定
            bool pushUpKey = verticalKey > 0;
            bool canHeight = jumpPos + jumpHeight > transform.position.y;
            bool canTime = jumpLimitTime > jumpTime;
            if (!pushUpKey || !canHeight || !canTime || isHead)
            {
                //キー入力・限界高度・入力時間・天井衝突のいずれかの条件により落下へと移行する
                IsJumping = false;
            }
            jumpTime += Time.deltaTime;

            //ジャンプ中は上方向の力が加わり続ける
            _rb.AddForce(Vector2.up * jumpAirPower);
        }
    }

    //Animatorのパラメータを設定する（速度など、物理関係の情報のみ）
    private void SetAnimatorParams(Vector2 velocity, bool isGround)
    {
        //接地と速度情報をAnimatorに伝える
        animator.SetFloat("xVelocity", velocity.x);
        animator.SetFloat("yVelocity", velocity.y);
        animator.SetBool("ground", isGround);
    }

    //既定の速度にしようとする力を計算する（既定速度に近い程加える力を緩める）
    public float CalcForce(float nowVelocity, float limitVelocity, float maxForce = 0, float minForce = 0, float alpha = 1.0f)
    {
        float force = alpha * (limitVelocity - nowVelocity);
        if (force > maxForce) force = maxForce;
        else if (force < minForce) force = minForce;
        return force;
    }
}
