using UnityEngine;

public class SpeedSkatingTrack : MonoBehaviour
{
    [Header("Display")]
    public Color iceColour = Color.white;
    public Color borderColour = Color.blue;
    public Color innerLineColour = Color.red;

    [Header("Shape")]
    [Min(0f)]
    public float trackWidth = 1f;
    [Min(0f)]
    public float trackHeight = 1f;
    [Min(0f)]
    public float trackThickness = 1f;
    [Min(0f)]
    public float innerLineThickness = 1f;

    [Header("References")]
    public Transform middleRectangle;
    public Transform middleLeftCircle;
    public Transform middleRightCircle;

    public Transform innerLineRectangle;
    public Transform innerLineLeftCircle;
    public Transform innerLineRightCircle;

    public Transform outerRectangle;

    public Transform trackTopRectangle;
    public Transform trackBottomRectangle;
    public Transform trackLeftCircle;
    public Transform trackRightCircle;

    private void OnValidate()
    {
        UpdateTransform();
        UpdateDisplay();
    }

    private void UpdateTransform()
    {
        // Middle
        float middleRadius = trackHeight / 2f - trackThickness - innerLineThickness;
        middleRectangle.localScale = new Vector3(trackWidth - trackThickness * 2f - innerLineThickness * 2f - middleRadius * 2f, middleRadius * 2f, 1f);
        middleLeftCircle.localScale = new Vector3(middleRadius * 2f, middleRadius * 2f, 1f);
        middleRightCircle.localScale = middleLeftCircle.localScale;

        middleRectangle.localPosition = Vector3.zero;
        middleRightCircle.localPosition = new Vector3(trackWidth / 2f - trackHeight / 2f, 0f, 0f);
        middleLeftCircle.localPosition = new Vector3(-middleRightCircle.localPosition.x, 0f, 0f);

        // Inner line
        float innerLineRadius = trackHeight / 2f - trackThickness;
        innerLineRectangle.localScale = new Vector3(trackWidth - trackThickness * 2f - innerLineRadius * 2f, innerLineRadius * 2f, 1f);
        innerLineLeftCircle.localScale = new Vector3(innerLineRadius * 2f, innerLineRadius * 2f, 1f);
        innerLineRightCircle.localScale = innerLineLeftCircle.localScale;

        innerLineRectangle.localPosition = Vector3.zero;
        innerLineRightCircle.localPosition = new Vector3(trackWidth / 2f - trackHeight / 2f, 0f, 0f);
        innerLineLeftCircle.localPosition = new Vector3(-innerLineRightCircle.localPosition.x, 0f, 0f);

        // Track
        trackTopRectangle.localScale = new Vector3(trackWidth - trackHeight, trackThickness, 1f);
        trackBottomRectangle.localScale = trackTopRectangle.localScale;
        trackRightCircle.localScale = new Vector3(trackHeight, trackHeight, 1f);
        trackLeftCircle.localScale = trackRightCircle.localScale;

        trackTopRectangle.localPosition = new Vector3(0f, trackHeight / 2f - trackThickness / 2f, 0f);
        trackBottomRectangle.localPosition = new Vector3(0f, -trackTopRectangle.localPosition.y, 0f);
        trackRightCircle.localPosition = new Vector3(trackWidth / 2f - trackHeight / 2f, 0f, 0f);
        trackLeftCircle.localPosition = new Vector3(-trackRightCircle.localPosition.x, 0f, 0f);
    }

    private void UpdateDisplay()
    {
        middleRectangle.GetComponent<SpriteRenderer>().color = iceColour;
        middleLeftCircle.GetComponent<SpriteRenderer>().color = iceColour;
        middleRightCircle.GetComponent<SpriteRenderer>().color = iceColour;

        innerLineRectangle.GetComponent<SpriteRenderer>().color = innerLineColour;
        innerLineLeftCircle.GetComponent<SpriteRenderer>().color = innerLineColour;
        innerLineRightCircle.GetComponent<SpriteRenderer>().color = innerLineColour;

        outerRectangle.GetComponent<SpriteRenderer>().color = borderColour;

        trackTopRectangle.GetComponent<SpriteRenderer>().color = iceColour;
        trackBottomRectangle.GetComponent<SpriteRenderer>().color = iceColour;
        trackLeftCircle.GetComponent<SpriteRenderer>().color = iceColour;
        trackRightCircle.GetComponent<SpriteRenderer>().color = iceColour;
    }
}
