using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class SmoothPhysicsGrab : MonoBehaviour
{
    public float followSpeed = 20f;
    public float rotateSpeed = 20f;

    private XRBaseInteractor interactor;
    private XRGrabInteractable grab;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grab = GetComponent<XRGrabInteractable>();

        grab.selectEntered.AddListener(OnGrab);
        grab.selectExited.AddListener(OnRelease);
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        interactor = args.interactorObject as XRBaseInteractor;
        rb.useGravity = false;
    }

    void OnRelease(SelectExitEventArgs args)
    {
        interactor = null;
        rb.useGravity = true;
    }

    void FixedUpdate()
    {
        if (!interactor) return;

        Transform target = interactor.GetAttachTransform(grab);
        Vector3 posDelta = target.position - transform.position;
        Quaternion rotDelta = target.rotation * Quaternion.Inverse(transform.rotation);

        rb.linearVelocity = posDelta * followSpeed;
        rb.angularVelocity = new Vector3(rotDelta.x, rotDelta.y, rotDelta.z) * rotateSpeed;
    }

}
