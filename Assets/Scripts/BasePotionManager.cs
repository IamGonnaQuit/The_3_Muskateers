using UnityEngine;

using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class BasePotionManager : MonoBehaviour
{
    public static BasePotionManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void RespawnBasePotion(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (prefab == null) return;

        // Instantiate the potion prefab
        GameObject newPotion = Instantiate(prefab, position, rotation);

        // --- Reset Rigidbody ---
        Rigidbody rb = newPotion.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // --- Reset XRGrabInteractable ---
        XRGrabInteractable grab = newPotion.GetComponent<XRGrabInteractable>();
        if (grab != null)
        {
            // Disable and re-enable to reset internal state
            grab.enabled = false;
            grab.enabled = true;
        }

        // --- Optional: reapply potion material/color ---
        Potion potionScript = newPotion.GetComponent<Potion>();
        if (potionScript != null)
        {
            potionScript.ApplyMaterial();
        }
    }
}
