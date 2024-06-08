using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricalPlatformsManager : MonoBehaviour
{
    [SerializeField] 
    private List<ElectricalPlatform> electricalPlatforms = new List<ElectricalPlatform>();

    [SerializeField]
    private float newAttackTime = 2f;


    [SerializeField]
    private float switchElectricalSpeed = 0.5f;

    private int index = 0;

    private enum DirectionMode
    {
        LeftToRight,
        RightToLeft
    }

    [SerializeField]
    private DirectionMode directionMode = 0;

    private void Start()
    {
        StartCoroutine("coroutineNewAttack");
    }

    private IEnumerator coroutineNewAttack()
    {
        yield return new WaitForSeconds(newAttackTime);
        StartCoroutine("coroutineAttack");
        if (directionMode == DirectionMode.LeftToRight)
            index = 0;
        else
            index = electricalPlatforms.Count - 1;

        Debug.Log("Start");
    }

    private IEnumerator coroutineAttack()
    {
        yield return new WaitForSeconds(switchElectricalSpeed);

        if ((directionMode == DirectionMode.LeftToRight && index == electricalPlatforms.Count) || (directionMode == DirectionMode.RightToLeft && index == -1))
        {
            StartCoroutine("coroutineNewAttack");
        }
        else
        {
            electricalPlatforms[index].Attack(switchElectricalSpeed);

            if (directionMode == DirectionMode.LeftToRight)
                index++;
            else
                index--;

            StartCoroutine("coroutineAttack");
        }
    }

}
