using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    [SerializeField]
    private bool hasToClick = true;

    [SerializeField]
    private int sceneIndex;
    
    [SerializeField]
    private bool doorChangeScene = true;

    [SerializeField]
    private Inputs inputs;

    [SerializeField]
    private Collider triggerZone = null;

    [SerializeField]
    private Animator animator = null;

    [SerializeField]
    private Animator animatorScene = null;

    [SerializeField]
    private float transitionTime = 3.0f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    public bool CharacterNearby
    {
        set
        {
            animator.SetBool("character_nearby", value);
        }
    }

    IEnumerator loadLevel()
    {
        animatorScene.SetTrigger("Trigger");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(sceneIndex);
    }

    void PlayerEnter()
    {
        Debug.Log("PlayerEnter");
        StartCoroutine("loadLevel");
    }

    // Update is called once per frame
    void Update()
    {
        if (doorChangeScene)
        {
            if (hasToClick)
            {
                if (animator.GetBool("character_nearby") && inputs.AttractKeyDown)
                    PlayerEnter();
            }
            else
            {
                if (animator.GetBool("character_nearby"))
                    PlayerEnter();
            }
        }
    }
}
