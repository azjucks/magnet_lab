using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TurretEnemy : EnemyAbstract
{
    private EnemyMovements em;

    [SerializeField] private float shootDelay;
    [SerializeField] private float castTime;

    [SerializeField] private Transform transformWeapon = null;
    [SerializeField] private Transform transformPointFire = null;
    [SerializeField] private Transform basePoint = null;
    [SerializeField] private GameObject chargeParticles = null;
    [SerializeField] private GameObject collisionParticles = null;

    private Transform target = null;
    private Vector3 posToShoot;
    private LayerMask rayMask;
    private LayerMask toAvoidMask;
    [SerializeField] private bool canShoot;
    private bool isEnabled;
    private RaycastHit hit;
    public bool IsEnabled
    {
        get { return isEnabled; }
        set { isEnabled = value; }
    }

    [SerializeField] LineRenderer shootLine;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        isEnabled = true;
        //shootLine.startColor = Color.white;
        //shootLine.endColor = Color.red;
        shootLine.enabled = true;
        shootLine.useWorldSpace = true;
        em = GetComponent<EnemyMovements>();
        //shootDelay = 1.2f;
        //castTime = 0.2f;
        canShoot = true;
        rayMask = LayerMask.GetMask("Default", "Magnetism", "Platform");
        toAvoidMask = LayerMask.GetMask("Magnetism", "Platform");
        target = transform;

        chargeParticles.SetActive(false);
    }

    public override void SetDefault()
    {
        isEnabled = true;
        canShoot = true;
        target = transform;
    }

    public override void Disable()
    {
        IsEnabled = false;
    }

    public override void AttackPlayer(Transform t)
    {
        t.GetComponent<Player>().Damaged();
    }

    public override void Trigger(Transform t)
    {
        em.Triggered = true;
        target = t;
        posToShoot = t.position;
    }

    public override void Untrigger()
    {
        em.Triggered = false;
        target = transform;
    }

    // LOS = Line of Sight
    public bool HasLOS()
    {
        float dist = Vector3.Distance(posToShoot, transform.position);

        if (Physics.Raycast(transform.position, (posToShoot - transform.position).normalized,
            out hit, dist, rayMask))
        {

            if (hit.collider.tag == "Enemy")
                return false;
            if (toAvoidMask == (toAvoidMask | (1 << hit.transform.gameObject.layer)))
               return false;

            return true;
        }
        return false;
    }

    public void DisableTurretsButton(float time)
    {
        isEnabled = false;
        StartCoroutine("EnableTurretsButton", time);
    }

    private IEnumerator EnableTurretsButton(float time)
    {
        yield return new WaitForSeconds(time);

        isEnabled = true;
    }

    private IEnumerator ShootToPlayer()
    {
        chargeParticles.SetActive(true);
        yield return new WaitForSeconds(castTime);

        PlaySound(Sounds.ATTACK);
        chargeParticles.SetActive(false);

        if (HasLOS())
        {
            //Physics.Raycast(transform.position, posToShoot - transform.position, out hit);

            if (hit.collider.tag == "Player")
                hit.collider.GetComponent<Player>().Damaged();
        }

        shootLine.positionCount = 2;
        shootLine.gameObject.SetActive(true);
        shootLine.SetPosition(0, transformPointFire.position);
        if (hit.collider != null)
        {
            shootLine.SetPosition(1, hit.point);
            Instantiate(collisionParticles, hit.transform);
        }
        else
        {
            shootLine.SetPosition(1, posToShoot);
            Instantiate(collisionParticles, posToShoot, Quaternion.identity);
        }

        StartCoroutine("TurretShotRay");

        StartCoroutine("EnableTurret");
    }

    private IEnumerator TurretShotRay()
    {
        yield return new WaitForSeconds(0.15f);
        shootLine.gameObject.SetActive(false);
    }

    public override void Triggered(Transform t)
    {
        if (canShoot && isEnabled)
        {
            posToShoot = target.position;
            canShoot = false;
            StartCoroutine("ShootToPlayer");
        }
    }

    private IEnumerator EnableTurret()
    {
        yield return new WaitForSeconds(shootDelay);

        canShoot = true;
    }

    void Update()
    {
        if (em.Triggered)
        {
            transformWeapon.LookAt(target);
        }
        else
        {
            transformWeapon.LookAt(basePoint);
        }
    }
}
