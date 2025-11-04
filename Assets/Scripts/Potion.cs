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

    [Header("Renderers")]
    public Renderer[] targetRenderers;

    [Header("Optional Overrides")]
    public Material overrideMaterial; // optional local override

    private XRGrabInteractable grabInteractable;
    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        // Hook up runtime events only while playing
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

    // In case of domain reload or disable we unsubscribe
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
        if (Application.isPlaying)
            ApplyMaterial();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
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

            // determine material to apply: override -> potionData -> don't change
            Material matToApply = overrideMaterial != null ? overrideMaterial : potionData.potionMaterial;

            if (matToApply != null && r.sharedMaterial != matToApply)
            {
                r.sharedMaterial = matToApply;
            }

            // Try to tint via property if available (uses sharedMaterial)
            var shared = r.sharedMaterial;
            if (shared != null)
            {
                if (shared.HasProperty(BaseColorId))
                    shared.SetColor(BaseColorId, potionData.potionColor);
                else if (shared.HasProperty("_Color"))
                    shared.SetColor("_Color", potionData.potionColor);
            }
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
