using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackObejct : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float moveSpeed;
    [SerializeField] float damage;

    public void Setting(Transform target, float damage)
    {
        this.target = target;
        this.damage = damage;

        gameObject.layer = target.CompareTag("Enemy") ? 8 : 9;
    }

    private void Update()
    {
        if(target == null) Destroy(gameObject);
        transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<IHit>() is IHit)
        {
            collision.gameObject.GetComponent<IHit>().TakeDamage(damage);
        }
        Destroy(gameObject);
    }
}
