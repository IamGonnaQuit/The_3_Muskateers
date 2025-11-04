using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class SimpleLever : MonoBehaviour
{
    [Header("References")]
    public Transform leverRoot;     // rotates
    public Transform leverHandle;   // the grabbable part

    [Header("Rotation")]
    public float minAngle = -45f;
    public float maxAngle = 45f;

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
    }

    private void Update()
    {
        if (!isGrabbed) return;

        // Simple lever rotation based on handle local rotation
        Vector3 localEuler = leverHandle.localEulerAngles;
        float z = Mathf.Clamp(localEuler.z, minAngle, maxAngle);

        leverRoot.localRotation = initialRotation * Quaternion.Euler(0f, 0f, z);
    }
}
