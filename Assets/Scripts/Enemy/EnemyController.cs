using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 1f;
    private Rigidbody2D rb;
    private Vector2 movementDirection;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool isAttacking = false;
    private bool isReturning = false;

    public float walkDistance = 2f; // The distance the enemy will walk before stopping
    public float pauseDuration = 2f; // Time in seconds the enemy pauses before returning
    private Vector2 startPosition;
    private Vector2 patrolPoint;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        startPosition = rb.position;
        StartCoroutine(Patrol());
    }

    // Patrol coroutine
    private IEnumerator Patrol()
    {
        while (true)
        {
            // Choose a random direction
            movementDirection = Random.insideUnitCircle.normalized;
            patrolPoint = startPosition + movementDirection * walkDistance;
            isReturning = false;

            // Walk to the patrol point
            while (!isReturning && (rb.position - patrolPoint).sqrMagnitude > 0.01f)
            {
                MoveTowards(patrolPoint);
                yield return null;
            }

            // Pause for a bit
            yield return new WaitForSeconds(pauseDuration);

            // Return to start position
            isReturning = true;
            while ((rb.position - startPosition).sqrMagnitude > 0.01f)
            {
                MoveTowards(startPosition);
                yield return null;
            }

            // Pause before choosing a new patrol point
            yield return new WaitForSeconds(pauseDuration);
        }
    }

    // Method to move towards a target position
    private void MoveTowards(Vector2 targetPosition)
    {
        rb.MovePosition(Vector2.MoveTowards(rb.position, targetPosition, moveSpeed * Time.deltaTime));
        FlipSprite(targetPosition - rb.position);
    }

    // Method to flip the sprite based on direction
    private void FlipSprite(Vector2 direction)
    {
        if (direction.x < 0)
            spriteRenderer.flipX = false; // Facing right
        else if (direction.x > 0)
            spriteRenderer.flipX = true; // Facing left
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAttacking && ShouldAttack())
        {
            StartCoroutine(Attack());
        }
    }


    private IEnumerator Attack()
    {
        isAttacking = true;
        animator.SetBool("isAttacking", true);

        // Perform attack logic here (this is a placeholder for your attack logic)
        
        yield return new WaitForSeconds(1f); // Duration of the attack animation

        isAttacking = false;
        animator.SetBool("isAttacking", false);
    }

    private bool ShouldAttack()
    {
        // Placeholder for your condition to decide if the enemy should attack
        // This could be based on distance to the player, line of sight, etc.
        return false;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Handle collision with player or other objects
    }
}
