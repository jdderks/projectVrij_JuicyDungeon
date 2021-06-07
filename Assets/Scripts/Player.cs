using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour, IDamageable
{
    [ProgressBar("Current Health", "currentHealth", EColor.Red), SerializeField] private float health = 100;

    [SerializeField] private float arrowSpawnHeight = 1.2f;

    [SerializeField] private ScriptableArrowObject arrowObject;


    private float currentHealth = 100f;

    void Update()
    {
        Shoot();
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
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
            GameObject arrGO = new GameObject("Arrow");
            Arrow arr = arrGO.AddComponent<Arrow>();
            arr.Setup(this.gameObject, transform.position + new Vector3(0, arrowSpawnHeight, 0), hitPosition, arrowObject);
        }
    }
}
