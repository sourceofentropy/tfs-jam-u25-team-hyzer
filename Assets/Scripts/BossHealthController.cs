using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthController : MonoBehaviour
{
    public static BossHealthController instance;

    public Slider bossHealthSlider;
    public int currentHealth = 30;
    public int maxHealth = 30;
    public BossBattle1 boss;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        bossHealthSlider.maxValue = maxHealth;
        bossHealthSlider.value = maxHealth;
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;

        if(currentHealth <= 0)
        {
            currentHealth = 0;

            boss.EndBattle();
        }

        bossHealthSlider.value = currentHealth;
    }
}
