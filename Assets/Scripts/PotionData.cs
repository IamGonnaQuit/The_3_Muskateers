using UnityEngine;


[CreateAssetMenu(fileName = "NewPotionData", menuName = "Potions/Potion Data")]
public class PotionData : ScriptableObject
{
    [Header("Identity")]
    public string potionName = "New Potion";
    public Color potionColor = Color.white;

    [Header("Which other potions this can interact with")]
    public PotionData[] compatibleWith;

    // Small helper to check compatibility in code
    public bool IsCompatibleWith(PotionData other)
    {
        if (other == null || compatibleWith == null) return false;
        for (int i = 0; i < compatibleWith.Length; i++)
            if (compatibleWith[i] == other) return true;
        return false;
    }
}