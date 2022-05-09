using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(MobStatus))]
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

    private Rigidbody2D _rb;
    private MobStatus _status;
    private Vector2 _moveVelocity;

    // Start is called before the first frame update
    void Start()
    {
        _status = GetComponent<MobStatus>();
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
            //TODO：通常攻撃処理
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            //TODO：落下攻撃処理
        }

        //Y軸の処理
        if (ground.IsGround())  //TODO：IsGround()を変数へ
        {
            //TODO：ジャンプ処理
        }

        _rb.velocity = _moveVelocity;
    }
}
