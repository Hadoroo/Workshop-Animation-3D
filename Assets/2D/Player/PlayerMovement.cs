using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float fallMultiplier = 2.5f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Attack")]
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Dont Edit this")]    
    [SerializeField]private Rigidbody2D rb;
    [SerializeField]private Animator animator;
    [SerializeField]private float moveDirection = 0f;
    [SerializeField] private bool jump = false;
    [SerializeField] private bool isGrounded;
    [SerializeField] private float lastAttackTime = 0f;
    [SerializeField]public float verticalVelocity = 0f;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        GroundCheck();
        SetAnimationParameters();
    }

    private void FixedUpdate()
    {
        Movement();
        Jump();
    }
    private void SetAnimationParameters()
    {
        animator.SetFloat("Speed", Mathf.Abs(moveDirection));
        animator.SetBool("isGrounded", isGrounded);
        animator.SetFloat("VerticalVelocity", verticalVelocity);
    }
    private void GroundCheck()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);
    }
    private void Movement()
    {
        rb.linearVelocity = new Vector2(moveDirection * moveSpeed, rb.linearVelocity.y);
        Flip();
    }
    private void Jump()
    {
        if (jump && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        if(!jump && !isGrounded)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }

        verticalVelocity = rb.linearVelocity.y;
    }

    private void Attack()
    {
        // if (Time.time < lastAttackTime + attackCooldown)
        // return;

        // lastAttackTime = Time.time;

        // Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

        // foreach (var enemy in hitEnemies)
        // {
        //     Debug.Log("Hit: " + enemy.name);
        // }
    }
    private void Flip()
    {
        if (moveDirection > 0)
        {
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
            // if (attackPoint != null) attackPoint.localPosition = new Vector3(Mathf.Abs(attackPoint.localPosition.x), attackPoint.localPosition.y, attackPoint.localPosition.z);
        }
        else if (moveDirection < 0)
        {
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
            // if (attackPoint != null) attackPoint.localPosition = new Vector3(-Mathf.Abs(attackPoint.localPosition.x), attackPoint.localPosition.y, attackPoint.localPosition.z);
        }
    }


#region Input System
    public void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();
        moveDirection = input.x;
    }

    public void OnJump(InputValue value)
    {
        jump = value.isPressed;
    }

    public void OnAttack(InputValue value)
    {
        Attack();
    }

#endregion
    private void OnDrawGizmos()
    {

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
    }
}
