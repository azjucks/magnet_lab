using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] 
    private GameObject rendererEnemy = null;
    [SerializeField]
    private GameObject explodeParticlesPrefab = null;
    private Collider colliderEnemy;
    private EnemyAbstract enemyAbstract;

    // Start is called before the first frame update
    void Start()
    {
        enemyAbstract = GetComponent<EnemyAbstract>();
        colliderEnemy = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        Player.PlayerDeathEvent += SetDefault;
    }

    private void OnDisable()
    {
        Player.PlayerDeathEvent -= SetDefault;
    }

    public void SetDefault()
    {
        rendererEnemy.SetActive(true);
        colliderEnemy.enabled = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Player")
        {
            if (!collision.gameObject.GetComponent<PlayerMagnet>().IsMeteore)
            {
                enemyAbstract.AttackPlayer(collision.transform);
            }
        }
    }

    public void SetDamage()
    {
        enemyAbstract.PlaySound(EnemyAbstract.Sounds.DEATH);
        enemyAbstract.Disable();
        colliderEnemy.enabled = false;
        rendererEnemy.SetActive(false);
        GameObject instanceGameObject = Instantiate(explodeParticlesPrefab, transform);
        instanceGameObject.transform.SetParent(null);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
