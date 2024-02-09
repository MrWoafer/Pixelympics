using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourBox : MonoBehaviour
{
    [Header("References")]
    public ColourPicker colourPicker;
    private Renderer renderer;
    public GameObject cursor;
    public BoxCollider outerHitBox;

    private void Awake()
    {
        renderer = GetComponent<Renderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        EnableOuterHitBox(false);
    }

    private void OnMouseDrag()
    {
        EnableOuterHitBox(true);
        ClickedOn();
    }
    private void OnMouseUp()
    {
        EnableOuterHitBox(false);
    }

    private void ClickedOn()
    {
        RaycastHit hit;

        if (Physics.Raycast(colourPicker.GetCamera().ScreenPointToRay(Input.mousePosition), out hit, 100f))
        {
            Vector3 coords = hit.point - transform.position;
            //Debug.Log(coords.x + " " + coords.y + " " + coords.z);

            coords = new Vector3(Mathf.Clamp(coords.x, -transform.lossyScale.x / 2f, transform.lossyScale.x / 2f), Mathf.Clamp(coords.y, -transform.lossyScale.y / 2f, transform.lossyScale.y / 2f), coords.z);

            colourPicker.SetColourWithEvent(new Vector3(colourPicker.GetHSV().x, coords.x / transform.lossyScale.x + 0.5f, coords.y / transform.lossyScale.y + 0.5f));
        }
    }

    private void SetCursor(Vector2 coords)
    {
        cursor.transform.localPosition = new Vector3(coords.x / 2f, coords.y / 2f, -0.001f);
    }
    public void SetColour(Vector3 hsv)
    {
        SetCursor(new Vector2(hsv.y * 2f - 1f, hsv.z * 2f - 1f));
        renderer.material.SetFloat("_Hue", hsv.x * 360f);
    }

    private void EnableOuterHitBox(bool on)
    {
        if (on)
        {
            outerHitBox.enabled = true;
        }
        else
        {
            outerHitBox.enabled = false;
        }
    }
}
