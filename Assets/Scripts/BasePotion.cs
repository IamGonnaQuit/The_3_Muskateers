using UnityEngine;

public class BasePotion : MonoBehaviour
{
    [Header("Assign the prefab for respawning")]
    public GameObject prefabReference;

    [HideInInspector] public Vector3 originalPosition;
    [HideInInspector] public Quaternion originalRotation;

    private void Awake()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

}
