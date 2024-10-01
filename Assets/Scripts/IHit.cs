using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHit
{
    public void TakeDamage(float damage);
    public Transform HitPoint();
}
