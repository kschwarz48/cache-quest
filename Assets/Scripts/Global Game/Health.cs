using UnityEngine;
using System;
using System.Collections;

public class Health : MonoBehaviour
{
    [SerializeField]
    private int maxHealth = 200;
    private int currentHealth;

    [SerializeField]
    private float invincibilityDuration = 0.5f; // Duration of invincibility after taking damage
    public bool isInvincible = false; // Tracks whether the invincibility effect is currently active
    public bool enableInvincibility = true; // Allows control over whether invincibility should be applied

    public event Action<int> OnHealthChanged;
    public event Action OnDeath;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    public virtual void TakeDamage(int amount, Vector2 knockbackDirection = default(Vector2))
    {
        if (isInvincible || !enableInvincibility) return; // Skip taking damage if currently invincible or invincibility is disabled

        currentHealth -= amount;
        OnHealthChanged?.Invoke(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
        else if (enableInvincibility)
        {
            StartCoroutine(ActivateInvincibility());
        }
    }

    private IEnumerator ActivateInvincibility()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        OnHealthChanged?.Invoke(currentHealth);
    }

    protected virtual void Die()
    {
        OnDeath?.Invoke();
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(maxHealth);
    }
}
