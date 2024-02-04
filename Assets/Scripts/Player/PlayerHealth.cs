using UnityEngine;
using System.Collections;

public class PlayerHealth : Health
{
    [SerializeField] 
    private bool isAlive = true;
    private Rigidbody2D rb;
    private Animator animator;
    public float knockbackStrength = 5000f;

    protected override void Awake()
    {
        base.Awake(); // Call the base class Awake method
        isAlive = true; // The player starts alive
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        animator.SetBool("isAlive", true);
    }

    public override void TakeDamage(int damage, Vector2 knockbackDirection = default)
    {
        if (!isAlive) return; // Don't take damage if already dead

        base.TakeDamage(damage); // Deduct health
        Debug.Log($"Player took {damage} damage! Current Health: {CurrentHealth}");

        if (CurrentHealth <= 0)
        {
            animator.SetBool("isAlive", false);
            PlayerController.Instance.LockMovement(); // Prevent player from moving
            Die(); // Trigger death logic
        }
        else
        {
            rb.AddForce(knockbackDirection.normalized * knockbackStrength, ForceMode2D.Force);
        }
    }

    protected override void Die()
    {
        if (!isAlive) return; // Prevent Die from running multiple times

        isAlive = false;
        
        Debug.Log("Player died!");
        // Call the GameManager to respawn the player after a delay
        // This delay allows the death animation to play out
        Invoke(nameof(RespawnPlayer), 2f); // Adjust delay as needed
    }

    IEnumerator RespawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Make sure this delay is long enough for the animation to play
        RespawnPlayer();
    }

    void RespawnPlayer()
    {
        base.ResetHealth();
        GameManager.Instance.RespawnPlayerAtSpawnPoint("InitalSpawnPoint");
 
        // Reset animation state
        animator.SetBool("isAlive", true);
        animator.Play("Player_Idle"); // Or any default animation state
        
        // Re-enable any components disabled during death
        GetComponent<PlayerController>().enabled = true;
        
        Debug.Log("Player respawned.");
        Debug.Log($"Current Health: {CurrentHealth}");
    }

}
