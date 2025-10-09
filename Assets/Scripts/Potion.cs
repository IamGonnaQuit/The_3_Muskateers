using UnityEngine;

[ExecuteAlways] // makes it work in edit mode
public class Potion : MonoBehaviour
{
    public PotionData potionData;
    public Renderer[] targetRenderers;

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

}
