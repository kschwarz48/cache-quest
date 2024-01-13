using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    [SerializeField]
    private int maxHealth = 100;

    private int currentHealth;

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
        currentHealth -= amount;
        OnHealthChanged?.Invoke(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
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
        // Basic death logic (can be overridden in subclasses)
    }
}
