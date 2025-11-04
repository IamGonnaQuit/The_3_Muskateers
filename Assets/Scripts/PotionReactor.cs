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
    [Tooltip("The prefab to spawn when the two input potions are mixed.")]
    public GameObject resultPotionPrefab;

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

        // Optional: check that both are potions
        if (targetA.GetComponent<Potion>() == null || targetB.GetComponent<Potion>() == null)
            return;

        // Spawn the result
        SpawnResultPotion();

        // Remove input potions
        DestroyInputPotion(targetA.gameObject, inputSocketA);
        DestroyInputPotion(targetB.gameObject, inputSocketB);
    }

    private void SpawnResultPotion()
    {
        if (resultPotionPrefab == null || outputSpawnPoint == null)
            return;

        Instantiate(resultPotionPrefab, outputSpawnPoint.position, outputSpawnPoint.rotation);
        hasSpawnedResult = true;
    }

    private void DestroyInputPotion(GameObject potionObject, XRSocketInteractor socket)
    {
        if (potionObject == null || socket == null)
            return;

        // Optional: if you have a BasePotion system to respawn base potions
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

        // Destroy any result potion in the output slot
        if (outputSpawnPoint != null)
        {
            foreach (Transform child in outputSpawnPoint)
            {
                DestroyImmediate(child.gameObject);
            }
        }
    }
}
