using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[ExecuteAlways]
[RequireComponent(typeof(XRGrabInteractable))]
public class Potion : MonoBehaviour
{
    [Header("Potion Data")]
    public PotionData potionData;

    [Header("Potion Models")]
    public GameObject wholeModel;   // intact model
    public GameObject brokenModel;  // shattered model

    [Header("Break Settings")]
    public float breakSpeedThreshold = 0.01f;
    public Rigidbody potionRigidbody;

    [Header("Visual / Audio")]
    public ParticleSystem potionParticles; // optional, just for visuals
    public AudioSource audioSource;
    public AudioClip breakSound;
    public float breakVolume = 1f;

    private XRGrabInteractable grabInteractable;
    private bool isBroken = false;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        if (potionRigidbody == null)
            potionRigidbody = GetComponent<Rigidbody>();

        if (Application.isPlaying && grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrab);
            grabInteractable.selectExited.AddListener(OnRelease);
        }
    }

    private void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrab);
            grabInteractable.selectExited.RemoveListener(OnRelease);
        }
    }

    private void OnDisable()
    {
        if (Application.isPlaying && grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrab);
            grabInteractable.selectExited.RemoveListener(OnRelease);
        }
    }

    // XR Interaction events
    private void OnGrab(SelectEnterEventArgs args)
    {
        if (potionData != null)
        {
            PotionInfoUI.Instance?.ShowPotionInfo(potionData, args.interactorObject as XRBaseInteractor);
            PotionNameUI.Instance?.ShowPotionName(potionData.potionName, args.interactorObject as XRBaseInteractor);
        }
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        PotionInfoUI.Instance?.HidePotionInfo(args.interactorObject as XRBaseInteractor);
        PotionNameUI.Instance?.ClearText(args.interactorObject as XRBaseInteractor);
    }

    // Break logic
    private void OnCollisionEnter(Collision collision)
    {
        if (isBroken || !Application.isPlaying) return;

        if (potionRigidbody != null && potionRigidbody.linearVelocity.magnitude >= breakSpeedThreshold)
        {
            BreakPotion();
        }
    }

    public void BreakPotion()
    {
        if (isBroken) return;
        isBroken = true;

        if (audioSource != null && breakSound != null)
            audioSource.PlayOneShot(breakSound, breakVolume);

        if (wholeModel != null) wholeModel.SetActive(false);
        if (brokenModel != null) brokenModel.SetActive(true);

        if (potionParticles != null)
            potionParticles.Stop();

        if (grabInteractable != null)
            grabInteractable.enabled = false;
    }
}
