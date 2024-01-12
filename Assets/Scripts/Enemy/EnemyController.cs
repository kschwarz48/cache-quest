using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    private enum State
    {
        Patrolling,
        Chasing,
        Attacking
    }

    public float moveSpeed = 1f;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private State currentState;
    private bool isChaseEnabled = true;

    public float walkDistance = 2f;
    public float pauseDuration = 2f;
    private Vector2 startPosition;
    private Vector2 patrolPoint;

    public GameObject player;
    public float detectionRange = 5f;
    public float attackRange = 2f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        startPosition = rb.position;
        currentState = State.Patrolling;
        StartCoroutine(Patrol());
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        switch (currentState)
        {
            case State.Patrolling:
                if (distanceToPlayer <= detectionRange)
                {
                    currentState = State.Chasing;
                }
                break;
            case State.Chasing:
                if (distanceToPlayer <= attackRange)
                {
                    StartCoroutine(Attack());
                }
                else if (distanceToPlayer > detectionRange)
                {
                    currentState = State.Patrolling;
                }
                else if (isChaseEnabled)
                {
                    MoveTowards(player.transform.position);
                }
                break;
            case State.Attacking:
                // Attack logic is handled in the Attack coroutine
                break;
        }
    }

    private IEnumerator Patrol()
    {
        while (currentState == State.Patrolling)
        {
            patrolPoint = startPosition + Random.insideUnitCircle.normalized * walkDistance;

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
        rb.MovePosition(Vector2.MoveTowards(rb.position, targetPosition, moveSpeed * Time.deltaTime));
        FlipSprite(targetPosition - rb.position);
    }

    private void FlipSprite(Vector2 direction)
    {
        if (direction.x < 0)
            spriteRenderer.flipX = false;
        else if (direction.x > 0)
            spriteRenderer.flipX = true;
    }

    private IEnumerator Attack()
    {
        currentState = State.Attacking;
        isChaseEnabled = false;

        // Lunge towards the player's position
        Vector2 originalPosition = transform.position;
        Vector2 targetPosition = player.transform.position;
        float lungeTime = 0.5f;

        float startTime = Time.time;
        while (Time.time < startTime + lungeTime)
        {
            transform.position = Vector2.Lerp(originalPosition, targetPosition, (Time.time - startTime) / lungeTime);
            yield return null;
        }

        yield return new WaitForSeconds(1f); // Pause after attack

        isChaseEnabled = true;
        currentState = State.Chasing;
    }
}
