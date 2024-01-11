using UnityEngine;

public class EnemyHealth : Health
{

    private bool canTakeDamage = true;
    private float damageCooldown = 1f; // Half a second cooldown, adjust as needed

    public bool CanTakeDamage => canTakeDamage;

    public void TriggerDamageCooldown()
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
        base.Die(); // Include base death logic
        // Enemy-specific death logic
        Debug.Log("Enemy died!");
        // Handle enemy death (e.g., drop loot)
        Destroy(gameObject);
    }
}
