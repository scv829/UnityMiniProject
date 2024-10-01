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

        // 만약 적일 경우
        if(target.CompareTag("Enemy"))
        {
            // 해당 적이 오브젝트 풀로 돌아갔을 때 발생할 함수 추가
            target.GetComponent<Enemy>().dieEvent.AddListener(Remove);
        }
    }

    public void Remove()
    {
        Destroy(gameObject);
    }

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // 적: 타겟이 없을 때
        // 플레이어: 날아가는 도중 타겟이 비활성화 될 때
        if (target == null)
        {
            Remove();
        }
        else
        {
            transform.LookAt(target);
            transform.position = Vector3.MoveTowards(transform.position, target.position, 0.03f);
        }
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
