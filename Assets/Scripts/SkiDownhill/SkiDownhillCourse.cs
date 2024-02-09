using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class SkiDownhillCourse : MonoBehaviour
{
    [Header("Settings")]
    public string courseName = "Course Name";
    public Color pisteEdgeLineColour = Color.blue;

    [Header("References")]
    private SpriteShapeController piste;
    private SpriteShapeController pisteEdgeLine;

    // Start is called before the first frame update
    void Start()
    {
        piste = transform.Find("Piste").GetComponent<SpriteShapeController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnValidate()
    {
        gameObject.name = courseName;
    }

    public void CreatePisteEdgeLine(float thickness)
    {
        pisteEdgeLine = Instantiate(piste, piste.transform.position, piste.transform.rotation, piste.transform);
        pisteEdgeLine.transform.localScale = Vector3.one;
        pisteEdgeLine.name = "Edge Line";

        pisteEdgeLine.GetComponent<SpriteShapeRenderer>().color = pisteEdgeLineColour;
        pisteEdgeLine.GetComponent<SpriteShapeRenderer>().sortingOrder = -1;

        Spline spline = pisteEdgeLine.spline;
        for (int i = 0; i < spline.GetPointCount(); i++)
        {
            spline.SetHeight(i, spline.GetHeight(i) + thickness);
        }

        pisteEdgeLine.RefreshSpriteShape();
    }
}
