using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyAbstract : MonoBehaviour
{
    [SerializeField]
    private AudioClip detectPlayer;
    [SerializeField]
    private AudioClip attackPlayer;
    [SerializeField]
    private AudioClip death;

    protected AudioSource source;

    public enum Sounds
    {
        DETECT,
        ATTACK,
        IDLE,
        DEATH,
    };

    public void PlaySound(Sounds sound)
    {
        source.Stop();
        switch (sound)
        {
            case Sounds.DETECT:
                source.loop = false;
                source.PlayOneShot(detectPlayer, 0.5f);
                break;

            case Sounds.ATTACK:
                source.loop = false;
                source.PlayOneShot(attackPlayer, 0.5f);
                break;

            case Sounds.IDLE:
                source.loop = true;
                source.volume = 0.1f;
                source.Play();
                break;

            case Sounds.DEATH:
                source.loop = false;
                source.PlayOneShot(death, 0.5f);
                break;
            default:
                break;
        }
    }


    public abstract void SetDefault();

    public abstract void AttackPlayer(Transform t);

    public abstract void Trigger(Transform t);

    public abstract void Untrigger();

    public abstract void Triggered(Transform t);

    public abstract void Disable();

    private void OnEnable()
    {
        Player.PlayerDeathEvent += SetDefault;
    }

    private void OnDisable()
    {
        Player.PlayerDeathEvent -= SetDefault;
    }
}
