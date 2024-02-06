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
    public float moveSpeed = 500f;
    private Rigidbody2D rb;
    private Vector2 movement;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private bool isRolling = false;
    private bool isAttacking = false;
    public float rollSpeedMultiplier = 200f;
    public float rollDuration = 0.34f;
    private float attackCooldown = 0.5f; // Cooldown duration in seconds
    private float lastAttackTime; // Time of the last attack
    private Collider2D currentAttackHitbox;
    private HashSet<GameObject> hitEnemies;
    private int attackSequence = 0;
    private float timeSinceLastAttack;
    private float attackSequenceResetTime = 1.0f;

    public AudioClip attackSound1;
    public AudioClip attackSound2;
    public AudioClip attackSound3;

    public float maxSpeed = 3.5f;

    private bool canMove = true;

    public float idleFriction = 0.9f;

    public static bool isGamePaused = false;

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
        if (isGamePaused) return;
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
                    AudioManager.Instance.PlaySoundEffect(attackSound1);
                    break;
                case 1:
                    animator.SetTrigger("Attack2");
                    AudioManager.Instance.PlaySoundEffect(attackSound2);
                    break;
                case 2:
                    animator.SetTrigger("Attack3");
                    AudioManager.Instance.PlaySoundEffect(attackSound3);
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
            Vector2 knockbackDirection = (Vector2) (collision.transform.position - transform.position).normalized;

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

        // Calculate the roll force based on the roll speed multiplier
        Vector2 rollForce = rollDirection * rollSpeedMultiplier;

        // Apply the roll force once as an impulse for immediate effect
        rb.AddForce(rollForce, ForceMode2D.Impulse);

        // Optionally, you might want to temporarily increase the drag on the Rigidbody2D to quickly slow down the roll after a certain duration
        float originalDrag = rb.drag;
        rb.drag = 5f; // Increase the drag to slow down the roll. Adjust this value as needed.

        // Wait for the roll to complete based on the roll duration
        yield return new WaitForSeconds(rollDuration);

        // After rolling, reset the drag to its original value
        rb.drag = originalDrag;

        isRolling = false;
    }


    void FixedUpdate()
    {
        if ((canMove || isRolling) && !isAttacking) // Check if the player can move or is rolling but not attacking
        {
            rb.AddForce(movement * moveSpeed * Time.deltaTime);

            if(rb.velocity.magnitude > maxSpeed) {
                float limitedSpeed = Mathf.Lerp(rb.velocity.magnitude, maxSpeed, idleFriction);
                rb.velocity = rb.velocity.normalized * limitedSpeed;
            }

        } else {
            rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, idleFriction);

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
