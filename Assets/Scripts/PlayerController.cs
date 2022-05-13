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
    public float speed;         //�ړ����x
    public float jumpHeight;    //�ő�W�����v���x�i�߂���Ǝ����ŗ����J�n�j
    public float gravity;       //�d�͉����x�i�Ƃ�����藎�����x�H�j
    public float jumpSpeed;     //�W�����v���̏㏸���x
    public float jumpLimitTime; //�W�����v�㏸�̎��Ԑ����i�߂���Ǝ����ŗ����J�n�j
    public GroundCheck ground;
    public GroundCheck head;
    public AnimationCurve dashCurve;    //�_�b�V�����x�Ȑ�
    public AnimationCurve jumpCurve;    //�W�����v���x�Ȑ�
    #endregion

    [SerializeField] private Animator animator;


    private bool IsJumping;         //�W�����v�㏸�����̃t���O
    private float dashTime = 0.0f;  //�_�b�V���̌o�ߎ���
    private float beforeKey = 0.0f; //�O�t���[���̉��������͗�
    private float jumpTime = 0.0f;  //������̓��͎���
    private float jumpPos = 0.0f;   //�W�����v�J�n�����ۂ�y���W


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
        //�ړ��L�[���͎擾
        float horizontalKey = Input.GetAxisRaw("Horizontal");
        float verticalKey = Input.GetAxisRaw("Vertical");
        //�n�ʂƂ̐ڐG����̎擾
        bool isGround = ground.IsGround();
        bool isHead = head.IsGround();

        //�U������
        if (Input.GetKeyDown(KeyCode.C))
        {
            //�ʏ�U��������
            _attack.AttackIfPossible();
        }
        else if (Input.GetKeyDown(KeyCode.X) && !isGround)
        {
            //�����U��������
            _attack.FallAttackIfPossible();
        }

        //X�����x�̏���
        if (_status.IsMovable)
        {
            //�����͂ɉ�����X�����x�̌v�Z
            _moveVelocity.x = CalcXSpeed(horizontalKey);
        }
        else
        {
            //�ړ��s�̏ꍇ��x�����x��0�ɂ���
            _moveVelocity.x = 0;
        }

        //Y�����x�̏���
        if (isGround)
        {
            //���n�ʂɂ���ꍇ
            //���x��0�ɐݒ�
            _moveVelocity.y = 0;

            //���Ⴊ�݉�������
            if (_status.IsSyagaming && verticalKey >= 0)
            {
                _status.GoToNormalStateIfPossible();
            }

            //�W�����v�E���Ⴊ�ݏ�Ԃւ̑J��
            if (_status.IsMovable)
            {
                if (verticalKey > 0)
                {
                    //����͂ŃW�����v�㏸��
                    _moveVelocity.y = jumpSpeed;    //�㏸���x�̕t�^
                    jumpPos = transform.position.y; //�W�����v�J�n���̍��W�̋L�^
                    jumpTime = 0.0f;
                    IsJumping = true;
                }
                else if (verticalKey < 0)
                {
                    //���Ⴊ�ݏ�Ԃ֑J��
                    _status.GoToSyagamiStateIfPossible();

                }
            }
        }
        else
        {
            //���󒆂ɂ���ꍇ
            //�㏸�I������
            if (IsJumping)
            {
                bool pushUpKey = verticalKey > 0;
                bool canHeight = jumpPos + jumpHeight > transform.position.y;
                bool canTime = jumpLimitTime > jumpTime;
                if (!pushUpKey || !canHeight || !canTime || isHead)
                {
                    //�L�[���́E���E���x�E���͎��ԁE�V��Փ˂̂����ꂩ�̏����ɂ�藎���ւƈڍs����
                    IsJumping = false;
                }
            }

            //y�����x�̌v�Z
            _moveVelocity.y = CalcYSpeedInAir(IsJumping);
        }

        //Animator�Ɉړ����x��񂩂瓾����p�����[�^��K�p����
        SetAnimatorParams(_moveVelocity, isGround);
        //�ړ����x��K�p����
        _rb.velocity = _moveVelocity;
    }

    //Animator�̃p�����[�^��ݒ肷��i���x�ȂǁA�����֌W�̏��̂݁j
    private void SetAnimatorParams(Vector2 velocity, bool isGround)
    {
        //�ڒn�Ƒ��x����Animator�ɓ`����
        animator.SetFloat("xVelocity", velocity.x);
        animator.SetFloat("yVelocity", velocity.y);
        animator.SetBool("ground", isGround);
    }

    //x�������̑��x���v�Z����
    private float CalcXSpeed(float horizontalKey)
    {
        float xSpeed;

        if (horizontalKey > 0)
        {
            //��������
            transform.localScale = new Vector3(1, 1, 1);
            dashTime += Time.deltaTime;
            xSpeed = speed;
        }
        else if (horizontalKey < 0)
        {
            //���E����
            transform.localScale = new Vector3(-1, 1, 1);
            dashTime += Time.deltaTime;
            xSpeed = -speed;
        }
        else
        {
            //���͂Ȃ��̏ꍇ�A�^�C�}�[�����Z�b�g
            dashTime = 0.0f;
            xSpeed = 0.0f;
        }

        //�U������̏���
        if (horizontalKey > 0 && beforeKey < 0)
        {
            dashTime = 0.0f;
        }
        else if (horizontalKey < 0 && beforeKey > 0)
        {
            dashTime = 0.0f;
        }

        //���x�Ƀ_�b�V���Ȑ���K�p
        xSpeed *= dashCurve.Evaluate(dashTime);

        //���͗ʂ��L�^���Ă���
        beforeKey = horizontalKey;

        return xSpeed;
    }

    //�󒆂ɋ���ۂ�y�������̑��x���v�Z����
    private float CalcYSpeedInAir(bool IsJumping)
    {
        float ySpeed;
        if (IsJumping)
        {
            //�W�����v�㏸�̑��x�i����͒��ŁA�㏸�\�ȍہj
            ySpeed = jumpSpeed;
            jumpTime += Time.deltaTime;
        }
        else
        {
            //�������̑��x
            ySpeed = -gravity;
        }

        //���x��Jump�Ȑ���K�p
        ySpeed *= jumpCurve.Evaluate(jumpTime);

        return ySpeed;
    }
}
