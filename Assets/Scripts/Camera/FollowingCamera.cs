using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingCamera : MonoBehaviour
{
    [SerializeField] Transform myObject;
    [SerializeField] Vector3 offset = new Vector3(0,0,-20);
    [SerializeField] private float speedMove = 5f;

    public static FollowingCamera instance;

    public void setObject(Transform t)
    {
        myObject = t;
    }

    public Transform getObject()
    {
        return myObject;
    }

    void Awake()
    {
        instance = this;
    }

   // private void Start()
   // {
   //     offset.x = 0;
   //     offset.y = 0;
   //     offset.z = -20;
   // }

    void Update()
    {
        //Not smooth 
        //transform.position = myObject.position + offset;

        //is Smooth
        Vector3 curVelocity = new Vector3();
        transform.position = Vector3.SmoothDamp(transform.position, myObject.position + offset, ref curVelocity, speedMove * Time.deltaTime);
    }
}
