using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PoleVaultBarController : MonoBehaviour
{
    [Header("References")]
    public GameObject inputFieldObj;
    private TMP_InputField inputField;
    public GameObject configObj;
    private PoleVaultConfig config;
    public GameObject pole1;
    public GameObject pole2;

    private Rigidbody rb;

    private int height;

    // Start is called before the first frame update
    void Start()
    {
        inputField = inputFieldObj.GetComponent<TMP_InputField>();

        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;

        config = configObj.GetComponent<PoleVaultConfig>();
        SetHeight(config.defaultHeight);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetHeightFromTextBox(string inputHeight)
    {
        try
        {
            int newHeight = int.Parse(inputHeight);

            if (newHeight >= 0f)
            {
                Debug.Log("Set height to " + newHeight + "cm");
                SetHeight(newHeight);
            }
            else
            {
                Debug.Log("Invalid height");
                inputField.text = height.ToString();
            }
        }
        catch
        {
            Debug.Log("Invalid height");
            inputField.text = height.ToString();
        }
    }

    public void SetHeight(int newHeight)
    {
        height = newHeight;
        transform.position = new Vector3(transform.position.x, height / 100f, transform.position.z);
        inputField.text = height.ToString();

        pole1.transform.localScale = new Vector3(pole1.transform.localScale.x, (height / 100f + 1f) * 2f, pole1.transform.localScale.z);
        pole2.transform.localScale = new Vector3(pole2.transform.localScale.x, (height / 100f + 1f) * 2f, pole2.transform.localScale.z);
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Fall();
        }
    }

    private void Fall()
    {
        rb.constraints = RigidbodyConstraints.None;
    }

    public int GetHeight()
    {
        return height;
    }
}
