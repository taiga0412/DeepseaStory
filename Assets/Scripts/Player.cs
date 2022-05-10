using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//旧Component。削除推奨。
public class Player : MonoBehaviour
{
    #region//インスペクターで設定する
    public float speed;
    public float jumpHeight;
    public float gravity;
    public float jumpSpeed;
    public float jumpLimitTime;
    public GroundCheck ground;
    public GroundCheck head;
    public AnimationCurve dashCurve;
    public AnimationCurve jumpCurve;
    #endregion

    #region//プライベート変数
    private Animator anim = null;
    private Rigidbody2D rb = null;
    private bool isHead = false;
    private bool isGround = false;
    private bool isJump = false;
    private bool isRun = false;
    private bool isSyagami = false;
    private bool isDamage = false;
    private bool isAttack = false;
    private bool isFall = false;
    private bool isOtherJump = false;
    private float jumpPos = 0.0f;
    private float otherJumpHeight = 0.0f;
    private float jumpTime = 0.0f;
    private float dashTime = 0.0f;
    private float beforeKey = 0.0f;
    private string enemyTag = "Enemy";
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

    }

    private void Update()
    {
        if (!isGround)
        {
            isFall = true;
        }
        else
        {
            isFall = false;
            anim.SetBool("fallattack2", isFall);
        }


        if (Input.GetKeyDown(KeyCode.C))
        {
            Attack();
        }

        if (isAttack && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            isAttack = false;
            
        }

        else if (Input.GetKeyDown(KeyCode.X) && isFall)
        {
            FallAttack();
        }
    }



    // Update is called once per frame
    void FixedUpdate()
    {



        if (!isDamage)
        {

            

            isGround = ground.IsGround();
            isHead = head.IsGround();

            


                  
            

                float xSpeed = Getxspeed();
                float ySpeed = Getyspeed();
                SetAnimation();
                rb.velocity = new Vector2(xSpeed, ySpeed);
                 
        }
        else
        {
            rb.velocity = new Vector2(0, -gravity);
        }
    }
    
    
    
    private float Getyspeed()
    {

        float verticalKey = Input.GetAxis("Vertical");
        float ySpeed = -gravity;

        if (isOtherJump)
        {
           
            bool canHeight = jumpPos + otherJumpHeight > transform.position.y;
            bool canTime = jumpLimitTime > jumpTime;

            if ( canHeight && canTime && !isHead)
            {
                ySpeed = jumpSpeed;
                jumpTime += Time.deltaTime;
            }
            else
            {
                isOtherJump = false;
                jumpTime = 0.0f;
            }
        }


        else if (isGround)
        {
           

            if (verticalKey > 0)
            {
                ySpeed = jumpSpeed;
                jumpPos = transform.position.y;
                isJump = true;
                jumpTime = 0.0f;
            }
            else if (verticalKey < 0)
            {
                isSyagami = true;
            }

            else
            {
                isJump = false;
                isSyagami = false;
            }
        }
        else if (isJump)
        {
            bool pushUpKey = verticalKey > 0;
            bool canHeight = jumpPos + jumpHeight > transform.position.y;
            bool canTime = jumpLimitTime > jumpTime;

            if (pushUpKey && canHeight && canTime && !isHead)
            {
                ySpeed = jumpSpeed;
                jumpTime += Time.deltaTime;
            }
            else
            {
                isJump = false;
                jumpTime = 0.0f;
            }
        }
        if (isJump || isOtherJump)
        {
            ySpeed *= jumpCurve.Evaluate(jumpTime);
        }

        return ySpeed;
    }

    private float Getxspeed()
    {
        float horizontalKey = Input.GetAxis("Horizontal");
        float xSpeed = 0.0f;
            
            if (isSyagami)
            {
                return 0.0f;
            }
        if (isAttack)
        {
            return 0.0f;
        }


            if (horizontalKey > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
                isRun = true;
                dashTime += Time.deltaTime;
                xSpeed = speed;
            }
            else if (horizontalKey < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
                isRun = true;
                dashTime += Time.deltaTime;
                xSpeed = -speed;
            }
            else
            {
                isRun = false;
                dashTime = 0.0f;
                xSpeed = 0.0f;
            }
            if (horizontalKey > 0 && beforeKey < 0)
            {
                dashTime = 0.0f;

            }
            else if (horizontalKey < 0 && beforeKey > 0)
            {
                dashTime = 0.0f;
            }
            xSpeed *= dashCurve.Evaluate(dashTime);

            return xSpeed; 
        
    }

    private void SetAnimation()
    {
        anim.SetBool("run", isRun);
        anim.SetBool("jump", isJump);
        anim.SetBool("ground", isGround);
        anim.SetBool("syagami", isSyagami);
       
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.tag == enemyTag)
        {
            anim.Play("PlayerDamage");
            isDamage = true;
        }
    }

    private void Attack()
    {

        isAttack = true;
        anim.SetTrigger("attack");
        
       
    }
    private void FallAttack()
    {
        anim.SetTrigger("fallattack");
        anim.SetBool("fallattack2", isFall);
    }

}
