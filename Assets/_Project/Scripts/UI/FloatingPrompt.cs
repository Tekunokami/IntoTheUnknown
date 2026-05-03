using UnityEngine;

public class FloatingPrompt : MonoBehaviour
{
    [Header("Entry Animation (Slide Up)")]
    public float slideSpeed = 6f;       // Medium speed for sliding up
    public float slideDistance = 0.5f;  // How far down it starts before sliding up

    [Header("Idle Animation (Bobbing)")]
    public bool doBobbing = true;
    public float bobSpeed = 3f;  
    public float bobHeight = 0.1f;

    private Vector3 targetPos;
    private Vector3 startPos;
    private bool isSlidingIn = true;

    private void Awake()
    {
        // Record the position you placed it at in the Unity Editor as its final "target"
        targetPos = transform.localPosition;
        
        // Calculate the starting position (slightly below the target)
        startPos = targetPos - new Vector3(0, slideDistance, 0); 
    }

    private void OnEnable()
    {
        // Every time the prompt turns on, reset it to the bottom and tell it to slide in
        transform.localPosition = startPos;
        isSlidingIn = true;
    }

    private void Update()
    {
        if (isSlidingIn)
        {
            // 1. Smoothly slide the prompt up from the bottom
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * slideSpeed);

            // Once it is close enough to the target, stop sliding and start bobbing
            if (Vector3.Distance(transform.localPosition, targetPos) < 0.05f)
            {
                isSlidingIn = false; 
            }
        }
        else if (doBobbing)
        {
            // 2. Idle Bobbing: Use a Sine wave to make it float gently up and down
            float newY = targetPos.y + (Mathf.Sin(Time.time * bobSpeed) * bobHeight);
            transform.localPosition = new Vector3(targetPos.x, newY, targetPos.z);
        }
    }
}