using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicCameraMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float movementSpeed = 6f;

    private Vector3 movementVector;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        movementVector = Vector3.zero;

        if (Input.GetKey(KeyCode.A))
        {
            movementVector += new Vector3(-1f, 0f, 0f);
        }
        if (Input.GetKey(KeyCode.D))
        {
            movementVector += new Vector3(1f, 0f, 0f);
        }
        if (Input.GetKey(KeyCode.W))
        {
            movementVector += new Vector3(0f, 1f, 0f);
        }
        if (Input.GetKey(KeyCode.S))
        {
            movementVector += new Vector3(0f, -1f, 0f);
        }

        movementVector.Normalize();

        transform.position += movementVector * movementSpeed * Time.deltaTime;
    }
}
