using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlternativesPlatformes : MonoBehaviour
{
    [SerializeField]
    private List<AlternativesClass> gameObjectsList = new List<AlternativesClass>();

    [SerializeField]
    private float timeAlternate = 2f;

    [SerializeField]
    private PlayerMagnetism playerMagnetism;

    private int index = 0;
    private int indexBefore;

    bool isStarted = false;

    private void Update()
    {
        if (isStarted)
            return;

        indexBefore = gameObjectsList.Count - 1;
        for (int i = 1; i < gameObjectsList.Count; i++)
        {
            gameObjectsList[i].SetActiveGameObjects(false, playerMagnetism);
        }

        StartCoroutine("coroutineAlternate");
        isStarted = true;
    }

    private IEnumerator coroutineAlternate()
    {
        yield return new WaitForSeconds(timeAlternate);
        if (index >= gameObjectsList.Count)
            index = 0;

        gameObjectsList[index].SetActiveGameObjects(true, playerMagnetism);
        gameObjectsList[indexBefore].SetActiveGameObjects(false, playerMagnetism);

        indexBefore = index;
        index++;

        StartCoroutine("coroutineAlternate");
    }

}
