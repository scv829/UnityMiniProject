using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCointroller : MonoBehaviour, IHit
{
    [Header("Property")]
    [SerializeField] float moveSpeed;
    [SerializeField] float runSpeed;
    [SerializeField] float rotateSpeed;
    [SerializeField] float hp;
    [SerializeField] float maxHp;
    [SerializeField] bool isDead;

    [Header("Attack")]
    [SerializeField] AttackArea attackArea;
    [SerializeField] float attackRange;
    [SerializeField] GameObject attackPrefab;
    [SerializeField] float attackDamage;
    [SerializeField] float attackSpeed;
    [SerializeField] Vector3 targetPosition;

    private Coroutine attackCoroutine;

    public void TakeDamage(int damage)
    {
        hp -= damage;
        if (hp <= 0) isDead = true;
    }

    private void Awake()
    {
        attackCoroutine = null;
    }

    private void Update()
    {
        Move();
        Rotate();
        Attack();
        if (isDead) { Debug.Log("PlayerDead"); Destroy(gameObject); }
    }

    private void Move()
    {
        float z = Input.GetAxis("Vertical");

        float speed = (Input.GetButton("Jump") && hp == maxHp)  ? runSpeed : moveSpeed;

        transform.Translate(Vector3.forward * z * speed * Time.deltaTime);
    }

    private void Rotate()
    {
        float x = Input.GetAxis("Horizontal");

        transform.Rotate(Vector3.up * x * rotateSpeed * Time.deltaTime);
    }

    private void Attack()
    {
        if (attackArea.Target != null && attackCoroutine == null)
        {
            Debug.Log("Attack Start");
            attackCoroutine = StartCoroutine(attacking());
        }
        else if (attackArea.Target == null && attackCoroutine != null)
        {
            Debug.Log("Attack Stop!");
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }
    }

    private IEnumerator attacking()
    {
        while (true)
        {
            GameObject instance = Instantiate(attackPrefab, transform.position, Quaternion.identity);
            instance.GetComponent<AttackObejct>().Setting(attackArea.Target, attackDamage);
            yield return new WaitForSeconds(attackSpeed);
        }
    }

    public void TakeDamage(float damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            isDead = true;
        }
    }
}
