using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 1f;
    private Rigidbody2D rb;
    private Vector2 movementDirection;
    private Animator animator;
    private bool isAttacking = false;
    
    public float patrolRadius = 2f; // The radius within which the enemy will patrol
    private Vector2 startPosition;
    private float moveTimer;
    private float directionChangeTime = 2f; // Time in seconds between direction changes

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        startPosition = rb.position;
        PickNewDirection();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAttacking && ShouldAttack())
        {
            StartCoroutine(Attack());
        }

        // Update the movement timer and switch directions if it's time
        moveTimer += Time.deltaTime;
        if (moveTimer >= directionChangeTime)
        {
            PickNewDirection();
        }
    }

    void FixedUpdate()
    {
        // If the enemy is too far from its start position, pick a new direction
        if ((rb.position - startPosition).sqrMagnitude > patrolRadius * patrolRadius)
        {
            PickNewDirection();
        }

        // Move the enemy
        rb.MovePosition(rb.position + movementDirection * moveSpeed * Time.fixedDeltaTime);
    }

    private void PickNewDirection()
    {
        moveTimer = 0f; // Reset the move timer

        // Randomly pick a direction
        float angle = Random.Range(0, 360) * Mathf.Deg2Rad;
        movementDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;

        // Reset the direction change time randomly
        directionChangeTime = Random.Range(1f, 3f); // Change direction every 1 to 3 seconds
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
