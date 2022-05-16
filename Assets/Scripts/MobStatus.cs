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

    //‚µ‚á‚ª‚İ’†‚©‚ğ•Ô‚·
    public bool IsSyagaming => _state == StateEnum.Syagami;

    //UŒ‚‰Â”\‚©‚ğ•Ô‚·
    public bool IsAttackable => _state == StateEnum.Normal;

    //”í’ed’¼’†‚©‚ğ•Ô‚·
    public bool IsDamaged => _state == StateEnum.Damaged;

    [SerializeField] private float _HPMax = 1.0f;
    public float HPMax => _HPMax;   //HP‚ÌÅ‘å’l
    public float HP { get; private set; }   //Œ»İ‚ÌHP
    [SerializeField] private float FrozenTime;  //”í’eŒã‚Ìd’¼ŠÔ
    [SerializeField] private float DamagedMutekiTime;   //”í’e‚©‚ç•œ‹A‚µ‚½Œã‚Ì–³“GŠÔ
    public bool IsMuteki { get; private set; }  //–³“G‚©‚Ç‚¤‚©‚Ìƒtƒ‰ƒO



    private const float FlickDuration = 0.1f;   //“_–ÅŠÔŠu

    private StateEnum _state;   //Œ»İ‚Ìó‘Ô
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

    //‰Â”\‚È‚ç’Êíó‘Ô‚Ö‘JˆÚ‚·‚é
    public void GoToNormalStateIfPossible()
    {
        if (_state == StateEnum.Die) return;

        if (_state == StateEnum.Syagami) _animator.SetBool("syagami", false);
        _state = StateEnum.Normal;
    }

    //‰Â”\‚È‚ç‚µ‚á‚ª‚İó‘Ô‚Ö‘JˆÚ‚·‚é
    public void GoToSyagamiStateIfPossible()
    {
        if (!IsMovable) return;

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
        IsMuteki = true;
        StartCoroutine(Flicker());
        StartCoroutine(EndDamagedTime());
    }

    //ƒ_ƒ[ƒW‚ğó‚¯‚éˆ—
    public void Damage(float n = 1)
    {
        if (_state == StateEnum.Die || IsMuteki) return;

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
    public virtual void OnDie()
    {
        _animator.SetTrigger("die");
        _state = StateEnum.Die;
    }

    //ƒ_ƒ[ƒWd’¼‚ÌI—¹
    public IEnumerator EndDamagedTime()
    {
        //d’¼ŠÔ•ª‘Ò‹@‚·‚é
        yield return new WaitForSeconds(FrozenTime);
        //‰Â”\‚È‚ç’Êíó‘Ô‚É‘JˆÚ‚·‚é
        _animator.SetBool("damage", false);
        GoToNormalStateIfPossible();
        //‚³‚ç‚É–³“GŠÔ•ª‘Ò‹@‚·‚é
        yield return new WaitForSeconds(DamagedMutekiTime);
        //–³“GŠÔ‚ğI‚í‚ç‚¹‚é
        IsMuteki = false;
    }

    //–³“GŠÔ’†‚ÍƒXƒvƒ‰ƒCƒg‚ğ“_–Å‚³‚¹‚é
    private IEnumerator Flicker()
    {
        //Šî–{F
        Color baseColor = new Color(255, 255, 255, 255);
        float alpha_Sin = 255;
        while (true)
        {
            //“§–¾“x‚ğŒvZ‚·‚é
            alpha_Sin = 255 - alpha_Sin;
            //“§–¾“x‚ğİ’è‚·‚é
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
