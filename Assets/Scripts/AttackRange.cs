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
        // ���� ������ ���� ���� �ִµ� ������ ���ϰ� �ִ�?
        if (attackTarget != null && attackCoroutine == null)
        {
            // ���� ����
            Debug.Log("���� ����");
            attackCoroutine = StartCoroutine(attacking());
        }
        // ���� �������� �����µ� ������ �ϰ� �ִ�?
        else if(attackTarget == null && attackCoroutine != null)
        {
            Debug.Log("���� ����!");
            // ������ ���߱�
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }
    }

    private IEnumerator attacking()
    {
        // ���� �����ȿ� ���� ģ���� Ÿ������
        while(true)
        {
            GameObject instance = Instantiate(attackPrefab, transform.parent.transform.position, Quaternion.identity);
            // ���� �ӵ��� ����ؼ� ������ �Ѵ�(=> �������� ������)
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
