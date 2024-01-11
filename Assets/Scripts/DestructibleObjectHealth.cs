using UnityEngine;

public class DestructibleObjectHealth : Health
{
    protected override void Die()
    {
        base.Die(); // Include base death logic
        // Destructible object-specific death logic
        Debug.Log("Destructible Object destroyed!");
        // Handle destruction (e.g., play animation, disable object)
    }
}
