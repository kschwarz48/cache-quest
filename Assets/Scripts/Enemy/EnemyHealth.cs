using UnityEngine;
using System.Collections;

public class EnemyHealth : Health
{
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Animator animator;
    public float knockbackStrength = 10f;
    protected override void Awake()
    {
        base.Awake(); // Call the base class Awake method
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        animator.SetBool("isAlive", true);
    }
    public override void TakeDamage(int damage, Vector2 knockbackDirection)
    {
        base.TakeDamage(damage); // This updates CurrentHealth in the base class.
        animator.SetTrigger("hit");
        Debug.Log($"Enemy took {damage} damage!");
        rb.AddForce(knockbackDirection.normalized * knockbackStrength, ForceMode2D.Impulse);
        Debug.Log($"Current Health: {CurrentHealth}");

        // Check if health depletes and manage death here if not handled in base class.
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
