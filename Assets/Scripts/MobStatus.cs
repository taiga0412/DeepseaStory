using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//���u�̏�Ԃ��Ǘ�����Component
[RequireComponent(typeof(Animator))]
public class MobStatus : MonoBehaviour
{
    //��Ԃ�\���񋓌^
    public enum StateEnum
    {
        Normal,     //�ʏ�
        Attack,     //�ʏ�U����
        FallAttack, //�����U����
        Syagami,    //���Ⴊ�ݏ��
        Damaged,    //��e��ԁi���G�j
        Die         //���S
    }

    //�ړ��\����Ԃ�
    public bool IsMovable => _state == StateEnum.Normal;

    //���Ⴊ�ݒ�����Ԃ�
    public bool IsSyagaming => _state == StateEnum.Syagami;

    //�U���\����Ԃ�
    public bool IsAttackable => _state == StateEnum.Normal;

    //��e�d��������Ԃ�
    public bool IsDamaged => _state == StateEnum.Damaged;

    [SerializeField] private float _HPMax = 1.0f;
    public float HPMax => _HPMax;   //HP�̍ő�l
    public float HP { get; private set; }   //���݂�HP
    [SerializeField] private float FrozenTime;  //��e��̍d������
    [SerializeField] private float DamagedMutekiTime;   //��e���畜�A������̖��G����
    public bool IsMuteki { get; private set; }  //���G���ǂ����̃t���O



    private const float FlickDuration = 0.1f;   //�_�ŊԊu

    private StateEnum _state;   //���݂̏��
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

    //�\�Ȃ�ʏ��Ԃ֑J�ڂ���
    public void GoToNormalStateIfPossible()
    {
        if (_state == StateEnum.Die) return;

        if (_state == StateEnum.Syagami) _animator.SetBool("syagami", false);
        _state = StateEnum.Normal;
    }

    //�\�Ȃ炵�Ⴊ�ݏ�Ԃ֑J�ڂ���
    public void GoToSyagamiStateIfPossible()
    {
        if (!IsMovable) return;

        _state = StateEnum.Syagami;
        _animator.SetBool("syagami", true);
    }

    //�\�Ȃ�ʏ�U����Ԃ֑J�ڂ���
    public void GoToAttackStateIfPossible()
    {
        if (!IsAttackable) return;

        _state = StateEnum.Attack;
        _animator.SetTrigger("attack");
    }

    //�\�Ȃ痎���U����Ԃ֑J�ڂ���
    public void GoToFallAttackStateIfPossible()
    {
        if (!IsAttackable) return;

        _state = StateEnum.FallAttack;
        _animator.SetTrigger("fallattack");
    }


    //�\�Ȃ��e��Ԃ֑J�ڂ���
    public void GoToDamagedStateIfPossible()
    {
        if (_state == StateEnum.Die) return;

        _state = StateEnum.Damaged;
        _animator.SetBool("damage", true);
        IsMuteki = true;
        StartCoroutine(Flicker());
        StartCoroutine(EndDamagedTime());
    }

    //�_���[�W���󂯂鏈��
    public void Damage(float n = 1)
    {
        if (_state == StateEnum.Die || IsMuteki) return;

        //HP�����炷
        HP -= n;

        //��������
        if (HP > 0)
        {
            //��e��Ԃ֑J�ڂ���
            GoToDamagedStateIfPossible();
            return;
        }
        //���S���Ă���Ƃ��̏���
        OnDie();
    }

    //���S�����ۂ̏���
    public virtual void OnDie()
    {
        _animator.SetTrigger("die");
        _state = StateEnum.Die;
    }

    //�_���[�W�d���̏I��
    public IEnumerator EndDamagedTime()
    {
        //�d�����ԕ��ҋ@����
        yield return new WaitForSeconds(FrozenTime);
        //�\�Ȃ�ʏ��ԂɑJ�ڂ���
        _animator.SetBool("damage", false);
        GoToNormalStateIfPossible();
        //����ɖ��G���ԕ��ҋ@����
        yield return new WaitForSeconds(DamagedMutekiTime);
        //���G���Ԃ��I��点��
        IsMuteki = false;
    }

    //���G���Ԓ��̓X�v���C�g��_�ł�����
    private IEnumerator Flicker()
    {
        //��{�F
        Color baseColor = new Color(255, 255, 255, 255);
        float alpha_Sin = 255;
        while (true)
        {
            //�����x���v�Z����
            alpha_Sin = 255 - alpha_Sin;
            //�����x��ݒ肷��
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
