using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class MobStatus : MonoBehaviour
{
    //ó‘Ô‚ğ•\‚·—ñ‹“Œ^
    public enum StateEnum
    {
        Normal,     //’Êí
        Attack,     //’ÊíUŒ‚’†
        FallAttack, //—‰ºUŒ‚’†
        Die         //€–S
    }

    //ˆÚ“®‰Â”\‚©‚ğ•Ô‚·
    public bool IsMovable => _state == StateEnum.Normal;

    //UŒ‚‰Â”\‚©‚ğ•Ô‚·
    public bool IsAttackable => _state == StateEnum.Normal;


    private StateEnum _state;
    protected Animator _animator;



    // Start is called before the first frame update
    protected virtual void Start()
    {
        _state = StateEnum.Normal;
        _animator = GetComponent<Animator>();
        
    }

    //‰Â”\‚È‚ç’Êíó‘Ô‚Ö‘JˆÚ‚·‚é
    public void GoToNormalStateIfPossible()
    {
        if (!IsMovable) return;

        _state = StateEnum.Normal;
    }

    //‰Â”\‚È‚ç’ÊíUŒ‚ó‘Ô‚Ö‘JˆÚ‚·‚é
    public void GoToAttackStateIfPossible()
    {
        if (!IsAttackable) return;

        _state = StateEnum.Attack;
        _animator.SetTrigger("attack");
    }

    //‰Â”\‚È‚ç—‰ºUŒ‚ó‘Ô‚Ö‘JˆÚ‚·‚é
    public void GoToFallAttackStateIfPossible()
    {
        if (!IsAttackable) return;

        _state = StateEnum.FallAttack;
        _animator.SetTrigger("fallattack");
    }

}
