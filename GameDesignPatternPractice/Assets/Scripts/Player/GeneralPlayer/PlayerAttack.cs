using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;

    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private Transform attackArea;
    [SerializeField] private float attackRadius;
    [SerializeField] private float attackCoolDown;

    private float canAttackTime;
    private float attackDir;
    private float verticalAttackDir;
    private Vector2 realAttackDir;

    private bool canAttack;
    private bool isAttacking;

    void Awake()
    {
        animator = transform.GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        isAttacking = false;
        canAttackTime = attackCoolDown;
        canAttack = true;
    }

    void LateUpdate()
    {
        AttackCheck();
    }

    private void AttackCheck()
    {
        if (Input.GetKeyDown(KeyCode.J) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S) && canAttack )
        {
            attackDir = PlayerController.instance.MoveDirection.x;
            verticalAttackDir = 0;
            realAttackDir = new Vector2(attackDir, verticalAttackDir);
            isAttacking = true;
            canAttack = false;
            VerticalAttack();
            Debug.Log("vertical");
        }

        if (Input.GetKeyDown(KeyCode.J) && Input.GetKey(KeyCode.W) && canAttack)
        {
            attackDir = PlayerController.instance.MoveDirection.x;
            verticalAttackDir = 1f;
            realAttackDir = new Vector2(attackDir, verticalAttackDir);
            isAttacking = true;
            canAttack = false;
            UpAttack();
        }

        if (Input.GetKeyDown(KeyCode.J) && Input.GetKey(KeyCode.S) && canAttack)
        {
            attackDir = PlayerController.instance.MoveDirection.x;
            verticalAttackDir = -1f;
            realAttackDir = new Vector2(attackDir, verticalAttackDir);
            isAttacking = true;
            canAttack = false;
            DownAttack();
        }

        if (!canAttack)
        {
            canAttackTime -= Time.deltaTime;
        }

        if (canAttackTime <= 0)
        {
            canAttack = true;
            canAttackTime = attackCoolDown;
        }
    }

    private void VerticalAttack()
    {
        animator.SetTrigger("isAttacking");
        animator.SetFloat("attackDir", attackDir);
        animator.SetFloat("verticalAttackDir", verticalAttackDir);
        Collider2D[] enemies = Physics2D.OverlapCircleAll(attackArea.position, attackRadius, enemyLayers);

       if(enemies.Length != 0)
        {
            rb.velocity = Vector2.zero;
            Debug.Log("!!!!!");
            rb.AddForce(-transform.right * 15, ForceMode2D.Impulse);
        }

        foreach (Collider2D enemy in enemies)
        {
            Debug.Log("Hit: " + enemy.name.ToString());
        }
    }

    private void UpAttack()
    {
        animator.SetTrigger("isAttacking");
        animator.SetFloat("attackDir", attackDir);
        animator.SetFloat("verticalAttackDir", verticalAttackDir);
        Collider2D[] enemies = Physics2D.OverlapCircleAll(attackArea.position, attackRadius, enemyLayers);

        foreach (Collider2D enemy in enemies)
        {
            Debug.Log("Hit: " + enemy.name.ToString());
        }
        verticalAttackDir = 0;
    }

    private void DownAttack()
    {
        animator.SetTrigger("isAttacking");
        animator.SetFloat("attackDir", attackDir);
        animator.SetFloat("verticalAttackDir", verticalAttackDir);
        Collider2D[] enemies = Physics2D.OverlapCircleAll(attackArea.position, attackRadius, enemyLayers);

        foreach (Collider2D enemy in enemies)
        {
            Debug.Log("Hit: " + enemy.name.ToString());
        }
    }

    private void OnDrawGizmosSelected()
    {
        //Gizmos.DrawSphere(transform.Find("AttackArea").position, attackRadius);
    }



}
