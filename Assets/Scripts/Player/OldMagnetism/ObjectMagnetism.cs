using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Outline))]
[RequireComponent(typeof(Rigidbody))]
public class ObjectMagnetism : MagnetismAbstract
{
    private PlayerMagnetism playerMagnetism = null;

    private Outline outline = null;

    [SerializeField] private float forceImpulse = 5f;
    [SerializeField] private float forceRepulse = 5f;
    [SerializeField] private float setDefaultTime = 2f;

    private Transform centerPointTransform = null;

    [SerializeField]
    private Collider colliderObject = null;

    [SerializeField]
    private Collider triggerObject = null;

    private bool isUsedAttract = false;
    private bool isUsedRepulse = true;

    private bool isMeteore = false;

    private LayerMask layerMask;

    private Rigidbody objectRb;
    private Vector3 startPos;
    private Quaternion startRot;

    [SerializeField]
    private Material materialTransparent;
    private Material materialMetalic;
    private Renderer rendererObject;


    private void Start()
    {
        layerMask = LayerMask.GetMask("Platform");
        outline = GetComponent<Outline>();
        outline.enabled = false;
        centerPointTransform = transform.GetChild(0);
        startPos = transform.position;
        startRot = transform.rotation;
        objectRb = GetComponent<Rigidbody>();

        rendererObject = GetComponent<Renderer>();
        materialMetalic = rendererObject.material;
    }
    // Magnetism Mode : 0 = Null, 4 = AttractObject, 5 = RepulseObject
    public override int GetMagnetismActionAttract(out float _forceImpulse, out float _setDefaultTime, out Transform _transform)
    {
        IsAttracted();
        _forceImpulse = forceImpulse;
        _transform = centerPointTransform;
        _setDefaultTime = setDefaultTime;
        return 4;
    }

    public override int GetMagnetismActionRepulse(out float _forceRepulse, out float _setDefaultTime, out Transform _transform)
    {
        IsRepulsed();
        _forceRepulse = forceRepulse;
        _transform = centerPointTransform;
        _setDefaultTime = setDefaultTime;
        return 5;
    }

    private IEnumerator CoroutineReset()
    {
        yield return new WaitForSeconds(setDefaultTime);
        SetDefault();
    }

    public override void SetDefault()
    {
        transform.rotation = startRot;
        transform.position = startPos;

        objectRb.isKinematic = true;
        objectRb.useGravity = false;

        if (!isUsedRepulse)
        {
            colliderObject.isTrigger = false;
            triggerObject.enabled = true;
            StopAllCoroutines();
            MagnetismPicked(false);
            transform.SetParent(null);
        }

        isUsedAttract = false;
        isUsedRepulse = true;

        isMeteore = false;


    }

    public override bool GetAttractState() => isUsedAttract;
    public override bool GetRepulseState() => isUsedRepulse;
    public override Vector3 GetTransformPosition() => centerPointTransform.transform.position;

    public override void IsAttracted()
    {
        isUsedAttract = true;
        isUsedRepulse = false;
        objectRb.isKinematic = false;
        colliderObject.isTrigger = true;
        triggerObject.enabled = false;
    }
    public override void IsRepulsed()
    {
        isUsedRepulse = true;
        colliderObject.isTrigger = false;
        triggerObject.enabled = true;
        objectRb.isKinematic = false;
        objectRb.useGravity = true;
        isMeteore = true;
        rendererObject.material = materialMetalic;
        StartCoroutine("CoroutineReset");
    }


    public override void MagnetismPicked(bool _isPicked)
    {
        outline.enabled = _isPicked;
        IsPicked = _isPicked;
    }

    public override void MagnetismOutlineColor(Color outlineColor)
    {
        outline.OutlineColor = outlineColor;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (!isMeteore)
            return;

        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<Enemy>().SetDamage();
            isMeteore = false;
        }

        //Number 8 is equal to layer "Platform" and 0 to default

        if (collision.gameObject.layer == 8 || collision.gameObject.layer == 0)
            isMeteore = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!colliderObject.isTrigger && other.tag == "Player")
        {
            if (isUsedAttract || !ActiveMagnetism)
                return;

            Vector3 posMagnetism = centerPointTransform.transform.position;
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

        //Number 8 is equal to layer "Platform"
        if (colliderObject.isTrigger && other.gameObject.layer == 8)
        {
            isUsedRepulse = true;
            rendererObject.material = materialTransparent;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!colliderObject.isTrigger && other.tag == "Player")
        {
            if (IsPicked)
            {
                playerMagnetism.DesactiveMagnetism();
                MagnetismPicked(false);
            }
        }

        if (colliderObject.isTrigger && other.tag == "Platform")
        {
            isUsedRepulse = false;
            rendererObject.material = materialMetalic;
        }
    }
}
