using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{

    private GameObject sender;
    private Rigidbody rigidbody;
    private BoxCollider collider;

    private GameObject prefabObject;

    private int damage;
    private Vector3 position;
    private float force;
    

    private LayerMask detectionLayer;

    private float rayCheckLength;

    private bool hasHitTarget = false;


    private Vector3 prevPos;
    private Vector3 currPos;

    public void Setup(GameObject sender, Vector3 position, Vector3 targetPos, ScriptableArrowObject arrowObject)
    {
        this.sender = sender;
        this.prefabObject = arrowObject.prefabObject;

        this.position = position;
        this.force = arrowObject.force;
        this.detectionLayer = arrowObject.detectionLayer;
        this.damage = arrowObject.damage;

        collider = gameObject.AddComponent<BoxCollider>();
        rigidbody = gameObject.AddComponent<Rigidbody>();
        collider.isTrigger = true;
        rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rigidbody.useGravity = false;
        transform.position = position;
        prevPos = transform.position;

        transform.LookAt(targetPos);

        rigidbody.AddForce(transform.forward * force);

        Destroy(gameObject, 10);
        Instantiate(prefabObject, transform);
    }
    

    void Update()
    {
        currPos = this.transform.position;
        Ray ray = new Ray(currPos, prevPos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, detectionLayer))
        {
            //Debug.Log("Yeet");
        }
        prevPos = currPos;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == sender) return;
        other.GetComponent<IDamageable>()?.TakeDamage(damage);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(currPos, prevPos);
    }
}