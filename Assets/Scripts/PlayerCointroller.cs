using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class PlayerCointroller : MonoBehaviour, IHit
{
    [Header("Property")]
    [SerializeField] float moveSpeed;
    [SerializeField] float runSpeed;
    [SerializeField] float rotateSpeed;
    [SerializeField] float hp;
    [SerializeField] float maxHp;
    [SerializeField] bool isDead;
    [SerializeField] Animator animator;

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
    [SerializeField] float attackCoolTime;
    [SerializeField] float currentAttackCoolTime;
    [SerializeField] Vector3 targetPosition;

    [Header("Die")]
    [SerializeField] GameObject dieEffect;

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
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        // hp 바 설정
        hpBar.maxValue = maxHp;
        hpBar.value = hp;
        hpBar.gameObject.SetActive(false);

        // 공격 속도에 따른 애니메이션 배속 설정
        attackCoolTime = 1f / attackSpeed;

        if (attackSpeed > 1) animator.SetFloat("AttackSpeed", attackSpeed);
        else animator.SetFloat("AttackSpeed", 1);


        targetScope.gameObject.SetActive(false);
    }

    private void Update()
    {
        Move();
        Rotate();
        Attack();
        if (isDead) 
        { 
            Debug.Log("PlayerDead"); 
            Destroy(gameObject);

            GameObject obj = Instantiate(dieEffect);
            obj.transform.position = transform.position;
            Destroy(obj, 2f);
        }

    }

    private void Move()
    {
        float z = Input.GetAxis("Vertical");

        float speed = (Input.GetButton("Jump") && hp == maxHp)  ? runSpeed : moveSpeed;

        transform.Translate(Vector3.forward * z * speed * Time.deltaTime);

        hpBar.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, transform.localScale.y + offset, 0));
        animator.SetFloat("Speed", speed * z);
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
            animator.SetTrigger("AttackTrigger");
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
            // 만약 공격 쿨타임이 돌았으면
            if (currentAttackCoolTime >= attackCoolTime)
            {
                // 쿨타임 초기화
                currentAttackCoolTime = 0f;
                // 공격 개시
                animator.SetTrigger("AttackTrigger");
                // 화살 프리팹으로 공격
                GameObject instance = Instantiate(attackPrefab, muzzlePoint.position, Quaternion.identity);
                instance.GetComponent<AttackObejct>().Setting(attackArea.Target, attackDamage);
            }

            currentAttackCoolTime += Time.deltaTime;
            yield return null;
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
