using System.Collections;
using UnityEngine;

public class DepthMagnet : AbstractMagnet
{
    [SerializeField] private float attractedTime;
    private Vector3 startPos;

    private void OnEnable()
    {
        Player.PlayerDeathEvent += SetDefault;
    }

    private void OnDisable()
    {
        Player.PlayerDeathEvent -= SetDefault;
    }

    void Start()
    {
        startPos = transform.position;
        layerMaskObsctacles = LayerMask.GetMask("Platform");
        MagnetMode = MagnetMode.DepthMagnet;

        Outline = GetComponent<Outline>();
        IsPicked = false;
    }

    public void DepthMagnetAttracted()
    {
        MagnetMode = MagnetMode.Magnet;
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);

        if (attractedTime == 0)
            return;

        StartCoroutine("DepthMagnetReset");
    }

    private IEnumerator DepthMagnetReset()
    {
        yield return new WaitForSeconds(attractedTime);
        SetDefault();
    }

    private void SetDefault()
    {
        MagnetMode = MagnetMode.DepthMagnet;
        transform.position = startPos;
    }

    public override void Triggered(Collider collider)
    {
        if (collider.tag == "Player")
        {
            if (PlayerMagnet == null)
                PlayerMagnet = collider.GetComponent<PlayerMagnet>();

            Vector3 ptMagnet;
            GetPointClosest(PlayerMagnet.transform.position, out ptMagnet);

            if (CheckNotObstacle(ptMagnet, PlayerMagnet.transform.position))
                PlayerMagnet.NewMagnet(this, new Vector3(ptMagnet.x, ptMagnet.y, 0));
        }
    }


    public override void Untriggered(Collider collider)
    {
        if (collider.tag == "Player")
        {
            if (IsPicked)
                PlayerMagnet.DeleteCurrentMagnet();
        }
    }
}
