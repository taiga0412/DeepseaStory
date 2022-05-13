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

    //���Ⴊ�݉\����Ԃ�
    public bool IsSyagamiable => _state == StateEnum.Normal;

    //���Ⴊ�ݒ�����Ԃ�
    public bool IsSyagaming => _state == StateEnum.Syagami;

    //�U���\����Ԃ�
    public bool IsAttackable => _state == StateEnum.Normal;

    //�U��������Ԃ�
    public bool IsAttacking => _state == StateEnum.Attack || _state == StateEnum.FallAttack;


    [SerializeField] private float _HPMax = 1.0f;
    public float HPMax => _HPMax;   //HP�̍ő�l
    public float HP { get; private set; }   //���݂�HP
    [SerializeField] private float DamagedTime; //��e����̖��G����


    private StateEnum _state;   //���݂̏��
    protected Animator _animator;



    // Start is called before the first frame update
    protected virtual void Start()
    {
        _state = StateEnum.Normal;
        _animator = GetComponent<Animator>();

        HP = HPMax;
    }

    //�\�Ȃ�ʏ��Ԃ֑J�ڂ���
    public void GoToNormalStateIfPossible()
    {
        if (_state == StateEnum.Die) return;

        _state = StateEnum.Normal;
        _animator.SetBool("syagami", false);
    }

    //�\�Ȃ炵�Ⴊ�ݏ�Ԃ֑J�ڂ���
    public void GoToSyagamiStateIfPossible()
    {
        if (!IsSyagamiable) return;

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
        StartCoroutine(EndDamagedTime());
    }

    //�_���[�W���󂯂鏈��
    public void Damage(float n = 1)
    {
        if (_state == StateEnum.Die) return;

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
    public void OnDie()
    {
        _animator.SetTrigger("die");
        _state = StateEnum.Die;
    }

    //�_���[�W�d���̏I��
    public IEnumerator EndDamagedTime()
    {
        //���G���ԕ��ҋ@����
        yield return new WaitForSeconds(DamagedTime);
        //�\�Ȃ�ʏ��ԂɑJ�ڂ���
        _animator.SetBool("damage", false);
        GoToNormalStateIfPossible();
    }
}
