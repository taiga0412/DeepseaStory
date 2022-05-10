using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�v���C���[������Ǘ�����Component
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(MobStatus))]
[RequireComponent (typeof(MobAttack))]
public class PlayerController : MonoBehaviour
{
    #region//�C���X�y�N�^�[�Őݒ肷��
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
    private Vector2 _moveVelocity;  //�v���C���[�̈ړ����x���

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
            //TODO�F�ړ�����
        }
        else
        {
            _moveVelocity.x = 0;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            //�ʏ�U������
            _attack.AttackIfPossible();
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            //�����U������
            _attack.FallAttackIfPossible();
        }

        //Y�����x�̏���
        if (ground.IsGround())  //TODO�FIsGround()��ϐ���
        {
            //TODO�F�W�����v����
            //TODO�F���Ⴊ�ݏ���
            _moveVelocity.y = 0;
        }
        else
        {
            //TODO�F��������
            _moveVelocity.y -= 9;
        }


        //TODO�FAnimator�Ɉړ����x��񂩂瓾����p�����[�^��K�p����

        //�ړ����x��K�p����
        _rb.velocity = _moveVelocity;
    }
}
