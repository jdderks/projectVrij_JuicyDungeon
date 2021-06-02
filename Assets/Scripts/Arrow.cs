using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    float rayCheckLength;

    private bool hasHitTarget = false;

    [SerializeField] private int Damage = 100;

    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float speed = 0.01f;

    private Vector3 prevPos;
    private Vector3 currPos;
    private void Start()
    {
        prevPos = transform.position;
    }


    void Update()
    {
        currPos = this.transform.position;
        Ray ray = new Ray(currPos, prevPos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, enemyLayer))
        {
            Debug.Log("Yeet");
        }
        prevPos = currPos;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<IDamageable>() != null)
        {
            if (other.gameObject.layer == enemyLayer)
            {
                other.GetComponent<IDamageable>().TakeDamage(Damage);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(currPos, prevPos);
    }
}