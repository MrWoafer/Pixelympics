using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcheryTargetController : MonoBehaviour
{
    [Header("References")]
    public GameObject centreTransform;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float GetDistance(Vector3 point)
    {
        return Mathf.Sqrt(
            (centreTransform.transform.position.x - point.x) * (centreTransform.transform.position.x - point.x) +
            (centreTransform.transform.position.y - point.y) * (centreTransform.transform.position.y - point.y) +
            (centreTransform.transform.position.z - point.z) * (centreTransform.transform.position.z - point.z));
    }
}
