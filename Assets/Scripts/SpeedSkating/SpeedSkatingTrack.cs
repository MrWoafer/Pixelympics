using UnityEngine;

public class SpeedSkatingTrack : MonoBehaviour
{
    [Header("Display")]
    public Color iceColour = Color.white;
    public Color borderColour = Color.blue;
    public Color innerLineColour = Color.red;
    public Color startLineColour = Color.red;

    [Header("Shape")]
    [Min(0f)]
    public float trackWidth = 1f;
    [Min(0f)]
    public float trackHeight = 1f;
    [Min(0f)]
    public float trackThickness = 1f;
    [Min(0f)]
    public float innerLineThickness = 1f;
    [Min(0f)]
    public float startLineThickness = 1f;

    [Header("References")]
    public Transform middleRectangle;
    public Transform middleLeftCircle;
    public Transform middleRightCircle;

    public Transform innerLineRectangle;
    public Transform innerLineLeftCircle;
    public Transform innerLineRightCircle;

    public Transform startLine;

    public Transform outerRectangle;

    public Transform trackRectangle;
    public Transform trackLeftCircle;
    public Transform trackRightCircle;

    public float bendX => trackWidth / 2f - trackHeight / 2f;

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
        middleRightCircle.localPosition = new Vector3(bendX, 0f, 0f);
        middleLeftCircle.localPosition = new Vector3(-middleRightCircle.localPosition.x, 0f, 0f);

        // Inner line
        float innerLineRadius = trackHeight / 2f - trackThickness;
        innerLineRectangle.localScale = new Vector3(trackWidth - trackThickness * 2f - innerLineRadius * 2f, innerLineRadius * 2f, 1f);
        innerLineLeftCircle.localScale = new Vector3(innerLineRadius * 2f, innerLineRadius * 2f, 1f);
        innerLineRightCircle.localScale = innerLineLeftCircle.localScale;

        innerLineRectangle.localPosition = Vector3.zero;
        innerLineRightCircle.localPosition = new Vector3(bendX, 0f, 0f);
        innerLineLeftCircle.localPosition = new Vector3(-innerLineRightCircle.localPosition.x, 0f, 0f);

        // Start line
        startLine.localScale = new Vector3(startLineThickness, trackThickness, 1f);
        startLine.localPosition = new Vector3(0f, -trackHeight / 2f + trackThickness / 2f, 0f);

        // Track
        trackRectangle.localScale = new Vector3(trackWidth - trackHeight, trackHeight, 1f);
        trackRightCircle.localScale = new Vector3(trackHeight, trackHeight, 1f);
        trackLeftCircle.localScale = trackRightCircle.localScale;

        trackRectangle.localPosition = Vector3.zero;
        trackRightCircle.localPosition = new Vector3(bendX, 0f, 0f);
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

        startLine.GetComponent<SpriteRenderer>().color = startLineColour;

        outerRectangle.GetComponent<SpriteRenderer>().color = borderColour;

        trackRectangle.GetComponent<SpriteRenderer>().color = iceColour;
        trackLeftCircle.GetComponent<SpriteRenderer>().color = iceColour;
        trackRightCircle.GetComponent<SpriteRenderer>().color = iceColour;
    }
}
