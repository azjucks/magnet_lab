using System.Collections;
using UnityEngine;

public class DroneEnemy : EnemyAbstract
{
    private EnemyMovements em;

    private Vector3 startingPos;

    [SerializeField] private SphereCollider triggerZone;
    [SerializeField] private Animator animator;

    [Header("Settings")]
    [SerializeField] private int forceImpulse = 2;
    [SerializeField] private float forceDash = 1.0f;
    [SerializeField] private float angle = 30;
    [SerializeField] private float timeBeforeDash = 1.0f;
    [SerializeField] private float disableTime = 2.0f;


    private float epsilon = 0.1f;

    private float maxDashDist;
    private Rigidbody rb;
    private bool pTrigger = false;

    private bool onCharge = false;
    private bool disabled = false;

    private bool Disabled
    {
        get { return disabled; }
        set
        {
            if (value == false && disabled == true && pTrigger == false)
            {
                em.Triggered = false;
                em.TargetPt = em.StartingPos;
                em.CurrentPtIdx = em.path.Count;
            }
            disabled = value;

        }
    }

    Transform pTransform;
    Vector3 pPos;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        startingPos = transform.position;
        em = GetComponent<EnemyMovements>();
        rb = GetComponent<Rigidbody>();
        maxDashDist = triggerZone.radius;
        pTransform = transform;
        PlaySound(Sounds.IDLE);
    }

    public override void Disable()
    {
        Debug.Log("A drone is disabled");
        pPos = startingPos;
        StopCoroutine("EnableDrone");
        disabled = true;
    }

    public override void SetDefault()
    {
        transform.position = startingPos;
        
        StopAllCoroutines();

        pTrigger = false;
        onCharge = false;
        rb.isKinematic = true;
        disabled = false;
    }

    private IEnumerator CoroutineDash()
    {
        yield return new WaitForSeconds(timeBeforeDash);

        rb.isKinematic = false;

        Vector3 dir = (pPos - transform.position).normalized;

        float dist = Vector3.Distance(transform.position, pPos);

        rb.AddForce(dir * forceDash * dist / maxDashDist, ForceMode.Impulse);

        onCharge = false;
        disabled = true;
        animator.SetBool("Idle", false);
        StartCoroutine("EnableDrone");
    }

    public override void Trigger(Transform t)
    {
        pTrigger = true;
        em.Triggered = true;
        if (!onCharge && !disabled)
        {
            pTransform = t;
            pPos = t.position;
        }
    }

    public override void Triggered(Transform t)
    {
        if (!onCharge && !disabled)
        {
            Debug.Log("Disabled:" + disabled);
            StartCoroutine("CoroutineDash");

            onCharge = true;
        }
    }

    public override void Untrigger()
    {
        pTrigger = false;
    }

    private IEnumerator EnableDrone()
    {
        yield return new WaitForSeconds(disableTime);

        pPos = pTransform.position;
        rb.isKinematic = true;
        Disabled = false;
        animator.SetBool("Idle", true);
        PlaySound(Sounds.IDLE);
    }

    private Vector3 GetPushVec(Vector3 v)
    {
        float side = Mathf.Sign(transform.position.x - v.x);

        Vector3 vec = side == -1 ? Vector3.right : Vector3.left;

        Vector3 toReturn = (Quaternion.Euler(new Vector3(0, 0, angle * side * -1)) * vec).normalized;

        return toReturn;
    }

    public override void AttackPlayer(Transform t)
    {
        if (Disabled)
            return;
        PlaySound(Sounds.ATTACK);
        Rigidbody playerRb = t.GetComponent<Rigidbody>();
        Vector3 axisKnockback = GetPushVec(t.position);

        playerRb.velocity = new Vector3(0, 0, 0);
        GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);

        playerRb.AddForce(axisKnockback * forceImpulse, ForceMode.Impulse);
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, pPos) <= epsilon)
        {
            rb.velocity = Vector3.zero;
        }
    }

}
