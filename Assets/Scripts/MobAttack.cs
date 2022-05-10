using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//���u�̍U�����Ǘ�����R���|�[�l���g
[RequireComponent(typeof(MobStatus))]
public class MobAttack : MonoBehaviour
{

    private MobStatus _status;

    private void Start()
    {
        _status = GetComponent<MobStatus>();

    }


    //�\�Ȃ�ʏ�U���Ɉڂ�
    public void AttackIfPossible()
    {
        if (!_status.IsAttackable) return;

        _status.GoToAttackStateIfPossible();
    }

    //�\�Ȃ痎���U���Ɉڂ�
    public void FallAttackIfPossible()
    {
        if (!_status.IsAttackable) return;

        _status.GoToFallAttackStateIfPossible();
    }


    //TODO�F�U�������������ۂ̏���


    //�U�����I�������ۂɌĂ΂��
    public void AttackFinished()
    {
        //�\�Ȃ�ʏ��Ԃ֑J�ڂ���
        _status.GoToNormalStateIfPossible();
    }
}
