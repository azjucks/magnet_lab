using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour
{
    //MESSAGE POUR HUGO CAR JE SAIS QU'IL N'EST PAS TRES FUTE
    //Tu mets le script sur un collider sur lequel tu coches onTrigger ok?????


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            other.transform.GetComponent<Player>().Damaged();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
