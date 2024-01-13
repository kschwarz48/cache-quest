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
    private int attackSequence = 0;
    private float timeSinceLastAttack;
    private float attackSequenceResetTime = 1.0f;

    private int lastDirection;


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
        lastMovementDirection = Vector2.down; // Default to facing down
        DisableAllHitboxes(); 
        lastDirection = 0; // Down

        // Initialize to idle down values
        animator.SetFloat("Horizontal", 0f);
        animator.SetFloat("Vertical", -1f); // Assuming -1 is the 'down' direction
        lastDirection = 0; // Assuming this is the index for 'down'
    }

    void Update()
    {
        if (!isRolling && !isAttacking)
        {
            HandleMovementInput();
            HandleAttackInput();
        }

        UpdateAnimatorParameters();
    }


    private void HandleMovementInput()
    {
        Vector2 newMovement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (newMovement != Vector2.zero)
        {
            lastMovementDirection = newMovement.normalized;
            movement = newMovement.normalized; // Normalize the movement vector
        }
        else if (movement != Vector2.zero) // Check if movement just stopped
        {
            movement = Vector2.zero;
            SetIdleParameters();
        }
    }


    private void HandleAttackInput()
    {
        if (Input.GetMouseButtonDown(0) && Time.time >= lastAttackTime + attackCooldown)
        {
            StartCoroutine(Attack());
            timeSinceLastAttack = 0; // Reset time since last attack
            attackSequence = (attackSequence + 1) % 3; // Update the attack sequence
        }
    }

    private void UpdateAttackSequence()
    {
        timeSinceLastAttack += Time.deltaTime;
        if (timeSinceLastAttack > attackSequenceResetTime)
        {
            attackSequence = 0; // Reset the attack sequence if there's a delay
        }
    }

    private void UpdateAnimatorParameters()
    {
        float speed = movement.magnitude;

        // Normalize the movement vector and update the Animator
        Vector2 normMovement = movement.normalized;
        animator.SetFloat("Speed", speed);
        animator.SetFloat("Horizontal", normMovement.x);
        animator.SetFloat("Vertical", normMovement.y);

        if (speed > 0f)
        {
            // Update the lastDirection based on the current movement direction
            UpdateDirectionBasedOnMovement(normMovement);
        }
        else
        {
            // Ensure that the idle animation in the last direction is used
            SetIdleParameters();
        }
    }

    private void SetIdleParameters()
    {
        animator.SetInteger("Direction", lastDirection);
        animator.SetFloat("Speed", 0f);
    }

    private void UpdateDirectionBasedOnMovement(Vector2 normMovement)
    {
        if (normMovement.x > 0) lastDirection = 3; // Right
        else if (normMovement.x < 0) lastDirection = 1; // Left
        else if (normMovement.y > 0) lastDirection = 2; // Up
        else if (normMovement.y < 0) lastDirection = 0; // Down

        // Update the Direction in Animator
        animator.SetInteger("Direction", lastDirection);
    }


    private IEnumerator Attack()
    { 
        isAttacking = true;
        lastAttackTime = Time.time;
        hitEnemies = new HashSet<GameObject>(); // Initialize the set for this attack
        animator.SetBool("isAttacking", true);

        isAttacking = true;
        animator.SetBool("isAttacking", true);

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
