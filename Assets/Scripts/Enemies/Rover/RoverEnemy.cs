using System.Collections;
using UnityEngine;


public class RoverEnemy : EnemyAbstract
{
    [SerializeField] 
    private Animator animator = null;
    [SerializeField]
    private float stopTimeChangeDir = 0.3f;
    [SerializeField]
    private Transform rendederRover = null;

    LayerMask layerMask;

    private EnemyMovements em;


    private int moveDirBuffer = 0;
    private bool triggerBuffer = false;

    private bool isEnabled;

    public bool IsEnabled
    {
        get { return isEnabled; }
        set { isEnabled = value; }
    }

    private Vector3 startingPos;

    void Start()
    {
        source = GetComponent<AudioSource>();
        layerMask = LayerMask.GetMask("Platform");
        //Rigidbody rb = GetComponent<Rigidbody>();
        //rb.constraints = RigidbodyConstraints.FreezePositionY;

        startingPos = transform.position;
        em = GetComponent<EnemyMovements>();
        isEnabled = true;
    }

    public override void SetDefault()
    {
        transform.position = startingPos;
        IsEnabled = true;
    }

    public override void Disable()
    {
        IsEnabled = false;
        em.Disable();
    }

    public override void Trigger(Transform t)
    {
        em.Triggered = true;
        em.TargetT = t;
    }

    public override void Triggered(Transform t)
    {
        if (!isEnabled)
            return;

        em.TargetPt = t.position;
        Vector3 targetPos = new Vector3(t.position.x, transform.position.y, transform.position.z);

        Vector3 vecRcastLeft = new Vector3(transform.position.x - 0.5f, transform.position.y -0.7f, transform.position.z);
        Vector3 vecRcastRight = new Vector3(transform.position.x + 0.5f, transform.position.y -0.7f, transform.position.z);

        Debug.DrawRay(vecRcastLeft, Vector3.down, Color.red);
        Debug.DrawRay(vecRcastRight, Vector3.down, Color.red);

        if (Physics.Raycast(vecRcastLeft, Vector3.down, 1f, layerMask) && Physics.Raycast(vecRcastRight, Vector3.down, 1f, layerMask))
            em.GoToTargetPt(targetPos);
        else
            em.GoToTargetPt(transform.position);
    }

    public override void Untrigger()
    {
        em.Triggered = false;
        em.TargetPt = em.StartingPos;
        em.CurrentPtIdx = em.path.Count;
    }

    public override void AttackPlayer(Transform t)
    {
        if (!isEnabled)
            return;

        t.GetComponent<Player>().Damaged();
    }

    private IEnumerator CoroutineStopWalk()
    {
        yield return new WaitForSeconds(stopTimeChangeDir);
        em.IsMove = true;
    }

    private void Update()
    {

        if (moveDirBuffer != em.MoveDir)
        {


            if (em.MoveDir < 0)
                rendederRover.eulerAngles = new Vector3(0, 270, 0);
            else
                rendederRover.eulerAngles = new Vector3(0, 90, 0);

            if (!em.Triggered)
            {

                animator.SetBool("changeDir", true);

                if (em.Triggered != triggerBuffer)
                {
                    em.IsMove = false;
                    StartCoroutine("CoroutineStopWalk");
                }
            }
        }
        else
        {
                animator.SetBool("changeDir", false);
        }

        moveDirBuffer = em.MoveDir;

        if (triggerBuffer != em.Triggered)
        {
            em.IsMove = false;
            StartCoroutine("CoroutineStopWalk");
            animator.SetBool("isTrigger", em.Triggered);
        }

        triggerBuffer = em.Triggered;
    }
}
