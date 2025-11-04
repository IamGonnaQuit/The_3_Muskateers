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
    public GameObject wholeModel;  // intact model
    public GameObject brokenModel; // broken/shattered model (disabled initially)

    [Header("Liquid Renderer")]
    [Tooltip("Renderer of the liquid mesh inside the whole potion model")]
    public Renderer liquidRenderer;

    [Header("Break Settings")]
    public float breakSpeedThreshold = 1.5f;
    public Rigidbody potionRigidbody;

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

        // Assign material from PotionData
        ApplyMaterial();
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

    private void Start()
    {
        ApplyMaterial();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        ApplyMaterial();
    }
#endif

    /// <summary>
    /// Assigns the material from the PotionData ScriptableObject to the liquid renderer.
    /// </summary>
    public void ApplyMaterial()
    {
        if (potionData == null || liquidRenderer == null) return;

        // Assign the material from the SO
        liquidRenderer.sharedMaterial = potionData.potionMaterial;

        // Set base color
        liquidRenderer.sharedMaterial.color = potionData.potionColor;

        // Set emission color to the same as base
        if (liquidRenderer.sharedMaterial.HasProperty("_EmissionColor"))
        {
            liquidRenderer.sharedMaterial.EnableKeyword("_EMISSION");
            liquidRenderer.sharedMaterial.SetColor("_EmissionColor", potionData.potionColor);
        }
    }





    // --- XR Grab Events ---
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

    // --- Break logic ---
    private void OnCollisionEnter(Collision collision)
    {
        if (isBroken || !Application.isPlaying) return;

        if (potionRigidbody != null && potionRigidbody.linearVelocity.magnitude >= breakSpeedThreshold)
        {
            BreakPotion();
        }
    }

    /// <summary>
    /// Switch from whole to broken model and preserve Rigidbody velocity.
    /// </summary>
    public void BreakPotion()
    {
        if (isBroken) return;
        isBroken = true;

        if (wholeModel != null) wholeModel.SetActive(false);
        if (brokenModel != null) brokenModel.SetActive(true);

        // Preserve velocity
        if (potionRigidbody != null)
        {
            potionRigidbody.linearVelocity *= 1f;
            potionRigidbody.angularVelocity *= 1f;
        }

        // Disable grab
        if (grabInteractable != null)
            grabInteractable.enabled = false;

        // Optional: add particles/sounds here
    }
}
