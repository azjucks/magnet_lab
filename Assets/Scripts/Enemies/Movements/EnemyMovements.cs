using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovements : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 startingPos;
    double epsilon = 0.1;
    [SerializeField] private bool triggered;

    [SerializeField] private float speed = 1.0f;
    [SerializeField] public List<Transform> path;

    private Vector3 targetPt;
    private int currentPtIdx;
    private Transform targetT;

    private bool isMove = true;
    private int moveDir = 0;

    private void OnEnable()
    {
        Player.PlayerDeathEvent += SetDefault;
    }

    private void OnDisable()
    {
        Player.PlayerDeathEvent -= SetDefault;
    }

    void SetDefault()
    {
        isMove = true;
        triggered = false;
    }

    public void Disable()
    {
        isMove = false;
    }

    public bool IsMove
    {
        get { return isMove; }
        set { isMove = value; }
    }

    public int MoveDir
    {
        get { return moveDir; }
    }

    public Vector3 StartingPos
    {
        get { return startingPos; }
    }

    public int CurrentPtIdx
    {
        get { return currentPtIdx; }

        set { currentPtIdx = value; }
    }

    public Vector3 TargetPt
    {
        get { return targetPt; }

        set { targetPt = value; }
    }

    public bool Triggered
    {
        get { return triggered; }

        set { triggered = value; }
    }

    public Transform TargetT
    {
        get { return targetT; }

        set { targetT = value; }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        currentPtIdx = 0;
        if (path.Count != 0)
            targetPt = path[0].position;
        triggered = false;
        startingPos = rb.position;

        moveDir = (int)Mathf.Sign(targetPt.x - transform.position.x);
    }

    public void GoToTargetPt(Vector3 target)
    {
        //Vector3 targetPos = new Vector3(target.x, transform.position.y, transform.position.z);
        rb.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        moveDir = (int)Mathf.Sign(target.x - transform.position.x);
    }

    private bool IsAtTargetPt()
    {
        //Mathf.Abs(rb.position.x - targetPt.x)
        if (Vector3.Distance(transform.position, targetPt) <= epsilon)
            return true;

        return false;
    }

    private void NextTargetPt()
    {
        currentPtIdx = (currentPtIdx + 1) % path.Count;
        targetPt = path[currentPtIdx].position;

        moveDir = (int)Mathf.Sign(targetPt.x - transform.position.x);
    }

    public void PathMode()
    {
        GoToTargetPt(targetPt);
        if (IsAtTargetPt())
        {
            NextTargetPt();
        }
    }

    public void TargetMode()
    {
        GetComponent<EnemyAbstract>().Triggered(targetT);
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isMove)
            return;

        if (!triggered)
            PathMode();
        else
            TargetMode();
    }
}
