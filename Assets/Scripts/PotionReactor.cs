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

    [Header("Result Prefab")]
    [Tooltip("Generic potion prefab that has the Potion script.")]
    public GameObject potionPrefab;

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
        if (hasSpawnedResult)
            return;

        // Get the objects currently in the sockets
        var targetA = inputSocketA.GetOldestInteractableSelected()?.transform;
        var targetB = inputSocketB.GetOldestInteractableSelected()?.transform;

        if (targetA == null || targetB == null)
            return;

        Potion potionA = targetA.GetComponent<Potion>();
        Potion potionB = targetB.GetComponent<Potion>();

        if (potionA == null || potionB == null)
            return;

        // Get resulting potion data
        PotionData resultData = potionA.potionData.GetMixResult(potionB.potionData);

        // Spawn the result
        SpawnResultPotion(resultData);

        // Remove input potions
        DestroyInputPotion(targetA.gameObject, inputSocketA);
        DestroyInputPotion(targetB.gameObject, inputSocketB);
    }

    private void SpawnResultPotion(PotionData resultData)
    {
        if (potionPrefab == null || resultData == null)
            return;

        GameObject newPotion = Instantiate(potionPrefab, outputSpawnPoint.position, outputSpawnPoint.rotation);
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
        if (potionObject == null || socket == null)
            return;

        BasePotion basePotion = potionObject.GetComponent<BasePotion>();

        // If this is a base potion, respawn it before destruction
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

        // Destroy any result potion in the output slot
        foreach (Transform child in outputSpawnPoint)
        {
            DestroyImmediate(child.gameObject);
        }
    }
}
