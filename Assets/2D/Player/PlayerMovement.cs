using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public enum AnimationTriggerType
    {
        GiveDamage,
        Footstep,
    }
    [Header("Animations Names")]
    [SerializeField] private string attackAnimationName = "Attack";
    [Space]
    [SerializeField] private string SpeedParameterName = "Speed";
    [SerializeField] private string isGroundedParameterName = "isGrounded";
    [SerializeField] private string verticalVelocityParameterName = "VerticalVelocity";

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float riseMultiplier = 2f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Attack")]
    [SerializeField] private float attackDamage = 1f;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private LayerMask enemyLayer;

    [Header("SFX")]
    [SerializeField] private AudioClip[] FootstepAudioClips;

    [Header("Dont Edit this")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private float moveDirection = 0f;
    [SerializeField] private bool jump = false;
    [SerializeField] private bool isGrounded;
    [SerializeField] private float lastAttackTime = 0f;
    [SerializeField] public float verticalVelocity = 0f;


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
        if(animator == null) return;
        animator.SetFloat(SpeedParameterName, Mathf.Abs(moveDirection));
        animator.SetBool(isGroundedParameterName, isGrounded);
        animator.SetFloat(verticalVelocityParameterName, verticalVelocity);
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

        if (rb.linearVelocity.y > 0 && !jump)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rb.linearVelocity.y > 0 && jump)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (riseMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }

        verticalVelocity = rb.linearVelocity.y;
    }


    private void Attack()
    {
        if (Time.time < lastAttackTime + attackCooldown) return;
        lastAttackTime = Time.time;

        animator.Play("Attack");
    }
    private void GiveDamage() {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

        foreach (Collider2D enemyCollider in hitEnemies)
        {
            IDamageable damageable = enemyCollider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(attackDamage);
                Debug.Log("Hit: " + enemyCollider.name);
            }
        }
    }

    private void Flip()
    {
        if (moveDirection > 0)
        {
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
        }
        else if (moveDirection < 0)
        {
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
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
#region Animation Events
    public void AnimationEvent(AnimationTriggerType triggerType)
    {
        switch (triggerType)
        {
            case AnimationTriggerType.GiveDamage:
                GiveDamage();
                break;
            case AnimationTriggerType.Footstep:
                OnFootstep();
                break;
        }
    }
#endregion

#region SFX
    private void OnFootstep()
    {
        if (FootstepAudioClips.Length > 0)
        {
            var index = Random.Range(0, FootstepAudioClips.Length);
            AudioSource.PlayClipAtPoint(FootstepAudioClips[index], groundCheckPoint.position);
        }
    }
#endregion

#region Gizmos
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (groundCheckPoint != null)
            Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);

        Gizmos.color = Color.yellow;
        if (attackPoint != null)
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
#endregion
}
