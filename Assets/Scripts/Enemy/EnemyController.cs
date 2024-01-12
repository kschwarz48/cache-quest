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

    public float walkDistance = 2f;
    public float pauseDuration = 2f;
    private Vector2 startPosition;
    private Vector2 patrolPoint;

    public GameObject player;
    public float detectionRange = 5f;
    public float attackRange = 2f;

    private bool isAttacking = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        startPosition = rb.position;
        currentState = State.Patrolling;
        StartCoroutine(Patrol());
        Debug.Log("Starting patrol.");
        if (player == null) {
        Debug.LogError("Player object not found for " + gameObject.name);
        } else {
            Debug.Log(gameObject.name + " is targeting player: " + player.name);
        }
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        Debug.Log(gameObject.name + " Distance to player: " + distanceToPlayer);


        if (!isAttacking)
        {
            if (distanceToPlayer <= detectionRange)
            {
                Debug.Log(gameObject.name + " is within detection range.");
                if (currentState != State.Chasing)
                {
                    currentState = State.Chasing;
                    StopCoroutine(Patrol());
                    Debug.Log(gameObject.name + " stopping patrol, starting chase.");
                }

                if (distanceToPlayer <= attackRange)
                {
                    currentState = State.Attacking;
                    StartCoroutine(Attack());
                    Debug.Log(gameObject.name + " attacking player.");
                }
                else
                {
                    MoveTowards(player.transform.position);
                    Debug.Log(gameObject.name + " chasing player.");
                }
            }
            else if (currentState != State.Patrolling)
            {
                currentState = State.Patrolling;
                StartCoroutine(Patrol());
                Debug.Log(gameObject.name + " player out of range, returning to patrol.");
            }
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
        isAttacking = true;

        // Lunge towards the player's position
        Vector2 originalPosition = transform.position;
        Vector2 targetPosition = player.transform.position;
        float lungeSpeed = moveSpeed * 2;
        float lungeTime = 0.5f;

        float startTime = Time.time;
        while (Time.time < startTime + lungeTime)
        {
            transform.position = Vector2.Lerp(originalPosition, targetPosition, (Time.time - startTime) / lungeTime);
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        isAttacking = false;
        currentState = State.Chasing;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Handle collision with player or other objects
    }
}
