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
    private float originalSpeed = 5f;
    public float rollSpeedMultiplier = 200f;
    public float rollDuration = 0.34f;
    private float attackCooldown = 0.5f; // Cooldown duration in seconds
    private float lastAttackTime; // Time of the last attack
    private Collider2D currentAttackHitbox;
    private HashSet<GameObject> hitEnemies;


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
        if (!isRolling)
        {
            Vector2 newMovement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            // Always update the movement vector, but update lastMovementDirection only when there's movement
            if (newMovement != Vector2.zero)
            {
                lastMovementDirection = newMovement.normalized; // Update the last movement direction
            }
            movement = newMovement;

            // Flip the character's sprite based on movement direction
            if (movement.x > 0)
            {
                spriteRenderer.flipX = false; // Face right
            }
            else if (movement.x < 0)
            {
                spriteRenderer.flipX = true; // Face left
            }
        }

        // Set the Speed parameter for animation
        float simulatedSpeed = movement.sqrMagnitude > 0 ? 0.5f : 0f;
        animator.SetFloat("Speed", simulatedSpeed);

        // Roll and Attack inputs
        if (Input.GetKeyDown(KeyCode.Space) && !isRolling && movement != Vector2.zero)
        {
            StartCoroutine(Roll());
        }
        if (Input.GetMouseButtonDown(0) && !isRolling && !isAttacking && Time.time >= lastAttackTime + attackCooldown)  
        {
            StartCoroutine(Attack());
        }
    }

    private IEnumerator Attack()
    { 
        isAttacking = true;
        lastAttackTime = Time.time;
        hitEnemies = new HashSet<GameObject>(); // Initialize the set for this attack
        animator.SetBool("isAttacking", true);

        currentAttackDirection = DetermineAttackDirection();
        currentAttackHitbox = GetAttackHitbox(currentAttackDirection);
        EnableAttackHitbox();

        yield return new WaitForSeconds(0.5f); 

        DisableAttackHitbox();
        animator.SetBool("isAttacking", false);
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
    void EnableCorrectHitbox(Vector2 direction)
    {
        DisableAllHitboxes(); // First, disable all hitboxes

        if (direction == Vector2.up && attackHitboxUp != null)
        {
            attackHitboxUp.enabled = true;
        }
        else if (direction == Vector2.down && attackHitboxDown != null)
        {
            attackHitboxDown.enabled = true;
        }
        else if (direction == Vector2.left && attackHitboxLeft != null)
        {
            attackHitboxLeft.enabled = true;
        }
        else if (direction == Vector2.right && attackHitboxRight != null)
        {
            attackHitboxRight.enabled = true;
        }
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
            if (enemyHealth != null)
            {
                Debug.Log("Attacked an enemy and dealing damage!");
                enemyHealth.TakeDamage(10);
                hitEnemies.Add(collision.gameObject);
            }
        }
        else if (collision.CompareTag("Destructible"))
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
        Vector2 rollDirection = movement.normalized;  // Capture the current movement direction at the start of the roll
        isRolling = true;
        animator.SetTrigger("RollNow");

        moveSpeed *= rollSpeedMultiplier;

        float rollEndTime = Time.time + rollDuration;
        while (Time.time < rollEndTime)
        {
            rb.MovePosition(rb.position + rollDirection * moveSpeed * Time.fixedDeltaTime);
            yield return null;  // Wait for the next frame
        }

        moveSpeed = originalSpeed;
        isRolling = false;
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }


    private Vector2 DetermineAttackDirection()
    {
        // Use the last movement direction if currently stationary
        return movement == Vector2.zero ? lastMovementDirection : movement.normalized;
    }
}
