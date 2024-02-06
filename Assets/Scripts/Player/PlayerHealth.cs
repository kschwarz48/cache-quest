using UnityEngine;
using System.Collections;
using System;

public class PlayerHealth : Health
{
    [SerializeField] 
    private bool isAlive = true;
    private Rigidbody2D rb;
    private Animator animator;
    public float knockbackStrength = 5000f;
    private SpriteRenderer spriteRenderer;
    public static event Action<float> OnHealthChanged;

    protected override void Awake()
    {
        base.Awake(); 
        isAlive = true; 
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        animator.SetBool("isAlive", true);
        spriteRenderer = GetComponent<SpriteRenderer>(); // Ensure this line is present and correct

    }

    public override void TakeDamage(int damage, Vector2 knockbackDirection = default)
    {
        if (!isAlive || isInvincible) return; 

        base.TakeDamage(damage); 

        // Broadcast the health change
        float healthPercentage = (float)CurrentHealth / MaxHealth;
        OnHealthChanged?.Invoke(healthPercentage); 
        Debug.Log($"Player took {damage} damage! Current Health: {CurrentHealth}");

        if (CurrentHealth <= 0)
        {
            animator.SetBool("isAlive", false);
            PlayerController.Instance.LockMovement(); 
            Die(); 
        }
        else
        {
            rb.AddForce(knockbackDirection.normalized * knockbackStrength, ForceMode2D.Force);
            StartCoroutine(BlinkEffect()); 
        }
    }
    private IEnumerator BlinkEffect()
    {
        Color originalColor = spriteRenderer.color; // Store the original color
        spriteRenderer.color = Color.red; // Change color to red to indicate damage
        yield return new WaitForSeconds(0.1f); // Wait for 0.1 second
        spriteRenderer.color = originalColor; // Revert to the original color
    }
    protected override void Die()
    {
        if (!isAlive) return;
        isAlive = false;
        Debug.Log("Player died!");
        Invoke(nameof(RespawnPlayer), 2f); 
    }

    IEnumerator RespawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); 
        RespawnPlayer();
    }

    void RespawnPlayer()
    {
        base.ResetHealth();
        float healthPercentage = (float)CurrentHealth / MaxHealth;
        OnHealthChanged?.Invoke(healthPercentage);
        GameManager.Instance.RespawnPlayerAtSpawnPoint("InitalSpawnPoint");
        animator.SetBool("isAlive", true);
        animator.Play("Player_Idle"); 
        GetComponent<PlayerController>().enabled = true;
        Debug.Log("Player respawned.");
        Debug.Log($"Current Health: {CurrentHealth}");
    }

}
