using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private Transform lineRenderer = null;

    [SerializeField]
    private float intervalTime = 2f;

    [SerializeField]
    private float timeChargeAttack = 0.2f;
    
    [SerializeField]
    private float timeAttack = 1f;

    [SerializeField]
    private float timeToStartAttack = 0;

    private bool laserActive = false;
    private bool laserIsCharged = false;
    private LayerMask layerMask;

    private float laserDist = 0;

    [SerializeField]
    private float maxDist = 300;

    [SerializeField]
    private Vector3 axis = Vector3.zero;

    [SerializeField]
    private GameObject collisionParticles = null;

    [SerializeField]
    private GameObject chargeParticles = null;


    private void Start()
    {
        collisionParticles.SetActive(false);
        layerMask = LayerMask.GetMask("Platform", "Magnetism");
        lineRenderer.localScale = Vector3.zero;
        chargeParticles.SetActive(false);
        StartCoroutine("coroutineStartAttack");
    }

    private IEnumerator coroutineStartAttack()
    {
        yield return new WaitForSeconds(timeToStartAttack);
        StartCoroutine("coroutineNewAttack");

    }

    private IEnumerator coroutineNewAttack()
    {
        yield return new WaitForSeconds(intervalTime);
        StartCoroutine("coroutineAttack");
        laserIsCharged = false;

        if (intervalTime != 0)
        {
            StartCoroutine("CoroutineChargeLaser");
        }
        else
        {
            laserIsCharged = true;
            collisionParticles.SetActive(true);
        }

        laserActive = true;
    }

    private IEnumerator coroutineAttack()
    {
        yield return new WaitForSeconds(timeAttack);
        lineRenderer.localScale = new Vector3(1, 1, 0);
        StartCoroutine("coroutineNewAttack");
        collisionParticles.SetActive(false);
        laserActive = false;
    }

    private float GetDistanceLaser()
    {
        RaycastHit raycastHit;
        if (Physics.Raycast(lineRenderer.position, axis, out raycastHit, maxDist, layerMask))
        {
            Debug.Log(raycastHit.collider.name);
            return raycastHit.distance;
        }

        return maxDist;
    }

    private IEnumerator CoroutineChargeLaser()
    {
        chargeParticles.SetActive(true);
        
        yield return new WaitForSeconds(timeChargeAttack);
        chargeParticles.SetActive(false);
        collisionParticles.SetActive(true);
        laserIsCharged = true;
    }

    private void Update()
    {
        if (laserActive && laserIsCharged)
        {
            laserDist = GetDistanceLaser();
            Vector3 curVelocity = Vector3.zero;
            laserDist = Vector3.SmoothDamp(new Vector3(0,0,lineRenderer.localScale.z), new Vector3(0, 0, laserDist), ref curVelocity, 0.05f).z;
            lineRenderer.localScale = new Vector3(1, 1, laserDist);

            collisionParticles.transform.localPosition = new Vector3(collisionParticles.transform.localPosition.x, -laserDist, collisionParticles.transform.localPosition.z);

            RaycastHit raycastHit;
            if (Physics.Raycast(transform.position, axis, out raycastHit, layerMask))
            {
                if (raycastHit.collider.tag == "Player")
                {
                    raycastHit.collider.GetComponent<Player>().Damaged();
                }
            }
        }
    }
}
