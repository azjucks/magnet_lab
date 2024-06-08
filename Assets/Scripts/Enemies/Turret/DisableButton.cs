using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableButton : MonoBehaviour
{
    private bool isPressed;
    [SerializeField] private float disableTime;
    [SerializeField] private List<TurretEnemy> turrets;

    // Start is called before the first frame update
    void Start()
    {
        isPressed = false;
    }

    private void SetDefault()
    {
        isPressed = false;
    }

    private void DisableTurrets()
    {
        isPressed = true;
        foreach(TurretEnemy t in turrets)
        {
            t.DisableTurretsButton(disableTime);
        }
        StartCoroutine("ButtonPressed");
    }

    private IEnumerator ButtonPressed()
    {
        yield return new WaitForSeconds(disableTime);

        isPressed = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Player" && !isPressed)
            DisableTurrets();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
