using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MagnetismAbstract : MonoBehaviour
{
    private bool activeMagnetism = true;

    public bool ActiveMagnetism
    {
        get { return activeMagnetism; }
        set 
        {
            MagnetismPicked(false);
            activeMagnetism = value; 
        }
    }

    private bool isPicked = false;

    public bool IsPicked
    {
        get { return isPicked; }
        set { isPicked = value; }
    }

    private void OnEnable()
    {
        Player.PlayerDeathEvent += SetDefault;
    }

    private void OnDisable()
    {
        Player.PlayerDeathEvent -= SetDefault;
    }

    public abstract int GetMagnetismActionAttract(out float forceImpulse, out float setDefaultTime, out Transform transform);

    public abstract int GetMagnetismActionRepulse(out float forceRepulse, out float setDefaultTime, out Transform transform);

    public abstract void SetDefault();

    //public abstract bool GetMagnetismState();
    public abstract bool GetAttractState();
    public abstract bool GetRepulseState();

    public abstract Vector3 GetTransformPosition();

    public abstract void IsAttracted();
    public abstract void IsRepulsed();
    public abstract void MagnetismPicked(bool _isPicked);
    public abstract void MagnetismOutlineColor(Color outlineColor);
}
