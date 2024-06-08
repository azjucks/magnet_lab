using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMagnetism : MonoBehaviour
{
    private MagnetismAbstract magnetismObject = null;
    private Transform magnetismObjectCenter = null;

    private MagnetismAbstract magnetismOldObject = null;

    [SerializeField] private Transform directionTransform;

    [Header("Outline Color")]
    [SerializeField] private Color outlineColorAttract = Color.blue;
    [SerializeField] private Color outlineColorRepulse = Color.red;
    [SerializeField] private Color outlineColorAll = Color.magenta;

    //Parameter for magnetism Action
    private float forceImpulse;
    private float forceRepulse;
    private float setDefaultTime;

    private bool objectAttractTrigger = false;
    private bool objectRepulseTrigger = false;

    private bool isMeteore = false;

    private Rigidbody playerRb;

    private LayerMask layerMask;

    private Inputs inputs;

    private PlayerMovements playerMovements;

    private bool actionMagnetism = false;

    public bool IsMeteore
    {
        get { return isMeteore; }
    }

    private enum MagnetismMode
    {
        Null,
        PlayerObjectAttractAttach,
        PlayerObjectRepulse,
        PlayerObjectAttrack,
        ObjectAttract,
        ObjectRepulse,
        PlatformDepth
    }

    MagnetismMode magnetismMode = 0;

    public float MagnestismMode
    {
        get { return (float)magnetismMode; }
    }

    private void Start()
    {
        inputs = GetComponent<Inputs>();
        layerMask = LayerMask.GetMask("Platform");
        playerRb = GetComponent<Rigidbody>();
        playerMovements = GetComponent<PlayerMovements>();
    }

    public void DesactiveMagnetismAlternatePlatform()
    {
        actionMagnetism = false;
        if (magnetismMode == MagnetismMode.PlayerObjectAttractAttach)
        {
            magnetismMode = MagnetismMode.Null;
            playerMovements.enabled = true;
            playerRb.velocity = Vector3.zero;
        }
    }

    public void DesactiveMagnetism()
    {
        if (magnetismMode == MagnetismMode.Null)
            actionMagnetism = false;
    }

    private void OnEnable()
    {
        Player.PlayerDeathEvent += SetDefault;
    }

    private void OnDisable()
    {
        Player.PlayerDeathEvent -= SetDefault;
    }

    public void SetDefault()
    {
        magnetismMode = MagnetismMode.Null;
        
        if (magnetismObject != null)
            magnetismObject = null;

        if (magnetismOldObject != null)
            magnetismOldObject = null;

        actionMagnetism = false;
        playerMovements.enabled = true;

        playerRb.velocity = Vector3.zero;
    }

    public void NewMagnetismObject(MagnetismAbstract objectTrigger)
    {
        if (magnetismObject == null && magnetismOldObject != objectTrigger)
        {
            ReplaceMagnestismObject(objectTrigger);
        }
        else if (magnetismObject == null || magnetismMode == MagnetismMode.PlayerObjectAttractAttach)
        {
            return;
        }
        else if (Vector3.Distance(transform.position, objectTrigger.transform.position) <=
                Vector3.Distance(transform.position, magnetismObject.transform.position))
        {
            magnetismObject.MagnetismPicked(false);
            ReplaceMagnestismObject(objectTrigger);
        }

    }

    public void ReplaceMagnestismObject(MagnetismAbstract objectTrigger)
    {
        if (magnetismMode != MagnetismMode.PlayerObjectAttractAttach && magnetismMode != MagnetismMode.ObjectAttract)
        {
            magnetismObject = objectTrigger;
            SetTriggerMagnestism(!magnetismObject.GetAttractState(), !magnetismObject.GetRepulseState());
            if (objectAttractTrigger && objectRepulseTrigger)
                magnetismObject.MagnetismOutlineColor(outlineColorAll);
            else if (objectAttractTrigger)
                magnetismObject.MagnetismOutlineColor(outlineColorAttract);
            else
                magnetismObject.MagnetismOutlineColor(outlineColorRepulse);
        }
        else if (magnetismMode == MagnetismMode.ObjectAttract)
        {
            magnetismObject = objectTrigger;
            SetTriggerMagnestism(!magnetismObject.GetAttractState(), true);
            magnetismObject.MagnetismOutlineColor(outlineColorAttract);
        }
        else
        {
            if (magnetismObject != objectTrigger)
            {
               if (objectTrigger.GetAttractState())
                   return;

                magnetismObject = objectTrigger;
                magnetismObject.MagnetismOutlineColor(outlineColorAttract);

                if (magnetismOldObject != null)
                    SetTriggerMagnestism(!magnetismObject.GetAttractState(), !magnetismObject.GetRepulseState());
                else
                    SetTriggerMagnestism(!magnetismObject.GetAttractState(), !magnetismOldObject.GetRepulseState());
            }

        }
            magnetismObject.MagnetismPicked(true);
            actionMagnetism = true;

    }

    void SetTriggerMagnestism(bool attract, bool repulse)
    {
        objectAttractTrigger = attract;
        objectRepulseTrigger = repulse;
    }

    void RaycastHitOtherObject()
    {
        if (magnetismObject == null)
            return;

        Vector3 posMagnetism = magnetismObject.GetTransformPosition();
        RaycastHit hit;
        float dist = Vector3.Distance(posMagnetism, transform.position);

        Debug.DrawRay(transform.position, (posMagnetism - transform.position).normalized * dist, Color.green);
        if (Physics.Raycast(transform.position, (posMagnetism - transform.position).normalized, 
            out hit, dist, layerMask))
        {
            Debug.DrawLine(posMagnetism, hit.point, Color.red);

            if (hit.collider.tag != "Player")
            {
                objectAttractTrigger = false;
                objectRepulseTrigger = false;
                magnetismObject.MagnetismPicked(false);
                magnetismObject = null;
            }
        }
    }

    void DirectionTransform()
    {
        Vector2 axis = (inputs.GetAxisDirection().normalized * 2f) + new Vector2(transform.position.x, transform.position.y);
        directionTransform.position = new Vector3(axis.x,axis.y, 0);
    }

    void AttractTrigger()
    {
        if (objectAttractTrigger && inputs.AttractKey)
        {
            if (magnetismMode == MagnetismMode.ObjectAttract)
            {
                magnetismOldObject.IsRepulsed();
                magnetismOldObject.transform.SetParent(null);
                magnetismOldObject.MagnetismPicked(false);
                magnetismOldObject = null;
            }

            if (magnetismObject == null)
                return;

            magnetismMode = (MagnetismMode)magnetismObject.GetMagnetismActionAttract(out forceImpulse, out setDefaultTime, out magnetismObjectCenter);
            objectAttractTrigger = false;

            if (magnetismOldObject != null)
                magnetismOldObject.MagnetismPicked(false);
            
            Vector2 axisDir = inputs.GetAxisDirection();
            if (axisDir.x == 0 && axisDir.y == 0 && magnetismMode == MagnetismMode.PlayerObjectAttractAttach)
            {
                magnetismOldObject = magnetismObject;
                magnetismObject = null;

                objectRepulseTrigger = !magnetismOldObject.GetRepulseState();

                if (objectRepulseTrigger)
                    magnetismOldObject.MagnetismOutlineColor(outlineColorRepulse);
                else
                    magnetismOldObject.MagnetismPicked(false);

                playerMovements.enabled = false;

                IEnumerator enumerator = playerObjectAttractTime(magnetismObjectCenter);
                StartCoroutine(enumerator);
            }
            else if (magnetismMode == MagnetismMode.PlayerObjectAttractAttach)
            {
                magnetismMode = MagnetismMode.PlayerObjectAttrack;
                if (objectRepulseTrigger)
                {
                    magnetismObject.MagnetismOutlineColor(outlineColorRepulse);
                }
                else
                {
                    magnetismObject.MagnetismPicked(false);
                }

                objectAttractTrigger = false;
                //playerMovements.enabled = false;
            }
            else if (magnetismMode == MagnetismMode.PlatformDepth)
            {
                objectAttractTrigger = false;
                magnetismObject.MagnetismPicked(false);
                magnetismObject = null;
            }
            else
            {
                magnetismOldObject = magnetismObject;
                magnetismObject = null;
                magnetismOldObject.MagnetismOutlineColor(outlineColorRepulse);
                magnetismOldObject.transform.SetParent(transform);
                objectRepulseTrigger = true;
            }

        }
    }

    void RepulseTrigger()
    {
       //if (objectRepulseTrigger && inputs.RepulseKeyDown)
       //{
       //    if (magnetismMode == MagnetismMode.PlayerObjectAttractAttach)
       //    {
       //        magnetismMode = (MagnetismMode)magnetismOldObject.GetMagnetismActionRepulse(out forceRepulse, out setDefaultTime, out magnetismObjectCenter);
       //        magnetismOldObject.MagnetismPicked(false);
       //
       //        isMeteore = true;
       //    }
       //    else if (magnetismMode == MagnetismMode.ObjectAttract)
       //    {
       //        if (magnetismOldObject.GetRepulseState())
       //            return;
       //
       //        magnetismMode = (MagnetismMode)magnetismOldObject.GetMagnetismActionRepulse(out forceRepulse, out setDefaultTime, out magnetismObjectCenter);
       //        magnetismOldObject.MagnetismPicked(false);
       //    }
       //    else
       //    {
       //        magnetismMode = (MagnetismMode)magnetismObject.GetMagnetismActionRepulse(out forceRepulse, out setDefaultTime, out magnetismObjectCenter);
       //
       //            if (!objectAttractTrigger)
       //            {
       //                magnetismObject.MagnetismPicked(false);
       //            }
       //
       //        isMeteore = true;
       //    }
       //
       //    objectRepulseTrigger = false;
       //    playerMovements.enabled = true;
       //}
    }

    void Update()
    {

        DirectionTransform();

        if (magnetismObject != null && !magnetismObject.ActiveMagnetism)
            magnetismObject = null;
       
       if (magnetismOldObject != null && !magnetismOldObject.ActiveMagnetism)
           magnetismOldObject = null;

        if (!actionMagnetism)
            return;

        if (magnetismMode != MagnetismMode.PlayerObjectAttractAttach)
            playerMovements.enabled = true;

        RaycastHitOtherObject();
        AttractTrigger();
        RepulseTrigger();
        

       //if (inputs.JumpKey && magnetismMode == MagnetismMode.PlayerObjectAttractAttach)
       //{
       //   magnetismMode = MagnetismMode.Null;
       //   magnetismOldObject.MagnetismPicked(false);
       //   playerMovements.enabled = true;
       //}

        switch (magnetismMode)
        {
            case MagnetismMode.Null:
                break;
            case MagnetismMode.PlayerObjectAttractAttach:
                PlayerObjectAttractAttach();
                break;
            case MagnetismMode.PlayerObjectRepulse:
                PlayerObjectPush();
                break;
            case MagnetismMode.PlayerObjectAttrack:
                PlayerObjectAttract();
                break;
            case MagnetismMode.ObjectAttract:
                ObjectAttract();
                break;
            case MagnetismMode.ObjectRepulse:
                ObjectRepulse();
                break;
        }


    }

    private IEnumerator playerObjectAttractTime(Transform transformMagnetObj)
    {
        yield return new WaitForSeconds(setDefaultTime);

        if (magnetismObjectCenter == transformMagnetObj)
        {
            magnetismObjectCenter = null;
            magnetismOldObject = null;
            playerMovements.enabled = true;
            if (magnetismMode == MagnetismMode.PlayerObjectAttractAttach)
            {
                magnetismMode = MagnetismMode.Null;
            }
        }

    }

    private void PlayerObjectAttractAttach()
    {
        if (magnetismObjectCenter == null)
        {
            magnetismMode = MagnetismMode.Null;
            return;
        }

        //Not optimized but that it works
        Vector3 axisJump = (magnetismObjectCenter.position - playerRb.transform.position).normalized;
        playerRb.velocity = new Vector3(0, 0, 0);
        playerRb.AddForce(axisJump * forceImpulse, ForceMode.Impulse);

    }

    private void PlayerObjectAttract()
    {
        if (magnetismObjectCenter == null)
        {
            magnetismMode = MagnetismMode.Null;
            return;
        }

        Vector2 axisDir = inputs.GetAxisDirection();

        Vector3 axisJump = ((magnetismObjectCenter.position - playerRb.transform.position).normalized 
            + new Vector3(axisDir.x, axisDir.y, 0).normalized).normalized;

        playerRb.velocity = new Vector3(0, 0, 0);
        playerRb.AddForce(axisJump * forceImpulse, ForceMode.Impulse);

        magnetismObjectCenter = null;

    }

    private void GetValueRounded(ref float value)
    {
        if (value > 0)
            value = 1;
        else if (value < 0)
            value = -1;
        else
            value = 0;
    }

    private void PlayerObjectPush()
    {
        Vector3 axisDir = inputs.GetAxisDirection().normalized;
        
        Vector3 axisJump;

        if (magnetismOldObject == null)
        {
            Vector3 playerObject = (playerRb.transform.position - magnetismObjectCenter.transform.position).normalized;
            GetValueRounded(ref playerObject.x);
            GetValueRounded(ref playerObject.y);
            axisJump = (playerObject + axisDir).normalized;
            Debug.Log("Axis Jump");
        }
        else
        {
            if (axisDir.x == 0 && axisDir.y == 0)
            {
                axisJump = (playerRb.transform.position - magnetismObjectCenter.transform.position).normalized;
                Debug.Log("Axis Base");
            }
            else
            {
                axisJump = axisDir;
                Debug.Log("Axis Dir");
            }
            Debug.Log(axisDir);
            magnetismOldObject = null;

        }


        playerRb.velocity = Vector3.zero;
        playerRb.AddForce( axisJump * forceRepulse, ForceMode.Impulse);
        magnetismMode = MagnetismMode.Null;
    }
    
    private void ObjectAttract()
    {
        //TODO : player.x Movements if -x or +x  rotation!
        Vector2 axisDir = inputs.GetAxisDirection().normalized * 2f;
        if (axisDir.x == 0 && axisDir.y == 0)
            axisDir.x = 2;

        Vector3 childPos = magnetismOldObject.transform.GetChild(0).localPosition;
        Vector3 desiredPos = new Vector3(axisDir.x, axisDir.y, 0) - new Vector3(childPos.x, childPos.y, childPos.z);
        magnetismOldObject.transform.localPosition = Vector3.Lerp(magnetismOldObject.transform.localPosition, desiredPos, 0.5f);
    }

    private void ObjectRepulse()
    {
        magnetismOldObject.transform.SetParent(null);
        Rigidbody objectRb = magnetismOldObject.GetComponent<Rigidbody>();
        Vector3 axisDir = inputs.GetAxisDirection().normalized;
        if (axisDir.x == 0 && axisDir.y == 0)
            axisDir.x = 1;
        objectRb.AddForce(axisDir * forceRepulse, ForceMode.Impulse);
        magnetismMode = MagnetismMode.Null;
        magnetismOldObject = null;
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
}
