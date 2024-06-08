using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDetection : MonoBehaviour
{
    [SerializeField] private EnemyAbstract enemy;


    public void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            enemy.Trigger(other.transform);
            enemy.PlaySound(EnemyAbstract.Sounds.DETECT);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            enemy.Untrigger();
        }
    }
}
