using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPath : MonoBehaviour
{
    [SerializeField] private List<Transform> path;
    [SerializeField] private float speed;
    private int currentPtIdx;
    private Vector3 targetPt;
    private float epsilon = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        currentPtIdx = 0;
        if (path.Count != 0)
            targetPt = path[0].position;
    }

    private void NextPathPt()
    {
        currentPtIdx = (currentPtIdx + 1) % path.Count;
        targetPt = path[currentPtIdx].position;
    }
    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPt, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPt) <= epsilon)
            NextPathPt();
    }
}
