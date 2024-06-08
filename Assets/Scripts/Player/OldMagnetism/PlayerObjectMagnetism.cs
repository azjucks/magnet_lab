using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Outline))]
[RequireComponent(typeof(SphereCollider))]
public class PlayerObjectMagnetism : MagnetismAbstract
{
    private PlayerMagnetism playerMagnetism = null;

    private Outline outline = null;

    [Header("Settings")]
    [SerializeField] private float forceImpulse = 5f;
    [SerializeField] private float forceRepulse = 5f;
    [SerializeField] private float setDefaultTime = 2f;
    [SerializeField] private float setDefaultDepthTime = 3f;

    private Vector3 startPos;

    private Transform centerPointTransform = null;

    private bool isUsedAttract = false;
    private bool isUsedRepulse = false;

    [SerializeField]
    private bool isDepthPlaform = false;

    private LayerMask layerMask;

    private bool startReset = false;
    private bool isDepthPlatformBuffer = false;

    void Start()
    {
        layerMask = LayerMask.GetMask("Platform");
        outline = GetComponent<Outline>();
        outline.enabled = false;
        centerPointTransform = transform.GetChild(0);

        if (isDepthPlaform) 
        {
            isUsedRepulse = true;
            startPos = transform.position;
            isDepthPlatformBuffer = isDepthPlaform;
        }
    }

    public override void SetDefault()
    {
        if (!isDepthPlatformBuffer)
        {
            isUsedAttract = false;
            isUsedRepulse = false;
        }
        else
        {
            SetDefaultDepthPlatform();
        }

        StopAllCoroutines();
    }

    public void SetDefaultDepthPlatform()
    {
        isUsedAttract = false;
        isUsedRepulse = true;
        transform.position = startPos;
        isDepthPlaform = true;
    }


    // Magnetism Mode : 0 = Null, 1 = AttractPlayerObject, 2 = RepulsePlayerObject, 6 = PlatformDepth
    public override int GetMagnetismActionAttract(out float _forceImpulse, out float _setDefaultTime, out Transform _transform)
    {
        IsAttracted();
        _forceImpulse = forceImpulse;
        _transform = centerPointTransform;
        _setDefaultTime = setDefaultTime;

        if (isDepthPlaform)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
            isDepthPlaform = false;

            if (setDefaultDepthTime != 0)
                StartCoroutine("coroutineResetDepthPlatform");
            return 6;
        }

        return 1;
    }

    public override int GetMagnetismActionRepulse(out float _forceRepulse, out float _setDefaultTime, out Transform _transform)
    {
        IsRepulsed();
        _forceRepulse = forceRepulse;
        _transform = centerPointTransform;
        _setDefaultTime = setDefaultTime;
        return 2;
    }

    public override bool GetAttractState() => isUsedAttract;
    public override bool GetRepulseState() => isUsedRepulse;
    public override Vector3 GetTransformPosition() => centerPointTransform.transform.position;

    public override void MagnetismPicked(bool _isPicked)
    {
        outline.enabled = _isPicked;
        IsPicked = _isPicked;
    }
    


    public override void MagnetismOutlineColor(Color outlineColor)
    {
        outline.OutlineColor = outlineColor;
    }

    private IEnumerator coroutineReset()
    {
        yield return new WaitForSeconds(setDefaultTime);
        if (!isDepthPlaform)
        {
            isUsedAttract = false;
            isUsedRepulse = false;
            startReset = false;
        }
    }

    private IEnumerator coroutineResetDepthPlatform()
    {
        yield return new WaitForSeconds(setDefaultDepthTime);
        SetDefaultDepthPlatform();
        startReset = false;
    }

    public override void IsAttracted()
    {
        if (!startReset)
        {
            StartCoroutine("coroutineReset");
            startReset = true;
        }

        isUsedAttract = true;
    }

    public override void IsRepulsed()
    {
        if (!startReset)
        {
            StartCoroutine("coroutineReset");
            startReset = true;
        }

        isUsedRepulse = true;

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            if ((isUsedAttract && isUsedRepulse) || !ActiveMagnetism)
                return;

            Vector3 posMagnetism = centerPointTransform.position;
            RaycastHit hit;
            float dist = Vector3.Distance(posMagnetism, other.transform.position);
            
            Debug.DrawRay(posMagnetism, (other.transform.position - posMagnetism).normalized * dist, Color.green);
            if (Physics.Raycast(posMagnetism, (other.transform.position - posMagnetism).normalized,
                out hit, dist, layerMask))
            {
                Debug.DrawLine(posMagnetism, hit.point, Color.red);
                if (hit.collider.tag != "Player")
                {
                    return;
                }
            }

            playerMagnetism = other.GetComponent<PlayerMagnetism>();
            playerMagnetism.NewMagnetismObject(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            if (IsPicked)
            {
               //Debug.Log("Exit Object");
               playerMagnetism.DesactiveMagnetism();
               MagnetismPicked(false);
            }
        }
    }
}
