using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    [SerializeField] private Image healthBarFill;

    private void OnEnable()
    {
        PlayerHealth.OnHealthChanged += UpdateHealthBar;
    }

    private void OnDisable()
    {
        PlayerHealth.OnHealthChanged -= UpdateHealthBar;
    }

    private void UpdateHealthBar(float healthPercentage)
    {
        healthBarFill.fillAmount = healthPercentage;
    }
}
