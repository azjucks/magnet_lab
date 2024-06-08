using UnityEngine;

public class TriggerDetectMagnet : MonoBehaviour
{
    private AbstractMagnet abstractMagnet;

    private void Start()
    {
        abstractMagnet = GetComponentInParent<AbstractMagnet>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (abstractMagnet == null)
            return;

        abstractMagnet.Triggered(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (abstractMagnet == null)
            return;

        abstractMagnet.Untriggered(other);
    }
}
