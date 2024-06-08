using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstablePlatform : MonoBehaviour
{
    [SerializeField]
    private List<Collider> colliders = new List<Collider>();


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            foreach (Collider collider in colliders)
            {
                collider.enabled = false;
            }
        }
    }
}
