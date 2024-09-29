using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField] Slider hpbar;
    [SerializeField] float offset;

    private float y_offset;

    private void Awake()
    {
        y_offset = transform.localScale.y + offset;
    }

    private void Update()
    {
        hpbar.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, transform.localScale.y + offset, 0));
    }

}
