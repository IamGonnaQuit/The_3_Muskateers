using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[ExecuteAlways] // makes it work in edit mode
public class Potion : MonoBehaviour
{
    [Header("Potion Data")]
    public PotionData potionData;

    [Header("Renderers")]
    public Renderer[] targetRenderers;

    private XRGrabInteractable grabInteractable;

    private void Awake()
    {
        // Try to get the XRGrabInteractable on this object
        grabInteractable = GetComponent<XRGrabInteractable>();

        // Only hook into events at runtime
        if (Application.isPlaying && grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrab);
            grabInteractable.selectExited.AddListener(OnRelease);
        }
    }

    private void OnDestroy()
    {
        // Clean up event listeners
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrab);
            grabInteractable.selectExited.RemoveListener(OnRelease);
        }
    }

    private void Start()
    {
        // Still run at runtime
        if (Application.isPlaying)
            ApplyMaterial();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Runs in the editor whenever you change something in the inspector
        if (!Application.isPlaying)
            ApplyMaterial();
    }
#endif

    public void ApplyMaterial()
    {
        if (potionData == null)
            return;

        if (targetRenderers == null || targetRenderers.Length == 0)
            targetRenderers = GetComponentsInChildren<Renderer>(true);

        foreach (var r in targetRenderers)
        {
            if (r == null) continue;

            if (potionData.potionMaterial != null && r.sharedMaterial != potionData.potionMaterial)
                r.sharedMaterial = potionData.potionMaterial;

            if (r.sharedMaterial.HasProperty("_BaseColor"))
                r.sharedMaterial.SetColor("_BaseColor", potionData.potionColor);
            else if (r.sharedMaterial.HasProperty("_Color"))
                r.sharedMaterial.color = potionData.potionColor;
        }
    }

    // --- XR Event Handlers ---
    private void OnGrab(SelectEnterEventArgs args)
    {
        if (potionData != null)
            PotionNameUI.Instance?.ShowPotionName(potionData.potionName, args.interactorObject as XRBaseInteractor);
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        PotionNameUI.Instance?.ClearText(args.interactorObject as XRBaseInteractor);
    }

}
