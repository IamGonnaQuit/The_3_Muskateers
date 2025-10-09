using UnityEngine;

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

        Instantiate(prefab, position, rotation);
    }
}
