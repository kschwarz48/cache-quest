using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    public Collider2D attackHitboxUp;
    public Collider2D attackHitboxDown;
    public Collider2D attackHitboxLeft;
    public Collider2D attackHitboxRight;
    private Vector2 currentAttackDirection;
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
        DisableAllHitboxes(); 
    }

    void Update()
    {
        if (!isRolling)  
        {
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");
        }

        // Flip the character's sprite based on movement direction
        if (movement.x > 0)
        {
            spriteRenderer.flipX = false; // Face right
        }
        else if (movement.x < 0)
        {
            spriteRenderer.flipX = true; // Face left
        }

        // Set the Speed parameter to simulate movement for debugging
        float simulatedSpeed = movement.sqrMagnitude > 0 ? 0.5f : 0f;
        animator.SetFloat("Speed", simulatedSpeed);

        if (Input.GetKeyDown(KeyCode.Space) && !isRolling && movement != Vector2.zero)
        {
            Debug.Log("Spacebar pressed and not rolling or moving.");
            StartCoroutine(Roll());
        }

        if (Input.GetMouseButtonDown(0) && !isRolling && !isAttacking)  
        {
            StartCoroutine(Attack());
        }
    }

    private IEnumerator Attack()
    { 
        isAttacking = true;
        animator.SetBool("isAttacking", true);

        currentAttackDirection = DetermineAttackDirection();
        EnableCorrectHitbox(currentAttackDirection);

        yield return new WaitForSeconds(0.5f); 

        DisableAllHitboxes();
        animator.SetBool("isAttacking", false);
        isAttacking = false;
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
        if (!isAttacking) 
        {
            // Ignore collision as we're not attacking
            return;
        }

        Debug.Log($"Hit: {collision.gameObject.name} with {currentAttackDirection} attack");

        if (collision.CompareTag("Enemy"))
        {
            Debug.Log("Attacked an enemy!");
            // Add logic to handle enemy collision here
        }
        else if (collision.CompareTag("Destructible"))
        {
            Debug.Log("Hit a destructible object!");
            // Add logic to handle destructible object collision here
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
        // Determine the attack direction based on the player's current movement direction
        if (Mathf.Abs(movement.x) > Mathf.Abs(movement.y))
        {
            return movement.x > 0 ? Vector2.right : Vector2.left;
        }
        else
        {
            return movement.y > 0 ? Vector2.up : Vector2.down;
        }
    }
}
