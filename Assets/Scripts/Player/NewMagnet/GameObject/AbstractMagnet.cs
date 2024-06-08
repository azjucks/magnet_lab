using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractMagnet : MonoBehaviour
{
    [SerializeField]
    private Color color = Color.yellow;

    protected LayerMask layerMaskObsctacles;

    private PlayerMagnet playerMagnet = null;

    private MagnetMode magnetMode = MagnetMode.Magnet;

    [SerializeField]
    private float attractForce = 20f;

    [SerializeField]
    private float repulseForce = 20f;

    public float AttractForce
    {
        get { return attractForce; }
    }

    public float RepulseForce
    {
        get { return repulseForce; }
    }

    public MagnetMode MagnetMode
    {
        get { return magnetMode; }
        set { magnetMode = value; }
    }


    public PlayerMagnet PlayerMagnet
    {
        get { return playerMagnet; }
        set { playerMagnet = value; }
    }

    private Outline outline;

    public Outline Outline
    {
        get { return outline; }
        set { outline = value; }
    }

    private bool isPicked;

    public bool IsPicked
    {
        get { return isPicked; }
        set 
        {
            isPicked = value;
            outline.enabled = value;

            if (value)
                outline.OutlineColor = color;
        }
    }

    public abstract void Triggered(Collider collider);
    public abstract void Untriggered(Collider collider);

    protected void GetPointClosest(Vector3 playerPos, out Vector3 ptMagnetPlayer)
    {
        ptMagnetPlayer = Physics.ClosestPoint(playerPos, GetComponent<Collider>(), transform.position, transform.rotation);
    }

    public bool CheckNotObstacle(Vector3 ptMagnet, Vector3 ptPlayer)
    {
        RaycastHit hit;
        float dist = Vector3.Distance(ptMagnet, ptPlayer);

        Debug.DrawRay(ptPlayer, (ptMagnet - ptPlayer).normalized * dist, Color.green);
        if (Physics.Raycast(ptPlayer, (ptMagnet - ptPlayer).normalized, out hit, dist, layerMaskObsctacles))
        {
            Debug.DrawLine(ptMagnet, hit.point, Color.red);
            if (hit.collider.tag != "Player")
            {
                return false;
            }
        }

        return true;
    }
}
