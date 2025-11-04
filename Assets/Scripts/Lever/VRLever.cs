using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class VRLever : MonoBehaviour
{
    public Transform leverRoot; // The part that actually rotates
    public XRGrabInteractable grabHandle;
    public float minAngle = 0f;
    public float maxAngle = 90f;

    private bool isGrabbed = false;
    private Vector3 grabStartPos;
    private float startAngle;

    private void OnEnable()
    {
        grabHandle.selectEntered.AddListener(OnGrab);
        grabHandle.selectExited.AddListener(OnRelease);
    }

    private void OnDisable()
    {
        grabHandle.selectEntered.RemoveListener(OnGrab);
        grabHandle.selectExited.RemoveListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        isGrabbed = true;
        grabStartPos = args.interactorObject.transform.position;
        startAngle = leverRoot.localEulerAngles.z; // depends on hinge axis
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        isGrabbed = false;
    }

    private void Update()
    {
        if (!isGrabbed) return;

        if (grabHandle.interactorsSelecting.Count == 0) return;

        var interactor = grabHandle.GetOldestInteractorSelecting();
        Vector3 controllerDelta = interactor.transform.position - grabStartPos;
        float deltaAngle = controllerDelta.y * 200f; // tweak sensitivity
        float newAngle = Mathf.Clamp(startAngle + deltaAngle, minAngle, maxAngle);

        leverRoot.localEulerAngles = new Vector3(
            leverRoot.localEulerAngles.x,
            leverRoot.localEulerAngles.y,
            newAngle
        );
    }

}
