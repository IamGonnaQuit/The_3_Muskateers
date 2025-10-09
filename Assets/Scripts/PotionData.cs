using UnityEngine;

[CreateAssetMenu(fileName = "NewPotionData", menuName = "Potions/Potion Data")]
public class PotionData : ScriptableObject
{
    [Header("Identity")]
    public string potionName = "New Potion";
    public Color potionColor = Color.white;

    [Header("Visual")]
    public Material potionMaterial;

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
        if (other == null)
            return defaultMixResult;

        foreach (var recipe in mixRecipes)
        {
            if (recipe.otherPotion == other)
                return recipe.resultPotion;
        }

        return defaultMixResult;
    }
}
