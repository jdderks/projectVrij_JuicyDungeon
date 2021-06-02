using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour, IDamageable
{
    [ProgressBar("Current Health", "currentHealth", EColor.Red), SerializeField] private float health = 100;
    [SerializeField] private GameObject arrow;
    [SerializeField] private float arrowSpeed = 5f;

    [SerializeField] private float arrowSpawnHeight = 1.2f;

    private float currentHealth = 100f;



    void Start()
    {
        
    }


    void Update()
    {
        Shoot();
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

    public void Shoot()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        Vector3 hitPosition = Vector3.zero;
        if (Physics.Raycast(ray, out hit, 100))
        {
            hitPosition = new Vector3(hit.point.x, arrowSpawnHeight, hit.point.z);
            Debug.DrawLine(transform.position, hitPosition);
        }

        if (Input.GetMouseButton(1))
        {
            GameObject projectile = Instantiate(arrow, transform.position + new Vector3(0,arrowSpawnHeight,0), Quaternion.identity);
            projectile.transform.LookAt(hitPosition);
            projectile.GetComponent<Rigidbody>().AddForce(projectile.transform.forward * arrowSpeed);
            Destroy(projectile, 10);
        }
    }
}
