using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    private int currHealth;
    public int MaxHealth { get { return maxHealth; } }

    public int CurrHealth { get { return currHealth; } }

    [SerializeField] private float invicTime;
    private float invicCounter;
    private bool isInvic;

    private bool isDead;

    private float hurtDisableInputTime;
    private float hurtDisableTimer;
    private bool isHurtByEnemy;


    private Animator animator;
    private Rigidbody2D rb;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        currHealth = maxHealth;
        invicCounter = invicTime;
        isInvic = false;
        isDead = false;

        hurtDisableInputTime = 0.3f;
        hurtDisableTimer = hurtDisableInputTime;
    }

    void Update()
    {
        CheckInvic();

        CheckHurt();
    }

    private void CheckInvic()
    {
        if (isInvic)
        {
            invicCounter -= Time.deltaTime;
        }

        if (invicCounter <= 0)
        {
            invicCounter = invicTime;
            isInvic = false;
            isHurtByEnemy = false;
        }
    }

    private void CheckHurt()
    {
        if (isHurtByEnemy && hurtDisableTimer >= 0)
        {
            
            hurtDisableTimer -= Time.deltaTime;
        }
        else
        {
            PlayerController.instance.DisableMove = false;
            hurtDisableTimer = hurtDisableInputTime;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Enemy")
        {
            if (!isInvic)
            {
                if (currHealth > 0)
                {
                    currHealth--;
                    HealthBarEvents.current.HealthDown();

                    isHurtByEnemy = true;
              
                    PlayerController.instance.DisableMove = true;
                    rb.AddForce(-transform.right * 15, ForceMode2D.Impulse);
                }
                if (currHealth == 0 && !isDead)
                {
                    animator.SetTrigger("isDead");
                    isDead = !isDead;
                    Debug.Log("You're so dead");
                }
                isInvic = true;
            }
        }
        
    }

}
