using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Events;

public class ReadOnlyAttribute : PropertyAttribute
{

}

[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property,
                                            GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position,
                               SerializedProperty property,
                               GUIContent label)
    {
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }
}

public class ColourPicker : MonoBehaviour
{
    [Header("Settings")]
    public Color defaultColour = Color.red;
    [ReadOnly][SerializeField]
    private Vector3 hsv;
    [ReadOnly]
    [SerializeField]
    private Color currentColour;

    [Header("Events")]
    public UnityEvent onColourChange;
    
    [Header("References")]
    public ColourBox colourBox;
    public HueScale hueScale;
    public Image colourPreview;
    public Slider rSlider;
    public InputField rText;
    public Slider gSlider;
    public InputField gText;
    public Slider bSlider;
    public InputField bText;
    private Camera camera;

    private bool rgbValueChangeLock = false;

    private void Awake()
    {
        camera = Camera.main;
        SetColour(defaultColour);
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    public void SetColour(Vector3 newColourHSV)
    {
        hsv = newColourHSV;

        currentColour = Color.HSVToRGB(hsv.x, hsv.y, hsv.z);
        
        UpdateDisplay();
    }
    public void SetColour(Color newColourRGB)
    {
        currentColour = newColourRGB;

        float h, s, v;
        Color.RGBToHSV(newColourRGB, out h, out s, out v);
        hsv = new Vector3(h, s, v);

        UpdateDisplay();
    }

    public void SetColourWithEvent(Vector3 newColourHSV)
    {
        SetColour(newColourHSV);
        onColourChange.Invoke();
    }
    public void SetColourWithEvent(Color newColourRGB)
    {
        SetColour(newColourRGB);
        onColourChange.Invoke();
    }

    private void UpdateDisplay()
    {
        colourBox.SetColour(hsv);
        hueScale.SetHue(hsv.x);
        SetSliders(Color.HSVToRGB(hsv.x, hsv.y, hsv.z));
        colourPreview.color = Color.HSVToRGB(hsv.x, hsv.y, hsv.z);
    }

    public Vector3 GetHSV()
    {
        return hsv;
    }
    public Color GetRGB()
    {
        return Color.HSVToRGB(hsv.x, hsv.y, hsv.z);
    }

    public Camera GetCamera()
    {
        return camera;
    }

    private void SetSliders(Color newColour)
    {
        rSlider.SetValueWithoutNotify(newColour.r);
        gSlider.SetValueWithoutNotify(newColour.g);
        bSlider.SetValueWithoutNotify(newColour.b);

        rText.SetTextWithoutNotify(Mathf.Ceil(newColour.r * 255f).ToString());
        gText.SetTextWithoutNotify(Mathf.Ceil(newColour.g * 255f).ToString());
        bText.SetTextWithoutNotify(Mathf.Ceil(newColour.b * 255f).ToString());
    }

    public void SlidersChanged()
    {
        SetColourWithEvent(new Color(rSlider.value, gSlider.value, bSlider.value));
    }

    public void RGBInputChanged()
    {
        SetColourWithEvent(new Color(float.Parse(rText.text) / 255f, float.Parse(gText.text) / 255f, float.Parse(bText.text) / 255f));
    }
}
