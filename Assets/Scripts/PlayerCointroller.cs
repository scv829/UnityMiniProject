using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCointroller : MonoBehaviour, IHit
{
    [Header("Property")]
    [SerializeField] float moveSpeed;
    [SerializeField] float runSpeed;
    [SerializeField] float rotateSpeed;
    [SerializeField] float hp;
    [SerializeField] float maxHp;
    [SerializeField] bool isDead;

    [Header("UI")]
    [SerializeField] Slider hpBar;
    [SerializeField] Image targetScope;
    [SerializeField] float offset;

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
        hp = maxHp;
    }

    private void Start()
    {
        hpBar.maxValue = maxHp;
        hpBar.value = hp;
        hpBar.gameObject.SetActive(false);
        targetScope.gameObject.SetActive(false);
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

        hpBar.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, transform.localScale.y + offset, 0));
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
            targetScope.gameObject.SetActive(true);
            Debug.Log("Attack Start");
            attackCoroutine = StartCoroutine(attacking());
        }
        else if (attackArea.Target == null && attackCoroutine != null)
        {
            targetScope.gameObject.SetActive(false);
            Debug.Log("Attack Stop!");
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }
        
        if(attackArea.Target != null)
            targetScope.transform.position = Camera.main.WorldToScreenPoint(attackArea.Target.position);
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
        hpBar.gameObject.SetActive(true);
        hp -= damage;
        if (hp <= 0)
        {
            isDead = true;
        }
        hpBar.value = hp;
    }
}
