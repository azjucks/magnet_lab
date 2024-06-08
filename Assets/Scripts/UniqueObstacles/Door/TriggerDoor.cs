using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDoor : MonoBehaviour
{
    [SerializeField]
    Door door = null;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            door.CharacterNearby = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
            door.CharacterNearby = false;
    }

}
