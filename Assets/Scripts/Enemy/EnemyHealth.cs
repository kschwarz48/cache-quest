using UnityEngine;
using System.Collections;

public class EnemyHealth : Health
{
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    public float knockbackStrength = 10f;
    public float blinkDuration = 0.1f;
    private bool canTakeDamage = true;
    private float damageCooldown = 1f;

    protected override void Awake()
    {
        base.Awake(); // Call the base class Awake method
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    public override void TakeDamage(int damage, Vector2 knockbackDirection)
    {
        if (!canTakeDamage) return;

        base.TakeDamage(damage);

        // Check if knockbackDirection is significant
        Debug.Log($"Knockback Direction: {knockbackDirection}, Force: {knockbackDirection.normalized * knockbackStrength}");

        rb.AddForce(knockbackDirection.normalized * knockbackStrength, ForceMode2D.Impulse);

        // Start the blink coroutine
        StartCoroutine(BlinkEffect());

        TriggerDamageCooldown(); 
    }

    private IEnumerator BlinkEffect()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(blinkDuration);
        spriteRenderer.color = Color.white;
    }

    private void TriggerDamageCooldown()
    {
        canTakeDamage = false;
        Invoke(nameof(ResetDamageCooldown), damageCooldown);
    }

    private void ResetDamageCooldown()
    {
        canTakeDamage = true;
    }

    protected override void Die()
    {
        base.Die();
        Debug.Log("Enemy died!");
        Destroy(gameObject);
    }
}
