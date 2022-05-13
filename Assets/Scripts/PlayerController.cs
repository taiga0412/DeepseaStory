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
    public float jumpHeight;    //最大ジャンプ高度（過ぎると自動で落下開始）
    public float gravity;       //重力加速度（というより落下速度？）
    public float jumpSpeed;     //ジャンプ時の上昇速度
    public float jumpLimitTime; //ジャンプ上昇の時間制限（過ぎると自動で落下開始）
    public GroundCheck ground;
    public GroundCheck head;
    public AnimationCurve dashCurve;    //ダッシュ速度曲線
    public AnimationCurve jumpCurve;    //ジャンプ速度曲線
    #endregion

    [SerializeField] private Animator animator;


    private bool IsJumping;         //ジャンプ上昇中かのフラグ
    private float dashTime = 0.0f;  //ダッシュの経過時間
    private float beforeKey = 0.0f; //前フレームの横方向入力量
    private float jumpTime = 0.0f;  //上方向の入力時間
    private float jumpPos = 0.0f;   //ジャンプ開始した際のy座標


    private Rigidbody2D _rb;
    private MobStatus _status;
    private MobAttack _attack;
    private Vector2 _moveVelocity;  //プレイヤーの移動速度情報

    // Start is called before the first frame update
    void Start()
    {
        _status = GetComponent<MobStatus>();
        _attack = GetComponent<MobAttack>();
        _rb = GetComponent<Rigidbody2D>();
        
    }

    // Update is called once per frame
    void Update()
    {
        //移動キー入力取得
        float horizontalKey = Input.GetAxisRaw("Horizontal");
        float verticalKey = Input.GetAxisRaw("Vertical");
        //地面との接触判定の取得
        bool isGround = ground.IsGround();
        bool isHead = head.IsGround();

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

        //X軸速度の処理
        if (_status.IsMovable)
        {
            //横入力に応じてX軸速度の計算
            _moveVelocity.x = CalcXSpeed(horizontalKey);
        }
        else
        {
            //移動不可の場合はx軸速度を0にする
            _moveVelocity.x = 0;
        }

        //Y軸速度の処理
        if (isGround)
        {
            //※地面にいる場合
            //速度を0に設定
            _moveVelocity.y = 0;

            //しゃがみ解除処理
            if (_status.IsSyagaming && verticalKey >= 0)
            {
                _status.GoToNormalStateIfPossible();
            }

            //ジャンプ・しゃがみ状態への遷移
            if (_status.IsMovable)
            {
                if (verticalKey > 0)
                {
                    //上入力でジャンプ上昇へ
                    _moveVelocity.y = jumpSpeed;    //上昇速度の付与
                    jumpPos = transform.position.y; //ジャンプ開始時の座標の記録
                    jumpTime = 0.0f;
                    IsJumping = true;
                }
                else if (verticalKey < 0)
                {
                    //しゃがみ状態へ遷移
                    _status.GoToSyagamiStateIfPossible();

                }
            }
        }
        else
        {
            //※空中にいる場合
            //上昇終了判定
            if (IsJumping)
            {
                bool pushUpKey = verticalKey > 0;
                bool canHeight = jumpPos + jumpHeight > transform.position.y;
                bool canTime = jumpLimitTime > jumpTime;
                if (!pushUpKey || !canHeight || !canTime || isHead)
                {
                    //キー入力・限界高度・入力時間・天井衝突のいずれかの条件により落下へと移行する
                    IsJumping = false;
                }
            }

            //y軸速度の計算
            _moveVelocity.y = CalcYSpeedInAir(IsJumping);
        }

        //Animatorに移動速度情報から得られるパラメータを適用する
        SetAnimatorParams(_moveVelocity, isGround);
        //移動速度を適用する
        _rb.velocity = _moveVelocity;
    }

    //Animatorのパラメータを設定する（速度など、物理関係の情報のみ）
    private void SetAnimatorParams(Vector2 velocity, bool isGround)
    {
        //接地と速度情報をAnimatorに伝える
        animator.SetFloat("xVelocity", velocity.x);
        animator.SetFloat("yVelocity", velocity.y);
        animator.SetBool("ground", isGround);
    }

    //x軸方向の速度を計算する
    private float CalcXSpeed(float horizontalKey)
    {
        float xSpeed;

        if (horizontalKey > 0)
        {
            //※左入力
            transform.localScale = new Vector3(1, 1, 1);
            dashTime += Time.deltaTime;
            xSpeed = speed;
        }
        else if (horizontalKey < 0)
        {
            //※右入力
            transform.localScale = new Vector3(-1, 1, 1);
            dashTime += Time.deltaTime;
            xSpeed = -speed;
        }
        else
        {
            //入力なしの場合、タイマーをリセット
            dashTime = 0.0f;
            xSpeed = 0.0f;
        }

        //振り向きの処理
        if (horizontalKey > 0 && beforeKey < 0)
        {
            dashTime = 0.0f;
        }
        else if (horizontalKey < 0 && beforeKey > 0)
        {
            dashTime = 0.0f;
        }

        //速度にダッシュ曲線を適用
        xSpeed *= dashCurve.Evaluate(dashTime);

        //入力量を記録しておく
        beforeKey = horizontalKey;

        return xSpeed;
    }

    //空中に居る際のy軸方向の速度を計算する
    private float CalcYSpeedInAir(bool IsJumping)
    {
        float ySpeed;
        if (IsJumping)
        {
            //ジャンプ上昇の速度（上入力中で、上昇可能な際）
            ySpeed = jumpSpeed;
            jumpTime += Time.deltaTime;
        }
        else
        {
            //落下中の速度
            ySpeed = -gravity;
        }

        //速度にJump曲線を適用
        ySpeed *= jumpCurve.Evaluate(jumpTime);

        return ySpeed;
    }
}
