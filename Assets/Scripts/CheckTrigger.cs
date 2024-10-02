using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckTrigger : MonoBehaviour
{
    [SerializeField] TutorialScript script;
    [SerializeField] int n;

    private void OnTriggerEnter(Collider other)
    {
        script.ShowCommnet(n);
    }

    private void OnTriggerExit(Collider other)
    {
        Destroy(gameObject);
    }
}
