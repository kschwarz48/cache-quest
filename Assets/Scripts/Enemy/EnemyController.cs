using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    private enum State
    {
        Patrolling,
        Chasing,
        Attacking,
        KnockedBack,
        Dead
    }

    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float walkDistance = 2f;
    [SerializeField] private float pauseDuration = 2f;
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float lungeSpeed = 10f;
    [SerializeField] private float lungeTime = 0.5f;
    [SerializeField] private GameObject player;
    [SerializeField] private float attackCooldown = 2f; // Time between attacks
    private float lastAttackTime = -999f;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Vector2 startPosition;
    private State currentState = State.Patrolling;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
        startPosition = rb.position;
    }

    void Start()
    {
        StartCoroutine(Patrol());
    }

    void Update()
    {
        if (currentState == State.Dead) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        switch (currentState)
        {
            case State.Patrolling:
            case State.Chasing:
                UpdateMovementState(distanceToPlayer);
                break;
            case State.KnockedBack:
                // Knockback behavior is handled in HandleKnockback
                break;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Health playerHealth = collision.gameObject.GetComponent<Health>();
            playerHealth.TakeDamage(50, (collision.transform.position - transform.position).normalized);
        }
    }


    // Inside UpdateMovementState, adjust to include attack cooldown check
    private void UpdateMovementState(float distanceToPlayer)
    {
        if (distanceToPlayer <= detectionRange)
        {
            if (currentState != State.Chasing && currentState != State.Attacking)
            {
                ChangeState(State.Chasing);
            }

            // Check if enough time has passed since the last attack
            if (distanceToPlayer <= attackRange && currentState != State.Attacking && Time.time > lastAttackTime + attackCooldown)
            {
                StartCoroutine(Attack());
            }
            else if (currentState != State.Attacking)
            {
                MoveTowards(player.transform.position);
            }
        }
        else
        {
            if (currentState != State.Patrolling)
            {
                ChangeState(State.Patrolling);
            }
        }
    }

    private IEnumerator Patrol()
    {
        ChangeState(State.Patrolling);
        while (currentState == State.Patrolling)
        {
            Vector2 patrolPoint = startPosition + Random.insideUnitCircle.normalized * walkDistance;
            yield return MoveTo(patrolPoint, pauseDuration);
            yield return MoveTo(startPosition, pauseDuration);
        }
    }

    private IEnumerator MoveTo(Vector2 target, float pauseTime)
    {
        while ((rb.position - target).sqrMagnitude > 0.01f)
        {
            MoveTowards(target);
            yield return null;
        }
        yield return new WaitForSeconds(pauseTime);
    }

    private void MoveTowards(Vector2 targetPosition)
    {
        if (currentState == State.Chasing || currentState == State.Patrolling)
        {
            animator.SetBool("isMoving", true);
            rb.MovePosition(Vector2.MoveTowards(rb.position, targetPosition, moveSpeed * Time.deltaTime));
        }
        else
        {
            animator.SetBool("isMoving", false);
        }
        FlipSprite(targetPosition - rb.position);
    }

    private IEnumerator Attack()
    {
        lastAttackTime = Time.time; // Record the time of attack
        ChangeState(State.Attacking);

        // Modify direction calculation to add unpredictability or prediction
        Vector2 direction = (player.transform.position - transform.position).normalized;

        // Optionally, adjust lunge speed or time based on player distance
        float currentLungeSpeed = lungeSpeed; // Could be adjusted dynamically

        rb.velocity = direction * currentLungeSpeed;
        yield return new WaitForSeconds(lungeTime);

        // After lunge, return to chasing or another state based on context
        ChangeState(State.Chasing);
    }

    public void HandleKnockback(Vector2 knockbackForce)
    {
        ChangeState(State.KnockedBack);
        rb.velocity = knockbackForce; // Apply knockback force
        StartCoroutine(KnockbackRecovery());
    }

    private IEnumerator KnockbackRecovery()
    {
        yield return new WaitForSeconds(0.5f); // Adjust this duration as needed
        if (currentState != State.Dead)
        {
            ChangeState(State.Patrolling);
        }
    }

    public void Die()
    {
        ChangeState(State.Dead);
        animator.SetBool("isAlive", false);
        rb.velocity = Vector2.zero;
        rb.isKinematic = true; // Prevent further physics interactions
        this.enabled = false; // Optionally disable this script to stop all behavior
    }

    private void FlipSprite(Vector2 direction)
    {
        spriteRenderer.flipX = direction.x < 0;
    }

    private void ChangeState(State newState)
    {
        currentState = newState;
        switch (currentState)
        {
            case State.Patrolling:
            case State.Chasing:
                animator.SetBool("isMoving", true);
                break;
            case State.Attacking:
                animator.SetBool("isAttacking", true);
                animator.SetBool("isMoving", false);
                break;
            case State.KnockedBack:
                animator.SetBool("isMoving", false);
                break;
            case State.Dead:
                animator.SetBool("isMoving", false);
                animator.SetBool("isAttacking", false);
                break;
        }
    }
}
