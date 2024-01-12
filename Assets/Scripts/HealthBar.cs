using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image healthBarFiller;
    public PlayerHealth playerHealth;

    void Update()
    {
        float healthPercent = (float)playerHealth.CurrentHealth / playerHealth.MaxHealth;
        healthBarFiller.fillAmount = healthPercent;
    }
}

