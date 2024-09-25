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
    [SerializeField] float attackDamage;
    [SerializeField] float attackSpeed;
    [SerializeField] int hp;

    private void Update()
    {
        Move();
        Rotate();
        
    }

    private void Move()
    {
        float z = Input.GetAxis("Vertical");

        transform.Translate(Vector3.forward * z * moveSpeed * Time.deltaTime);
    }

    private void Rotate()
    {
        float x = Input.GetAxis("Horizontal");

        transform.Rotate(Vector3.up * x * rotateSpeed * Time.deltaTime);
    }
}
