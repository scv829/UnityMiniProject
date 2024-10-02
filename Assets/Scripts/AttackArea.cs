using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackArea : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField] Transform target;
    [SerializeField] SphereCollider col;

    [Header("Target_LayerMask")]
    [SerializeField] LayerMask targetLayerMask;

    public Transform Target { get { return target; } }
    public float Redius { get {return col.radius; } set { col.radius = value; } }

    private void Awake()
    {
        col = GetComponent<SphereCollider>();
    }

    public void ResetTarget()
    {
        target = null;
    }

    private void SetEvent()
    {
        if (target.gameObject.GetComponent<Enemy>() != null)
        {
            target.gameObject.GetComponent<Enemy>().dieEvent.AddListener(ResetTarget);
        }
    }

    private void ResetEvent()
    {
        if (target.gameObject.GetComponent<Enemy>() != null)
        {
            target.gameObject.GetComponent<Enemy>().dieEvent.RemoveListener(ResetTarget);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if ( ((targetLayerMask & (1 << other.gameObject.layer)) != 0) && target == null)
        {
            target = other.transform;
            SetEvent();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if ( ( (targetLayerMask & (1 << other.gameObject.layer) ) != 0) 
            && target == null)
        {
            ResetEvent();
            target = other.transform;
            SetEvent();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (target != null && other.gameObject.Equals(target.gameObject))
        {
            ResetTarget();
            target = null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, transform.GetComponent<SphereCollider>().radius);
    }

}
