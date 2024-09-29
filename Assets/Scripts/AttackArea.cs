using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackArea : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField] Transform target;

    [Header("Target_LayerMask")]
    [SerializeField] LayerMask targetLayerMask;

    public Transform Target { get { return target; } }


    private void OnTriggerEnter(Collider other)
    {
        if ( ((targetLayerMask & (1 << other.gameObject.layer)) != 0) && target == null)
        {
            target = other.transform;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if ( ( (targetLayerMask & (1 << other.gameObject.layer) ) != 0) 
            && target == null)
        {
            target = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (target != null && other.gameObject.Equals(target.gameObject))
        {
            target = null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, transform.GetComponent<SphereCollider>().radius);
    }

}
