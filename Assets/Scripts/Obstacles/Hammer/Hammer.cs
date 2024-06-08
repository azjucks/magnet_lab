using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Hammer : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField]
    private float force = 50f;

    private bool isUsed = false;

    private Vector3 startPos;
    private Quaternion startRotation;

    private void OnEnable()
    {
        Player.PlayerDeathEvent += SetDefault;
    }

    private void OnDisable()
    {
        Player.PlayerDeathEvent -= SetDefault;
    }

    private void SetDefault()
    {
        rb.isKinematic = true;

        transform.position = startPos;
        transform.rotation = startRotation;


        isUsed = false;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.isKinematic = true;

        startPos = transform.position;
        startRotation = transform.rotation;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (isUsed)
            return;

        if (other.tag == "Player")
        {
            rb.isKinematic = false;
            rb.AddTorque(Vector3.left * force, ForceMode.Impulse);
            isUsed = true;
        }
    }
}
