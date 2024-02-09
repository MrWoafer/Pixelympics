using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MedalTooltipController : MonoBehaviour
{
    [Header("References")]
    public Text textBox;
    public RectTransform rect;
    public bool moveToStayOnScreen = true;
    public float stayOnScreenCorrectionIncrement = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetText(string text)
    {
        textBox.text = text;
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }

    public void SetPosition(Vector2 position)
    {
        SetPosition(position.x, position.y);
    }

    public void SetPosition(float x, float y)
    {
        transform.position = new Vector3(x, y, transform.position.z);

        if (moveToStayOnScreen)
        {
            //Debug.Log(Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, rect.position.y + rect.rect.height / 2f, transform.position.z)).y);
            while (Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, rect.position.y + rect.rect.height / 2f, transform.position.z)).y > Screen.height)
            {
                transform.position -= new Vector3(0f, stayOnScreenCorrectionIncrement, 0f);
            }

            while (Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, rect.position.y - rect.rect.height / 2f, transform.position.z)).y < 0f)
            {
                transform.position += new Vector3(0f, stayOnScreenCorrectionIncrement, 0f);
            }

            while (Camera.main.WorldToScreenPoint(new Vector3(rect.position.x - rect.rect.width / 2f, transform.position.y, transform.position.z)).x < 0f)
            {
                transform.position += new Vector3(stayOnScreenCorrectionIncrement, 0f, 0f);
            }

            while (Camera.main.WorldToScreenPoint(new Vector3(rect.position.x + rect.rect.width / 2f, transform.position.y, transform.position.z)).x > Screen.width)
            {
                transform.position -= new Vector3(stayOnScreenCorrectionIncrement, 0f, 0f);
            }
        }
    }
}
