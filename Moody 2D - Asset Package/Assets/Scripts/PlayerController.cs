using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    public float Speed = 50;

    bool isFacingLeft;
    Animator anim;
    TrailRenderer tr;

    bool canDash, isDashing;
    float dashDir;
    public float DashForce;

    #region Jump
    public float JumpForce;
    public LayerMask WhatIsGround;
    public float JumpRadius;
    bool IsGrounded;
    

    public Transform groundCheckPos;
    public int jumpAmount;
    int jumpCounter;
    #endregion

    #region dash
    public float waitTimeDash;
    public int DashAmnt;
    int dashCounter;
    #endregion

    float xInput;

    #region Wall Jump

    bool canGrab;
    bool isGrabbing;
    public float WallJumpRadius;
    public Transform WallJumpCheckPos;

    float initalGravityScale;
    public float WallJumpGravity;

    public float wallJumpForceX, wallJumpForceY;

    //Timer
    public float startWallJumpTimer;
    float wallJumpTimer;

    float clampedScaleAlt;
    float scaleX;
    #endregion

    public GameObject DeathEffect;

    public Collectables pauseScript;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        tr = GetComponent<TrailRenderer>();
        canDash = true;
        tr.emitting = false;
        dashCounter = DashAmnt;

        initalGravityScale = rb.gravityScale;
    }

    void Update()
    {
        #region Movement
        if(!pauseScript.paused)
        {
            if (wallJumpTimer <= 0)
            {
                if (!isDashing)
                {
                    xInput = Input.GetAxisRaw("Horizontal");
                    rb.velocity = new Vector2(xInput * Speed * Time.deltaTime, rb.velocity.y);
                }
            }
            else
            {
                wallJumpTimer -= Time.deltaTime;
            }


            if (xInput > 0 && isFacingLeft == true)
            {
                Flip();
            }
            else if (xInput < 0 && isFacingLeft == false)
            {
                Flip();
            }
        }

        #endregion

        #region Jump
        if (!pauseScript.paused)
        {
            IsGrounded = Physics2D.OverlapCircle(groundCheckPos.position, JumpRadius, WhatIsGround);

            if (IsGrounded)
            {
                jumpCounter = jumpAmount;
                dashCounter = DashAmnt;

            }

            if (Input.GetKeyDown(KeyCode.Space) && jumpCounter > 0)
            {
                rb.velocity = Vector2.up * JumpForce;
                if (!IsGrounded)
                {
                    jumpCounter--;
                }
            }
        }

        #endregion

        #region Animations

        if(xInput != 0)
        {
            anim.SetBool("isWalking", true);
        }
        else
        {
            anim.SetBool("isWalking", false);
        }

        anim.SetBool("hasJumped", !IsGrounded);



        #endregion

        #region dash
        if (!pauseScript.paused)
        {
            if (Input.GetKeyDown(KeyCode.Q) && canDash && dashCounter > 0)
            {
                // dash
                dashDir = Input.GetAxisRaw("Horizontal");
                dashCounter--;
                isDashing = true;
                canDash = false;
                rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
                StartCoroutine(stopDash());
            }

            if (isDashing)
            {

                if (dashDir == 0)
                {
                    dashDir = transform.localScale.x;
                }
                rb.velocity = new Vector2(dashDir * DashForce, rb.velocity.y) * Time.deltaTime;
                tr.emitting = true;

            }
        }
        #endregion

        #region wallJump
        if (!pauseScript.paused)
        {
            canGrab = Physics2D.OverlapCircle(WallJumpCheckPos.position, WallJumpRadius, WhatIsGround);


            isGrabbing = false;
            if (!IsGrounded && canGrab)
            {
                scaleX = transform.localScale.x;
                if ((scaleX > 0 && xInput > 0) || (scaleX < 0 && xInput < 0))
                {
                    isGrabbing = true;
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        wallJumpTimer = startWallJumpTimer;
                        rb.velocity = new Vector2(-xInput * wallJumpForceX * Time.deltaTime, wallJumpForceY * Time.deltaTime);
                        rb.gravityScale = initalGravityScale;
                        isGrabbing = false;

                    }

                }
            }

            if (isGrabbing)
            {
                rb.gravityScale = WallJumpGravity;
                rb.velocity = Vector2.zero;

            }
            else
            {
                rb.gravityScale = initalGravityScale;
            }
        }

        #endregion


       

    } 

    void Flip()
    {
        isFacingLeft = !isFacingLeft;
        transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
    }


    IEnumerator stopDash()
    {
        yield return new WaitForSeconds(waitTimeDash);
        rb.constraints = ~RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
        canDash = true;
        isDashing = false;
        tr.emitting = false;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(groundCheckPos.position, JumpRadius);
        Gizmos.DrawSphere(WallJumpCheckPos.position, WallJumpRadius);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Death")
        {
            Die();
        }
    }

    public IEnumerator deathAfterWhile(float time)
    {
        yield return new WaitForSeconds(time);
        Instantiate(DeathEffect, transform.position, transform.rotation);
        Destroy(gameObject);
    }
    public void Die()
    {
        Instantiate(DeathEffect, transform.position, transform.rotation);
        Destroy(gameObject);
    }    
}