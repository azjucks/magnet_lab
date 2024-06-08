using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiscInputs : MonoBehaviour
{
    private bool exitGameKey = false;
    private bool resetSceneKey = false;
    private bool pauseKey = false;

    private int xboxController;

    public bool PauseKey
    {
        get { return pauseKey; }
    }

    public bool ResetSceneKey
    {
        get { return resetSceneKey; }
    }
    public bool ExitGameKey
    {
        get { return exitGameKey; }
    }

    // Start is called before the first frame update
    void Start()
    {
        xboxController = PlayerPrefs.GetInt("XboxController");
    }

    // Update is called once per frame
    void Update()
    {
        resetSceneKey = Input.GetKeyDown(KeyCode.R);
        exitGameKey = Input.GetKeyDown(KeyCode.Escape);

        if (xboxController == 1)
            pauseKey = Input.GetButtonDown("PauseXB");
        else
            pauseKey = Input.GetButtonDown("PausePS");
    }
}
