using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMagnet : MonoBehaviour
{
    private MagnetMode currentMagnetMode = MagnetMode.Null;

    private AbstractMagnet currentMagnet = null;
    private Vector3 ptCurrentMagnet = Vector3.zero;

    private Inputs inputs;
    private Rigidbody playerRb;

    private LayerMask layerMask;

    [SerializeField]
    private float timeJoystick = 0.5f;
    private bool isJoystickDown = false;

    private bool isMeteore = false;
    public bool IsMeteore
    {
        get { return isMeteore; }
    }


    [SerializeField]
    private GameObject magnetParticles = null;

    [SerializeField]
    private List<GameObject> meteoreParticles = new List<GameObject>();

    private void OnEnable()
    {
        Player.PlayerDeathEvent += SetDefault;
    }

    private void OnDisable()
    {
        Player.PlayerDeathEvent -= SetDefault;
    }

    private void Start()
    {

        inputs = GetComponent<Inputs>();
        playerRb = GetComponent<Rigidbody>();
        layerMask = LayerMask.GetMask("Platform");

        magnetParticles = Instantiate(magnetParticles, transform.position, Quaternion.identity);
        magnetParticles.transform.SetParent(null);
        magnetParticles.SetActive(false);
    }

    private void SetDefault()
    {
        DeleteCurrentMagnet();
    }

    public void DeleteCurrentMagnet()
    {
        if (currentMagnetMode == MagnetMode.ObjectMagnet)
            currentMagnet.transform.SetParent(null);

        currentMagnetMode = MagnetMode.Null;

        if (currentMagnet != null)
        {
            currentMagnet.IsPicked = false;
            currentMagnet = null;
        }

        magnetParticles.SetActive(false);
    }

    public void NewMagnet(AbstractMagnet abstractMagnet, Vector3 ptNewMagnet)
    {
        if (currentMagnetMode == MagnetMode.ObjectMagnet && ((ObjectMagnet)currentMagnet).IsAttracted)
            return;

        if (currentMagnetMode == MagnetMode.ObjectMagnet && currentMagnet == abstractMagnet)
            ptCurrentMagnet = ptNewMagnet;

        if (currentMagnet == null)
        {
            currentMagnet = abstractMagnet;
            ptCurrentMagnet = ptNewMagnet;

            currentMagnet.IsPicked = true;
            currentMagnetMode = currentMagnet.MagnetMode;
        }
        else if (Vector3.Distance(transform.position, ptNewMagnet) <=
               Vector3.Distance(transform.position, ptCurrentMagnet))
        {
            //Old
            if (currentMagnetMode == MagnetMode.ObjectMagnet)
                currentMagnet.transform.SetParent(null);

            currentMagnet.IsPicked = false;

            //New
            currentMagnet = abstractMagnet;
            ptCurrentMagnet = ptNewMagnet;

            currentMagnet.IsPicked = true;
            currentMagnetMode = currentMagnet.MagnetMode;

        }

        if (currentMagnet != null)
        {
            magnetParticles.transform.position = ptCurrentMagnet;
            magnetParticles.SetActive(true);
        }
        else
        {
            magnetParticles.SetActive(false);
        }
    }

    void RaycastHitOtherObject()
    {
        if (currentMagnet == null)
            return;

        Vector3 posMagnetism = ptCurrentMagnet;
        RaycastHit hit;
        float dist = Vector3.Distance(posMagnetism, transform.position);

        Debug.DrawRay(transform.position, (posMagnetism - transform.position).normalized * dist, Color.green);
        if (Physics.Raycast(transform.position, (posMagnetism - transform.position).normalized,
            out hit, dist, layerMask))
        {
            Debug.DrawLine(posMagnetism, hit.point, Color.red);

            if (hit.collider.tag != "Player")
            {
                currentMagnetMode = MagnetMode.Null;
                currentMagnet.IsPicked = false;
                currentMagnet = null;
            }
        }
    }

    void Update()
    {
        RaycastHitOtherObject();

        switch (currentMagnetMode)
        {
            case MagnetMode.Null:
                break;
            case MagnetMode.Magnet:
                MagnetAction();
                break;
            case MagnetMode.DepthMagnet:
                DepthMagnetAction();
                break;
            case MagnetMode.ObjectMagnet:
                ObjectMagnet();
                break;
        }
    }

    void ObjectMagnet()
    {
        if (inputs.AttractKey)
        {
            ObjectMagnet objectMagnet = (ObjectMagnet)currentMagnet;
            if (!objectMagnet.IsAttracted)
            {
                objectMagnet.Attract();
                currentMagnet.transform.SetParent(transform);
            }

            Vector2 axisDir = inputs.GetAxisDirection().normalized * 2f;
            if (axisDir.x == 0 && axisDir.y == 0)
                axisDir.x = 2;

            Vector3 childPos = currentMagnet.transform.GetChild(0).localPosition;
            Vector3 desiredPos = new Vector3(axisDir.x, axisDir.y, 0) - childPos;
            currentMagnet.transform.localPosition = Vector3.Lerp(currentMagnet.transform.localPosition, desiredPos, 0.5f);

            magnetParticles.SetActive(false);
        }
        else if (inputs.AttractKeyUp)
        {
            magnetParticles.SetActive(false);
            ObjectMagnet objectMagnet = (ObjectMagnet)currentMagnet;
            if (objectMagnet.IsAttracted)
            {
                objectMagnet.Repulse();
                currentMagnet.transform.SetParent(null);

                Rigidbody objectRb = currentMagnet.GetComponent<Rigidbody>();
                Vector3 axisDir = inputs.GetAxisDirection().normalized;

                if (axisDir.x == 0 && axisDir.y == 0)
                    axisDir.x = 1;

                objectRb.AddForce(axisDir * currentMagnet.RepulseForce, ForceMode.Impulse);
            }
        }
    }

    void DepthMagnetAction()
    {
        if (inputs.AttractKeyDown)
            ((DepthMagnet)currentMagnet).DepthMagnetAttracted();
    }

   private IEnumerator coroutineJoystickDir()
    {
        yield return new WaitForSeconds(timeJoystick);
        isJoystickDown = true;
    }

    void MagnetAction()
    {

        if (inputs.JoystickDirDown)
        {
            StopCoroutine("coroutineJoystickDir");
            isJoystickDown = false;

            StartCoroutine("coroutineJoystickDir");
        }

        Vector3 axisDir = inputs.GetAxisDirection();
        Vector3 separateAxis;

        #region Calcul Joystick Angle 
        separateAxis = (ptCurrentMagnet - transform.position).normalized;

        float radSAxis_0;
        float radSAxis_1;

        radSAxis_0 = Mathf.Atan2(separateAxis.y, separateAxis.x);
        if (radSAxis_0 < 0)
            radSAxis_0 += Mathf.PI;
        radSAxis_0 += Mathf.PI / 2;
        radSAxis_1 = radSAxis_0 + Mathf.PI;


        radSAxis_0 *= Mathf.Rad2Deg;
        if (radSAxis_0 > 360)
            radSAxis_0 -= 360;

        radSAxis_1 *= Mathf.Rad2Deg;
        if (radSAxis_1 > 360)
            radSAxis_1 -= 360;

        if (axisDir.y < 0)
            axisDir.x = -axisDir.x;

        float ptAxis = Mathf.Atan2(axisDir.y, axisDir.x) * Mathf.Rad2Deg;

       if (ptAxis < 0)
       {
           ptAxis -= 180;
           ptAxis = Mathf.Abs(ptAxis);
       }

        ptAxis *= Mathf.Deg2Rad;
        radSAxis_0 *= Mathf.Deg2Rad;
        radSAxis_1 *= Mathf.Deg2Rad;

       if (radSAxis_0 > radSAxis_1)
       {
           float buffer = radSAxis_0;
           radSAxis_0 = radSAxis_1;
           radSAxis_1 = buffer;
       }

        #endregion

        if ((axisDir.x != 0 || axisDir.y != 0) && isJoystickDown)
        {
            if ((separateAxis.x >= 0 && separateAxis.y >= -0.001) || separateAxis.x > 0)
            {
                if (ptAxis >= radSAxis_0 && ptAxis <= radSAxis_1)
                {
                    MagnetActionRepulse();
                    Debug.Log("Repulse_0");
                }
                else
                {
                    Debug.Log("Attract_0");
                    MagnetActionAttrack();

                }
            }
            else
            {
                if (ptAxis >= radSAxis_0 && ptAxis <= radSAxis_1)
                {
                    Debug.Log("Attract_1");
                    MagnetActionAttrack();

                }
                else
                {
                    Debug.Log("Repulse_1");
                    MagnetActionRepulse();
                }
            }
        }

        isJoystickDown = false;
    }

    void MagnetActionAttrack()
    {
        //Vector3 axisJump = ((ptCurrentMagnet - playerRb.transform.position).normalized);
        //
        //playerRb.velocity = Vector3.zero;
        //playerRb.AddForce(axisJump * currentMagnet.AttractForce, ForceMode.Impulse);

        Vector2 axisDir = inputs.GetAxisDirection();

        Vector3 axisJump = ((ptCurrentMagnet - playerRb.transform.position).normalized
            + new Vector3(axisDir.x, axisDir.y, 0).normalized).normalized;

        //playerRb.velocity = new Vector3(0, 0, 0);
        playerRb.AddForce(axisJump * currentMagnet.AttractForce, ForceMode.Impulse);
    }

    void MagnetActionRepulse()
    {
        Vector3 axisDir = inputs.GetAxisDirection().normalized;

        Vector3 axisJump = axisDir;

        playerRb.AddForce(axisJump * currentMagnet.RepulseForce, ForceMode.Impulse);

        currentMagnetMode = MagnetMode.Null;

        isMeteore = true;
        foreach (GameObject gameObject in meteoreParticles)
        {
            gameObject.SetActive(true);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isMeteore)
            return;

        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<Enemy>().SetDamage();
            isMeteore = false;

            foreach (GameObject gameObject in meteoreParticles)
            {
                gameObject.SetActive(false);
            }
        }

        //Layer : 9 = Magnetism - 8 = Platform - 0 = default
        if (collision.gameObject.layer == 8 || collision.gameObject.layer == 0 || collision.gameObject.layer == 9)
        {
            isMeteore = false;

            foreach (GameObject gameObject in meteoreParticles)
            {
                gameObject.SetActive(false);
            }
        }

    }
}
