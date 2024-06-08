using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovements : MonoBehaviour
{

    private Rigidbody rb;
    [SerializeField] private Transform playerRenderTransform = null;
    [SerializeField] private float acceleration = 2.0f;
    [SerializeField] private float velocityReset = 0.1f;
    [SerializeField] private float maxMovementsVelocity = 20.0f;
    [SerializeField] private float maxVelocity = 20.0f;
    [Range(0f, 1f)] [SerializeField] private float decelerationForce = 0.5f;

    [SerializeField]
    private Transform raycastFoot_0 = null;

    [SerializeField]
    private Transform raycastFoot_1 = null;

    private PlayerAnimations playerAnimations;
    private bool groundedAnim = false;
    private Inputs inputs;
    private int lastSide = 1;

    void Start()
    {
        playerAnimations = GetComponent<PlayerAnimations>();
        inputs = GetComponent<Inputs>();
        rb = GetComponent<Rigidbody>();
    }

    private void Move(float axisValue)
    {
            if (Mathf.Abs(rb.velocity.x) <= maxMovementsVelocity/3)
        {
            playerAnimations.IsMoving = true;
        }
        else
        {
            playerAnimations.IsRunning = true;
        }

        if (Mathf.Abs(rb.velocity.x) < maxMovementsVelocity)
        {
            Vector3 movement = new Vector3(axisValue * acceleration, 0, 0);
            rb.AddForce(movement, ForceMode.Force);
        }
        else if (Mathf.Sign(rb.velocity.x) != axisValue)
        {
            Vector3 movement = new Vector3(axisValue * acceleration, 0, 0);
            rb.AddForce(movement, ForceMode.Force);
        }

        if (axisValue < 0)
        {
            playerRenderTransform.eulerAngles = new Vector3(0, 180, 0);
        }
        else
        {
            playerRenderTransform.eulerAngles = new Vector3(0, 0, 0);
        }
    }

    private bool isGrounded(Transform _transform)
    {
        if (Physics.Raycast(_transform.position, Vector3.down, 0.25f))
        {
            return true;
        }

        return false;
    }

    private void Decelerate()
    {
        playerAnimations.IsIdling = true;
        if (isGrounded(raycastFoot_0) || isGrounded(raycastFoot_1))
           rb.velocity = new Vector3(rb.velocity.x * decelerationForce, rb.velocity.y, rb.velocity.z);
    }

    void setMaxVelocity(float vel)
    {
        if (Mathf.Abs(rb.velocity.x) > vel)
            rb.velocity = new Vector3(Mathf.Sign(rb.velocity.x) * vel, rb.velocity.y, rb.velocity.z);
        if (Mathf.Abs(rb.velocity.y) > vel)
            rb.velocity = new Vector3(rb.velocity.x, Mathf.Sign(rb.velocity.y) * vel, rb.velocity.z);
    }

    IEnumerator VelocityReset()
    {
        yield return new WaitForSeconds(velocityReset);

        rb.velocity = Vector3.zero;
    }

    private void FixedUpdate()
    {
        float axisXValue = inputs.GetHorizontalAxis();
        if (axisXValue != 0)
            Move(axisXValue);
        else
            Decelerate();

        if (isGrounded(raycastFoot_0) || isGrounded(raycastFoot_1))
        {
            if (Mathf.Sign(axisXValue) != lastSide)
                StartCoroutine("VelocityReset");

            setMaxVelocity(maxVelocity);
        }

        if (!isGrounded(raycastFoot_0) && !isGrounded(raycastFoot_1))
        {
            playerAnimations.IsMidAir = true;
        }

        setMaxVelocity(maxVelocity);

        lastSide = (int) Mathf.Sign(axisXValue);
    }
}
