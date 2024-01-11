using UnityEngine;

public class PlayerHealth : Health
{
    protected override void Die()
    {
        base.Die(); // Include base death logic
        // Player-specific death logic
        Debug.Log("Player died!");
        // Handle player death (e.g., respawn, game over)
    }
}
