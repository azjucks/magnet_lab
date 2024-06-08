using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ObjectMagnet : AbstractMagnet
{
    private bool isAttracted = false;
    public bool IsAttracted
    {
        get { return isAttracted; }
    }
    
    [SerializeField]
    private float resetPosTime = 5f;

    private Vector3 startPos;
    private Quaternion startRot;

    private Rigidbody rb;
    private Collider colliderMagnet;

    private bool blockRepulse = false;

    [SerializeField]
    private Material materialTransparent = null;
    private Material materialDefault;

    private Renderer rendererObject;

    [SerializeField]
    private GameObject triggerZone = null;

    private bool isMeteore = false;
    [SerializeField]
    private GameObject meteoreParticles = null;

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
        layerMaskObsctacles = LayerMask.GetMask("Platform");
        MagnetMode = MagnetMode.ObjectMagnet;

        Outline = GetComponent<Outline>();
        IsPicked = false;

        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        colliderMagnet = GetComponent<Collider>();

        startPos = transform.position;
        startRot = transform.rotation;

        rendererObject = GetComponent<Renderer>();
        materialDefault = rendererObject.material;
    }

    public void Attract()
    {
        triggerZone.SetActive(false);
        isAttracted = true;
        rb.isKinematic = true;
        colliderMagnet.isTrigger = true;
        isMeteore = false;
        meteoreParticles.SetActive(false);

        if (blockRepulse)
        {
            rendererObject.material = materialDefault;
            blockRepulse = false;
        }
        StopAllCoroutines();
    }

    public void Repulse()
    {
        triggerZone.SetActive(true);
        isAttracted = false;
        rb.isKinematic = false;

        if (!blockRepulse)
        {
            colliderMagnet.isTrigger = false;
            rendererObject.material = materialDefault;
            isMeteore = true;
            meteoreParticles.SetActive(true);

        }

        StartCoroutine("ResetPos");
    }

    private void SetDefault()
    {
        rb.isKinematic = true;
        transform.position = startPos;
        transform.rotation = startRot;

        isAttracted = false;
        blockRepulse = false;
        colliderMagnet.isTrigger = false;

        triggerZone.SetActive(true);
        rendererObject.material = materialDefault;
        isMeteore = false;
        meteoreParticles.SetActive(false);

    }

    private IEnumerator ResetPos()
    {
        yield return new WaitForSeconds(resetPosTime);
        SetDefault();
    }

    public override void Triggered(Collider collider)
    {
        if (!isAttracted && collider.tag == "Player")
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
        if (!isAttracted && collider.tag == "Player")
        {
            if (IsPicked)
                PlayerMagnet.DeleteCurrentMagnet();
        }
    }

    //Layer : 9 = Magnetism - 8 = Platform - 0 = default
    private void OnCollisionEnter(Collision collision)
    {
        if (!isMeteore)
            return;

        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<Enemy>().SetDamage();
            isMeteore = false;
            meteoreParticles.SetActive(false);

        }

        if (collision.gameObject.layer == 8 || collision.gameObject.layer == 0 || collision.gameObject.layer == 9)
        {
            isMeteore = false;
            meteoreParticles.SetActive(false);
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (isAttracted && (other.gameObject.layer == 8 || other.gameObject.layer == 9))
        {
            rendererObject.material = materialTransparent;
            blockRepulse = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (isAttracted && (other.gameObject.layer == 8 || other.gameObject.layer == 9))
        {
            rendererObject.material = materialDefault;
            blockRepulse = false;
        }
    }
}
