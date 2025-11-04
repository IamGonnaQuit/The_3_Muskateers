using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class PotionReactor : MonoBehaviour
{
    [Header("Sockets")]
    public XRSocketInteractor inputSocketA;
    public XRSocketInteractor inputSocketB;

    [Tooltip("Where the resulting potion will appear.")]
    public Transform outputSpawnPoint;

    private bool hasSpawnedResult = false;

    private void OnEnable()
    {
        inputSocketA.selectEntered.AddListener(OnSocketChanged);
        inputSocketB.selectEntered.AddListener(OnSocketChanged);
        inputSocketA.selectExited.AddListener(OnSocketChanged);
        inputSocketB.selectExited.AddListener(OnSocketChanged);
    }

    private void OnDisable()
    {
        inputSocketA.selectEntered.RemoveListener(OnSocketChanged);
        inputSocketB.selectEntered.RemoveListener(OnSocketChanged);
        inputSocketA.selectExited.RemoveListener(OnSocketChanged);
        inputSocketB.selectExited.RemoveListener(OnSocketChanged);
    }

    private void OnSocketChanged(SelectEnterEventArgs args)
    {
        TryMixPotions();
    }

    private void OnSocketChanged(SelectExitEventArgs args)
    {
        ClearResult();
    }

    private void TryMixPotions()
    {
        if (hasSpawnedResult) return;

        var targetA = inputSocketA.GetOldestInteractableSelected()?.transform;
        var targetB = inputSocketB.GetOldestInteractableSelected()?.transform;

        if (targetA == null || targetB == null) return;

        // Only objects tagged as Potion
        if (!targetA.CompareTag("Potion") || !targetB.CompareTag("Potion")) return;

        Potion potionA = targetA.GetComponent<Potion>();
        Potion potionB = targetB.GetComponent<Potion>();

        if (potionA == null || potionB == null) return;
        if (potionA.potionData == null || potionB.potionData == null) return;

        // --- Check recipes ---
        PotionData resultData = potionA.potionData.GetMixResult(potionB.potionData);

        // If recipe is not found, check the reverse order (make mixing order-independent)
        if (resultData == potionA.potionData.defaultMixResult)
            resultData = potionB.potionData.GetMixResult(potionA.potionData);

        if (resultData == null || resultData == potionA.potionData.defaultMixResult)
        {
            Debug.Log("No valid recipe for this combination!");
            return;
        }

        // Spawn the resulting potion
        SpawnResultPotion(resultData);

        // Destroy / respawn input potions
        DestroyInputPotion(targetA.gameObject, inputSocketA);
        DestroyInputPotion(targetB.gameObject, inputSocketB);
    }

    private void SpawnResultPotion(PotionData resultData)
    {
        if (resultData.wholePotionPrefab == null || outputSpawnPoint == null) return;

        GameObject newPotion = Instantiate(resultData.wholePotionPrefab, outputSpawnPoint.position, outputSpawnPoint.rotation);

        // Assign PotionData to the spawned object
        Potion potion = newPotion.GetComponent<Potion>();
        if (potion != null)
        {
            potion.potionData = resultData;
            potion.ApplyMaterial();
        }

        hasSpawnedResult = true;
    }

    private void DestroyInputPotion(GameObject potionObject, XRSocketInteractor socket)
    {
        if (potionObject == null || socket == null) return;

        BasePotion basePotion = potionObject.GetComponent<BasePotion>();
        if (basePotion != null && basePotion.prefabReference != null)
        {
            BasePotionManager.Instance.RespawnBasePotion(
                basePotion.prefabReference,
                basePotion.originalPosition,
                basePotion.originalRotation
            );
        }

        // Release from socket if necessary
        if (socket.hasSelection)
        {
            var interactable = socket.GetOldestInteractableSelected();
            if (interactable != null)
            {
                socket.interactionManager.SelectExit(socket, interactable);
            }
        }

        Destroy(potionObject);
    }

    private void ClearResult()
    {
        hasSpawnedResult = false;

        if (outputSpawnPoint != null)
        {
            foreach (Transform child in outputSpawnPoint)
            {
                DestroyImmediate(child.gameObject);
            }
        }
    }
}
