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
        Die         //���S
    }

    //�ړ��\����Ԃ�
    public bool IsMovable => _state == StateEnum.Normal;

    //���Ⴊ�݉\����Ԃ�
    public bool IsSyagamiable => _state == StateEnum.Normal;

    //�U���\����Ԃ�
    public bool IsAttackable => _state == StateEnum.Normal;


    //TODO�FHP�Ǘ��̒ǉ�


    private StateEnum _state;
    protected Animator _animator;



    // Start is called before the first frame update
    protected virtual void Start()
    {
        _state = StateEnum.Normal;
        _animator = GetComponent<Animator>();
        
    }

    //�\�Ȃ�ʏ��Ԃ֑J�ڂ���
    public void GoToNormalStateIfPossible()
    {
        if (!IsMovable) return;

        _state = StateEnum.Normal;
        _animator.SetBool("syagami", false);
        _animator.SetBool("fallattack2", false);
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
        _animator.SetBool("fallattack2", true);
    }

}
