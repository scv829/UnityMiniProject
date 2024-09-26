using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class AttackRange : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField] Transform attackTarget;
    [SerializeField] GameObject attackPrefab;
    [SerializeField] float attackDamage;
    [SerializeField] float attackSpeed;
    private Coroutine attackCoroutine;

    private void Awake()
    {
        attackCoroutine = null;
    }

    private void Update()
    {
        Attack();
    }

    private void Attack()
    {
        // 공격 범위에 적이 들어와 있는데 공격을 안하고 있다?
        if (attackTarget != null && attackCoroutine == null)
        {
            // 공격 시작
            Debug.Log("공격 시작");
            attackCoroutine = StartCoroutine(attacking());
        }
        // 공격 범위에서 나갔는데 공격을 하고 있다?
        else if(attackTarget == null && attackCoroutine != null)
        {
            Debug.Log("공격 정지!");
            // 공격을 멈추기
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }
    }

    private IEnumerator attacking()
    {
        // 공격 범위안에 들어온 친구를 타겟으로
        while(true)
        {
            GameObject instance = Instantiate(attackPrefab, transform.parent.transform.position, Quaternion.identity);
            // 공격 속도에 비례해서 공격을 한다(=> 프리펩을 날린다)
            instance.GetComponent<AttackObejct>().SetTarget(attackTarget);
            yield return new WaitForSeconds(attackSpeed);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy") && attackTarget == null )
        {
            Debug.Log("Attack");
            attackTarget = other.transform;
        }
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy") && attackTarget == null)
        {
            Debug.Log("stayAttack");
            attackTarget = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.Equals(attackTarget.gameObject))
        {
            attackTarget = null;
        }
    }
}
