using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inputs : MonoBehaviour
{
    private bool attractKey = false;
    private bool attractKeyDown = false;
    private bool attractKeyUp = false;

    private bool debugKey = false;

    private bool joystickDirDown= false;
    private bool joystickDirReset = false;

    private int xboxController;

    public bool JoystickDirDown
    {
        get { return joystickDirDown; }
    }

    public bool DebugKey
    {
        get { return debugKey; }
    }

    public bool AttractKey
    {
        get { return attractKey; }
    }

    public bool AttractKeyDown
    {
        get { return attractKeyDown; }
    }

    public bool AttractKeyUp
    {
        get { return attractKeyUp; }
    }

    private void Start()
    {
        xboxController = PlayerPrefs.GetInt("XboxController");
    }

    void Update()
    {
        ManageInputs();
        joystickDirDownUpdate();
    }

    private void setJoystickDirDown()
    {
        if (joystickDirDown == false && joystickDirReset)
        {
            joystickDirDown = true;
            joystickDirReset = false;
        }
        else
        {
            joystickDirDown = false;
        }
    }

    private void joystickDirDownUpdate()
    {
        if (xboxController == 1)
        {
            if (Input.GetAxisRaw("HorizontalDirXB") != 0 || Input.GetAxisRaw("VerticalDirXB") != 0)
            {
                setJoystickDirDown();
            }
        }
        else
        {
            if (Input.GetAxisRaw("HorizontalDirPS") != 0 || Input.GetAxisRaw("VerticalDirPS") != 0)
            {
                setJoystickDirDown();
            }
        }

        if (xboxController == 1)
        {
            if (Input.GetAxisRaw("HorizontalDirXB") == 0 && Input.GetAxisRaw("VerticalDirXB") == 0)
            {
                joystickDirReset = true;
            }
        }
        else
        {
            if (Input.GetAxisRaw("HorizontalDirPS") == 0 && Input.GetAxisRaw("VerticalDirPS") == 0)
            {
                joystickDirReset = true;
            }
        }
    }

    private void ManageInputs()
    {
        debugKey = Input.GetKeyDown(KeyCode.Y);

        attractKey = Input.GetButton("Attract");
        attractKeyDown = Input.GetButtonDown("Attract");
        attractKeyUp = Input.GetButtonUp("Attract");
    }

    public float GetHorizontalAxis()
    {
        return Input.GetAxis("Horizontal");
    }

    public Vector2 GetAxis()
    {
        return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    public Vector2 GetAxisDirection()
    {
        if (xboxController == 1)
            return new Vector2(Input.GetAxis("HorizontalDirXB"), Input.GetAxis("VerticalDirXB"));
        else
            return new Vector2(Input.GetAxis("HorizontalDirPS"), Input.GetAxis("VerticalDirPS"));
    }
}
