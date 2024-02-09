using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideBar : MonoBehaviour
{
    [Header("Bar Settings")]
    [SerializeField]
    private DirectionLR startingDirection = DirectionLR.right;
    [Tooltip("How quickly the bar moves")]
    public float barSpeed = 1f;

    [Header("Zone Settings")]
    [SerializeField]
    private float zoneSize = 1f;

    [Header("Visual Settings")]
    public bool isVisible = true;
    [Min(0f)]
    [SerializeField]
    private float barWidth = 4f;
    [Min(0f)]
    [SerializeField]
    private float barHeight = 1f;
    [Min(0f)]
    [SerializeField]
    private float borderSize = 0.05f;
    [Min(0f)]
    [SerializeField]
    private float barThickness = 0.05f;
    [Min(0f)]
    [SerializeField]
    private float dividerThickness = 0.05f;

    [Header("References")]
    [SerializeField]
    private GameObject display;
    [SerializeField]
    private GameObject border;
    [SerializeField]
    private GameObject barArea;
    [SerializeField]
    private GameObject bar;
    [SerializeField]
    private GameObject leftDivider;
    [SerializeField]
    private GameObject rightDivider;
    [SerializeField]
    private GameObject leftZone;
    [SerializeField]
    private GameObject rightZone;

    private bool moving = false;
    private DirectionLR direction = DirectionLR.right;

    // Start is called before the first frame update
    void Start()
    {
        direction = startingDirection;
    }

    // Update is called once per frame
    void Update()
    {
        if (moving)
        {
            bar.transform.position += new Vector3((float)direction * barSpeed * Time.deltaTime, 0f, 0f);

            if (bar.transform.localPosition.x > barWidth / 2f)
            {
                bar.transform.position = new Vector3(barWidth / 2f, transform.localPosition.y, transform.localPosition.z);
            }
            if (bar.transform.localPosition.x < -barWidth / 2f)
            {
                bar.transform.position = new Vector3(-barWidth / 2f, transform.localPosition.y, transform.localPosition.z);
            }
        }
    }

    private void UpdateSize()
    {
        barArea.transform.localScale = new Vector3(barWidth, barHeight, 1f);
        border.transform.localScale = barArea.transform.localScale + new Vector3(borderSize * 2f, borderSize * 2f, 0f);

        leftDivider.transform.localScale = new Vector3(dividerThickness, barHeight, 1f);
        rightDivider.transform.localScale = new Vector3(dividerThickness, barHeight, 1f);
        bar.transform.localScale = new Vector3(barThickness, barHeight, 1f);

        leftDivider.transform.localPosition = new Vector3(-barWidth / 2f + zoneSize + dividerThickness / 2f, 0f, 0f);
        rightDivider.transform.localPosition = -leftDivider.transform.localPosition;

        leftZone.transform.localScale = new Vector3(zoneSize, barHeight, 1f);
        rightZone.transform.localScale = new Vector3(zoneSize, barHeight, 1f);

        leftZone.transform.localPosition = new Vector3(-barWidth / 2f + zoneSize / 2f, 0f, 0f);
        rightZone.transform.localPosition = -leftZone.transform.localPosition;
    }

    private void OnValidate()
    {
        UpdateSize();
        SetVisible(isVisible);
    }

    public void SetVisible(bool isVisible)
    {
        this.isVisible = isVisible;
        display.SetActive(this.isVisible);
    }

    public void SetMoving(bool isMoving)
    {
        moving = isMoving;
    }

    public void ChangeDirection()
    {
        SetDirection((DirectionLR)(-(int)direction));
    }

    public void SetDirection(DirectionLR direction)
    {
        this.direction = direction;
    }

    /// <summary>
    /// 0 is the centre; 1 is the very end of the bar; -1 is on the zone divider.
    /// </summary>
    /// <param name="showGhostBar">Whether to briefly show a faint line at the retrieved value position.</param>
    /// <returns></returns>
    public float GetValue(bool showGhostBar = false)
    {
        float value;

        if (direction == DirectionLR.left)
        {
            value = (leftZone.transform.localPosition.x - bar.transform.localPosition.x) / (zoneSize / 2f);
        }
        else
        {
            value = (bar.transform.localPosition.x - rightZone.transform.localPosition.x) / (zoneSize / 2f);
        }

        return value;
    }

    public float GetWidth()
    {
        return barWidth;
    }

    public void SetWidth(float width)
    {
        /// If shrinking the bar width, move the cursor bar to a sitable point (shift it along with the ends of the bar but not past the centre).
        if (width < barWidth)
        {
            bar.transform.localPosition -= new Vector3(Mathf.Min(((barWidth - width) / 2f), Mathf.Abs(bar.transform.localPosition.x)) * Mathf.Sign(bar.transform.localPosition.x), 0f, 0f);
        }
        
        barWidth = width;
        UpdateSize();
    }
}
