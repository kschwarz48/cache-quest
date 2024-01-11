using UnityEngine;
using System.Collections;


public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

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
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

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

        if (Input.GetKeyDown(KeyCode.Space) && !isRolling)
        {
            Debug.Log("Spacebar pressed and not rolling.");
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

        // Wait for the animation to finish
        yield return new WaitForSeconds(0.5f);

        animator.SetBool("isAttacking", false);
        isAttacking = false;
    }


    private IEnumerator Roll()
    {
        Debug.Log("Coroutine started. rollSpeedMultiplier: " + rollSpeedMultiplier);
        isRolling = true;
        animator.SetTrigger("RollNow");
        Debug.Log("RollNow trigger set. Original speed: " + moveSpeed);

        moveSpeed *= rollSpeedMultiplier;  // Or directly use a hardcoded value for testing
        Debug.Log("After multiplication, speed: " + moveSpeed);

        yield return new WaitForSeconds(rollDuration);

        moveSpeed = originalSpeed;
        isRolling = false;
        Debug.Log("Coroutine ended. Speed reset to: " + moveSpeed);
    }


    void FixedUpdate()
    {
        Debug.Log("FixedUpdate: current speed: " + moveSpeed);
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

}
