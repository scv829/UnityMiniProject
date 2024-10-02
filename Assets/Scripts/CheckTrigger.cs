using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckTrigger : MonoBehaviour
{
    [SerializeField] TutorialScript script;

    private void OnTriggerEnter(Collider other)
    {
        script.ShowCommnet();
    }

    private void OnTriggerExit(Collider other)
    {
        Destroy(gameObject);
    }
}
