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
    public float speed;
    public float jumpHeight;
    public float gravity;
    public float jumpSpeed;
    public float jumpLimitTime;
    public GroundCheck ground;
    public GroundCheck head;
    public AnimationCurve dashCurve;
    public AnimationCurve jumpCurve;
    #endregion

    [SerializeField] private Animator animator;

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
        if (_status.IsMovable)
        {
            //TODO：移動処理
        }
        else
        {
            _moveVelocity.x = 0;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            //通常攻撃処理
            _attack.AttackIfPossible();
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            //落下攻撃処理
            _attack.FallAttackIfPossible();
        }

        //Y軸速度の処理
        if (ground.IsGround())  //TODO：IsGround()を変数へ
        {
            //TODO：ジャンプ処理
            //TODO：しゃがみ処理
            _moveVelocity.y = 0;
        }
        else
        {
            //TODO：落下処理
            _moveVelocity.y -= 9;
        }


        //TODO：Animatorに移動速度情報から得られるパラメータを適用する

        //移動速度を適用する
        _rb.velocity = _moveVelocity;
    }
}
