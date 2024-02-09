using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HueScale : MonoBehaviour
{
    [Header("References")]
    public ColourPicker colourPicker;
    public GameObject cursor;
    public BoxCollider outerHitBox;

    private void Start()
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
            coords = new Vector3(coords.x, Mathf.Clamp(coords.y, -transform.lossyScale.y / 2f, transform.lossyScale.y / 2f), coords.z);

            colourPicker.SetColourWithEvent(new Vector3(coords.y / transform.lossyScale.y + 0.5f, colourPicker.GetHSV().y, colourPicker.GetHSV().z));
        }
    }

    public void SetHue(float hue)
    {
        cursor.transform.localPosition = new Vector3(0f, (hue - 0.5f) * transform.localScale.y, -0.001f);
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
