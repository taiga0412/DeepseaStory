using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ƒ‚ƒu‚Ìó‘Ô‚ğŠÇ—‚·‚éComponent
[RequireComponent(typeof(Animator))]
public class MobStatus : MonoBehaviour
{
    //ó‘Ô‚ğ•\‚·—ñ‹“Œ^
    public enum StateEnum
    {
        Normal,     //’Êí
        Attack,     //’ÊíUŒ‚’†
        FallAttack, //—‰ºUŒ‚’†
        Syagami,    //‚µ‚á‚ª‚İó‘Ô
        Die         //€–S
    }

    //ˆÚ“®‰Â”\‚©‚ğ•Ô‚·
    public bool IsMovable => _state == StateEnum.Normal;

    //‚µ‚á‚ª‚İ‰Â”\‚©‚ğ•Ô‚·
    public bool IsSyagamiable => _state == StateEnum.Normal;

    //UŒ‚‰Â”\‚©‚ğ•Ô‚·
    public bool IsAttackable => _state == StateEnum.Normal;


    //TODOFHPŠÇ—‚Ì’Ç‰Á


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
        _animator.SetBool("syagami", false);
        _animator.SetBool("fallattack2", false);
    }

    //‰Â”\‚È‚ç‚µ‚á‚ª‚İó‘Ô‚Ö‘JˆÚ‚·‚é
    public void GoToSyagamiStateIfPossible()
    {
        if (!IsSyagamiable) return;

        _state = StateEnum.Syagami;
        _animator.SetBool("syagami", true);
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
        _animator.SetBool("fallattack2", true);
    }

}
