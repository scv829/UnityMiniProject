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

    private void Update()
    {
        Move();
        Rotate();
    }

    private void Move()
    {
        float z = Input.GetAxis("Vertical");

        float speed = (Input.GetButton("Jump") && hp == maxHp)  ? moveSpeed * 2f : moveSpeed;

        transform.Translate(Vector3.forward * z * speed * Time.deltaTime);
    }

    private void Rotate()
    {
        float x = Input.GetAxis("Horizontal");

        transform.Rotate(Vector3.up * x * rotateSpeed * Time.deltaTime);
    }
}
