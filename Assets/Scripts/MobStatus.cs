using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class MobStatus : MonoBehaviour
{
    //��Ԃ�\���񋓌^
    public enum StateEnum
    {
        Normal,     //�ʏ�
        Attack,     //�ʏ�U����
        FallAttack, //�����U����
        Die         //���S
    }

    //�ړ��\����Ԃ�
    public bool IsMovable => _state == StateEnum.Normal;

    //�U���\����Ԃ�
    public bool IsAttackable => _state == StateEnum.Normal;


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

}
