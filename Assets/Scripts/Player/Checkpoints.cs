using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoints : MonoBehaviour
{
    private Vector3 tpPos;

    [SerializeField] 
    private bool teleportation = false;

    [SerializeField] 
    GameObject newBoardStart = null;

    // Start is called before the first frame update
    void Start()
    {
        if (teleportation)
            tpPos = newBoardStart.transform.position;
        else
            tpPos = transform.position;
    }

    private void OnTriggerEnter(Collider ohter)
    {
        if (ohter.tag == "Player")
        {
            ohter.transform.GetComponent<Player>().CheckPointPos = tpPos;
            Debug.Log("checkPoint");

            if (teleportation)
                ohter.transform.GetComponent<Player>().SetDefault();
        }
    }
}
