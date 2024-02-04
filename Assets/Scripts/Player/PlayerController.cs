using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    public Collider2D attackHitboxUp;
    public Collider2D attackHitboxDown;
    public Collider2D attackHitboxLeft;
    public Collider2D attackHitboxRight;
    private Vector2 currentAttackDirection;
    private Vector2 lastMovementDirection;
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 movement;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private bool isRolling = false;
    private bool isAttacking = false;
    private float originalSpeed = 4f;
    public float rollSpeedMultiplier = 200f;
    public float rollDuration = 0.34f;
    private float attackCooldown = 0.5f; // Cooldown duration in seconds
    private float lastAttackTime; // Time of the last attack
    private Collider2D currentAttackHitbox;
    private HashSet<GameObject> hitEnemies;
    private int attackSequence = 0;
    private float timeSinceLastAttack;
    private float attackSequenceResetTime = 1.0f;

    private bool canMove = true;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Debug.Log("Destroying duplicate player instance.");
            Destroy(gameObject);
        }

        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        lastMovementDirection = Vector2.right; // Default direction, adjust as needed
        DisableAllHitboxes(); 
    }

    void Update()
    {
        if (!isRolling && canMove)
        {
            Vector2 newMovement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            movement = newMovement;

            // Update the sprite's orientation only when not attacking
            if (newMovement != Vector2.zero && !isAttacking)
            {
                lastMovementDirection = newMovement.normalized;
                spriteRenderer.flipX = lastMovementDirection.x < 0;
            }

            // Update simulated speed for animator
            float simulatedSpeed = movement.sqrMagnitude > 0 ? 0.5f : 0f;
            animator.SetFloat("Speed", simulatedSpeed);
        }

        // Increment the attack timer outside of the movement check to ensure it's always updated
        timeSinceLastAttack += Time.deltaTime;

        // Check for Roll input
        if (Input.GetKeyDown(KeyCode.Space) && !isRolling && canMove && movement != Vector2.zero)
        {
            StartCoroutine(Roll());
        }

        if (Input.GetMouseButtonDown(0) && !isRolling && canMove && !isAttacking && Time.time >= lastAttackTime + attackCooldown)
        {
            // Reset attack sequence if needed
            if (timeSinceLastAttack > attackSequenceResetTime)
            {
                attackSequence = 0;
            }

            StartCoroutine(Attack());
            timeSinceLastAttack = 0; // Reset time since last attack
        }
    }


        private IEnumerator Attack()
        {
            isAttacking = true;
            lastAttackTime = Time.time;
            hitEnemies = new HashSet<GameObject>(); // Initialize the set for this attack
            animator.SetBool("isAttacking", true);

            currentAttackDirection = lastMovementDirection;  // Use the last movement direction for attack direction
            currentAttackHitbox = GetAttackHitbox(currentAttackDirection);
            EnableAttackHitbox();

            // Trigger the appropriate attack animation based on the sequence
            switch (attackSequence)
            {
                case 0:
                    animator.SetTrigger("Attack1");
                    break;
                case 1:
                    animator.SetTrigger("Attack2");
                    break;
                case 2:
                    animator.SetTrigger("Attack3");
                    break;
            }

            attackSequence = (attackSequence + 1) % 3;

            yield return new WaitForSeconds(0.5f); // Wait for the attack to finish

            DisableAttackHitbox();
            animator.SetBool("isAttacking", false);
            while (!canMove)
            {
                yield return null;
            }
            isAttacking = false;
        }

        private Collider2D GetAttackHitbox(Vector2 direction)
    {
        if (direction == Vector2.up)
            return attackHitboxUp;
        else if (direction == Vector2.down)
            return attackHitboxDown;
        else if (direction == Vector2.left)
            return attackHitboxLeft;
        else // assuming right direction
            return attackHitboxRight;
    }

    public void EnableAttackHitbox()
    {
        if (currentAttackHitbox != null)
            currentAttackHitbox.enabled = true;
    }

    public void DisableAttackHitbox()
    {
        if (currentAttackHitbox != null)
            currentAttackHitbox.enabled = false;
    }

    void DisableAllHitboxes()
    {
        if (attackHitboxUp != null)
            attackHitboxUp.enabled = false;
        if (attackHitboxDown != null)
            attackHitboxDown.enabled = false;
        if (attackHitboxLeft != null)
            attackHitboxLeft.enabled = false;
        if (attackHitboxRight != null)
            attackHitboxRight.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isAttacking) return;
        Debug.Log($"Collision with: {collision.gameObject.name}");

        if (collision.CompareTag("Enemy") && !hitEnemies.Contains(collision.gameObject))
        {
            EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();
   
            Debug.Log("Attacked an enemy and dealing damage!");

            // Calculate knockback direction
            Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;

            // Apply damage and knockback
            enemyHealth.TakeDamage(10, knockbackDirection);

            hitEnemies.Add(collision.gameObject);
            return;
        }
       
        if (collision.CompareTag("Destructible"))
        {
            Debug.Log("Hit a destructible object!");
            DestructibleObjectHealth destructibleHealth = collision.GetComponent<DestructibleObjectHealth>();
            if (destructibleHealth != null)
            {
                destructibleHealth.TakeDamage(10); // Adjust the damage value as needed
            }
        }
    }

    private IEnumerator Roll()
    {
        Vector2 rollDirection = movement.normalized; // Capture the current movement direction at the start of the roll
        isRolling = true;
        animator.SetTrigger("RollNow");

        moveSpeed *= rollSpeedMultiplier;

        float rollEndTime = Time.time + rollDuration;
        while (Time.time < rollEndTime)
        {
            rb.MovePosition(rb.position + rollDirection * moveSpeed * Time.fixedDeltaTime);
            yield return null; // Wait for the next frame
        }

        moveSpeed = originalSpeed;
        isRolling = false;
    }

    void FixedUpdate()
    {
        if (canMove || isRolling) // Allow movement if canMove is true or if the player is rolling
        {
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }
    }

    void OnDrawGizmos()
    {
        // Set the color of the Gizmo
        Gizmos.color = Color.red;

        // Draw hitbox for attackHitboxUp
        if (attackHitboxUp != null && attackHitboxUp.enabled)
        {
            DrawPolygonHitboxGizmo(attackHitboxUp);
        }

        // Repeat for other hitboxes
        if (attackHitboxDown != null && attackHitboxDown.enabled)
        {
            DrawPolygonHitboxGizmo(attackHitboxDown);
        }

        if (attackHitboxLeft != null && attackHitboxLeft.enabled)
        {
            DrawPolygonHitboxGizmo(attackHitboxLeft);
        }

        if (attackHitboxRight != null && attackHitboxRight.enabled)
        {
            DrawPolygonHitboxGizmo(attackHitboxRight);
        }
    }

    void DrawPolygonHitboxGizmo(Collider2D hitbox)
    {
        if (hitbox is PolygonCollider2D polygonCollider)
        {
            // Draw a polygon for PolygonCollider2D
            Vector2[] points = polygonCollider.points;
            for (int i = 0; i < points.Length; i++)
            {
                // Transform the local points to world space
                Vector2 currentPoint = hitbox.transform.TransformPoint(points[i]);
                Vector2 nextPoint = hitbox.transform.TransformPoint(points[(i + 1) % points.Length]);

                Gizmos.DrawLine(currentPoint, nextPoint);
            }
        }
    }

    public void LockMovement()
    {
        canMove = false;
    }

    public void UnlockMovement()
    {
        canMove = true;
    }
}
