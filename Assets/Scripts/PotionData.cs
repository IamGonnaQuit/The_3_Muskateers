using UnityEngine;

[CreateAssetMenu(fileName = "NewPotionData", menuName = "Potions/Potion Data")]
public class PotionData : ScriptableObject
{
    [Header("Identity")]
    public string potionName = "New Potion";
    public PotionTag[] potionTags;

    [Header("Info")]
    [TextArea(3, 6)]
    public string potionDescription;

    [Header("Visual")]
    public Material potionMaterial;
    public Color potionColor = Color.white;

    [Header("Prefabs")]
    public GameObject wholePotionPrefab;
    public GameObject brokenPotionPrefab;

    [Header("Mixing Recipes")]
    public Recipe[] mixRecipes;
    public PotionData defaultMixResult;

    [System.Serializable]
    public struct Recipe
    {
        public PotionData otherPotion;
        public PotionData resultPotion;
    }

    public PotionData GetMixResult(PotionData other)
    {
        if (other == null) return defaultMixResult;
        foreach (var recipe in mixRecipes)
            if (recipe.otherPotion == other)
                return recipe.resultPotion;
        return defaultMixResult;
    }

    public bool HasTag(PotionTag tag)
    {
        if (potionTags == null) return false;
        foreach (var t in potionTags)
            if (t == tag) return true;
        return false;
    }
}
