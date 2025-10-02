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

            //replace with enum ref
            if (gameObject.GetComponentInParent<BossBattle1>().isBoss1)
            {
                GameManager.Instance.isBoss1Dead = true;
            }
            if (gameObject.GetComponentInParent<BossBattle1>().isBoss2)
            {
                GameManager.Instance.isBoss2Dead = true;
            }
            boss.EndBattle();
        }

        bossHealthSlider.value = currentHealth;
    }
}
