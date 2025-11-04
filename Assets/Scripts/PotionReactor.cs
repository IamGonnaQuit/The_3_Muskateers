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

    private void OnSocketChanged(SelectEnterEventArgs args) => TryMixPotions();
    private void OnSocketChanged(SelectExitEventArgs args) => ClearResult();

    private void TryMixPotions()
    {
        if (hasSpawnedResult) return;

        // Get the objects in the sockets
        var targetA = inputSocketA.GetOldestInteractableSelected()?.transform;
        var targetB = inputSocketB.GetOldestInteractableSelected()?.transform;
        if (targetA == null || targetB == null) return;

        // Get Potion components
        Potion potionA = targetA.GetComponent<Potion>();
        Potion potionB = targetB.GetComponent<Potion>();
        if (potionA == null || potionB == null) return;

        // Check if both have valid PotionData
        if (potionA.potionData == null || potionB.potionData == null) return;

        // Get the resulting PotionData from the recipe
        PotionData resultData = potionA.potionData.GetMixResult(potionB.potionData);
        if (resultData == null || resultData.wholePotionPrefab == null) return;

        // Spawn the resulting potion prefab
        SpawnResultPotion(resultData.wholePotionPrefab);

        // Remove input potions
        DestroyInputPotion(targetA.gameObject, inputSocketA);
        DestroyInputPotion(targetB.gameObject, inputSocketB);
    }

    private void SpawnResultPotion(GameObject resultPrefab)
    {
        if (resultPrefab == null || outputSpawnPoint == null) return;

        Instantiate(resultPrefab, outputSpawnPoint.position, outputSpawnPoint.rotation);
        hasSpawnedResult = true;
    }

    private void DestroyInputPotion(GameObject potionObject, XRSocketInteractor socket)
    {
        if (potionObject == null || socket == null) return;

        // Respawn base potion if applicable
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
                socket.interactionManager.SelectExit(socket, interactable);
        }

        Destroy(potionObject);
    }

    private void ClearResult()
    {
        hasSpawnedResult = false;

        if (outputSpawnPoint != null)
        {
            foreach (Transform child in outputSpawnPoint)
                DestroyImmediate(child.gameObject);
        }
    }
}
