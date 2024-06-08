using UnityEngine;

public class FootParticles : MonoBehaviour
{
    [SerializeField]
    private GameObject footParticles = null;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8 || other.gameObject.layer == 0 || other.gameObject.layer == 9)
            Instantiate(footParticles, transform.position, Quaternion.identity).GetComponent<Transform>().SetParent(null);
    }
}
