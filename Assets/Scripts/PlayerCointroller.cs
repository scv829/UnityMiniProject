using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCointroller : MonoBehaviour
{
    [Header("Property")]
    [SerializeField] float moveSpeed;
    [SerializeField] float runSpeed;
    [SerializeField] float rotateSpeed;
    [SerializeField] int hp;
    [SerializeField] int maxHp;
    [SerializeField] bool isDead;

    public void TakeDamage(int damage)
    {
        hp -= damage;
        if (hp <= 0) isDead = true;
    }

    private void Update()
    {
        Move();
        Rotate();
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
}
