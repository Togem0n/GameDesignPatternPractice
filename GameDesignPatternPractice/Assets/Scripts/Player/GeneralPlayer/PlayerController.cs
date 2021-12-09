using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Component
    private Animator animator;
    private Rigidbody2D rigidbody2d;

    //Singleton
    public static PlayerController instance;

    [Header("Movement")]

    [SerializeField] private float movementSpeed;
    [SerializeField] private bool disableMove;
    private Vector2 moveDirection;
    private float moveInput;
    private bool isRunning;

    public Vector2 MoveDirection { get { return moveDirection; } }
    public bool DisableMove { get { return disableMove; } set { disableMove = value; } }

    [Header("Jumping")]

    [SerializeField] private float jumpForce;
    [SerializeField] private float maxJumpTime;
    [SerializeField] private float startGravityScale;
    [SerializeField] private float checkRadius;
    private LayerMask WhatIsGround;
    private Transform feetPos;
    private bool isGrounded;
    private bool isJumping;
    private bool isFalling;
    private float jumpTimeCounter;
    private int jumpTimesRemain;
    private int maxJumpTimes;

    public bool IsJumping { get { return isJumping; } }
    public bool IsFalling { get { return isFalling; } }

    [Header("Dash")]

    [SerializeField] private float totalDashTime;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float canDashTime;
    private Vector2 startDashingDir;
    private float dashTime;
    private float canDashCount;
    private bool isDashing;
    private bool canDash;

    [Header("Slide")]

    [SerializeField] private float wallSlidingSpeed;
    private Transform frontCheck;
    private bool isTouchingFront;
    private bool isSliding;

    [Header("WallJump")]

    [SerializeField] private float wallJumpTime;
    [SerializeField] private float xWallForce;
    [SerializeField] private float yWallForce;
    private float wallJumpDir;
    private float countWallJumpTime;
    private float afterSliceDirection;
    private bool isWallJumping;
    private bool afterSlideChanged;

    [Header("Test")]

    [SerializeField] GameObject testToSpawn;
    [SerializeField] private Transform testFeetPos;
    [SerializeField] private Transform testFrontCheck;

    private void Awake()
    {
        //Singleton
        if(instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }

        disableMove = false;
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        WhatIsGround = LayerMask.GetMask("Ground");
        feetPos = transform.Find("FeetPos");
        frontCheck = transform.Find("FrontArea");

        // movement
        isRunning = false;
        moveDirection = new Vector2(1, 0);

        // jump
        isJumping = false;
        isFalling = false;
        maxJumpTimes = 2;
        jumpTimesRemain = maxJumpTimes;

        // dash
        isDashing = false;
        canDash = true;
        dashTime = totalDashTime;
        canDashCount = canDashTime;

        // slide
        isSliding = false;
        countWallJumpTime = wallJumpTime;

        rigidbody2d.gravityScale = startGravityScale;

    }


    private void Update()
    {

        //MoveAnimation();

        Jump();

        JumpAnimation();

        DashCheck();

        DashAnimation(); 

        IsSlidingCheck();

        WallJump();

    }

    private void FixedUpdate()
    {

        Vector2 feetAreaLU = new Vector2();
        feetAreaLU.x = feetPos.position.x - 0.31f;
        feetAreaLU.y = feetPos.position.y - 0.025f;

        Vector2 feetAreaRB = new Vector2();
        feetAreaRB.x = feetPos.position.x + 0.31f;
        feetAreaRB.y = feetPos.position.y + 0.025f;

        isGrounded = Physics2D.OverlapArea(feetAreaLU, feetAreaRB, WhatIsGround);

        isTouchingFront = Physics2D.OverlapCircle(frontCheck.position, checkRadius, WhatIsGround);

        Dash();

        if (!disableMove)
        {
            MoveCheck();
            MoveAnimation();
            SetVelocity();
        }
    }


    private void MoveCheck()
    {

        moveInput = Input.GetAxisRaw("Horizontal");
        isRunning = (moveInput != 0 && !isWallJumping) ? true : false;

    }

    private void MoveAnimation()
    {
        if (!isSliding)
        {
            Vector2 move = new Vector2(moveInput, 0);

            if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
            {
                moveDirection.Set(move.x, move.y);
                moveDirection.Normalize();
            }
            if (!isRunning && !isDashing)
            {
                animator.SetFloat("MoveX", moveDirection.x);
                animator.SetFloat("MoveSpeed", 0);
            }
            else if (isRunning)
            {
                animator.SetFloat("MoveX", moveDirection.x);
                animator.SetFloat("MoveSpeed", rigidbody2d.velocity.magnitude);
            }
        }
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && jumpTimesRemain > 0 && !isSliding && !isWallJumping && !disableMove)
        {
            isJumping = true;
            isFalling = false;

            jumpTimesRemain -= 1;
            jumpTimeCounter = maxJumpTime;

            rigidbody2d.velocity = Vector2.zero;
            rigidbody2d.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        }

        if (Input.GetKey(KeyCode.Space) && isJumping)
        {
            if (jumpTimeCounter > 0)
            {
                jumpTimeCounter -= Time.deltaTime;
                isJumping = true;
                isFalling = false;
            }
            else
            {
                isJumping = false;
                isFalling = true;
            }
        }

        if (Input.GetKeyUp(KeyCode.Space) && isJumping)
        {

            isJumping = false;
            isFalling = true;

            rigidbody2d.velocity = new Vector2(rigidbody2d.velocity.x, 0);
        }

        if (!isGrounded && !isJumping)
        {
            isFalling = true;
        }

        if (isGrounded && isFalling)
        {
            isJumping = false;
            isFalling = false;
            jumpTimesRemain = maxJumpTimes;
        }
    }

    private void JumpAnimation()
    {
        animator.SetBool("isJumping", isJumping);
        animator.SetBool("isFalling", isFalling);
    }

    private void SetVelocity()
    {
        if (!isDashing && !isWallJumping)
        {
            Vector3 tmp = new Vector3();
            tmp.x = (isRunning) ? moveInput * movementSpeed : 0;

            if (isSliding)
            {
                tmp.y = -wallSlidingSpeed;
            }
            else
            {
                tmp.y = rigidbody2d.velocity.y;
            }

            tmp.y = Mathf.Clamp(tmp.y, -15f, 100f);
            tmp.z = 0;
            rigidbody2d.velocity = tmp;
        }

    }

    private void DashCheck()
    {
        if (Input.GetKeyDown(KeyCode.K) && canDash && !disableMove)
        {
            canDash = false;
            isDashing = true;
            startDashingDir = moveDirection;
        }

        if (!canDash)
        {
            canDashCount -= Time.deltaTime;
        }

        if (canDashCount <= 0)
        {
            canDash = true;
            canDashCount = canDashTime;
        }

    }

    private void Dash()
    {
        if (isDashing)
        {
            disableMove = true;

            rigidbody2d.gravityScale = 0;
            dashTime -= Time.deltaTime;

            Vector2 tmp = new Vector2();
            tmp.x = startDashingDir.x * dashSpeed;
            tmp.y = 0;
            rigidbody2d.velocity = tmp;
        }

        if (dashTime <= 0)
        {
            disableMove = false;
            isDashing = false;
            dashTime = totalDashTime;
            rigidbody2d.gravityScale = startGravityScale;
        }
    }

    private void DashAnimation()
    {
        animator.SetBool("isDashing", isDashing);
        animator.SetFloat("MoveX", startDashingDir.x);
    }

    private void IsSlidingCheck()
    {
        if (isTouchingFront && !isGrounded && isFalling && !isDashing)
        {
            isSliding = true;
            jumpTimesRemain = maxJumpTimes;

            if (afterSlideChanged)
            {
                afterSlideChanged = false;
                afterSliceDirection = -afterSliceDirection;
            }
            moveDirection.x = afterSliceDirection;
            animator.SetFloat("MoveX", afterSliceDirection);
        }
        else
        {
            isSliding = false;
            afterSliceDirection = moveDirection.x;
            afterSlideChanged = true;
            animator.SetFloat("MoveX", afterSliceDirection);
        }
        animator.SetBool("isSliding", isSliding);
    }

    private void WallJump()
    {
        if (isSliding && Input.GetKeyDown(KeyCode.Space))
        {
            isWallJumping = true;
            isSliding = false;
            wallJumpDir = moveDirection.x;
        }

        if (isWallJumping)
        {
            isRunning = false;
            disableMove = true;
            countWallJumpTime -= Time.deltaTime;
            rigidbody2d.velocity = new Vector2(xWallForce * wallJumpDir, yWallForce);
            animator.SetFloat("MoveX", wallJumpDir);
        }

        if (countWallJumpTime <= 0)
        {
            countWallJumpTime = wallJumpTime;
            isWallJumping = false;
            disableMove = false;
        }
        animator.SetBool("isWallJumping", isWallJumping);
    }

    private void OnDrawGizmosSelected()
    {

        Gizmos.DrawCube(testFeetPos.position, new Vector3(0.62f, 0.05f, 1f));
        Gizmos.DrawWireSphere(testFrontCheck.position, checkRadius);

    }

}