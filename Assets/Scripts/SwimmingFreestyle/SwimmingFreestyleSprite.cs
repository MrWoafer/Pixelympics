using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwimmingFreestyleSprite : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private SwimmingFreestylePlayer player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PushOffEnd()
    {
        player.PushOffEnd();
    }
}
