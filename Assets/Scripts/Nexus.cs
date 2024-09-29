using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nexus : MonoBehaviour, IHit
{
    [SerializeField] float hp;

    public void TakeDamage(float damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            Debug.Log("Gameover");
            Time.timeScale = 0f;
            Destroy(gameObject);
        }
    }
}
