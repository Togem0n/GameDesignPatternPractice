using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossIdle : StateMachineBehaviour
{

    public float speed = 3f;

    public Transform player;
    Rigidbody2D rb;

    Boss boss;

    Vector2 wanderingStartPos;
    Vector2 wanderingEndPos;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = animator.GetComponent<Rigidbody2D>();
        boss = animator.GetComponent<Boss>();

        wanderingStartPos = new Vector2(4f, 3f);

        wanderingEndPos = new Vector2(-2f, 3f);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        Vector2 playerPos = new Vector2(player.position.x, player.position.y);

        if(Mathf.Abs(playerPos.x - rb.position.x) < 3f)
        {
            boss.LookAtPlayer();
            // go next state
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

}
