using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour, IDamageable
{
    [ProgressBar("Current Health", "currentHealth", EColor.Red), SerializeField] 
    private float health = 100;

    private float currentHealth = 100f;

    void Start()
    {
        
    }


    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log(health);
        if (health < 0)
        {
            Die();
        }
    }

    public void Die()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
