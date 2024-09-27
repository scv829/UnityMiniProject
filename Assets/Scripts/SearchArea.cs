using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchArea : MonoBehaviour
{
    [Header("Search")]
    [SerializeField] Transform target;

    public Transform Target { get { return target; } }

    private void OnTriggerEnter(Collider other)
    {
        if ( (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Nexus")) && target == null)
        {
            target = other.transform;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if ((other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Nexus")) && target == null)
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
        Gizmos.color =  Color.green;
        Gizmos.DrawWireSphere(transform.position, transform.GetComponent<SphereCollider>().radius);
    }
}
