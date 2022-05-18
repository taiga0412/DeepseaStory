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
    public float runForceAlpha; //�ړ����ɉ�����͂̌v�Z�Ŏg�p����W���i�傫�����͂������Ȃ�j
    public float runForceLimit; //�ړ����ɉ�����͂̑傫���̌��E�l

    public float jumpPower;     //�W�����v�́i��{�^������u�������ۂ̃W�����v�͂����܂�j
    public float jumpAirPower;  //�󒆃W�����v�́i��{�^���������������ۂ̒����͂̏オ�蕝�����܂�j
    public float jumpHeight;    //�ő�W�����v���x�i�߂���Ǝ����ŗ����J�n�j
    public float jumpLimitTime; //�W�����v�㏸�̎��Ԑ����i�߂���Ǝ����ŗ����J�n�j

    public float damagedAnimSpeed;  //��e���̉��ړ����x
    public float damagedAnimTime;   //��e���̉��ړ�����
    public GroundCheck ground;
    public GroundCheck head;
    #endregion

    [SerializeField] private Animator animator;
    private float gravity;          //�d�͉����x�irigidbody����擾����j


    private bool IsJumping;         //�W�����v�㏸�����̃t���O
    private float jumpTime = 0.0f;  //������̓��͎���
    private float jumpPos = 0.0f;   //�W�����v�J�n�����ۂ�y���W
    private float damagedPassedTime = 0.0f;   //��e���Ă���̌o�ߎ���


    private Rigidbody2D _rb;
    private MobStatus _status;
    private MobAttack _attack;
    private bool isGround;          //���̐ڒn������
    private float horizontalKey;    //���L�[���͏��
    private float verticalKey;      //�c�L�[���͏��

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
        //�ړ��L�[���͎擾
        horizontalKey = Input.GetAxisRaw("Horizontal");
        verticalKey = Input.GetAxisRaw("Vertical");

        //�f�o�b�O�p�B�{�^���������ƃ_���[�W���󂯂�
        if (Input.GetKeyDown(KeyCode.Z))
        {
            _status.Damage();
        }

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

        //���Ⴊ�݊֘A����
        if (isGround)
        {
            if (_status.IsSyagaming && verticalKey >= 0)
            {
                //���Ⴊ�݉���
                _status.GoToNormalStateIfPossible();
                _rb.gravityScale = gravity;
            }
            else if (_status.IsMovable && verticalKey < 0)
            {
                //���Ⴊ�ݏ�Ԃֈڍs
                _status.GoToSyagamiStateIfPossible();
                _rb.gravityScale = gravity * 10;
            }
        }else if (_status.IsSyagaming)
        {
            //���Ⴊ�񂾏�Ԃŋ󒆂ɂ���Ȃ炵�Ⴊ�ݏ�Ԃ�����
            _status.GoToNormalStateIfPossible();
        }

        //Animator�Ɉړ����x��񂩂瓾����p�����[�^��K�p����
        SetAnimatorParams(_rb.velocity, isGround);
    }

    //�͂��v�Z��������
    private void FixedUpdate()
    {
        //�n�\�Ƃ̐ڐG����̎擾
        isGround = ground.IsGround();
        bool isHead = head.IsGround();

        //X�����x�̏���
        if (_status.IsDamaged)
        {
            //��e��������͉��Ɉړ�����
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
                //�v���C���[�̌�����ς���
                if (horizontalKey > 0) transform.localScale = new Vector3(1, 1, 1);
                else if(horizontalKey < 0) transform.localScale = new Vector3(-1, 1, 1);
                //�����͂ɉ����ė͂�������
                float runForcePower = CalcForce(_rb.velocity.x, horizontalKey * speed, runForceLimit, -runForceLimit, runForceAlpha);
                _rb.AddForce(Vector2.right * runForcePower);
            }
        }

        //Y�����x�̏���
        if (isGround && !IsJumping)
        {
            //���n�ʂɂ���ꍇ
            //�W�����v�E���Ⴊ�ݏ�Ԃւ̑J��
            if (_status.IsMovable && verticalKey > 0)
            {
                //����͂ŃW�����v�㏸��
                _rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
                jumpPos = transform.position.y; //�W�����v�J�n���̍��W�̋L�^
                jumpTime = 0.0f;
                IsJumping = true;
            }
        }
        else if (IsJumping)
        {
            //���󒆂ŃW�����v���Ă���ꍇ
            //�㏸�I������
            bool pushUpKey = verticalKey > 0;
            bool canHeight = jumpPos + jumpHeight > transform.position.y;
            bool canTime = jumpLimitTime > jumpTime;
            if (!pushUpKey || !canHeight || !canTime || isHead)
            {
                //�L�[���́E���E���x�E���͎��ԁE�V��Փ˂̂����ꂩ�̏����ɂ�藎���ւƈڍs����
                IsJumping = false;
            }
            jumpTime += Time.deltaTime;

            //�W�����v���͏�����̗͂�����葱����
            _rb.AddForce(Vector2.up * jumpAirPower);
        }
    }

    //Animator�̃p�����[�^��ݒ肷��i���x�ȂǁA�����֌W�̏��̂݁j
    private void SetAnimatorParams(Vector2 velocity, bool isGround)
    {
        //�ڒn�Ƒ��x����Animator�ɓ`����
        animator.SetFloat("xVelocity", velocity.x);
        animator.SetFloat("yVelocity", velocity.y);
        animator.SetBool("ground", isGround);
    }

    //����̑��x�ɂ��悤�Ƃ���͂��v�Z����i���葬�x�ɋ߂���������͂��ɂ߂�j
    public float CalcForce(float nowVelocity, float limitVelocity, float maxForce = 0, float minForce = 0, float alpha = 1.0f)
    {
        float force = alpha * (limitVelocity - nowVelocity);
        if (force > maxForce) force = maxForce;
        else if (force < minForce) force = minForce;
        return force;
    }
}
