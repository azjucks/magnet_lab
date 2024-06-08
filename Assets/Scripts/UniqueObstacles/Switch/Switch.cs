using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> gameObjectsToDesactive = new List<GameObject>();

    [SerializeField]
    private List<GameObject> gameObjectsToActive = new List<GameObject>();

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "ObjectMagnetism")
        {
            foreach (GameObject gameObject in gameObjectsToDesactive)
            {
                gameObject.SetActive(false);
            }

            foreach (GameObject gameObject in gameObjectsToActive)
            {
                gameObject.SetActive(true);
            }
        }
    }
}
