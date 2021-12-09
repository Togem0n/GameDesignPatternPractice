using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SetHealthText : MonoBehaviour
{

    private TextMeshProUGUI healthText;
    private PlayerHealth playerHealth;

    private void Start()
    {
        healthText = transform.GetComponent<TextMeshProUGUI>();
        playerHealth = GameObject.FindWithTag("Player").GetComponent<PlayerHealth>();
        HealthBarEvents.current.onHealthDown += SetHealthUI;

        healthText.SetText("Current Health: " + playerHealth.MaxHealth.ToString());

    }


    private void SetHealthUI()
    {
        healthText.SetText("Current Health: " + playerHealth.CurrHealth.ToString());
    }
}
