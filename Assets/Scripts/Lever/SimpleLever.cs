using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class SimpleLever : MonoBehaviour
{
    [Header("References")]
    public Transform leverRoot;      // The pivot that rotates
    public Transform leverHandle;    // The XRGrabInteractable handle

    [Header("Rotation")]
    public float minAngle = -45f;    // Up position
    public float maxAngle = 45f;     // Down position
    public bool snapToEnds = true;   // Snap lever to min/max on release

    private XRGrabInteractable grab;
    private bool isGrabbed = false;
    private Quaternion initialRotation;

    private void Awake()
    {
        grab = leverHandle.GetComponent<XRGrabInteractable>();
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

        if (snapToEnds)
        {
            // Snap lever to closest end
            float angle = leverRoot.localEulerAngles.z;
            angle = (angle > 180) ? angle - 360 : angle; // convert to -180..180
            float targetAngle = Mathf.Abs(angle - minAngle) < Mathf.Abs(angle - maxAngle) ? minAngle : maxAngle;
            leverRoot.localRotation = initialRotation * Quaternion.Euler(0f, 0f, targetAngle);
        }
    }

    private void Update()
    {
        if (!isGrabbed) return;

        // Rotate lever based on handle local rotation
        float handleZ = leverHandle.localEulerAngles.z;
        handleZ = (handleZ > 180) ? handleZ - 360 : handleZ; // convert to -180..180
        handleZ = Mathf.Clamp(handleZ, minAngle, maxAngle);

        leverRoot.localRotation = initialRotation * Quaternion.Euler(0f, 0f, handleZ);
    }
}
