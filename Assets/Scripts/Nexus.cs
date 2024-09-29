using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Nexus : MonoBehaviour, IHit
{
    [SerializeField] float hp;
    [Header("UI")]
    [SerializeField] Slider hpBar;
    [SerializeField] float offset;

    private void Start()
    {
        hpBar.maxValue = hp;
        hpBar.value = hp;
        hpBar.gameObject.SetActive(false);
    }

    private void Update()
    {
        hpBar.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, transform.localScale.y + offset, 0));
    }

    public void TakeDamage(float damage)
    {
        hpBar.gameObject.SetActive(true);
        hp -= damage;
        if (hp <= 0)
        {
            Debug.Log("Gameover");
            Time.timeScale = 0f;
            Destroy(gameObject);
        }
        hpBar.value = hp;
    }
}
