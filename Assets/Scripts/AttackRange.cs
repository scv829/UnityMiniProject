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
        if (attackTarget != null && attackCoroutine == null)
        {
            Debug.Log("Attack Start");
            attackCoroutine = StartCoroutine(attacking());
        }
        else if(attackTarget == null && attackCoroutine != null)
        {
            Debug.Log("Attack Stop!");
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }
    }

    private IEnumerator attacking()
    {
        while(true)
        {
            GameObject instance = Instantiate(attackPrefab, transform.parent.transform.position, Quaternion.identity);
            instance.GetComponent<AttackObejct>().SetTarget(attackTarget);
            yield return new WaitForSeconds(attackSpeed);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy") && attackTarget == null )
        {
            attackTarget = other.transform;
        }
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy") && attackTarget == null)
        {
            attackTarget = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(attackTarget != null && other.gameObject.Equals(attackTarget.gameObject))
        {
            attackTarget = null;
        }
    }
}
