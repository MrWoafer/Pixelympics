using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedClimbingDuplicate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject obj = Instantiate(gameObject, new Vector3(40f - transform.position.x, transform.position.y, transform.position.z), transform.rotation);
        obj.transform.localScale = new Vector3(-transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);
        Destroy(obj.GetComponent<SpeedClimbingDuplicate>());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
