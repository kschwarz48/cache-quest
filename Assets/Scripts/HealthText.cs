using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthText : MonoBehaviour
{
    private TMPro.TextMeshProUGUI healthText;
    private Health health;

    public float timeToLive = 3f;

    float timeElapsed = 0;

    void Start()
    {
        healthText = GetComponent<TMPro.TextMeshProUGUI>();
        health = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
        health.OnHealthChanged += HandleHealthChanged;
        HandleHealthChanged(health.CurrentHealth);
    }

    void Update()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed >= timeToLive)
        {
            Destroy(gameObject);
        }
    }
    private void HandleHealthChanged(int currentHealth)
    {
        healthText.text = "Health: " + currentHealth;
    }

    void OnDestroy()
    {
        health.OnHealthChanged -= HandleHealthChanged;
    }
}
