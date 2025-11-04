using UnityEngine;

[CreateAssetMenu(fileName = "NewPotionData", menuName = "Potions/Potion Data")]
public class PotionData : ScriptableObject
{
    [Header("Identity")]
    public string potionName = "New Potion";
    public PotionTag[] potionTags;

    [Header("Description")]
    [TextArea(3, 6)]
    public string potionDescription;

    [Header("Liquid Visuals")]
    [Tooltip("Material used for the liquid of this potion.")]
    public Material potionMaterial;

    [Tooltip("Tint color for the liquid, applied when material is set.")]
    public Color potionColor = Color.white;

    [Header("Prefabs")]
    [Tooltip("Prefab for the whole/intact potion.")]
    public GameObject wholePotionPrefab;
    [Tooltip("Prefab for the broken potion.")]
    public GameObject brokenPotionPrefab;

    [Header("Mixing Recipes")]
    public Recipe[] mixRecipes;
    public PotionData defaultMixResult;

    public GameObject floorSplashPrefab;


    [System.Serializable]
    public struct Recipe
    {
        public PotionData otherPotion;
        public PotionData resultPotion;
    }

    /// <summary>
    /// Returns the result of mixing this potion with another.
    /// </summary>
    public PotionData GetMixResult(PotionData other)
    {
        if (other == null) return defaultMixResult;
        foreach (var recipe in mixRecipes)
            if (recipe.otherPotion == other)
                return recipe.resultPotion;
        return defaultMixResult;
    }

    /// <summary>
    /// Returns true if this potion has the specified tag.
    /// </summary>
    public bool HasTag(PotionTag tag)
    {
        if (potionTags == null) return false;
        foreach (var t in potionTags)
            if (t == tag) return true;
        return false;
    }
}
