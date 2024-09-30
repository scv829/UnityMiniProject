using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackObejct : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float moveSpeed;
    [SerializeField] float damage;
    [SerializeField] Rigidbody rigid;

    public void Setting(Transform target, float damage)
    {
        this.target = target;
        this.damage = damage;

        gameObject.layer = target.CompareTag("Enemy") ? 8 : 9;
        rigid.velocity = Vector3.up * moveSpeed;
    }

    public void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if(target == null) Destroy(gameObject);

        transform.position += transform.up * moveSpeed * Time.deltaTime;

        Vector3 dir = (target.position - transform.position).normalized;
        transform.up = Vector3.Lerp(transform.up, dir, 0.25f);
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
