using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public LayerMask collisionMask;
    public float bulletSpeed=10f;
    public float damage = 1;
    float lifeTime = 3f;
    float skinWidth = 0.1f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
        Collider[] initialcollisions = Physics.OverlapSphere(transform.position, 0.1f, collisionMask);
        if (initialcollisions.Length > 0)
        {
            OnHitObject(initialcollisions[0],transform.position);
        }
    }
    public void SetSpeed(float newSpeed)
    {
        bulletSpeed = newSpeed;
    }
    void Update()
    {
        float moveDistance = bulletSpeed * Time.deltaTime;
        CheckCollision(moveDistance);
        transform.Translate(Vector3.forward *moveDistance);
    }

    void CheckCollision(float moveDistance)
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, moveDistance+skinWidth, collisionMask, QueryTriggerInteraction.Collide))
        {
           // Debug.Log(hit.point);
            OnHitObject(hit.collider,hit.point);
        }
    }

    void OnHitObject(Collider c, Vector3 hitPoint)
    {
        IDamageable damageableObject = c.gameObject.GetComponent<IDamageable>();
        if (damageableObject != null)
        {
            damageableObject.TakeHit(damage,hitPoint,transform.forward);
        }
        GameObject.Destroy(gameObject);
    }
}
