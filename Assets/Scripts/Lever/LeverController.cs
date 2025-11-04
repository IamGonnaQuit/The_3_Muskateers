using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class LeverController : MonoBehaviour
{
    [Header("Lever Settings")]
    public Transform leverRoot; // the part that rotates
    public float minAngle = -30f; // up position
    public float maxAngle = 30f;  // down position
    public float leverSpeed = 10f;
    public float activateThreshold = 0.9f; // how far to activate state

    [Header("Events")]
    public UnityEngine.Events.UnityEvent OnLeverUp;
    public UnityEngine.Events.UnityEvent OnLeverDown;

    private XRGrabInteractable grab;
    private Quaternion initialRotation;
    private bool isGrabbed = false;
    private float leverValue = 0f; // 0 = up, 1 = down
    private bool hasTriggeredUp = false;
    private bool hasTriggeredDown = false;

    private void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
        grab.selectEntered.AddListener(OnGrab);
        grab.selectExited.AddListener(OnRelease);

        initialRotation = leverRoot.localRotation;
    }

    private void OnDestroy()
    {
        grab.selectEntered.RemoveListener(OnGrab);
        grab.selectExited.RemoveListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        isGrabbed = true;
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        isGrabbed = false;
    }

    private void Update()
    {
        if (isGrabbed)
        {
            // Read hand movement
            Vector3 localUp = transform.parent.InverseTransformDirection(transform.up);
            leverValue = Mathf.Clamp01((localUp.y + 1f) / 2f);
        }

        // Rotate lever
        float targetAngle = Mathf.Lerp(minAngle, maxAngle, leverValue);
        Quaternion targetRot = initialRotation * Quaternion.Euler(targetAngle, 0f, 0f);
        leverRoot.localRotation = Quaternion.Lerp(leverRoot.localRotation, targetRot, Time.deltaTime * leverSpeed);

        // Trigger events
        if (!hasTriggeredDown && leverValue >= activateThreshold)
        {
            hasTriggeredDown = true;
            hasTriggeredUp = false;
            OnLeverDown?.Invoke();
        }
        else if (!hasTriggeredUp && leverValue <= (1f - activateThreshold))
        {
            hasTriggeredUp = true;
            hasTriggeredDown = false;
            OnLeverUp?.Invoke();
        }
    }
}
