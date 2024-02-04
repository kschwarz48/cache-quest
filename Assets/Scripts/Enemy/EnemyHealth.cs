using UnityEngine;
using System.Collections;

public class EnemyHealth : Health
{
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Animator animator;

    private EnemyController enemyController;
    public float knockbackStrength = 5000f;
    protected override void Awake()
    {
        base.Awake(); // Call the base class Awake method
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        enemyController = GetComponent<EnemyController>(); // Get the EnemyController
        animator.SetBool("isAlive", true);
    }
     public override void TakeDamage(int damage, Vector2 knockbackDirection)
    {
        if (CurrentHealth <= 0) return; // Prevent further damage if already dead

        base.TakeDamage(damage);
        animator.SetTrigger("hit");
        Debug.Log($"Enemy took {damage} damage!");

        // Apply knockback
        rb.AddForce(knockbackDirection.normalized * knockbackStrength, ForceMode2D.Impulse);
        if (enemyController != null)
        {
            enemyController.HandleKnockback(); // Tell the enemy controller to handle knockback
        }

        // Check for death
        if (CurrentHealth <= 0)
        {
            animator.SetBool("isAlive", false);
            Die();
        }
    }

    protected override void Die()
    {   
        base.Die();
        Debug.Log("Enemy died!");
        // Consider delaying destruction to allow animation to play.
        StartCoroutine(DestroyAfterDelay(1.0f)); // Example delay.
    }

    IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

}
