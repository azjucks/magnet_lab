using UnityEngine;

public class Magnet : AbstractMagnet
{

    private void Start()
    {
        layerMaskObsctacles = LayerMask.GetMask("Platform");
        MagnetMode = MagnetMode.Magnet;

        Outline = GetComponent<Outline>();
        IsPicked = false;
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
