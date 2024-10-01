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
        // 공격 피사체의 맞는 부분
        this.target = target.gameObject.GetComponent<IHit>().HitPoint();
        // 공격 데미지
        this.damage = damage;

        // 적이 날린 투사체 -> 플레이어만
        // 플레이어가 날린 투사체 -> 적만
        gameObject.layer = target.CompareTag("Enemy") ? 8 : 9;
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
