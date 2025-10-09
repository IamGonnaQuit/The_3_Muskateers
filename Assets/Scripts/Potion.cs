using UnityEngine;

[ExecuteAlways] // runs in editor so you see changes immediately
public class Potion : MonoBehaviour
{
    [Tooltip("The ScriptableObject that defines this potion.")]
    public PotionData potionData;

    [Tooltip("Renderers on this object that should change color. If empty, will auto-find child Renderers.")]
    public Renderer[] targetRenderers;

    MaterialPropertyBlock _mpb;

    void OnEnable()
    {
        if (_mpb == null) _mpb = new MaterialPropertyBlock();
        ApplyColor();
    }

    void Start()
    {
        // At runtime ensure color is applied
        ApplyColor();
    }

    void OnValidate()
    {
        // Called in editor when fields change
        if (_mpb == null) _mpb = new MaterialPropertyBlock();
        ApplyColor();
    }

    public void ApplyColor()
    {
        if (potionData == null) return;

        // Auto-find renderers if none assigned
        if (targetRenderers == null || targetRenderers.Length == 0)
            targetRenderers = GetComponentsInChildren<Renderer>(true);

        if (targetRenderers == null) return;

        foreach (var r in targetRenderers)
        {
            if (r == null) continue;

            if (_mpb == null) _mpb = new MaterialPropertyBlock();
            r.GetPropertyBlock(_mpb);

            // Try the common color properties so it works with Standard/URP/HDRP shaders
            if (_mpb != null)
            {
                _mpb.SetColor("_Color", potionData.potionColor);      // Standard
                _mpb.SetColor("_BaseColor", potionData.potionColor);  // URP/HDRP
                r.SetPropertyBlock(_mpb);
            }

#if UNITY_EDITOR
            // Editor fallback to update preview for shaders that don't respond to property blocks.
            // NOTE: this modifies the sharedMaterial color in editor only — prefer using material instances
            if (!Application.isPlaying && r.sharedMaterial != null)
            {
                if (r.sharedMaterial.HasProperty("_Color"))
                    r.sharedMaterial.color = potionData.potionColor;
                else if (r.sharedMaterial.HasProperty("_BaseColor"))
                    r.sharedMaterial.SetColor("_BaseColor", potionData.potionColor);
            }
#endif
        }
    }
}