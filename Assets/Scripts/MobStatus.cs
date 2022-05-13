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
        Damaged,    //”í’eó‘Ôi–³“Gj
        Die         //€–S
    }

    //ˆÚ“®‰Â”\‚©‚ğ•Ô‚·
    public bool IsMovable => _state == StateEnum.Normal;

    //‚µ‚á‚ª‚İ‰Â”\‚©‚ğ•Ô‚·
    public bool IsSyagamiable => _state == StateEnum.Normal;

    //‚µ‚á‚ª‚İ’†‚©‚ğ•Ô‚·
    public bool IsSyagaming => _state == StateEnum.Syagami;

    //UŒ‚‰Â”\‚©‚ğ•Ô‚·
    public bool IsAttackable => _state == StateEnum.Normal;

    //UŒ‚’†‚©‚ğ•Ô‚·
    public bool IsAttacking => _state == StateEnum.Attack || _state == StateEnum.FallAttack;


    [SerializeField] private float _HPMax = 1.0f;
    public float HPMax => _HPMax;   //HP‚ÌÅ‘å’l
    public float HP { get; private set; }   //Œ»İ‚ÌHP
    [SerializeField] private float DamagedTime; //”í’e’¼Œã‚Ì–³“GŠÔ


    private StateEnum _state;   //Œ»İ‚Ìó‘Ô
    protected Animator _animator;



    // Start is called before the first frame update
    protected virtual void Start()
    {
        _state = StateEnum.Normal;
        _animator = GetComponent<Animator>();

        HP = HPMax;
    }

    //‰Â”\‚È‚ç’Êíó‘Ô‚Ö‘JˆÚ‚·‚é
    public void GoToNormalStateIfPossible()
    {
        if (_state == StateEnum.Die) return;

        _state = StateEnum.Normal;
        _animator.SetBool("syagami", false);
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
    }


    //‰Â”\‚È‚ç”í’eó‘Ô‚Ö‘JˆÚ‚·‚é
    public void GoToDamagedStateIfPossible()
    {
        if (_state == StateEnum.Die) return;

        _state = StateEnum.Damaged;
        _animator.SetBool("damage", true);
        StartCoroutine(EndDamagedTime());
    }

    //ƒ_ƒ[ƒW‚ğó‚¯‚éˆ—
    public void Damage(float n = 1)
    {
        if (_state == StateEnum.Die) return;

        //HP‚ğŒ¸‚ç‚·
        HP -= n;

        //¶‘¶”»’è
        if (HP > 0)
        {
            //”í’eó‘Ô‚Ö‘JˆÚ‚·‚é
            GoToDamagedStateIfPossible();
            return;
        }
        //€–S‚µ‚Ä‚¢‚é‚Æ‚«‚Ìˆ—
        OnDie();
    }

    //€–S‚µ‚½Û‚Ìˆ—
    public void OnDie()
    {
        _animator.SetTrigger("die");
        _state = StateEnum.Die;
    }

    //ƒ_ƒ[ƒWd’¼‚ÌI—¹
    public IEnumerator EndDamagedTime()
    {
        //–³“GŠÔ•ª‘Ò‹@‚·‚é
        yield return new WaitForSeconds(DamagedTime);
        //‰Â”\‚È‚ç’Êíó‘Ô‚É‘JˆÚ‚·‚é
        _animator.SetBool("damage", false);
        GoToNormalStateIfPossible();
    }
}
