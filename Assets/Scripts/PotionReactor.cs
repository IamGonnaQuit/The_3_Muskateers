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
        inputSocketA.selectEntered.AddListener(OnSocketEntered);
        inputSocketB.selectEntered.AddListener(OnSocketEntered);
        inputSocketA.selectExited.AddListener(OnSocketExited);
        inputSocketB.selectExited.AddListener(OnSocketExited);
    }

    private void OnDisable()
    {
        inputSocketA.selectEntered.RemoveListener(OnSocketEntered);
        inputSocketB.selectEntered.RemoveListener(OnSocketEntered);
        inputSocketA.selectExited.RemoveListener(OnSocketExited);
        inputSocketB.selectExited.RemoveListener(OnSocketExited);
    }

    private void OnSocketEntered(SelectEnterEventArgs args)
    {
        TryMixPotions();
    }

    private void OnSocketExited(SelectExitEventArgs args)
    {
        // Only clear result when BOTH sockets are empty
        if (!inputSocketA.hasSelection && !inputSocketB.hasSelection)
        {
            ClearResult();
        }
    }

    private void TryMixPotions()
    {
        if (hasSpawnedResult)
        {
            Debug.Log("Already has spawned result, waiting for cleanup");
            return;
        }

        var targetA = inputSocketA.GetOldestInteractableSelected()?.transform;
        var targetB = inputSocketB.GetOldestInteractableSelected()?.transform;

        Debug.Log($"Checking mix: Socket A has {targetA?.name}, Socket B has {targetB?.name}");

        if (targetA == null || targetB == null)
        {
            Debug.Log("One or both sockets are empty");
            return;
        }

        if (!targetA.CompareTag("Potion") || !targetB.CompareTag("Potion"))
        {
            Debug.Log("One or both objects are not potions");
            return;
        }

        Potion potionA = targetA.GetComponent<Potion>();
        Potion potionB = targetB.GetComponent<Potion>();

        if (potionA == null || potionB == null)
        {
            Debug.Log("One or both objects don't have Potion component");
            return;
        }
        if (potionA.potionData == null || potionB.potionData == null)
        {
            Debug.Log("One or both potions missing PotionData");
            return;
        }

        Debug.Log($"Attempting to mix: {potionA.potionData.potionName} + {potionB.potionData.potionName}");

        PotionData resultData = FindMixResult(potionA.potionData, potionB.potionData);

        if (resultData == null)
        {
            Debug.Log($"No valid recipe found for {potionA.potionData.potionName} + {potionB.potionData.potionName}");
            return;
        }

        Debug.Log($"Recipe found! Result: {resultData.potionName}");

        SpawnResultPotion(resultData);

        DestroyInputPotion(targetA.gameObject, inputSocketA);
        DestroyInputPotion(targetB.gameObject, inputSocketB);
    }

    private PotionData FindMixResult(PotionData dataA, PotionData dataB)
    {
        PotionData result = dataA.GetMixResult(dataB);

        if (result == null || result == dataA.defaultMixResult)
        {
            result = dataB.GetMixResult(dataA);
        }

        if (result == null || result == dataB.defaultMixResult)
        {
            return null;
        }

        return result;
    }

    private void SpawnResultPotion(PotionData resultData)
    {
        if (resultData.wholePotionPrefab == null)
        {
            Debug.LogError("Result potion prefab is null!");
            return;
        }

        if (outputSpawnPoint == null)
        {
            Debug.LogError("Output spawn point is null!");
            return;
        }

        GameObject newPotion = Instantiate(resultData.wholePotionPrefab, outputSpawnPoint.position, outputSpawnPoint.rotation);
        Debug.Log($"Spawned result potion: {resultData.potionName}");

        Potion potion = newPotion.GetComponent<Potion>();
        if (potion != null)
        {
            potion.potionData = resultData;
            // No ApplyMaterial needed – prefab handles visuals.
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
                Destroy(child.gameObject);
            }
        }
    }
}
