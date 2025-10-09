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

        // 🔸 Modern XR API
        var targetA = inputSocketA.GetOldestInteractableSelected()?.transform;
        var targetB = inputSocketB.GetOldestInteractableSelected()?.transform;

        if (targetA == null || targetB == null)
            return;

        Potion potionA = targetA.GetComponent<Potion>();
        Potion potionB = targetB.GetComponent<Potion>();

        if (potionA == null || potionB == null)
            return;

        PotionData resultData = potionA.potionData.GetMixResult(potionB.potionData);

        SpawnResultPotion(resultData);
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
            potion.ApplyMaterial(); // now public
        }

        hasSpawnedResult = true;
    }

    private void ClearResult()
    {
        hasSpawnedResult = false;

        foreach (Transform child in outputSpawnPoint)
        {
            DestroyImmediate(child.gameObject);
        }
    }
}
