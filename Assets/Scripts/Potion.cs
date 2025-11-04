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

    [Header("Visual Effects")]
    public ParticleSystem potionParticles;
    public bool useColorForParticles = true;

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
    /// Public method to assign the liquid material from the PotionData and set color/emission.
    /// Safe to call from other scripts. Uses sharedMaterial in editor (no leak),
    /// and creates a runtime instance (renderer.material) when playing so colors don't affect other objects.
    /// </summary>
    public void ApplyMaterial()
    {
        if (potionData == null || liquidRenderer == null)
            return;

#if UNITY_EDITOR
        // In edit mode (including when working with prefab assets), assign sharedMaterial so we don't instantiate materials
        if (!Application.isPlaying)
        {
            if (potionData.potionMaterial != null)
                liquidRenderer.sharedMaterial = potionData.potionMaterial;

            var shared = liquidRenderer.sharedMaterial;
            if (shared != null)
            {
                // Base color
                if (shared.HasProperty("_BaseColor"))
                    shared.SetColor("_BaseColor", potionData.potionColor);
                else if (shared.HasProperty("_Color"))
                    shared.SetColor("_Color", potionData.potionColor);

                // Emission
                if (shared.HasProperty("_EmissionColor"))
                {
                    shared.EnableKeyword("_EMISSION");
                    shared.SetColor("_EmissionColor", potionData.potionColor);
                }
            }

            return;
        }
#endif

        // Runtime: assign a material instance so changing color won't affect other renderers that share the same asset
        if (potionData.potionMaterial != null)
            liquidRenderer.material = potionData.potionMaterial;
        else
            liquidRenderer.material = liquidRenderer.sharedMaterial; // fallback

        var mat = liquidRenderer.material;
        if (mat == null) return;

        // Base color
        if (mat.HasProperty("_BaseColor"))
            mat.SetColor("_BaseColor", potionData.potionColor);
        else if (mat.HasProperty("_Color"))
            mat.SetColor("_Color", potionData.potionColor);

        // Emission
        if (mat.HasProperty("_EmissionColor"))
        {
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", potionData.potionColor);
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

        if (potionParticles != null)
            potionParticles.Stop();

        if (grabInteractable != null)
            grabInteractable.enabled = false;
    }
}
