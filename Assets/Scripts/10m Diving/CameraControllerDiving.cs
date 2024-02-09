using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControllerDiving : MonoBehaviour
{
    public GameObject diverObj;
    private DivingController diver;

    // Start is called before the first frame update
    void Start()
    {
        diver = diverObj.GetComponent<DivingController>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (diver.HasJumped() && !diver.HasSplashed())
        {
            //Debug.Log("Yes, I should be following the diver.");
            //transform.position = new Vector3(diver.transform.position.x, Functions.RoundToRange(diver.transform.position.y, 2f, 100f), -8f);
            transform.position = new Vector3(diver.transform.position.x, Functions.RoundToRange(diver.transform.position.y, 3f, 100f), -8f);
            //transform.position = new Vector3(diver.transform.position.x + 7.5f, Functions.RoundToRange(diver.transform.position.y, 3f, 100f), -8f);
            //transform.eulerAngles = new Vector3(0f, 0f, 0f);
            //transform.eulerAngles = new Vector3(0f, -30f, 0f);
        }
    }
}
