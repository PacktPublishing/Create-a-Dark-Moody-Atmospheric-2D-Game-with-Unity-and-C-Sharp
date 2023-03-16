using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    #region public vars
    public Transform rayOrigin;
    public LayerMask rayMask;
    public float rayLength;
    public float attackDistance;
    public float timer;
    public float moveSpeed;
    public Animator anim;

    #endregion

    #region private vars
    RaycastHit2D attackRay;
    GameObject player;
    float playerDistance;
    bool attackMode;
    bool nearBy;
    bool cooling;
    float intTimer;
    #endregion
    private void Awake()
    {
        intTimer = timer;
        
    }

    void Update()
    {
        if(nearBy)
        {
            attackRay = Physics2D.Raycast(rayOrigin.position, Vector2.right, rayLength, rayMask);
        }
        if(attackRay.collider != null)
        {
            enemyLogic();
        }
        else if(attackRay.collider == null)
        {
            nearBy = false;
        }
        if(!nearBy)
        {
            anim.SetBool("move", false);
            anim.SetBool("attack", false);
            StopAttack();
        }
    }

    void enemyLogic()
    {
        playerDistance = Vector2.Distance(transform.position, player.transform.position);
        if(playerDistance > attackDistance)
        {
            // run anim
            anim.SetBool("move", true);
            anim.SetBool("attack", false);
            Move();
            StopAttack();
        }
        else if(attackDistance >= playerDistance)
        {
            if(timer <= 0)
            {
                anim.SetBool("move", false);
                anim.SetBool("attack", true);
                // attack anim
                Attack();
                timer = intTimer;
            }
            else
            {
                
                
                anim.SetBool("attack", false);
                timer -= Time.deltaTime;
            }
           
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            player = collision.gameObject;
            nearBy = true;
        }
    }
    void Move()
    {
        Vector2 playerPos = new Vector2(player.transform.position.x, transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, playerPos, moveSpeed * Time.deltaTime);
    }

    void StopAttack()
    {
        attackMode = false;
        cooling = false;
    }

    void Attack()
    {
        Debug.Log("Attacking!");
        attackMode = true;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(rayOrigin.position, Vector2.right * rayLength);
    }
}
