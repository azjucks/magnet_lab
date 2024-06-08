using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricalPlatform : MonoBehaviour
{
    [SerializeField]
    private GameObject electricalParticules = null;

    private bool isAttack = false;
    private float timeAttack = 0;

    private void Start()
    {
        if (electricalParticules == null)
            Debug.Log("ElectricalParticules is null");

        electricalParticules.SetActive(false);

    }
    public void Attack(float _timeAttack)
    {
        isAttack = true;
        timeAttack = _timeAttack;
        electricalParticules.SetActive(true);
        StartCoroutine("coroutineAttack");
    }

    private IEnumerator coroutineAttack()
    {
        yield return new WaitForSeconds(timeAttack);
        isAttack = false;
        electricalParticules.SetActive(false);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!isAttack)
            return;

        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<Player>().Damaged();
        }
    }

}
