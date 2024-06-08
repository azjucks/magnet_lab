using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    private Vector3 checkPointPos;

    public Vector3 CheckPointPos
    {
        get { return checkPointPos; }
        set { checkPointPos = value; }
    }

    public delegate void SetDefaultEvent();
    public static event SetDefaultEvent PlayerDeathEvent;

    void Start()
    {
        checkPointPos = transform.position;
    }

    public void setCheckPointPos(Vector3 pos)
    {
        checkPointPos = pos;
    }

    public void SetDefault()
    {
        transform.position = checkPointPos;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    public void ResetPlayer()
    {
        SetDefault();
        PlayerDeathEvent();
    }

    public void Damaged()
    {
        GameManager.Instance.PlayerDie();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
            Damaged();
    }
}
